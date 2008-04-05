
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
					if(image[j,i]!=MathTextBitmap.White)
					{
						projection[i]++;	
					}
				}						
			}
		}

		/// <summary>
		/// Crea una representacion grafica de la proyeccion vertical.
		/// </summary>
		/// <returns>Un <c>Bitmap</c> con la imagen que representa la proyeccion.</returns>
		/*protected override Pixbuf CreateBitmap()
		{
			int max=-1;
			foreach(int i in projection)
			{
				if(max<i){
					max=i;
				}		
			}

			Bitmap res=new Bitmap(max,projection.Length);
			using(Graphics g=Graphics.FromImage(res)){
				g.Clear(Color.White);
				for(int j=0;j<projection.Length;j++){
					g.DrawLine(Pens.Black,0,j,projection[j],j);					
				}				
			}
			return res;
		}*/
	
	}
}
