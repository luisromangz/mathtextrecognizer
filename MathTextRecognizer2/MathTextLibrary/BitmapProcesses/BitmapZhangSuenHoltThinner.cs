using System;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.BitmapProcesses
{
	/// <summary>
	/// Esta clase implementa BitmapProcess, usando como metodo
	/// de adelgazado el algoritmo de Zhang Suen, con la eliminacion
	/// de "escaleras" de Holt.
	/// </summary>
	[BitmapProcessDescription("Adelgazado de Zhang, Suen y Holt")]
	public class BitmapZhangSuenHoltThinner: BitmapProcess
	{
		// variables globales del algoritmo
		private bool t00, t01, t11, t01s;

		private bool removeStair;

		private const int NORTH=1;
		private const int SOUTH=3;

		private int height;
		private int width;

		/// <summary>
		/// El costructor de la clase BitmapZhangSuenHoltThinner.
		/// </summary>		
		public BitmapZhangSuenHoltThinner()
			: this(false)
		{
		
		}
		
		/// <summary>
		/// El costructor de la clase BitmapZhangSuenHoltThinner.
		/// </summary>
		/// <param name="removeStair">
		/// Si es cierto, se producira el eliminado de las "escaleras" de la imagen.
		/// </param>
		public BitmapZhangSuenHoltThinner(bool removeStair)
		{
			this.removeStair = removeStair;
		}
		
		public override string Values
		{
			get 
			{ 
				return "Eliminar escalera: "+(removeStair?"Sí":"No");
			}
		}

		[BitmapProcessPropertyDescription("Eliminar escaleras")]
		public virtual bool RemoveStair {
			get {
				return removeStair;
			}
			
			set
			{
				removeStair = value;
			}
		}


		/// <summary>
		/// Creamos una nueva imagen con dos filas y columnas mas, necesesario para el algoritmo.
		/// </summary>
		/// <param name="image">La imagen a procesar.</param>
		/// <returns>La imagen con las filas y columnas añadidas.</returns>
		private float[,] CreateAuxImage(float[,] image)
		{
			// La nueva imagen tiene dos filas y dos columnas mas que la original
			float[,] newImage=new float[width,height];

			float brightness;
			for(int i = 0; i < width - 2; i++)
			{
				for(int j = 0; j < height - 2; j++)
				{
										
					brightness = image[i, j];
					
					newImage[i + 1,j + 1] = brightness;
				}
			}
			
			for(int i=0;i< width;i++) 
			{
				newImage[i,0]=1;
				newImage[i,height-1]=1;
			}
			for(int j=0;j< height;j++)
			{
				newImage[0,j]=1;
				newImage[width-1,j]=1;
			}
			return newImage;
		}

		/// <summary>
		/// El metodo que se debe invocar para realizar el adelgazamiento.
		/// </summary>
		/// <param name="image">
		/// La imagen que deseamos adelgazar.
		/// </param>
		/// <returns>
		/// La imagen adelgazada.
		/// </returns>
		public override float[,] Apply(float[,] image)
		{
			width = image.GetLength(0) + 2;
			height = image.GetLength(1) + 2;

			float[,] newImage = CreateAuxImage(image);
			
			ZhangSuenHoltThinning(newImage);
			
			float [,] res = new float[width - 2 , height -2];

			
			for(int i=0;i<width-2; i++)
			{
				for(int j=0;j<height-2; j++)
				{
					res[i,j] = newImage[i + 1,j + 1];
				}
			}
					
			return res;
		}

		/// <summary>
		/// En este metodo se implementa el algoritmo de adelgazamiento de Zhang Suen.
		/// </summary>
		/// <param name="image">
		/// La imagen a adelgazar.
		/// </param>
		private void ZhangSuenHoltThinning(float[,] image)
		{
			int i,j;
			bool again=true;

			float[,] tmp=(float[,])image.Clone();
			if(tmp==image)
				throw new Exception("LA CLONACION DE ARRAYS SOLO COPIA LA REFERENCIA!");

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

				for(i=1;i<width-1;i++)
				{
					for(j=1;j<height-1;j++)
					{
						if (image[i,j]==1) 
						{
							if(!Edge(image,i,j) || 
									(Edge(image,i,j+1) && image[i-1,j]==1 && image[i+1,j]==1) ||
									(Edge(image,i+1,j) && image[i,j-1]==1 && image[i,j+1]==1) ||
									(Edge(image,i,j+1) && Edge(image, i+1,j+1) && Edge(image, i+1, j)) )
								tmp[i,j] = 0;
							else 
							{
								tmp[i,j]=1;
								again=true; 
							}
						} 
						else tmp[i,j]=1;
					}
				}

				DeleteMarkedPixels(image, tmp);
			}

			/* Staircase removal */
			if(removeStair)
			{
				Stair(image,tmp,NORTH);
				DeleteMarkedPixels(image,tmp);
				Stair(image,tmp,SOUTH);
				DeleteMarkedPixels(image,tmp);
			}

			/* Restore levels */
			for(i=1;i<width-1;i++) 
			{
				for(j=1;j<height-1;j++)
				{
					if(image[i,j]>0)
						image[i,j]=0;
					else
						image[i,j]=MathTextBitmap.White;
				}
			}
		}

		/// <summary>
		/// En este metodo se implementa la eliminacion de los puntos 
		/// de escalera de Holt.
		/// </summary>
		/// <param name="image">La imagen sobre la que se efectua la eliminacion.</param>
		/// <param name="tmp">Una imagen auxiliar para el proceso.</param>
		/// <param name="direction">El sentido en que se aplica el proceso.</param>
		private void Stair(float[,] image, float[,] tmp, int direction)
		{
			int i,j;
			bool N, S, E, W, NE, NW, SE, SW, C;

			if(direction==NORTH)
			{
				for(i=1;i<width-1;i++)
				{
					for(j=1;j<height-1;j++)
					{
						NW=image[i-1,j-1]!=0; N=image[i-1,j]!=0; NE=image[i-1,j+1]!=0;
						W=image[i,j-1]!=0; C=image[i,j]!=0; E=image[i,j+1]!=0;
						SW=image[i+1,j-1]!=0; S=image[i+1,j]!=0; SE=image[i+1,j+1]!=0;

						if(direction==NORTH)
						{
							if(C && !(N && 
									((E && !NE && !SW && (!W || !S)) || 
									(W && !NW && !SE && (!E || !S)) )) )
								tmp[i,j]=0;		/* Sobrevive */
							else
								tmp[i,j]=1;
						} 
						else if(direction==SOUTH)
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

		/// <summary>
		/// En este metodo se eliminan los pixeles marcados por el algoritmo de
		/// Zang Suen en una pasada.
		/// </summary>
		/// <param name="image">La imagen que procesamos.</param>
		/// <param name="tmp">Una imagen auxiliar para el proceso.</param>
		private void DeleteMarkedPixels(float[,] image, float[,] tmp)
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

		/// <summary>
		/// Realiza una comprobacion necesaria para 
		/// determinar si un pixel esta en borde.
		/// </summary>
		/// <param name="v1">El valor de un pixel.</param>
		/// <param name="v2">El valor de uno de sus vecinos.</param>
		/// <param name="v3">El valor de otro de sus vecinos.</param>
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

		/// <summary>
		/// Comprueba si un pixel de la imagen esta en un borde.
		/// </summary>
		/// <param name="image">La imagen que estamos adelgazando.</param>
		/// <param name="r">La coordenada Y del pixel.</param>
		/// <param name="c">La coordenada X del pixel.</param>
		/// <returns>Cierto si esta en el borde, falso e.o.c.</returns>
		private bool Edge (float[,] image, int r, int c)
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
	}
}
