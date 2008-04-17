// SegmentingAndMathingStepWidget.cs created with MonoDevelop
// User: luis at 20:16 14/04/2008

using System;
using System.IO;
using System.Threading;

using Gtk;
using Glade;
using Gdk;

using MathTextCustomWidgets.ImageArea;
using MathTextCustomWidgets.CommonDialogs;
using MathTextCustomWidgets;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Controllers;

using MathTextRecognizer.Controllers;

using MathTextRecognizer.Steps.Nodes;

namespace MathTextRecognizer.Steps
{
	
	/// <summary>
	/// Esta clase implementa el widget que se usa para mostrar el 
	/// proceso de segmentacion y reconocimiento.
	/// </summary>
	public class SegmentingAndMatchingStepWidget : RecognizingStepWidget
	{
#region Widgets
		[WidgetAttribute]
		private HPaned segmentingAndMatchingHPaned;
		
		[WidgetAttribute]
		private Frame frameOriginal;
		
		[WidgetAttribute]
		private Frame frameNodeActual;
		
		[WidgetAttribute]
		private Frame frameNodeProcessed;
		
		[WidgetAttribute]
		private Notebook processedImageNB;
		
		[WidgetAttribute]
		private Button btnNextStep;
		
		[WidgetAttribute]
		private Button btnNextNode;
		
		[WidgetAttribute]
		private ScrolledWindow scrolledtree;
		
		[WidgetAttribute]
		private Alignment alignNextButtons;
		
		[WidgetAttribute]
		private Button btnTilEnd;
		
		[WidgetAttribute]
		private Menu segmentedNodeMenu;
		
		[WidgetAttribute]
		private ImageMenuItem learnImageItem;
		
#endregion Widgets
		
#region Atributos
		private NodeStore store;
		private NodeView treeview;	
		
		private ImageArea imageAreaOriginal;
		
		private Pixbuf imageOriginal;
		
		private ImageArea imageAreaNode;
		
		private SegmentingAndSymbolMatchingController controller;	
		
		private float zoom;
		
		private SegmentedNode currentNode;		
		
		private MathTextBitmap rootBitmap;
		
		private Thread recognizingThread;
		
		private bool recognizementFinished;
		
		private ControllerStepMode stepMode;
		
		// Needed by the popup actions' handler methods
		private SegmentedNode selectedNode;
		
#endregion Atributos.
		
		public SegmentingAndMatchingStepWidget(MainRecognizerWindow window)
			: base(window)
		{
			Glade.XML gxml = new Glade.XML("mathtextrecognizer.glade",
			                               "segmentingAndMatchingHPaned");

			gxml.Autoconnect(this);
			
			this.Add(segmentingAndMatchingHPaned);
			
			// We load the contextual menu.
			gxml = new Glade.XML("mathtextrecognizer.glade", 
			               "segmentedNodeMenu");
			
			gxml.Autoconnect(this);
			
			
			controller = new SegmentingAndSymbolMatchingController();
			
			// Asignamos los eventos que indican que se han alcanzado hitos
			// en el reconocimiento de un cáracter.
			controller.MessageLogSent += new MessageLogSentHandler(OnMessageLog);
			    		
			controller.RecognizementProcessFinished +=
			    new ProcessFinishedHandler(OnRecognizeProcessFinished);
			    
			controller.BitmapBeingRecognized +=
			    new BitmapBeingRecognizedHandler(OnBitmapBeingRecognized);
			
			InitializeChildren();
		}

		
#region Metodos publicos
		/// <summary>
		/// Establece la imagen inicial para segmentar y reconocer sus
		/// caracteres. 
		/// </summary>
		/// <param name="image">
		/// La imagen inicial.
		/// </param>
		public void SetInitialImage(string  filename)
		{
			
			imageOriginal = new Pixbuf(filename);
			imageAreaOriginal.Image=imageOriginal;				
			store.Clear();
			
			// Generamos el MaxtTextBitmap inical, y lo añadimos como
			// un nodo al arbol.
			MathTextBitmap mtb = new MathTextBitmap(imageOriginal);			
			SegmentedNode node = 
				new SegmentedNode(System.IO.Path.GetFileNameWithoutExtension(filename),
				                  mtb,
				                  treeview);
			    
			store.AddNode(node);
			controller.StartNode = node;
			rootBitmap=mtb;
			
			currentNode = null;
			
			Log("¡Archivo de imagen «{0}» cargado correctamente!", filename);

			
			
			alignNextButtons.Sensitive=true;
		}
#endregion Metodos publicos
		
#region Metodos privados
		
