using System;

namespace MathTextLibrary.BitmapProcesses
{
	/// <summary>
	/// Esta clase es una implemenacion de BitmapProcess
	/// que realiza la binarizacion usando el metodo de los
	/// dos picos, tal y como aparece en Parker.
	/// </summary>
	[BitmapProcessDescription("Binarizado de los dos picos")]
	public class BitmapTwoPeaksThresholder: BitmapProcess
	{
		/// <summary>
		/// El constructor de la clase BitmapTwoPeaksThresholder.
		/// </summary>
		public BitmapTwoPeaksThresholder()
		{
		}	
		

		/// <summary>
		/// Binariza por el metodo de los dos picos.
		/// </summary>
		/// <param name="image">
		/// La imagen que queremos binarizar,en formato de matriz bidimensional.
		/// </param>		
		/// <returns>
		/// La imagen binarizada.
		/// </returns>
		public override float[,] Apply(float [,] image)
		{
			int nrows=image.GetLength(0);
			int ncols=image.GetLength(1);

			int i,j,k,  t= -1;
			int [] hist =new int[256];


			/// Calculamos el histograma de la imagen.
			for (i=0; i<256; i++)
				hist[i] = 0;

			
			for (i=0; i<nrows; i++)
			{
				for (j=0; j<ncols; j++)
				{
					hist[(int)(255*image[i,j])] ++;
				}
			}

			
			j = 0;
			for (i=0; i<256; i++)
				if (hist[i] > hist[j]) 
					j = i;

			
			k = 0;
			for (i=0; i<256; i++)
			{
				if (i>0 && hist[i-1]<=hist[i] && i<255 && hist[i+1]<=hist[i])
				{
					if ((k-j)*(k-j)*hist[k] < (i-j)*(i-j)*hist[i])
					{
						k = i;
					}
				}
			}

			t = j;
			if (j<k)
			{
				for (i=j; i<k; i++)
				{
					if (hist[i] < hist[t])
						t = i;
				}
			} 
			else 
			{
				for (i=k; i<j; i++)
				{
					if (hist[i] < hist[t])
						t = i;
				}
			}	
		
			float [,] res = new float[nrows, ncols];
			for (i=0; i<nrows; i++)
			{
				for (j=0; j<ncols; j++)
					if (image[i,j] < t)
					{
						res[i,j] = MathTextBitmap.Black;
					}
					else
					{
						res[i,j] = MathTextBitmap.White;
					}
			}
			
			return res;
		}
	}
}
