using System;

using MathTextLibrary;
using MathTextLibrary.Bitmap;

namespace MathTextLibrary.BitmapProcesses
{
	/// <summary>
	/// Esta clase ofrece una implementacion de BitmapProcess, 
	/// en la que el adelgazado se realiza siguiendo el metodo
	/// de Stentiford, tal como aparece en Parker.
	/// </summary>
	[BitmapProcessDescription("Adelgazado de Stentiford")]
	public class BitmapStentifordThinner : BitmapProcess
	{
		//La altura de la imagen a adelgazar
		int nrows;
		//La anchura de la imagen a adelgazar
		int ncols;
		
		//Indica si realizamos un suavizado previo o no
		bool presmooth;

		/// <summary>
		/// El constructor de la clase BitmapStentifordThinner.
		/// </summary>		
		public BitmapStentifordThinner()
			: this(false)
		{
           
		}
		
		/// <summary>
		/// El constructor de la clase BitmapStentifordThinner.
		/// </summary>
		/// <param name="presmooth">
		/// Si vale cierto, realizaremos un suavizado antes de adelgazar.
		/// </param>
		public BitmapStentifordThinner(bool presmooth)
		{
            this.presmooth=presmooth;
		}
		
		/// <value>
		/// Contiene la cadena con los parametros del algoritmo formateado.
		/// </value>
		public override string Values
		{
			get
			{
				return "Pre-suavizado: "+(presmooth?"Sí":"No");
			}
		}

		/// <value>
		/// Contiene el valor que indica si se usara el pre-suavizado al
		/// aplicar el algoritmo.
		/// </value>
		[BitmapProcessPropertyDescription("Pre-suavizado")]
		public virtual bool PreSmoothing {
			get {
				return presmooth;
			}
			set {
				presmooth = value;
			}
		}
		
		/// <summary>
		/// Este metodo se invoca para adelgazar una imagen.
		/// </summary>
		/// <param name="image">
		/// La imagen que queremos adelgazar, en formato de matriz  bidimensional.
		/// </param>
		/// <returns>
		/// La imagen adelgazada.
		/// </returns>
		public override FloatBitmap Apply(FloatBitmap image)
		{
			//Seguimos el algoritmo que aparece en Parker, reescrito en CSharp
			
			FloatBitmap im;
			int i,j;

			nrows=image.Width+2;
			ncols=image.Height+2;

			im = new FloatBitmap(nrows,ncols);
			for (i=0; i<nrows-2; i++)
				for (j=0; j<ncols-2; j++)
					im[i+1,j+1] = image[i,j];

			for (i=0; i<nrows; i++) 
			{
				im[i,0] = 1;
				im[i,ncols-1] = 1;
			}
			for (j=0; j<ncols; j++)
			{
				im[0,j] = 1;
				im[nrows-1,j] = 1;
			}

			if(presmooth)
			{
				PreSmooth(im);
			}

			ThinStentiford(im);
			
			FloatBitmap res = new FloatBitmap(nrows - 2, ncols -2);

			for (i=0; i<nrows-2; i++)
			{
				for (j=0; j<ncols-2; j++)
				{
					res[i,j] = im[i+1,j+1];
				}
			}
			
			return res;
		}

		/// <summary>
		/// Este metodo es el metodo auxiliar que realiza el adelgazamiento.
		/// </summary>
		/// <param name="im">La imagen a adelgazar.</param>
		private void ThinStentiford (FloatBitmap im)
		{
			int i,j;
			bool again=true;

			/* BLACK = 0, WHITE = 1. */
			for (i=0; i<nrows; i++)
			{
				for (j=0; j<ncols; j++)
				{
					if (im[i,j] > 0)
					{
						im[i,j] = 1;
					}
				}
			}

			/* Mark and delete */
			while (again)
			{
				again = false;

				/* Matching template M1 - scan top-bottom, left - right */
				for (i=1; i<nrows-1; i++)
				{
					for (j=1; j<ncols-1; j++)
					{
						if (im[i,j] == 0)
						{
							if (im[i-1,j] == 1 && im[i+1,j] != 1)
							{
								if (NumberAdjacents8(im, i, j) != 1  && Yokoi (im, i, j) == 1)
								{
									im[i,j] = 2;
								}
							}
						}
					}
				}

				/* Template M2 bottom-top, left-right */
				for (j=1; j<ncols-1; j++)
				{
					for (i=nrows-2; i>=1; i--)
					{
						if (im[i,j] == 0)
						{
							if (im[i,j-1] == 1 && im[i,j+1] != 1)
							{
								if (NumberAdjacents8(im, i, j) != 1  && Yokoi (im, i, j) == 1)
								{
									im[i,j] = 2;
								}
							}
						}
					}
				}

				/* Template M3 right-left, bottom-top */
				for (i=nrows-2; i>=1; i--)
				{
					for (j=ncols-2; j>=1; j--)
					{
						if (im[i,j] == 0)
						{
							if (im[i-1,j] != 1 && im[i+1,j] == 1)
							{
								if (NumberAdjacents8(im, i, j) != 1  && Yokoi (im, i, j) == 1)
								{
									im[i,j] = 2;
								}
							}
						}
					}
				}

				/* Template M4 */
				for (j=ncols-2; j>=1; j--)
				{
					for (i=1; i<nrows-1; i++)
					{
						if (im[i,j] == 0)
						{
							if (im[i,j-1] != 1 && im[i,j+1] == 1)
							{
								if (NumberAdjacents8(im, i, j) != 1  && Yokoi (im, i, j) == 1)
								{
									im[i,j] = 2;
								}
							}
						}
					}
				}

				/* Delete pixels that are marked (== 2) */
				for (i=1; i<nrows-1; i++)
				{
					for (j=1; j<ncols-1; j++)
					{
						if (im[i,j] == 2)
						{
							im[i,j] = 1;
							again = true;
						}
					}
				}

				/*Display (im); */
			}

			for(i=1;i<nrows-1;i++)
				for(j=1;j<ncols-1;j++)
					if(im[i,j]>0) im[i,j]=FloatBitmap.White;

			/* Display (im); */
		}

