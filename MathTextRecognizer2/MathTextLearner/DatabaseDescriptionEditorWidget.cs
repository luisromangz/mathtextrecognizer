// DatabaseDescriptionEditorWidget.cs created with MonoDevelop
// User: luis at 13:18 20/04/2008

using System;
using System.Collections.Generic;

using Gtk;


namespace MathTextLearner
{
	
	/// <summary>
	/// This class implements the symbol label property editor used
	/// both in MathTextLearner and MathTextRecognizer.
	/// </summary>
	public class DatabaseDescritpionEditorWidget : Gtk.Alignment
	{
		[Glade.WidgetAttribute]
		private VBox databaseDescriptionEditorVB;
		
		[Glade.WidgetAttribute]
		private Entry shortDescEntry;
		
		[Glade.WidgetAttribute]
		private TextView longDescTextV;
		
		
		public DatabaseDescritpionEditorWidget() : base(0.5f, 0.5f, 1.0f, 1.0f)
		{
			Glade.XML gxml = new Glade.XML(null,
			                               "mathtextlearner.glade", 
			                               "databaseDescriptionEditorVB",
			                               null);
			
			gxml.Autoconnect(this);
			
			InitializeWidgets();			
			
			this.ShowAll();
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
				return longDescTextV.Buffer.Text;
			}
			
			set
			{
				longDescTextV.Buffer.Text = value;
			}
		}
		
		
#endregion Properties
	
		
#region Private methods
		/// <summary>
		/// Initializes the widget's children widgets.
		/// </summary>
		private void InitializeWidgets()
		{
		
			this.Add(databaseDescriptionEditorVB);
		}
		
#endregion Private methods
	}
}
