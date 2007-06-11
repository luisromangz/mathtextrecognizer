
using System;

using Gdk;

namespace MathTextLibrary.Utils
{
	
	/// <summary>
	/// Esta clase contiene métodos estáticos de utilidad para el uso
	/// de imagenes.
	/// </summary>
	public class ImageUtils
	{
	
		/// <summary>
		/// Convierte la imagen pasada como array de float a un <c>Pixbuf</c>
		/// en escala de grises.
		/// </summary>
		/// <param name="image">
		/// Imagen a convertir a <c>Pixbuf</c>.
		/// </param>
		/// <returns>
		/// <c>Pixbuf</c> en escala de grises equivalente a <c>image</c>
		/// </returns>
		public static Pixbuf CreatePixbufFromMatrix(float [,] image)
		{
			int width = image.GetLength(0);
			int height = image.GetLength(1);
			
			// Creamos el Pixbuf, y lo rellenamos de blanco, así nos
			// aseguramos de que el array de píxeles esta creado.
			Pixbuf p = new Pixbuf(Colorspace.Rgb, false,8, width, height);
			p.Fill(0xFFFFFF);
			
			// Comprobamos el Rowstride del Pixbuf, para compensarlo.
			int rowstrideCompensation = p.Rowstride - 3 * width; 
			
			int k = 0;
			byte color;
			unsafe
			{
				byte* data = (byte*) p.Pixels;
				for(int j = 0; j < height; j++)
				{
					for(int i = 0; i < width; i++)					
					{
						color=(byte)(255*image[i,j]);
						
						/// Establecemos las tres componentes
						data[k] = color;
						data[k + 1] = color;
						data[k + 2] = color;						
						
						k += 3;
					}		
					
					k += rowstrideCompensation;		
				}	
			}
			return p;
		}
	
		/// <summary>
		/// Convierte el <c>Pixbuf</c> pasado como parámetro a un array de float.
		/// </summary>
		/// <param name="b">
		/// Imagen bitmap a convertir.
		/// </param>
		/// <returns>
		/// Array de float bidimensional conteniendo la misma
		/// informacion de pixeles que el bitmap original, pero en escala de
		/// grises, y de forma que la coordenada Y de la imagen se almacena
		/// en la primera componente del array.
		/// </returns>
		public static float[,] CreateMatrixFromPixbuf(Pixbuf b)
		{
			
			float[,] imageRes =new float [b.Width , b.Height];
			
			int pixelStep = b.NChannels;
			
			// Tenemos que compensar los pixeles que se añaden para tener
			// un rowstride optimo.			
			int rowstrideCompensation = b.Rowstride - pixelStep * b.Width;
			
			unsafe
			{
				byte* data = (byte*) b.Pixels;
				int k = 0;		
				float color;
				for(int j = 0; j < b.Height; j++)
				{
					for(int i = 0; i < b.Width; i++)					
					{
						// Usamos la formula para la luminosidad NTSC
						color = 
							data[k]*0.299f + data[k + 1]*0.587f + data[k + 2] * 0.114f;
						
						imageRes[i,j]= color/255.0f;
								
						k += pixelStep;		
					}
					
					k+= rowstrideCompensation;
				}
			
			}
			
			if(imageRes.GetLength(0) != b.Width
				|| imageRes.GetLength(1) != b.Height)
			{
				throw new Exception("Error al crear la matriz a partir del Pixbuf");
			}
		
			return imageRes;
		}
		
	}
}