		/// <summary>
		/// Calcula el numero de adyacencias de un pixel de la imagen,
		/// usando 8-adyacencia.
		/// </summary>
		/// <param name="im">La imagen que estamos tratando.</param>
		/// <param name="r">La coordenada Y del pixel.</param>
		/// <param name="c">La coordenada X del pixel.</param>
		/// <returns></returns>
		private int NumberAdjacents8 (FloatBitmap im, int r, int c)
		{
			int i,j,k=0;

			for (i=r-1; i<=r+1; i++)
			{
				for (j=c-1; j<=c+1; j++)
				{
					if (i!=r || c!=j)
					{
						if (im[i,j] == 0)
							k++;
					}
				}
			}

			return k;
		}

		/// <summary>
		/// Calcula el numero de Yokoi de un pixel.
		/// </summary>
		/// <param name="im">La imagen donde se encuentra el pixel.</param>
		/// <param name="r">La coordenada Y del pixel.</param>
		/// <param name="c">La coordenada X del pixel.</param>
		/// <returns></returns>
		private int Yokoi (FloatBitmap im, int r, int c)
		{
			int[] N=new int[9];
			int i,k, i1, i2;

			N[0] = (im[r,c]      != 0?1:0);
			N[1] = (im[r,c+1]    != 0?1:0);
			N[2] = (im[r-1,c+1]  != 0?1:0);
			N[3] = (im[r-1,c]    != 0?1:0);
			N[4] = (im[r-1,c-1]  != 0?1:0);
			N[5] = (im[r,c-1]    != 0?1:0);
			N[6] = (im[r+1,c-1]  != 0?1:0);
			N[7] = (im[r+1,c]    != 0?1:0);
			N[8] = (im[r+1,c+1]  != 0?1:0);

			k = 0;
			for (i=1; i<=7; i+=2)
			{
				i1 = i+1; if (i1 > 8) i1 -= 8;
				i2 = i+2; if (i2 > 8) i2 -= 8;
				k += (N[i] - N[i]*N[i1]*N[i2]);
			}
			/*
			Console.WriteLine ("Yokoi: ({0},{1})",r, c);
			Console.WriteLine ("{0} {1} {2}", im[r-1,c-1], im[r-1,c],	im[r-1,c+1]);
			Console.WriteLine ("{0} {1} {2}", im[r,c-1], im[r,c],im[r,c+1]);
			Console.WriteLine ("{0} {1} {2}", im[r+1,c-1], im[r+1,c],im[r+1,c+1]);
			for (i=0; i<9; i++){
				Console.WriteLine ("{0} ", N[i]);
			}
			
			Console.WriteLine();
			Console.WriteLine ("Y = {0}\n", k);			
			*/
			return k;
		}

		/// <summary>
		/// Este metodo implementa el presuavizado que se puede aplicar antes de
		/// adelgazar con este metodo.
		/// </summary>
		/// <param name="im">La imagen a la que se aplicara el presuavizado.</param>
		private void PreSmooth (FloatBitmap im)
		{
			int i,j;

			for (i=0; i<nrows; i++)
			{
				for (j=0; j<ncols; j++)
				{
					if (im[i,j] == 0)
					{
						if (NumberAdjacents8(im, i, j) <= 2 && Yokoi (im, i, j)<2)
							im[i,j] = 2;
					}
				}
			}
			
			for (i=0; i<nrows; i++)
			{
				for (j=0; j<ncols; j++)
				{
					if (im[i,j] == 2)
						im[i,j] = 1;
				}
			}
		}
	}
}
