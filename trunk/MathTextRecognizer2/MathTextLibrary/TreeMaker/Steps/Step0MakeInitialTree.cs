using System;
using System.Collections.Generic;

namespace MathTextLibrary.TreeMaker.Steps
{
	/// <summary>
	/// Este paso crea el arbol inicial de <c>RecognizedTreeNode</c> a partir
	/// del arbol obtenido por segmentacion.
	/// </summary>
	/// <remarks>
	/// Esta clase no implementa la interfaz ITreeMakerStep porque al ser
	/// el paso inicial debe convertir el arbol de <c>MathTextBitmap</c>
	/// producido por la segmentacion en un arbol de <c>RecognizedTreeNode</c>.
	/// </remarks>
	public class Step0MakeInitialTree
	{
		public Step0MakeInitialTree()
		{
		}
		
		public RecognizedTreeNode ApplyStep(MathTextBitmap mtb)
		{
			List<RecognizedTreeNode> children=new List<RecognizedTreeNode>();
			if(mtb.Children!=null)
			{
				foreach(MathTextBitmap child in mtb.Children)
				{
					children.Add(ApplyStep(child));
				}
			}
			
			return new RecognizedTreeNode(mtb, children);
		}
	}
}
