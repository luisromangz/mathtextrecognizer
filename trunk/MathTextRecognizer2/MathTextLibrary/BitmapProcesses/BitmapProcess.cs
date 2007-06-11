
using System;

namespace MathTextLibrary.BitmapProcesses
{
	
	[BitmapProcessDescription("Descripción por defecto de procesado de imagen")]
	public abstract class BitmapProcess
	{
		/// <summary>
		/// Este método aplica el algoritmo del proceso a la imagen.
		/// </summary>
		/// <param name = "image">
		/// La imagen en formato de matriz bidimensional de floats entre 0 y 1.
		/// </param>		
		/// <returns>
		/// La imagen con el procesamiento aplicado.
		/// </returns>
		public abstract float[,] Apply(float[,] image);	
		
		public virtual string Values
		{
			get
			{
				return "";
			}
			
		}
	}
	
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
		
		public string Description
		{
			get
			{
				return _description;
			}
		}
	}
	
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
		
		public string Description
		{
			get
			{
				return _description;
			}
		}
		
		public int Min
		{
			get{
				return _min;
			}
			set{
				_min = value;
			}
		}
		
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
