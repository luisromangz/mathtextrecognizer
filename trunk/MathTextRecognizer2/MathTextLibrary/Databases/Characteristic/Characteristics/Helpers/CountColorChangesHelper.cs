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
		public static int NumColorChangesRow(FloatBitmap image, int row) 
		{
			
			int nChanges=0;

			for(int i=1; i<image.Width; i++) 
			{
				if(image[i,row]!=image[i-1,row])
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
		public static int NumColorChangesColumn(FloatBitmap image, int column) 
		{
			int nChanges=0;

			for(int j=1; j<image.Height; j++) 
			{				
				if(image[column,j] != image[column,j-1])
				{
					nChanges++;
				}
			}

			return nChanges;
		}
	}
}