		protected void InitializeChildren()
		{
			store = new NodeStore(typeof(SegmentedNode));
			
			// Creamos el NodeView, podría hacerse en el fichero de Glade,
			// aunque alguna razón habría por la que se hizo así.
			treeview=new NodeView(store);
			treeview.RulesHint = true;
			
			treeview.ShowExpanders = true;
			treeview.AppendColumn ("Imagen", new CellRendererText (), "text", 0);
			treeview.AppendColumn ("Etiqueta", new CellRendererText (), "text", 1);
			treeview.AppendColumn ("Posición", new CellRendererText (), "text", 2);
			scrolledtree.Add(treeview);
			
			// Asignamos el evento para cuando se produzca la selección de un
			// nodo en el árbol.
			treeview.NodeSelection.Changed += OnTreeviewSelectionChanged;
						
			treeview.ButtonPressEvent += 
				new ButtonPressEventHandler(OnTreeViewButtonPress);
			
			imageAreaOriginal = new ImageArea();
			imageAreaOriginal.ImageMode = ImageAreaMode.Zoom;
			imageAreaOriginal.ZoomChanged += OnImageAreaOriginalZoomChanged;
			
			frameOriginal.Add(imageAreaOriginal);
			
			
			imageAreaNode=new ImageArea();
			imageAreaNode.ImageMode=ImageAreaMode.Zoom;			
			frameNodeActual.Add(imageAreaNode);
			
			learnImageItem.Image = ImageResources.LoadImage("database16");
		}
		
		/// <summary>
		/// Metodo que se encarga de dibujar un recuadro sobre la imagen original
		/// para señalar la zona que estamos tratando.
		/// </summary>
		/// <param name="mtb">La subimagen que estamos tratando.</param>
		private void MarkImage(MathTextBitmap mtb)
		{
			// TODO MarkImage Gdk style!
			Pixbuf originalMarked= imageOriginal.Copy();			
			
			imageAreaOriginal.Image=originalMarked;
		}
		
		/// <summary>
		/// Metodo que se invocara para indicar al controlador que deseamos
		/// dar un nuevo paso de procesado.
		/// </summary>
		private void NextStep()
		{			
			
			if(recognizingThread == null 
			   || recognizingThread.ThreadState == ThreadState.Stopped)
			{
				recognizingThread =
					new Thread(new ThreadStart(controller.RecognizeProcess));
				recognizingThread.Start();
				
				controller.Databases = Databases;
			}
		}
			
		
		/// <summary>
		/// Manejo del evento que provocado por el controlador cuando comienza 
		/// a tratar una nueva imagen.
		/// </summary>
		/// <param name="sender">El objeto que provoca el evento.</param>
		/// <param name="arg">El argumento del evento.</param>
		private void OnBitmapBeingRecognized(object sender, 
		                                     BitmapBeingRecognizedArgs arg)
		{
			Gtk.Application.Invoke(sender, arg, OnBitmapBeingRecognizedThreadSafe);		
		}
		
		private void OnBitmapBeingRecognizedThreadSafe(object sender, EventArgs a)
		{		
			if(treeview.NodeSelection.SelectedNodes.Length>0)
			{
				// Si hay un simbolo seleccionado, 
				// nos traemos sus imagenes procesadas.
				
				SegmentedNode node = 
					(SegmentedNode)(treeview.NodeSelection.SelectedNode);
			
				
				ImageArea imageAreaProcessed = new ImageArea();
				imageAreaProcessed.Image=
					node.MathTextBitmap.LastProcessedImage.CreatePixbuf();
				imageAreaProcessed.ImageMode=ImageAreaMode.Zoom;
				
			
				processedImageNB.AppendPage(imageAreaProcessed,
				                            new Label(String.Format("BD {0}",
				                                                    processedImageNB.NPages+1)));
				
				processedImageNB.Page=processedImageNB.NPages-1;
				
				// Solo mostramos los tabs si hay mas de una imagen procesada
				processedImageNB.ShowTabs = 
					node.MathTextBitmap.ProcessedImages.Count>1;
			}
		}
		
		
		
