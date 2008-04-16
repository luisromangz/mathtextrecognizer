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
			
			InitializeWidgets();
			
			
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
				//return symbolsCBEntry.ActiveText;
				return "";
			}
			
			set
			{
				//symbolsCBEntry.Entry.Text = value;
				;
			}
		}
		
		
#endregion Properties
		
#region Private methods
		/// <summary>
		/// Initializes this widget children.
		/// </summary>
		private void InitializeWidgets()
		{
			this.Add(this.symbolLabelEditorHB);

			// A model is neccesary to store the different values
			Gtk.ListStore model = new Gtk.ListStore(typeof(string), 
			                                        typeof(string));

			symbolsCBEntry.WrapWidth = 2;
			
			
			
			symbolsCBEntry.Model = model;
			
			symbolsCBEntry.TextColumn = 0;
			Gtk.CellRendererText cell = new Gtk.CellRendererText();
			
			cell.Xalign = 0.0f;
			
			symbolsCBEntry.PackStart(cell, true);
			symbolsCBEntry.AddAttribute(cell,"text",1);
			
			// ∀∃∄∇∊∫∩∪⋂⋀⋁∧∨∞
			
			model.AppendValues("+", "Suma");
			model.AppendValues("-", "Resta");	
			model.AppendValues("·", "Multiplicación");
			model.AppendValues("/", "División");
			model.AppendValues("√", "Raíz");
			model.AppendValues("∑", "Sumatorio");
			model.AppendValues("∫", "Integral");
			model.AppendValues("⋂", "Intersecctador");
			model.AppendValues("⋃", "Unidor");
			model.AppendValues("∪", "Unión");
			model.AppendValues("∩", "Intersección");
			model.AppendValues("⋀", "Conjuntor");
			model.AppendValues("⋁", "Disyuntor");
			model.AppendValues("∧", "Conjunción");
			model.AppendValues("∨", "Disyunción");
			model.AppendValues("∊", "Pertenece");
			model.AppendValues("∀", "Para todo");
			model.AppendValues("∃", "Existe");
			model.AppendValues("∄", "No existe");
			model.AppendValues("∞", "Infinito");
			
				
		}
		
#endregion Private methods
		
		
	}
}
