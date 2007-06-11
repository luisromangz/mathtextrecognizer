// created on 06/01/2006 at 17:07
using System;
using System.IO;
using System.Text;
using MathTextLibrary.TreeMaker;
using MathTextLibrary;

namespace MathTextLibrary.Output
{
	/// <summary>
	/// Esta clase nos permite obtener un documento MathML a partir
	/// del arbol obtenido a partir de la imagen de una formula.
	/// </summary>
	public class MathMLGenerator
	{
		private RecognizedTreeNode tree;
		private StringWriter writer;
		
		/// <summary>
		/// Constructor de la clase <code>MathMLGenerator</code>.
		/// </summary>
		/// <param name="tree">
		/// El nodo del arbol a partir del cual se generara el documento MathML.
		/// </param>
		public MathMLGenerator(RecognizedTreeNode tree)
		{
			this.tree=tree;
			this.writer=new StringWriter();
		}
		
		
		/// <summary>
		/// Devuelve la representacion MathML del arbol
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			GenerateMathML();
			return writer.ToString();
		}
		
		/// <summary>
		/// Calcula el documento MathML que representa el arbol tree
 		/// escribiendo el resultado en writer
		/// </summary>
		public void GenerateMathML()
		{
			writer.WriteLine(@"<math xmlns=""http://www.w3.org/1998/Math/MathML"">");
			writer.WriteLine(@"<mrow>");
			writer.Write(GenerateMathMLNode(tree));
			writer.WriteLine(@"</mrow>");
			writer.WriteLine(@"</math>");
		}
		
		/// <summary>
		/// Permite obtener una representacion adecuada para MathML de algunos simbolos especiales.
		/// </summary>
		/// <param name="text">El texto del simbolo a traducir.</param>
		/// <returns>La representacion MathML del simbolo.</returns>
		private string TranslateSymbolText(string text){
			string res;
			switch(text){
				case("<"):
					res="&lt;";
					break;
				case(">"):
					res="&gt;";
					break;
				case("alpha"):
					res="&alpha;";
					break;
				case("beta"):
					res="&beta;";
					break;
				case("gamma"):
					res="&gamma;";
					break;
				case("pi"):
					res="&pi;";
					break;
				default:
					res	=text;	
					break;
			}
			return res;
		}
		
		/// <summary>
		/// Crea un nodo MathML a partir del nodo del arbol que estamos tratando,
		/// de manera recursiva.
		/// </summary>
		/// <param name="node">El nodo cuyo MathML vamos a generar.</param>
		/// <returns>El texto MathML del nodo y sus hijos.</returns>
		public string GenerateMathMLNode(RecognizedTreeNode node)
		{
			StringBuilder text=new StringBuilder();
			
			switch(node.Symbol.SymbolType)
			{
				case(MathSymbolType.Identifier):
					text.Append("<mi>");
					text.Append(TranslateSymbolText(node.Symbol.Text));
					
					text.Append("</mi>\n");
					break;
				case(MathSymbolType.Number):
					text.Append("<mn>");
					text.Append(TranslateSymbolText(node.Symbol.Text));
					text.Append("</mn>\n");
					break;
				case(MathSymbolType.Operator):
					text.Append("<mo>");
					text.Append(TranslateSymbolText(node.Symbol.Text));
					text.Append("</mo>\n");
					break;
				case(MathSymbolType.Fraction):
					text.Append("<mfrac>\n");
					text.Append("<mrow>\n");
					text.Append(GenerateMathMLNode(node[0]));
					text.Append("</mrow>\n");
					text.Append("<mrow>\n");
					text.Append(GenerateMathMLNode(node[1]));
					text.Append("</mrow>\n");
					text.Append("</mfrac>\n");
					break;
				case(MathSymbolType.LeftDelimiter):
					text.Append("<mo>");
					text.Append(TranslateSymbolText(node.Symbol.Text));
					text.Append("</mo>\n");
					break;
				case(MathSymbolType.RightDelimiter):
					text.Append("<mo>");
					text.Append(node.Symbol.Text);
					text.Append("</mo>\n");
					break;
				case(MathSymbolType.Superindex):
					text.Append("<msup>\n");
					text.Append(GenerateMathMLNode(node[0]));
					text.Append(GenerateMathMLNode(node[1]));
					text.Append("</msup>\n");
					break;
				case(MathSymbolType.Subindex):
					text.Append("<msub>\n");
					text.Append(GenerateMathMLNode(node[0]));
					text.Append(GenerateMathMLNode(node[1]));
					text.Append("</msub>\n");
					break;
				case(MathSymbolType.NotRecognized):
					foreach(RecognizedTreeNode child in node.Children)
					{
						text.Append(GenerateMathMLNode(child));
					}
					break;
			}
			
			return text.ToString();
		}
		
		
	}
}
