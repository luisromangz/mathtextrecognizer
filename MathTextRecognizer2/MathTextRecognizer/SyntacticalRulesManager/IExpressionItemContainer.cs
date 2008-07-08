// IExpressionItemContainer.cs created with MonoDevelop
// User: luis at 17:37 17/05/2008

using System;
using Gtk;

namespace MathTextRecognizer.SyntacticalRulesManager
{
	
	/// <summary>
	/// This interface marks those widgets able to contain 
	/// expression items.
	/// </summary>
	public interface IExpressionItemContainer
	{
	


#region Properties
		
		/// <value>
		/// Contains the number of items of the container.
		/// </value>
		int ItemCount
		{
			get;
		}
		
		
		Gtk.Box.BoxChild this[Widget w]
		{
			get;
		}
		
		Window Window
		{
			get;
		}
		
#endregion Properties

#region Methods
		/// <summary>
		/// Adds an item to the item container.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/> to be added.
		/// </param>
		void AddItem(ExpressionItemWidget widget);
		
		/// <summary>
		/// Removes an item from the container.
		/// </summary>
		/// <param name="widget">
		/// The <see cref="ExpressionItemWidget"/> to be removed.
		/// </param>
		void RemoveItem(ExpressionItemWidget widget);
		
			
		/// <summary>
		/// Moves an items towars the container start.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		void MoveItemBackwards(ExpressionItemWidget widget);
		
		/// <summary>
		/// Moves an item towards the container end.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		void MoveItemFordwards(ExpressionItemWidget widget);
		
#endregion Methods
	
	}
}
