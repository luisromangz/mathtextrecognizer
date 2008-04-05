
using System;

using Gdk;

using MathTextLibrary;
using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Projection
{
	
	/// <summary>
	/// La clase <c>HorizontalBitmapProjection</c> especializa <c>BitmapProjection</c>
	/// para ofrecer proyecciones sobre el eje X.
	/// </summary>
	public class HorizontalBitmapProjection : BitmapProjection
	{

		internal HorizontalBitmapProjection(MathTextBitmap image)
			: base(image)
		{
		
		}		
		
		/// <summary>
		/// Crea la proyeccion horizontal de una imagen.
		/// </summary>
		/// <param name="image">
		/// La imagen a la que vamos a calcular la proyeccion.
		/// </param>
		protected override void CreateProjection(MathTextBitmap image)
		{	
			projection=new int [image.Width];
			for(int i=0;i<image.Width;i++)
			{
				for(int j=0;j<image.Height;j++)
				{
					if(image[i,j]!=MathTextBitmap.White)
					{
						projection[i]++;	
					}
				}						
			}
		}

		
		/// <summary>
		/// Crea una representacion grafica de la proyeccion horizontal.
		/// </summary>
		/// <returns>
		/// Un <c>Pixbuf</c> con la imagen que representa la proyeccion.
		/// </returns>
		/*protected override Pixbuf CreateBitmap()
		{
			int max=-1;
			foreach(int i in projection)
			{
				if(max<i)
				{
					max=i;
				}		
			}

			Pixbuf res=new Pixbuf(projection.Length,max);
			
			res.Fill(0x0);
			for(int j=0;j<projection.Length;j++)
			{
				.DrawLine(Pens.Black,j,0,j,projection[j]);					
			}				
			
			return res;
		}*/
	}
}
