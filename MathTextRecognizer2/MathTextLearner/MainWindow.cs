/*
 * Created by SharpDevelop.
 * User: Ire
 * Date: 27/12/2005
 * Time: 11:03
 */
 
using System;
using System.IO;
using System.Threading;
using System.Drawing;

using Gtk;
using Gdk;
using Glade;

using CustomGtkWidgets;
using CustomGtkWidgets.Logger;
using CustomGtkWidgets.ImageArea;
using CustomGtkWidgets.CommonDialogs;

using MathTextLibrary;
using MathTextLibrary.Databases;
using MathTextLibrary.Databases.Caracteristic;

namespace MathTextLearner
{
	/// <summary>
	/// Esta clase representa la ventana principal de la aplicacion de
	/// aprendizaje de caracteres en la base de datos.
	/// </summary>
	public class MainWindow
	{
		#region Glade widgets
		//Aqui se declaran los controles de la interfaz.		
		[WidgetAttribute]
		private Gtk.Window mainWindow;
		
		[WidgetAttribute]
		private MenuItem menuDatabase;
		
		[WidgetAttribute]
		private MenuItem menuLoad;
		
		[WidgetAttribute]
		private MenuItem menuSaveAs;
		
		[WidgetAttribute]
		private ImageMenuItem menuLoadImage;
		
		[WidgetAttribute]
		private MenuItem menuOpen;
		
		[WidgetAttribute]
		private Toolbar toolbar;
		
		[WidgetAttribute]
		private Button btnLearn;
		
		[WidgetAttribute]
		private HBox hboxSymbolWidgets;
		
		[WidgetAttribute]
		private Frame frameOriginal;
		
		[WidgetAttribute]
		private Frame frameProcessed;				
		
		[WidgetAttribute]
		private Button btnNext;
		
		[WidgetAttribute]
		private VButtonBox vboxNextButtons;
		
		[WidgetAttribute]
		private ComboBox comboSymbolType;
		
		[WidgetAttribute]
		private Entry entrySymbol;
		
		[WidgetAttribute]
		private Expander expanderLog;
		
		[WidgetAttribute]
		private ToolButton toolLoadImage;
		
		#endregion Glade widgets	
		
		#region Otros widgets
		
		private LogView logView;
				
		private ImageArea imageAreaOriginal;
		private ImageArea imageAreaProcessed;
		
		#endregion Otros widgets
		
		#region Otros atributos
		
		private MathTextDatabase database;
		
		private MathTextBitmap mtb;
		private MathSymbol symbol;
		
		private Thread learningThread;
		
		private const string title="Aprendedor de carácteres matemáticos - ";
		
		private bool databaseModified;
		
		#endregion Otros atributos
		
		public static void Main(string[] args)
		{			
			new MainWindow();
		}
		
		/// <summary>
		/// El constructor de <code>MainWindow</code>.
		/// </summary>
		public MainWindow()
		{
			
			Application.Init();
			Glade.XML gxml = new Glade.XML (null, 
				"mathtextlearner.glade", "mainWindow", null);
				
			gxml.Autoconnect (this);
			
			Initialize();
			Application.Run();
		}
		
		/// <summary>
		/// Inicializa los controles de la ventana.
		/// </summary>
		private void Initialize()
		{
			mainWindow.Title=title+"Nueva base de datos";
			
			database= new MathTextDatabase(new CaracteristicDatabase());
			database.SymbolLearned += 
				new SymbolLearnedEventHandler(OnSymbolLearned);
				
			database.LearningStepDone +=
				new ProcessingStepDoneEventHandler(OnLearningStepDone);
			
			imageAreaOriginal = new ImageArea();
			imageAreaOriginal.ImageMode = ImageAreaMode.Zoom;
			frameOriginal.Add(imageAreaOriginal);
			
			imageAreaProcessed = new ImageArea();
			imageAreaProcessed.ImageMode = ImageAreaMode.Zoom;			
			frameProcessed.Add(imageAreaProcessed);			
			
			toolLoadImage.IconWidget = new Gtk.Image(ImageResources.ImageLoadIcon22);
			menuLoadImage.Image =  new Gtk.Image(ImageResources.ImageLoadIcon16);
			
				
			logView = new LogView();
			expanderLog.Add(logView);
			
			
			mainWindow.ShowAll();		
			
		}		
		
