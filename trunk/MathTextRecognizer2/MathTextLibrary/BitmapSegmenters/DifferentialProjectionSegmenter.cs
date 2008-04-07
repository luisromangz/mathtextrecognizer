// created on 29/12/2005 at 14:08

using System;

using System.Collections.Generic;

using MathTextLibrary;
using MathTextLibrary.Projection;

namespace MathTextLibrary.BitmapSegmenters{

	/// <summary>
	/// Especializa la clase <c>ProjectionBitmapSegmenter</c>, eliminando los huecos
	/// de tamao menor al valor medio de la mayor diferencia entre dos tamaos de huecos consecutivos,
	/// de forma que algunas zonas de la imagen quedan agrupadas sin segmentar.
	/// </summary>
	public class DifferentialProjectionSegmenter: ProjectionSegmenter
	{
		
		public DifferentialProjectionSegmenter(ProjectionMode mode)
			:base(mode){}
			
			
		protected override int GetImageCutThreshold(List<Hole> holes)
		{
			int i;			
			int threshold=0;		
			
			int maxDifference=0;	
			int difference=0;		
			
			for(i=1;i<holes.Count-2;i++){
				difference =Math.Abs(((Hole)holes[i]).Size-((Hole)holes[i+1]).Size);				
				if(difference>maxDifference){
					maxDifference=difference;
					threshold=Math.Max(((Hole)holes[i]).Size,((Hole)holes[i+1]).Size)-(difference+1)/2;
				}
			}

			threshold-=difference/2;
			
			return threshold;	
		
		}	
	}
}
