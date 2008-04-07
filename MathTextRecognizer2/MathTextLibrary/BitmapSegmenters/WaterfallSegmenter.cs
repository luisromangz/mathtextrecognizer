// FlowSegmenter.cs created with MonoDevelop
// User: luis at 15:35 07/04/2008

using System;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Projection;

namespace MathTextLibrary.BitmapSegmenters
{
	
	/// <summary>
	/// Implementa un segmentador de imagenes que usa un metodo que permite
	/// separar simbolos que no estan separados por huecos completamente
	/// verticales u horizontales, sino con formas arbitrarias.
	/// </summary>
	public class WaterfallSegmenter : IBitmapSegmenter
	{
		private WaterfallSegmenterMode mode;
		
		
		
		/// <summary>
		/// Constructor de <c>WaterfallSegmenter</c>
		/// </summary>
		/// <param name="mode">
		/// A <see cref="WaterfallSegmenterMode"/>
		/// </param>
		public WaterfallSegmenter(WaterfallSegmenterMode mode)
		{
			this.mode = mode;
		}

		/// <summary>
		/// Separa una imagen en los simbolos que la contienen usando el 
		/// metodo en cascada.
		/// </summary>
		/// <param name="mtb">
		/// A <see cref="MathTextBitmap"/>
		/// </param>
		/// <returns>
		/// A <see cref="List`1"/>
		/// </returns>
		public List<MathTextBitmap> Segment (MathTextBitmap mtb)
		{
			return WaterfallSegment(mtb);
		}
		
		private List<MathTextBitmap> WaterfallSegment(MathTextBitmap bitmap)
		{
			FloatBitmap image = Rotate(bitmap.FloatImage);
			
			// Buscamos un pixel blanco en el que empezar.
			
			int x0=0;
			int y0=0;
			
			
			
			List<Gdk.Point> points = new List<Gdk.Point>();
			
			// Aqui vamos a pintar la linea que divide las dos partes de la imagen.
			FloatBitmap cutImage = new FloatBitmap(image.Width,image.Height);
			
			// Buscamos por filas, desde abajo, para encontrar el primer pixel 
			// negro mas cercano a la esquina (0,0).
			int i,j,k;
			bool startFound = false;
			for(j=0; j<image.Height && !startFound; j++)
			{
				for(i=0; i<image.Width && !startFound;i++)
				{
					
					if(image[i,j] != MathTextBitmap.White)
					{
						for(k=0; k<j ;k++)
						{
							// Añadimos a la lista de puntos los puntos 
							// entre el borde inferior y el punto en la fila
							// inmediatamente inferior a la del punto negro.
							
							points.Add(new Gdk.Point(i,k));
							
							Console.WriteLine(i+" "+k);
							
							cutImage[i,k] = MathTextBitmap.Black;
						}
						
						startFound = true;
					}					
				}
			}
			
			int x = points[points.Count-1].X;
			int y = points[points.Count-1].Y;
				
			bool newPoints = true;
			bool borderFound = false;
			while(x < image.Width && y < image.Height && newPoints && !borderFound)
			{
				if(x == image.Width -1
				   || y == image.Height -1)
				{
					
					borderFound = true;					
					
				}
				else
				{
					// Aqui almacenamos los posibles vectores..
					int [] vectors = new int[]{0,1,1,1,1,0,1,-1,-1,1,-1,0,-1,-1,0,-1};
						
					bool notNewFound = true;
					for ( k = 0; k < vectors.Length && notNewFound; k+=2)
					{
						
						int xd = vectors[k];
						int yd = vectors[k+1];
						// Vamos comprobando los casos
						// Tenemos que:
						// · No salirnos de los limites
						// · El pixel ha de ser blanco.
						// . No puede ser el anterior del actual, para no meternos
						//   en un bucle.	
						if(x + xd > 0
						   && y + yd > 0
						   && x + xd < image.Width
						   && y + yd < image.Height
						   && image[x+ xd, y+yd] == MathTextBitmap.White
						   && !points.Contains(new Gdk.Point(x+xd, y +yd)))
							
						{
							x = x +xd;
							y = y + yd;
							points.Add(new Gdk.Point(x,y));
							
							Console.WriteLine(x +" "+y);
							
							cutImage[x,y]= MathTextBitmap.Black;
							
							// Indicamos que hemos encontrado el valor.
							notNewFound = false;
						}
					}
					
					newPoints = !notNewFound;
					
				}
			}
			
			if(!newPoints)
			{
				
			}
			else
			{
				
			}
				
			
			List<MathTextBitmap> bitmaps = new List<MathTextBitmap>();
				bitmaps.Add(new MathTextBitmap(cutImage, new Gdk.Point(0,0)));
			return bitmaps;
		}
		
		/// <summary>
		/// Gira la imagen para adecuarla al algorimto.
		/// </summary>
		/// <param name="bitmap">
		/// La imagen a rotar.
		/// </param>
		/// <returns>
		/// La imagen girada.
		/// </returns>
		private FloatBitmap Rotate(FloatBitmap bitmap)
		{
			FloatBitmap image = null;
			switch(mode)
			{
				
				case(WaterfallSegmenterMode.RightToLeft):
					image = bitmap.Rotate90();
					break;
				case(WaterfallSegmenterMode.TopToBottom):
					image = new FloatBitmap(bitmap);
					break;
				case(WaterfallSegmenterMode.BottomToTop):					
					image = bitmap.Rotate90().Rotate90();
					break;
				default:
					image = bitmap.Rotate90().Rotate90().Rotate90();
					break;
			}
			
			return image;
		}
		
	}
}
