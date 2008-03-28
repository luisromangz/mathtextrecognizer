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
		/// <param name="t">
		/// El tipo del algorimto a que contiene el nodo.
		/// </param>
		/// <param name="description">
		/// La descripcion del algoritmo que contiene el nodo.
		/// </param>
		public BitmapProcessNode(Type t, string description)
		{		
			this.description = description;
			
			process =
				(BitmapProcess)(t.GetConstructor(new Type[]{}).Invoke(null));
			
		}	
		
		/// <summary>
		/// El constructor de la clase toma una instancia de<c>BitmapProcess</c>.
		/// para crear un nodo que la contenga.
		/// </summary>
		/// <param name="bp">
		/// El algoritmo a contener.
		/// </param>
		/// <param name="description">
		/// La descripcion del tipo del algoritmo.
		/// </param>
		public BitmapProcessNode(BitmapProcess bp, string description)
		{		
			this.description = description;
			
			this.process = bp; 
			
		}
		
		/// <summary>
		/// Permite recuperar la descripcion del algoritmo de procesado.
		/// </summary>
		[TreeNodeValue(Column=0)]
		public string ProcessDescription
		{
			get
			{
				return description;
			}
		}
		
		/// <summary>
		/// Permite recuperar los valores de los parametros del algoritmo,
		/// en forma de cadena formateada.
		/// </summary>
		[TreeNodeValue(Column=1)]
		public string ProcessValues
		{
			get
			{
				return process.Values;
			}
		}
			
		/// <value>
		/// Permite recuperar el algoritmo de preocesado de imagenes
		/// </value>
		public BitmapProcess Process
		{				
			get
			{
				return process;
			}
		
		}
	}
}
