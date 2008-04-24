// LearnSymbolDatabaseChooserDialog.cs created with MonoDevelop
// User: luis at 20:25 24/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

using Glade;
using Gtk;

using MathTextLibrary.Databases;

using MathTextRecognizer.DatabaseManager;

namespace MathTextRecognizer
{
	
	/// <summary>
	/// This class implements a dialog for choosing the database in
	/// which the selected segmented node's image will be learnt.
	/// </summary>
	public class LearnSymbolDatabaseChooserDialog
	{
		[WidgetAttribute]
		private Dialog learnSymbolDatabaseChooserDialog = null;
		
		/// <summary>
		/// <c>LearnSymbolDatabaseChooserDialog</c>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// The dialog's parent dialog, to which it's modal.
		/// </param>
		public LearnSymbolDatabaseChooserDialog(Window parent,
		                                        List<DatabaseFileInfo> databases)
		{
			XML gxml = new XML(null, 
			                   "mathtextrecognizer.glade", 
			                   "learnsymbolDataabseChooserDialog",
			                   null);
			
			gxml.Autoconnect(this);
			
		}
		
#region Properties
		
		public MathTextDatabase ChoosenDatabase
		{
			get
			{
				
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
		}
		
#endregion Private methods
	}
}
