// SymbolLabelEditorWidget.cs created with MonoDevelop
// User: luis at 18:55 15/04/2008

using System;

namespace MathTextCustomWidgets.Editors
{
	
	/// <summary>
	/// This class implements the symbol label property editor used
	/// both in MathTextLearner and MathTextRecognizer.
	/// </summary>
	public class SymbolLabelEditorWidget : Gtk.Alignment
	{
		[Glade.WidgetAttribute]
		private Gtk.ComboBoxEntry symbolsCBEntry;
		
		[Glade.WidgetAttribute]
		private Gtk.HBox symbolLabelEditorHB;
		
		public SymbolLabelEditorWidget()
			: base(0.5f, 0.5f, 1.0f, 1.0f)
		{
			Glade.XML gxml = 
				new Glade.XML(null,"gui.glade", "symbolLabelEditorHB",null);
			gxml.Autoconnect(this);
			
			this.Add(this.symbolLabelEditorHB);
			
			this.ShowAll();
		}
		
#region Properties

		/// <value>
		/// Contains the label of the symbol.
		/// </value>
		public String Label
		{
			get
			{
				return symbolsCBEntry.ActiveText;
			}
			
			set
			{
				symbolsCBEntry.Entry.Text = value;
			}
		}
		
		
#endregion Properties
		
		
	}
}
