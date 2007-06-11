using System;

using MathTextLibrary;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers
{
	// cuadrantes noroeste, noreste, suroeste, sureste
	public enum Quadrant { NW, NE, SW, SE }
	
	// mitad superior, inferior, izquierda, derecha
	public enum Half { Top, Bottom, Left, Right }
	
	/// <summary>
	/// Esta clase cuenta el numero de pixeles negros en una mitad
	/// o cuadrante de una imagen.
	/// </summary>
	public class CountPixelsHelper
	{
		public CountPixelsHelper()
		{
		}
		
		/// <summary>
		/// Cuenta el numero de pixeles negros en la mitad indicada por <c>h</c>.
		/// </summary>
		/// <remarks>
		/// Si la imagen tiene un numero impar de pixeles se incluye la fila
		/// o columna central, segun el caso.
		/// </remarks>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <param name="h">Mitad a analizar</param>
		/// <returns>Numero de pixeles negros</returns>
		public static int NumBlackPixelsInHalf(MathTextBitmap image, Half h)
		{
			int n=0;
			float[,] im=image.ProcessedImage;
			int size=image.ProcessedImageSize;
			int mitad = size/2;
			
			if(size%2 == 0)
			{
				switch(h) {
					case(Half.Top):
						n=NumPixelsInArea(im,MathTextBitmap.Black,0,0,size-1,mitad-1);
						break;
					case(Half.Bottom):
						n=NumPixelsInArea(im,MathTextBitmap.Black,0,mitad,size-1,size-1);
						break;
					case(Half.Left):
						n=NumPixelsInArea(im,MathTextBitmap.Black,0,0,mitad-1,size-1);
						break;
					case(Half.Right):
						n=NumPixelsInArea(im,MathTextBitmap.Black,mitad,0,size-1,size-1);
						break;
				}
			}
			else
			{
				switch(h) {
					case(Half.Top):
						n=NumPixelsInArea(im,MathTextBitmap.Black,0,0,size-1,mitad);
						break;
					case(Half.Bottom):
						n=NumPixelsInArea(im,MathTextBitmap.Black,0,mitad,size-1,size-1);
						break;
					case(Half.Left):
						n=NumPixelsInArea(im,MathTextBitmap.Black,0,0,mitad,size-1);
						break;
					case(Half.Right):
						n=NumPixelsInArea(im,MathTextBitmap.Black,mitad,0,size-1,size-1);
						break;
				}
			}
			
			return n;			
		}

		/// <summary>
		/// Cuenta el numero de pixeles negros en el cuadrante indicado por
		/// <c>q</c>.
		/// </summary>
		/// <remarks>
		/// Si la imagen tiene un numero impar de pixeles se incluye la fila
		/// o columna central, segun el caso.
		/// </remarks>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <param name="q">Cuadrante a analizar</param>
		/// <returns>Numero de pixeles negros</returns>
		public static int NumBlackPixelsInQuadrant(MathTextBitmap image, Quadrant q)
		{
			int n=0;
			float[,] im=image.ProcessedImage;
			int size=image.ProcessedImageSize;
			int mitad = size/2;
			
			if(size%2 == 0)
			{
				switch(q) {
					case(Quadrant.NW):
						n=NumPixelsInArea(im,MathTextBitmap.Black,0,0,mitad-1,mitad-1);
						break;
					case(Quadrant.NE):
						n=NumPixelsInArea(im,MathTextBitmap.Black,mitad,0,size-1,mitad-1);
						break;
					case(Quadrant.SW):
						n=NumPixelsInArea(im,MathTextBitmap.Black,0,mitad,mitad-1,size-1);
						break;
					case(Quadrant.SE):
						n=NumPixelsInArea(im,MathTextBitmap.Black,mitad,mitad,size-1,size-1);
						break;
				}
			}
			else	// si el tamaño de la imagen es impar hacemos que la fila
			{		// y columna central se cuenten por duplicado
				switch(q) {
					case(Quadrant.NW):
						n=NumPixelsInArea(im,MathTextBitmap.Black,0,0,mitad,mitad);
						break;
					case(Quadrant.NE):
						n=NumPixelsInArea(im,MathTextBitmap.Black,mitad,0,size-1,mitad);
						break;
					case(Quadrant.SW):
						n=NumPixelsInArea(im,MathTextBitmap.Black,0,mitad,mitad,size-1);
						break;
					case(Quadrant.SE):
						n=NumPixelsInArea(im,MathTextBitmap.Black,mitad,mitad,size-1,size-1);
						break;
				}
			}
			
			return n;
		}
		
		/// <summary>
		/// Cuenta los píxeles de valor <c>value</c> en el area rectangular de
		/// esquina superior izquierda (x1,y1) e inferior derecha (x2,y2),
		/// ambas inclusive.
		/// </summary>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <param name="value">Color a contabilizar</param>
		/// <param name="x1">Minima coordenada horizontal</param>
		/// <param name="y1">Minima coordenada vertical</param>
		/// <param name="x2">Maxima coordenada horizontal</param>
		/// <param name="y2">Maxima coordenada vertical</param>
		/// <returns>Numero de pixeles de valor <c>value</c></returns>
		protected static int NumPixelsInArea(float[,] image, float value,
		                                    int x1, int y1, int x2, int y2)
		{
			if(x1 > x2 || y1 > y2)
				throw new ApplicationException("Área errónea en CountPixelsInQuadrant.NumPixelsInArea()");
			
			int n=0;
			
			for(int i=x1; i<=x2; i++)
			{
				for(int j=y1; j<=y2; j++)
				{
					if(image[i,j] == value)
						n++;
				}
			}
			
			return n;
		}		                                    
	}
}
