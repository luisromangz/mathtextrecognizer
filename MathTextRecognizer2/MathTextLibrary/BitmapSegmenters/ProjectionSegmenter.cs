// created on 28/12/2005 at 13:26
using System;
using System.Collections.Generic;

using Gdk;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Projection;

namespace MathTextLibrary.BitmapSegmenters{

	/// <summary>
	/// La clase abstracta <c>ProjectionBitmapSegmenter</c> representa la base
	/// de todos los segmentadores de imagenes que usan la proyeccion de la 
	/// misma para descomponerla en partes mas pequeñas.
	/// </summary>
	public abstract class ProjectionSegmenter : BitmapSegmenter
	{
		
		
		private ProjectionMode mode;
		
		/// <summary>
		/// Constructor de la clase <code>ProjectionBitmapSegmenter</code>.
		/// </summary>
		/// <param name="mode">El modo que se usara para obtener la proyeccion (horizontal,vertical).</param>
		public ProjectionSegmenter(ProjectionMode mode)
		{
			this.mode=mode;		
		}
		
		/// <summary>
		/// Este metodo realiza la descomposicion de la imagen descomponiendola por
		/// los bordes de los huecos presentes en la proyeccion.
		/// Distintas implementaciones difieren en si se eliminan alguno de estos huecos
		/// de forma que se agrupan ciertas subimagenes para ser tratadas como una sola.
		/// </summary>
		/// <param name="image">La imagen que deseamos descomponer.</param>
		/// <returns>
		/// Un array con las distintas partes en que hemos descompuesto
		/// la imagen de entrada.
		/// </returns>
		public override List<MathTextBitmap> Segment(MathTextBitmap image)
		{
		
			//Creamos la proyeccion
			BitmapProjection proj=BitmapProjection.CreateProjection(mode,image);
			List<Hole> holes=proj.Holes;
					
			//Aqui decidimos a partir de que tamao de hueco vamos a cortar la imagen			
						
			int threshold = GetImageCutThreshold(holes);	
			
			//Eliminamos los huecos que tengan menor tamaño que el umbral
			int i=1;
			while(i<holes.Count-1){

				if(((Hole)holes[i]).Size<threshold){
					holes.Remove(holes[i]);				
				}else{
					i++;
				}
			}		
			
			return ImageCut(image,holes);
		}	
		
		/// <summary>
		/// Metodo que se usa para obtener la cantidad de huecos
		/// que hay en una proyeccion, segun su tamao.
		/// </summary>
		/// <param name="holes">La lista que contiene los huecos.</param>
		/// <returns>Un array donde a[i] es el numero de huecos de tamao i.</returns>
		protected int [] CreateHolesHistogram(List<Hole> holes)
		{
			int maxTam=0;
			foreach(Hole h in holes)
			{
				if(h.Size>maxTam)
				{
					maxTam=h.Size;
				}
			}			
			
			int [] histoHoles =new int [maxTam+1];
			
			foreach(Hole h in holes)
			{
				histoHoles[h.Size]++;			
			}
			
			return histoHoles;
		}

		/// <summary>
		/// Este metodo establece el punto de corte a partir de la cual se eliminaran
		/// los huecos. Es el metodo que varia en las distintas implementaciones de esta
		/// clase abstracta.
		/// </summary>
		/// <param name="holes"></param>
		/// <returns></returns>
		protected abstract int GetImageCutThreshold(List<Hole> holes);			

		/// <summary>
		/// Metodo que obtiene las subimagenes a partir de los huecos.
		/// </summary>
		/// <param name="image">La imagen a segmentar.</param>
		/// <param name="holes">
		/// Los huecos considerados para separar las partes de la imagen.
		/// </param>
		/// <returns>Un array con las subimagenes obtenidas al segmentar.</returns>
		private List<MathTextBitmap> ImageCut(MathTextBitmap image,
		                                      List<Hole> holes)
		{
			
			List<MathTextBitmap> newBitmaps=new List<MathTextBitmap>();
			
			int start,size;
			int xpos=image.Position.X;
			int ypos=image.Position.Y;
			
			for(int i=0;i<holes.Count-1;i++)
			{
				//El texto esta entre el final de un hueco, y el inicio del siguiente;
				start=((Hole)holes[i]).EndPixel;
				size=((Hole)holes[i+1]).StartPixel-start+1;
				
				int x0,y0;
				int width0, height0;
				if(mode == ProjectionMode.Horizontal)
				{
					x0 = start;
					y0 = 0;
					width0 = size;
					height0 = image.Height;
				}
				else
				{
					x0 = 0;
					y0 = start;
					width0 = image.Width;
					height0 = size;
				}
				
				// Recortamos
				FloatBitmap cutImage= 
					image.FloatImage.SubImage(x0, y0, width0,height0);
				
				// Encuadramos
				Gdk.Point pd;
				Gdk.Size sd;				
				GetEdges(cutImage, out pd, out sd);
				cutImage =  cutImage.SubImage(pd.X, pd.Y, sd.Width,sd.Height);
				// Montamos el bitmap
				MathTextBitmap newBitmap =
					new MathTextBitmap(cutImage,
					                   new Point(xpos + x0 +  pd.X,
					                             ypos + y0+ pd.Y));
					
				newBitmaps.Add(newBitmap);
			}		
			
			if (newBitmaps.Count == 1)
			{
				newBitmaps.Clear();
			}
			
			
			return newBitmaps;
		}
	}

}
