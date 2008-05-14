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
	
		public bool Match(TokenSequence sequence, out string text)
		{
			Console.WriteLine(this.ToString());
			
			foreach (SyntacticalExpression expression in expressions)  
			{
				
				string expressionRes;
				if(expression.Match(sequence, out expressionRes))
				{
					// If the matching is successful, we consider 
					// the output valid.
					text = expressionRes;
					return true;
				}
				
			}
			
			text ="";
			return false;
		}
		
		public override string ToString ()
		{
			List<string> expressionStrings = new List<string>();
			foreach (SyntacticalExpression exp in expressions)
			{
				expressionStrings.Add(exp.ToString());
			}
			
			return String.Format("{0} : {1}",
			                     this.ruleName,
			                     String.Join(" | ", expressionStrings.ToArray()));
		}

		
#endregion Public methods
		
	}
}
