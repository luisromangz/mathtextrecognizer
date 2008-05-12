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
		
		private List<SyntacticalExpressionItem> items;
		
		
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
				if(items[0].Type == SyntacticalExpressionItemType.Token)
				{
					return items[0].Label;
				}
			}
		} 
		
		
#endregion Properties
		
#region Public methods
		
		public string Match(TokenSequence sequence)
		{
		
			return null;
		}
		
#endregion Public methods
		
		
	}
}
