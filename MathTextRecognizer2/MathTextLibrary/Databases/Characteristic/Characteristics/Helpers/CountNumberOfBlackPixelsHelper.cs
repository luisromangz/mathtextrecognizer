using System;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Databases.Characteristic.Characteristics.Helpers
{
	/// <summary>
	/// Esta clase cuenta el numero de pixeles negros en una fila
	/// o columna de una imagen.
	/// </summary>
	public class CountNumberOfBlackPixelsHelper
	{
		public CountNumberOfBlackPixelsHelper()
		{
		}

		/// <summary>
		/// Cuenta el numero de pixeles negros en una imagen en la
		/// fila indicada.
		/// </summary>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <param name="row">Fila a analizar</param>
		/// <returns>Numero de pixeles negros</returns>
		public static int NumBlackPixelsRow(FloatBitmap image, int row) 
		{
			int nBlackPixels=0;

			for(int i=0; i<image.Width; i++) 
			{
				if(image[i,row] != FloatBitmap.White)
				{
					nBlackPixels++;
				}
			}

			return nBlackPixels;
		}

		/// <summary>
		/// Cuenta el numero de pixeles negros en una imagen en la
		/// columna indicada.
		/// </summary>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <param name="column">Columna a analizar</param>
		/// <returns>Numero de pixeles negros</returns>
		public static int NumBlackPixelsColumn(FloatBitmap image, int column) 
		{
			
			int nBlackPixels=0;

			for(int i=0; i<image.Height; i++) 
			{
				if(image[column,i] != FloatBitmap.White)
				{
					nBlackPixels++;
				}
			}

			return nBlackPixels;
		}
	}
}
