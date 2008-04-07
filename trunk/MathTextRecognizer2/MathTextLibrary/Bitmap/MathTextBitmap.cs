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
#region Constantes
		
		/// <summary>
		/// El valor que representa al color negro en los <c>MathTextBitmap</c>.
		/// </summary>
		public const float Black=0;
		
		/// <summary>
		/// El valor que representa al color blanco en los <c>MathTextBitmap</c>.
		/// </summary>
		public const float White=1;
		
#endregion Atributos privados
		
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
		private FloatBitmap processedImage;
		
		// Tamaño de la imagen procesada
		private int processedImageSize;
		
		// Simbolo por el que se ha reconocido la imagen
		private MathSymbol symbol;
		
		// Anchura de la imagen sin procesar
		private int width;
		
#endregion Atributos no públicos
		
#region Eventos
		
		public event SymbolChangedHandler SymbolChanged;
		
#endregion Eventos
		
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
		public Pixbuf Bitmap
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
		/// Contiene el <c>Pixbuf</c> que representa a la imagen procesada.
		/// </value>
		/// <remarks>
		/// El bitmap creado estara en escala de grises.
		/// </remarks>
		public Pixbuf ProcessedBitmap
		{
			get
			{		
			
				return processedImage.CreatePixbuf();
			}
		}		

		/// <value>
		/// Contiene la imagen procesada como un array de float.
		/// </value>
		public FloatBitmap ProcessedImage
		{
			get
			{
				return processedImage;
			}
		}
		
		/// <value>
		/// Contiene el simbolo asociado a la imagen.
		/// </value>
		/// <remarks>
		/// Al modificar el simbolo se llama al metodo
		/// <c>MathTextBitmap.OnSymbolChanged()</c> salvo si el tipo del nuevo
		/// simbolo es <c>MathSymbolType.NotRecognized</c>.
		/// </remarks>
		public MathSymbol Symbol
		{
			get
			{
				return symbol;
			}
			set
			{
				symbol=value;				
				OnSymbolChangedLauncher();
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

		public FloatBitmap FloatImage {
			get {
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
			processedImage = image;
			
			foreach(BitmapProcess process in processes)
			{
				processedImage = process.Apply(processedImage);
			}
		}
		
		

		#endregion Métodos públicos
		
		#region Métodos no públicos		
		
		/// <summary>
		/// Metodo que se llama cuando se modifica el simbolo de la imagen 
		/// para notificar al controlador.
		/// </summary>
		protected void OnSymbolChangedLauncher()
		{
			if(SymbolChanged!=null)
			{
				SymbolChanged(this,EventArgs.Empty);			
			}
		}
		
		#endregion Métodos no públicos
	}
}
