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
	
		private string matcherLabel;
		private string matcherType;
		
		private NodeView container;
		
		private string matchedTokens;
	
		
		public SyntacticalCoverNode(SyntacticalMatcher matcher, NodeView container)
		{
			this.container = container;
			this.matcherLabel = matcher.Label;
			
			this.matcherType = matcher.Type;
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
				return matcherLabel;
			}
		}
		
		/// <value>
		/// Contains the node's item type.
		/// </value>
		[TreeNodeValue(Column = 1)]
		public string MatcherType
		{
			get
			{
				
				return matcherType;
			}
		}
		
		[TreeNodeValue(Column = 2)]
		public string MatchedTokens
		{
			get
			{
				return matchedTokens;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		public void Select()
		{
			container.NodeSelection.SelectNode(this);
			
			container.ScrollToCell(container.Selection.GetSelectedRows()[0],
			                       container.Columns[0],
			                       true, 0.5f, 0.5f);
		}
		
		/// <summary>
		/// Adds a token to the items list of matched tokens.
		/// </summary>
		/// <param name="matchedToken">
		/// A <see cref="Token"/>
		/// </param>
		public void AddMatchedToken(Token matchedToken)
		{
			string newText = String.Format("«{0}»", matchedToken.Text);
			if(String.IsNullOrEmpty(matchedTokens))
			{
				matchedTokens = newText;
				
			}
			else
			{
				matchedTokens = ", " +newText;
			}
			
			container.QueueDraw();
		}
		
#endregion Public methods

		

	}
}
