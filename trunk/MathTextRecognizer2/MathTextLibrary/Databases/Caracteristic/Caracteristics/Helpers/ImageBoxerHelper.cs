using System;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers
{
	/// <summary>
	/// Esta clase obtiene el menor rectángulo que contiene los pixeles
	/// negros de la imagen.
	/// </summary>
	public class ImageBoxerHelper
	{
		public ImageBoxerHelper()
		{
		}

		/// <summary>
		/// Obtiene el menor rectangulo que contiene los pixeles negros de
		/// la imagen y devuelve sus esquinas superior izquierda e
		/// inferior derecha como (x1,y1) y (x2,y2) respectivamente.
		/// </summary>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <param name="x1">Minima coordenada horizontal</param>
		/// <param name="y1">Minima coordenada vertical</param>
		/// <param name="x2">Maxima coordenada horizontal</param>
		/// <param name="y2">Maxima coordenada vertical</param>
		public static void BoxImage(MathTextBitmap image, out int x1, out int y1, out int x2, out int y2)
		{
			float[,] im=image.ProcessedImage;
			
			BoxImage(im,out x1,out y1,out x2,out y2);
		}
		
		/// <summary>
		/// Obtiene el menor rectangulo que contiene los pixeles negros de
		/// la imagen y devuelve sus esquinas superior izquierda e
		/// inferior derecha como (x1,y1) y (x2,y2) respectivamente.
		/// </summary>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <param name="x1">Minima coordenada horizontal</param>
		/// <param name="y1">Minima coordenada vertical</param>
		/// <param name="x2">Maxima coordenada horizontal</param>
		/// <param name="y2">Maxima coordenada vertical</param>
		public static void BoxImage(float[,] image, out int x1, out int y1, out int x2, out int y2)
		{
			x1 = FindLeft(image);
			y1 = FindTop(image);
			x2 = FindRight(image);
			y2 = FindBottom(image);
		}
		
		/// <summary>
		/// Obtiene la coordenada superior del minimo rectangulo que contiene
		/// todos los pixeles negros de la imagen.
		/// </summary>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <returns>Coordenada del borde superior del rectangulo que
		/// contiene los pixeles negros de la imagen</returns>
		/// <exception "System.ApplicationException">Lanzada si no se encuentra
		/// ningun pixel negro en la imagen</exception>
		private static int FindTop(float [,] image) 
		{
			int width=image.GetUpperBound(0)+1;
			int height=image.GetUpperBound(1)+1;
			
			for(int i=0;i<height;i++)
			{
				for(int j=0;j<width;j++)
				{
					if(image[j,i]==MathTextBitmap.Black)
					{
						return i;
					}
				}
			}
			
			throw new ApplicationException("No se ha encontrado ningun pixel negro en ImageBoxerHelper.FindTop()!");
		}

		/// <summary>
		/// Obtiene la coordenada inferior del minimo rectangulo que contiene
		/// todos los pixeles negros de la imagen.
		/// </summary>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <returns>Coordenada del borde inferior del rectangulo que
		/// contiene los pixeles negros de la imagen</returns>
		/// <exception "System.ApplicationException">Lanzada si no se encuentra
		/// ningun pixel negro en la imagen</exception>
		private static int FindBottom(float [,] image) 
		{
			int width=image.GetUpperBound(0)+1;
			int height=image.GetUpperBound(1)+1;
			
			for(int i=height-1;i>=0;i--)
			{
				for(int j=0;j<width;j++)
				{
					if(image[j,i]==MathTextBitmap.Black)
					{
						return i;
					}
				}
			}
			throw new ApplicationException("No se ha encontrado ningun pixel negro en ImageBoxerHelper.FindBottom()!");
		}

		/// <summary>
		/// Obtiene la coordenada izquierda del minimo rectangulo que contiene
		/// todos los pixeles negros de la imagen.
		/// </summary>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <returns>Coordenada del borde izquierdo del rectangulo que
		/// contiene los pixeles negros de la imagen</returns>
		/// <exception "System.ApplicationException">Lanzada si no se encuentra
		/// ningun pixel negro en la imagen</exception>
		private static int FindLeft(float [,] image) 
		{
			int width=image.GetUpperBound(0)+1;
			int height=image.GetUpperBound(1)+1;

			for(int i=0;i<width;i++)
			{
				for(int j=0;j<height;j++)
				{
					if(image[i,j]==MathTextBitmap.Black)
					{
						return i;
					}
				}
			}
			throw new ApplicationException("No se ha encontrado ningun pixel negro en ImageBoxerHelper.FindLeft()!");
		}

		/// <summary>
		/// Obtiene la coordenada derecha del minimo rectangulo que contiene
		/// todos los pixeles negros de la imagen.
		/// </summary>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <returns>Coordenada del borde derecho del rectangulo que
		/// contiene los pixeles negros de la imagen</returns>
		/// <exception "System.ApplicationException">Lanzada si no se encuentra
		/// ningun pixel negro en la imagen</exception>
		private static int FindRight(float [,] image) 
		{
			int width=image.GetUpperBound(0)+1;
			int height=image.GetUpperBound(1)+1;

			for(int i=width-1;i>=0;i--)
			{
				for(int j=0;j<height;j++)
				{
					if(image[i,j]==MathTextBitmap.Black)
					{
						return i;
					}
				}
			}
			throw new ApplicationException("No se ha encontrado ningun pixel negro en ImageBoxerHelper.FindRight()!");
		}
	}
}
