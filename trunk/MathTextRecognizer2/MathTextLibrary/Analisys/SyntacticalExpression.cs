// SyntacticExpression.cs created with MonoDevelop
// User: luis at 14:19 12/05/2008

using System;
using System.Collections.Generic;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This class implements the expressions used in syntactical analysis.
	/// </summary>
	public class SyntacticalExpression : ISyntacticMatcher
	{
		
		private List<ExpressionItem> items;
		
		private List<Token> firstTokens;
		
		
		public SyntacticalExpression()
		{
		}
		
#region Properties
		
		/// <value>
		/// Contains the tokens that may appear if this rule can match 
		/// the input.
		/// </value>
		public List<Token> FirstTokens
		{
			get
			{
				// We should calculate the first token set
				if(firstTokens == null)
				{
					CreateFirstTokensSet();
				}
				return null;
			}
		} 
		
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Matches a sequence of tokens with a sequence.
		/// </summary>
		/// <param name="sequence">
		/// The token sequence to be matched.
		/// </param>
		/// <returns>
		/// The string result of the matching.
		/// </returns>
		public string Match(TokenSequence sequence)
		{		
			string res = "";
			foreach (ExpressionItem item in items) 
			{
				res +=item.Match(sequence);
			}
			
			return res;
		}
		
#endregion Public methods
		
#region Non-public methods
		
		/// <summary>
		/// Creates the set of first tokens of the expression.
		/// </summary>
		private void CreateFirstTokensSet() 
		{
			firstTokens = new List<Token>();
			
			// We test each child item.		
			foreach (ExpressionItem item in items) 
			{
				foreach (Token t in item.FirstTokens) 
				{
					if(!firstTokens.Contains(t))
					{
						// We only add the token if it isn't already pressent.
						firstTokens.Add(t);
					}					
				}
				
				if(!(item.Modifier == ExpressionItemModifier.NonCompulsory
				     || item.Modifier == ExpressionItemModifier.RepeatingNonCompulsory))
				{
					// If the tested token isn't nullable,
					// we can stop as the first token set won't change now.
					// We can also remove the empty token from the list.
					firstTokens.Remove(Token.Empty);
					break;
				}
					
			}
			
		}
		
#endregion Non-public methods
		
		
	}
}
