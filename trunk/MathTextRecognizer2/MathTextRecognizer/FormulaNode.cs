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
		private MathTextBitmap bitmap;
		private NodeView view;
		
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
			this.bitmap=bitmap;
			
			this.view = view;
			
			bitmap.SymbolChanged += new SymbolChangedHandler(OnSymbolChanged);
		}
		
		/// <value>
		/// Contiene el texto del nodo.
		/// </value>
		[TreeNodeValue (Column=0)]
		public string Text
		{
			get
			{
				return name;
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
		
		/// <summary>
		/// Metodo que maneja el evento provocado al asociarse un simbolo 
		/// al <code>MathTextBitmap</code>.
		/// </summary>
		private void OnSymbolChanged(object sender,EventArgs arg)
		{
			this.name = this.name+": «"+bitmap.Symbol.Text+"»";
			this.name = String.Format("{0}: «{1}»", this.name, bitmap.Symbol.Text);
			this.view.QueueDraw(); 
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
			FormulaNode node = new FormulaNode(String.Format("Subimagen {0}",
			                                                 this.ChildCount+1),
			                                   childBitmap,
			                                   view);
				
			this.AddChild(node);
			
			view.ExpandAll();
			view.QueueDraw();
			
			return node;
		}
	}
}