
using System;

using Gdk;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Projection
{
	/// <summary>
	/// This class specializes <see cref="BitmapProjection"/> and generates
	/// projections on the Y axis.
	/// </summary>
	public class VerticalBitmapProjection:BitmapProjection
	{
		
		internal VerticalBitmapProjection(MathTextBitmap image)
			: base(image)
		{
		
		}
	
		/// <summary>
		/// Creates the projection.
		/// </summary>
		/// <param name="image">
		/// The <see cref="MathTextBitmap"/> to be projected. 
		/// </param>
		protected override void CreateProjection(MathTextBitmap image)
		{			
			Values=new int [image.Height];
			for(int i=0;i<image.Height;i++)
			{
				for(int j=0;j<image.Width;j++)
				{
					if(image[j,i]!=FloatBitmap.White)
					{
						Values[i]++;	
					}
				}						
			}
		}
	}
}
