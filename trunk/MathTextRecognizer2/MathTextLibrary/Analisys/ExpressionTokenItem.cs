// ExpressionTokenItem.cs created with MonoDevelop
// User: luis at 12:57 13/05/2008

using System;
using System.Collections.Generic;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This class implements a expression item to be matched by 
	/// a given <see cref="Token"/> type.
	/// </summary>
	public class ExpressionTokenItem : ExpressionItem
	{
		private string tokenType;	
		
		private bool forceTokenSearch;
		
		private List<ExpressionItem> relatedItems;
		
		
		/// <summary>
		/// <see cref="ExpressionTokenItem"/>'s constructor.
		/// </summary>
		public ExpressionTokenItem() : base()
		{
			forceTokenSearch = false;
			
			relatedItems = new List<ExpressionItem>();		
		}
		
#region Properties
		
		/// <value>
		/// Contains the token type that matches this expression item.
		/// </value>
		public string TokenType 
		{
			get 
			{
				return tokenType;
			}
			set 
			{
				tokenType = value;
			}
		}

		/// <value>
		/// Contains a value indicating if the token must be searched instead
		/// just taking the first token in the sequence.
		/// </value>
		public bool ForceTokenSearch 
		{
			get 
			{
				return forceTokenSearch;
			}
			set
			{
				forceTokenSearch = value;
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
		
#endregion Properties
		
#region Non-public methods
		
		protected override bool MatchSequence (TokenSequence sequence, 
		                                       out string output)
		{
			output ="";
			int idx = 0;
			if(forceTokenSearch)
			{
				idx = sequence.SearchToken(new Token(this.tokenType));
			}		
			
			if(idx==-1 || !this.FirstTokens.Contains(sequence[idx]))
			{
				return false || !IsCompulsory;
			}
			
			output= sequence[idx].Text;
			sequence.RemoveAt(idx);
			return true;
			
		}

		protected override List<Token> CreateFirstTokensSet ()
		{
			List<Token> firstTokens = new List<Token>();
			
			firstTokens.Add(new Token(this.tokenType));
			if(!this.IsCompulsory)
			{
				firstTokens.Add(Token.Empty);
			}
			
			return firstTokens;
		}

		protected override string ToStringAux ()
		{
			
			
			if(relatedItems.Count == 0)
			{
				return this.tokenType;
			}
			else
			{
				string res=this.TokenType + " {";
				
				foreach (ExpressionItem item in relatedItems)
				{
					switch(item.Position)
					{
						case ExpressionItemPosition.Above:
							res+= "↑";
							break;
						case ExpressionItemPosition.Below:
							res+="";
							break;
						case ExpressionItemPosition.Inside:
							res+="↶";
							break;
						case ExpressionItemPosition.RootIndex:
							res+="↖";
							break;						
						case ExpressionItemPosition.SubIndex:
							res+="↘";
							break;						
						case ExpressionItemPosition.SuperIndex:
							res+="↗";
							break;
							
					}
					
					res+= "{"+item.ToString()+"}";
				}
				
				res +="}";
				
				return res;
			}
			
		}
		
		/// <summary>
		/// Tries to match the tokens related items.
		/// </summary>
		/// <param name="index">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="sequence">
		/// A <see cref="TokenSequence"/>
		/// </param>
		private void MatchRelatedItems(int index, TokenSequence sequence)
		{
			
		}

		
#endregion Non-public methods
	}
}
