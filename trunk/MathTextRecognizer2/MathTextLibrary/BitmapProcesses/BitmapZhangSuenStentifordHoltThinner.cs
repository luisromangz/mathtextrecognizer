using System;

using MathTextLibrary;
using MathTextLibrary.Bitmap;

namespace MathTextLibrary.BitmapProcesses
{
	/// <summary>
	/// Esta clase implementa BitmapProcess, usando para adelgazar
	/// el algoritmo de Zhang y Suen, con el prepocesado de Stentiford,
	/// y el postprocesado de Holt, tal y como aparece en Parker.
	/// </summary>
	[BitmapProcessDescription("Adelgazado de Zhang, Suen, Stentiford y Holt")]
	public class BitmapZhangSuenStentifordHoltThinner: BitmapProcess
	{
		// variables globales del algoritmo
		private bool t00, t01, t11, t01s;

		private const int NORTH=1;
		private const int SOUTH=3;

		private int height;
		private int width;

		/// <summary>
		/// El constructor de BitmapZhangSuenStentifordHoltThinner.
		/// </summary>
		public BitmapZhangSuenStentifordHoltThinner()
		{
		
		}
		

		/// <summary>
		/// Crea una imagen auxiliar con dos filas y columnas mas que la original,
		/// necesario para el correcto funcionamiento del algoritmo de adelgazado.
		/// </summary>
		/// <param name="image">La imagen que vamos a adelgazar.</param>
		/// <returns>La imagen con las columnas y filas aadidas.</returns>
		private FloatBitmap CreateAuxImage(FloatBitmap image)
		{
			// La nueva imagen tiene dos filas y dos columnas ms que la original
			FloatBitmap newImage=new FloatBitmap(width,height);

			for(int i=0;i<width-2;i++)
				for(int j=0;j<height-2;j++)
					newImage[i+1,j+1]=image[i,j];
			
			// Aadimos un marco blanco de un pixel de ancho			
			for(int i=0;i<width;i++) 
			{
				newImage[i, 0] = 1;
				newImage[i, height - 1] = 1;
			}
			
			for(int j=0; j < height; j++)
			{
				newImage[0,j]=1;
				newImage[width-1,j]=1;
			}
			
			return newImage;
		}

		/// <summary>
		/// Este es el metodo a invocar para realizar el
		/// adelgazamiento de la imagen.
		/// </summary>
		/// <param name="image">
		/// La imagen a ser procesada.
		/// </param>
		public override FloatBitmap Apply(FloatBitmap image)
		{
			width=image.Width+2;
			height=image.Height+2;

			FloatBitmap newImage=CreateAuxImage(image);

			/* Pre_process */
			PreSmooth(newImage);
			Aae(newImage);
			
			ZhangSuenStentifordHoltThinning(newImage);

			FloatBitmap res = new FloatBitmap(width - 2 , height -2);

			
			for(int i=0;i<width-2; i++)
			{
				for(int j=0;j<height-2; j++)
				{
					res[i,j] = newImage[i + 1,j + 1];
				}
			}
			
			return res;
		}

		private void ZhangSuenStentifordHoltThinning(FloatBitmap image)
		{
			int i,j,k;
			bool again=true;

			FloatBitmap tmp= new FloatBitmap(image);

			/* BLACK = 1, WHITE = 0. */
			for(i=0;i<width;i++)
			{
				for (j=0;j<height;j++)
				{
					if(image[i,j]>0)
						image[i,j]=0;
					else
						image[i,j]=1;
					tmp[i,j]=0;
				}
			}

			/* Mark and delete */
			while(again)
			{
				again=false;

				/* Second sub-iteration */
				for (i=1; i<width; i++)
					for (j=1; j<height; j++)
					{
						if (image[i,j] != 1) continue;
						k = Nays8(image, i, j);
						if ((k >= 2 && k <= 6) && Connectivity(image, i,j)==1)
						{
							if (image[i,j+1]*image[i-1,j]*image[i,j-1] == 0 &&
									image[i-1,j]*image[i+1,j]*image[i,j-1] == 0)
							{
								tmp[i,j] = 1;
								again=true;
							}
					    }
					}
				Delete (image, tmp);
				if(!again) break;

				/* First sub-iteration */
				for (i=1; i<width; i++)
				{
					for (j=1; j<height; j++)
					{
						if (image[i,j] != 1) continue;
						k = Nays8(image, i, j);
						if ((k >= 2 && k <= 6) && Connectivity(image, i,j)==1)
						{
							if (image[i-1,j]*image[i,j+1]*image[i+1,j] == 0 &&
								image[i,j+1]*image[i+1,j]*image[i,j-1] == 0)
							{
								tmp[i,j] = 1;
								again = true;
							}
						}
					}
				}
				Delete (image, tmp);
			} // fin de while(again)

			/* Post_process */
			Stair (image, tmp, NORTH);
			Delete (image, tmp);
			Stair (image, tmp, SOUTH);
			Delete (image, tmp);

			/* Restore levels */
			for (i=1; i<width; i++)
				for (j=1; j<height; j++)
					if(image[i,j] > 0)
						image[i,j] = 0;
					else
						image[i,j] = MathTextBitmap.White;
		}

