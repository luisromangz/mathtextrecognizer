using System;
using Gtk;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;

namespace CustomGtkWidgets.HandWriting
{

	
	public class RasterHandWritingArea : IHandWritingArea
	{		
	
		//Lista de objetos object
		//private ArrayList strokes;
		private Stroke lastStroke;	
		
		private Bitmap bgBitmap;
				
		private bool mouseButtonPressed;
		
		private int oldX;
		private int oldY;
		
		private int strokeCount;
		
		//El constructor, con su llamadita al de la superclase		
		public RasterHandWritingArea()
		    :base()
		{}

		protected override void InitializeWidget()
		{
			mouseButtonPressed=false;
						
			foreground=Pens.Black;		
			strokeCount=0;
		
			BackgroundInitialize();
		}			
		
		private void BackgroundInitialize()
		{
		
			if(bgBitmap==null)
			{
				bgBitmap=new Bitmap(this.Screen.Width,this.Screen.Height);				
				
			}
			
			using(Graphics bgGraphics=Graphics.FromImage(bgBitmap))
			{				
				bgGraphics.SmoothingMode=smoothingMode;
				bgGraphics.Clear(Color.White);
			}
		}	
		
		public override void Clear()
		{		
			//strokes.Clear();
			using(Graphics bgGraphics = Graphics.FromImage(bgBitmap))
			{
			   bgGraphics.Clear(Color.White);
			}
			lastStroke=null;
			strokeCount=0;
			//El metodo QueueDraw de los widget gtk fuerza el redibujado del control
			QueueDraw();
			ClearedEvent();
		}

		public override void UndoLastStroke()
		{
			if(lastStroke!=null)
			{
				lastStroke=null;
				strokeCount--;
				QueueDraw();
				StrokeRemovedEvent();
				if(strokeCount==0)
				{
					ClearedEvent();
				}				
			}				
		}

		/// <value>
		/// Contiene la imagen que se muestra en el widget.
		/// </value>		
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
					g.Clear(Color.White);
					
					g.DrawImageUnscaled(bgBitmap,0,0,sx,sy);
					
					if(lastStroke!=null)
						lastStroke.Draw(g);
				}
				
				return res;
			
			}
		
		}		
		
		
		//Ocultando el metodo OnExposeEvent de DrawingArea conseguimos gestionar
		//el evento de redibujado del control sin mas problemas
		protected override bool OnExposeEvent(Gdk.EventExpose arg)
		{
			
			using(Graphics g=Gtk.DotNet.Graphics.FromDrawable(this.GdkWindow))
			{
				
				//Dentro del using creo un grafico de .Net usando la clase puente que
				//han añadido en la ultima versión de gtksharp, que permite usar las 
				//funciones de GDI para pintar, y siendo una libreria de alto nivel de
				//graficos nos va a permitir dibujar en plan "fasil"
				
							
				
				int x,y,sx,sy,d;
				this.GdkWindow.GetGeometry(out x,out y,out sx,out sy,out d);	
				g.DrawImageUnscaled(bgBitmap,0,0,sx,sy);			
				
				g.SmoothingMode=smoothingMode;	
				if(lastStroke!=null)
				{
					lastStroke.Draw(g);
				}
			}
			
			return true;
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
				
				if(lastStroke!=null)
				{
					using(Graphics bgGraphics = Graphics.FromImage(bgBitmap))
					{
						lastStroke.Draw(bgGraphics);				
					}
				}
				
				lastStroke=new Stroke(foreground);
				lastStroke.AddPoint(new Point((int)arg.X,(int)arg.Y));
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
				//lastStroke.Add(new Point((int)arg.X,(int)arg.Y));				
				strokeCount++;
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
					
					lastStroke.AddPoint(new Point((int)arg.X,(int)arg.Y));
				
						
					oldX=(int)arg.X;
					oldY=(int)arg.Y;
					
					QueueDraw();
				}
			
			}
			
			return true;
		}
		
		private class Stroke
		{
		
			private Pen pencil;
			
			private List<Point> points;
			private Point[] arrayPoints;
			
			public Stroke(Pen p)
			{
				points=new List<Point>();
				pencil=p;
			}
			
			public void Draw(Graphics g)
			{
				
				arrayPoints=new Point[points.Count];
				
				points.CopyTo(arrayPoints);
				
				g.DrawCurve(pencil,arrayPoints);
			}
			
			public Pen Pencil
			{
				set
				{
					pencil=value;
				}
				
				get
				{
					return pencil;
				}
			
			}
			
			public void AddPoint(Point p)
			{
				points.Add(p);		
			}
		
		}
		
	}

}
