/*
 * Created by SharpDevelop.
 * User: Ire
 * Date: 02/01/2006
 * Time: 23:06 
 */
 
using System;
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
using MathTextLibrary.Symbol;
using MathTextLibrary.Controllers;
using MathTextLibrary.Databases.Characteristic.Characteristics;


namespace MathTextRecognizerGUI
{
	/// <summary>
	/// Esta clase representa la ventana principal de la aplicacion de
	/// reconocimiento de formulas.
	/// </summary>
	public class MainWindow
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
		private Button btnNextC;
		
		[WidgetAttribute]
		private Button btnNextNode;
		
		[WidgetAttribute]
		private Button btnTilEnd;
		
		[WidgetAttribute]		
		private ToolButton toolLoad;
		
		[WidgetAttribute]		
		private ToolButton toolLatex;
		
		[WidgetAttribute]		
		private ToolButton toolOpen;	
		
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
		private ImageMenuItem menuOpenDatabase;
		
		[WidgetAttribute]
		private ImageMenuItem menuMakeOutput;
		
		[WidgetAttribute]
		private ImageMenuItem menuLoadImage;
		
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
		
		private MathTextRecognizerController controller;		
		
		private LogView logView;
		
		private const string title="Reconocedor de carácteres matemáticos - ";	
		
		private MTBNode currentNode;		
		
		private MathTextBitmap rootBitmap;
		
		#endregion Otros atributos
		
		public static void Main(string[] args)
		{
			new MainWindow();
		}
		
		/// <summary>
		/// El constructor de <code>MainClass</code>.
		/// </summary>
		public MainWindow()
		{
		
			Application.Init();
			Glade.XML gxml = new Glade.XML (null, "mathtextrecognizer.glade",
			                                 "mainWindow", null);
			gxml.Autoconnect (this);
			
			Initialize();
			
			OnOpenDatabaseClicked(this,EventArgs.Empty);
			
			Application.Run();
		}
		
		/// <summary>
		/// Para facilitar la inicializacion de los widgets.
		/// </summary>
		private void Initialize()
		{		
		
			controller=new MathTextRecognizerController();
			
			// Asignamos los eventos que indican que se han alcanzado hitos
			// en el reconocimiento de un cáracter.
			controller.LogMessageSent+=
			    new ControllerLogMessageSentEventHandler(OnMessageLog);
			    		
			controller.RecognizeProcessFinished+=
			    new ControllerProcessFinishedEventHandler(OnRecognizeProcessFinished);
			    
			controller.BitmapBeingRecognized+=
			    new ControllerBitmapBeingRecognizedEventHandler(OnBitmapBeingRecognized);
			    
			store=new NodeStore(typeof(MTBNode));
			
			// Creamos el NodeView, podría hacerse en el fichero de Glade,
			// aunque alguna razón habría por la que se hizo así.
			treeview=new NodeView(store);
			treeview.AppendColumn ("Imagen", new CellRendererText (), "text", 0);
			scrolledtree.Add(treeview);
			
			
			// Asignamos el evento para cuando se produzca la selección de un
			// nodo en el árbol.
			treeview.Selection.Changed += OnTreeviewSelectionChanged;
				
			mainWindow.Title = title + "Nueva base de datos";
			
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
			
			menuLoadImage.Image = new Gtk.Image(ImageResources.ImageLoadIcon16);
			toolLoad.IconWidget = new Gtk.Image(ImageResources.ImageLoadIcon22);
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
			if(recognizementFinished && treeview.Selection.CountSelectedRows() > 0)
			{
				// Recuperamos el TreePath del nodo seleccionado.
				TreePath path = treeview.Selection.GetSelectedRows()[0];
				
				MTBNode node=
				    (MTBNode)(store.GetNode(path));
				
				imageAreaNode.Image=node.MathTextBitmap.Bitmap;
				imageAreaProcessed.Image=node.MathTextBitmap.ProcessedBitmap;
				
				// Esto por si acaso
				if(node.MathTextBitmap.Symbol.SymbolType==MathSymbolType.NotRecognized)	
				{			
					node.MathTextBitmap.Bitmap.Save(node.Text+".png","png");
				}
				
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
		    ControllerBitmapBeingRecognizedEventArgs arg)
		{
			Gtk.Application.Invoke(sender, arg, OnBitmapBeingRecognizedThreadSafe);		
		}
		
		private void OnBitmapBeingRecognizedThreadSafe(object sender, EventArgs a)
		{		
		    ControllerBitmapBeingRecognizedEventArgs arg = 
		        a as ControllerBitmapBeingRecognizedEventArgs;
		    
		    imageAreaNode.Image=arg.MathTextBitmap.Bitmap;
			imageAreaProcessed.Image=arg.MathTextBitmap.ProcessedBitmap;
			
			MarkImage(arg.MathTextBitmap);
			
			
			SelectProccesedNode();			
			
			ClearLog();
		}
		
		private void SelectProccesedNode()
		{
			if(currentNode == null)
			{
				// Elegimos en nodo raíz.
				currentNode = store.GetNode(new TreePath("0")) as MTBNode;
			}
			else if (currentNode.ChildCount > 0)
			{
				// Si tiene hijos nos vamos al primero de ellos.
				currentNode = currentNode[0] as MTBNode;
			}
			else if (currentNode.Parent.ChildCount == currentNode.Parent.IndexOf(currentNode)+1)
			{
				// Si es el último hijo de un padre, nos vamos al siguiente tio.
				currentNode = currentNode.Parent as MTBNode;
				int idx = currentNode.Parent.IndexOf(currentNode)+1;
				
				if(idx < currentNode.Parent.ChildCount)
					currentNode = currentNode.Parent[idx] as MTBNode;
								
			}
			else
			{
				// Si no es el último, simplemente nos vamos al hermano
				int idx = currentNode.Parent.IndexOf(currentNode)+1;
				currentNode = currentNode.Parent[idx] as MTBNode;
			}
			
			
			treeview.NodeSelection.UnselectAll();
			treeview.NodeSelection.SelectNode(currentNode);
			
			treeview.ScrollToCell(treeview.Selection.GetSelectedRows()[0],null,true,0,1);
			
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
			/*Graphics g=Graphics.FromImage(originalMarked);
			
			
						
			g.DrawRectangle(new Pen(Color.Red,15/zoom),
				new Rectangle(mtb.Position,
				new Size(mtb.Width-1,mtb.Height-1)));*/
				
			
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
			menuOpenDatabase.Sensitive=true;
			menuMakeOutput.Sensitive=true;
			
			toolLoad.Sensitive=true;
			toolOpen.Sensitive=true;
			
			imageAreaOriginal.Image = null;
			
			recognizementFinished=true;
		}		
		
		/// <summary>
		/// Maneja el evento del controlador que sirve para enviar un mensaje a 
		/// la interfaz.
		/// </summary>
		/// <param name="sender">El objeto que provoca el evento.</param>
		/// <param name="msg">El mensaje que deseamos mostrar.</param>
		private void OnMessageLog(object sender,MessageLogSentEventArgs a)
		{
		    // Llamamos a través de invoke para que funcione bien.			
			Application.Invoke(sender, a,OnMessageLogThreadSafe);
		}
		
		private void OnMessageLogThreadSafe(object sender, EventArgs a)
		{		   
		    Log(((MessageLogSentEventArgs)a).Message);
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
				"Reconocedor de carácteres matemáticos",
				"Este programa se encarga de el reconocimiento del las "
				+ "fórmulas contenidas en imágenes,y su posterior conversión.");
		}
		
