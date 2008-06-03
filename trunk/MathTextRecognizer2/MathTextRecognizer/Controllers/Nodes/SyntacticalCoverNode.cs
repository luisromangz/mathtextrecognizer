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
		
		private string output;
		
		private NodeView container;
		
		private string matchedTokens;
		
		private SyntacticalMatcher matcher;
		
		private string padding ;
	
		
		public SyntacticalCoverNode(SyntacticalMatcher matcher, NodeView container)
		{
			this.container = container;
			this.matcherLabel = matcher.Label;
			this.matcher = matcher;
			
			this.matcherType = matcher.Type;
			
			matchedTokens ="";
			
			padding = "";
			
			this.ChildAdded += new TreeNodeAddedHandler(OnChildAdded);
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
				return String.Format("{0}{1}: {2}{3}{4}", 
				                     padding,
					                 matcherType, 
					                 matcherLabel,
					                 matchedTokens,
					                 output);
			}
		}
		
		public SyntacticalMatcher Matcher
		{
			get
			{
				return matcher;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		public void Select()
		{
			container.NodeSelection.SelectNode(this);
			
			container.ScrollToCell(container.Selection.GetSelectedRows()[0],
			                       null,
			                       true, 0.3f, 0.5f);
			
			container.QueueDraw();
		}
		
		/// <summary>
		/// Adds a token to the items list of matched tokens.
		/// </summary>
		/// <param name="matchedToken">
		/// A <see cref="Token"/>
		/// </param>
		public void AddMatchedToken(Token matchedToken)
		{
		
			if(String.IsNullOrEmpty(matchedTokens))
			{
				matchedTokens = "; Reconocido: "+ matchedToken.Text;
				
			}
			else
			{
				matchedTokens = ", " +matchedToken.Text;
			}
			
			container.QueueDraw();
		}
		
		public void SetOutput(string output)
		{
			if(output != "")
			{
				this.output = "; «"+output +"»";
				container.QueueDraw();
			}
			
				
		}
		
#endregion Public methods
		
#region Private methods
		
		private void OnChildAdded(object sender, ITreeNode _child)
		{
			SyntacticalCoverNode child = (SyntacticalCoverNode)_child;
			
			child.padding = this.padding +"   ";
		}
		
#endregion Private methods

		

	}
}
