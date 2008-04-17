// SymbolLabelEditorDialog.cs created with MonoDevelop
// User: luis at 17:42 17/04/2008

using System;

using Gtk;
using Glade;

namespace MathTextCustomWidgets.Dialogs.SymbolLabel
{
	
	/// <summary>
	/// This class implements the editor for <c>SymbolLabelDialog</c> list
	/// entries.
	/// </summary>
	public class SymbolLabelListDialog
	{
	
		[WidgetAttribute]
		private Entry symbolEntry;
		
		[WidgetAttribute]
		private Entry labelEntry;
		
		[WidgetAttribute]
		private Dialog symbolLabelEditorDialog;
		
		[WidgetAttribute]
		private Button okBtn;
		
		/// <summary>
		/// <c>SymbolLabelEditorDialog</c>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// This dialog parent's window.
		/// </param>
		public SymbolLabelListDialog(Window parent)
		{
			XML gxml = new XML(null, "gui.glade", "symbolLabelEditorDialog",null);			
			gxml.Autoconnect(this);
			
			symbolLabelEditorDialog.TransientFor = parent;
			
			symbolLabelEditorDialog.AddActionWidget(okBtn, ResponseType.Ok);
		}

		/// <value>
		/// Contains the label of the symbol.
		/// </value>
		public string Label 
		{
			get 
			{
				return labelEntry.Text.Trim();
			}
			set 
			{
				labelEntry.Text = value;
			}
		}

		/// <value>
		/// Contains the symbol string.
		/// </value>
		public string Symbol 
		{
			get
			{
				return symbolEntry.Text.Trim();
			}
			set 
			{
				symbolEntry.Text = value;
			}
		}
		
		/// <summary>
		/// Shows the dialog, and waits for a response.
		/// </summary>
		/// <returns>
		/// A <see cref="ResponseType"/>
		/// </returns>
		public ResponseType Show()
		{
			return (ResponseType)(symbolLabelEditorDialog.Run());
		}
		
		
		/// <summary>
		/// Frees the dialog's resources.
		/// </summary>
		public void Destroy()
		{
			symbolLabelEditorDialog.Destroy();
		}
		
	}
}
