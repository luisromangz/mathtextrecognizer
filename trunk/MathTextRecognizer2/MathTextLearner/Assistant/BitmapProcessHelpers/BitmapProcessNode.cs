//Creado por: Luis Román Gutiérrez a las 21:56 de 06/14/2007

using System;

using Gtk;

using MathTextLibrary.BitmapProcesses;

namespace MathTextLearner.Assistant.BitmapProcessHelpers
{
	/// <summary>
	/// Esta clase represnta los nodos de la vista de los procesados.
	/// </summary>
	public class BitmapProcessNode : TreeNode
	{
		private string description;
		
		private BitmapProcess process;
		
		
		/// <summary>
		/// El constructor de la clase toma un tipo, y creará una instancia
		/// de una subclase de <c>BitmapProcess</c>.
		/// </summary>
		public BitmapProcessNode(Type t, string description)
		{		
			this.description = description;
			
			process =
				(BitmapProcess)(t.GetConstructor(new Type[]{}).Invoke(null));
			
		}		
		
		[TreeNodeValue(Column=0)]
		public string ProcessDescription
		{
			get
			{
				return description;
			}
		}
		
		[TreeNodeValue(Column=1)]
		public string ProcessValues
		{
			get
			{
				return process.Values;
			}
		}
			
		public BitmapProcess Process
		{
				
			get
			{
				return process;
			}
		
		}
	}
}
