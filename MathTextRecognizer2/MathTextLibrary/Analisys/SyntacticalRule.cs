// SyntacticRule.cs created with MonoDevelop
// User: luis at 14:18 12/05/2008

using System;
using System.Collections.Generic;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This class represent the rules used for syntactila analysis.
	/// </summary>
	public class SyntacticalRule : ISyntacticMatcher
	{
		private List<SyntacticalExpression> expressions;
		private string ruleName;
		
		private List<Token> firstItems;
		
		/// <summary>
		/// <see cref="SyntacticRule"/>'s default constructor.
		/// </summary>
		public SyntacticalRule()
		{
			expressions = new List<SyntacticalExpression>();
		}
		
		public SyntacticalRule(string ruleName) : this()
		{
			this.ruleName = ruleName;
		}
		
#region Properties
		
		/// <summary>
		/// Contains the rule's name.
		/// </summary>
		public string Name
		{
			get
			{
				return ruleName;
			}
			set
			{
				ruleName = value.Trim();
			}
		}
		
		/// <value>
		/// Contains the expressions used to 
		/// </value>
		public List<SyntacticalExpression> Expressions
		{
			get
			{
				return expressions;
			}
			set
			{
				expressions = value;
			}
		}
		
		/// <value>
		/// Contains the first tokens that may appear in the rule's expressions.
		/// </value>
		public List<Token> FirstTokens
		{
			get
			{
				if(firstItems == null)
				{
					firstItems = new List<Token>();
					
					foreach (SyntacticalExpression exp in expressions) 
					{
						foreach (Token t in exp.FirstTokens ) 
						{
							if(!firstItems.Contains(t))
							{
								// We add the token only if it wasn't alread
								// present.
								firstItems.Add(t);
							}
						}
					}
				}
				
				return firstItems;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Tries to match a token sequence with any of the syntactical rules
		/// contained.
		/// </summary>
		/// <param name="sequence">
		/// A <see cref="TokenSequence"/> 
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public string Match(TokenSequence sequence)
		{
			string res ="";
			
			foreach (SyntacticalExpression expression in expressions)  
			{
				// We check the first token of the sequence against the 
				// expression's set of possible firt tokens.
				if(expression.FirstTokens.Contains(sequence[0]))
				{
					res = expression.Match(sequence);
					break;
				}
			}
			
			return res;
		}
		
#endregion Public methods
		
	}
}
