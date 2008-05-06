// TokenNode.cs created with MonoDevelop
// User: luis at 23:17 06/05/2008

using System;
using System.Collections.Generic;

using Gtk;

using MathTextLibrary.Analisys.Lexical;

namespace MathTextRecognizer.Controllers.Nodes
{
	
	
	
	
	/// <summary>
	/// This class implements a node for the sequence treeview.
	/// </summary>
	public class SequenceNode : TreeNode
	{
		private TokenSequence sequence;
		private List<Token> tokens;
		
		private string sequenceLabel;
		private string tokensLabel;
		
		private NodeView widget;
		
		public SequenceNode(TokenSequence sequence)
		{
			this.sequence = sequence;
			this.tokens = new List<Token>();
			
			this.sequence.Changed += new EventHandler(OnSequenceChangedAdded);	
		}
		
#region Properties
		
		/// <value>
		/// Contains the node sequence column text.
		/// </value>
		[TreeNodeValue(Column =0)]
		public string SequenceText
		{
			get
			{
				return sequenceLabel;
			}
		}
	
		/// <value>
		/// Contains the node tokens column text.
		/// </value>
		[TreeNodeValue(Column =1)]
		public string TokensText
		{
			get
			{
				return tokensLabel;
			}
		}

		/// <value>
		/// Contains the tokens assigned to this node's token sequence.
		/// </value>
		public List<Token> Tokens 
		{
			get 
			{
				return tokens;
			}
		}

		/// <value>
		/// Contains the node's token sequence.
		/// </value>
		public TokenSequence Sequence 
		{
			get 
			{
				return sequence;
			}
		}

		/// <value>
		/// Contains the treeviw which shows the node.
		/// </value>
		public NodeView Widget
		{
			get 
			{
				return widget;
			}
			set 
			{
				widget = value;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Adds a child to the node.
		/// </summary>
		/// <param name="childNode">
		/// A <see cref="SequenceNode"/>
		/// </param>
		public void AddSequenceChild(SequenceNode childNode)
		{
			AddSequenceChildArgs args = new AddSequenceChildArgs(childNode);
			Application.Invoke(this, 
			                   args,
			                   AddSequenceChildThread);
		}
		
		
		
		
		/// <summary>
		/// Appends a token to the node's token list.
		/// </summary>
		/// <param name="foundToken">
		/// A <see cref="Token"/>
		/// </param>
		public void AppendToken(Token foundToken)
		{
			Application.Invoke(this, 
			                   new AppendTokenArgs(foundToken),
			                   AppendTokenInThread);
		}
		
		
		/// <summary>
		/// Removes the node's children.
		/// </summary>
		public void RemoveSequenceChildren()
		{
			Application.Invoke(RemoveSequenceChildrenInThread);
		}
		
		/// <summary>
		/// Selects the node.
		/// </summary>
		public void Select()
		{
			Application.Invoke(SelectInThread);
		}
#endregion Public methods
		
#region Event handlers	
		/// <summary>
		/// Creates the label for the sequence column when the sequence 
		/// is modified.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnSequenceChangedAdded(object sender, EventArgs args)
		{
			Application.Invoke(OnSequenceItemAddedInThread);
		}
		
		private void OnSequenceItemAddedInThread(object sender, EventArgs args)
		{
			List<string> res = new List<string>();
			
			foreach (Token t in sequence) 
			{
				res.Add(String.Format("«{0}»",t.Text));
			}
			
			sequenceLabel =  String.Join(", ", res.ToArray());
			
			if(sequenceLabel =="")
			{
				// If we have removed the label, we warn the user.
				sequenceLabel = "Error al reconocer";
			}
		}

		
		private void AddSequenceChildThread(object sender, 
		                                   EventArgs args)
		{
			AddSequenceChildArgs a = args as AddSequenceChildArgs;
			this.AddChild(a.ChildNode);
			
			// We set the tree view of the child node.
			a.ChildNode.Widget = this.widget;
			
			// We expand the node and resize the columns (if necessary).
			widget.ExpandAll();
			widget.ColumnsAutosize();
		}
		
		private void AppendTokenInThread(object sender, EventArgs args)
		{
			// We append the node to the list, then build the label.
			AppendTokenArgs a = args as AppendTokenArgs;
			
			tokens.Add(a.FoundToken);
			
			if(tokens.Count == 1)
			{
				tokensLabel = a.FoundToken.Type;
			}
			else if(tokens.Count >1)
			{
				tokensLabel += String.Format(", {0}", a.FoundToken.Type);
			}
			
			widget.ColumnsAutosize();
		}
		
		private void RemoveSequenceChildrenInThread(object sender, 
		                                            EventArgs args)
		{
			// We remove all children.
			while(this.ChildCount > 0)
				this.RemoveChild(this[0] as TreeNode);
		}
		
		private void SelectInThread(object sender, EventArgs args)
		{
			widget.NodeSelection.SelectNode(this);
		}
	
		
#endregion Event handlers
	}
	
	/// <summary>
	/// Implements a helper class so we can pass the child node to the
	/// delegate adding childs in the application's thread.
	/// </summary>
	class AddSequenceChildArgs : EventArgs
	{
		private SequenceNode childNode;
		public AddSequenceChildArgs(SequenceNode childNode)
		{
			this.childNode= childNode;
		}
		
		/// <summary>
		/// Contains the node to be added.
		/// </summary>
		public SequenceNode ChildNode
		{
			get
			{
				return childNode;
			}
		}
	}
	
	/// <summary>
	/// Implements a helper class so we can pass the found token
	/// so its added in the app's main thread.
	/// </summary>
	class AppendTokenArgs : EventArgs
	{
		private Token foundToken;
		public AppendTokenArgs(Token foundToken)
		{
			this.foundToken= foundToken;
		}
		
		/// <summary>
		/// Contains the node to be added.
		/// </summary>
		public Token FoundToken
		{
			get
			{
				return foundToken;
			}
		}
	}
}
