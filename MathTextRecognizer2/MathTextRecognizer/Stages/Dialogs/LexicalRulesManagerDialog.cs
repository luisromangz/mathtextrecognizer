// LexicalRulesManagerDialog.cs created with MonoDevelop
// User: luis at 19:10 28/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

using Gtk;
using Glade;

namespace MathTextRecognizer
{
	
	/// <summary>
	/// This class implements the dialog which provides access to the set of
	/// lexical rules.
	/// </summary>
	public class LexicalRulesManagerDialog
	{
#region Glade widgets
		[WidgetAttribute]
		private Dialog lexicalRulesManagerDialog = null;
		
		[WidgetAttribute]
		private TreeView rulesTV = null;
		
		[WidgetAttribute]
		private Button upBtn = null;
		
		[WidgetAttribute]
		private Button downBtn = null;
		
		[WidgetAttribute]
		private Button editBtn = null;
		
#endregion Glade widgets
		
#region Other fields
		
		private ListStore rulesStore;
		
#endregion Other fields
		
		/// <summary>
		/// <c>LexicalRulesManagerDialog</c>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// The dialog's parente window.
		/// </param>
		public LexicalRulesManagerDialog(Window parent)
		{
			XML gxml = new XML(null, 
			                   "mathtextrecognizer.glade",
			                   "lexicalRulesManagerDialog",
			                   null);
			
			gxml.Autoconnect(this);
			
			lexicalRulesManagerDialog.TransientFor = parent;
			
		}
		
#region Public methods
		
		/// <summary>
		/// Shows the dialog and waits for it to respond.
		/// </summary>
		/// <returns>
		/// The dialog's response type.
		/// </returns>
		public ResponseType Show()
		{
			return (ResponseType)(lexicalRulesManagerDialog.Run());
		}
		
		/// <summary>
		/// Destroys the dialog's resources  and hides it.
		/// </summary>
		public void Destroy()
		{
			lexicalRulesManagerDialog.Destroy();
		}
		
#endregion Public methods
		
#region Non-public methods
		
		/// <summary>
		/// Initializes the dialog's widgets;
		/// </summary>
		private void InitializeWidgets()
		{
			
		}
		
		/// <summary>
		/// Opens the new rule dialog, and adds the rule if that dialog response
		/// is ok.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnAddBtnClicked(object sender, EventArgs args)
		{
			
		}

		/// <summary>
		/// Removes the selected rule from the rule-list, if the user
		/// gives its confirmation.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnRemoveBtnClicked(object sender, EventArgs args)
		{			
		}	
		
		/// <summary>
		/// Moves the selected rule one positon higher.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnUpBtnClicked(object sender, EventArgs args)
		{
			
		}
		
		/// <summary>
		/// Moves the selected rule one postion lower.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnDownBtnClicked(object sender, EventArgs args)
		{
			
		}
		
		/// <summary>
		/// Opens the lexical rule editor dialog for the selected rule.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnEditBtnClicked(object sender, EventArgs args)
		{
			
		}
		
		/// <summary>
		/// If the user agrees, saves the rule-list.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnSaveBtnClicked(object sender, EventArgs args)
		{
			
		}
		
#endregion Non-public methods
	}
}
