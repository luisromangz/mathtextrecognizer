// SegmentingAndMathingStepWidget.cs created with MonoDevelop
// User: luis at 20:16 14/04/2008

using System;
using System.IO;
using System.Threading;

using Gtk;
using Glade;
using Gdk;

using CustomGtkWidgets.ImageArea;
using CustomGtkWidgets.CommonDialogs;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Controllers;

using MathTextRecognizer.Controllers;

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
#endregion Widgets
		
#region Atributos
		private NodeStore store;
		private NodeView treeview;	
		
		private ImageArea imageAreaOriginal;
		
		private Pixbuf imageOriginal;
		
		private ImageArea imageAreaNode;
		
		private SegmentingAndSymbolMatchingController controller;	
		
		private MainRecognizerWindow window;
		
		private float zoom;
		
		private FormulaNode currentNode;		
		
		private MathTextBitmap rootBitmap;
		
		private Thread recognizingThread;
		
		private bool recognizementFinished;
		
		private ControllerStepMode stepMode;
		
#endregion Atributos.
		
		public SegmentingAndMatchingStepWidget(MainRecognizerWindow window)
		{
			Glade.XML gxml = new XML("mathtextrecognizer.glade", 
			                         "segmentingAndMatchingHPaned");
			
			gxml.Autoconnect(this);
			
			this.window = window;
			
			
			controller = new SegmentingAndSymbolMatchingController();
			
			// Asignamos los eventos que indican que se han alcanzado hitos
			// en el reconocimiento de un cáracter.
			controller.MessageLogSent += new MessageLogSentHandler(OnMessageLog);
			    		
			controller.RecognizeProcessFinished +=
			    new ProcessFinishedHandler(OnRecognizeProcessFinished);
			    
			controller.BitmapBeingRecognized +=
			    new BitmapBeingRecognizedHandler(OnBitmapBeingRecognized);
			
			InitializeWidgets();
		}
#region Propiedades

		/// <value>
		/// Contiene el widget que muestra el procesado relativo a la segmentacion
		/// y reconocimiento de caracteres.
		/// </value>
		public override Widget Widget 
		{
			get 
			{
				return this.segmentingAndMatchingHPaned;
			}
		}

	
		
#endregion Propiedades
		
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
			FormulaNode node = 
				new FormulaNode(Path.GetFileNameWithoutExtension(filename),
				            mtb,
			                treeview);
			    
			store.AddNode(node);
			controller.StartNode = node;
			rootBitmap=mtb;
			
			currentNode = null;
			
			window.Log("¡Archivo de imagen «{0}» cargado correctamente!",
			           filename);

			
			
			alignNextButtons.Sensitive=true;
		}
#endregion Metodos publicos
		
#region Metodos privados
		
		private void InitializeWidgets()
		{
			store = new NodeStore(typeof(FormulaNode));
			
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
			treeview.RowActivated += OnTreeviewRowActivated;
			
			imageAreaOriginal = new ImageArea();
			imageAreaOriginal.ImageMode = ImageAreaMode.Zoom;
			imageAreaOriginal.ZoomChanged += OnImageAreaOriginalZoomChanged;
			
			frameOriginal.Add(imageAreaOriginal);
			
			
			imageAreaNode=new ImageArea();
			imageAreaNode.ImageMode=ImageAreaMode.Zoom;			
			frameNodeActual.Add(imageAreaNode);
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
				
				controller.Databases = window.DatabaseManager.Databases;
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
				
				FormulaNode node = 
					(FormulaNode)(treeview.NodeSelection.SelectedNode);
			
				
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
			window.LogAreaExpanded = true;
			
			stepMode = ControllerStepMode.NodeByNode;
				
			NextStep();
		}
		
		/// <summary>
		/// Metodo que maneja el evento lanzado al hacerse click sobre el 
		/// boton "Siguiente caracteristica".
		/// </summary>
		private void OnBtnNextStepClicked(object sender, EventArgs arg)
		{		    
			window.LogAreaExpanded = true;
			
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
				FormulaNode node=
					(FormulaNode)(treeview.NodeSelection.SelectedNode);
				
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
		    window.Log(((MessageLogSentArgs)a).Message);
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
		    window.Log("¡Reconocimiento terminado!");
			
			OkDialog.Show(
				window.MainWindow,
				MessageType.Info,
			    "¡Proceso de reconocimiento terminado!\n"
			    + "Ahora puede revisar el resultado.");
			    
			 
						
			ResetState();			
			
		}
		
		/// <summary>
		/// Manejamos el evento producido al activar una fila.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="RowActivatedArgs"/>
		/// </param>
		private void OnTreeviewRowActivated(object sender,
		                                    RowActivatedArgs args)
		{	
			if(recognizementFinished)
			{
				FormulaNode activatedNode= 
						(FormulaNode) (treeview.NodeStore.GetNode(args.Path));
				
				FormulaNodeActionDialog actionDialog = 
					new FormulaNodeActionDialog(window.MainWindow, 
					                           String.IsNullOrEmpty(activatedNode.Label));
				
				if(actionDialog.Show() == ResponseType.Ok)
				{
					
					
					
					ResponseType res= 
						ConfirmDialog.Show(window.MainWindow,
						                   "¿Deseas guardar la imagen del nodo «{0}»?",
						                   activatedNode.Name);
					
					if (res == ResponseType.Yes)
					{
						string filename="";
						res = ImageSaveDialog.Show(window.MainWindow,out filename);
						
						if(res == ResponseType.Ok)
						{
							string extension = 
								Path.GetExtension(filename).ToLower().Trim('.');
							activatedNode.MathTextBitmap.Pixbuf.Save(filename,extension);
						}
					}
				}
				
				actionDialog.Destroy();
			}
		}
		
		private void OnImageAreaOriginalZoomChanged(object sender, EventArgs a)
		{
			zoom = imageAreaOriginal.Zoom;			
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