		private void Delete(FloatBitmap image, FloatBitmap tmp)
		{
			int i,j;
			
			/* Borrar los pxeles marcados*/
			for(i=1;i<width-1;i++)
				for(j=1;j<height-1;j++)
					if(tmp[i,j]!=0)
					{
						image[i,j]=0;
						tmp[i,j]=0;
					}
		}

		/*	Number of neighboring 1 pixels */
		private int Nays8(FloatBitmap im, int r, int c)
		{
			int i,j,k=0;

			for(i=r-1; i<=r+1; i++)
				for(j=c-1; j<=c+1; j++)
					if(i!=r || c!=j)
						if(im[i,j] >= 1) k++;

			return k;
		}

		/*	Number of neighboring 0 pixels */
		private int Snays(FloatBitmap im, int r, int c)
		{
			int i,j,k=0;

			for (i=r-1; i<=r+1; i++)
				for (j=c-1; j<=c+1; j++)
					if (i!=r || c!=j)
						if(im[i,j] == 0) k++;
							return k;
		}

		/*	Connectivity by counting black-white transitions on the boundary */
		private int Connectivity(FloatBitmap im, int r, int c)
		{
			int N=0;

			if (im[r,c+1]   >= 1 && im[r-1,c+1] == 0) N++;
			if (im[r-1,c+1] >= 1 && im[r-1,c]   == 0) N++;
			if (im[r-1,c]   >= 1 && im[r-1,c-1] == 0) N++;
			if (im[r-1,c-1] >= 1 && im[r,c-1]   == 0) N++;
			if (im[r,c-1]   >= 1 && im[r+1,c-1] == 0) N++;
			if (im[r+1,c-1] >= 1 && im[r+1,c]   == 0) N++;
			if (im[r+1,c]   >= 1 && im[r+1,c+1] == 0) N++;
			if (im[r+1,c+1] >= 1 && im[r,c+1]   == 0) N++;

			return N;
		}

		/* Stentiford's boundary smoothing method */
		private void PreSmooth(FloatBitmap im)
		{
			int i,j;

			for (i=0; i<width; i++)
			  for (j=0; j<height; j++)
			    if (im[i,j] == 0)
				if (Snays(im, i, j) <= 2 && Yokoi (im, i, j)<2)
				  im[i,j] = 2;

			for (i=0; i<width-1; i++)
			  for (j=0; j<height-1; j++)
			    if (im[i,j] == 2) im[i,j] = 1;
		}

		/*	Stentiford's Acute Angle Emphasis	*/
		private void Aae(FloatBitmap im)
		{
			bool again = false;
			int i,j, k;

			for (k=5; k>= 1; k-=2)
			{
			  for (i=2; i<width-2; i++)
			    for (j=2; j<height-2; j++)
			      if (im[i,j] == 0)
				MatchDu (im, i, j, k);

			  for (i=2; i<width-2; i++)
			    for (j=2; j<height-2; j++)
			    if (im[i,j] == 2)
			    {
					again = true;
					im[i,j] = 1;
			    }

			  if(!again) break;
			} 
		}

