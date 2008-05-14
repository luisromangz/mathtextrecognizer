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
		
		/// <summary>
		/// <see cref="ExpressionTokenItem"/>'s constructor.
		/// </summary>
		public ExpressionTokenItem() : base()
		{
			forceTokenSearch = false;
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
		
#endregion Properties
		
#region Non-public methods
		
		protected override bool MatchSequence (TokenSequence sequence, 
		                                       out string output)
		{
			Console.WriteLine("Token Item {0}, ({1})", this.ToString(), sequence.ToString());
			output ="";
			int idx = 0;
			if(forceTokenSearch)
			{
				idx = sequence.SearchToken(new Token(this.tokenType));
			}		
			
			if(idx==-1 || !this.FirstTokens.Contains(sequence[idx]))
			{
				Console.WriteLine("meh {0}", this.FirstTokens[0].Type);
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
			return this.tokenType;
		}

		
#endregion Non-public methods
	}
}
