// SymbolLabelEditorWidget.cs created with MonoDevelop
// User: luis at 18:55 15/04/2008

using System;
using System.Collections.Generic;

using MathTextLibrary.Config;

namespace MathTextCustomWidgets.Widgets
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
		
		[Glade.WidgetAttribute]
		private Gtk.ListStore model;
		
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
				return symbolsCBEntry.ActiveText;
			}
			
			set
			{
				symbolsCBEntry.Entry.Text = value;
			}
		}
		
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Loads the symbols from the config.
		/// </summary>
		/// <param name="symbolLabels">
		/// The <c>SymbolLabelInfo</c> instances to be added.
		/// </param>
		public void LoadSymbols()
		{		
			model.Clear();
			List<SymbolLabelInfo> symbolsInfo = LibraryConfig.Instance.Symbols;
			foreach(SymbolLabelInfo info in symbolsInfo)
			{
				model.AppendValues(info.Label, info.Symbol);
			}
		}
		
#endregion Public methods
		
#region Private methods
		/// <summary>
		/// Initializes this widget children.
		/// </summary>
		private void InitializeWidgets()
		{
			this.Add(this.symbolLabelEditorHB);

			// A model is neccesary to store the different values
			model = new Gtk.ListStore(typeof(string), 
			                          typeof(string));			
			symbolsCBEntry.Model = model;
			
			symbolsCBEntry.TextColumn = 0;
			Gtk.CellRendererText cell = new Gtk.CellRendererText();
			symbolsCBEntry.PackStart(cell, true);
			symbolsCBEntry.AddAttribute(cell,"text",1);
			
			
			LoadSymbols();
			
			// We adjust the wrap so it is mostrly a square.
			symbolsCBEntry.WrapWidth = 
				(int) Math.Ceiling(Math.Sqrt(model.IterNChildren())) -1;
		}
		
#endregion Private methods
		
		
	}
}
