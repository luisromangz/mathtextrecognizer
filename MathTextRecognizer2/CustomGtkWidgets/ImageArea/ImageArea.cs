// project created on 22/12/2005 at 18:27
using System;

using Gtk;
using Gdk;

namespace CustomGtkWidgets.ImageArea
{
    
	
	/// <summary>
	/// Esta clase implementa un control GTK para la visualización de imágenes
	/// representadas por la clase <code>System.Drawing.Image</code>.
	/// </summary>
	public class ImageArea: DrawingArea
	{
		private ImageAreaMode mode;
		private Pixbuf image;
		
		private float zoom;
		
		public event EventHandler ZoomChanged;
		
		/// <summary>
		/// Constructor por defecto de ImageArea.
		/// </summary>
		public ImageArea() : base()
		{		
			AddEvents((int)(Gdk.EventMask.ButtonPressMask
							|Gdk.EventMask.ButtonReleaseMask
							|Gdk.EventMask.PointerMotionMask));			
							
			mode = ImageAreaMode.Strecht;			
						
			ShowAll();
		}

        /// <summary>
        /// Propiedad para establecer o recuperar el modo de ajuste que se
        /// usará para la imagen al representarla en el control.
        /// </summary>
		public ImageAreaMode ImageMode
		{
			get
			{
				return mode;
			}
			set
			{
				mode=value;
				QueueDraw();
			}	
		}
		
		/// <summary>
		/// Permite recuperar el escalado de la imagen.
		/// Si el modo es <c>Stretch</c>, devuelve siempre 0.
		public float Zoom
		{
			get
			{
				return zoom;
			}
			set
			{
				if(value != zoom)
				{				
					if(ZoomChanged != null)
						ZoomChanged(this,EventArgs.Empty);
						
					zoom=value;		
				
				}	
			}
		}
		
		/// <summary>
		/// Propiedad para establecer o recuperar la imagen representada en el
		/// control.
		/// </summary>
		public Pixbuf Image
		{
			get
			{
				return image;
			}
			
			set
			{
				image=value;
				QueueDraw();				
			}
		}
		
		/// <summary>
		/// Método para gestionar el redibujado del control.
		/// </summary>
		protected override bool OnExposeEvent(EventExpose arg)
		{
			if(image!=null)
			{
			    
				int x,y,sx,sy,d;					
				GdkWindow.GetGeometry(out x,out y,out sx,out sy,out d);					
				Pixbuf scaled; 
				// Según el modo de ajuste seleccionado, actuamos de una u 
				// otra manera.
				switch(mode)
				{
				case(ImageAreaMode.Strecht):
				    // Ajuste de la imagen a las dimensiones del control.
				    zoom = 0;
					
					scaled = image.ScaleSimple(sx,sy, InterpType.Bilinear);
					
					GdkWindow.DrawPixbuf(
						new Gdk.GC(GdkWindow),
						scaled,
						0,0,
						0,0,
						sx,sx,
						RgbDither.None,0,0);
					
						
					break;
					
					
				case(ImageAreaMode.Zoom):
				    // Aquí tenemos que ajustar solamente a la demensión
				    // mayor, tenemos que calcular esto y luego multiplicar
				    // la otra dimensión por el factor obtenido para mantener
				    // la proporción entre alto y ancho de la imagen.
				    
					int sizeX,sizeY;
					int dx=0,dy=0;
					if(((sx+1.0f)/(sy+1.0f))<((image.Width+1.0f)/(1.0f+image.Height)))
					{							    					
						sizeX=sx;
						
						sizeY=(int)(image.Height*((float)sx)/image.Width);
						dy=(sy-sizeY)/2;
					}
					else
					{								
						sizeY=sy;
						
						sizeX=(int)(image.Width*((float)sy)/image.Height);
						dx=(sx-sizeX)/2;
					}
					
					Zoom = ((float)image.Width+1f) / ((float)(sx+1f));
					
					if(sizeX >= 5 && sizeY >= 5)
					{					    
						scaled = image.ScaleSimple(sizeX,sizeY, InterpType.Bilinear);			
					    
					
						GdkWindow.DrawPixbuf(
							new Gdk.GC(GdkWindow),
							scaled,
							0,0,
							dx,dy,
							sizeX,sizeY,
							RgbDither.None,0,0);
					}
					break;
				}			
				
			}
			return true;
			
		}

	}

}