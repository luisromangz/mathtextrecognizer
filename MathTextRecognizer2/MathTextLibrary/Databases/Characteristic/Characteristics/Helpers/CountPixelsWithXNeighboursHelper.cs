using System;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Databases.Characteristic.Characteristics.Helpers
{
	/// <summary>
	/// Esta clase cuenta el numero de pixeles negros con un numero
	/// determinado de vecinos (en 8-adyacencia).
	/// </summary>
	public class CountPixelsWithXNeighboursHelper
	{
		public CountPixelsWithXNeighboursHelper()
		{
		}
		
		/// <summary>
		/// Cuenta el numero de pixeles negros en la imagen que tengan
		/// <c>neighbours</c> vecinos.
		/// </summary>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <param name="neighbours">Numero de vecinos</param>
		/// <returns>Numero de pixeles negros con <c>neighbours</c> vecinos</returns>
		public static int CountPixelsXNeighbours(FloatBitmap image, int neighbours)
		{
			int count=0;
			
			int width = image.Width;
			int height = image.Height;

			for(int i=0;i<width;i++)
			{
				for(int j=0;j<height;j++)
				{
					if(image[i,j]==FloatBitmap.Black
							&& CountBlackNeighboursHelper.BlackNeighbours(image,i,j)==neighbours)
					{
						
						count++;
					}
				}
			}

			return count;
		}

		/// <summary>
		/// Cuenta el numero de pixeles negros en la imagen que tengan
		/// <c>neighbours</c> o mas vecinos.
		/// </summary>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <param name="neighbours">Numero de vecinos</param>
		/// <returns>Numero de pixeles negros con <c>neighbours</c> o mas
		/// vecinos</returns>
		public static int CountPixelsXOrMoreNeighbours(FloatBitmap image, 
		                                               int neighbours)
		{
			int count=0;
			
			int width=image.Width;
			int height = image.Height;

			for(int i=0;i<width;i++)
			{
				for(int j=0;j<height;j++)
				{
					if(image[i,j]==FloatBitmap.Black
							&& CountBlackNeighboursHelper.BlackNeighbours(image,i,j)>=neighbours)
					{
						count++;
					}
				}
			}
			
			return count;
		}
	}
}
