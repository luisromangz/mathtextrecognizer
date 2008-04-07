// HorizontalProjectionSegmenter.cs created with MonoDevelop
// User: luis at 15:26 07/04/2008

using System;
using System.Collections.Generic;

using MathTextLibrary.Projection;

namespace MathTextLibrary.BitmapSegmenters
{
	
	/// <summary>
	/// Esta clase especializa <c>ProjcetionSegmenter</c> para que corte por 
	/// columnas en todos los huecos.
	/// </summary>
	public class ColumnsProjectionSegmenter : ProjectionSegmenter		
	{
		
		public ColumnsProjectionSegmenter() :
			base(ProjectionMode.Horizontal)
		{
			
		}
		
		protected override int GetImageCutThreshold (List<Hole> holes)
		{
			return 0;
		}

	}
}
