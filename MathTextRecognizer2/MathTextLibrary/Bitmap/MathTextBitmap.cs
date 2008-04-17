using System;
using System.Collections.Generic;

using Gdk;

using MathTextLibrary.Symbol;
using MathTextLibrary.Projection;
using MathTextLibrary.BitmapProcesses;
using MathTextLibrary.BitmapSegmenters;

namespace MathTextLibrary.Bitmap
{
	/// <summary>
	/// Abstrae una imagen y multiples operaciones que se pueden realizar
	/// sobre ella.
	/// </summary>
	public class MathTextBitmap
	{


		
#region Atributos privados
		
		// Imagen sólo binarizada
		private FloatBitmap binaryzedImage;
		
		
		// Altura de la imagen sin procesar
		private int height;
		
		// La imagen como un array de float
		private FloatBitmap image;
		
		// Posición de la esquina superior izquierda de la imagen
		// en la imagen completa
		private Point position;
		
		// Imagen con el preprocesamiento completo
		private List<FloatBitmap> processedImages;
		
		// Anchura de la imagen sin procesar
		private int width;
		
#endregion Atributos no públicos
		
#region Constructores
		
		/// <summary>
		/// Constructor de un nuevo <c>MathTextBitmap</c> a partir de un
		/// <c>Bitmap</c>.
		/// </summary>
		/// <remarks>
		/// El bitmap se convierte a escala de grises y se procesa mediante
		/// el metodo <c>ProcessImage()</c>.
		/// </remarks>
		/// <seealso cref="System.Drawing.Bitmap"/>
		public MathTextBitmap(Pixbuf b)
		{
			this.position = new Point(0,0);
			
			processedImages = new List<FloatBitmap>();
			
			image = FloatBitmap.CreateFromPixbuf(b);	
			width = b.Width;
			height = b.Height;
		}	
		
		/// <summary>
		/// Constructor de un nuevo <c>MathTextBitmap</c> a partir de un
		/// array de float y su posicion y un modo de proyeccion.
		/// </summary>
		/// <remarks>
		/// El array se clona para evitar efectos laterales. La imagen se
		/// procesa mediante el metodo <c>ProcessImage()</c>. 
		/// </remarks>
		public MathTextBitmap(FloatBitmap image, Point pos)
		{
			this.image =  image;
			this.position = pos;
			
			processedImages = new List<FloatBitmap>();
			
			width = image.Width;
			height = image.Height;
		}
		
		#endregion Constructores
		
		#region Propiedades
		
		/// <value>
		/// Contiene el valor de un pixel de la imagen.
		/// </value>
		public float this[int i,int j]
		{
			get
			{
				return image[i,j];
			}
		}
				
		/// <value>
		/// Contiene el <c>Pixbuf</c> que representa a la imagen sin procesar.
		/// </value>
		/// <remarks>
		/// El bitmap creado estara en escala de grises.
		/// </remarks>		
		public Pixbuf Pixbuf
		{
			get
			{		
				return image.CreatePixbuf();
			}	
		}
		
		
		
		/// <value>
		/// Contienela altura de la imagen sin procesar.
		/// </value>
		public int Height
		{
			get
			{				
				return height;
			}
		}
		
		/// <value>
		/// Contienela posición de la esquina superior izquierda de la imagen
		/// actual dentro de la imagen inicial.
		/// </value>
		public Point Position
		{
			get
			{
				return position;
			}
		}	
		
		/// <value>
		/// Contiene los <c>Pixbuf</c> que representas a la imagen procesada.
		/// segun las distintas bases de datos aplicadas.
		/// </value>
		/// <remarks>
		/// Los bitmaps 
		/// </remarks>
		public List<Pixbuf> ProcessedPixbufs
		{
			get
			{	
				List<Pixbuf> pixbufs = new List<Pixbuf>();
				foreach(FloatBitmap img in processedImages)
				{
					pixbufs.Add(img.CreatePixbuf());
				}
				
				return pixbufs;
			}
		}		
		
		/// <value>
		/// Contiene la ultima imagen procesada.
		/// </value>
		public FloatBitmap LastProcessedImage
		{
			get
			{
				if (processedImages.Count == 0)
				{
					return null;
				}
				else
				{
					return processedImages[processedImages.Count-1];
				}
			}
		}

		/// <value>
		/// Contiene las imagenes procesada como un array de float.
		/// </value>
		public List<FloatBitmap> ProcessedImages
		{
			get
			{
				return processedImages;
			}
		}
		
	
		
		/// <value>
		/// Contiene la anchura de la imagen sin procesar.
		/// </value>
		public int Width
		{
			get
			{
				return width;
			}	
		}

		public FloatBitmap FloatImage 
		{
			get 
			{
				return image;
			}
		}
		
#endregion Propiedades
		
#region Metodos públicos

		

		/// <summary>
		/// Procesa la imagen actual mediante binarizacion, encuadre,
		/// normalizacion y adelgazamiento.
		/// </summary>
		public void ProcessImage(List<BitmapProcess> processes)
		{			
			FloatBitmap processedImage = new FloatBitmap(image);
			
			foreach(BitmapProcess process in processes)
			{
				processedImage = process.Apply(processedImage);
			}
			
			
			processedImages.Add(processedImage);
			
		}
		
		

		#endregion Métodos públicos
		
	}
}