		/*	Template matches for acute angle emphasis	*/
		private void MatchDu(FloatBitmap im, int r, int c, int k)
		{

		/* D1 */
			if (im[r-2,c-2] == 0 && im[r-2,c-1] == 0 &&
			    im[r-2,c]   == 1 && im[r-2,c+1] == 0 &&
			    im[r-2,c+2] == 0 &&
			    im[r-1,c-2] == 0 && im[r-1,c-1] == 0 &&
			    im[r-1,c]   == 1 && im[r-1,c+1] == 0 &&
			    im[r-1,c+2] == 0 &&
			    im[r,c-2] == 0 && im[r,c-1] == 0 &&
			    im[r,c]   == 0 && im[r,c+1] == 0 &&
			    im[r,c+2] == 0 &&
			    im[r+1,c-2] == 0 && im[r+1,c-1] == 0 &&
			    im[r+1,c]   == 0 && im[r+1,c+1] == 0 &&
			    im[r+1,c+2] == 0 &&
			    im[r+2,c-1] == 0 &&
			    im[r+2,c]   == 0 && im[r+2,c+1] == 0 )
			{
				im[r,c] = 2;
				return;
			}

		/* D2 */
			if (k >= 2)
			{
				if (im[r-2,c-2] == 0 && im[r-2,c-1] == 1 &&
				    im[r-2,c]   == 1 && im[r-2,c+1] == 0 &&
				    im[r-2,c+2] == 0 &&
				    im[r-1,c-2] == 0 && im[r-1,c-1] == 0 &&
				    im[r-1,c]   == 1 && im[r-1,c+1] == 0 &&
				    im[r-1,c+2] == 0 &&
				    im[r,c-2] == 0 && im[r,c-1] == 0 &&
				    im[r,c]   == 0 && im[r,c+1] == 0 &&
				    im[r,c+2] == 0 &&
				    im[r+1,c-2] == 0 && im[r+1,c-1] == 0 &&
				    im[r+1,c]   == 0 && im[r+1,c+1] == 0 &&
				    im[r+1,c+2] == 0 &&
				    im[r+2,c-1] == 0 &&
				    im[r+2,c]   == 0 && im[r+2,c+1] == 0 )
				{
					im[r,c] = 2;
					return;
				}
			}

		/* D3 */
			if (k>=3)
			if (im[r-2,c-2] == 0 && im[r-2,c-1] == 0 &&
			    im[r-2,c]   == 1 && im[r-2,c+1] == 1 &&
			    im[r-2,c+2] == 0 &&
			    im[r-1,c-2] == 0 && im[r-1,c-1] == 0 &&
			    im[r-1,c]   == 1 && im[r-1,c+1] == 0 &&
			    im[r-1,c+2] == 0 &&
			    im[r,c-2] == 0 && im[r,c-1] == 0 &&
			    im[r,c]   == 0 && im[r,c+1] == 0 &&
			    im[r,c+2] == 0 &&
			    im[r+1,c-2] == 0 && im[r+1,c-1] == 0 &&
			    im[r+1,c]   == 0 && im[r+1,c+1] == 0 &&
			    im[r+1,c+2] == 0 &&
			    im[r+2,c-1] == 0 &&
			    im[r+2,c]   == 0 && im[r+2,c+1] == 0 )
			{
				im[r,c] = 2;
				return;
			}

		/* D4 */
			if (k>=4)
			if (im[r-2,c-2] == 0 && im[r-2,c-1] == 1 &&
			    im[r-2,c]   == 1 && im[r-2,c+1] == 0 &&
			    im[r-2,c+2] == 0 &&
			    im[r-1,c-2] == 0 && im[r-1,c-1] == 1 &&
			    im[r-1,c]   == 1 && im[r-1,c+1] == 0 &&
			    im[r-1,c+2] == 0 &&
			    im[r,c-2] == 0 && im[r,c-1] == 0 &&
			    im[r,c]   == 0 && im[r,c+1] == 0 &&
			    im[r,c+2] == 0 &&
			    im[r+1,c-2] == 0 && im[r+1,c-1] == 0 &&
			    im[r+1,c]   == 0 && im[r+1,c+1] == 0 &&
			    im[r+1,c+2] == 0 &&
			    im[r+2,c-1] == 0 &&
			    im[r+2,c]   == 0 && im[r+2,c+1] == 0 )
			{
				im[r,c] = 2;
				return;
			}

		/* D5 */
			if (k>=5)
			if (im[r-2,c-2] == 0 && im[r-2,c-1] == 0 &&
			    im[r-2,c]   == 1 && im[r-2,c+1] == 1 &&
			    im[r-2,c+2] == 0 &&
			    im[r-1,c-2] == 0 && im[r-1,c-1] == 0 &&
			    im[r-1,c]   == 1 && im[r-1,c+1] == 1 &&
			    im[r-1,c+2] == 0 &&
			    im[r,c-2] == 0 && im[r,c-1] == 0 &&
			    im[r,c]   == 0 && im[r,c+1] == 0 &&
			    im[r,c+2] == 0 &&
			    im[r+1,c-2] == 0 && im[r+1,c-1] == 0 &&
			    im[r+1,c]   == 0 && im[r+1,c+1] == 0 &&
			    im[r+1,c+2] == 0 &&
			    im[r+2,c-1] == 0 &&
			    im[r+2,c]   == 0 && im[r+2,c+1] == 0 )
			{
				im[r,c] = 2;
				return;
			}

		/* U1 */
			if (im[r+2,c-2] == 0 && im[r+2,c-1] == 0 &&
			    im[r+2,c]   == 1 && im[r+2,c+1] == 0 &&
			    im[r+2,c+2] == 0 &&
			    im[r+1,c-2] == 0 && im[r+1,c-1] == 0 &&
			    im[r+1,c]   == 1 && im[r+1,c+1] == 0 &&
			    im[r+1,c+2] == 0 &&
			    im[r,c-2] == 0 && im[r,c-1] == 0 &&
			    im[r,c]   == 0 && im[r,c+1] == 0 &&
			    im[r,c+2] == 0 &&
			    im[r-1,c-2] == 0 && im[r-1,c-1] == 0 &&
			    im[r-1,c]   == 0 && im[r-1,c+1] == 0 &&
			    im[r-1,c+2] == 0 &&
			    im[r-1,c-1] == 0 &&
			    im[r-1,c]   == 0 && im[r-1,c+1] == 0 )
			{
				im[r,c] = 2;
				return;
			}

		/* U2 */
			if (k>=2)
			if (im[r+2,c-2] == 0 && im[r+2,c-1] == 1 &&
			    im[r+2,c]   == 1 && im[r+2,c+1] == 0 &&
			    im[r+2,c+2] == 0 &&
			    im[r+1,c-2] == 0 && im[r+1,c-1] == 0 &&
			    im[r+1,c]   == 1 && im[r+1,c+1] == 0 &&
			    im[r+1,c+2] == 0 &&
			    im[r,c-2] == 0 && im[r,c-1] == 0 &&
			    im[r,c]   == 0 && im[r,c+1] == 0 &&
			    im[r,c+2] == 0 &&
			    im[r-1,c-2] == 0 && im[r-1,c-1] == 0 &&
			    im[r-1,c]   == 0 && im[r-1,c+1] == 0 &&
			    im[r-1,c+2] == 0 &&
			    im[r-2,c-1] == 0 &&
			    im[r-2,c]   == 0 && im[r-2,c+1] == 0 )
			{
				im[r,c] = 2;
				return;
			}

		/* U3 */
			if (k>=3)
			if (im[r+2,c-2] == 0 && im[r+2,c-1] == 0 &&
			    im[r+2,c]   == 1 && im[r+2,c+1] == 1 &&
			    im[r+2,c+2] == 0 &&
			    im[r+1,c-2] == 0 && im[r+1,c-1] == 0 &&
			    im[r+1,c]   == 1 && im[r+1,c+1] == 0 &&
			    im[r+1,c+2] == 0 &&
			    im[r,c-2] == 0 && im[r,c-1] == 0 &&
			    im[r,c]   == 0 && im[r,c+1] == 0 &&
			    im[r,c+2] == 0 &&
			    im[r-1,c-2] == 0 && im[r-1,c-1] == 0 &&
			    im[r-1,c]   == 0 && im[r-1,c+1] == 0 &&
			    im[r-1,c+2] == 0 &&
			    im[r-2,c-1] == 0 &&
			    im[r-2,c]   == 0 && im[r-2,c+1] == 0 )
			{
				im[r,c] = 2;
				return;
			}

		/* U4 */
			if (k>=4)
			if (im[r+2,c-2] == 0 && im[r+2,c-1] == 1 &&
			    im[r+2,c]   == 1 && im[r+2,c+1] == 0 &&
			    im[r+2,c+2] == 0 &&
			    im[r+1,c-2] == 0 && im[r+1,c-1] == 1 &&
			    im[r+1,c]   == 1 && im[r+1,c+1] == 0 &&
			    im[r+1,c+2] == 0 &&
			    im[r,c-2] == 0 && im[r,c-1] == 0 &&
			    im[r,c]   == 0 && im[r,c+1] == 0 &&
			    im[r,c+2] == 0 &&
			    im[r-1,c-2] == 0 && im[r-1,c-1] == 0 &&
			    im[r-1,c]   == 0 && im[r-1,c+1] == 0 &&
			    im[r-1,c+2] == 0 &&
			    im[r-2,c-1] == 0 &&
			    im[r-2,c]   == 0 && im[r-2,c+1] == 0 )
			{
				im[r,c] = 2;
				return;
			}

		/* U5 */
			if (k>=5)
			if (im[r+2,c-2] == 0 && im[r+2,c-1] == 0 &&
			    im[r+2,c]   == 1 && im[r+2,c+1] == 1 &&
			    im[r+2,c+2] == 0 &&
			    im[r+1,c-2] == 0 && im[r+1,c-1] == 0 &&
			    im[r+1,c]   == 1 && im[r+1,c+1] == 1 &&
			    im[r+1,c+2] == 0 &&
			    im[r,c-2] == 0 && im[r,c-1] == 0 &&
			    im[r,c]   == 0 && im[r,c+1] == 0 &&
			    im[r,c+2] == 0 &&
			    im[r-1,c-2] == 0 && im[r-1,c-1] == 0 &&
			    im[r-1,c]   == 0 && im[r-1,c+1] == 0 &&
			    im[r-1,c+2] == 0 &&
			    im[r-2,c-1] == 0 &&
			    im[r-2,c]   == 0 && im[r-2,c+1] == 0 )
			{
				im[r,c] = 2;
				return;
			}
		}

