/*
 * Created by SharpDevelop.
 * User: Ire
 * Date: 02/01/2006
 * Time: 23:06 
 */
 
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Gtk;
using Gdk;
using Glade;

using CustomGtkWidgets;
using CustomGtkWidgets.Logger;
using CustomGtkWidgets.ImageArea;
using CustomGtkWidgets.CommonDialogs;

using MathTextLibrary;
using MathTextLibrary.Bitmap;
using MathTextLibrary.BitmapSegmenters;
using MathTextLibrary.Symbol;
using MathTextLibrary.Controllers;
using MathTextLibrary.Databases.Characteristic.Characteristics;

using MathTextRecognizer.DatabaseManager;
using MathTextRecognizer.Controllers;

namespace MathTextRecognizer
{
	/// <summary>
	/// Esta clase representa la ventana principal de la aplicacion de
	/// reconocimiento de formulas.
	/// </summary>
	public class MainRecognizerWindow
	{
		#region Glade-Widgets
		[WidgetAttribute]
		private Gtk.Window mainWindow;
		
		[WidgetAttribute]
		private Frame frameOriginal;
		
		[WidgetAttribute]
		private Frame frameNodeActual;
		
		[WidgetAttribute]
		private Frame frameNodeProcessed;
		
		[WidgetAttribute]
		private Button btnNextStep;
		
		[WidgetAttribute]
		private Button btnNextNode;
		
		[WidgetAttribute]
		private Button btnTilEnd;
		
		[WidgetAttribute]		
		private ToolButton toolLoadImage;
		
		[WidgetAttribute]		
		private ToolButton toolLatex;
		
		[WidgetAttribute]		
		private ToolButton toolDatabase;	
		
		[WidgetAttribute]
		private ScrolledWindow scrolledtree;
		
		[WidgetAttribute]
		private Alignment alignNextButtons;
		
		[WidgetAttribute]
		private Expander expLog;
		
		[WidgetAttribute]
		private Toolbar toolbar;
		
		[WidgetAttribute]
		private ImageMenuItem menuNewSession;
		
		[WidgetAttribute]
		private ImageMenuItem menuOpenDatabaseManager;
		
		[WidgetAttribute]
		private ImageMenuItem menuMakeOutput;
		
		[WidgetAttribute]
		private ImageMenuItem menuLoadImage;
		
		[WidgetAttribute]
		private HBox messageInfoHB;
		
		[WidgetAttribute]
		private ToolButton toolNewSession;
		
		#endregion Glade-Widgets
		
		#region Otros atributos
		
		private float zoom;
		
		private bool recognizementFinished;
		
		private NodeStore store;
		private NodeView treeview;	
		
		private ImageArea imageAreaOriginal;
		
		private Pixbuf imageOriginal;
		
		private ImageArea imageAreaNode;
		private ImageArea imageAreaProcessed;
		
		private SegmentingAndSymbolMatchingController controller;		
		
		private LogView logView;
		
		private const string title="Reconocedor de caracteres matemáticos - ";	
		
		private FormulaNode currentNode;		
		
		private MathTextBitmap rootBitmap;
		
		private DatabaseManagerDialog databaseManagerDialog;
		
		private Thread recognizingThread;
		
		private ControllerStepMode stepMode;
		
		#endregion Otros atributos
		
		public static void Main(string[] args)
		{			
			Application.Init();
			new MainRecognizerWindow();
			
			
			
			Application.Run();
		}
		
		/// <summary>
		/// El constructor de <code>MainWindow</code>.
		/// </summary>
		public MainRecognizerWindow()
		{
			Glade.XML gxml = new Glade.XML (null, 
			                                "mathtextrecognizer.glade",
			                                "mainWindow",
			                                null);
			gxml.Autoconnect (this);			
			this.Initialize();			
			
			databaseManagerDialog = new DatabaseManagerDialog(this.mainWindow);
			databaseManagerDialog.DatabaseListChanged += 
				new EventHandler(OnDatabaseManagerDialogDatabaseListChanged);
			
			// Asignamos la configuracion inicial al dialogo de gestion de
			// bases de datos.			
			databaseManagerDialog.DatabaseFilesInfo = 
				Config.RecognizerConfig.Instance.DatabaseFilesInfo;
		}
		
