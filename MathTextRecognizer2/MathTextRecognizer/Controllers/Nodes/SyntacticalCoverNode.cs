// SyntacticalCoverNode.cs created with MonoDevelop
// User: luis at 16:42 22/05/2008

using System;
using Gtk;

using MathTextLibrary.Analisys;

namespace MathTextRecognizer.Controllers.Nodes
{
	
	/// <summary>
	/// This class implements the node used to display the rule covering of 
	/// the parsing process.
	/// </summary>
	public class SyntacticalCoverNode : TreeNode
	{
	
		private SyntacticalMatcher matcher;
	
		
		public SyntacticalCoverNode(SyntacticalMatcher matcher)
		{
			this.matcher = matcher;
		}
		
#region Properties
		
		/// <value>
		/// Contains the label shown for the cover node.
		/// </value>
		[TreeNodeValue(Column = 0)]
		public string Label
		{
			get
			{
				return matcher.Label;
			}
			
			
		}
		
		/// <value>
		/// Contains the element matching in this node.
		/// </value>
		public SyntacticalMatcher Matcher
		{
			get 
			{
				return matcher;
			}
			set 
			{
				matcher = value;
			}
		}
		
#endregion Properties
	}
}
