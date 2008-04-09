// created on 28/12/2005 at 13:23
using System.Collections.Generic;
using Gdk;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.BitmapSegmenters{
	/// <summary>
	/// Esta interfaz define los metodos comunes a todas las
	/// clases que implementan metodos para descomponer 
	/// formulas en sus distintas partes.
	/// </summary>
	public abstract class BitmapSegmenter
	{
		
		/// <summary>
		/// El metodo que sera invocado para intentar descomponer
		/// una imagen.
		/// </summary>
		/// <param name="mtb">La imagen que queremos descomponer.</param>
		/// <returns>
		/// Un array con las distintas partes en que se
		/// ha descompuesto la imagen.
		/// </returns>
		public abstract List<MathTextBitmap> Segment(MathTextBitmap mtb);
		
		
		
		/// <summary>
		/// Enmarca una imagen.
		/// </summary>
		/// <param name="image">
		/// La imagen a enmarcar.
		/// </param>
		/// <param name="pos">
		/// La esquina de la zona a recortar.
		/// </param>
		/// <param name="size">
		/// El tamaño de la zona a recortar.
		/// </param>
		protected void GetEdges(FloatBitmap image, out Point pos, out Size size)
		{
			pos = new Point(0,0);
			size = new Size(image.Width, image.Height);
			
			bool found =false;
			
			for(int i = 0; i < image.Width && !found; i++)
			{
				for(int j = 0; j < image.Height && !found; j++)
				{
					if (image[i, j] != FloatBitmap.White)
					{
						pos.X = i-1;
						found = true;
					}
				}
			}
			
			found =false;
			for(int i = image.Width-1; i >=0 && !found; i--)
			{
				for(int j = 0; j < image.Height && !found; j++)
				{
					if (image[i, j] != FloatBitmap.White)
					{
						size.Width = i - pos.X +2 ;
						found = true;
					}
				}
			}
			
			found =false;
			for(int j = 0; j < image.Height && !found; j++)
			{
				for(int i = 0; i < image.Width&& !found; i++)
				{
					if (image[i, j] != FloatBitmap.White)
					{
						pos.Y = j-1;
						found = true;
					}
				}
			}
			
			found =false;
			for(int j = image.Height-1; j >=0 && !found; j--)
			{
				for(int i = 0; i < image.Width && !found; i++)
				{
					if (image[i, j] != FloatBitmap.White)
					{
						size.Height = j - pos.Y +2;
						found = true;
					}
				}
			}
		}
	}

	
}