		/// <summary>
		/// Manejador del evento provocado al hacer click en el boton de abrir una
		/// base de datos nueva.
		/// </summary>
		private void OnMenuDatabaseClicked(object sender, EventArgs arg)
		{			
			ShowDBSaveQuestionDialog();
			database = new MathTextDatabase(new CaracteristicDatabase());			
			SetTitle("Nueva base de datos",false);
			LogLine("¡Nueva base de datos creada con éxito!");
			
		}
		
		/// <summary>
		/// Metodo que maneja el evento provocado al aprenderse un simbolo en la
		/// base de datos.
		/// </summary>
		private void OnSymbolLearned(object sender,EventArgs arg)
		{
			Application.Invoke(OnSymbolLearnedThread);
		}
		
		private void OnSymbolLearnedThread(object sender,EventArgs arg)
		{
			ResetWidgets();
			string msg="!Símbolo aprendido con éxito!";
			
			SetTitle(null,true);
				
			LogLine(msg);	
			OkDialog.Show(mainWindow, MessageType.Info, msg);
			
		}
		
		/// <summary>
		/// Metodo para restaurar la interfaz a su estado inicial tras haber
		/// aprendidio un caracter.
		/// </summary>
		public void ResetWidgets()
		{
			vboxNextButtons.Sensitive = false;
			toolbar.Sensitive = true;
			entrySymbol.Text = "";
			comboSymbolType.Active = -1;
			menuOpen.Sensitive = true;
			menuLoadImage.Sensitive = true;
			menuSaveAs.Sensitive = true;
			menuDatabase.Sensitive = true;
			imageAreaOriginal.Image = null;
			imageAreaProcessed.Image = null;
		}
		
		
		/// <summary>
		/// Metodo que maneja el evento provocado al completarse un paso
		/// del proceso durante el aprendizaje.
		/// </summary>
		private void OnLearningStepDone(object sender,
		                                ProcessingStepDoneEventArgs arg)
		{
			// TODO Refactorizar las bases de datos para MathTextDatabase 
			// tenga los eventos
			Application.Invoke(sender,arg,OnLearningStepDoneThread);	
		}
		
		private void OnLearningStepDoneThread(object sender, EventArgs a)
		{
			
			ProcessingStepDoneEventArgs arg = (ProcessingStepDoneEventArgs) a;
			btnNext.Sensitive = true;
			LogLine(arg.Process.GetType() + ": " + arg.Result);
		}
		
		/// <summary>
		/// Metodo que maneja el evento provocado al hacer click en la opción 
		/// de menu  "Abrir una base de datos"
		/// </summary>
		private void OnMenuOpenClicked(object sender, EventArgs arg)
		{
			OpenDatabase();
		}
		
		/// <summary>
		/// Metodo que maneja el evento que se provoca cuando se hace click en la opcion de menu
		/// de guardar la base de datos.
		/// </summary>
		private void OnMenuSaveAsClicked(object sender, EventArgs arg)
		{
			SaveDatabase();
		}
		
		/// <summary>
		/// Metodo que maneja el evento provocado al hacer click en la opcion de menu "acerca de".
		/// </summary>
		private void OnMenuAboutClicked(object sender, EventArgs arg)
		{
			AppInfoDialog.Show(
				mainWindow,
				"Aprendedor de carácteres matemáticos",
				"Esta aplicación permite aprender un carácter y añadirlo a"+
				" una base de datos de carácteres nueva o creada previamente.");	
			
		}
		
		/// <summary>
		/// Metodo que gestiona el evento que se provoca el cerrar la ventana.
		/// </summary>
		private void OnMainWindowDeleted(object sender,DeleteEventArgs arg)
		{			
			OnExit();
		}
		
		
		/// <summary>
		/// Metodo que gestiona el cierre de la aplicacion.
		/// </summary>
		private void OnExit()
		{
			ShowDBSaveQuestionDialog();
			imageAreaOriginal.Image=null;
			imageAreaProcessed.Image=null;
			try
			{
				learningThread.Abort();					
			}
			catch(Exception)
			{}
			Application.Quit();			
		}
		
