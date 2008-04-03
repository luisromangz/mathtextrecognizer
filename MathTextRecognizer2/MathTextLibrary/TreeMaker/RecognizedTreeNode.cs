using System;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;

namespace MathTextLibrary.TreeMaker
{
	/// <summary>
	/// <para>Esta clase se utiliza para representar un arbol que resulte sencillo
	/// convertir en el futuro a un formato de salida.</para>
	/// <para>Este arbol es un arbol n-ario, donde cada nodo tiene una lista
	/// de hijos.</para>
	/// </summary>
	/// <remarks>
	/// El simbolo contenido en un <c>RecognizedTreeNode</c> puede ser distinto
	/// al contenido en la imagen asociada al mismo.
	/// </remarks>
	public class RecognizedTreeNode
	{
		MathTextBitmap image;
		
		// en los constructores se garantiza que la lista nunca es null, como
		// minimo es una lista vacia
		List<RecognizedTreeNode> children;
		
		MathSymbol symbol;
		
		/// <summary>
		/// Constructor a partir de una imagen y una lista de hijos. El simbolo
		/// asociado al nodo se toma de la imagen <c>mtb</c>.
		/// </summary>
		/// <param name="mtb">Imagen asociada al nodo</param>
		/// <param name="children">Hijos del nodo</param>
		/// <exception cref="System.ApplicationException">Lanzada si
		/// la imagen <c>mtb</c> es <c>null</c></exception>
		public RecognizedTreeNode(MathTextBitmap mtb,
		                          List<RecognizedTreeNode> children)
		{
			if(mtb==null)
				throw new ApplicationException("¡Sin imagen en el constructor!");
			this.image=mtb;
			this.symbol=mtb.Symbol;
			if(children==null)
				children=new List<RecognizedTreeNode>();
			else
				this.children=children;
		}
		
		/// <summary>
		/// Constructor con un simbolo y una lista de hijos. El nodo no tendra
		/// asignada ninguna imagen (este es el caso de los nodos auxiliares
		/// de las fracciones y los super y subindices, por ejemplo).
		/// </summary>
		/// <param name="symbol">Simbolo asociado al nodo</param>
		/// <param name="children">Hijos del nodo</param>
		/// <exception cref="System.ApplicationException">Lanzada si
		/// el simbolo <c>symbol</c> es <c>null</c></exception>
		public RecognizedTreeNode(MathSymbol symbol,
		                          List<RecognizedTreeNode> children)
		{
			if(symbol==null)
				throw new ApplicationException("¡Sin simbolo en el constructor!");
			this.image=null;
			this.symbol=symbol;
			if(children==null)
				children=new List<RecognizedTreeNode>();
			else
				this.children=children;
		}

		/// <value>
		/// Propiedad que permite recuperar la lista de hijos de un nodo.
		/// </value>
		public List<RecognizedTreeNode> Children
		{
			get{
				return children;
			}		
		}

		/// <value>
		/// Propiedad que permite recuperar la imagen de la que se obtuvo el nodo
		/// del arbol.
		/// </value>
		public MathTextBitmap Image
		{
			get{
				return image;
			}		
		}
		
		/// <value>
		/// Si el nodo tiene simbolo (<code>symbol!=null</code>) entonces se
		/// devuelve este. En caso contrario se devuelve el simbolo de la imagen
		/// asociada al nodo.
		/// </value>
		public MathSymbol Symbol
		{
			get{
				if(symbol==null)
					return image.Symbol;
				else
					return symbol;
			}
		}
		
		/// <value>
		/// Contiene el hijo <c>i</c> del nodo.
		/// </value>
		public RecognizedTreeNode this[int i]
		{
			get{
				return (RecognizedTreeNode)children[i];
			}
			set{
				children[i]=value;
			}
		}
		
		public override string ToString()
		{
			string text="Raiz";
			
			text+=" "+NodeText();
			if(children!=null)
			{
				text+="  Hijos: ";
				foreach(RecognizedTreeNode child in children)
				{
					text+=child.NodeText();
				}
				text+="\n";
				foreach(RecognizedTreeNode child in children)
				{
					text+=child.ToString();
				}
			}
			
			return text;
		}
		
		private string NodeText()
		{
			string text="(";
			
			if(symbol!=null)
			{
				text+=symbol.Text;
			}
			if(image!=null)
			{
				text+="; "+image.Position.X+","+image.Position.Y+",w:"+image.Width+",h:"+image.Height;
			} else
			{
				text+="; sin imagen";
			}
						
			return text+")";
		}

	}
}
