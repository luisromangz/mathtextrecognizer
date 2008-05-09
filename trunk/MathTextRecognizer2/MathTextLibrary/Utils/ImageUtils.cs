
using System;

using Gdk;

namespace MathTextLibrary.Utils
{
	
	/// <summary>
	/// Esta clase contiene métodos estáticos de utilidad para el uso
	/// de imagenes.
	/// </summary>
	public class ImageUtils
	{
	
		
	
		
		
		/// <summary>
		/// Crea una imagen mas pequeña a partir de otra.
		/// </summary>
		/// <param name="image">
		/// La imagen a la que se le quiere hacer una previsualización.
		/// </param>
		/// <param name="size">
		/// El tamaño de la previsualización.
		/// </param>
		public static Gdk.Pixbuf MakeThumbnail(Gdk.Pixbuf image, int size)
		{
			float scaleX, scaleY;
			
			// La escalamos para que no se distorsione.
			if(image.Width > image.Height)
			{
				scaleX = 1;
				scaleY = (float)(image.Height)/image.Width;
			}
			else
			{
				scaleX = (float)(image.Width)/image.Height;
				scaleY = 1;
			}
			
			Pixbuf res = image.ScaleSimple((int)(size*scaleX),
			                               (int)(size*scaleY), 
			                               Gdk.InterpType.Nearest);
				
			return res;
			
		}
		
	}
}