		/// <summary>
		/// Metodo que gestiona el evento que se provoca al hacer 
		/// click en el boton "Aprender" de la interfaz.
		/// </summary>
		private void OnBtnLearnClicked(object sender, EventArgs arg)
		{
			string errorMsg="";
			symbol=new MathSymbol();
			if(entrySymbol.Text.Trim()=="")
			{
				errorMsg=".- El texto del símbolo no es válido.\n";
			}
			else
			{
				symbol.Text=entrySymbol.Text.Trim();
			}			
			
			switch(comboSymbolType.Active){
				case(0):
					symbol.SymbolType=MathSymbolType.Identifier;
					break;
				case(1):
					symbol.SymbolType=MathSymbolType.Number;
					break;
				case(2):
					symbol.SymbolType=MathSymbolType.Operator;
					break;
				case(3):
					symbol.SymbolType=MathSymbolType.LeftDelimiter;
					break;
				case(4):
					symbol.SymbolType=MathSymbolType.RightDelimiter;
					break;
				default:					
					errorMsg+=".- El tipo del símbolo no es correcto.\n";		
					break;
			}
			
			if(errorMsg=="")
			{
				//NO hay errores de validación
				vboxNextButtons.Sensitive=true;
				hboxSymbolWidgets.Sensitive=false;		
				menuDatabase.Sensitive=false;
				menuLoadImage.Sensitive=false;
				menuSaveAs.Sensitive=false;
				menuOpen.Sensitive=false;
				learningThread=null;
				toolbar.Sensitive=false;							
				
			}
			else
			{
				//Informamos de que no podemos aprender el caracter.
				OkDialog.Show(
					mainWindow,
					MessageType.Error,
					"El símbolo no puede ser aprendido porque:\n\n{0}",
					errorMsg);				
					
				LogLine(errorMsg);
			}			
		
		}
		
		/// <summary>
		/// Metodo que invoca el proceso de aprendizaje de simbolos de la base de datos.
		/// </summary>
		private void LearnProccess()
		{
			try
			{
				database.Learn(mtb,symbol);
				databaseModified=true;
				SetTitle(null,true);
			}
			catch(ExistingSymbolException e)
			{
				Application.Invoke(e,
					new LearningFailedArgs(e.ExistingSymbol),
					OnLearningProccessFailed);
			}		
		}
		
		public void OnLearningProccessFailed(object sender, EventArgs a)
		{
			
			string msg="!Ya hay un símbolo, «"+(a as LearningFailedArgs).ExistingSymbol 
					+"», con las mismas caracteristicas binarias en la base de datos!";	
								
			LogLine(msg);
			ResetWidgets();
			
			
			OkDialog.Show(mainWindow, MessageType.Error,msg);
		}
		
		/// <summary>
		/// Gestor del evento que se provoca al hacer click sobre el boton de "Paso a paso".
		/// </summary>
		private void OnBtnNextClicked(object sender, EventArgs arg)
		{
			if(learningThread == null){
				database.StepByStep = true;
				expanderLog.Expanded = true;
				learningThread = new Thread(new ThreadStart(LearnProccess));	
				learningThread.Priority = ThreadPriority.Highest;		
				learningThread.Start();						
			}
			else
			{			
				if(learningThread.IsAlive)
				{
					learningThread.Resume();
					btnNext.Sensitive = false;				
				}
			}				
		}
		
		/// <summary>
		/// Gestor del evento que se provoca al hacer click sobre el boton "Hasta el final"
		/// de la interfaz.
		/// </summary>
		private void OnBtnTilEndClicked(object sender, EventArgs arg)
		{
			database.StepByStep=false;
			if(learningThread!=null)
			{
				learningThread.Resume();
			}
			else
			{
				LearnProccess();				
			}
		}
		
		/// <summary>
		/// Gestor del evento provocado al hacer click en la opcion de menu de "Salir".
		/// </summary>
		private void OnMenuExitClicked(object sender,EventArgs arg)
		{
			OnExit();
		}
		
