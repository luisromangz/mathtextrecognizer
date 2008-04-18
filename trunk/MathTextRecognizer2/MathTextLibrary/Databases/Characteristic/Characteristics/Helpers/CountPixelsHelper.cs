using System;

using MathTextLibrary;
using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Databases.Characteristic.Characteristics.Helpers
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
		public static int NumBlackPixelsInHalf(FloatBitmap image, Half h)
		{
			int n=0;
			
			int width = image.Width;
			int height = image.Height;
			int halfWidth = width/2;
			int halfHeight = height/2;
			
			switch(h) 
			{
				case(Half.Top):
					n=NumPixelsInArea(image,FloatBitmap.Black,
					                  0,0,
					                  width,halfHeight);
					break;
				case(Half.Bottom):
					n=NumPixelsInArea(image,FloatBitmap.Black,
					                  0,halfHeight,
					                  width,height);
					break;
				case(Half.Left):
					n=NumPixelsInArea(image,FloatBitmap.Black,
					                  0,0,
					                  halfWidth,height);
					break;
				case(Half.Right):
					n=NumPixelsInArea(image,FloatBitmap.Black,
					                  halfWidth,0,
					                  width,height);
					break;
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
		public static int NumBlackPixelsInQuadrant(FloatBitmap image, Quadrant q)
		{
			int n=0;
			
			int width = image.Width;
			int height = image.Height;
			int halfWidth = width/2;
			int halfHeight = height/2;
			
			
			switch(q) 
			{
				case(Quadrant.NW):
					n=NumPixelsInArea(image,FloatBitmap.Black,
					                  0,0,
					                  halfWidth,halfHeight);
					break;
				case(Quadrant.NE):
					n=NumPixelsInArea(image,FloatBitmap.Black,
					                  halfWidth,0,
					                  width,halfHeight);
					break;
				case(Quadrant.SW):
					n=NumPixelsInArea(image,FloatBitmap.Black,
					                  0,halfHeight,
					                  halfWidth,height);
					break;
				case(Quadrant.SE):
					n=NumPixelsInArea(image,FloatBitmap.Black,
					                  halfWidth,halfHeight,
					                  width,height);
					break;
				
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
		protected static int NumPixelsInArea(FloatBitmap image, float value,
		                                    int x1, int y1, 
		                                     int x2, int y2)
		{
			if(x1 > x2 || y1 > y2)
				throw new ApplicationException("Área errónea en CountPixelsInQuadrant.NumPixelsInArea()");
			
			int n=0;
			
			for(int i=x1; i<x2; i++)
			{
				for(int j=y1; j<y2; j++)
				{
					if(image[i,j] == value)
						n++;
				}
			}
			
			return n;
		}		                                    
	}
}
