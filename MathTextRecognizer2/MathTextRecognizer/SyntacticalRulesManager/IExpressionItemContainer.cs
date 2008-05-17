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
		/// <summary>
		/// Adds an item to the item container.
		/// </summary>
		void AddItem(ExpressionItemWidget widget);
		
		void RemoveItem(ExpressionItemWidget widget);
		
		/// <value>
		/// Contains the number of items of the container.
		/// </value>
		int ItemCount
		{
			get;
		}
	}
}
