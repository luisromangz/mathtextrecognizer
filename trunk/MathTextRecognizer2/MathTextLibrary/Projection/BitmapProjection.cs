// created on 15/12/2005 at 14:43
using System;
using System.Collections.Generic;

using Gdk;

namespace MathTextLibrary.Projection
{
	/// <summary>
	/// La clase abstracta <c>BitmapProjection</c> representa
	/// la base comun necesaria para la realizacion de proyecciones
	/// horizontales u verticales de imagenes, es decir,la suma por
	/// columnas o por filas del numero de pixeles de una imagen, 
	/// respectivamente.
	/// </summary>
	public abstract class BitmapProjection
	{
		/// <summary>
		/// Array donde se almacena la suma por filas o columnas,
		/// segun el caso.
		/// </summary>
		protected int [] projection;	
		/// <summary>
		/// El modo con el que se ha obtenido la proyeccion.
		/// </summary>
		protected ProjectionMode mode;
		/// <summary>
		/// Un <c>Pixbuf</c> en el que se almacena la representacion 
		/// grafica de la proyeccion.
		/// </summary>
		protected Pixbuf projBitmap;
		
		/// <summary>
		/// Una lista con los huecos que presenta la proyeccion.
		/// </summary>
		protected List<Hole> holes;
		/// <summary>
		/// Metodo fabrica para construir los distintos tipos de 
		/// proyeccion disponibles.
		/// </summary>
		/// <param name="mode">
		/// El sentido en el que se hara la proyeccion, vertical u horizontal.
		/// </param>
		/// <param name="mtb">
		/// La imagen a la que se calculara la proyeccion.
		/// </param>
		/// <returns>
		/// La proyeccion de la imagen <c>mtb</c> con el modo <c>mode</c>.
		/// </returns>
		public static BitmapProjection CreateProjection(
			ProjectionMode mode, MathTextBitmap mtb)
		{
			BitmapProjection res=null;
			
			switch(mode)
			{
				case(ProjectionMode.Horizontal):
					res=new HorizontalBitmapProjection(mtb);
					break;
				case(ProjectionMode.Vertical):
					res=new VerticalBitmapProjection(mtb);
					break;
				default:
					throw new ArgumentException(
						"No puede usar None para crear una nueva projección");				
			}
			return res;
		}
		
		/// <summary>
		/// Permite obtener la lista de huecos presentes en la proyeccion.
		/// </summary>
		public List<Hole> Holes
		{
			get
			{
				if(holes==null)
				{
					CreateHoles();
				}
				return holes;			
			}		
		}
		
		/// <summary>
		/// Este metodo se encarga de construir la lista de huecos de la
		/// proyección.
		/// </summary>
		private void CreateHoles()
		{
			holes=new List<Hole>();					
			for(int i=0;i<projection.Length;i++)
			{
				if(projection[i]==0)
				{
					int j=0;				
					// Si, esto es asi, con el punto y coma al final.
					for(j=i;j<projection.Length && projection[j]==0;j++);					
					// Cuando acaba el bucle, tenemos el comienzo y 
					// el final del hueco;
					holes.Add(new Hole(i,j-1));					
					i=j;			
				}	
			}		
		}
		
		/// <summary>
		/// Este metodo sirve para invocar al abstracto <c>CreateProjection</c>.
		/// </summary>
		/// <param name="image">
		/// La imagen a la que le queremos calcular la proyeccion.
		/// </param>
		protected BitmapProjection(MathTextBitmap image)
		{		
			CreateProjection(image);
		}
		/// <summary>
		/// Propiedad de solo lectura que permite obtener el tamaño de la
		/// proyección.
		/// </summary>
		public int Size
		{
			get
			{
				return projection.Length;
			}		
		}
		
		/// <summary>
		/// Esta propiedad de solo lectura permite obtener la representación
		/// gráfica de la proyección.
		/// </summary>
		/*public Pixbuf ProjectionImage		{
			get
			{
				if(projBitmap==null)
				{
					projBitmap=CreateBitmap();
				}
				return projBitmap;
			}
		}*/
		/// <summary>
		/// Crea una imagen que representa la proyeccion.
		/// </summary>
		/// <returns>
		/// Un <c>Pixbuf</c> representando la proyeccion.
		/// </returns>
		// protected abstract Pixbuf CreateBitmap();		
		/// <summary>
		/// Crea la proyección de una imagen.
		/// </summary>
		/// <param name="image">
		/// La imagen cuya proyeccion calcularemos.
		/// </param>
		protected abstract void CreateProjection(MathTextBitmap image);
	}
}
