// ExpressionItemWIdget.cs created with MonoDevelop
// User: luis at 17:52 17/05/2008

using System;
using System.Collections.Generic;

using Gtk;
using Glade;

using MathTextLibrary.Analisys;

namespace MathTextRecognizer.SyntacticalRulesManager
{
	
	/// <summary>
	/// This class implements a widget to set a token expression item.
	/// </summary>
	public class ExpressionTokenWidget : ExpressionItemWidget
	{
		
#region Glade widgets
		
		[Widget]
		private Entry expTokenTypeEntry = null;
		
		[Widget]
		private HBox expressionTokenWidgetBase = null;
		
		
		
#endregion Glade widgets
		
#region Fields
		
#endregion Fields
		
		/// <summary>
		/// <see cref="ExpressionTokenWidget"/>'s constructor.
		/// </summary>
		/// <param name="container">
		/// A <see cref="IExpressionItemContainer"/>
		/// </param>
		public ExpressionTokenWidget(IExpressionItemContainer container) 
			: base(container)
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "expressionTokenWidgetBase");
			
			gladeXml.Autoconnect(this);
			
			this.Add(expressionTokenWidgetBase);
			
			this.HeightRequest = expressionTokenWidgetBase.HeightRequest;
			
			this.ShowAll();
		}
		
#region Properties
		/// <value>
		/// Contains the widget's item.
		/// </value>
		public override ExpressionItem ExpressionItem 
		{
			get 
			{
				ExpressionTokenItem res = new ExpressionTokenItem();
				res.TokenType = expTokenTypeEntry.Text.Trim();
				
				return res;
			}
			set 
			{
				if(value.GetType() != typeof(ExpressionTokenItem))
				{
					throw new ArgumentException("The type of the value wasn't ExpressionTokenItem");
				}
				
				expTokenTypeEntry.Text = (value as ExpressionTokenItem).TokenType;
			}
		}
		
		
#endregion Properties	
		
#region Non-public methods
		
		/// <summary>
		/// Removes the expression token widget from its container.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected void OnExpTokenRmBtnClicked(object sender, EventArgs args)
		{
			container.RemoveItem(this);
		}
		
#endregion Non-public methods
		
	}
}
