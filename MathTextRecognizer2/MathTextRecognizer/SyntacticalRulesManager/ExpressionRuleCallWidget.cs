// ExpressionRuleCallWidget.cs created with MonoDevelop
// User: luis at 11:15 18/05/2008

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
	public class ExpressionRuleCallWidget : ExpressionItemWidget
	{
		
#region Glade widgets
		
		[Widget]
		private Entry expTokenTypeEntry = null;
		
		[Widget]
		private HBox expressionRuleWidgetBase = null;
		
		[Widget]
		private Button expRuleNextBtn = null;
		
		[Widget]
		private Button expRulePreviousBtn = null;
		
		[Widget]
		private VSeparator expRuleSeparator = null;
		
		[Widget]
		private VBox expRuleButtonsBox = null;
		
#endregion Glade widgets
		
#region Fields
		
#endregion Fields
		
		/// <summary>
		/// <see cref="ExpressionRuleCallWidget"/>'s constructor.
		/// </summary>
		/// <param name="container">
		/// A <see cref="IExpressionItemContainer"/>
		/// </param>
		public ExpressionRuleCallWidget(IExpressionItemContainer container) 
			: base(container)
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "expressionRuleWidgetBase");
			
			gladeXml.Autoconnect(this);
			
			this.Add(expressionRuleWidgetBase);
			
			this.HeightRequest = expressionRuleWidgetBase.HeightRequest;
			
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
				ExpressionRuleCallItem res = new ExpressionRuleCallItem();
				res.RuleName = expTokenTypeEntry.Text.Trim();
				
				return res;
			}
			set 
			{
				if(value.GetType() != typeof(ExpressionRuleCallItem))
				{
					throw new ArgumentException("The type of the value wasn't ExpressionRuleCallItem");
				}
				
				expTokenTypeEntry.Text = (value as ExpressionTokenItem).TokenType;
			}
		}
		
		
#endregion Properties	

#region Public methods
		
		/// <summary>
		/// Checks the item's position, and sets some controls consecuently.
		/// </summary>
		/// <param name="position">
		/// A <see cref="System.Int32"/>
		/// </param>
		public override void CheckPosition()
		{
			int position = container[this].Position;
			expRuleNextBtn.Sensitive = position < container.ItemCount -1;
			expRuleSeparator.Visible = position < container.ItemCount -1;
			
			expRulePreviousBtn.Sensitive =  position > 0;
			
			
		}
		
		/// <summary>
		/// Sets the widget in a mode suitable to be shown inside 
		/// a <see cref="RelatedItemWidget"/>.
		/// </summary>
		public override void SetRelatedMode ()
		{
			expRuleButtonsBox.Visible = false;
			expRuleSeparator.Visible = false;
		}

		
		
#endregion Public methods
		
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
		protected void OnExpRuleRmBtnClicked(object sender, EventArgs args)
		{
			container.RemoveItem(this);
		}
		
		protected void OnExpRuleNextBtnClicked(object sender, EventArgs args)
		{
			this.MoveFordwards();
		}
		
		protected void OnExpRulePreviousBtnClicked(object sender, EventArgs args)
		{
			this.MoveBackwards();
		}
		
		protected void OnExpRuleOptionsBtnClicked(object sender, EventArgs args)
		{
			ExpressionItemOptionsDialog dialog =
				new ExpressionItemOptionsDialog(this.container.Window,
				                                typeof(ExpressionRuleCallItem));
			
			dialog.Show();
			
			dialog.Destroy();
		}
		
#endregion Non-public methods
		
	}
}
