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
		
		public SyntacticalCoverNode(SyntacticalMatcher matcher, NodeView container)
		{
			this.container = container;
			this.matcherLabel = matcher.Label;
			this.matcher = matcher;
			
			this.matcherType = matcher.Type;
			
			matchedTokens ="";
			
			
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
				return String.Format("{0}: {1}{2}{3}", 
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
			                       container.Columns[0],
			                       true, 0.3f, 1f);
			
			container.QueueDraw();
			container.QueueResize();
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
				matchedTokens = "; Encaja: <i>"+ matchedToken.Text+"</i>";
				
			}
			else
			{
				matchedTokens = ", <i>" +matchedToken.Text+"</i>";
			}
			
			container.ColumnsAutosize();
			
			
			container.QueueResize();
			container.QueueDraw();
			
			container.Hadjustment.Value = container.Hadjustment.Upper;
			container.QueueResize();
			container.QueueDraw();
		
		}
		
		public void SetOutput(string output)
		{
			if(output != "")
			{
				this.output = "; <i>"+output +"</i>";
				container.ColumnsAutosize();
				container.QueueResize();
				container.QueueDraw();
				
				container.Hadjustment.Value = container.Hadjustment.Upper;
				container.QueueResize();
				container.QueueDraw();
			}
			
				
		}
		
#endregion Public methods
		
#region Private methods
		
		private void OnChildAdded(object sender, ITreeNode _child)
		{
			SyntacticalCoverNode child = (SyntacticalCoverNode)_child;
			
		}
		
#endregion Private methods

		

	}
}
