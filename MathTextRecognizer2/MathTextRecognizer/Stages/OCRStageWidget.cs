// SegmentingAndMathingStepWidget.cs created with MonoDevelop
// User: luis at 20:16 14/04/2008

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Gtk;
using Glade;
using Gdk;

using MathTextCustomWidgets.Widgets.ImageArea;
using MathTextCustomWidgets.Dialogs;
using MathTextCustomWidgets;

using MathTextLibrary.Databases;
using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;
using MathTextLibrary.Controllers;

using MathTextRecognizer.Controllers;
using MathTextRecognizer.DatabaseManager;
using MathTextRecognizer.Controllers.Nodes;
using MathTextRecognizer.Stages.Dialogs;

using MathTextLearner;

namespace MathTextRecognizer.Stages
{
	
	/// <summary>
	/// Esta clase implementa el widget que se usa para mostrar el 
	/// proceso de segmentacion y reconocimiento.
	/// </summary>
	public class OCRStageWidget : RecognizingStageWidget
	{
#region Widgets
		[WidgetAttribute]
		private HPaned segmentingAndMatchingHPaned = null;
		
		[WidgetAttribute]
		private Frame frameOriginal = null;
		
		[WidgetAttribute]
		private Frame frameNodeActual = null;
		
		
		[WidgetAttribute]
		private Notebook processedImageNB = null;
		
		[WidgetAttribute]
		private ScrolledWindow scrolledtree = null;
		
		[WidgetAttribute]
		private Alignment alignNextButtons = null;
			
		[WidgetAttribute]
		private Menu segmentedNodeMenu= null;
		
		[WidgetAttribute]
		private ImageMenuItem learnImageItem = null;
		
		[WidgetAttribute]
		private MenuItem forceSegmentItem = null;
		
		[WidgetAttribute]
		private Notebook buttonsNB = null;
		
		[WidgetAttribute]
		private Button segmentBtn = null;
		
		[WidgetAttribute]
		private Button gotoTokenizerBtn = null;
		
#endregion Widgets
		
#region Attributes
		
		private NodeStore store;
		private NodeView treeview;	
		
		private ImageArea imageAreaOriginal;
		
		private Pixbuf imageOriginal;
		
		private ImageArea imageAreaNode;
		
		private OCRController controller;	
			
		private bool recognizementFinished;
		
		// Needed by the popup actions' handler methods
		private SegmentedNode selectedNode;
		
		private SegmentedNode rootNode;		
		
#endregion Attributes
		
		public OCRStageWidget(MainRecognizerWindow window)
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
			
			
			controller = new OCRController();
			
			// Asignamos los eventos que indican que se han alcanzado hitos
			// en el reconocimiento de un cáracter.
			controller.MessageLogSent += 
				new MessageLogSentHandler(OnControllerMessageLogSent);
			
			controller.NodeBeingProcessed += 
				new EventHandler(OnNodeBeingProcessed);
			    		
			controller.ProcessFinished +=
			    new ProcessFinishedHandler(OnRecognizeProcessFinished);
			    
			controller.BitmapProcessedByDatabase +=
			    new BitmapProcessedHandler(OnBitmapProcessedByDatabase);
			
			InitializeWidgets();
		}
		
		/// <summary>
		/// <c>OCRStageWidget</c>'s static field initializer.
		/// </summary>
		static OCRStageWidget()
		{
			widgetLabel = 
				"Segmentación de la imagen y reconocimiento de caracteres";
		}
		
#region Properties
		
		/// <value>
		/// Contains the leaf nodes, which have no children so they contain
		/// matched symbols.
		/// </value>
		public List<SegmentedNode> LeafNodes
		{
			get
			{
				return GetLeafs(rootNode);
			}
		}
		
#endregion Properties
		
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
			
			rootNode = node;
			
			controller.StartNode = node;

			Log("¡Archivo de imagen «{0}» cargado correctamente!", filename);
			
