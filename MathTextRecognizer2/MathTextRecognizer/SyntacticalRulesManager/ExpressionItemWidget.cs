// IExpressionItemWiget.cs created with MonoDevelop
// User: luis at 17:39 17/05/2008

using System;
using System.Collections.Generic;

using Gtk;

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
		
		
		public ExpressionItemWidget(IExpressionItemContainer container) 
			: base(0.5f, 0.5f, 1,1)
		{
			this.container = container;
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
		
		/// <summary>
		/// Moves the item towards the container's end.
		/// </summary>
		public void MoveFordwards()
		{
			this.container.MoveItemFordwards(this);
		}
		
		/// <summary>
		/// Moves the item towards the container's beginning.
		/// </summary>
		public void MoveBackwards()
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
	}
}