		/// <summary>
		/// Método que maneja el evento provocado al hacerse click sobre 
		/// el boton "Siguente nodo".
		/// </summary>
		private void OnBtnNextNodeClicked(object sender, EventArgs arg)
		{		    
			LogAreaExpanded = true;
			
			stepMode = ControllerStepMode.NodeByNode;
				
			NextStep();
		}
		
		/// <summary>
		/// Metodo que maneja el evento lanzado al hacerse click sobre el 
		/// boton "Siguiente caracteristica".
		/// </summary>
		private void OnBtnNextStepClicked(object sender, EventArgs arg)
		{		    
			LogAreaExpanded = true;
			
			stepMode = ControllerStepMode.StepByStep;		    
			
			NextStep();
		}
		
			/// <summary>
		/// Método que maneja el evento que se provoca al pulsar el botón
		/// "Hasta el final".
		/// </summary>
		private void OnBtnTilEndClicked(object sender, EventArgs arg)
		{
			alignNextButtons.Sensitive=false;
			stepMode = ControllerStepMode.UntilEnd;
			
			NextStep();
		}
		
		
		/// <summary>
		/// Manejo del evento provocado cuando se hace click en un nodo de la 
		/// vista de árbol.
		/// </summary>
		/// <param name="sender">El objeto que provoco el evento.</param>
		/// <param name="arg">Los argumentos del evento.</param>
		private void OnTreeviewSelectionChanged(object sender, EventArgs arg)
		{
		    // Si hemos acabado el proceso y hemos seleccionado algo.
			if(treeview.Selection.CountSelectedRows() > 0)
			{
				SegmentedNode node=
					(SegmentedNode)(treeview.NodeSelection.SelectedNode);
				
				imageAreaNode.Image=node.MathTextBitmap.Pixbuf;

				// Vaciamos el notebook
				while(processedImageNB.NPages > 0)
					processedImageNB.RemovePage(0);
				
				if(recognizementFinished)
				{
					// Añadimos las imagenes procesasdas al notebook

					// Solo mostramos los tabs si hay mas de una imagen procesada
					processedImageNB.ShowTabs = 
						node.MathTextBitmap.ProcessedImages.Count>1;
					
					// Si hemos terminado podemos hacer esto sin peligro.
					foreach(Pixbuf p in node.MathTextBitmap.ProcessedPixbufs)
					{
						ImageArea imageAreaProcessed = new ImageArea();
						imageAreaProcessed.Image=p;
						imageAreaProcessed.ImageMode=ImageAreaMode.Zoom;
						
					
						processedImageNB.AppendPage(imageAreaProcessed,
						                            new Label(String.Format("BD {0}",
						                                                    processedImageNB.NPages+1)));
					}
				}
				
				MarkImage(node.MathTextBitmap);				
			}
		}
		
		/// <summary>
		/// Maneja el evento del controlador que sirve para enviar un mensaje a 
		/// la interfaz.
		/// </summary>
		/// <param name="sender">El objeto que provoca el evento.</param>
		/// <param name="msg">El mensaje que deseamos mostrar.</param>
		private void OnMessageLog(object sender,MessageLogSentArgs a)
		{
		    // Llamamos a través de invoke para que funcione bien.			
			Application.Invoke(sender, a,OnMessageLogThreadSafe);
		}
		
		private void OnMessageLogThreadSafe(object sender, EventArgs a)
		{		   
		    Log(((MessageLogSentArgs)a).Message);
		}
		
		/// <summary>
		/// Manejo del evento provocado por el controlador cuando finaliza el
		/// proceso de reconocimiento.
		/// </summary>
		/// <param name="sender">El objeto que provoca el evento.</param>
		/// <param name="arg">Los argumentos del evento.</param>
		private void OnRecognizeProcessFinished(object sender, EventArgs arg)
		{			
		    // Llamamos a través de invoke para que funcione.
			Gtk.Application.Invoke(OnRecognizeProccessFinishedThreadSafe);			
		}
		