		/*	Yokoi's connectivity measure	*/
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

			return k;
		}
		
		/*	Holt's staircase removal stuff */
		private void Check(int v1, int v2, int v3)
		{
			if(v2==0 && (v1==0 || v3==0))
				t00=true;
			if(v2!=0 && (v1!=0 || v3!=0))
				t11=true;
			if((v1==0 && v2!=0) || (v2==0 && v3!=0))
			{
				t01s=t01;
				t01=true;
			}
		}

		private bool Edge(FloatBitmap image, int r, int c)
		{
			if (image[r,c] == 0)
				return false;
			t00 = t01 = t01s = t11 = false;

			/* CHECK(vNW, vN, vNE) */
			Check((int)image[r-1,c-1], (int)image[r-1,c], (int)image[r-1,c+1]);

			/* CHECK(vNE, vE, vSE) */
			Check((int)image[r-1,c+1], (int)image[r,c+1], (int)image[r+1,c+1]);

			/* CHECK(vSE, vS, vSW) */
			Check((int)image[r+1,c+1], (int)image[r+1,c], (int)image[r+1,c-1]);

			/* CHECK(vSW, vW, vNW) */
			Check((int)image[r+1,c-1], (int)image[r,c-1], (int)image[r-1,c-1]);

			return t00 && t11 && !t01s;
		}

