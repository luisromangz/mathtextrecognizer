using System;
using System.Collections.Generic;

namespace MathTextLibrary.TreeMaker.Steps
{
	/// <summary>
	/// Este paso busca superindices y subindices en el arbol de entrada
	/// y crea un arbol nuevo cuya conversion a texto es mas sencilla.
	/// </summary>
	/// <remarks>
	/// Al encontrar un superindice o un subindice se crea un nuevo nodo sin
	/// imagen asociada, y de simbolo <c>MathSymbolType.Superindex</c> o
	/// <c>MathSymbolType.Subindex</c> segun corresponda. Este nodo tendra
	/// exactamente dos hijos: el primero es la base y el segundo el super
	/// o subindice.
	/// </remarks>
	public class Step2SearchSuperAndSub
	{
		public Step2SearchSuperAndSub()
		{
		}
		
		public RecognizedTreeNode ApplyStep(RecognizedTreeNode tree)
		{
			List<RecognizedTreeNode> newChildren=new List<RecognizedTreeNode>();
			RecognizedTreeNode first, second;
			
			if(tree.Symbol.SymbolType==MathSymbolType.Fraction)
			{
				newChildren.Add(this.ApplyStep(tree[0]));
				newChildren.Add(this.ApplyStep(tree[1]));
			}
			else if(tree.Children.Count>1)
			{
				for(int i=0; i<tree.Children.Count; i++)
				{
					first=this.ApplyStep(tree[i]);
					if(i==tree.Children.Count-1)// estamos procesando el último
					{							// elemento, no hay segundo
						newChildren.Add(first);
					}
					else
					{
						second=tree[i+1];
						if(IsSuperscript(first.Image, second.Image))
						{
							AddSuperNode(first, second, newChildren);
							//pasamos al third (si existe) saltándonos el second
							i++;
							if(i<tree.Children.Count)
							{
								first=tree[i];
							}
						} else if(IsSubscript(first.Image, second.Image))
							{
							AddSubNode(first, second, newChildren);
							//pasamos al third (si existe) saltándonos el second
							i++;
							if(i<tree.Children.Count)
							{
								first=tree[i];
							}
						} else
						{
							newChildren.Add(first);
							//newChildren.Add(new RecognizedTreeNode(first.Image, first.Children));
							first=second;
						}
						first=this.ApplyStep(first);
					}
				}
			}
			else if(tree.Children.Count==1) {
				first=this.ApplyStep(tree[0]);
				newChildren.Add(first);				
			}
			
			return new RecognizedTreeNode(tree.Image, newChildren);
		}
		
		/// <summary>
		/// Crea un nodo de superindice, de hijos <c>first</c> y <c>second</c>,
		/// y lo añade a la lista <c>newChildren</c>.
		/// </summary>
		/// <param name="first">Base del superindice</param>
		/// <param name="second">Superindice</param>
		/// <param name="newChildren">Lista de nuevos hijos</param>
		protected void AddSuperNode(RecognizedTreeNode first,
		                            RecognizedTreeNode second,
		                            List<RecognizedTreeNode> newChildren)
		{
			MathSymbol supSymbol=new MathSymbol("sup",MathSymbolType.Superindex);
			newChildren.Add(CreateSymbolNode(supSymbol,first,second));
		}
		                         
		/// <summary>
		/// Crea un nodo de subindice, de hijos <c>first</c> y <c>second</c>,
		/// y lo añade a la lista <c>newChildren</c>.
		/// </summary>
		/// <param name="first">Base del subindice</param>
		/// <param name="second">Subindice</param>
		/// <param name="newChildren">Lista de nuevos hijos</param>
		protected void AddSubNode(RecognizedTreeNode first, 
		                          RecognizedTreeNode second, 
		                          List<RecognizedTreeNode> newChildren)
		{
			MathSymbol subSymbol=new MathSymbol("sub",MathSymbolType.Subindex);
			newChildren.Add(CreateSymbolNode(subSymbol,first,second));
		}
		
		/// <summary>
		/// Metodo auxiliar que crea un nuevo nodo, sin imagen y de
		/// simbolo <c>symbol</c> y con dos hijos: <c>first</c> y <c>second</c>.
		/// </summary>
		/// <param name="symbol">Simbolo del nodo</param>
		/// <param name="first">Base del subindice</param>
		/// <param name="second">Subindice</param>
		private RecognizedTreeNode CreateSymbolNode(MathSymbol symbol, RecognizedTreeNode first, RecognizedTreeNode second)
		{
			List<RecognizedTreeNode> children=new List<RecognizedTreeNode>();
			children.Add(first);
			children.Add(second);
			return new RecognizedTreeNode(symbol, children);
		}
		
		/// <summary>
		/// Determina si <c>image2</c> es superindice de <c>image1</c>.
		/// </summary>
		/// <param name="image1">Una imagen</param>
		/// <param name="image2">Otra imagen</param>
		/// <returns>true si image2 es superindice de image1</returns>
		protected bool IsSuperscript(MathTextBitmap image1, MathTextBitmap image2)
		{
			int posY1=image1.Position.Y;
			int posY2=image2.Position.Y;
			int h1=image1.Height;
			int h2=image2.Height;
			
			return posY2 < posY1 && posY2+h2<posY1+h1; //&& posY2+h2>posY1;
		}

		/// <summary>
		/// Determina si <c>image2</c> es subindice de <c>image1</c>.
		/// </summary>
		/// <param name="image1">Una imagen</param>
		/// <param name="image2">Otra imagen</param>
		/// <returns>true si image2 es subindice de image1</returns>
		protected bool IsSubscript(MathTextBitmap image1, MathTextBitmap image2)
		{
			int posY1=image1.Position.Y;
			int posY2=image2.Position.Y;
			int h1=image1.Height;
			int h2=image2.Height;
			
			return posY2 > posY1 && posY2+h2 > posY1+h1; //&& posY2<posY1+h1;
		}
	}
}
