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
using Gdk;
using Glade;

using MathTextCustomWidgets;
using MathTextCustomWidgets.Widgets;
using MathTextCustomWidgets.Widgets.Logger;
using MathTextCustomWidgets.Widgets.ImageArea;
using MathTextCustomWidgets.Dialogs;
using MathTextCustomWidgets.Dialogs.SymbolLabel;

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
		private Gtk.Window mainWindow = null;
		
		[WidgetAttribute]
		private ImageMenuItem menuDatabase = null;
		
		[WidgetAttribute]
		private MenuItem menuSaveAs = null;
		
		[WidgetAttribute]
		private MenuItem menuSave = null;
		
		[WidgetAttribute]
		private MenuItem menuOpen = null;
		
		[WidgetAttribute]
		private Toolbar toolbar = null;
		
		[WidgetAttribute]
		private ToolButton toolNewDatabase = null;
		
		[WidgetAttribute]
		private ToolButton toolSave = null;
			
		[WidgetAttribute]
		private HBox hboxSymbolWidgets = null;
		
		[WidgetAttribute]
		private Frame frameOriginal = null;
		
		[WidgetAttribute]
		private Frame frameProcessed = null;				
		
		[WidgetAttribute]
		private Button btnNext = null;
		
		[WidgetAttribute]
		private HBox nextButtonsHB = null;
		
		[WidgetAttribute]
		private Alignment symbolEditorPlaceholder = null;
		
		[WidgetAttribute]
		private Expander expanderLog= null;
		
		[WidgetAttribute]
		private IconView imagesIV = null;
		
		[WidgetAttribute]
		private VBox imagesVB = null;
		
		[WidgetAttribute]
		private ToolButton toolSaveAs = null;
		
			
		[WidgetAttribute]
		private Button removeImageBtn = null;
		
		[WidgetAttribute]
		private Button nextImageBtn = null;
		
		[WidgetAttribute]
		private Button editPropertiesBtn = null;
		
		[WidgetAttribute]
		private HBox imagesHB = null;
	
		
		[WidgetAttribute]
		private HBox messageInfoHB = null;
		
		[WidgetAttribute]
		private Label databaseDescriptionLabel = null;
		
		[WidgetAttribute]
		private Label databaseTypeLabel = null;
		
#endregion Glade widgets	
		
#region Otros widgets
		
		private int conflicts;
		
		private LogView logView;
		
		private ListStore imagesStore;
				
		private ImageArea imageAreaOriginal;
		private ImageArea imageAreaProcessed;
		
		
		private SymbolLabelEditorWidget symbolLabelEditor;
		
#endregion Otros widgets
		
#region Otros atributos
		
		private MathTextDatabase database;
		
		private MathTextBitmap mtb;
		private MathSymbol symbol;
		
		private Thread learningThread;
		
		private const string title="Aprendedor de caracteres matemáticos";
		
		private bool databaseModified;	
		private string databasePath;
		
		// Indica si el proceso de reconocimiento debe realizarse paso a paso
		private bool stepByStep; 
		
		private Tooltips labelTooltips;
		
		
#endregion Otros atributos

