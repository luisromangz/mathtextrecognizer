
using System;

using MathTextLibrary.Bitmap;

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
		public abstract FloatBitmap Apply(FloatBitmap image);	
		
		/// <value>
		/// Permite recuperar la cadena formateada con los paremetros del
		/// algoritmo.
		/// </value>
		public abstract string Values
		{
			get;
			
		}
	}
	
	
	
	
}
