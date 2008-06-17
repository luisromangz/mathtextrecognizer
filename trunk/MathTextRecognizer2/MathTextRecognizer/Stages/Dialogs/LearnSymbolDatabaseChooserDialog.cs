// LearnSymbolDatabaseChooserDialog.cs created with MonoDevelop
// User: luis at 20:25 24/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

using Glade;
using Gtk;

using MathTextCustomWidgets.Dialogs;

using MathTextLibrary.Databases;

using MathTextRecognizer.DatabaseManager;

namespace MathTextRecognizer.Stages.Dialogs
{
	
	/// <summary>
	/// This class implements a dialog for choosing the database in
	/// which the selected segmented node's image will be learnt.
	/// </summary>
	public class LearnSymbolDatabaseChooserDialog
	{
		[WidgetAttribute]
		private Dialog learnSymbolDatabaseChooserDialog = null;
		
		[WidgetAttribute]
		private VBox optionsVB = null;
		
		
		private Tooltips optionsTooltips;
		
		private RadioButton newRB;
		
		private Dictionary<string, DatabaseFileInfo> databaseHash;
		
		private DatabaseFileInfo choosenDatabase;
		
		/// <summary>
		/// <c>LearnSymbolDatabaseChooserDialog</c>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// The dialog's parent dialog, to which it's modal.
		/// </param>
		/// <param name="databases">
		/// The databases the user can choose from.
		/// </param>
		public LearnSymbolDatabaseChooserDialog(Window parent,
		                                        List<DatabaseFileInfo> databases)
		{
			XML gxml = new XML(null, 
			                   "mathtextrecognizer.glade", 
			                   "learnSymbolDatabaseChooserDialog",
			                   null);
			
			gxml.Autoconnect(this);
			
			learnSymbolDatabaseChooserDialog.Modal=true;
			learnSymbolDatabaseChooserDialog.Resizable = false;
			learnSymbolDatabaseChooserDialog.TransientFor = parent;
			
			databaseHash = new Dictionary<string,DatabaseFileInfo>();
			
			optionsTooltips = new Tooltips();
			
			RadioButton groupRB = new RadioButton("group");
			foreach(DatabaseFileInfo databaseInfo in databases)
			{
				// We add a new option per database
				string label = System.IO.Path.GetFileName(databaseInfo.Path);
				RadioButton optionRB = new RadioButton(groupRB, label);
				
				optionRB.Clicked += new EventHandler(OnOptionRBClicked);
				optionsVB.Add(optionRB);
				
				MathTextDatabase database = databaseInfo.Database;
			
				
				optionsTooltips.SetTip(optionRB, 
				                       String.Format("{0}\n{1}",
				                                     database.ShortDescription,
				                                     database.Description),
				                       "database description");
				
				databaseHash.Add(label, databaseInfo);
			}
			
			// We add the option of creating a new database.
			newRB = new RadioButton(groupRB, "Crear nueva base de datos");
			newRB.Clicked += new EventHandler(OnOptionRBClicked);
			optionsVB.Add(newRB);
			optionsTooltips.SetTip(newRB, 
			                       "Te permite crear una base de datos nueva",
			                       "new databse description");
			
			optionsTooltips.Enable();
			
			learnSymbolDatabaseChooserDialog.ShowAll();
		}
		
#region Properties
		
		/// <value>
		/// Contains the selected database in which the symbol will be learned.
		/// </value>
		public DatabaseFileInfo ChoosenDatabase
		{
			get
			{
				return choosenDatabase;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Shows the dialog and waits for it to emit a response.
		/// </summary>
		/// <returns>
		/// The response given by the dialog.
		/// </returns>
		public ResponseType Show()
		{
			ResponseType res = ResponseType.None;
			
			while (res == ResponseType.None)
			{
				// We have to wait until a valid response has been emited
				res = (ResponseType)(learnSymbolDatabaseChooserDialog.Run());
			}
			
			return res;
		}
		
		/// <summary>
		/// Hides the dialog, and frees its resources.
		/// </summary>
		public void Destroy()
		{
			learnSymbolDatabaseChooserDialog.Destroy();
		}
		
#endregion Public methods
		
#region Private methods
		
		/// <summary>
		/// Indicates if the dialog has validation errors, and shows them.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private bool HasErrors()
		{
			if(choosenDatabase == null && !newRB.Active)
			{
				OkDialog.Show(learnSymbolDatabaseChooserDialog,
				              MessageType.Warning,
				              "Debes seleccionar una opción para continuar.");
				return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// Handles the clicking in the ok button.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnOkBtnClicked(object sender, EventArgs args)
		{
			if(HasErrors())
			{
				learnSymbolDatabaseChooserDialog.Respond(ResponseType.None);
			}			
			else
			{
				learnSymbolDatabaseChooserDialog.Respond(ResponseType.Ok);
			}
		}
		
		/// <summary>
		/// Handles the choosing of a database.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnOptionRBClicked(object sender, EventArgs args)
		{
			RadioButton clickedRB = (RadioButton)sender;
			
			if(clickedRB == newRB)
			{
				choosenDatabase = null;
			}
			else
			{
				choosenDatabase = databaseHash[clickedRB.Label];
			}
		}
		
#endregion Private methods
	}
}
