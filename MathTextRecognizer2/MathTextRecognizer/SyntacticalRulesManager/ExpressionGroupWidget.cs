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
		private VSeparator expGroupSeparator = null;
		
		
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
			
			
			
			for(int i=0;i<expGroupItemsBox.Children.Length; i++)
			{
				(expGroupItemsBox.Children[i] as ExpressionItemWidget).CheckPosition(i);
			}
		
			CheckHeight();
			
			expGroupItemsScroller.Hadjustment.Value =
				expGroupItemsScroller.Hadjustment.Upper;
			
			
				
			widget.HeightRequestChanged += 
				new EventHandler(OnChildWidgetHeightRequestChanged);
				
			
			
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
			
			for(int i=0;i<expGroupItemsBox.Children.Length; i++)
			{
				ExpressionItemWidget childWidget =
					(expGroupItemsBox.Children[i] as ExpressionItemWidget);
			
				childWidget.CheckPosition(i);
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
			widget.CheckPosition(position+1);
			
			(expGroupItemsBox.Children[position] as ExpressionItemWidget).CheckPosition(position);
			
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
			widget.CheckPosition(position-1);
			
			(expGroupItemsBox.Children[position] as ExpressionItemWidget).CheckPosition(position);
		}
		
		/// <summary>
		/// Checks the position of the widget, and sets some properties
		/// depending on that.
		/// </summary>
		/// <param name="position">
		/// A <see cref="System.Int32"/>
		/// </param>
		public override void CheckPosition (int position)
		{
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
			container.RemoveItem(this);
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

		
#endregion Non-public methods
	}
}