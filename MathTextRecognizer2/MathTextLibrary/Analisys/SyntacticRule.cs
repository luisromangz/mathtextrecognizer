// SyntacticRule.cs created with MonoDevelop
// User: luis at 14:18 12/05/2008

using System;
using System.Collections.Generic;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This class represent the rules used for syntactila analysis.
	/// </summary>
	public class SyntacticRule : ISyntacticMatcher
	{
		private List<SyntacticalExpression> expressions;
		private string ruleName;
		
		/// <summary>
		/// <see cref="SyntacticRule"/>'s default constructor.
		/// </summary>
		public SyntacticRule()
		{
			expressions = new List<SyntacticalExpression>();
		}
		
		public SyntacticRule(string ruleName) : this()
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
			
			return null;
		}
		
#endregion Public methods
		
	}
}
