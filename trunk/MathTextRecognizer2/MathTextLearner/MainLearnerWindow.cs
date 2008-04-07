/*
 * Created by SharpDevelop.
 * User: Ire
 * Date: 27/12/2005
 * Time: 11:03
 */
 
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using Gtk;
using Glade;

using CustomGtkWidgets;
using CustomGtkWidgets.Logger;
using CustomGtkWidgets.ImageArea;
using CustomGtkWidgets.CommonDialogs;

using MathTextLibrary;
using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;
using MathTextLibrary.Utils;
using MathTextLibrary.Databases;
using MathTextLibrary.Databases.Characteristic;

using MathTextLearner.Assistant;

namespace MathTextLearner
{
	/// <summary>
	/// Esta clase representa la ventana principal de la aplicacion de
	/// aprendizaje de caracteres en la base de datos.
	/// </summary>
	public class MainLearnerWindow
	{
		
#region Glade widgets
		//Aqui se declaran los controles de la interfaz.		
		[WidgetAttribute]
		private Gtk.Window mainWindow;
		
		[WidgetAttribute]
		private ImageMenuItem menuDatabase;
		
		[WidgetAttribute]
		private MenuItem menuLoad;
		
		[WidgetAttribute]
		private MenuItem menuSaveAs;
		
		[WidgetAttribute]
		private MenuItem menuOpen;
		
		[WidgetAttribute]
		private Toolbar toolbar;
		
		[WidgetAttribute]
		private ToolButton toolNewDatabase;
		
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
		private HBox nextButtonsHB;
		
		[WidgetAttribute]
		private ComboBox comboSymbolType;
		
		[WidgetAttribute]
		private Entry entrySymbol;
		
		[WidgetAttribute]
		private Expander expanderLog;
		
		[WidgetAttribute]
		private IconView imagesIV;
		
		[WidgetAttribute]
		private VBox imagesVB;
		
		[WidgetAttribute]
		private ToolButton toolSaveAs;
		
		[WidgetAttribute]
		private Button addImageBtn;
		
		[WidgetAttribute]
		private Button removeImageBtn;
		
		[WidgetAttribute]
		private Button nextImageBtn;
		
		[WidgetAttribute]
		private HBox imagesHB;
		
		[WidgetAttribute]
		private HBox buttonsHB;
		
		[WidgetAttribute]
		private HBox messageInfoHB;
		
#endregion Glade widgets	
		
#region Otros widgets
		
		private int conflicts;
		
		private LogView logView;
		
		private ListStore imagesStore;
				
		private ImageArea imageAreaOriginal;
		private ImageArea imageAreaProcessed;
		
#endregion Otros widgets
		
#region Otros atributos
		
		private MathTextDatabase database;
		
		private MathTextBitmap mtb;
		private MathSymbol symbol;
		
		private Thread learningThread;
		
		private const string title="Aprendedor de caracteres matemáticos";
		
		private bool databaseModified;	
		
		// Indica si el proceso de reconocimiento debe realizarse paso a paso
		private bool stepByStep; 
		
		
#endregion Otros atributos

#region Metodos publicos
		public static void Main(string[] args)
		{	
			Application.Init();		
			new MainLearnerWindow();	
			Application.Run();
		}
		
		/// <summary>
		/// El constructor de <code>MainWindow</code>.
		/// </summary>
		public MainLearnerWindow()
		{			
			
			Glade.XML gxml = new Glade.XML (null,
			                                "mathtextlearner.glade", 
			                                "mainWindow", 
			                                null);
				
			gxml.Autoconnect (this);
			
			Initialize();
		
		}
		

		/// <summary>
		/// Metodo para restaurar la interfaz a su estado inicial tras haber
		/// aprendidio un caracter.
		/// </summary>
		public void ResetWidgets()
		{
			nextButtonsHB.Sensitive = false;
			toolbar.Sensitive = true;
			entrySymbol.Text = "";
			comboSymbolType.Active = -1;
			menuOpen.Sensitive = true;			
			menuSaveAs.Sensitive = true;
			menuDatabase.Sensitive = true;
			imageAreaOriginal.Image = null;
			imageAreaProcessed.Image = null;
		}
		
#endregion Metodos publicos
		
#region Metodos privados
		
