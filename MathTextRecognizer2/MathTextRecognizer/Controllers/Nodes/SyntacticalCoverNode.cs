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
		
#endregion Properties
		
#region Public methods
		
		public void Select()
		{
			Application.Invoke(SelectInThread);
		}
		
#endregion Public methods
		
#region Non-public methods
		
		private void SelectInThread(object sender, EventArgs args)
		{
			container.NodeSelection.SelectNode(this);
			
			container.ScrollToCell(container.Selection.GetSelectedRows()[0],
			                       container.Columns[0],
			                       true, 0.5f, 0f);
		}
		
	
		
#endregion Non-public methods
		
#region Event handlers
		
		
#endregion Event handlers
	}
}
