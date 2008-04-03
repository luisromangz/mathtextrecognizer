using System;

using MathTextLibrary;
using MathTextLibrary.Bitmap;

namespace MathTextLibrary.BitmapProcesses
{
	/// <summary>
	/// Esta clase ofrece una implentacion de IBitmapThresholder que 
	/// pasa una imagen en escala de grises a otra en blanco y negro
	/// usando un umbral fijo pasado como parametro.
	/// </summary>
	[BitmapProcessDescription("Binarizado de umbral fijo")]
	public class BitmapFixedThresholder : BitmapProcess
	{
		///Umbral a utilizar, [0,255]
		private int threshold;
		
		/// <summary>
		/// El constructor de la clases BitmapFixedThresholdThresholder.
		/// </summary>		
		public BitmapFixedThresholder()	: this(220)
		{
						
		}
		
		/// <summary>
		/// El constructor de la clases BitmapFixedThresholdThresholder.
		/// </summary>
		/// <param name="threshold">
		/// El umbral con el que decidiremos si un pixel es blanco o negro.
		/// </param>
		public BitmapFixedThresholder(int threshold)
		{
			this.threshold=threshold;			
		}

		/// <summary>
		/// Se aplica el algoritmo de binarización de umbral fijo.
		/// </summary>
		/// <param name = "image">
		/// La imagen que se binarizará.
		/// </param>
		/// <returns>
		/// La imagen binarizada.
		/// </returns>		
		public override float[,] Apply(float [,] image)
		{
			int nrows=image.GetLength(0);
			int ncols=image.GetLength(1);
			
			float [,] res = new float[nrows, ncols];
			
			float fthreshold = threshold / 255f;

			for (int i=0; i<nrows; i++)
			{
				for (int j=0; j<ncols; j++)
				{
					if (image[i,j] < fthreshold)
					{
						res[i,j] = MathTextBitmap.Black;
					}
					else
					{
						res[i,j] = MathTextBitmap.White;
					}
				}
			}
			
			return res;
		}
		
		/// <value>
		/// Contiene la cadena con los parametros del algormo formateado.
		/// </value>
		public override string Values 
		{
			get 
			{  
				return "Umbral: " + threshold;
			}
		}

		/// <value>
		/// Contiene el valor del limite considerado al binarizar.
		/// </value>
		[BitmapProcessPropertyDescription("Umbral",Max = 255, Min = 0)]
		public virtual int Threshold
		{
			get
			{
				return threshold;
			}
			
			set
			{
				threshold = value;
			}
		}
			
		
		

	}
}
