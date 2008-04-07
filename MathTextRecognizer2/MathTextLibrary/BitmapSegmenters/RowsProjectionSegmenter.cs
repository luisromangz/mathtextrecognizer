// VerticalProjectionSegmenter.cs created with MonoDevelop
// User: luis at 15:27 07/04/2008

using System;
using System.Collections.Generic;

using MathTextLibrary.Projection;

namespace MathTextLibrary.BitmapSegmenters
{
	
	
	/// <summary>
	/// Esta clase especializa <c>ProjcetionSegmenter</c> para que corte por 
	/// filas en todos los huecos.
	/// </summary>
	public class RowsProjectionSegmenter : ProjectionSegmenter		
	{
		
		public RowsProjectionSegmenter() :
			base(ProjectionMode.Vertical)
		{
			
		}
		
		protected override int GetImageCutThreshold (List<Hole> holes)
		{
			return 0;
		}

	}
}
