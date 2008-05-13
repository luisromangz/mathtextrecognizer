// SyntacticExpressionItem.cs created with MonoDevelop
// User: luis at 22:58 12/05/2008

using System;
using System.Collections.Generic;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This enumeration contains the possible kinds of appearance modifiers
	/// applicable to <see cref="ExpressionItem"/> instances.
	/// </summary>
	public enum ExpressionItemModifier
	{
		/// <summary>
		/// There is no modifier.
		/// </summary>
		None,
		/// <summary>
		/// The item is not required to be present.
		/// </summary>
		NonCompulsory,
		/// <summary>
		/// The item will appear one or more times.
		/// </summary>
		Repeating,
		
		/// <summary>
		/// The item may appear one or more times, or not appear even one time.
		/// </summary>
		RepeatingNonCompulsory
		
	}
	
		
	/// <summary>
	/// This enumerate details the positions in which an 
	/// <see cref="ExpresssionItem"/> can be placed with respect to
	/// another.
	/// </summary>
	public enum ExpressionItemPosition
	{
		/// <summary>
		/// The item is placed above the related one.
		/// </summary>
		Above,
		
		/// <summary>
		/// The item will be placed below the related one.
		/// </summary>
		Below,
		
		/// <summary>
		/// The item is a super-index of the related one.
		/// </summary>
		SuperIndex,
		
		/// <summary>
		/// The item is a sub-index of the related one.
		/// </summary>
		SubIndex,
		
		/// <summary>
		/// The item is the index of a root-like symbol item.
		/// </summary>
		RootIndex,
		
		/// <summary>
		/// The expression is inside the related expression.
		/// </summary>
		Inside
	}
	
	/// <summary>
	/// This class is the base for all kinds of expression items.
	/// </summary>
	public abstract class ExpressionItem : ISyntacticMatcher
	{
		private ExpressionItemPosition position;
		private ExpressionItem relatedItem;
		
		private ExpressionItemModifier modifier;
		private List<Token> firstTokens;
		
		private List<ExpressionItem> relatedItems;
		
		/// <summary>
		/// <see cref="ExpressionItem"/>'s constructor
		/// </summary>
		public ExpressionItem()
		{
			relatedItems = new List<ExpressionItem>();			
		}

		
		
#region Properties
		
		/// <value>
		/// Contains the first tokens of the item.
		/// </value>
		public List<Token> FirstTokens
		{
			get
			{
				if (firstTokens == null)
				{
					firstTokens = CreateFirstTokensSet();
				}
				return firstTokens;
			}
		}
		
		/// <value>
		/// Contains a value indicating if the item is compulsory.
		/// </value>
		public bool IsCompulsory
		{
			get
			{
				return (modifier == ExpressionItemModifier.None 
				        || modifier == ExpressionItemModifier.Repeating);
			}
		}
		
		/// <value>
		/// The position of the item with respect the related item position.
		/// </value>
		public ExpressionItemPosition Position 
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

		/// <value>
		/// This item's related item. When assigned, a tree is formed.
		/// </value>
		public ExpressionItem RelatedItem 
		{
			get 
			{
				return relatedItem;
			}
			set 
			{
				relatedItem = value;
				value.relatedItems.Add(this);
			}
		}
		
		/// <value>
		/// Contains the list of items placed in a special position 
		/// relative to this item.
		/// </value>
		public List<ExpressionItem> RelatedItems
		{
			get
			{
				return relatedItems;
			}
		}

		/// <value>
		/// The item's appearance modifier.
		/// </value>
		public ExpressionItemModifier Modifier 
		{
			get 
			{
				return modifier;
			}
			set 
			{
				modifier = value;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Matches a sequence with the expression.
		/// </summary>
		/// <param name="sequence">
		/// A <see cref="TokenSequence"/> to be matched.
		/// </param>
		/// <returns>
		/// The string containing the rule.
		/// </returns>
		public string Match (TokenSequence sequence)
		{
			string res = "";
			switch(modifier)				
			{
			case ExpressionItemModifier.Repeating:
				do
				{
					res += this.MatchSequence(sequence);
				}
				while(FirstTokens.Contains(sequence[0]));
				break;
			case ExpressionItemModifier.RepeatingNonCompulsory:
			
				while(FirstTokens.Contains(sequence[0]))
				{
					res+= this.MatchSequence(sequence);
				}
				break;
			case ExpressionItemModifier.NonCompulsory:
			
				if(FirstTokens.Contains(sequence[0]))
					res+= this.MatchSequence(sequence);
				break;
			
			default:
				res+= this.MatchSequence(sequence);
				break;
			}
			return res;
		}
		
#endregion Public methods
		
#region Non-public methods
		
		protected abstract string MatchSequence(TokenSequence sequence);
				      
				      
		protected abstract List<Token> CreateFirstTokensSet();
		
#endregion Non-public methods
	}
	
}
