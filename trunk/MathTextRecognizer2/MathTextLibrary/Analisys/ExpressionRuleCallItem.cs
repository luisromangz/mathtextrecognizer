// ExpressionSubexpressionItem.cs created with MonoDevelop
// User: luis at 14:46 13/05/2008

using System;
using System.Collections.Generic;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This class implements an expression item that represents the use
	/// of another expression in the parent expression.
	/// </summary>
	public class ExpressionRuleCallItem : ExpressionItem
	{
		string expressionName;
		
		
		/// <summary>
		/// <see cref="ExpressionSubexpressionItem"/>'s constructor.
		/// </summary>
		public ExpressionRuleCallItem() : base()
		{
		}
		


#region Properties
		
		/// <value>
		/// Contains the name of the subexpression to be used.
		/// </value>
		public string RuleName 
		{
			get 
			{
				return expressionName;
			}
			set 
			{
				expressionName = value;
			}
		}
		
#endregion Properties

#region Public methods

		
#endregion Public methods
		
#region Non-public methods
		
		protected override List<Token> CreateFirstTokensSet ()
		{
			// We just have to retrieve the referenced rule's first token set.
			return SyntacticalRulesManager.Instance[expressionName].FirstTokens;
		}
		
		protected override bool MatchSequence (TokenSequence sequence, out string output)
		{
			// The actual matching is done by the rule.
			return SyntacticalRulesManager.Instance[expressionName].Match(sequence, out output);
		}

		protected override string ToStringAux ()
		{
			return this.expressionName;
		}


		
#endregion Non-public methods
	}
}