#region Metodos publicos	
		
		/// <summary>
		/// <c>MainLearnerWindow</c>'s default constructor.
		/// </summary>
		public MainLearnerWindow() : this(null, null,null, null, null)
		{
			
		}
		
		/// <summary>
		/// <c>MainLearnerWindow</c>'s parametriced constructor.
		/// </summary>
		/// <param name="parent">
		/// The windo's parent window. 
		/// </param>
		/// <param name="inputDatabase">
		/// A database to be loaded upon start.
		/// </param>
		/// <param name="inputDatabasePath">
		/// The path of the input database.
		/// </param>
		/// <param name="inputImage">
		/// An image to be learned upon start.
		/// </param>
		public MainLearnerWindow(Gtk.Window parent,
		                         MathTextDatabase inputDatabase, 
		                         string inputDatabasePath,
		                         Pixbuf inputImage,
		                         string inputImageName)
		{			
			
			Glade.XML gxml = new Glade.XML (null,
			                                "mathtextlearner.glade", 
			                                "mainWindow", 
			                                null);
				
			gxml.Autoconnect (this);
			
			Initialize();
			
			if (parent !=null)
			{
				mainWindow.Modal = true;
				mainWindow.TransientFor = parent;
			}
			
			// We try loading the image.
			
			if(inputDatabase!=null)
			{
				// We have to load a database.
				SetDatabase(inputDatabase);	
				
				SetTitle(inputDatabasePath);
				
				if (inputImage != null)
				{
					LoadNewImage(inputImage);
				}				
			}
			else if(inputImage!=null)
			{
				// We haven't specified a database, but want to learn and image,
				// so we launch the new database wizard, and add that image.
				NewDatabaseAsisstant assistant = 
					new NewDatabaseAsisstant(mainWindow,
					                         inputImage, 
					                         inputImageName);
				
				ResponseType res  = assistant.Run();
				if(res == ResponseType.Ok)
				{
					SetDatabase(assistant.Database);
					
					LoadNewImages(assistant.Images);
				}				
				
				assistant.Destroy();
				
				
			}
		
		}
		

		/// <summary>
		/// Metodo para restaurar la interfaz a su estado inicial tras haber
		/// aprendidio un caracter.
		/// </summary>
		public void ResetWidgets()
		{
			nextButtonsHB.Sensitive = false;
			toolbar.Sensitive = true;
			symbolLabelEditor.Label = "";
			menuOpen.Sensitive = true;			
			menuSaveAs.Sensitive = true;
			menuDatabase.Sensitive = true;
			imageAreaOriginal.Image = null;
			imageAreaProcessed.Image = null;
		}
		
#endregion Metodos publicos
		
