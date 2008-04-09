using System;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Databases.Characteristic.Characteristics.Helpers
{
	/// <summary>
	/// Esta clase cuenta el numero de cambios de color blanco-negro y
	/// negro-blanco en una fila o columna de una imagen. 
	/// </summary>
	/// <remarks>
	/// Entendemos que se produce un cambio de color cuando un pixel
	/// es blanco y el siguiente negro, o viceversa.
	/// </remarks>
	public class CountColorChangesHelper
	{
		public CountColorChangesHelper()
		{
		}

		/// <summary>
		/// Cuenta el numero de cambios blanco-negro en una imagen en la
		/// fila indicada.
		/// </summary>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <param name="row">Fila a analizar</param>
		/// <returns>Numero de cambios de color</returns>
		public static int NumColorChangesRow(MathTextBitmap image, int row) 
		{
			FloatBitmap im=image.LastProcessedImage;
			int nChanges=0;

			for(int i=1; i<im.Height; i++) 
			{
				if(im[row,i]!=im[row,i-1])
				{
					nChanges++;
				}
			}
			return nChanges;
		}

		/// <summary>
		/// Cuenta el numero de cambios blanco-negro en una imagen en la
		/// columna indicada.
		/// </summary>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <param name="column">Columna a analizar</param>
		/// <returns>Numero de cambios de color</returns>
		public static int NumColorChangesColumn(MathTextBitmap image, int column) 
		{
			FloatBitmap im=image.LastProcessedImage;
			int nChanges=0;

			for(int i=1; i<im.Width; i++) 
			{
				
				if(im[i,column]!=im[i-1,column])
				{
					nChanges++;
				}
			}

			return nChanges;
		}
	}
}
