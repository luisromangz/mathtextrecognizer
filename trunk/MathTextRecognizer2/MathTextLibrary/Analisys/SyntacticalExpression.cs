// SyntacticExpression.cs created with MonoDevelop
// User: luis at 14:19 12/05/2008

using System;
using System.Collections.Generic;

using System.Xml.Serialization;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This class implements the expressions used in syntactical analysis.
	/// </summary>
	[XmlInclude(typeof(ExpressionTokenItem))]
	[XmlInclude(typeof(ExpressionGroupItem))]
	[XmlInclude(typeof(ExpressionRuleCallItem))]
	public class SyntacticalExpression : SyntacticalMatcher
	{
		
		private List<ExpressionItem> items;
		
		private string formatString;
		
		public SyntacticalExpression()
		{
			items = new List<ExpressionItem>();
		}
		
#region Properties
		
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
		
		/// <value>
		/// Contains the label showed by the expression.
		/// </value>
		public override string Label 
		{
			get { return this.ToString(); }
		}
		
		
		public override string Type {
			get { return "Expresión"; }
		}


		
		
#endregion Properties
		
#region Public methods
		
		
		public override bool Match(ref TokenSequence sequence, out string output)
		{		

			MatchingInvoker();
			List<string> outputList = new List<string>();
			
			bool res;
			
			
			foreach (ExpressionItem item in items) 
			{
				string expressionString;
				res = item.Match(ref sequence,out expressionString);
				if(!res)
				{
					output="";				
					MatchingFinishedInvoker(output);
					return false;
				}
				
				outputList.Add(expressionString);
			}
			
			output = String.Format(formatString, outputList.ToArray());
			MatchingFinishedInvoker(output);
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
		
		
#endregion Non-public methods
		
		
	}
}
