
using System;
using System.IO;
using System.Text;
using MathTextLibrary.TreeMaker;
using MathTextLibrary;

namespace MathTextLibrary.Output
{
	/// <summary>
	/// Esta clase permite obtener un documento de LaTeX a partir del arbol
	/// de simbolos que representa una determinada formula.
	/// </summary>
	public class LaTeXGenerator
	{
		private RecognizedTreeNode tree;
		private string output;
		
		/// <summary>
		/// Construcctor de la clase <code>LaTeXGenerator</code>
		/// </summary>
		/// <param name="tree">
		/// La raiz del arbol a partir del cual se generara la salida.
		/// </param>
		public LaTeXGenerator(RecognizedTreeNode tree)
		{	
			this.tree=tree;
			
		}
		
		/// <summary>
		/// Devuelve la representacion LaTeX del arbol.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			GenerateLaTeX();
			return output;
		}
		
		/// <summary>
		/// Genera la salida LaTeX del arbol.
		/// </summary>
		private void GenerateLaTeX()
		{
			output="\\documentclass[10pt,a4paper]{article}\n";
			output+="\\usepackage[utf8]{inputenc}\n\n";			
			output+="\\begin{document}\n$";
			output+=GenerateLaTeXNode(tree);			
			output+="$\n\\end{document}";
		}
		
		/// <summary>
		/// Permite transformar el texto de un simbolo en una representacion
		/// adecuada a LaTeX.
		/// </summary>
		/// <param name="text">El texto del simbolo.</param>
		/// <returns>La secuencia de escape LaTeX para el simbolo.</returns>
		private string TranslateSymbolText(string text){
			string res;
			switch(text){				
				case("alpha"):
					res=@"\alpha";
					break;
				case("beta"):
					res=@"\beta";
					break;
				case("gamma"):
					res=@"\gamma";
					break;
				case("pi"):
					res=@"\pi";
					break;
				default:
					res	=text;	
					break;
			}
			return res+" ";
		}
		
		/// <summary>
		/// Crea una secuencia LaTeX a partir de un nodo que vamos tratando recursivamente.
		/// </summary>
		/// <param name="node">El nodo que tratamos.</param>
		/// <returns>La salida LaTeX del nodo e hijos.</returns>
		private string GenerateLaTeXNode(RecognizedTreeNode node)
		{
			string res ="";
			
			switch(node.Symbol.SymbolType)
			{
				case(MathSymbolType.Identifier):
					res+=TranslateSymbolText(node.Symbol.Text);					
					break;
				case(MathSymbolType.Number):
					res+=TranslateSymbolText(node.Symbol.Text);					
					break;
				case(MathSymbolType.Operator):					
					res+=TranslateSymbolText(node.Symbol.Text);					
					break;
				case(MathSymbolType.Fraction):
					res+=@"{\frac{";					
					res+=GenerateLaTeXNode(node[0]);
					res+="}{";					
					res+=GenerateLaTeXNode(node[1]);
					res+="}}";					
					break;
				case(MathSymbolType.LeftDelimiter):
					res+=@"{\left ";
					res+=TranslateSymbolText(node.Symbol.Text);
					res+=" ";
					break;
				case(MathSymbolType.RightDelimiter):
					res+=@"\right ";
					res+=node.Symbol.Text;
					res+=" }";
					break;
				case(MathSymbolType.Superindex):					
					res+=GenerateLaTeXNode(node[0]);
					res+="^";
					res+=GenerateLaTeXNode(node[1]);
					
					break;
				case(MathSymbolType.Subindex):
					res+="{";
					res+=GenerateLaTeXNode(node[0]);
					res+="}_{";
					res+=GenerateLaTeXNode(node[1]);
					res+="}";
					break;
				case(MathSymbolType.NotRecognized):
					foreach(RecognizedTreeNode child in node.Children)
					{
						res+=GenerateLaTeXNode(child);
					}
					break;
			}
			
			return res;
		}
	}
}
