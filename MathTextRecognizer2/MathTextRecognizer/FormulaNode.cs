// created on 03/01/2006 at 16:01

using System;
using System.Collections.Generic;

using Gtk;

using MathTextLibrary;
using MathTextLibrary.Bitmap;

using MathTextLibrary.Controllers;

namespace MathTextRecognizer
{
	
	/// <summary>
	/// La clase <code>FormulaNode</code> especializa la clase <c>TreeNode</c> 
	/// para poder a los nodos del arbol que mostramos en la interfaz un objeto
	/// <c>MathTextBitmap</c> que represente la imagen correspondiente al nodo.
	/// </summary>	
	public class FormulaNode : TreeNode
	{
	
		private string name;
		private string label;
		private MathTextBitmap bitmap;
		private NodeView view;
		private string position;
		
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
		public FormulaNode(string name, MathTextBitmap bitmap, NodeView view)
		    : base()
		{
			this.name=name;
			this.label="";
			this.bitmap=bitmap;
			
			position = String.Format("({0}, {1})",
			                         bitmap.Position.X,
			                         bitmap.Position.Y);
			
			this.view = view;
			
			bitmap.SymbolChanged += new SymbolChangedHandler(OnSymbolChanged);
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
		
		/// <summary>
		/// Metodo que maneja el evento provocado al asociarse un simbolo 
		/// al <code>MathTextBitmap</code>.
		/// </summary>
		private void OnSymbolChanged(object sender,EventArgs arg)
		{
			// TODO Arreglar cambio del texto del nodo.
			
			if(bitmap.Symbol !=null)
			{
				// Si es nulo es pq no se reconoció.				
				this.label=bitmap.Symbol.Text;
				this.view.QueueDraw();
			}
		}
		
		/// <summary>
		/// Añade un nodo hijo al nodo.
		/// </summary>
		/// <param name="childBitmap">
		/// A <see cref="MathTextBitmap"/>
		/// </param>
		/// <returns>
		/// El nodo añadido.
		/// </returns>
		public FormulaNode AddChild(MathTextBitmap childBitmap)
		{		
			string name="";
			if(this.Parent ==null)
			{
				// Si no tiene padre
				name = String.Format("Imagen {0}",this.ChildCount+1);
			}
			else
			{
				name = String.Format("{0}.{1}",this.name, this.ChildCount+1);
			}
			
			
			
			FormulaNode node = new FormulaNode(name,
			                                   childBitmap,
			                                   view);
				
			this.AddChild(node);
			
			view.ExpandAll();
			view.QueueDraw();
			
			return node;
		}
		
		/// <summary>
		/// Selecciona el nodo.
		/// </summary>
		public void Select()
		{
			Application.Invoke(new EventHandler(SelectThread));
		}
			
		/// <summary>
		/// Realiza la selecion del nodo en el hilo de la interfaz.
		/// </summary>
		private void SelectThread(object sender, EventArgs args)
		{
			view.NodeSelection.SelectNode(this);
			TreePath path = view.Selection.GetSelectedRows()[0];
			view.ScrollToCell(path,view.Columns[0],true,1,0);
		}
	}
}