		/// <summary>
		/// Muestra el mensaje que indica como termino el proceso de aprendizaje, 
		/// e inicicializa contadores y otras variables relacionadas
		/// con el proceso de aprendizaje actual.
		/// </summary>
		private void AllImagesLearned()
		{
			
			string conflictsMessage ="";
			
			// Generamos la cadena que informa de los conflictos
			switch(conflicts)
			{
				case 0:
					break;
				case 1:
					conflictsMessage = "Hubo un conflicto entre caracteres";
					break;
				default:
					conflictsMessage = 
						String.Format("Hubo {0} conflictos entre caracteres.",
						              conflicts);
					break;
			}
			
			// Informamos al usuario.
			OkDialog.Show(mainWindow,
				          MessageType.Info,
				          "Todos los caracteres fueron procesados.{0}",
				          conflictsMessage);
				
			mtb = null;
			
			// Reinciamos el contador de conflictos.
			conflicts = 0;
			
			buttonsHB.Sensitive = false;
			
			imagesVB.Sensitive = true;
		}
		
		/// <summary>
		/// Inicializa los controles de la ventana.
		/// </summary>
		private void Initialize()
		{
			mainWindow.Title=title;	
			
			imageAreaOriginal = new ImageArea();
			imageAreaOriginal.ImageMode = ImageAreaMode.Zoom;
			frameOriginal.Add(imageAreaOriginal);
			
			imageAreaProcessed = new ImageArea();
			imageAreaProcessed.ImageMode = ImageAreaMode.Zoom;			
			frameProcessed.Add(imageAreaProcessed);		
				
			logView = new LogView();
			expanderLog.Add(logView);
			
			// La imagen reducida en la primera columna
			imagesIV.PixbufColumn = 0;
			
			imagesStore = new ListStore(typeof(Gdk.Pixbuf), typeof(Gdk.Pixbuf)); 
			imagesIV.Model = imagesStore;
			
			toolNewDatabase.IconWidget =ImageResources.LoadImage("database-new22");
			
			menuDatabase.Image =ImageResources.LoadImage("database-new16");
				
			mainWindow.ShowAll();			
		}		
		
		/// <summary>
		/// Metodo que invoca el proceso de aprendizaje de simbolos de la base
		///  de datos.
		/// </summary>
		private void LearnProccess()
		{
			try
			{
				// Lanzamos la excepcion para que no se modifique
				// la base de datos
				database.Learn(mtb, symbol);
				databaseModified=true;
				SetTitle(null,true);
			}
			catch(DuplicateSymbolException e)
			{
				Application.Invoke(e,
					new LearningFailedArgs(e.DuplicatedSymbol),
					OnLearningProccessFailedThread);
			}		
		}
		
		/// <summary>
		/// Carga una nueva imagen en la lista.
		/// </summary>
		/// <param name="image">
		/// La imagen a cargar.
		/// </param>
		private void LoadNewImage(Gdk.Pixbuf image)
		{
			Gdk.Pixbuf smallImage = ImageUtils.MakeThumbnail(image, 48);
			TreeIter iter = imagesStore.AppendValues(smallImage, image);
			
			imagesIV.ScrollToPath(imagesStore.GetPath(iter));			
			imagesIV.SelectPath(imagesStore.GetPath(iter));
			
			buttonsHB.Sensitive = true;
		}
		
