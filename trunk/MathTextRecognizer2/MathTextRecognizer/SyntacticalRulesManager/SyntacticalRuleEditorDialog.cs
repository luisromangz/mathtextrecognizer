// SyntacticalRulesEditorDialog.cs created with MonoDevelop
// User: luis at 16:16 16/05/2008

using System;
using System.Collections.Generic;

using Gtk;
using Glade;


using MathTextLibrary.Analisys;
using MathTextCustomWidgets.Dialogs;

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
		private SyntacticalRulesManagerDialog manager;
		
		private bool editing;
		
		private string oldRuleName;
#endregion Fields
		
#region Constructors
		
		/// <summary>
		/// <see cref="SyntacticalRuleEditorDialog"/>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// The dialog's parent <see cref="Window"/>.
		/// </param>
		public SyntacticalRuleEditorDialog(SyntacticalRulesManagerDialog parent)
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "syntacticalRuleEditorDialog");
			
			gladeXml.Autoconnect(this);
			
			manager = parent;
			
			this.syntacticalRuleEditorDialog.TransientFor = manager.Window;
			
			editing = false;
			
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
		
		/// <value>
		/// 
		/// </value>
		public SyntacticalRule Rule
		{
			get
			{
				SyntacticalRule rule = new SyntacticalRule();
				rule.Name =  synEdRuleNameEntry.Text.Trim();
				
				
				foreach (SyntacticalExpressionWidget widget in 
				         synEdExpressionsVB.Children) 
				{
					rule.Expressions.Add(widget.Expression);
				}
				
				return rule;
			}
			set
			{
				// We tell the dialog that we are in edit mode.
				editing = true;
				synEdRuleNameEntry.Text = value.Name;
				oldRuleName = value.Name;
				
				foreach (SyntacticalExpression expression in value.Expressions) 
				{
					SyntacticalExpressionWidget expWidget =
						this.AddExpression();
					
					expWidget.Expression = expression;
				}
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
			synEdExpressionsVB.Remove(widget);
			
			foreach (SyntacticalExpressionWidget childWidget in 
			         synEdExpressionsVB.Children) 
			{
				childWidget.CheckPosition();
			}
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
			
			SyntacticalExpressionWidget widget = 
				new SyntacticalExpressionWidget(this);
			
			synEdExpressionsVB.Add(widget);
			
			foreach (SyntacticalExpressionWidget expWidget in synEdExpressionsVB) 
			{
				expWidget.CheckPosition();
			}
			
			
			synEdExpressionScroller.Vadjustment.Value = synEdExpressionScroller.Vadjustment.Upper;
			
			return widget;
			
		}
		
		/// <summary>
		/// Initializes the dialog's children widgets.
		/// </summary>
		private void InitializeWidgets()
		{
			synEdExpressionScroller.Vadjustment.ValueChanged+=
				delegate (object sender, EventArgs args)
			{
				synEdExpressionScroller.QueueDraw();
			};
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
			List<string> errors = new List<string>();
			
			string ruleName = synEdRuleNameEntry.Text.Trim();
			if(String.IsNullOrEmpty(ruleName))
			{
				errors.Add("· No se ha establecido un nombre para la regla");
			}
			else if(!editing || oldRuleName != ruleName)
			{
				if(manager.ExistsRuleName(ruleName))
				{
					errors.Add("· Ya existe una regla con el nombre usado.");
				}
			}
			
			if(synEdExpressionsVB.Children.Length == 0)
			{
				errors.Add("· No se ha definido ninguna expresión para la regla.");
			}
			else
			{
				foreach (SyntacticalExpressionWidget widget in 
			         synEdExpressionsVB.Children) 
				{
					List<string> expressionErrors = widget.CheckErrors();
					
					errors.AddRange(expressionErrors);
				}
			}
			
			if(errors.Count>0)
			{
				// There were validations errors, we inform the user.
				string errorString = String.Join("\n", errors.ToArray());
				
				OkDialog.Show(this.Window,
				              MessageType.Info,
				              "Para continuar, debes solventar los siguientes errores:\n\n{0}",
				              errorString);
				
				syntacticalRuleEditorDialog.Respond(ResponseType.None);
				
				synEdExpressionScroller.QueueDraw();
				
			}
			else
			{
				syntacticalRuleEditorDialog.Respond(ResponseType.Ok);
			}
		}
		
#endregion Non-public methods
	}
}
