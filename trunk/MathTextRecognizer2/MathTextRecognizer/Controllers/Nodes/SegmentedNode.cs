// created on 03/01/2006 at 16:01

using System;
using System.Collections.Generic;

using Gtk;

using MathTextLibrary;
using MathTextLibrary.Symbol;
using MathTextLibrary.Bitmap;

using MathTextLibrary.Controllers;
using MathTextLibrary.Utils;

namespace MathTextRecognizer.Controllers.Nodes
{
	
	/// <summary>
	/// La clase <code>FormulaNode</code> especializa la clase <c>TreeNode</c> 
	/// para poder a los nodos del arbol que mostramos en la interfaz un objeto
	/// <c>MathTextBitmap</c> que represente la imagen correspondiente al nodo.
	/// </summary>	
	public class SegmentedNode : TreeNode
	{
	
		private string name;
		private string label;
		private MathTextBitmap bitmap;
		private NodeView view;
		private string position;
		
		private List<MathSymbol> symbols;
		
		/// <summary>
		/// El constructor de <code>FormulaNode</code>
		/// </summary>
		/// <param name="name">
		/// El texto que aparecera en el nodo.
		/// </param>
		/// <param name="bitmap">
		/// El objeto <code>MathTextBitmap</code> asociado al nodo.
		/// </param>
		/// <param name="view">
		/// El árbol al que añadimos el nodo.
		/// </param>		
		public SegmentedNode(string name, MathTextBitmap bitmap, NodeView view)
		    : base()
		{
			this.name=name;
			this.label="";
			this.bitmap=bitmap;
			
			position = String.Format("({0}, {1})",
			                         bitmap.Position.X,
			                         bitmap.Position.Y);
			
			this.view = view;
			
			this.symbols = new List<MathSymbol>();
		}
		
		/// <summary>
		/// <c>SegmentedNode</c>'s constructor
		/// </summary>
		/// <param name="bitmap">
		/// A <see cref="MathTextBitmap"/>
		/// </param>
		/// <param name="view">
		/// The parent node's <c>NodeView</c>.
		/// </param>
		public SegmentedNode(MathTextBitmap bitmap, NodeView view)
		{
		
			this.view = view;
			
			this.bitmap =bitmap;
			
			this.position = String.Format("({0}, {1})",
			                              bitmap.Position.X, bitmap.Position.Y);
		}
		
		
		/// <value>
		/// Contiene la etiqueta del nodo.
		/// </value>
		[TreeNodeValue (Column=1)]
		public string Label
		{
			get
			{
				return label;
			}		
		}
		  
		/// <value>
		/// Contiene la imagen asociada al nodo.
		/// </value>
		public MathTextBitmap MathTextBitmap
		{
			get
			{
				return bitmap;
			}			
		}

		/// <value>
		/// Contiene el nombre de la imagen del nodo.
		/// </value>
		[TreeNodeValue (Column=0)]
		public string Name 
		{
			get {
				return name;
			}
		}

		/// <value>
		/// Contiene la posicion del bitmap.
		/// </value>
		[TreeNodeValue (Column=2)]
		public string Position 
		{
			get {
				return position;
			}
		}

		/// <value>
		/// Contains the symbols associed with the node.
		/// </value>
		public List<MathSymbol> Symbols 
		{
			get 
			{
				return symbols;
			}
			set 
			{
				symbols = value;
				SetLabels();
			}
		}

		/// <value>
		/// Contains the <c>NodeView</c> instance in which the node is shown.
		/// </value>
		public NodeView View 
		{
			get 
			{
				return view;
			}
		}

	
		/// <summary>
		/// Metodo que maneja el evento provocado al asociarse un simbolo 
		/// al <code>MathTextBitmap</code>.
		/// </summary>
		public void SetLabels()
		{
			// We prepare the string.
			string text ="";
			foreach(MathSymbol s in symbols)
			{
				text += String.Format("{0}, ", s.Text);
			}
			
			label = text.TrimEnd(',',' ');
			
			view.ColumnsAutosize();
			
			view.QueueDraw();
			
		}
		 
		/// <summary>
		/// Añade un nodo hijo al nodo.
		/// </summary>
		/// <param name="childBitmap">
		/// A <see cref="MathTextBitmap"/>
		/// </param>
		public void AddSegmentedChild(SegmentedNode node)
		{		
			Application.Invoke(this, 
			                   new AddNodeArgs(node),
			                   AddSegmentedChildInThread);
			
		}
		
		/// <summary>
		/// Auxiliary method used to add the node in the main app's thread.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void AddSegmentedChildInThread(object sender, EventArgs arg)
		{
			AddNodeArgs a = arg as AddNodeArgs;
			this.AddChild(a.Node);
			
			if(this.Parent ==null)
			{
				// Si no tiene padre
				a.Node.name = String.Format("Img. {0}",this.ChildCount);
			}
			else
			{
				a.Node.name = String.Format("{0}.{1}",this.name, this.ChildCount);
			}
			
			view.ExpandAll();
			view.ColumnsAutosize();
		}
		
		/// <summary>
		/// Selecciona el nodo.
		/// </summary>
		public void Select()
		{
			Application.Invoke(new EventHandler(SelectInThread));
		}
			
		/// <summary>
		/// Realiza la selecion del nodo en el hilo de la interfaz.
		/// </summary>
		private void SelectInThread(object sender, EventArgs args)
		{
			view.NodeSelection.SelectNode(this);
			TreePath path = view.Selection.GetSelectedRows()[0];
			view.ScrollToCell(path,view.Columns[0],true,0.5f,0f);
		}
		
		/// <summary>
		/// Contains the arguments of the handler method for the addition
		/// of children nodes in the gui's thread.
		/// </summary>
		public class AddNodeArgs : EventArgs
		{
			private SegmentedNode node;
			
			/// <summary>
			/// <c>AddNodeArgs</c>'s constructor.
			/// </summary>
			public AddNodeArgs(SegmentedNode node)
			{
				this.node= node;
			}
			
			/// <summary>
			/// Contains the node added.
			/// </summary>
			public SegmentedNode Node
			{
				get
				{
					return node;
				}
			}
		}
	}
}
