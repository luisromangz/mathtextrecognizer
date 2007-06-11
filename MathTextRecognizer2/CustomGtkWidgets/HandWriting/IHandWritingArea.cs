using System;
using Gtk;
using System.Collections;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;

namespace CustomGtkWidgets.HandWriting{

	//Aqui se declaran dos tipos metodo para usarlos para gestionar eventos
	public delegate void StrokeAddedEventHandler(object sender);
	public delegate void ClearedEventHandler(object sender);
	public delegate void StrokeRemovedEventHandler(object sender);
	
	public abstract class IHandWritingArea : DrawingArea
	{	
		//Se declaran eventos, que se gestionaran con métodos de los tipos indicados
		public event StrokeAddedEventHandler StrokeAdded;
		public event ClearedEventHandler Cleared;
		public event StrokeRemovedEventHandler StrokeRemoved;
	
	
		protected Pen foreground;
		protected SmoothingMode smoothingMode;
		
		//El constructor, con su llamadita al de la superclase		
		public IHandWritingArea():base(){
			
			//Para que el widget gtk sea capaz de escuchar los eventos,
			//hay que establecer unos flags de eventos
			AddEvents((int)(Gdk.EventMask.ButtonPressMask
							|Gdk.EventMask.ButtonReleaseMask
							|Gdk.EventMask.PointerMotionMask));						
			
			
			InitializeWidget();
			
			ShowAll();
		}
	
	
		
		protected abstract void InitializeWidget();		
		
		
		//Para permitir lanzar los eventos de forma facil, los envolvemos en
		//un metodo protegido, de forma que de paso hacemos que el evento pueda
		//ser lanzado desde clases derivadas, cosa que a pelo con el evento no es
		//posible
		protected void StrokeAddedEvent(){
			if(StrokeAdded!=null){
				StrokeAdded(this);
			}			
		}
		
		protected void StrokeRemovedEvent(){
			if(StrokeRemoved!=null){
				StrokeRemoved(this);
			}		
		}
		
		protected void ClearedEvent(){
			
			if(Cleared!=null){
				Cleared(this);			
			}
		
		}		 
		
		public abstract void Clear();

		public abstract void UndoLastStroke();

		//Esto es una propiedad, basicamente es como tener un metodo set, get o los dos,
		//con la gracia de que se usa como si fuese un atributo.		
		public abstract Bitmap Bitmap{
			get;
		
		}
		
		//Otra propiedad, con un uso más clasico. Lo bueno de las propiedades es que
		//se usan como atributos, pero permiten meter codigo para por ejemplo gestion 
		//de errores, concurrencia etc.
		
		public SmoothingMode SmoothingMode{
			get{
				return smoothingMode;				
			}
			
			set{
			
				smoothingMode=value;
			}
		
		}
		
		public Pen LineStyle{
			get{
				return foreground;
			}
			set{
				foreground=value;
			}			
		}
		
		//Ocultando el metodo OnExposeEvent de DrawingArea conseguimos gestionar
		//el evento de redibujado del control sin mas problemas
		protected abstract override bool OnExposeEvent(Gdk.EventExpose arg);		
		
	}
	
	public class Stroke{
	
		private Pen pencil;
		
		private ArrayList points;
		private Point[] arrayPoints;
		
		public Stroke(Pen p){
			points=new ArrayList();
			pencil=p;
		}
		
		public void Draw(Graphics g){
			
			arrayPoints=new Point[points.Count];
			
			points.CopyTo(arrayPoints);
			
			g.DrawCurve(pencil,arrayPoints);
		}
		
		public Pen Pencil{
			set{
				pencil=value;
			}
			
			get{
				return pencil;
			}
		
		}
		
		public void AddPoint(Point p){
			points.Add(p);		
		}
	
	}	
}
