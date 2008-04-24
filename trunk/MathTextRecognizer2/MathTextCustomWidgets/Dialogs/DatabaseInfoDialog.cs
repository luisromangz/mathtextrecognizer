// DatabaseInfoDialog.cs created with MonoDevelop
// User: luis at 14:58 21/04/2008

using System;

using Gtk;
using Glade;

using MathTextLibrary.Databases;

namespace MathTextCustomWidgets.Dialogs
{
	
	/// <summary>
	/// This class implements a dialog showing info about a database.
	/// </summary>
	public class DatabaseInfoDialog
	{
		[WidgetAttribute]
		private Dialog databaseInfoDialog = null;
		
		[WidgetAttribute]
		private Label shortDescLabel =null;
		
		[WidgetAttribute]
		private Label longDescLabel =null;
		
		[WidgetAttribute]
		private Label typeLabel = null;
		
		
		
		/// <summary>
		/// <c>DatabaseInfoDialog</c>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// The dialog's parent window.
		/// </param>
		public DatabaseInfoDialog(Window parent)
		{
			XML gxml = new XML(null, "gui.glade", "databaseInfoDialog", null);
			gxml.Autoconnect(this);
			
			
		}
		
		/// <summary>
		/// Sets the appropiate labels based on the database.
		/// </summary>
		/// <param name="database">
		/// The database which info we want to be showed.
		/// </param>
		public void SetDatabase(MathTextDatabase database)
		{
			shortDescLabel.Text = String.Format("<i>{0}</i>", 
			                                    database.ShortDescription);
			
			longDescLabel.Text = database.Description;
			
			typeLabel.Text = database.DatabaseTypeShortDescription;
		}
		
		/// <summary>
		/// Shows the dialog and waits for its response.
		/// </summary>
		/// <returns>
		/// A <see cref="ResponseType"/>
		/// </returns>
		public ResponseType Show()
		{
			return (ResponseType)(databaseInfoDialog.Run());
		}
		
		/// <summary>
		/// Frees the dialog's resources.
		/// </summary>
		public void Destroy()
		{
			databaseInfoDialog.Destroy();
		}
		                    
	}
}
