// created on 29/12/2005 at 13:51


using System;
using System.Collections.Generic;

using MathTextLibrary;
using MathTextLibrary.Projection;

namespace MathTextLibrary.BitmapSegmenters{

	/// <summary>
	/// Especializa la clase abstracta <code>ProjectionBitmapSegmenter</code>,
	/// no eliminando ningun hueco y por tanto obteniendo por separado todos las 
	/// partes separadas por huecos encontradas.
	/// </summary>
	public class AllHolesProjectionBitmapSegmenter: ProjectionBitmapSegmenter
	{
		
		public AllHolesProjectionBitmapSegmenter(ProjectionMode mode):base(mode)
		{
		
		}
			
			
		protected override int GetImageCutThreshold(List<Hole> holes)
		{
			return 0;		
		}
	
	}
}
