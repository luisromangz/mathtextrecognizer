// SyntacticalExpressionWidget.cs created with MonoDevelop
// User: luis at 18:21 16/05/2008

using System;

using Gtk;
using Glade;

using MathTextCustomWidgets.Dialogs;

namespace MathTextRecognizer.SyntacticalRulesManager
{
	
	/// <summary>
	/// This class implements a widget to edit a lexical expression.
	/// </summary>
	public class SyntacticalExpressionWidget : Alignment
	{
#region Glade widgets
		
		[Widget]
		private Alignment syntacticalExpressionWidgetBase = null;
		
		[Widget]
		private ComboBox expAddItemCombo = null;
		
		[Widget]
		private Label expEdOrLbl =null;
		
		[Widget]
		private Entry expFormatEntry = null;
		
		[Widget]
		private Button expUpBtn =null;
		
		[Widget]
		private Button expDownBtn = null;
		
		[Widget]
		private HBox  expItemsBox =null;
		
		[Widget]
		private ScrolledWindow expItemsScroller = null;
		
#endregion Glade widgets
		
#region Fields
		
		private SyntacticalRuleEditorDialog dialog;
		
#endregion Fields
		
#region Constructors
		
		/// <summary>
		/// <see cref="SyntacticalExpressionWidget"/>'s constructor.
		/// </summary>
		public SyntacticalExpressionWidget(SyntacticalRuleEditorDialog dialog) 
			: base(0, 0, 1, 1)			
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "syntacticalExpressionWidgetBase");
			
			this.dialog =  dialog;			
			
			this.Add(syntacticalExpressionWidgetBase);
			
			this.ShowAll();
		}
		
#endregion Constructors
	}
}
