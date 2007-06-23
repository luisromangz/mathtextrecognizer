// created on 28/12/2005 at 13:26
using System;
using System.Collections.Generic;

using Gdk;

using MathTextLibrary;
using MathTextLibrary.Projection;

namespace MathTextLibrary.BitmapSegmenters{

	/// <summary>
	/// La clase abstracta <c>ProjectionBitmapSegmenter</c> representa la base
	/// de todos los segmentadores de imagenes que usan la proyeccion de la 
	/// misma para descomponerla en partes mas pequeas.
	/// </summary>
	public abstract class ProjectionBitmapSegmenter : IBitmapSegmenter
	{
		
		
		private ProjectionMode mode;
		
		/// <summary>
		/// Constructor de la clase <code>ProjectionBitmapSegmenter</code>.
		/// </summary>
		/// <param name="mode">El modo que se usara para obtener la proyeccion (horizontal,vertical).</param>
		public ProjectionBitmapSegmenter(ProjectionMode mode)
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
		public List<MathTextBitmap> Segment(MathTextBitmap image)
		{
		
			//Creamos la proyeccion
			BitmapProjection proj=BitmapProjection.CreateProjection(mode,image);
			List<Hole> holes=proj.Holes;
			//Buscamos el maximo hueco
			
			
			//Aqui decidimos a partir de que tamao de hueco vamos a cortar la imagen			
						
			int threshold=GetImageCutThreshold(holes);	
			
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
			
			MathTextBitmap newBitmap;
			
			int start,size;
			int xpos=image.Position.X;
			int ypos=image.Position.Y;
			
			for(int i=0;i<holes.Count-1;i++)
			{
				//El texto esta entre el final de un hueco, y el inicio del siguiente;
				start=((Hole)holes[i]).EndPixel;
				size=((Hole)holes[i+1]).StartPixel-start+1;
				
				int edge1,edge2;
				
				GetEdges(image,start,size,out edge1,out edge2);
				
				if(mode==ProjectionMode.Horizontal)
				{
					newBitmap=new MathTextBitmap(
						image.SubImage(start,edge1,size,edge2-edge1+1),
						new Point(start+xpos,ypos+edge1),
						mode);
				}
				else
				{
					newBitmap=new MathTextBitmap(
						image.SubImage(edge1,start,edge2-edge1+1,size),
						new Point(xpos+edge1,start+ypos),
						mode);
				}
				
				newBitmaps.Add(newBitmap);
			}		
			
			
			return newBitmaps;
		}

		/// <summary>
		/// Metodo auxiliar para obtner los bordes de cada subimagen para ajustarla en
		/// la direccion en la que no estamos segmentando.
		/// </summary>
		/// <param name="image">La imagen a ajustar.</param>
		/// <param name="start">El pixel de inicio de la zona segmentada a tratar.</param>
		/// <param name="size">El tamao en pixeles de la zona segmentada a tratar.</param>
		/// <param name="edge1">El borde mas cercano en el sentido que no segmentamos.</param>
		/// <param name="edge2">El borde mas lejano en el sentido que no segmentamos.</param>
		private void GetEdges(MathTextBitmap image,int start, int size,
			out int edge1,out int edge2){
			
			edge1=-1;
			edge2=-1;
			int i,j;
			if(mode==ProjectionMode.Horizontal){
				
				for(i=0;i<image.Height && edge1<0;i++){
					for	(j=0;j<size && edge1<0;j++){
						if(image.BinaryzedImage[j+start,i]==MathTextBitmap.Black){
							edge1=i-1;
						}
					}
				}					
				
				for(i=image.Height-1;i>=edge1 && edge2<0;i--){
					for	(j=0;j<size && edge2<0;j++){
						if(image.BinaryzedImage[j+start,i]==MathTextBitmap.Black){
							edge2=i+1;
						}
					}					
				}
				
			}else{
				for(i=0;i<image.Width && edge1<0;i++){
					for	(j=0;j<size && edge1<0;j++){
						if(image.BinaryzedImage[i,j+start]==MathTextBitmap.Black){
							edge1=i-1;
						}
					}
				}					
				
				for(i=image.Width-1;i>=edge1 && edge2<0;i--){
					for	(j=0;j<size && edge2<0;j++){
						if(image.BinaryzedImage[i,j+start]==MathTextBitmap.Black){
							edge2=i+1;
						}
					}					
				}
			}
		
		}
	}

}
