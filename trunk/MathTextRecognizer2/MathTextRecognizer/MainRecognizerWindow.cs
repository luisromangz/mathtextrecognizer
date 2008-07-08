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

using Mono.Unix;

using MathTextCustomWidgets;
using MathTextCustomWidgets.Widgets.Logger;
using MathTextCustomWidgets.Widgets.ImageArea;
using MathTextCustomWidgets.Dialogs;
using MathTextCustomWidgets.Dialogs.SymbolLabel;

using MathTextLibrary;
using MathTextLibrary.Bitmap;
using MathTextLibrary.BitmapSegmenters;
using MathTextLibrary.Symbol;
using MathTextLibrary.Controllers;
using MathTextLibrary.Databases.Characteristic.Characteristics;

using MathTextRecognizer.Stages;
using MathTextRecognizer.Controllers;
using MathTextRecognizer.DatabaseManager;
using MathTextRecognizer.LexicalRulesManager;
using MathTextRecognizer.SyntacticalRulesManager;

namespace MathTextRecognizer
{
	/// <summary>
	/// This class implements the MathTextRecognizer app's main window.
	/// </summary>
	public class MainRecognizerWindow
	{
#region Glade-Widgets
		[WidgetAttribute]
		private Gtk.Window mainWindow = null;		
		
		[WidgetAttribute]		
		private ToolButton toolLoadImage = null;
				
		[WidgetAttribute]		
		private ToolButton toolDatabase = null;
		
		[WidgetAttribute]
		private Expander expLog = null;
		
		[WidgetAttribute]
		private ImageMenuItem menuOpenDatabaseManager = null;
		
		[WidgetAttribute]
		private ImageMenuItem menuLoadImage = null;
		
		[WidgetAttribute]
		private HBox messageInfoHB = null;
		
		[WidgetAttribute]
		private Notebook recognizingStepsNB = null;
		
		[WidgetAttribute]
		private Label stageNameLabel = null;
		
		[WidgetAttribute]
		private Label messageInfoLabel = null;
		
#endregion Glade-Widgets

#region Fields
		
		private bool recognizementFinished;
		
		private OCRStageWidget ocrWidget;		
		private TokenizingStageWidget  tokenizingWidget;		
		private ParsingStageWidget parsingWidget;
			
		private LogView logView;
		
		private static string title;		
		
		
		private string imageFile;
		
#endregion Fields
		
#region Main method
		
		/// <summary>
		/// The app's entry point.
		/// </summary>
		public static void Main(string[] args)
		{			
			
			Application.Init();
			new MainRecognizerWindow();
			Application.Run();
		}
#endregion Main method
		
#region Constructors
		/// <summary>
		/// <see cref="MainRecognizer">'s constructor.
		/// </summary>
		public MainRecognizerWindow()
		{
			Glade.XML gxml = new Glade.XML (null, 
			                                "mathtextrecognizer.glade",
			                                "mainWindow",
			                                null);
			gxml.Autoconnect (this);			
			this.InitializeWidgets();			
			
			
			Config.RecognizerConfig.Instance.Changed+= 
				new EventHandler(OnConfigChanged);	
			
			
			this.mainWindow.Icon = 
				ImageResources.LoadPixbuf("mathtextrecognizer48");
			
			OnConfigChanged(this, EventArgs.Empty);
			
			
		}
	
		/// <summary>
		/// <see cref="MainRecognizer"/>'s static constructor.
		/// </summary>
		static MainRecognizerWindow()
		{
			title ="Reconocedor de texto matemático";
		}
		
#endregion Constructors
		
#region Properties
		

		/// <value>
		/// Contains the log area expanded state value.
		/// </value>
		public bool LogAreaExpanded
		{
			get
			{
				return expLog.Expanded;
			}
			set
			{
				expLog.Expanded = value;
				logView.Follow = value;
			}
		}

		/// <value>
		/// Contains the <see cref="Gdk.Window"/> used to actually draw de 
		/// app's window.
		/// </value>
		public Gtk.Window Window 
		{
			get 
			{
				return mainWindow;
			}
		}
		
		/// <value>
		/// Contains a boolean value indicating if some elements of the gui
		/// which shouldn't be clicked while a process is running are sensitive.
		/// </value>
		public bool ProcessItemsSensitive
		{
			get
			{
				return toolDatabase.Sensitive;
			}
			set
			{
				toolDatabase.Sensitive =value;
				toolLoadImage.Sensitive =value;
				
				menuLoadImage.Sensitive =value;
				menuOpenDatabaseManager.Sensitive =value;
			}
		}
		
		/// <value>
		/// Contains the widget used to represent and control the 
		/// segmentation and matching process.
		/// </value>
		public OCRStageWidget OCRWidget
		{
			get
			{
				return ocrWidget;				
			}
		}
		
		/// <value>
		/// Contains the widget used to show and control the sintactical
		/// analisys process.
		/// </value>
		public TokenizingStageWidget TokenizingWidget
		{
			get
			{
				return tokenizingWidget;
			}			
		}

