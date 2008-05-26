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
		
		private NodeView container;
	
		
		public SyntacticalCoverNode(SyntacticalMatcher matcher, NodeView container)
		{
			this.container = container;
			this.matcher = matcher;
			matcher.Matching += OnMatcherMatching;
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
		
#region Non-public methods
		
		private void OnMatcherMatchingInThread(object sender, EventArgs args)
		{
			container.NodeSelection.SelectNode(this);
			
			container.ScrollToCell(container.Selection.GetSelectedRows()[0],
			                       container.Columns[0],
			                       true, 0.5f, 0f);
			
			Type type = this.matcher.GetType();
			Console.WriteLine(matcher.ToString());
			
			if(type == typeof(SyntacticalRule))
			{
				SyntacticalRule rule = matcher as SyntacticalRule;
				foreach (SyntacticalExpression exp in rule.Expressions) 
				{
					this.AddChild(new SyntacticalCoverNode(exp, container));
				}
			}
			else if(type == typeof(SyntacticalExpression))
			{
				SyntacticalExpression exp = matcher as SyntacticalExpression;
				
				foreach (ExpressionItem item in exp.Items) 
				{
					
					this.AddChild(new SyntacticalCoverNode(item, container));
				}
			}
			else if(type.GetType() == typeof(ExpressionGroupItem))
			{
				ExpressionGroupItem group = matcher as ExpressionGroupItem;
				
				foreach (ExpressionItem item in group.ChildrenItems) 
				{
					this.AddChild(new SyntacticalCoverNode(item, container));
				}
			}
			
			container.ExpandAll();
		}
		
#endregion Non-public methods
		
#region Event handlers
		
		private void OnMatcherMatching(object sender, EventArgs args)
		{
			Application.Invoke(OnMatcherMatchingInThread);
		}
		
#endregion Event handlers
	}
}
