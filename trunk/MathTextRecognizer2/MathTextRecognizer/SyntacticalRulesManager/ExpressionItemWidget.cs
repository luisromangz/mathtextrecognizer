// IExpressionItemWiget.cs created with MonoDevelop
// User: luis at 17:39 17/05/2008

using System;
using System.Collections.Generic;

using Gtk;

using MathTextLibrary.Analisys;

namespace MathTextRecognizer.SyntacticalRulesManager
{
	
	/// <summary>
	/// This interface is used to mark all the classes used to show expression
	/// items.
	/// </summary>
	public abstract class ExpressionItemWidget : Alignment
	{
		protected IExpressionItemContainer container;
		
		
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
		
		
	}
}
