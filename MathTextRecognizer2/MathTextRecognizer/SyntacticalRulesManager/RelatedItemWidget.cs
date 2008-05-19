// RelatedItemWidget.cs created with MonoDevelop
// User: luis at 16:00 19/05/2008

using System;

using Gtk;
using Glade;

using MathTextLibrary.Analisys;

namespace MathTextRecognizer.SyntacticalRulesManager
{
	
	/// <summary>
	/// This class implements a widget used to contain a related item of 
	/// a token.
	/// </summary>
	public class RelatedItemWidget : ExpressionItemWidget
	{
		
#region Glade widgets
		
		[Widget]
		private Alignment relatedItemPlaceholder =null;
		
		[Widget]
		private Alignment relatedItemWidgetBase = null;
		
		[Widget]
		private Button relatedItemUpBtn = null;
		
		[Widget]
		private Button relatedItemDownBtn = null;
		
		[Widget]
		private HSeparator relatedItemSeparator = null;
		
		[Widget]
		private ComboBox relatedItemPositionCombo = null;
		
#endregion Glade widgets
		
		
#region Fields
		private ExpressionItemWidget itemWidget;
		
#endregion Fields
		
		public RelatedItemWidget(ExpressionItemWidget itemWidget, 
		                         IExpressionItemContainer container)
			:base(container)
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "relatedItemWidgetBase");
			
			gladeXml.Autoconnect(this);
			
			this.itemWidget = itemWidget;
			
			this.Add(relatedItemWidgetBase);
			
			
			this.relatedItemPlaceholder.Add(itemWidget);
			
			this.ShowAll();
		}
		
		
#region Properties
		
		
		/// <value>
		/// Contains the item's expression item.
		/// </value>
		public override ExpressionItem ExpressionItem 
		{
			get 
			{
				itemWidget.ExpressionItem.Position =
					(ExpressionItemPosition)(relatedItemPositionCombo.Active);
				return itemWidget.ExpressionItem;
			}
			set 
			{  
				itemWidget.ExpressionItem = value;
				relatedItemPositionCombo.Active = (int)(value.Position);
			}
		}
		
		/// <value>
		/// Contains the actual widget containing the expression item.
		/// </value>
		public ExpressionItemWidget ItemWidget 
		{
			get 
			{
				return itemWidget;
			}
			set 
			{
				itemWidget = value;
			}
		}
		
		
		
#endregion Properties
		
			
#region Public methods
		
		/// <summary>
		/// Checks the widget's position and changes some widget's properties
		/// depending on that.
		/// </summary>
		public override void CheckPosition ()
		{
			int position = container[this].Position;
			
			relatedItemUpBtn.Sensitive =  position > 0;
			relatedItemDownBtn.Sensitive =  position < container.ItemCount-1;
			relatedItemSeparator.Visible = position < container.ItemCount -1;
		}


		public override void SetRelatedMode ()
		{
			throw new NotImplementedException ();
		}
	
#endregion Public methods
		
#region Non-public methods
		
		protected void OnRelatedItemRmBtnClicked(object sender, EventArgs args)
		{
			Remove();
		}
		
		protected void OnRelatedItemUpBtnClicked(object sender, EventArgs args)
		{
			this.MoveBackwards();
		}
		
		protected void OnRelatedItemDownBtnClicked(object sender, EventArgs args)
		{
			this.MoveFordwards();
		}
		
#endregion Non-public methods
	}
	


}
