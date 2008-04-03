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
		private float [,] binaryzedImage;
		
		// Hijos de la imagen productos de la segmentación
		private List<MathTextBitmap> children;
		
		// A partir de qué proyección se ha obtenido la imagen
		private ProjectionMode fromProjection;
		
		// Altura de la imagen sin procesar
		private int height;
		
		// La imagen como un array de float
		private float [,] image;
		
		// Posición de la esquina superior izquierda de la imagen
		// en la imagen completa
		private Point position;
		
		// Imagen con el preprocesamiento completo
		private float [,] processedImage;
		
		// Tamaño de la imagen procesada
		private int processedImageSize;
		
		// Simbolo por el que se ha reconocido la imagen
		private MathSymbol symbol;
		
		// Anchura de la imagen sin procesar
		private int width;
		
#endregion Atributos no públicos
		
#region Eventos

		public event MathTextBitmapChildrenAddedEventHandler ChildrenAdded;
		public event MathTextBitmapSymbolChangedEventHandler SymbolChanged;
		
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
			image = Utils.ImageUtils.CreateMatrixFromPixbuf(b);		
			fromProjection = ProjectionMode.None;	
			width = b.Width;
			height = b.Height;
		}	
		
		/// <summary>
		/// Constructor de un nuevo <c>MathTextBitmap</c> a partir de un
		/// array de float, una posicion y un modo de proyeccion.
		/// </summary>
		/// <remarks>
		/// El array se clona para evitar efectos laterales. La imagen se
		/// procesa mediante el metodo <c>ProcessImage()</c>. 
		/// </remarks>
		public MathTextBitmap(float [,] image, Point pos, ProjectionMode pM)
		{
			this.image=(float [,])(image.Clone());
			this.position=pos;			
			this.fromProjection=pM;	
			
			width = image.GetLength(0);
			height = image.GetLength(1);
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
		/// Contiene el <c>Pixbuf</c> que representa a la imagen binarizada
		/// mediante el metodo <c>MathTextBitmap.CreateBitmapFromGrayImage()</c>.
		/// </value>
		/// <remarks>
		/// El bitmap creado estara en escala de grises.
		/// </remarks>
		public Pixbuf BinaryzedBitmap
		{
			get
			{			
				return Utils.ImageUtils.CreatePixbufFromMatrix(binaryzedImage);
			}
		}
		
		/// <value>
		/// Contiene la imagen binarizada como un array de float.
		/// </value>
		public float[,] BinaryzedImage
		{
			get{
				return binaryzedImage;
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
				return Utils.ImageUtils.CreatePixbufFromMatrix(image);
			}	
		}
		
		/// <value>
		/// Contiene los hijos de la imagen actual que se hayan obtenido
		/// mediante segmentacion.
		/// </value>
		public List<MathTextBitmap> Children
		{			
			get{
				return children;
			}			
		}	
		
		/// <value>
		/// Contiene el modo de proyeccion desde que originó la imagen.
		/// </value>
		public ProjectionMode FromProjection
		{
			get
			{
				return fromProjection;			
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
				return Utils.ImageUtils.CreatePixbufFromMatrix(processedImage);
			}
		}		

		/// <value>
		/// Contiene la imagen procesada como un array de float.
		/// </value>
		public float[,] ProcessedImage
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
				if(symbol.SymbolType!=MathSymbolType.NotRecognized)
				{
					OnSymbolChangedLauncher();
				}
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
		
#endregion Propiedades
		
#region Metodos públicos

		/// <summary>
		/// Intenta dividir la imagen por segmentacion. En caso de tener exito
		/// se actualizan los hijos de la imagen actual.
		/// </summary>
		/// <remarks>
		/// Siempre se intenta segmentar la imagen actual en horizontal,
		/// y si esto no tiene exito entonces se intenta segmentar en vertical.
		/// </remarks>
		public void CreateChildren()
		{
			if(children==null)
			{							
				children = HorizontalSegmentation();
			
				if(children != null && children.Count>1)
				{
					OnChildrenAddedInvoker(children);	
				}
				
				/*
				// antiguo metodo de segmentacion: si la imagen actual se habia
				// obtenido por segmentacion horizontal, empezamos intentando
				// segmentar en vertical, y viceversa
				if(fromProjection==ProjectionMode.Horizontal){
					children = VerticalSegmentation();
				}else{
					children = HorizontalSegmentation();
				}
				*/
			}
		}		

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
		
		/// <summary>
		/// Devuelve la imagen procesada como un array de float.
		/// </summary>
		/// <param name="x">Minima posicion horizontal de la subimagen.</param>
		/// <param name="y">Minima posicion vertical de la subimagen.</param>
		/// <param name="width">Anchura de la subimagen medida desde <c>x</c></param>
		/// <param name="height">Altura de la subimagen medida desde <c>y</c></param>
		/// <returns>Array de float que representa la subimagen deseada.</returns>
		public float[,] SubImage(int x,int y,int width,int height)
		{
			float[,] resImage=new float[width,height];
			
			for(int i = 0; i < width; i++)
			{
				for(int j = 0; j < height; j++)
				{
					resImage[i,j] = image[x + i, y + j];
				}			
			}
			
			return resImage;
		}

		#endregion Métodos públicos
		
		#region Métodos no públicos
		
		/// <summary>
		/// Intenta segmentar la imagen en horizontal y si no tiene exito
		/// prueba en vertical.
		/// </summary>
		/// <remarks>
		/// Actualmente se usa el segmentador <c>AllHolesProjectionBitmapSegmenter</c>
		/// para el segmentado horizontal y <c>FractionVerticalBitmapSegmenter</c>
		/// para el segmentado vertical.
		/// </remarks>
		private List<MathTextBitmap> HorizontalSegmentation()
		{
			IBitmapSegmenter segmenter;
			List<MathTextBitmap> childrenTemp;
			
			segmenter = new AllHolesProjectionBitmapSegmenter(ProjectionMode.Horizontal);
					
			childrenTemp=segmenter.Segment(this);
			
			if(childrenTemp.Count < 2)
			{ 
				//Segmentacion sin exito :/
				segmenter=new FractionVerticalBitmapSegmenter();
				childrenTemp=segmenter.Segment(this);
				if(childrenTemp.Count < 2)
				{
					childrenTemp=null;
					//throw new Exception("Umm no se ha podido segmentar else");							
				}
			}
			return childrenTemp;
		}
		
		/// <summary>
		/// Metodo que se llama cuando se añaden hijos a la imagen para
		/// notificar al controlador.
		/// </summary>
		protected void OnChildrenAddedInvoker(List<MathTextBitmap> children)
		{
			if(ChildrenAdded != null)
			{
				ChildrenAdded(this,
				              new MathTextBitmapChildrenAddedEventArgs(children));
			}		
		}
		
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
		
		/// <summary>
		/// Intenta segmentar la imagen en vertical y si no tiene exito
		/// prueba en horizontal.
		/// </summary>
		/// <remmarks>
		/// Este método no se usa actualmente.
		/// </remmarks>
		private List<MathTextBitmap> VerticalSegmentation()
		{
			IBitmapSegmenter segmenter;
			List<MathTextBitmap> childrenTemp;
			segmenter=new FractionVerticalBitmapSegmenter();
			childrenTemp = segmenter.Segment(this);
			
			if(childrenTemp.Count < 2)
			{ 
				// Segmentacion sin exito :/
				segmenter=new DifferentialProjectionBitmapSegmenter(ProjectionMode.Horizontal);
				childrenTemp = segmenter.Segment(this);
				if(childrenTemp.Count < 2)
				{
					childrenTemp=null;
					//throw new Exception("Umm no se ha podido segmentar");							
				}
			}
			return childrenTemp;
		}
		
		#endregion Métodos no públicos
	}
}
