using System;

using MathTextLibrary.Projection;
using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.BitmapSegmenters
{
	/// <summary>
	/// Un segmentador de imagenes diseado especificamente para tratar el caso
	/// en que estemos tratando una fraccion.
	/// </summary>
	public class FractionVerticalBitmapSegmenter:IBitmapSegmenter
	{
		/// <summary>
		/// Constructor de la clase <c>FractionVerticalBitmapSegmenter</c>.
		/// </summary>
		public FractionVerticalBitmapSegmenter(){}
		
		/// <summary>
		/// Divide una imagen considerada fraccion en tres zonas,
		/// una para el numerador, la raya de fraccion y el denominador.
		/// </summary>
		/// <param name="mtb">La imagen a segmentar.</param>
		/// <returns>Un array con las subimageners en que se divede la original.</returns>
		public MathTextBitmap [] Segment(MathTextBitmap mtb)
		{
			IBitmapSegmenter segmenter=new AllHolesProjectionBitmapSegmenter(ProjectionMode.Vertical);
			MathTextBitmap[] segments=segmenter.Segment(mtb);
			
			if(segments.Length>=3)
			{
				int x1,y1,x2,y2;
				ImageBoxerHelper.BoxImage(mtb.BinaryzedImage,out x1,out y1,out x2,out y2);
				int width=x2-x1+1;
				
				// buscamos la raya de fraccion
				MathTextBitmap fraction = null;
				foreach(MathTextBitmap m in segments)
				{
					if(m.Width>=width-2)
						fraction=m;
				}
				
				if(fraction != null)
				{
		            // obtenemos los huecos encima y abajo de la fraccion para segmentar
					int huecoSuperior=fraction.Position.Y-mtb.Position.Y-1;
					int huecoInferior=huecoSuperior+fraction.Height+1;

		            segmenter=new TwoHolesBitmapSegmenter(ProjectionMode.Vertical,huecoSuperior,huecoInferior);
					segments=segmenter.Segment(mtb);
				}
				else
				{
					segments=new MathTextBitmap[1];
				}
			}
			
			return segments;
		}
	}
}
