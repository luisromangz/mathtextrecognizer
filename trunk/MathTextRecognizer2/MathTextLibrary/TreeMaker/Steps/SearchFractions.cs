using System;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;

namespace MathTextLibrary.TreeMaker.Steps
{
	/// <summary>
	/// Este paso busca fracciones en el arbol de entrada y crea un arbol
	/// nuevo cuya conversion a texto es mas sencilla.
	/// </summary>
	/// <remarks>
	/// Al encontrar una fraccion se introduce un nodo sin imagen de
	/// simbolo <c>MathSymbolType.Fraction</c> que tendra exactamente dos
	/// hijos: el primero es el numerador y el segundo el denominador.
	/// </remarks>
	public class SearchFractions: ITreeMakerStep
	{
		public SearchFractions()
		{
		}
		
		public RecognizedTreeNode ApplyStep(RecognizedTreeNode tree)
		{
			bool fraction=false;
			List<RecognizedTreeNode> newChildren=new List<RecognizedTreeNode>();
			
			for(int i=0; i<tree.Children.Count; i++)
			{
				newChildren.Add(this.ApplyStep(tree[i]));
			}
			
			for(int i=0; i<newChildren.Count-2; i++)
			{
				RecognizedTreeNode first=(RecognizedTreeNode)newChildren[i];
				RecognizedTreeNode second=(RecognizedTreeNode)newChildren[i+1];
				RecognizedTreeNode third=(RecognizedTreeNode)newChildren[i+2];
				if(IsFraction(first.Image,second.Image)
					&& IsFraction(third.Image,second.Image))
				{
					fraction=true;
					newChildren.RemoveAt(i+1);
					break;
				}
			}
			
			if(fraction)
			{
				MathTextBitmap image=tree.Image;
				image.Symbol=new MathSymbol("-",MathSymbolType.Fraction);
				return new RecognizedTreeNode(tree.Image,newChildren);
			}
			else
			{
				return new RecognizedTreeNode(tree.Image,newChildren);
			}
		}
		
		/// <summary>
		/// Este metodo determina si <c>image2</c> es una raya de fraccion
		/// respecto de <c>image1</c>.
		/// </summary>
		/// <returns>true si <c>image2</c> es una raya de fraccion
		/// respecto de <c>image1</c></returns>.
		private bool IsFraction(MathTextBitmap image1, MathTextBitmap image2)
		{
			return image2.Symbol.Text.Equals("-")
				&& image2.Position.X <= image1.Position.X
				&& image2.Position.X+image2.Width >= image1.Position.X+image1.Width;
		}
	}
}
