using System;
using Gtk;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;

namespace CustomGtkWidgets.HandWriting
{	
	
	public class VectorHandWritingArea : IHandWritingArea
	{	
		//Lista de objetos object
		private List<List<Point>> strokes;
		private List<Point> lastStroke;	
		
		
		private Color background;
		
		private bool mouseButtonPressed;
		
		private int oldX;
		private int oldY;
		
		//El constructor, con su llamadita al de la superclase		
		public VectorHandWritingArea()
		    :base()
		{
		}

		protected override void InitializeWidget()
		{
			strokes=new List<List<Point>>();
			
			mouseButtonPressed=false;
			
			background=Color.White;
			foreground=Pens.Black;			
		} 
		
		public override void Clear()
		{		
			strokes.Clear();
			
			//El metodo QueueDraw de los widget gtk fuerza el redibujado del control
			QueueDraw();
			ClearedEvent();
		}

		public override void UndoLastStroke()
		{
			strokes.RemoveAt(strokes.Count-1);
			QueueDraw();
			StrokeRemovedEvent();
			if(strokes.Count==0){
				 ClearedEvent();	
			}		
		}

		//Esto es una propiedad, basicamente es como tener un metodo set, get o los dos,
		//con la gracia de que se usa como si fuese un atributo.		
		public override Bitmap Bitmap
		{
			get
			{
				int x,y,sx,sy,d;
				this.GdkWindow.GetGeometry(out x,out y,out sx,out sy,out d);				
				Bitmap res=new Bitmap(sx,sy);
				
				using(Graphics g =Graphics.FromImage(res))
				{
				
					g.SmoothingMode=smoothingMode;
					g.Clear(background);
					
					foreach(List<Point> stroke in strokes)
					{
						DrawStroke(g,stroke);					
					}				
				}
				
				return res;
			
			}
		
		}
		
		//Otra propiedad, con un uso más clasico. Lo bueno de las propiedades es que
		//se usan como atributos, pero permiten meter codigo para por ejemplo gestion 
		//de errores, concurrencia etc.
		public Color BackgroundColor
		{
			get
			{
				return background;
			}
			
			set
			{
				background=value;
			}		
		}
		
		// Ocultando el metodo OnExposeEvent de DrawingArea conseguimos gestionar
		// el evento de redibujado del control sin mas problemas
		protected override bool OnExposeEvent(Gdk.EventExpose arg)
		{
			
			using(Graphics g=Gtk.DotNet.Graphics.FromDrawable(this.GdkWindow))
			{
				
				g.SmoothingMode=smoothingMode;
				g.Clear(background);
				
				foreach(List<Point> stroke in strokes)
				{
					DrawStroke(g,stroke);					
				}
				
				if(mouseButtonPressed)
				{
					DrawStroke(g,lastStroke);
				}
			}
			
			return true;
		}		
		
				
		private void DrawStroke(Graphics g, List<Point> stroke)
		{
			Point[] arraystroke=new Point[stroke.Count];
			//Creo un array a partir de una lista
			stroke.CopyTo(arraystroke);
			//Dibujo una spline usando la funcion de GDI
			g.DrawCurve(foreground,arraystroke);
		}
		
		//Gestion del apretar el boton del raton en el control,
		//empezamos a dibujar
		protected override bool OnButtonPressEvent(Gdk.EventButton arg)
		{
			//Vemos si es el botón principal del ratón
			if(arg.Button==1)
			{						
				
				mouseButtonPressed=true;
				oldX=(int)arg.X;
				oldY=(int)arg.Y;	
				
				lastStroke=new List<Point>();
				lastStroke.Add(new Point((int)arg.X,(int)arg.Y));
			}
			
			return true;
		}
		
		//Gestion del soltar el boton del raton sobre el control,
		//dejamos de dibujar
		protected override bool OnButtonReleaseEvent(Gdk.EventButton arg)
		{
			//Vemos si es el botón principal del ratón
			if(arg.Button==1)
			{							
				mouseButtonPressed=false;				
				strokes.Add(lastStroke);
				StrokeAddedEvent();
			}
			return true;
		}
		
		//Gestion del movimiento del raton sobre el control, si estamos dibujando,
		//vamos almacenando los puntos por los que pasa el raton, para formar el trazo
		protected override bool OnMotionNotifyEvent(Gdk.EventMotion arg)
		{
			
			if(mouseButtonPressed)
			{
				double d1=oldX-arg.X;				
				double d2=oldY-arg.Y;
				
				//Solo añadimos el punto si esta algo separado del anterior,
				//para evitar mucho petamiento
				if((d1*d1+d2*d2)>5)
				{								
					
					lastStroke.Add(new Point((int)arg.X,(int)arg.Y));
					
					oldX=(int)arg.X;
					oldY=(int)arg.Y;
					
					QueueDraw();
				}
			
			}
			
			return true;
		}
	}

}