		/// <summary>
		/// Para facilitar la inicializacion de los widgets.
		/// </summary>
		private void Initialize()
		{		
		
			controller = new SegmentingAndSymbolMatchingController();
			
			// Asignamos los eventos que indican que se han alcanzado hitos
			// en el reconocimiento de un cáracter.
			controller.MessageLogSent += new MessageLogSentHandler(OnMessageLog);
			    		
			controller.RecognizeProcessFinished +=
			    new ProcessFinishedHandler(OnRecognizeProcessFinished);
			    
			controller.BitmapBeingRecognized +=
			    new BitmapBeingRecognizedHandler(OnBitmapBeingRecognized);
			    
			store = new NodeStore(typeof(FormulaNode));
			
			// Creamos el NodeView, podría hacerse en el fichero de Glade,
			// aunque alguna razón habría por la que se hizo así.
			treeview=new NodeView(store);
			treeview.AppendColumn ("Imagen", new CellRendererText (), "text", 0);
			scrolledtree.Add(treeview);
			
			// Asignamos el evento para cuando se produzca la selección de un
			// nodo en el árbol.
			treeview.Selection.Changed += OnTreeviewSelectionChanged;
			treeview.RowActivated += OnTreeviewRowActivated;
			
			mainWindow.Title = title + "Sin imagen";
			
			imageAreaOriginal = new ImageArea();
			imageAreaOriginal.ImageMode = ImageAreaMode.Zoom;
			imageAreaOriginal.ZoomChanged += OnImageAreaOriginalZoomChanged;
			
			frameOriginal.Add(imageAreaOriginal);
			
			
			imageAreaNode=new ImageArea();
			imageAreaNode.ImageMode=ImageAreaMode.Zoom;			
			frameNodeActual.Add(imageAreaNode);
			
			
			imageAreaProcessed = new ImageArea();
			imageAreaProcessed.ImageMode = ImageAreaMode.Zoom;
			
			frameNodeProcessed.Add(imageAreaProcessed);
			
			// Ponemos iconos personalizados en los botones
			menuLoadImage.Image = ImageResources.LoadImage("insert-image16");
			toolLoadImage.IconWidget = ImageResources.LoadImage("insert-image22");
			
			menuOpenDatabaseManager.Image = ImageResources.LoadImage("database16");
			toolDatabase.IconWidget = ImageResources.LoadImage("database22");
			
			toolNewSession.IconWidget = ImageResources.LoadImage("window-new22");
			menuNewSession.Image = ImageResources.LoadImage("window-new16");
			
			// Creamos el cuadro de registro.
			logView = new LogView();
			expLog.Add(logView);		
			
			mainWindow.ShowAll();
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
			if(recognizementFinished
			   && treeview.Selection.CountSelectedRows() > 0)
			{
				// Recuperamos el TreePath del nodo seleccionado.
				TreePath path = treeview.Selection.GetSelectedRows()[0];
				
				FormulaNode node=
				    (FormulaNode)(store.GetNode(path));
				
				imageAreaNode.Image=node.MathTextBitmap.Bitmap;
				imageAreaProcessed.Image=node.MathTextBitmap.ProcessedBitmap;
				
				MarkImage(node.MathTextBitmap);
				
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
			ClearLog();
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
		    ClearLog();
			Log("¡Reconocimiento terminado!");
			
			OkDialog.Show(
				mainWindow,
				MessageType.Info,
			    "¡Proceso de reconocimiento terminado!\n"
			    + "Ahora puede revisar el resultado.");
			    
			    if(expLog.Expanded)
			    	expLog.Expanded = false;
						
			ResetState();			
			
		}
		
		/// <summary>
		/// Coloca los widgets al estado inicial para preparar la interfaz para trabajar
		/// con una nueva imagen.
		/// </summary>
		private void ResetState()
		{
			alignNextButtons.Sensitive=false;
			imageAreaNode.Image=null;
			imageAreaProcessed.Image=null;
			toolLatex.Sensitive=true;
			
			menuLoadImage.Sensitive=true;
			menuOpenDatabaseManager.Sensitive=true;
			menuMakeOutput.Sensitive=true;
			
			toolLoadImage.Sensitive=true;
			toolDatabase.Sensitive=true;
			
			imageAreaOriginal.Image = null;
			
			recognizementFinished=true;
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
		/// Metodo que maneja el evento provocado al cerrar la ventana.
		/// </summary>
		/// <param name="sender">El objeto que provoca el evento.</param>
		/// <param name="arg">Los argumentos del evento.</param>
		private void OnMainWindowDeleteEvent(object sender,DeleteEventArgs arg)
		{
			OnExit();
		}		
		
		/// <summary>
		/// Manejo del evento provocado al hacer clic sobre la opcion de menu
		/// "Acerca de".
		/// </summary>
		private void OnMenuAboutClicked(object sender, EventArgs arg)
		{
			AppInfoDialog.Show(
				mainWindow,
				"Reconocedor de caracteres matemáticos",
				"Este programa se encarga de el reconocimiento del las "
				+ "fórmulas contenidas en imágenes,y su posterior conversión.");
		}
		
		/// <summary>
		///	Manejo del evento provocado al hacer click en el boton 
		/// "Abrir base de datos". 
		/// </summary>
		private void OnOpenDatabaseManagerClicked(object sender, EventArgs arg)
		{	
			databaseManagerDialog.Run();
			
			
		}
		
		/// <summary>
		/// Manejo del evento provocado al pulsar en el boton de crear la salida
		/// de texto.
		/// </summary>
		private void OnLatexClicked(object sender, EventArgs arg)
		{
			Output.OutputDialog outputDialog = 
				new Output.OutputDialog(rootBitmap);
			outputDialog.Run();
		}
		
		/// <summary>
		/// Manejo del evento provocado al hacer click en la opcion "Salir"
		/// del menu.
		/// </summary>
		private void OnExitClicked(object sender, EventArgs arg)
		{
			OnExit();
		}
		
		/// <summary>
		/// Manejo del evento provocado al hacer click en el boton 
		/// "Cargar imagen".
		/// </summary>
		private void OnLoadImageClicked(object sender, EventArgs arg)
		{
			
			ResponseType res = ResponseType.Yes;
			
			if(recognizementFinished)
			{
				 res = ConfirmDialog.Show(
					mainWindow,
					"Si cargas una nueva imágen perderás el reconocimiento realizado.\n"+
					"¿Deseas continuar?");
			}
			
			if(res==ResponseType.Yes)
			{			
				LoadImage();
			}
		}
		
			
		/// <summary>
		/// Manejo del evento provocado al hacer click en el botón
		/// "Nueva sesion".
		/// </summary>
		private void OnNewSessionClicked(object sender, EventArgs arg)
		{			
			System.Diagnostics.Process newSession =  
				System.Diagnostics.Process.Start(System.Environment.CommandLine);			
		}
		
		/// <summary>
		/// Metodo que se invocara para indicar al controlador que deseamos
		/// dar un nuevo paso de procesado.
		/// </summary>
		private void NextStep()
		{			
			
			toolDatabase.Sensitive=false;
			toolLoadImage.Sensitive=false;
			menuOpenDatabaseManager.Sensitive=false;
			menuLoadImage.Sensitive=false;
			
			if(recognizingThread == null 
			   || recognizingThread.ThreadState == ThreadState.Stopped)
			{
				recognizingThread =
					new Thread(new ThreadStart(controller.RecognizeProcess));
				recognizingThread.Start();
				
				controller.Databases = databaseManagerDialog.Databases;
			}
			
			mainWindow.QueueDraw();
		}
			
		/// <summary>
		/// Método que maneja el evento provocado al hacerse click sobre 
		/// el boton "Siguente nodo".
		/// </summary>
		private void OnBtnNextNodeClicked(object sender, EventArgs arg)
		{		    
			expLog.Expanded = true;
			
			stepMode = ControllerStepMode.NodeByNode;
				
			NextStep();
		}
		
		/// <summary>
		/// Metodo que maneja el evento lanzado al hacerse click sobre el 
		/// boton "Siguiente caracteristica".
		/// </summary>
		private void OnBtnNextStepClicked(object sender, EventArgs arg)
		{		    
			expLog.Expanded = true;
			
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
		/// Método usado para escribir un mensaje en la zona de información de proceso.
		/// </summary>
		/// <param name="message">El mensaje a escribir.</param>
		private void Log(string message, params object[] args)
		{			
			logView.LogLine(message, args);
		}
		
		/// <summary>
		/// Método que permite borrar la zona de informacion de proceso.
		/// </summary>
		private void ClearLog()
		{			
			logView.ClearLog();			
		}
		
		/// <summary>
		/// Metodo que maneja el evento provocado al cerrarse el dialogo de 
		/// apertura de imagen.
		/// </summary>
		private void LoadImage()
		{
			string filename;
			
			if(ImageLoadDialog.Show(mainWindow, out filename)
				== ResponseType.Ok)
			{			
				// Cargamos la imagen desde disco
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
				
				ClearLog();
				
				recognizementFinished=false;
				toolLatex.Sensitive=false;
				menuMakeOutput.Sensitive=false;
				
				Log("¡Archivo de imagen «"+filename+"» cargado correctamente!");

				this.mainWindow.Title = 
					title + System.IO.Path.GetFileName(filename);
				
				alignNextButtons.Sensitive=true;
			}
		}
		
		private void OnImageAreaOriginalZoomChanged(object sender, EventArgs a)
		{
			zoom = imageAreaOriginal.Zoom;			
		}

		/// <summary>
		/// Metodo que se encarga de gestionar la salida de la aplicacion.
		/// </summary>
		private void OnExit()
		{
			
			imageAreaOriginal.Image=null;
			imageAreaNode.Image=null;
			imageAreaProcessed.Image=null;
			
			Application.Quit();			
		}	
		
		/// <summary>
		/// Maneja el evento producido al cambiar la lista de bases de datos usadas
		/// para reconocer. 
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnDatabaseManagerDialogDatabaseListChanged(object sender,
		                                                        EventArgs args)
		{
			messageInfoHB.Visible = 
				databaseManagerDialog.DatabaseFilesInfo.Count ==0;
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
			FormulaNode activatedNode= 
				(FormulaNode) (treeview.NodeStore.GetNode(args.Path));
			
			ResponseType res= 
				ConfirmDialog.Show(this.mainWindow,
				                   "¿Deseas guardar la imagen del nodo «{0}»?",
				                   activatedNode.Name);
			
			if (res== ResponseType.Yes)
			{
				string filename="";
				res = ImageSaveDialog.Show(this.mainWindow,out filename);
				
				if(res == ResponseType.Ok)
				{
					string extension = 
						Path.GetExtension(filename).ToLower().Trim('.');
					activatedNode.MathTextBitmap.Bitmap.Save(filename,extension);
				}
			}
			
		}
	}
}
