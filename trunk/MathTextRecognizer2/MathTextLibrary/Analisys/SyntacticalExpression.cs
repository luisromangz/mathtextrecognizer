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
		
		private string formatString;
		
		public SyntacticalExpression()
		{
			items = new List<ExpressionItem>();
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

		/// <value>
		/// Contains the format string used to format the contents matched
		/// by the expression.
		/// </value>
		public string FormatString 
		{
			get 
			{
				return formatString;
			}
			set 
			{
				formatString = value;
			}
		}

		/// <value>
		/// Contains the items forming the expression.
		/// </value>
		public List<ExpressionItem> Items
		{
			get
			{
				return items;
			}
			
			set
			{
				items = value;
			}
		}
		
		
#endregion Properties
		
#region Public methods
		
		
		public bool Match(TokenSequence sequence, out string output)
		{		

			List<string> outputList = new List<string>();
			
			bool res;
			
			TokenSequence backupSequence = new TokenSequence();
			foreach (Token t in sequence) 
			{
				backupSequence.Append(t);
			}
			
			foreach (ExpressionItem item in items) 
			{
				string expressionString;
				res = item.Match(sequence,out expressionString);
				if(!res)
				{
					output="";
					// We revert the state to the original.
					sequence = backupSequence;
					return false;
				}
				
				outputList.Add(expressionString);
			}
			
			output = String.Format(formatString, outputList.ToArray());
			
			return true;
		}
		
		public override string ToString ()
		{
			List<string> resStrings = new List<string>();
			foreach (ExpressionItem item in items) 
			{
				resStrings.Add(item.ToString());
			}
			
			return String.Join(" ", resStrings.ToArray());
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
