using System;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers
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
		public static int numPixelsXNeighbours(MathTextBitmap image, int neighbours)
		{
			int count=0;
			int size=image.ProcessedImageSize;
			float[,] im=image.ProcessedImage;

			for(int i=0;i<size;i++)
				for(int j=0;j<size;j++)
					if(im[i,j]==MathTextBitmap.Black
							&& CountBlackNeighboursHelper.BlackNeighbours(im,i,j,size,size)==neighbours)
						count++;

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
		public static int numPixelsXOrMoreNeighbours(MathTextBitmap image, int neighbours)
		{
			int count=0;
			int size=image.ProcessedImageSize;
			float[,] im=image.ProcessedImage;

			for(int i=0;i<size;i++)
				for(int j=0;j<size;j++)
					if(im[i,j]==MathTextBitmap.Black
							&& CountBlackNeighboursHelper.BlackNeighbours(im,i,j,size,size)>=neighbours)
						count++;
			
			return count;
		}
	}
}
