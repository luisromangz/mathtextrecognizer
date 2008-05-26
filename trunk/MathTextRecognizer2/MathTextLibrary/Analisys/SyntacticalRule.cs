// SyntacticRule.cs created with MonoDevelop
// User: luis at 14:18 12/05/2008

using System;
using System.Collections.Generic;

using System.Xml.Serialization;

namespace MathTextLibrary.Analisys
{
	
	
	/// <summary>
	/// This class represent the rules used for syntactila analysis.
	/// </summary>
	[XmlInclude(typeof(SyntacticalExpression))]
	public class SyntacticalRule : SyntacticalMatcher
	{
		private List<SyntacticalExpression> expressions;
		private string ruleName;
		
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
		
		public SyntacticalRule(SyntacticalRule rule)
		{
			this.expressions =rule.expressions;
			this.ruleName = rule.ruleName;
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
		/// Contains the label used by the rule.
		/// </value>
		public override string Label {
			get { return Name; }
		}
		
		public override string Type {
			get { return "Regla"; }
		}


		
		/// <value>
		/// Contains the expressions used to actually match the rule.
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
		
		
#endregion Properties
		
#region Public methods
	
		/// <summary>
		/// Tries to math a <see cref="TokenSequence"/> with the rule's 
		/// expressions.
		/// </summary>
		/// <param name="sequence">
		/// The <see cref="TokenSequence"/> we want to match.
		/// </param>
		/// <param name="text">
		/// The <see cref="System.String"/> output text produced by the
		/// expression that matched the input sequence.
		/// </param>
		/// <returns>
		/// <c>true</c> it the sequence was matched correctly, <c>false</c>
		/// if there were errors.
		/// </returns>
		public override bool Match(TokenSequence sequence, out string text)
		{

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
		
		/// <summary>
		/// Puts the rules info in string format.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public override string ToString ()
		{
			List<string> expressionStrings = new List<string>();
			foreach (SyntacticalExpression exp in expressions)
			{
				expressionStrings.Add(exp.ToString());
			}
			
			return String.Format("{0} : {1}",
			                     this.ruleName,
			                     String.Join("\n| ", expressionStrings.ToArray()));
		}

		
#endregion Public methods
		
	}
}
