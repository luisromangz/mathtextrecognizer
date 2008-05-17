// SyntacticalRulesEditorDialog.cs created with MonoDevelop
// User: luis at 16:16 16/05/2008

using System;
using System.Collections.Generic;

using Gtk;
using Glade;

namespace MathTextRecognizer.SyntacticalRulesManager
{
	
	/// <summary>
	/// This class implements a dialog window to edit syntactical rules.
	/// </summary>
	public class SyntacticalRuleEditorDialog
	{
		
#region Glade widgets
		[Widget]
		private Dialog syntacticalRuleEditorDialog = null;
		
		[Widget]
		private ScrolledWindow synEdExpressionScroller = null;
		
		[Widget]
		private VBox synEdExpressionsVB = null;
		
		[Widget]
		private Entry synEdRuleNameEntry = null;		
		
		
#endregion Glade widgets
		
#region Fields
		
#endregion Fields
		
#region Constructors
		
		/// <summary>
		/// <see cref="SyntacticalRuleEditorDialog"/>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// The dialog's parent <see cref="Window"/>.
		/// </param>
		public SyntacticalRuleEditorDialog(Window parent)
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "syntacticalRuleEditorDialog");
			
			gladeXml.Autoconnect(this);
			
			this.syntacticalRuleEditorDialog.TransientFor = parent;
			
			InitializeWidgets();
		}
		
#endregion Constructors
		
#region Properties
		/// <value>
		/// Contains the box used to store the different expressions.
		/// </value>
		public VBox ExpressionsBox 
		{
			get 
			{
				return synEdExpressionsVB;
			}
		}
		
		/// <value>
		/// Contains the dialog's window.
		/// </value>
		public Window Window
		{
			get
			{
				return syntacticalRuleEditorDialog;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Shows the dialog and waits for a response.
		/// </summary>
		/// <returns>
		/// The dialog's <see cref="ResponseType"/>.
		/// </returns>
		public ResponseType Show()
		{
			// We have to check that the response has meaining.
			ResponseType res;
			do
			{
				res = (ResponseType)(syntacticalRuleEditorDialog.Run());
			}
			while(res == ResponseType.None);
			
			return res;
		}
		
		/// <summary>
		/// Hides the dialog and destroys its resources.
		/// </summary>
		public void Destroy()
		{
			syntacticalRuleEditorDialog.Destroy();
		}
		
		/// <summary>
		/// Removes a expression from the expression list.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="SyntacticalExpressionWidget"/>
		/// </param>
		public void RemoveExpression(SyntacticalExpressionWidget widget)
		{
			
			for(int i = 0; i<synEdExpressionsVB.Children.Length; i++)
			{
				if(synEdExpressionsVB.Children[i] == widget)
				{
					// Whe remove the separator.
					if(i > 0)
					{
						synEdExpressionsVB.Remove(synEdExpressionsVB.Children[i-1]);
					}
					else if(synEdExpressionsVB.Children.Length > 1)
					{
						synEdExpressionsVB.Remove(synEdExpressionsVB.Children[1]);
					}
					
					break;
				}
			}
			
			synEdExpressionsVB.Remove(widget);
		}
		
#endregion Public methods
		
#region Non-public methods
		
		/// <summary>
		/// Adds an expression to the expressions list.
		/// </summary>
		/// <returns>
		/// A <see cref="SyntacticalExpressionWidget"/>
		/// </returns>
		private SyntacticalExpressionWidget AddExpression()
		{
			
			if(synEdExpressionsVB.Children.Length>0)
			{
				HSeparator separator = new HSeparator();
				synEdExpressionsVB.Add(separator);
				separator.Show();
			}
			
			SyntacticalExpressionWidget widget = 
				new SyntacticalExpressionWidget(this);
			
			synEdExpressionsVB.Add(widget);
			
			synEdExpressionScroller.Vadjustment.Value = synEdExpressionScroller.Vadjustment.Upper;
			
			return widget;
		}
		
		/// <summary>
		/// Initializes the dialog's children widgets.
		/// </summary>
		private void InitializeWidgets()
		{
			
		}
		
		/// <summary>
		/// Adds a new expression to the widget.
		/// </summary>
		private void OnSynEdAddExpBtnClicked(object sender, EventArgs args)
		{
			AddExpression();
		}
		
		/// <summary>
		/// Shows a message dialog with info about the dialog usage.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnSynEdInfoBtnClicked(object sender, EventArgs args)
		{
			
		}
		
		/// <summary>
		/// Oks the dialog.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		[GLib.ConnectBefore]
		private void OnSynEdOkBtnClicked(object sender, EventArgs args)
		{
			syntacticalRuleEditorDialog.Respond(ResponseType.Ok);
		}
		
#endregion Non-public methods
	}
}
