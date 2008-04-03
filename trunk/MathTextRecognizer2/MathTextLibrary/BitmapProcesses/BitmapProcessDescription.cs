// BitmapProcessDescription.cs created with MonoDevelop
// User: luis at 16:50 03/04/2008

using System;

namespace MathTextLibrary.BitmapProcesses
{
	/// <summary>
	/// Esta clase define un atributo para ser usado como descripción para 
	/// los procesadoso de imagenes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, 
	                AllowMultiple = false, 
	                Inherited = true) ] 
	public class BitmapProcessDescription : Attribute
	{
		private string _description;
		
		public BitmapProcessDescription(string description)
		{
			_description = description;
		}
		
		/// <value>
		/// Contiene la descripcion del algoritmo de procesado.
		/// </value>
		public string Description
		{
			get
			{
				return _description;
			}
		}
	}
	
	
}
