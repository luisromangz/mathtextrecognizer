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
		public static int NumBlackPixelsRow(MathTextBitmap image, int row) 
		{
			float[,] im=image.ProcessedImage;
			int nBlackPixels=0;

			for(int i=0; i<im.GetLength(0); i++) 
			{
				if(im[i,row] == MathTextBitmap.Black)
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
		public static int NumBlackPixelsColumn(MathTextBitmap image, int column) 
		{
			float[,] im=image.ProcessedImage;
			int nBlackPixels=0;

			for(int i=0; i<im.GetLength(1); i++) 
			{
				if(im[column,i] == MathTextBitmap.Black)
				{
					nBlackPixels++;
				}
			}

			return nBlackPixels;
		}
	}
}