		/// <summary>
		/// Carga un conjunto de imagenes en la lista.
		/// </summary>
		/// <param name="images">
		/// La lista que contiene el conjunto de imagenes a cambiar.
		/// </param>
		private void LoadNewImages(List<Gdk.Pixbuf> images)
		{
			Gdk.Pixbuf scaledDown;
			foreach(Gdk.Pixbuf p in images)
			{
				LoadNewImage(p);
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
		/// Maneja el uso del boton de añadir imagenes.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnAddImageBtnClicked(object sender, EventArgs arg)
		{
			string filename;
			
			if(ImageLoadDialog.Show(mainWindow , out filename) 
				== ResponseType.Ok)
			{								
				Gdk.Pixbuf b = new Gdk.Pixbuf(filename);	

				LoadNewImage(b);	
				
				LogLine("¡Archivo de imagen «{0}» añadido correctamente!",
				        filename);
				
				
			}
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
				nextButtonsHB.Sensitive=true;
				hboxSymbolWidgets.Sensitive=false;		
				menuDatabase.Sensitive=false;				
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
		/// Gestor del evento que se provoca al hacer click sobre el boton de "Paso a paso".
		/// </summary>
		private void OnBtnNextClicked(object sender, EventArgs arg)
		{
			if(learningThread == null){
				stepByStep = true;
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
			stepByStep=false;
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
		
		private void OnLearningProccessFailedThread(object sender, 
		                                              EventArgs a)
		{
			
			string msg=
				"!Ya hay un símbolo, «"
				+(a as LearningFailedArgs).DuplicateSymbol 
				+"», con las mismas propiedades en la base de datos!";	
								
			LogLine(msg);
			ResetWidgets();
			
			// Indicamos que ha habido un conflicto.
			conflicts++;
			
			OkDialog.Show(mainWindow, MessageType.Error,msg);
			
			PrepareForNewImage();
		}
		
		private void OnImagesIVSelectionChanged(object s, EventArgs a)
		{
			removeImageBtn.Sensitive = imagesIV.SelectedItems.Length > 0;
			
			TreeIter selectedIter;
			
			if(imagesIV.SelectedItems.Length > 0)
			{
			
				TreePath selectedImagePath = imagesIV.SelectedItems[0];

				imagesStore.GetIter(out selectedIter, selectedImagePath);
				
				Gdk.Pixbuf orig = (Gdk.Pixbuf)(imagesStore.GetValue(selectedIter,1));
				
				mtb = new MathTextBitmap(orig);
				mtb.ProcessImage(database.Processes);
				
				imageAreaOriginal.Image = orig;
				imageAreaProcessed.Image = mtb.ProcessedBitmap;
			}
			else
			{
				imageAreaOriginal.Image = null;
				imageAreaProcessed.Image = null;
			}
		}
	
		/// <summary>
		/// Metodo que maneja el evento provocado al completarse un paso
		/// del proceso durante el aprendizaje.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="ProcessingStepDoneArgs"/>
		/// </param>
		private void OnLearningStepDone(object sender, ProcessingStepDoneArgs arg)
		{
			
			Application.Invoke(sender, 
			                   arg, 
			                   OnLearningStepDoneThread);	
		}
		
		private void OnLearningStepDoneThread(object sender, EventArgs a)
		{
			
			ProcessingStepDoneArgs arg = (ProcessingStepDoneArgs) a;
			btnNext.Sensitive = true;
			LogLine("{0}: {1}", arg.Process.GetType(), arg.Result);
		}	
		
		/// <summary>
		/// Metodo que gestiona el evento que se provoca el cerrar la ventana.
		/// </summary>
		private void OnMainWindowDeleted(object sender,DeleteEventArgs arg)
		{			
			OnExit();
		}
		
		/// <summary>
		/// Metodo que maneja el evento provocado al hacer click en la opcion de menu "acerca de".
		/// </summary>
		private void OnMenuAboutClicked(object sender, EventArgs arg)
		{
			AppInfoDialog.Show(
				mainWindow,
				"Aprendedor de caracteres matemáticos",
				"Esta aplicación permite aprender un caracter y añadirlo a"+
				" una base de datos de caracteres nueva o creada previamente.");	
			
		}
		
		
		/// <summary>
		/// Gestor del evento provocado al hacer click en la opcion de menu de
		/// "Salir".
		/// </summary>
		private void OnMenuExitClicked(object sender,EventArgs arg)
		{
			OnExit();
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
		/// Manejador del evento provocado al hacer click en el boton de abrir una
		/// base de datos nueva.
		/// </summary>
		private void OnNewDatabaseClicked(object sender, EventArgs arg)
		{			
			ShowDBSaveQuestionDialog();
			
			NewDatabaseAsisstant assistant = 
				new NewDatabaseAsisstant(mainWindow);
			
			ResponseType res = assistant.Run();
			
			if(res == ResponseType.Ok)
			{
				SetDatabase(assistant.Database);
				
				LoadNewImages(assistant.Images);
				
				nextImageBtn.Sensitive = true;
				
				SetTitle("Nueva base de datos",false);
				LogLine("¡Nueva base de datos creada con éxito!");
			}
			else
			{
				LogLine("Creación de base de datos cancelada");
			}
			
			assistant.Destroy();
			
			
			
		}
		
		
		private void OnNextImageBtnClicked(object sender, EventArgs arg)
		{
			nextImageBtn.Sensitive = false;
			hboxSymbolWidgets.Sensitive = true;
			
			TreeIter iter;
			imagesStore.GetIterFirst(out iter);
			TreePath selectedImagePath = imagesStore.GetPath(iter);			
				                     
			imagesIV.ScrollToPath(selectedImagePath);
			imagesIV.SelectPath(selectedImagePath);
			
			
			imagesVB.Sensitive = false;
			
		}
			
		
		private void OnRemoveImageBtnClicked(object sender, EventArgs arg)
		{
			TreeIter iter;
			imagesStore.GetIter(out iter,imagesIV.SelectedItems[0]);
			
			imagesStore.Remove(ref iter);
			
			if(imagesStore.IterNChildren() == 0)
				buttonsHB.Sensitive = false;
					
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
			
			databaseModified = true;
			menuSaveAs.Sensitive = true;
			toolSaveAs.Sensitive = true;
			
			PrepareForNewImage();
			
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
				MathTextDatabase database = MathTextDatabase.Load(file);
				if(database == null)
				{
					// No se abrio un archivo de base de datos, informamos.
					OkDialog.Show(this.mainWindow,
					              MessageType.Warning,
					              "El archivo «{0}» no contiene una base de datos "+
					              "correcta, y no se pudo abrir.",
					              file);
					
					return;
				}
				
				SetDatabase(database);				
					
				this.SetTitle(file,false);
				
				LogLine("¡Base de datos «"+ file+ "» cargada correctamente!");
				databaseModified=false;
			}
		}
		
		private void PrepareForNewImage()
		{
			hboxSymbolWidgets.Sensitive = false;
			nextButtonsHB.Sensitive =false;
			nextImageBtn.Sensitive = true;
			
			TreeIter iter;
			imagesStore.GetIterFirst(out iter);			
			imagesStore.Remove(ref iter);   
			
			if(imagesStore.IterNChildren() == 0)
			{
				
				AllImagesLearned();
				
				           
			}
			
		}
		
		private void SaveDatabase()
		{
			string file;
			if (DatabaseSaveDialog.Show(mainWindow,out file)
				== ResponseType.Ok)
			{				
				string ext = Path.GetExtension(file);
				
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
						"Base de datos guardada correctamente en «{0}»",
						Path.GetFileName(file));
						
					LogLine(
						"¡Base de datos guardada con éxito en «{0}»!",
						Path.GetFileName(file));
						
					databaseModified=false;
					menuSaveAs.Sensitive = false;
					toolSaveAs.Sensitive = false;
					
					SetTitle(file,false);	
				}
			}
		}
		
		private void SetDatabase(MathTextDatabase database)
		{
			this.database = database; 
			
			database.SymbolLearned += new SymbolLearnedHandler(OnSymbolLearned);
				
			database.LearningStepDone +=
				new ProcessingStepDoneHandler(OnLearningStepDone);
			
			messageInfoHB.Visible = false;
			
			mtb = null;
			

			toolSaveAs.Sensitive = false;
			menuSaveAs.Sensitive = false;
			
			imagesHB.Sensitive = true;			
			imagesVB.Sensitive = true;
			
			imagesStore.Clear();
			
			buttonsHB.Sensitive = true;
			
			hboxSymbolWidgets.Sensitive = false;
			nextButtonsHB.Sensitive =false;
			
			entrySymbol.Text = "";
			comboSymbolType.Active = -1;
			
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
					title
						+ " - "
						+ Path.GetFileName(databaseName) 
						+(modified?" (Modificada)":"");
			}
			else
			{
				mainWindow.Title=
					title
						+" - "
						+ "Nueva base de datos " 
						+(modified?" (Modificada)":"");
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
#endregion Metodos privados
		
	}
	
	/// <summary>
	/// Encapsula el simbolo duplicado para poder pasarlo como argumento
	/// al manejador del evento en el hilo de la interfaz.
	/// </summary>
	class LearningFailedArgs : EventArgs
	{
		
		private MathSymbol duplicateSymbol;
		
		public LearningFailedArgs(MathSymbol symbol)
		{
			duplicateSymbol = symbol;
		}
		
		/// <value>
		/// El simbolo duplicado que genero el fallo.
		/// </value>
		public MathSymbol DuplicateSymbol
		{
			get 
			{
				return duplicateSymbol;
			}
		}
		
		
	}
}