		/// <value>
		/// Contains the widget used to show the syntactical analysis.
		/// </value>
		public ParsingStageWidget FormulaMatchingWidget 
		{
			get 
			{
				return parsingWidget;
			}
		}

		/// <value>
		/// Contains the image file path to be recognized.
		/// </value>
		public string ImageFile 
		{
			get 
			{
				return imageFile;
			}
		}

		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Clears the log view.
		/// </summary>
		public void ClearLog()
		{			
			logView.ClearLog();			
		}
		
		/// <summary>
		/// Writes a message in the log view.
		/// </summary>
		/// <param name="message">
		/// The message to be written.
		/// </param>
		public void Log(string message, params object[] args)
		{			
			logView.LogLine(message, args);
		}
		
		/// <summary>
		/// Creates the OCR stage widget.
		/// </summary>		
		public void CreateOCRWidget()
		{
			// We add the OCR widget.
			ocrWidget = new OCRStageWidget(this);
			
			
			recognizingStepsNB.AppendPage(ocrWidget,
			                              new Label(OCRStageWidget.WidgetLabel));
			
			ocrWidget.ShowAll();
		}
		
		/// <summary>
		/// Creates the unassisted stage widget.
		/// </summary>	
		public void CreateUnassistedWidget()
		{
			UnassistedStageWidget widget = new UnassistedStageWidget(this);
			
			recognizingStepsNB.AppendPage(widget,
			                              new Label(UnassistedStageWidget.WidgetLabel));
			
			widget.ShowAll();
		}
		
		/// <summary>
		/// Creates the tokenizing stage widget.
		/// </summary>	
		public void CreateTokenizingWidget()
		{
			// We add the tokenizer widget.
			tokenizingWidget = new TokenizingStageWidget(this);
			recognizingStepsNB.AppendPage(tokenizingWidget,
			                              new Label(TokenizingStageWidget.WidgetLabel));
			tokenizingWidget.ShowAll();
		}
		
		/// <summary>
		/// Creates the parsing stage widget.
		/// </summary>	
		public void CreateParsingWidget()
		{
			parsingWidget = new ParsingStageWidget(this);
			recognizingStepsNB.AppendPage(parsingWidget,
			                              new Label(ParsingStageWidget.WidgetLabel));
			parsingWidget.ShowAll();
		}
		
		/// <summary>
		/// Creates the blackboard stage widget.
		/// </summary>	
		public void CreateBlackboardWidget()
		{
			Widget stage = new BlackboardStageWidget(this);
			recognizingStepsNB.AppendPage(stage,
			                              new Label(BlackboardStageWidget.WidgetLabel));
			stage.ShowAll();
		}
		
		
		/// <summary>
		/// Allows the user to select an image and then loads it.
		/// </summary>
		public bool LoadImage()
		{
			string filename;
			
			if(ImageLoadDialog.Show(mainWindow, out filename)
				== ResponseType.Ok)
			{			
				this.imageFile = filename;
		
				recognizementFinished=false;
				ClearLog();
				ResetState();
				return true;
			}
			
			return false;
		}
		
	
		
#endregion Public methods
		
#region Non-public methods
		
	
		/// <summary>
		/// Intializes the windows widgets.
		/// </summary>
		private void InitializeWidgets()
		{		
			mainWindow.Title = title;
			
			// We load the icons
			menuLoadImage.Image = ImageResources.LoadImage("insert-image16");
			toolLoadImage.IconWidget = ImageResources.LoadImage("insert-image22");
			
			menuOpenDatabaseManager.Image = ImageResources.LoadImage("database16");
			toolDatabase.IconWidget = ImageResources.LoadImage("database22");
			
			// The log view is created
			logView = new LogView();
			expLog.Add(logView);	
			
			while(recognizingStepsNB.NPages > 0)
			{
				recognizingStepsNB.RemovePage(0);
			
			}
			
			recognizingStepsNB.AppendPage(new InitialStageWidget(this),
			                              new Label(InitialStageWidget.WidgetLabel));
			
			mainWindow.ShowAll();
			
			recognizingStepsNB.Page = 0;
		}
		
		
		/// <summary>
		/// Handles the window destruction.
		/// </summary>
		/// <param name="sender">An <see cref="object"/></param>
		/// <param name="arg">A <see cref="DeleteEventArgs"/></param>
		private void OnMainWindowDeleteEvent(object sender,DeleteEventArgs arg)
		{
			OnExit();
		}		
		
		/// <summary>
		/// Shows the app's info dialog.
		/// </summary>
		private void OnMenuAboutClicked(object sender, EventArgs arg)
		{
			AppInfoDialog.Show(
				mainWindow,
				"Reconocedor de texto matemático",
				"Este programa se encarga de el reconocimiento de fórmulas contenidas en imágenes y su conversión a un formato de texto plano especificado a traves de reglas.",
			     "mathtextrecognizer");
		}
		