			alignNextButtons.Sensitive=true;
			segmentBtn.Sensitive = true;
			gotoTokenizerBtn.Sensitive = false;
			buttonsNB.Page = 0;
		}
#endregion Metodos publicos
		
#region Metodos privados
		
		protected void InitializeWidgets()
		{
			store = new NodeStore(typeof(SegmentedNode));
			
			// Creamos el NodeView, podría hacerse en el fichero de Glade,
			// aunque alguna razón habría por la que se hizo así.
			treeview=new NodeView(store);
			treeview.RulesHint = true;
			
			treeview.ShowExpanders = true;
			treeview.AppendColumn ("Imagen", 
			                       new CellRendererText (), 
			                       "text", 0);
			
			CellRendererText cellRenderer = new CellRendererText();
			cellRenderer.Xalign = 0.5f;
			treeview.AppendColumn ("Etiqueta", cellRenderer, "text", 1);
			treeview.AppendColumn ("Posición", 
			                       new CellRendererText (), 
			                       "text", 2);
			
			scrolledtree.Add(treeview);
			
			foreach (TreeViewColumn col in treeview) 
			{
				col.Sizing = TreeViewColumnSizing.Autosize;
			}
			
			
			
			// Asignamos el evento para cuando se produzca la selección de un
			// nodo en el árbol.
			treeview.NodeSelection.Changed += OnTreeviewSelectionChanged;
						
			treeview.ButtonPressEvent += 
				new ButtonPressEventHandler(OnTreeViewButtonPress);
			
			imageAreaOriginal = new ImageArea();
			imageAreaOriginal.ImageMode = ImageAreaMode.Zoom;
				
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
			
			Pixbuf originalMarked= imageOriginal.Copy();	
			
			// We tint the copy in red.
			originalMarked = 
				originalMarked.CompositeColorSimple(originalMarked.Width,
				                                    originalMarked.Height,
				                                    InterpType.Bilinear,
				                                    100,1,
				                                    0xAAAAAA,0xAAAAAA);
				
			// Over the red tinted copy, we place the piece we want to be
			// normal.
			imageOriginal.CopyArea(mtb.Position.X,
			                       mtb.Position.Y,
			                       mtb.Width,
			                       mtb.Height,
			                       originalMarked,
			                       mtb.Position.X,
			                       mtb.Position.Y);
		
			
			imageAreaOriginal.Image=originalMarked;
			
		}
		
		/// <summary>
		/// Metodo que se invocara para indicar al controlador que deseamos
		/// dar un nuevo paso de procesado.
		/// </summary>
		private void NextStep(ControllerStepMode stepMode)
		{			
			controller.Next(stepMode);
		}
			
		
		/// <summary>
		/// Manejo del evento que provocado por el controlador cuando comienza 
		/// a tratar una nueva imagen.
		/// </summary>
		/// <param name="sender">El objeto que provoca el evento.</param>
		/// <param name="arg">El argumento del evento.</param>
		private void OnBitmapProcessedByDatabase(object sender, 
		                                         BitmapProcessedArgs arg)
		{
			Gtk.Application.Invoke(sender, arg, OnBitmapProcessedByDatabaseInThread);
		}
		
		private void OnBitmapProcessedByDatabaseInThread(object sender, EventArgs a)
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
			alignNextButtons.Sensitive=false;
			LogAreaExpanded = false;				
			NextStep(ControllerStepMode.NodeByNode);
		}
		
		/// <summary>
		/// Metodo que maneja el evento lanzado al hacerse click sobre el 
		/// boton "Siguiente caracteristica".
		/// </summary>
		private void OnBtnNextStepClicked(object sender, EventArgs arg)
		{					
			LogAreaExpanded = true;			
			NextStep(ControllerStepMode.StepByStep);
		}
		
		/// <summary>
		/// Método que maneja el evento que se provoca al pulsar el botón
		/// "Hasta el final".
		/// </summary>
		private void OnBtnTilEndClicked(object sender, EventArgs arg)
		{
			
			LogAreaExpanded=false;
			alignNextButtons.Sensitive=false;
			NextStep(ControllerStepMode.UntilEnd);
		}
		
		/// <summary>
		/// Handles the click on the "go to tokenizer" button.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnGoToTokenizerBtnClicked(object sender, EventArgs arg)
		{
			
			// We have to check the leaf nodes for problems.
			
			SegmentedNode analizedNode = rootNode;
			List<string> errors = CheckNode(analizedNode);
			
			if(errors.Count == 0)
			{
				TokenizingStageWidget tokenizingWidget =  
					NextStage() as TokenizingStageWidget;
				
				tokenizingWidget.SetStartSymbols(this.LeafNodes);				
				
			}			
			else
			{
				string errorss = String.Join("\n", errors.ToArray());
				OkDialog.Show(this.MainWindow.Window,
				              MessageType.Info,
				              "Para continuar a la siguente fase de procesado,"
				              +"debes solucionar los siguentes problemas:\n\n{0}",
				              errorss);
			}
		}
		
		/// <summary>
		/// Checks a node to see if its a leaf node, and if so it has some
		/// label related issues.
		/// </summary>
		/// <param name="analizedNode">
		/// The node to be analized
		/// </param>
		/// <returns>
		/// A list with the problems found.
		/// </returns>
		private List<string> CheckNode(SegmentedNode analizedNode)
		{
			List<string> errors = new List<string>();
			if(analizedNode.ChildCount>0)
			{
				for(int i=0; i<analizedNode.ChildCount; i++)
				{
					SegmentedNode node = (SegmentedNode)(analizedNode[i]);
					errors.AddRange(CheckNode(node));
				}
				
			}
			else
			{
				// We have a problem if we have many symbols in a node, or
				// we don't have any.
				
				if(analizedNode.Symbols.Count == 0)
				{
					errors.Add(String.Format("· El nodo «{0}» no tiene etiqueta.",
					                         analizedNode.Name));
				}
				else if(analizedNode.Symbols.Count > 1)
				{
					errors.Add(String.Format("· El nodo «{0}» tiene varias etiquetas.",
					                         analizedNode.Name));					
				}
			}
			
			return errors;
		}
		
		/// <summary>
		/// Handles the event produced when a new node is going to be processed
		/// by the controller.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnNodeBeingProcessed(object sender, EventArgs args)
		{
			Application.Invoke(OnNodeBeingProcessedInThread);
		}
		
		private void OnNodeBeingProcessedInThread(object sender, EventArgs args)
		{
			if(controller.StepMode != ControllerStepMode.UntilEnd)
				alignNextButtons.Sensitive=true;
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
		/// Handles the click on the "learn image" treeview's context menu item.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnLearnImageItemActivate(object sender, EventArgs args)
		{
			// We ask for confirmation
			ResponseType res = ConfirmDialog.Show(MainWindow.Window,
			                                    "¿Realmente quieres añadir el símbolo «{0}» a una base de datos?",
			                                    selectedNode.Name);
			
			if(res == ResponseType.Yes)
			{	
				// We let the user select the database to be modified.
				LearnSymbolDatabaseChooserDialog databaseDialog =
					new LearnSymbolDatabaseChooserDialog(MainWindow.Window,
					                                     MainWindow.DatabaseManager.DatabaseFilesInfo);
				
				res = databaseDialog.Show();
				databaseDialog.Destroy();
				
				if(res == ResponseType.Ok)
				{				
					DatabaseFileInfo selectedDatabase = 
						databaseDialog.ChoosenDatabase;
				
					MathTextDatabase database = null;
					string databasePath = "";
					if(selectedDatabase != null)
					{
						database = selectedDatabase.Database;
						databasePath = selectedDatabase.Path;
					}
					
					MainLearnerWindow learner =
						new MainLearnerWindow(this.MainWindow.Window,
						                      database,
						                      databasePath,
						                      selectedNode.MathTextBitmap.Pixbuf,
						                      selectedNode.Name);
				}
				
				
			}
				                                      
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
			Gtk.Application.Invoke(OnRecognizeProcessFinishedInThread);			
		}
		
		private void OnRecognizeProcessFinishedInThread(object sender,
		                                                EventArgs a)
		{
		    Log("¡Reconocimiento terminado!");
			
			OkDialog.Show(
				MainWindow.Window,
				MessageType.Info,
			    "¡Proceso de reconocimiento terminado!\n"
			    + "Ahora puede revisar el resultado.");
			    
			
						
			ResetState();
			
			gotoTokenizerBtn.Sensitive = true;
			
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
				new Dialogs.SymbolLabelEditorDialog(MainWindow.Window,
				                                    selectedNode);
			
			if(dialog.Show() == ResponseType.Ok)
			{
				bool changeLabel = true;
				if(selectedNode.ChildCount > 0)
				{
					ResponseType res = 
						ConfirmDialog.Show(MainWindow.Window,
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
					// We remove all the symbols, then add the new one.
					selectedNode.Symbols.Clear();					
					selectedNode.Symbols.Add(new MathSymbol(dialog.Label));
					selectedNode.SetLabels();
				}
			}
			
			dialog.Destroy();
			
		}
		
		/// <summary>
		/// Handles the selection of the "force segmenting" context menu item.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnForceSegmentItemClicked(object sender, EventArgs args)
		{			
			
			ResponseType res = 
				ConfirmDialog.Show(this.MainWindow.Window,
				                   "Si fuerza el segmentado perderás el "
				                   + "reconocimiento realizado, ¿quieres continuar?");
			
			if(res == ResponseType.Yes)
			{
				// We reload the databases because the new segmentation may be
				// due a database change.
				controller.Databases = this.Databases;
				
				controller.StartNode = selectedNode;
				controller.SearchDatabase = false;
				buttonsNB.Page = 1;
				alignNextButtons.Sensitive = true;
				controller.Next(ControllerStepMode.NodeByNode);
			}
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
						ConfirmDialog.Show(MainWindow.Window,
						                   "¿Deseas guardar la imagen del nodo «{0}»?",
						                   selectedNode.Name);
					
			if (res == ResponseType.Yes)
			{
				string filename="";
				res = ImageSaveDialog.Show(MainWindow.Window, out filename);
				
				if(res == ResponseType.Ok)
				{
					string extension = 
						System.IO.Path.GetExtension(filename).ToLower().Trim('.');
					selectedNode.MathTextBitmap.Pixbuf.Save(filename,extension);
				}
			}
		}
		
		/// <summary>
		/// Handles the click in the segment button.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnSegmentBtnClicked(object sender, EventArgs args)
		{
			controller.Databases = this.Databases;
			this.MainWindow.ProcessItemsSensitive=false;
			segmentBtn.Sensitive = false;
			gotoTokenizerBtn.Sensitive =  true;
			buttonsNB.Page = 1;
			controller.SearchDatabase = false;
			controller.Next(ControllerStepMode.NodeByNode);
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
			
			treeview.NodeSelection.UnselectAll();
			
			buttonsNB.Page = 0;
			
			gotoTokenizerBtn.Sensitive = false;
			
			this.MainWindow.ProcessItemsSensitive = true;
			
		}
		
		/// <summary>
		/// Retrieves the leaf nodes of a given node.
		/// </summary>
		/// <param name="node">
		/// The node to check;
		/// </param>
		/// <returns>
		/// A list with the leafs.
		/// </returns>
		private List<SegmentedNode> GetLeafs(SegmentedNode node)
		{
			List<SegmentedNode> leafs = new List<SegmentedNode>();
			
			if(node.ChildCount == 0)
			{
				leafs.Add(node);
			}
			else
			{
				for(int i=0; i< node.ChildCount; i++)
				{
					leafs.AddRange(GetLeafs((SegmentedNode)node[i]));
				}
			}
			
			return leafs;
		}
		
#endregion Metodos privados
	}
}
