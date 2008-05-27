
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
			float scale;
			
			// La escalamos para que no se distorsione.
			if(image.Width > image.Height)
			{
				scale = (float)(size)/image.Width;
			}
			else
			{
				scale = (float)(size)/image.Height;
			}
			
			int newWidth = (int)(scale*image.Width);
			int newHeight = (int)(scale*image.Height);
			
			Pixbuf res = 
				new Pixbuf(image.Colorspace, image.HasAlpha, image.BitsPerSample, size, size);
			
			res.Fill(0xFFFFFFFF);
				
			image.Scale(res,
			            0,0,
			            size, size,
			            (size -newWidth)/2,(size-newHeight)/2,
			            scale, scale, 
			            Gdk.InterpType.Bilinear );
			
				
			return res;
			
		}
		
	}
}
