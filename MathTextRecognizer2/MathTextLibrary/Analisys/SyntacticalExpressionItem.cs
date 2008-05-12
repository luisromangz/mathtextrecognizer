// SyntacticExpressionItem.cs created with MonoDevelop
// User: luis at 22:58 12/05/2008

using System;
using System.Collections.Generic;

namespace MathTextLibrary.Analisys
{
	/// <summary>
	/// This enumerate type defines the different types of expreission items
	/// avalaible.
	/// </summary>
	public enum SyntacticalExpressionItemType
	{
		Token,
		Rule
	}
	
	/// <summary>
	/// This enumerate details the positions in which an 
	/// <see cref="SyntacticalExpresssionItem"/> can be placed with respect to
	/// another.
	/// </summary>
	public enum SyntacticalExpressionItemPosition
	{
		Above,
		Below,
		Superindex,
		Subindex,
		Index
	}
	
	/// <summary>
	/// This class implements the elements present in
	/// <see cref="SyntacticExpresion"/>s.
	/// </summary>
	public class SyntacticalExpressionItem
	{
		private SyntaticExpressionItemType type;
		private string label;
		private SyntacticalExpressionItemPosition position;
		private List<SyntacticalExpressionItem> relatedItems; 
		
		
		public SyntacticalExpressionItem()
		{
			relatedItems = new List<SyntacticalExpressionItem>();
		}
		
#region Properties
		
		/// <value>
		/// Contains the kind of the expression represented by the item.
		/// </value>
		public SyntacticalExpressionItemType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		/// <value>
		/// Contains the label for the expression item, it may be the token's
		/// type, or the expression's identifier.
		/// </value>
		public string Label 
		{
			get 
			{
				return label;
			}
			set 
			{
				label = value;
			}
		}

		/// <value>
		/// Contains the position of the item relative to its related item.
		/// </value>
		public SyntacticalExpressionItemPosition Position 
		{
			get 
			{
				return position;
			}
			set 
			{
				position = value;
			}
		}
		
#endregion Properties
	}
}
