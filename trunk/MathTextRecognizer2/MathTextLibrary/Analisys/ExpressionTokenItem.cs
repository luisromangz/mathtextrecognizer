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
		
		/// <summary>
		/// <see cref="ExpressionTokenItem"/>'s constructor.
		/// </summary>
		public ExpressionTokenItem() : base()
		{
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
		
#endregion Properties
		
#region Non-public methods
		
		protected override string MatchSequence (TokenSequence sequence)
		{
			// TODO: The index isn't the first, but the first in the baseline.
			string text = sequence[0].Text;
			sequence.RemoveAt(0);
			
			return text;
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

		
#endregion Non-public methods
	}
}