		/// <summary>
		///	Manejo del evento provocado al hacer click en el boton "Abrir base de datos". 
		/// </summary>
		private void OnOpenDatabaseClicked(object sender, EventArgs arg)
		{		
			string filename;	
			
			if(DatabaseOpenDialog.Show(mainWindow, out filename)
				== ResponseType.Ok)
			{				
				controller.LoadDatabase(filename);
				mainWindow.Title=title + Path.GetFileName(filename);
				ClearLog();
				Log("Base de datos en «{0}» cargada correctamente!", filename);
			}
		}
		
		/// <summary>
		/// Manejo del evento provocado al pulsar en el boton de crear la salida de texto.
		/// </summary>
		private void OnLatexClicked(object sender, EventArgs arg)
		{
			OutputDialog outputDialog=new OutputDialog(rootBitmap);
			outputDialog.Run();
		}
		
		/// <summary>
		/// Manejo del evento provocado al hacer click en la opcion "Salir" del menu.
		/// </summary>
		private void OnExitClicked(object sender, EventArgs arg)
		{
			OnExit();
		}
		
		/// <summary>
		/// Manejo del evento provocado al hacer click en el boton "Cargar imagen".
		/// </summary>
		private void OnLoadClicked(object sender, EventArgs arg)
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
		private void OnNewDatabaseClicked(object sender, EventArgs arg)
		{			
			new MainWindow();
		}
		
		/// <summary>
		/// Metodo que se invocara para indicar al controlador que deseamos
		/// dar un nuevo paso de procesado.
		/// </summary>
		private void NextStep()
		{			
			
			toolOpen.Sensitive=false;
			toolLoad.Sensitive=false;
			menuOpenDatabase.Sensitive=false;
			menuLoadImage.Sensitive=false;
			controller.NextRecognizeStep();
			
			mainWindow.QueueDraw();
		}
			
		/// <summary>
		/// Método que maneja el evento provocado al hacerse click sobre 
		/// el boton "Siguente nodo".
		/// </summary>
		private void OnBtnNextNodeClicked(object sender, EventArgs arg)
		{		    
			expLog.Expanded = true;
			
			controller.StepMode =
				 MathTextRecognizerControllerStepMode.NodeByNode;
				
			NextStep();
		}
		
		/// <summary>
		/// Metodo que maneja el evento lanzado al hacerse click sobre el 
		/// boton "Siguiente caracteristica".
		/// </summary>
		private void OnBtnNextCaractClicked(object sender, EventArgs arg)
		{		    
			expLog.Expanded = true;
			
			controller.StepMode = 
			    MathTextRecognizerControllerStepMode.NodeByNodeWithCharacteristicCheck;		    
			
			NextStep();
		}
		
		/// <summary>
		/// Método que maneja el evento que se provoca al pulsar el botón
		/// "Hasta el final".
		/// </summary>
		private void OnBtnTilEndClicked(object sender, EventArgs arg)
		{
			alignNextButtons.Sensitive=false;
			controller.StepMode = 
				MathTextRecognizerControllerStepMode.UntilEnd;
			
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
				imageOriginal = new Pixbuf(filename);				
				imageAreaOriginal.Image=imageOriginal;				
				store.Clear();
				
				MathTextBitmap mtb = new MathTextBitmap(imageOriginal);			
				MTBNode node = 
					new MTBNode(
						Path.GetFileNameWithoutExtension(filename), 
						mtb, treeview);
				    
				store.AddNode(node);
				controller.StartImage=mtb;
				rootBitmap=mtb;
				
				currentNode = null;
				
				ClearLog();
				
				recognizementFinished=false;
				toolLatex.Sensitive=false;
				menuMakeOutput.Sensitive=false;
				
				Log("¡Archivo de imagen «"+filename+"» cargado correctamente!");
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
		
		
	}
}