		private void OnRecognizeProccessFinishedThreadSafe(object sender, EventArgs a)
		{
		    Log("¡Reconocimiento terminado!");
			
			OkDialog.Show(
				Window,
				MessageType.Info,
			    "¡Proceso de reconocimiento terminado!\n"
			    + "Ahora puede revisar el resultado.");
			    
			 
						
			ResetState();			
			
		}
			
		/// <summary>
		/// Maneja el click de raton sobre el <c>TreeView</c>, mostrando el
		/// menu contextual si hicimos click con el boton derecho.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="ButtonPressEventArgs"/>
		/// </param>
		// Para que ocurra antes que se consuma el evento de seleccion
		[GLib.ConnectBeforeAttribute] 
		private void OnTreeViewButtonPress(object sender, 
		                                   ButtonPressEventArgs args)
		{
			// Nos quedamos unicamente con los clicks derechos
			if(recognizementFinished 
			   && args.Event.Button == 3)
			{
				TreePath path = new TreePath();
                // Obtenemos el treepath con las coordenadas del cursor.
                treeview.GetPathAtPos(System.Convert.ToInt16 (args.Event.X), 
				                      System.Convert.ToInt16 (args.Event.Y),				              
				                      out path);
                
				if( path != null)
				{
					// We try only if a node was found.			
					SegmentedNode node =  
						(SegmentedNode)(treeview.NodeStore.GetNode(path));	
					
					selectedNode = node;
					
					// The learning item is only shown when no label has
					// been found.
					learnImageItem.Visible =  String.IsNullOrEmpty(node.Label);
					
					segmentedNodeMenu.Popup();
                }
                        
			}
		}
		
		/// <summary>
		/// Handles the event lauched when the "edit label" option is clicked
		/// in the formula node's contextual menu.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnEditLabeItemActivate(object sender, EventArgs a)
		{
			Dialogs.SymbolLabelEditorDialog dialog = 
				new Dialogs.SymbolLabelEditorDialog(Window,
				                                    selectedNode);
			
			if(dialog.Show()== ResponseType.Ok)
			{
				bool changeLabel = true;
				if(selectedNode.ChildCount > 0)
				{
					ResponseType res = 
						ConfirmDialog.Show(Window,
						                   "Este nodo tiene hijos, y se estableces "
						                   + "una etiqueta se eliminarán, ¿quieres"
						                   +" continuar?");
					
					if(res == ResponseType.Yes)
					{
						// We remove the nodes.
						
						while(selectedNode.ChildCount > 0)
						{
							selectedNode.RemoveChild((TreeNode)(selectedNode[0]));
						}
					}
					else
					{
						changeLabel = false;
					}
				}
				
				if(changeLabel)
				{
					selectedNode.MathTextBitmap.Symbol = 
						new MathTextLibrary.Symbol.MathSymbol(dialog.Label);
				}
			}
			
			
			dialog.Destroy();
			
		}
		
		private void OnImageAreaOriginalZoomChanged(object sender, EventArgs a)
		{
			zoom = imageAreaOriginal.Zoom;			
		}
		
		/// <summary>
		/// Handles the click on the "Save image" formula node context menu item.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnSaveImageItemActivate(object sender, EventArgs args)
		{
			ResponseType res= 
						ConfirmDialog.Show(Window,
						                   "¿Deseas guardar la imagen del nodo «{0}»?",
						                   selectedNode.Name);
					
			if (res == ResponseType.Yes)
			{
				string filename="";
				res = ImageSaveDialog.Show(Window,out filename);
				
				if(res == ResponseType.Ok)
				{
					string extension = 
						System.IO.Path.GetExtension(filename).ToLower().Trim('.');
					selectedNode.MathTextBitmap.Pixbuf.Save(filename,extension);
				}
			}
		}
			
			
		/// <summary>
		/// Coloca los widgets al estado inicial para preparar la interfaz 
		/// para trabajar con una nueva imagen.
		/// </summary>
		public override void ResetState()
		{
			alignNextButtons.Sensitive=false;
			imageAreaNode.Image=null;
			
			
			// Vaciamos el notebook.
			while(processedImageNB.NPages > 0)
				processedImageNB.RemovePage(0);
			
			recognizementFinished = true;
			
			imageAreaOriginal.Image = null;
			
		}
		
#endregion Metodos privados
	}
}