		/// <summary>
		/// Gestor del evento que se provoca al hacer click en el botón
		/// "Cargar imagen".
		/// </summary>
		private void OnLoadImageClicked(object sender, EventArgs arg)
		{
			string filename;
			
			if(ImageLoadDialog.Show(mainWindow , out filename) 
				== ResponseType.Ok)
			{								
				Pixbuf b = new Pixbuf(filename);				
				imageAreaOriginal.Image = b;
				
				mtb = new MathTextBitmap(b);
				imageAreaProcessed.Image = mtb.ProcessedBitmap;
				hboxSymbolWidgets.Sensitive = true;
				
				LogLine("¡Archivo de imagen «"+filename+"» cargado correctamente!");
			}
		}
		
		/// <summary>
		/// Gestor del evento provocado al hacer click en el boton "Abrir base de datos".
		/// </summary>
		private void OnToolOpenClicked(object sender, EventArgs arg)
		{
			OpenDatabase();
		}
		
		private void OpenDatabase()
		{			
			// Preguntamos si se quiere salvar lo actual.
			ShowDBSaveQuestionDialog();
			
			// Abrimos la base de datos.
			string file;		
			if(DatabaseOpenDialog.Show(mainWindow, out file) 
				== ResponseType.Ok)
			{
				// El usuario acepta la apertura del archivo.
				database = MathTextDatabase.Load(file);				
					
				this.SetTitle(file,false);
				
				LogLine("¡Base de datos «"+ file+ "» cargada correctamente!");
				databaseModified=false;
			}
		}
		
		/// <summary>
		/// Metodo para facilitar el mostrar el cuadro de dialogo de confirmacion 
		/// de guardar la base de datos.
		/// </summary>
		private void ShowDBSaveQuestionDialog()
		{
			if(databaseModified)
			{
				// Solo guardamos si el usuario quiere, y habiamos modificado.
				ResponseType result = 
					ConfirmDialog.Show(
						mainWindow,
						"¿Desea guardar los cambios en la base de datos?");
					
				if(result == ResponseType.Yes)
				{
					SaveDatabase();
				}
			}
		}	
		
		
		private void SaveDatabase()
		{
			string file;
			if (DatabaseSaveDialog.Show(mainWindow,out file)
				== ResponseType.Ok)
			{				
				string ext = Path.GetExtension(file);
				
				OkDialog.Show(mainWindow, MessageType.Other, ext);
				
				if(!(	ext==".xml" 
					|| 	ext==".XML"
					|| 	ext==".jilfml"
					|| 	ext==".JILFML"))				
				{
					file += ".jilfml";
				}
				
				bool save = true;
				
				if(File.Exists(file)
					&& ConfirmDialog.Show(
						mainWindow,
						"El archivo «{0}» ya existe. ¿Desea sobreescibirlo?",
						Path.GetFileName(file)) 
							== ResponseType.No)
						
				{
					// No queremos sobreescribir un archivo existente.
					save = false;
				}
				
				if(save)
				{				
					database.Save(file);
						
					OkDialog.Show(
						mainWindow,
						MessageType.Info,
						"Base de datos guardada correctamente en {0}",
						Path.GetFileName(file));
						
					LogLine(
						"¡Base de datos guardada con éxito en «{0}»!",
						Path.GetFileName(file));
						
					databaseModified=false;
					
					SetTitle(file,false);	
				}
			}
		}
		
		
		/// <summary>
		/// Metodo que permite añadir un mensaje a la zona de texto de
		/// mensajes de informacion en una nueva línea.
		/// </summary>
		/// <param name="message">
		/// El mensaje que se mostrará.
		/// </param>
		private void LogLine(string message, params object [] args)
		{			
			logView.LogLine(message, args);
				
		}		
		
		/// <summary>
		/// Este método permite cambiar el título de la venta de forma sencilla.
		/// </summary>
		/// <param name="databaseName">
		/// El nombre de la base de datos que se está editando.
		/// </param>
		/// <param name="modified">
		/// Si la base de datos ha sido modificada o no.
		/// </param>
		private void SetTitle(string databaseName,bool modified)
		{
			if(databaseName!=null)
			{
			    // Si tenemos base de datos, ponemos su nombre en el titulo.				
				
				mainWindow.Title=
					title + Path.GetFileName(databaseName) +(modified?" (Modificada)":"");
			}
			else
			{
				mainWindow.Title=title+ "Nueva base de datos " +(modified?" (Modificada)":"");
			}
		}
	}
	
	
}