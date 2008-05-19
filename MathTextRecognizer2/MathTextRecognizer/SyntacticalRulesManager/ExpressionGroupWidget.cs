// ExpressionGroupWidget.cs created with MonoDevelop
// User: luis at 11:30 18/05/2008

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
	public class ExpressionGroupWidget : 
		ExpressionItemWidget, IExpressionItemContainer
	{
		
#region Glade widgets
		
		[Widget]
		private HBox expressionGroupWidgetBase = null;
		
		[Widget]
		private HBox expGroupItemsBox = null;
		
		[Widget]
		private ScrolledWindow expGroupItemsScroller = null;
		
		[Widget]
		private Button expGroupNextBtn = null;
		
		[Widget]
		private Button expGroupPreviousBtn = null;
		
		[Widget]
		private Button expGroupRmBtn = null;
		
		[Widget]
		private VSeparator expGroupSeparator = null;
		
		[Widget]
		private Entry expGroupFormatEntry = null;
		
		
#endregion Glade widgets
		
#region Fields
		
		private AddSubItemMenu addItemMenu;
		
#endregion Fields
		
		/// <summary>
		/// <see cref="ExpressionRuleCallWidget"/>'s constructor.
		/// </summary>
		/// <param name="container">
		/// A <see cref="IExpressionItemContainer"/>
		/// </param>
		public ExpressionGroupWidget(IExpressionItemContainer container) 
			: base(container)
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "expressionGroupWidgetBase");
			
			gladeXml.Autoconnect(this);
			
			this.Add(expressionGroupWidgetBase);
			
			this.HeightRequest = expressionGroupWidgetBase.HeightRequest;
			
			addItemMenu = new AddSubItemMenu(this);
			
			expGroupItemsScroller.Hadjustment.ValueChanged +=
				delegate(object sender, EventArgs args)
			{
				expGroupItemsScroller.QueueDraw();
			};
			
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
				ExpressionGroupItem res = new ExpressionGroupItem();
				return res;
			}
			set 
			{
				if(value.GetType() != typeof(ExpressionGroupItem))
				{
					throw new ArgumentException("The type of the value wasn't ExpressionGroupItem");
				}
			}
		}

		/// <value>
		/// Contains the number of children that this contains.
		/// </value>
		public int ItemCount 
		{
			get 
			{
				return expGroupItemsBox.Children.Length;
			}
		}
		
		/// <summary>
		/// Contains the <see cref="BoxChild"/> associated to a widget.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public new Gtk.Box.BoxChild this[Widget w]
		{
			get
			{
				return expGroupItemsBox[w] as Gtk.Box.BoxChild;
			}
		}
		

		
		
#endregion Properties	
		
#region Public methods
		
		
		/// <summary>
		/// Adds an item widget to the group.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void AddItem (ExpressionItemWidget widget)
		{
			expGroupItemsBox.Add(widget);
			
			foreach (ExpressionItemWidget childWidget in  expGroupItemsBox) 
			{
				childWidget.CheckPosition();
			}
		
			CheckHeight();
			
			expGroupItemsScroller.Hadjustment.Value =
				expGroupItemsScroller.Hadjustment.Upper;
			
			
				
			widget.HeightRequestChanged += 
				new EventHandler(OnChildWidgetHeightRequestChanged);
				
			
			
		}
		
		/// <summary>
		/// Sets the widget in a mode suitable to be shown inside 
		/// a <see cref="RelatedItemWidget"/>.
		/// </summary>
		public override void SetRelatedMode ()
		{
			expGroupNextBtn.Visible =false;
			expGroupPreviousBtn.Visible = false;
			expGroupRmBtn.Visible = false;
			
			
			
			expGroupSeparator.Visible = false;
		}

		
		
		/// <summary>
		/// Remove an item from this container.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void RemoveItem (ExpressionItemWidget widget)
		{
			expGroupItemsBox.Remove(widget);
			
			foreach (ExpressionItemWidget childWidget in expGroupItemsBox) 
			{
				childWidget.CheckPosition();
			}
			
			CheckHeight();
			
		}
		
		/// <summary>
		/// Moves the widget towards the end of the item group.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void MoveItemFordwards(ExpressionItemWidget widget)
		{
			int position = (expGroupItemsBox[widget] as Gtk.Box.BoxChild).Position;
			expGroupItemsBox.ReorderChild(widget, position+1);
			widget.CheckPosition();
			
			(expGroupItemsBox.Children[position] as ExpressionItemWidget).CheckPosition();
			
		}
		
		/// <summary>
		/// Moves the widget towards the start of the item group.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void MoveItemBackwards(ExpressionItemWidget widget)
		{
			int position = (expGroupItemsBox[widget] as Gtk.Box.BoxChild).Position;
			expGroupItemsBox.ReorderChild(widget, position-1);
			widget.CheckPosition();
			
			(expGroupItemsBox.Children[position] as ExpressionItemWidget).CheckPosition();
		}
		
		/// <summary>
		/// Checks the position of the widget, and sets some properties
		/// depending on that.
		/// </summary>
		/// <param name="position">
		/// A <see cref="System.Int32"/>
		/// </param>
		public override void CheckPosition ()
		{
			int position = container[this].Position;
			expGroupNextBtn.Sensitive = position < container.ItemCount -1;
			expGroupSeparator.Visible = position < container.ItemCount -1;
			
			expGroupPreviousBtn.Sensitive =  position > 0;
		}
			
		
#endregion Public methods
		
#region Non-public methods


		/// <summary>
		/// Sets the heigth of the widget.
		/// </summary>
		private void CheckHeight()
		{
			if(expGroupItemsBox.Children.Length ==0)
			{
				this.HeightRequest = 165;
				expGroupItemsBox.HeightRequest = 130;
			}
			else
			{
				int maxRequest = -1;
				foreach (Widget childWidget in  expGroupItemsBox.Children) 
				{
					if (maxRequest < childWidget.HeightRequest)
						maxRequest = childWidget.HeightRequest;
				}
				
				this.HeightRequest = maxRequest + 75;
				expGroupItemsScroller.HeightRequest = maxRequest + 40;
			}
			
			
		}
		
		/// <summary>
		/// Removes the expression token widget from its container.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected void OnExpGroupRmBtnClicked(object sender, EventArgs args)
		{
			Remove();
		}
		
		/// <summary>
		/// Shows the contextual menu that creates the children item widget.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		protected void OnExpGroupAddItemBtnClicked(object sender, EventArgs args)
		{
			addItemMenu.Popup();
		}

		
		
		/// <summary>
		/// Changes the height of the container based on the height of its
		/// children, when a height request is detected.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnChildWidgetHeightRequestChanged(object sender,
		                                               EventArgs args)
		{
			CheckHeight();
		}
		
		private void OnExpGroupNextBtnClicked(object sender, EventArgs args)
		{
			this.MoveFordwards();
		}
		
		private void OnExpGroupPreviousBtnClicked(object sender, EventArgs args)
		{
			this.MoveBackwards();
		}
		
		private void OnExpGroupOptionsBtnClicked(object sender, EventArgs args)
		{
			ExpressionItemOptionsDialog dialog =
				new ExpressionItemOptionsDialog(container.Window,
				                                typeof(ExpressionGroupItem));
			
			dialog.Show();
			dialog.Destroy();
		}

#endregion Non-public methods
	}
}