#region Private methods
		
		/// <summary>
		/// Adds an image to the image list.
		/// </summary>
		/// <param name="filename">
		/// A <see cref="System.String"/> containing the image's location.
		/// </param>
		private void AddImageFile(string filename)
		{
			Gdk.Pixbuf b = new Gdk.Pixbuf(filename);	

			LoadNewImage(b);	
			
			LogLine("¡Archivo de imagen «{0}» añadido correctamente!",
			        filename);
		}
			
		
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
					conflictsMessage = "Hubo un conflicto entre caracteres.";
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
				          "Todos los caracteres fueron procesados.\n{0}",
				          conflictsMessage);
				
			mtb = null;
			
			// Reinciamos el contador de conflictos.
			conflicts = 0;
			
			nextImageBtn.Sensitive = false;
			
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
			imagesIV.SelectionChanged += 
				new EventHandler(OnImagesIVSelectionChanged);
			
			imagesStore = new ListStore(typeof(Gdk.Pixbuf), 
			                            typeof(Gdk.Pixbuf));
						
			imagesIV.Model = imagesStore;
			
			imagesStore.RowInserted += OnImagesStoreRowInserted;
			imagesStore.RowDeleted += OnImagesStoreRowDeleted;
			
			toolNewDatabase.IconWidget =
				ImageResources.LoadImage("database-new22");
			
			menuDatabase.Image =ImageResources.LoadImage("database-new16");
			
			symbolLabelEditor = new SymbolLabelEditorWidget();
			
			symbolEditorPlaceholder.Add(symbolLabelEditor);
			
			labelTooltips = new Tooltips();
				
			mainWindow.ShowAll();			
		}		
		
		/// <summary>
		/// Metodo que invoca el proceso de aprendizaje de simbolos de la base
		///  de datos.
		/// </summary>
		private void LearnProccess()
		{
			
			// Lanzamos la excepcion para que no se modifique
			// la base de datos
			bool learned = database.Learn(mtb, symbol);
			if(learned)
			{
				SetModified(true);
				Application.Invoke(OnSymbolLearnedInThread);
			}
			else
			{
				Application.Invoke(this,
					new LearningFailedArgs(symbol),
					OnLearningProccessFailedInThread);
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
			
			
		}
		
		/// <summary>
		/// Carga un conjunto de imagenes en la lista.
		/// </summary>
		/// <param name="images">
		/// La lista que contiene el conjunto de imagenes a cambiar.
		/// </param>
		private void LoadNewImages(List<Gdk.Pixbuf> images)
		{
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
				AddImageFile(filename);
			
				
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
			if(symbolLabelEditor.Label.Trim()=="")
			{
				errorMsg=".- El texto del símbolo no es válido.\n";
			}
			else
			{
				symbol.Text=symbolLabelEditor.Label.Trim();
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
				
				
				
				learningThread = new Thread(new ThreadStart(LearnProccess));
				learningThread.Start();
				learningThread.Suspend();
				
				return;
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
			nextButtonsHB.Sensitive = false;
			expanderLog.Expanded = true;
			stepByStep = true;
			learningThread.Resume();
		}
		
		/// <summary>
		/// Gestor del evento que se provoca al hacer click sobre el boton "Hasta el final"
		/// de la interfaz.
		/// </summary>
		private void OnBtnTilEndClicked(object sender, EventArgs arg)
		{
			stepByStep=false;
			nextButtonsHB.Sensitive = false;
			
			learningThread.Resume();
		}
		
	
		
		/// <summary>
		/// Handles the event launched when the database properties' edit button
		/// is clicked.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnEditPropertiesBtnClicked(object sender, EventArgs arg)
		{
			DatabaseDescritpionEditorDialog dialog = 
				new DatabaseDescritpionEditorDialog(this.mainWindow);
			
			dialog.ShortDescription = database.ShortDescription;
			dialog.LongDescription = database.Description;
			
			ResponseType res = dialog.Show();
			
			if(res == ResponseType.Ok)
			{
				database.ShortDescription = dialog.ShortDescription;
				database.Description = dialog.LongDescription;
				
				SetModified(true);
			}
			
			dialog.Destroy();
		}
		
		/// <summary>
		/// Metodo que gestiona el cierre de la aplicacion.
		/// </summary>
		private void OnExit()
		{
		
			SaveDatabase();
				
			
			imageAreaOriginal.Image=null;
			imageAreaProcessed.Image=null;
			try
			{
				learningThread.Abort();					
			}
			catch(Exception)
			{}
			
			if(mainWindow.TransientFor == null)
			   Application.Quit();
			 
		}
		
		private void OnLearningProccessFailedInThread(object sender, 
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
		
		/// <summary>
		/// Handles the row inserted image tree model event.
		/// </summary>
		/// <param name="s">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="RowInsertedArgs"/>
		/// </param>
		private void OnImagesStoreRowInserted(object s, RowInsertedArgs a)
		{
			nextImageBtn.Sensitive = true;
		}
		
		/// <summary>
		/// Handles the row deletion event of the image tree model.
		/// </summary>
		/// <param name="s">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="RowDeletedArgs"/>
		/// </param>
		private void OnImagesStoreRowDeleted(object s, RowDeletedArgs a)
		{
			nextImageBtn.Sensitive = imagesStore.IterNChildren() > 0;
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
				imageAreaProcessed.Image = mtb.LastProcessedImage.CreatePixbuf();
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
		private void OnLearningStepDone(object sender, StepDoneArgs arg)
		{
			Application.Invoke(sender, 
			                   arg, 
			                   OnLearningStepDoneInThread);	
			
			if(stepByStep)
			{
				
				learningThread.Suspend();				
			}
		}
		
		private void OnLearningStepDoneInThread(object sender, EventArgs a)
		{
			if(stepByStep)
			{
				nextButtonsHB.Sensitive = true;
				btnNext.IsFocus = true;
			}
			
			StepDoneArgs arg = (StepDoneArgs) a;
			btnNext.Sensitive = true;
			LogLine(arg.Message);
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
			SaveDatabaseAs();
		}
		
		
		/// <summary>
		/// Manejador del evento provocado al hacer click en el boton de abrir una
		/// base de datos nueva.
		/// </summary>
		private void OnNewDatabaseClicked(object sender, EventArgs arg)
		{			
			SaveDatabase();
			
			NewDatabaseAsisstant assistant = 
				new NewDatabaseAsisstant(mainWindow);
			
			ResponseType res = assistant.Run();
			
			if(res == ResponseType.Ok)
			{
				SetDatabase(assistant.Database);
				
				LoadNewImages(assistant.Images);
				
				nextImageBtn.Sensitive = true;
				
				SetTitle("");				
				SetModified(false);
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
			
			symbolLabelEditor.IsFocus = true;
			
		}
			
		
		private void OnRemoveImageBtnClicked(object sender, EventArgs arg)
		{
			TreeIter iter;
			imagesStore.GetIter(out iter,imagesIV.SelectedItems[0]);
			
			imagesStore.Remove(ref iter);					
		}
		
			/// <summary>
		/// Handles the events launched when the save menu item or tool are 
		/// clicked.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnSaveDatabaseClicked(object sender, EventArgs arg)
		{
			SaveDatabase();
			
			
		}
		
	
		private void OnSymbolLearnedInThread(object sender,EventArgs arg)
		{
			ResetWidgets();
			string msg="!Símbolo aprendido con éxito!";
			
			SetModified(true);
				
			LogLine(msg);	
			OkDialog.Show(mainWindow, MessageType.Info, msg);
			
			
			PrepareForNewImage();
			
		}
		
		
		/// <summary>
		/// Handles the activation of the edit symbols menu item.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgas"/>
		/// </param>
		private void OnSymbolsEditItemActivate(object sender, EventArgs args)
		{
			SymbolLabelDialog dialog = 
				new SymbolLabelDialog(mainWindow);
			
			dialog.Show();
			dialog.Destroy();
			
			symbolLabelEditor.LoadSymbols();
			
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
			SaveDatabase();
			
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
				
				SetTitle(file);
				databaseModified=false;
				
				LogLine("¡Base de datos «"+ file+ "» cargada correctamente!");
				
			}
		}
		
		private void PrepareForNewImage()
		{
			hboxSymbolWidgets.Sensitive = false;
			nextButtonsHB.Sensitive =false;
			nextImageBtn.Sensitive = true;
			
			nextImageBtn.IsFocus = true;
			
			TreeIter iter;
			imagesStore.GetIterFirst(out iter);			
			imagesStore.Remove(ref iter);   
			
			if(imagesStore.IterNChildren() == 0)
			{
				
				AllImagesLearned();			
				           
			}
			
		}
		
		/// <summary>
		/// Asks the user for confirmation and then saves a modified database.
		/// </summary>
		private void SaveDatabase()
		{
			if(databaseModified)
			{
				ResponseType res = 
				ConfirmDialog.Show(mainWindow, 
				                   "¿Quieres guardar "
				                   +"los cambios de la base de datos?");
			
				if(res == ResponseType.Yes)
				{
					if(!String.IsNullOrEmpty(databasePath))
					{
						// We save the database in the same path as it was loaded.
						database.Save(databasePath);
						
						SetModified(false);
						
						OkDialog.Show(
							mainWindow,
							MessageType.Info,
							"Base de datos guardada correctamente en «{0}»",
							Path.GetFileName(databasePath));
							
						LogLine(
							"¡Base de datos guardada con éxito en «{0}»!",
							Path.GetFileName(databasePath));
					}
					else
					{
						// If it is a new database, we make use the save as method.
						string path = SaveDatabaseAs();
						if(!String.IsNullOrEmpty(path))
						{
							SetTitle(path);
							SetModified(false);
						}
							
					}
				}
			}
			
		}
		
		/// <summary>
		/// Launches the save as dialog.
		/// </summary>
		/// <returns>
		/// The path the file was saved into, if any.
		/// </returns>
		private string SaveDatabaseAs()
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
						"El archivo «{0}» ya existe. ¿Deseas sobreescibirlo?",
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
						
				
					
					return file;
				}
			}
			
			return "";
		}
		
		/// <summary>
		/// Sets the database the symbols will be learned into, and makes the
		/// proper interface elementents (un)sensitive.
		/// </summary>
		/// <param name="database">
		/// A <see cref="MathTextDatabase"/>
		/// </param>
		private void SetDatabase(MathTextDatabase database)
		{
			this.database = database; 
			
			database.StepDone +=
				new ProcessingStepDoneHandler(OnLearningStepDone);
			
			messageInfoHB.Visible = false;
			
			mtb = null;
			

			toolSaveAs.Sensitive = true;
			menuSaveAs.Sensitive = true;
			
			imagesHB.Sensitive = true;			
			imagesVB.Sensitive = true;
			
			imagesStore.Clear();
			
			hboxSymbolWidgets.Sensitive = false;
			nextButtonsHB.Sensitive =false;
			
			editPropertiesBtn.Sensitive = true;
			
			symbolLabelEditor.Label = "";
			
			SetDatabaseInfo();
			
			
		}
		
		private void SetDatabaseInfo()
		{
			databaseDescriptionLabel.Text = database.ShortDescription;
			
			labelTooltips.SetTip(databaseDescriptionLabel, 
			                     database.Description, 
			                     "longer description");
			
			
			databaseTypeLabel.Text = database.DatabaseTypeShortDescription;
			
			labelTooltips.SetTip(databaseTypeLabel, 
			                     database.DatabaseTypeDescription, 
			                     "longer description");
		}
		
		/// <summary>
		/// Sets the database modified flag and modifies the title acordingly.
		/// </summary>
		/// <param name="value">
		/// A <see cref="System.Boolean"/>
		/// </param>
		private void SetModified(bool value)
		{
			databaseModified = value;
			string modifiedFlag = " (modificada)";
			
			SetTitle(databasePath);
			if(value)
			{
				this.mainWindow.Title = this.mainWindow.Title + modifiedFlag;
			}
			
			toolSave.Sensitive = value;
			menuSave.Sensitive = value;
		}

		/// <summary>
		/// Este método permite cambiar el título de la venta de forma sencilla.
		/// </summary>
		/// <param name="databaseName">
		/// El nombre de la base de datos que se está editando.
		/// </param>
		private void SetTitle(string databasePath)
		{
			if(!String.IsNullOrEmpty(databasePath))
			{
			    // Si tenemos base de datos, ponemos su nombre en el titulo.			
			
				mainWindow.Title= String.Format("{0} - {1}",
				                                title,
				                                Path.GetFileName(databasePath));
				
				this.databasePath = databasePath;
			}
			else
			{
				mainWindow.Title= String.Format("{0} - {1}",
				                                title,
				                                "Nueva base de datos");
				
				this.databasePath = "";
			}
		}
		
		/// <summary>
		/// Addes the images from a folder.
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnAddFolderImagesBtnClicked(object o, EventArgs a)
		{
			// Selccionamos la carpeta
			string folderPath;
			if(FolderOpenDialog.Show(this.mainWindow, out folderPath)
				== ResponseType.Ok)
			{
				
				int added = 0;
								
				string []  extensions = 
				     new string []{"*.jpg", "*.JPG","*.png","*.PNG"};
				
				foreach (string extension in extensions)
				{
					foreach (string file in Directory.GetFiles(folderPath, 
					                                           extension))
					{
						// Si es png o jpg intentamos añadirlo.
						try
						{
							AddImageFile(file);
							added++;
						}
						catch(Exception)
						{
							// Si peta, el fichero tenia una extensión que no
							// hacia honor a su contenido.
						}
					}
				}
				
				if(added > 0)
				{
					// Decimos el número de archivos que hemos añadido.
					OkDialog.Show(
						this.mainWindow,
						MessageType.Info,
						"Se añidieron {0} archivos(s) de imagen.",
						added);
				}
				else
				{
					// Nos quejamos si no pudimos añadir ningún fichero.
					OkDialog.Show(
						this.mainWindow,
						MessageType.Warning,
						"No se encotró ningún archivo de imagen válido en la"
						+" carpeta seleccionada",
						added);
				}
			}			
		}
		
	
#endregion Private methods
		
	}
	
#region Helper classes
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
	
#endregion Helper classes
}