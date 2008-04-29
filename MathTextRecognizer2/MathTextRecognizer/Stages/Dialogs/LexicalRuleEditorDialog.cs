// LexicalRuleEditorDialog.cs created with MonoDevelop
// User: luis at 13:06 29/04/2008

using System;
using System.Collections.Generic;
using Gtk;
using Glade;

using MathTextCustomWidgets.Dialogs;
using MathTextLibrary.Analisys.Lexical;

namespace MathTextRecognizer
{
	
	/// <summary>
	/// Implements a dialog which purpose is to edit and create 
	/// instances of the <c>LexicalRule</c> class.
	/// </summary>
	public class LexicalRuleEditorDialog
	{
#region Glade widgets
		[WidgetAttribute]
		private Dialog lexicalRuleEditorDialog = null;
		
		[WidgetAttribute]
		private Entry ruleNameEntry = null;
		
		[WidgetAttribute]
		private VBox expressionsVB = null;
		
		[WidgetAttribute]
		private ScrolledWindow expressionsSW = null;
		
#endregion Glade widgets
		
		/// <summary>
		/// <c>LexicalRuleEditorDialog</c>'s constructor method.
		/// </summary>
		/// <param name="parent">
		/// The window this dialog will be modal to.
		/// </param>
		public LexicalRuleEditorDialog(Window parent)
		{
			XML gxml = new XML(null,
			                   "mathtextrecognizer.glade",
			                   "lexicalRuleEditorDialog",
			                   null);
			gxml.Autoconnect(this);
			
			lexicalRuleEditorDialog.TransientFor = parent;
			
			AddExpression("");
			
		}
		
#region Properties
		
		/// <value>
		/// Contains the rule represented edited by the dialog.
		/// </value>
		public LexicalRule Rule
		{
			get
			{
				
				LexicalRule res = new LexicalRule();
				res.RuleName = ruleNameEntry.Text.Trim();
				
				foreach (LexicalExpressionWidget widget 
				         in expressionsVB.AllChildren ) 
				{
					res.LexicalExpressions.Add(widget.Expression);
				}
				return res;
			}
			
			set
			{
				ruleNameEntry.Text = value.RuleName;
				
				// We clear the vertical box.
				foreach (LexicalExpressionWidget widget 
				         in expressionsVB.AllChildren ) 
				{
					widget.Destroy();
				}
				
				foreach (string expression in value.LexicalExpressions) 
				{
					AddExpression(expression);
				}
			}
		}
		
#endregion Properties
				
#region Public methods
		
		/// <summary>
		/// Shows the dialog and waits for its response.
		/// </summary>
		/// <returns>
		/// A <see cref="ResponseType"/>
		/// </returns>
		public ResponseType Show()
		{		
			ResponseType res = ResponseType.None;
			// We wait until there is a sensible response.
			while(res == ResponseType.None)
			{
				res = (ResponseType)(lexicalRuleEditorDialog.Run());
			}
			
			return res;
		}
		
		/// <summary>
		/// Destroys the dialog's resources, hiding it.
		/// </summary>
		public void Destroy()
		{
			lexicalRuleEditorDialog.Destroy();			
		}
				
#endregion Public methods
				
#region Non-public methods
		
		/// <summary>
		/// Adds an expressión to the expression list.
		/// </summary>
		/// <param name="expression">
		/// The expression to be added.
		/// </param>
		private void AddExpression(string expression)
		{
			LexicalExpressionWidget widget = 
				new LexicalExpressionWidget(this.lexicalRuleEditorDialog);
			widget.Expression = expression;
			
			expressionsVB.Add(widget);
		}
		
		/// <summary>
		/// Adds a new expression widget when the add button is clicked.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnAddExpressionBtnClicked(object sender, EventArgs args)
		{
			AddExpression("");
			expressionsSW.Vadjustment.Value = expressionsSW.Vadjustment.Upper;
		}
		
		/// <summary>
		/// Responds an ok if there aren't validation errors, none otherwise.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="EventArgs">
		/// A <see cref="EventArgs"/>
		/// </param>
		[GLib.ConnectBeforeAttribute]
		private void OnOkBtnClicked(object sender, EventArgs args)
		{
			List<string> errors = new List<string>();
			
			if(String.IsNullOrEmpty(ruleNameEntry.Text.Trim()))
			{
				errors.Add("· La regla necesita un nombre.");				
			}
				
			if(expressionsVB.Children.Length == 0)
			{
				errors.Add("· Debes añadir al menos una expresión a la regla.");
			}
			else
			{
				foreach (Widget w in expressionsVB.Children) 
				{
					LexicalExpressionWidget widget = (LexicalExpressionWidget)w;
					if(String.IsNullOrEmpty(widget.Expression.Trim()))
					{
						errors.Add("· Hay expresiones en blanco.");
						break;
					}
				}				
			}
			
				
			if(errors.Count > 0)		
			{
				string errorss = string.Join("\n", errors.ToArray());
				OkDialog.Show(lexicalRuleEditorDialog,
				              MessageType.Warning,
				              "Para continuar, debes solucionar los siguentes problemas:\n\n{0}",
				             errorss);
				
				lexicalRuleEditorDialog.Respond(ResponseType.None);
			}
			else
			{
				
				lexicalRuleEditorDialog.Respond(ResponseType.Ok);
			}
		}
				
#endregion Non-public methods
		
		
#region LexicalExpressionWidget class
		/// <summary>
		/// This internal class implements the expression editor widget.
		/// </summary>
		public class LexicalExpressionWidget : Alignment
		{
			[WidgetAttribute]
			private Entry expressionEntry = null;
			
			[WidgetAttribute]
			private Frame expressionFrame = null;
			
			private Window parentWindow;
			
			/// <summary>
			/// <c>LexicalExpressionWidget</c>'s constructor.
			/// </summary>
			public LexicalExpressionWidget(Window parentWindow) 
				: base(0, 0.5f, 0, 0)
			{
				XML gxml = new XML(null, 
				                   "mathtextrecognizer.glade",
				                   "expressionFrame",
				                   null);
				
				gxml.Autoconnect(this);
				
				this.parentWindow = parentWindow;
				
				this.Add(expressionFrame);
				
				this.expressionEntry.IsFocus = true;
				
				this.ShowAll();
			}
			
			
			/// <value>
			/// Contains the widget's expression.
			/// </value>
			public string Expression
			{
				
				get
				{
					return expressionEntry.Text.Trim();					
				}
				set
				{
					expressionEntry.Text = value;
				}
			}
			
			/// <summary>
			/// Moves the widget one position before the actual.
			/// </summary>
			private void OnUpBtnClicked(object sender, EventArgs args)
			{
				
			}
			
			/// <summary>
			/// Moves the widget one position after the actual.
			/// </summary>
			private void OnDownBtnClicked(object sender, EventArgs args)
			{
				
			}
			
			
			
			/// <summary>
			/// Removes the widget from the list.
			/// </summary>
			private void OnRemoveBtnClicked(object sender, EventArgs args)
			{
				ResponseType res = 
					ConfirmDialog.Show(parentWindow,
					                   "¿Quieres eliminar la expresión?");
				
				if(res == ResponseType.Yes)
				{
					this.Destroy();
				}
			}
		}
		
#endregion LexicalExpressionWidget class
	}
	
	
}