		/// <summary>
		/// Opens the database manager.
		/// </summary>
		private void OnOpenDatabaseManagerClicked(object sender, EventArgs arg)
		{	
			DatabaseManagerDialog dialog = 
				new DatabaseManagerDialog(this.Window);
			dialog.Show();			
			dialog.Destroy();
		}
		
			
		/// <summary>
		/// Exits the application.
		/// </summary>
		private void OnExitClicked(object sender, EventArgs arg)
		{
			OnExit();
		}
		
		/// <summary>
		/// Handles the clicking on the load image button.
		/// </summary>
		private void OnLoadImageClicked(object sender, EventArgs arg)
		{
			
			ResponseType res = ResponseType.Yes;
			
			if(recognizementFinished)
			{
				 res = 
				 	ConfirmDialog.Show(mainWindow,
									   "Si cargas una nueva imágen perderás el reconocimiento realizado.\n"+
									   "¿Deseas continuar?");
			}
			
			if(res==ResponseType.Yes)
			{			
				(recognizingStepsNB.Children[1] as RecognizingStageWidget).SetInitialData();
			}
		}
		
			
		/// <summary>
		/// Handles the change in the notebook holding the widgets for the
		/// recognizing stages, so we can chow its name in a label.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="ChangeCurrentPageArgs"/>
		/// </param>
		private void OnRecognizingStepsNBSwitchPage(object sender,
		                                            SwitchPageArgs arg)
		{
			
			Widget page = 
				recognizingStepsNB.Children[(int)arg.PageNum];
			stageNameLabel.Markup = 
				String.Format("<b><i>{0}</i></b>",
				              recognizingStepsNB.GetTabLabelText(page));
			
			bool hasLoadImage = 
				(page.GetType() == typeof(OCRStageWidget)					
				|| page.GetType() == typeof(UnassistedStageWidget));
			
			toolLoadImage.Sensitive = hasLoadImage;
			this.menuLoadImage.Sensitive = hasLoadImage;
		}
		
		/// <summary>
		/// Opens the symbol list editor.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnSymbolListEditorItem(object sender, EventArgs arg)
		{
			SymbolLabelDialog dialog = 
				new SymbolLabelDialog(mainWindow);
			
			dialog.Show();
			dialog.Destroy();
		}		

		/// <summary>
		/// Takes care of running threads before exiting the app.
		/// </summary>
		private void OnExit()
		{
			foreach (RecognizingStageWidget widget in recognizingStepsNB) 
			{
				widget.Abort();
			}
			Application.Quit();			
		}	
		
		/// <summary>
		/// Handles the event produced when the config changes.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnConfigChanged(object sender, EventArgs args)
		{
			if(Config.RecognizerConfig.Instance.DatabaseFilesInfo.Count ==0)
			{
				messageInfoHB.Visible = true;
				messageInfoLabel.Text = "No hay bases de datos  de caracteres para reconocer, añada una en el gestor de bases de datos.";
			}			
			else if(Config.RecognizerConfig.Instance.LexicalRules.Count == 0)
			{
				messageInfoHB.Visible = true;
				messageInfoLabel.Text = "No hay reglas léxicas definidas, añada una en el gestor de reglas léxicas.";
			}
			else if(Config.RecognizerConfig.Instance.SyntacticalRules.Count == 0)
			{
				messageInfoHB.Visible = true;
				messageInfoLabel.Text = "No hay reglas sintácticas definidas, añada una en el gestor de reglas sintácticas.";
			}
			else
			{
				messageInfoHB.Visible = false;
			}
			
				
		}
		
		
		
		/// <summary>
		/// Resets the widgets values to sanitized standards.
		/// </summary>
		private void ResetState()
		{
			
			menuOpenDatabaseManager.Sensitive=true;
			
			toolDatabase.Sensitive=true;
			
			// We reset the state of the stage widgets,
			// so the information generated for previous seasons is discarded.
			
			while(recognizingStepsNB.NPages > 2)
			{
				recognizingStepsNB.RemovePage(2);
			}
			
			
			
			recognizementFinished=true;
		}
		
		/// <summary>
		/// Opens the lexical rules manager.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnLexicalManagerItemClicked(object sender, EventArgs args)
		{
			LexicalRulesManagerDialog dialog = 
				new LexicalRulesManagerDialog(this.Window);
			
			dialog.Show();
			
			dialog.Destroy();
		}
		
		/// <summary>
		/// Opens the syntactical rules manager.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnSyntacticalManagerItemActivate(object sender, 
		                                              EventArgs args)
		{
			SyntacticalRulesManagerDialog dialog = 
				new SyntacticalRulesManagerDialog(this.Window);
			dialog.Show();
			
			dialog.Destroy();
		}
		
		/// <summary>
		/// Opens the output settings dialog.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnOutputSettingsItemActivate(object sender, EventArgs args)
		{
			Output.OutputSettingsDialog dialog = 
				new Output.OutputSettingsDialog(this.Window);
			
			dialog.Show();
			dialog.Destroy();
		}
		
#endregion Non-public methods
	}
	

}
