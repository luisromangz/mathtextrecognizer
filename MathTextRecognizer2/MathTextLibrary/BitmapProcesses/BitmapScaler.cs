using System;

using Gdk;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.BitmapProcesses
{
	/// <summary>
	/// Esta clase se encarga de escalar una imagen cuadrada.
	/// </summary>
	[BitmapProcessDescription("Escala de imagen")]
	public class BitmapScaler : BitmapProcess
	{
		#region Atributos
		
		private int normalizedSize;		
		
		#endregion Atributos

		#region Propiedades		
		
		/// <value>
		/// Propiedad para establecer y recuperar el tamaño final de las
		/// imagenes procesadas con una instancia de esta clase.
		/// </value>
		[BitmapProcessPropertyDescription("Tamaño final")]
		public int NewSize
		{
			get
			{
				return normalizedSize;
			}
			set
			{
				normalizedSize=value;
			}
		}
		
		/// <value>
		/// Contiene la cadena con los parametros del algoritmo formateados.
		/// </value>
		public override string Values 
		{
			get { return "Tamaño: "+normalizedSize; }
		}

				
		#endregion Propiedades
		
		#region Métodos publicos
		
		/// <summary>
		/// Constructor de la clase <c>BitmapScaler</c>.
		/// </summary>
		public BitmapScaler()
			: this(50)
		{					
		
		}
		
		/// <summary>
		/// Constructor de la clase <c>BitmapScaler</c>.
		/// </summary>
		/// <param name = "size">
		/// El tamaño de las imagenes resultantes del procesar con el
		/// objeto creado.
		/// </param>
		public BitmapScaler(int size) 
		{					
			normalizedSize=size;
		}
		
		
		/// <summary>
		/// Este metodo efectua el escalado de la imagen.
		/// </summary>
		/// <param name="image">La imagen que queremos escalar.</param>
		/// <returns>La imagen escalada al tamaño establecido.</returns>
		public override FloatBitmap Apply(FloatBitmap image) 
		{
			Pixbuf pb = image.CreatePixbuf();
				
			pb = pb.ScaleSimple(normalizedSize, normalizedSize, InterpType.Bilinear);
			
			return FloatBitmap.CreateFromPixbuf(pb);
		}
		
		#endregion Métodos publicos


		
	}
}
