
using System;

using Gdk;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Projection
{
	/// <summary>
	/// La clase <c>VerticalBitmapProjection</c> especializa <c>BitmapProjection</c>
	/// para generar proyecciones sobre el eje Y.
	/// </summary>
	public class VerticalBitmapProjection:BitmapProjection
	{
		
		internal VerticalBitmapProjection(MathTextBitmap image)
			: base(image)
		{
		
		}
	
		/// <summary>
		/// Crea la proyeccion vertical de la imagen.
		/// </summary>
		/// <param name="image">
		/// La imagen a la que se calcula su proyeccion vertical.
		/// </param>
		protected override void CreateProjection(MathTextBitmap image)
		{			
			projection=new int [image.Height];
			for(int i=0;i<image.Height;i++)
			{
				for(int j=0;j<image.Width;j++)
				{
					if(image[j,i]!=FloatBitmap.White)
					{
						projection[i]++;	
					}
				}						
			}
		}
	}
}
