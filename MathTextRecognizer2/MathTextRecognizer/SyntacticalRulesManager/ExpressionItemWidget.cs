// IExpressionItemWiget.cs created with MonoDevelop
// User: luis at 17:39 17/05/2008

using System;
using System.Collections.Generic;

using Gtk;

using MathTextCustomWidgets.Dialogs;

using MathTextLibrary.Analisys;

namespace MathTextRecognizer.SyntacticalRulesManager
{
	
	/// <summary>
	/// This class is the base of  all the classes used to show expression
	/// items.
	/// </summary>
	public abstract class ExpressionItemWidget : Alignment
	{
		protected IExpressionItemContainer container;
		
		public event EventHandler HeightRequestChanged;
		
		protected ExpressionItemOptions options;
		
		
		public ExpressionItemWidget(IExpressionItemContainer container) 
			: base(0.5f, 0.5f, 1,1)
		{
			this.container = container;
			
			options = new ExpressionItemOptions();
		}
		
		/// <value>
		/// Contains the widget's item.
		/// </value>
		public abstract ExpressionItem ExpressionItem
		{
			get;
			set;
		}
		
		/// <value>
		/// Contains the position of the item in its container.
		/// </value>
		public int Position
		{
			get
			{
				if(container.GetType() == typeof(ExpressionItemOptionsDialog))
				{
					Widget widget = this;
					
					while(widget.GetType()!= typeof (RelatedItemWidget))
					{
						widget =  widget.Parent;
					}
					
					return container[widget].Position +1;
				}
				else
				{
					return container[this].Position + 1;			
				}
			}
		}
		
		/// <value>
		/// Sets the widget height request.
		/// </value>
		public new int HeightRequest
		{
			get
			{
				return base.HeightRequest;
			}
			set
			{
				base.HeightRequest = value;
				if(HeightRequestChanged !=null)
					HeightRequestChanged(this, EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Checks the widget for validation errors.
		/// </summary>
		/// <returns>
		/// A <see cref="List`1"/> containing the found errors.
		/// </returns>
		public abstract List<string> CheckErrors();
		
		/// <summary>
		/// Sets some widgets attributes that depends on the item widget's 
		/// position.
		/// </summary>
		/// <param name="position">
		/// A <see cref="System.Int32"/>
		/// </param>
		public abstract void CheckPosition();
		
		/// <summary>
		/// Sets the widget in a mode suitable to be shown inside 
		/// a <see cref="RelatedItemWidget"/>.
		/// </summary>
		public abstract void SetRelatedMode();
		
		protected void Remove()
		{
			ResponseType res = 
				ConfirmDialog.Show(this.container.Window,
				                   "¿Realmente quieres eliminar este elemento?");
			
			if(res == ResponseType.No)
				return;
			
			container.RemoveItem(this);
		}
		
		/// <summary>
		/// Moves the item towards the container's end.
		/// </summary>
		protected void MoveFordwards()
		{
			this.container.MoveItemFordwards(this);
		}
		
		/// <summary>
		/// Moves the item towards the container's beginning.
		/// </summary>
		protected void MoveBackwards()
		{
			this.container.MoveItemBackwards(this);
		}
		
		/// <value>
		/// Contains the widget's parent window.
		/// </value>
		public Window Window
		{
			get
			{
				return container.Window;
			}
		}
		
		/// <summary>
		/// Creates a widget based on the type of the item pased.
		/// </summary>
		/// <param name="item">
		/// The <see cref="ExpressionItem"/> that will be contained in the
		/// widget.
		/// </param>
		/// <param name="container">
		/// The container holding the new widget.
		/// </param>
		/// <returns>
		/// The widget holding the given item.
		/// </returns>
		public static ExpressionItemWidget CreateWidget(ExpressionItem item, 
		                                                IExpressionItemContainer container)
		{
			ExpressionItemWidget widget = null;
			
			if(item.GetType() == typeof(ExpressionGroupItem))
			{
				widget= new ExpressionGroupWidget(container);
			}
			else if(item.GetType() == typeof(ExpressionTokenItem))		
			{
				widget =  new ExpressionTokenWidget(container);
			}
					
			if(item.GetType() == typeof(ExpressionRuleCallItem))
			{
				widget =  new ExpressionRuleCallWidget(container);
			}
			
			widget.ExpressionItem = item;
			
			return widget;
		}
		
		/// <summary>
		/// Shows the options dialog.
		/// </summary>
		protected void ShowOptions()
		{
			ExpressionItemOptionsDialog dialog = 
				new ExpressionItemOptionsDialog(this.container.Window , 
				                                this.GetType());
			
			dialog.Options = this.options;
			ResponseType res = dialog.Show();
			if(res == ResponseType.Ok)
			{
				this.options = dialog.Options;
				
			}
			dialog.Destroy();
		}
	}
}
