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
	public class SyntacticalExpressionWidget : Alignment, IExpressionItemContainer
	{
#region Glade widgets
		
		[Widget]
		private Alignment syntacticalExpressionWidgetBase = null;
				
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
		
		private AddSubItemMenu addSubItemMenu;
		
		
		
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
			
			gladeXml.Autoconnect(this);
			
			this.dialog =  dialog;			
			
			this.Add(syntacticalExpressionWidgetBase);
			
			InitializeWidgets();
			
			this.CanFocus = true;
			
			this.ShowAll();
		}
		
#endregion Constructors
	
#region Properties
		
		/// <value>
		/// Contains the expression's number of items.
		/// </value>
		public int ItemCount 
		{
			get 
			{
				return expItemsBox.Children.Length;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Adds an item to the items container.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void AddItem (ExpressionItemWidget widget)
		{
			expItemsBox.Add(widget);
			
			for(int i=0;i<expItemsBox.Children.Length; i++)
			{
				(expItemsBox.Children[i] as ExpressionItemWidget).CheckPosition(i);
			}
			
			
			widget.HeightRequestChanged +=
				new EventHandler(OnChildWidgetHeightRequestChanged);
			
			// We expand the scroller is it's necessary to show the whole widget.
			if(widget.HeightRequest + 20 > expItemsScroller.HeightRequest)
				expItemsScroller.HeightRequest = widget.HeightRequest + 20;
			
			expItemsScroller.Hadjustment.Value =
				expItemsScroller.Hadjustment.Upper;
		}
		
		/// <summary>
		/// Removes a widget from the container.
		/// </summary>
		/// <param name="widget">
		/// The <see cref="ExpressionItemWidget"/> that will be removed
		/// </param>
		public void RemoveItem(ExpressionItemWidget widget)
		{
			expItemsBox.Remove(widget);
				
			
			if(expItemsBox.Children.Length ==0)
			{
				expItemsScroller.HeightRequest = 85;
			}
			else
			{
				int maxRequest = -1;
				
				for(int i=0;i<expItemsBox.Children.Length; i++)
				{
					ExpressionItemWidget childWidget =
						(expItemsBox.Children[i] as ExpressionItemWidget);
					
					if (maxRequest < childWidget.HeightRequest)
						maxRequest = childWidget.HeightRequest;
					
					childWidget.CheckPosition(i);
				}
			
				
				expItemsScroller.HeightRequest = maxRequest + 20;
			}
		}
		
		/// <summary>
		/// Moves the item towards the expression's end.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void MoveItemFordwards(ExpressionItemWidget widget)
		{
			int position = (expItemsBox[widget] as Gtk.Box.BoxChild).Position;
			expItemsBox.ReorderChild(widget, position+1);
			widget.CheckPosition(position+1);
			
			(expItemsBox.Children[position] as ExpressionItemWidget).CheckPosition(position);
		}
		
		/// <summary>
		/// Moves the widget towards the beginning of the expression.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void MoveItemBackwards(ExpressionItemWidget widget)
		{
			int position = (expItemsBox[widget] as Gtk.Box.BoxChild).Position;
			expItemsBox.ReorderChild(widget, position-1);
			widget.CheckPosition(position-1);
			
			(expItemsBox.Children[position] as ExpressionItemWidget).CheckPosition(position);
		}
		
#endregion Public methods
		
#region Non-public methods
		
		/// <summary>
		/// Intilialize the widget's children widgets.
		/// </summary>
		private void InitializeWidgets()
		{
			addSubItemMenu = new AddSubItemMenu(this);
			
			expItemsScroller.Hadjustment.ValueChanged+= 
				delegate(object sender, EventArgs args)
			{
				expItemsScroller.QueueDraw();
			};
		}
		
		/// <summary>
		/// Shows a menu used to select the kind of item that will be added to 
		/// the expressión.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnExpAddItemBtnClicked(object sender, EventArgs args)
		{
			addSubItemMenu.Popup();
		}
		
		/// <summary>
		/// Removes the expression from the rule.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnExpRemoveBtnClicked(object sender, EventArgs args)
		{
			ResponseType res =  ResponseType.Yes;
				
			if(expItemsBox.Children.Length >0 || 
			   !String.IsNullOrEmpty(expFormatEntry.Text.Trim()))
			{
				res=ConfirmDialog.Show(dialog.Window,
				                   "¿Realmente quieres eliminar la expresión?");			
			}
				
			if(res == ResponseType.No)
				return;
			
			dialog.RemoveExpression(this);
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
			if(expItemsBox.Children.Length ==0)
			{
				expItemsBox.HeightRequest = 90;
			}
			else
			{
				int maxRequest = -1;
				foreach (Widget childWidget in  expItemsBox.Children) 
				{
					if (maxRequest < childWidget.HeightRequest)
						maxRequest = childWidget.HeightRequest;
				}
				
				expItemsScroller.HeightRequest = maxRequest + 20;
				
			}
		}
		
#endregion Non-public methods
	}
}
