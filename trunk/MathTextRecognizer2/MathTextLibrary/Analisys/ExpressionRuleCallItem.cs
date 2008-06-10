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
		private string ruleName;
		
		
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
				return ruleName;
			}
			set 
			{
				ruleName = value;
			}
		}
		
		/// <value>
		/// Contains the label shown by the item.
		/// </value>
		public override string Label 
		{
			get 
			{ 
				return ToString(); 
			}
		}
		
		/// <value>
		/// Contains a label for the rule call type.
		/// </value>
		public override string Type 
		{
			get 
			{ 
				return "Llamada a regla"; 
			}
		}
		
		/// <value>
		/// Contains a list with the name of the rule called.
		/// </value>
		public override List<string> RulesUsed 
		{
			get 
			{ 
				List<string> ruleNames = new List<string>();
				
				ruleNames.Add(this.ruleName);
				
				return ruleNames;
			}
		}
		
#endregion Properties

#region Non-public methods
		
		protected override bool MatchSequence (ref TokenSequence sequence, out string output)
		{
			// The actual matching is done by the rule.
			SyntacticalRule ruleCalled = 
				SyntacticalRulesLibrary.Instance[ruleName];
			
			bool res =  ruleCalled.Match(ref sequence, out output);
			if(res)
			{
				output = String.Format(formatString, output);
			}
			
			return res;
		}

		protected override string SpecificToString ()
		{
			return this.ruleName;
		}


		
#endregion Non-public methods
	}
}
