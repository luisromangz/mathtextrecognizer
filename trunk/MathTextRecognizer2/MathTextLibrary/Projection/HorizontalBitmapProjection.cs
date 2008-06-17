
using System;

using Gdk;

using MathTextLibrary;
using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Projection
{
	
	/// <summary>
	/// This class specializes <see cref="BitmapProjecto"/> so it creates
	/// projectons on the X-axis.
	/// </summary>
	public class HorizontalBitmapProjection : BitmapProjection
	{

		internal HorizontalBitmapProjection(MathTextBitmap image)
			: base(image)
		{
		
		}		
		
		/// <summary>
		/// Creates the projection
		/// </summary>
		/// <param name="image">
		/// The <see cref="MathTextBitmap"/> to be projected.
		/// </param>
		protected override void CreateProjection(MathTextBitmap image)
		{	
			Values=new int [image.Width];
			for(int i=0;i<image.Width;i++)
			{
				for(int j=0;j<image.Height;j++)
				{
					if(image[i,j]!=FloatBitmap.White)
					{
						Values[i]++;	
					}
				}						
			}
		}
	
	}
}
