// created on 28/12/2005 at 13:26
using System;
using System.Collections.Generic;

using Gdk;

using MathTextLibrary;
using MathTextLibrary.Bitmap;
using MathTextLibrary.Projection;

namespace MathTextLibrary.BitmapSegmenters{

	/// <summary>
	/// Esta clase implementa <code>IBitmapSegmenter</code>, de forma que segmentamos la imagen,
	/// por dos huecos de una proyeccion obtenida anteriormente unicamente.
	/// Se usa conjuntamente con el <code>FractionVerticalBitmapSegmenter</code>.
	/// </summary>
	public class TwoHolesBitmapSegmenter:IBitmapSegmenter
	{
		private int hole1, hole2;
		private ProjectionMode mode;
			
		/// <summary>
		/// Constructor de la clase <code>TwoHolesBitmapSegmenter</code>.
		/// </summary>
		/// <param name="mode">El modo de proyeccion con que se obtuvieron los huecos.</param>
		/// <param name="hole1">El primer hueco considerado para cortar.</param>
		/// <param name="hole2">El segundo hueco considerado para cortar.</param>
		public TwoHolesBitmapSegmenter(ProjectionMode mode,int hole1, int hole2)
		{
			this.mode=mode;
			this.hole1=hole1;
			this.hole2=hole2;
		}
		
		/// <summary>
		/// Este metodo se encarga de obtener las subimagenes que forman la imagen original.
		/// </summary>
		/// <param name="image">La imagen que queremos segmentar.</param>
		/// <returns>Un array con las tres partes de la imagen en que divide este segmentador.</returns>
		public List<MathTextBitmap> Segment(MathTextBitmap image)
		{		
			
	
			List<Hole> holes=new List<Hole>();
			holes.Add(new Hole(0,0));
			holes.Add(new Hole(hole1,hole1));
			holes.Add(new Hole(hole2,hole2));
			holes.Add(new Hole(image.Height-1,image.Height-1));

			return ImageCut(image,holes);
		}	
		
		/// <summary>
		/// Metodo auxiliar que realiza el corte de la imagen propiamente dicho.
		/// </summary>
		/// <param name="image">La imagen que se va a segmentar.</param>
		/// <param name="holes">Los huecos que se van a considerar para segmentar.</param>
		/// <returns>Un array con las subimagenes que forman la imagen de entrada.</returns>
		private List<MathTextBitmap> ImageCut(MathTextBitmap image,
		                                   List<Hole> holes){		
			
			List<MathTextBitmap> newBitmaps=new List<MathTextBitmap>();
			MathTextBitmap newBitmap;
			int start,size;
			int xpos=image.Position.X;
			int ypos=image.Position.Y;
			for(int i=0;i<holes.Count-1;i++){
				//El texto esta entre el final de un hole, y el inicio del siguiente;
				start = holes[i].EndPixel;
				size = holes[i+1].StartPixel-start+1;
				
				int edge1,edge2;
				
				GetEdges(image,start,size,out edge1,out edge2);
				
				if(mode == ProjectionMode.Horizontal)
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
		/// Obtiene los bordes del contenido de cada imagen segmentada en la direccion en que
		/// no se segmento.
		/// </summary>
		/// <param name="image">La imagen que segmentamos.</param>
		/// <param name="start">El punto de inicio de un zona que vamos a cortar.</param>
		/// <param name="size">El tamao de la zona que vamos a cortar.</param>
		/// <param name="edge1">El borde mas cercano en el sentido contrario al que segmentamos.</param>
		/// <param name="edge2">El borde mas lejano en el sentido contrario al que segmentamos.</param>
		private void GetEdges(MathTextBitmap image,int start, int size,
			out int edge1,out int edge2){
			
			edge1=-1;
			edge2=-1;
			int i,j;
			if(mode==ProjectionMode.Horizontal){
				
				for(i=0;i<image.Height && edge1<0;i++){
					for	(j=0;j<size && edge1<0;j++){
						if(image.BinaryzedImage[j+start,i]!=MathTextBitmap.White){
							edge1=i-1;
						}
					}
				}					
				
				for(i=image.Height-1;i>=edge1 && edge2<0;i--){
					for	(j=0;j<size && edge2<0;j++){
						if(image.BinaryzedImage[j+start,i]!=MathTextBitmap.White){
							edge2=i+1;
						}
					}					
				}
				
			}else{
				for(i=0;i<image.Width && edge1<0;i++){
					for	(j=0;j<size && edge1<0;j++){
						if(image.BinaryzedImage[i,j+start]!=MathTextBitmap.White){
							edge1=i-1;
						}
					}
				}					
				
				for(i=image.Width-1;i>=edge1 && edge2<0;i--){
					for	(j=0;j<size && edge2<0;j++){
						if(image.BinaryzedImage[i,j+start]!=MathTextBitmap.White){
							edge2=i+1;
						}
					}					
				}
			}
		
		}
	}

}
