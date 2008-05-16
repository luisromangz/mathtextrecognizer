// LexicalRuleEditorDialog.cs created with MonoDevelop
// User: luis at 13:06 29/04/2008

using System;
using System.Collections.Generic;
using Gtk;
using Glade;

using MathTextCustomWidgets.Dialogs;
using MathTextLibrary.Analisys;

namespace MathTextRecognizer.LexicalRulesManager
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
			
			lexicalRuleEditorDialog.ShowAll();
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
				res.Name = ruleNameEntry.Text.Trim();
				
				foreach (LexicalExpressionWidget widget 
				         in expressionsVB.AllChildren ) 
				{
					res.LexicalExpressions.Add(widget.Expression);
				}
				return res;
			}
			
			set
			{
				ruleNameEntry.Text = value.Name;
				
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
				new LexicalExpressionWidget(this.lexicalRuleEditorDialog,
				                            expressionsVB);
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
			
			bool regExErrors = false;
			
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
				for(int i =0; i < expressionsVB.Children.Length; i++) 
				{
					
					LexicalExpressionWidget widget = 
						(LexicalExpressionWidget)(expressionsVB.Children[i]);
					
					if(String.IsNullOrEmpty(widget.Expression.Trim()))
					{
						errors.Add(String.Format("· La expresión {0} está en blanco.", i +1));
						break;
					}
					else
					{
						// We are gonna check if the expression 
						// is well formed, the hard way.
						try
						{
							System.Text.RegularExpressions.Regex.IsMatch("meh",widget.Expression.Trim());
						}
						catch(Exception)
						{
							errors.Add(String.Format("· La expresión {0} no es una expressión regular de .Net/Mono válida.", i+1));
							regExErrors = true;
						}
						
					}
				}				
			}
			
				
			if(errors.Count > 0)		
			{
				string errorss = string.Join("\n", errors.ToArray());
				OkDialog.Show(lexicalRuleEditorDialog,
				              MessageType.Warning,
				              "Para continuar, debes solucionar los siguentes problemas:\n\n{0}{1}",
				             errorss,
				              regExErrors?"\n\nPara mas información sobre el formato de las expresiones regulares, visite http://msdn.microsoft.com/es-es/library/hs600312(VS.80).aspx.":"");
				
				
				lexicalRuleEditorDialog.Respond(ResponseType.None);
			}
			else
			{				
				lexicalRuleEditorDialog.Respond(ResponseType.Ok);
			}
		}
		
		/// <summary>
		/// Shows a message box with info about the dialog.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnInfoBtnClicked(object sender, EventArgs args)
		{
			OkDialog.Show(this.lexicalRuleEditorDialog,
			              MessageType.Info,
			             "Aquí puedes editar una regla de análisis léxico. En la "
			              +"zona izquierda se establece el nombre de la regla (que"
			              +" será el tipo del item creado a partir de la misma) y"
			              +" en la zona derecha el conjunto de expresiones que la"
			              +" generarán.");
			
			
		}
				
#endregion Non-public methods		

	}
	
	
}
