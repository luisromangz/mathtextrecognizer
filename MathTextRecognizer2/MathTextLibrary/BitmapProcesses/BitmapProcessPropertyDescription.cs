// BitmapProcessPropertyDescription.cs created with MonoDevelop
// User: luis at 16:49 03/04/2008


using System;

namespace MathTextLibrary.BitmapProcesses
{
	/// <summary>
	/// Esta clase define un atributo para ser usado como descripción para las
	/// propiedades de los procesados de imagenes de imagenes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true) ] 
	public class BitmapProcessPropertyDescription : Attribute
	{
		private string _description;
		private int _max;
		private int _min;
		
		public BitmapProcessPropertyDescription(string description)
		{
			_description = description;
			_max = -1;
			_min = -1;
		}
		
		/// <value>
		/// Contiene la descripcion del parametro.
		/// </value>
		public string Description
		{
			get
			{
				return _description;
			}
		}
		
		/// <value>
		/// Contiene el valor minimo del parametro.
		/// </value>
		public int Min
		{
			get{
				return _min;
			}
			set{
				_min = value;
			}
		}
		
		/// <value>
		/// Contiene el valor maximo del parametro.
		/// </value>
		public int Max
		{
			get{
				return _max;
			}
			
			set{
				_max = value;
			}
		}
	}
	
}
	
	