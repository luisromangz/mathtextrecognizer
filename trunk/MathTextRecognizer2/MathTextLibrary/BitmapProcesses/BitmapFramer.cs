using System;

using MathTextLibrary;
using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.BitmapProcesses
{
	/// <summary>
	/// La clase BitmapFramer tiene el cometido de, a partir de
	/// una imagen con unas dimensiones determinadas, generar una imagen cuadrada
	/// con un borde, para poder aplicar otros algoritmos correctamente.
	/// </summary>	
	[BitmapProcessDescription("Encuadre de imagen")]
	public class BitmapFramer : BitmapProcess
	{
		/// <summary>
		/// El constructor de la clase BitmapFramer.
		/// </summary>
		public BitmapFramer()
		{
		}

		/// <summary>
		/// En este metodo se realiza el recuadre de la imagen.
		/// </summary>
		/// <param name="image">
		/// La matriz bidimensional que contiene la imagen que deseamos 
		/// recuadrar.
		/// </param>
		/// <returns>
		/// Una matriz bisimensional con la imagen recuadrada.
		/// </returns>
		public override FloatBitmap Apply(FloatBitmap image)
		{
			// Coordenadas del menor rectangulo que contiene la imagen
			int x1, x2, y1, y2;
			
			try
			{
				ImageBoxerHelper.BoxImage(image,out x1,out y1,out x2,out y2);
			} 
			catch(ApplicationException)
			{
				return image;
			}
			
			// Numero de filas o columnas a añadir
			int relleno;
			
			int height= y2 - y1 + 1;
			int width= x2 - x1 + 1;

			FloatBitmap framedImage=null;

			if(height >= width) 
			{
				relleno = (height - width);
				framedImage = 
					CreateNewImageColumns(image, relleno, y1, x1, height, width);
			} 
			else 
			{
				relleno = (width - height);
				framedImage = CreateNewImageRows(image, relleno, 
				                                 y1, x1,
				                                 height, width);
			}

			return framedImage;
		}

		/// <summary>
		/// Crea una nueva imagen añadiendo columnas en blanco a izquierda y derecha.
		/// </summary>
		/// <param name="image">La imagen a la que vamos a añadir columnas.</param>
		/// <param name="pad">El ancho del relleno a añadir.</param>
		/// <param name="y1">La coordenada Y de la esquina superior izquierda del contenido.</param>
		/// <param name="x1">La coordenada X de la esquina superior izquierda del contenido.</param>
		/// <param name="height">La altura del contenido.</param>
		/// <param name="width">La anchura del contenido.</param>
		/// <returns> 
		/// Una matriz bidimensional con la imagen con las columnas añadidas.
		/// </returns>
		private FloatBitmap CreateNewImageColumns(FloatBitmap image, int pad, int y1,
		                                          int x1, int height, int width)
		{
			// La nueva altura es la antigua mas dos, porque añdimos una
			// filas en blanco como borde
			// la nueva anchura es igual a la altura
			int newWidth=height+2;
			int newHeight=height+2;			
			
			FloatBitmap newImage = new FloatBitmap(newWidth,newHeight);
			
			for(int i=0;i<newWidth;i++)
			{
				for(int j=0;j<newHeight;j++)
				{
					newImage[i,j]= FloatBitmap.White;
				}
			}
			
			// Copiamos la imagen original centrada
			for(int i=0;i<width;i++)
			{
				for(int j=0;j<height;j++)
				{
					int centerH=i+(int)Math.Ceiling(((double)pad)/2.0)+1;
					newImage[centerH,j+1]=image[i+x1,j+y1];
				}
			}	
			
			return newImage;
		}

		/// <summary>
		/// Crea una nueva imagen añadiendo filas en blanco arriba y abajo.
		/// </summary>
		/// <param name="image">
		/// Una matriz bidemiensional con la imagen la que queremos añadir filas.
		/// </param>
		/// <param name="pad">El ancho del relleno.</param>
		/// <param name="y1">
		/// La coordenada Y de la esquina superior izquierda del contenido.
		/// </param>
		/// <param name="x1">
		/// La coordenada X de la esquina superior izquierda del contenido.
		/// </param>
		/// <param name="height">La altura del contenido.</param>
		/// <param name="width">La anchura del contenido.</param>
		/// <returns>
		/// Una matriz bidimensional con la imagen con las filas añadidas.
		/// </returns>
		private FloatBitmap CreateNewImageRows(FloatBitmap image, int pad, int y1,
			int x1, int height, int width)
		{
			int newWidth=width+2;
			int newHeight=width+2;
			
			// La nueva altura es la antigua mas dos, porque añadimos una
			// fila en blanco como borde la nueva anchura es igual a la altura
			FloatBitmap newImage = new FloatBitmap(newWidth,newHeight);
			
			for(int i=0; i<newWidth; i++)
			{
				for(int j=0; j<newHeight; j++)
				{
					newImage[i, j]=FloatBitmap.White;
				}
			}
			
			for(int i=0;i<width;i++)
			{
				for(int j=0;j<height;j++)
				{
					int centerV = j + (int)Math.Ceiling(((double)pad)/2.0)+1;
					newImage[i+1, centerV]=image[i+x1, j+y1];
				}
			}			
						
			return newImage;
		}

		/// <value>
		/// Contiene la cadena con los parametros de la clase en forma de cadena.
		/// </value>
		public override string Values 
		{
			get { return ""; }
		}


	}
}
