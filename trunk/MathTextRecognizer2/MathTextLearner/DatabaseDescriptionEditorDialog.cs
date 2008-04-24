// DatabaseDescriptionEditorDialog.cs created with MonoDevelop
// User: luis at 16:10 20/04/2008

using System;
using System.Collections.Generic;

using Gtk;

using MathTextCustomWidgets.Dialogs;


namespace MathTextLearner
{
	
	/// <summary>
	/// This class implements the symbol label property editor used
	/// both in MathTextLearner and MathTextRecognizer.
	/// </summary>
	public class DatabaseDescritpionEditorDialog
	{
		[Glade.WidgetAttribute]
		private Dialog databaseDescriptionEditorDialog = null;
		
		[Glade.WidgetAttribute]
		private Entry shortDescEntry = null;
		
		[Glade.WidgetAttribute]
		private TextView longDescTextV = null;
		
		
		public DatabaseDescritpionEditorDialog(Window parent)
		{
			Glade.XML gxml = new Glade.XML(null,
			                               "mathtextlearner.glade", 
			                               "databaseDescriptionEditorDialog",
			                               null);
			
			gxml.Autoconnect(this);
			
			databaseDescriptionEditorDialog.TransientFor = parent;
			
		}
		
#region Properties

		/// <value>
		/// Contains the short description for the database.
		/// </value>
		public string ShortDescription
		{
			get
			{
				return shortDescEntry.Text.Trim();
			}
			set
			{
				shortDescEntry.Text = value;
			}
		}
		
		/// <value>
		/// Contains the long description for the database.
		/// </value>
		public string LongDescription
		{
			get
			{
				return longDescTextV.Buffer.Text.Trim();
			}
			
			set
			{
				longDescTextV.Buffer.Text = value;
			}
		}
		
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Shows the dialog, and waits until a response is given.
		/// </summary>
		/// <returns>
		/// The dialog's response.
		/// </returns>
		public ResponseType Show()
		{
			ResponseType res = ResponseType.None;
			while(res == ResponseType.None)
			{
				res = (ResponseType)(databaseDescriptionEditorDialog.Run());
			}
			
			return res;
		}
		
		/// <summary>
		/// Destroys the dialog's resources.
		/// </summary>
		public void Destroy()
		{
			databaseDescriptionEditorDialog.Destroy();
		}
		
#endregion Public methods
	
		
#region Private methods
		
		/// <summary>
		/// Handles the event launched when the close butotn is clicked.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		[GLib.ConnectBefore]
		private void OnCloseBtnClicked(object sender, EventArgs args)
		{
			List<string> errors = new List<string>();
			
			if(String.IsNullOrEmpty(ShortDescription))
			{
				errors.Add("· Debes escribir una descripción corta.");
			}
			
			if(String.IsNullOrEmpty(LongDescription))
			{
				errors.Add("· Debes escribir una descripción larga.");
			}
			
			if(errors.Count > 0)
			{		
				// We found errors and have to inform the user.
				
				string errorMsg = "";
				foreach(string error in errors)
				{
					errorMsg += String.Format("{0}\n",error);
				}
				
				OkDialog.Show(databaseDescriptionEditorDialog,
				              MessageType.Warning,
				              "Para continuar, tienes que solucionar los "
				              +"siguientes problemas:\n\n{0}",
				              errorMsg);	
				
				databaseDescriptionEditorDialog.Respond(ResponseType.None);
			}
			else
			{
				databaseDescriptionEditorDialog.Respond(ResponseType.Ok);
			}
		}
		
		
#endregion Private methods
	}
}
