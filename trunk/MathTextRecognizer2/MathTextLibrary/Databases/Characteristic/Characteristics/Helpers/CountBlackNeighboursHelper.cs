using System;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Databases.Characteristic.Characteristics.Helpers
{
	/// <summary>
	/// Esta clase cuenta el numero de vecinos negros (en 8 adyacencia)
	/// que tiene un pixel indicado en una imagen.
	/// </summary>
	public class CountBlackNeighboursHelper
	{
		public CountBlackNeighboursHelper()
		{			
		}

		public static int BlackNeighbours(FloatBitmap image,
		                                  int x, int y, int width, int height)
		{
			int res=0;
			
			if(x-1>=0 && y-1 >= 0 && image[x-1,y-1]==MathTextBitmap.Black)
				res++;
			if(x-1>=0 && image[x-1,y]==MathTextBitmap.Black)
				res++;
			if(x-1>=0 && y+1 < height && image[x-1,y+1]==MathTextBitmap.Black)
				res++;			
			if(y-1 >= 0 && image[x,y-1]==MathTextBitmap.Black)
				res++;
			if(y+1 < height && image[x,y+1]==MathTextBitmap.Black)
				res++;			
			if(x+1 < width && y-1 >= 0 && image[x+1,y-1]==MathTextBitmap.Black)
				res++;
			if(x+1 <= width && image[x+1,y]==MathTextBitmap.Black)
				res++;
			if(x+1 < width && y+1 < height && image[x+1,y+1]==MathTextBitmap.Black)
				res++;
			return res;
			
		}
	}
}
