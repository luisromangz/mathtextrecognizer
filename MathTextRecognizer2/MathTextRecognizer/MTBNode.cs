// created on 03/01/2006 at 16:01

using System;
using Gtk;

using MathTextLibrary;
using MathTextLibrary.Bitmap;

namespace MathTextRecognizerGUI
{
	
	/// <summary>
	/// La clase <code>MTBNode</code> especializa
	/// la clase <code>TreeNode</code> para poder a los nodos del arbol que mostramos
	/// en la interfaz un objeto <code>MathTextBitmap</code> que represente la imagen
	/// correspondiente al nodo.
	/// </summary>	
	public class MTBNode : TreeNode
	{
	
		private string name;
		private MathTextBitmap bitmap;
		private NodeView view;
		
		/// <summary>
		/// El constructor de <code>MTBNode</code>
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
		public MTBNode(string name, MathTextBitmap bitmap, NodeView view)
		    : base()
		{
			this.name=name;
			this.bitmap=bitmap;
			
			this.view = view;
			
			this.bitmap.ChildrenAdded +=
			    new MathTextBitmapChildrenAddedEventHandler(OnChildrenAdded);
			
			this.bitmap.SymbolChanged +=
			    new MathTextBitmapSymbolChangedEventHandler(OnSymbolChanged);
		}
		
		/// <summary>
		/// Propiedad de solo lectura que permite obtener el texto del nodo.
		/// </summary>
		[TreeNodeValue (Column=0)]
		public string Text
		{
			get
			{
				return name;
			}		
		}
		  
		/// <summary>
		/// Propiedad de solo lectura que permite obtener la imagen asociada al nodo.
		/// </summary>
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
			this.view.QueueDraw(); 
		}
		
		/// <summary>
		/// Método que maneja el evento provocado al añdirse los hijos al 
		/// <code>MathTextBitmap</code> tras
		/// segmentar para que el cambio se refleje en la interfaz.
		/// </summary>
		private void OnChildrenAdded(object sender, MathTextBitmapChildrenAddedEventArgs arg)
		{			
			MTBNode node;
			int i=0;
			foreach(MathTextBitmap child in arg.Children)			
			{		
				i++;					
				node=new MTBNode("Subimagen "+i, child, view);
				AddChild(node);			
			}
			
			view.ExpandAll();
			view.QueueDraw();
			
		}
	}
}