		private void Stair(FloatBitmap image, FloatBitmap tmp, int direction)
		{
			int i,j;
			bool N, S, E, W, NE, NW, SE, SW, C;

			if(direction == NORTH)
			{
				for(i=1; i<width-1; i++)
				{
					for(j=1; j<height-1; j++)
					{
						NW=image[i-1,j-1]!=0; N=image[i-1,j]!=0; NE=image[i-1,j+1]!=0;
						W=image[i,j-1]!=0; C=image[i,j]!=0; E=image[i,j+1]!=0;
						SW=image[i+1,j-1]!=0; S=image[i+1,j]!=0; SE=image[i+1,j+1]!=0;

						if(direction == NORTH)
						{
							if(C && !(N && 
									((E && !NE && !SW && (!W || !S)) || 
									(W && !NW && !SE && (!E || !S)) )) )
								tmp[i,j]=0;		/* Sobrevive */
							else
								tmp[i,j]=1;
						} 
						else if(direction == SOUTH)
						{
							if(C && !(S && 
									((E && !SE && !NW && (!W || !N)) || 
									(W && !SW && !NE && (!E || !N)) )) )
								tmp[i,j] = 0;		/* Sobrevive */
							else
								tmp[i,j] = 1;
						}
					}
				}
			}
		}
		
		/// <value>
		/// Contiene los valores de los parametros del algoritmo en forma de
		/// cadena.
		/// </value>
		public override string Values
		{
			get { return ""; }
		}

	}
}
