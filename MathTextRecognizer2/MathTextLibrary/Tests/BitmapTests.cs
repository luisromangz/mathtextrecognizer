// BitmapTests.cs created with MonoDevelop
// User: luis at 18:38 07/04/2008

using System;
using NUnit.Framework;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary
{
	
	
	[TestFixture()]
	public class BitmapTests
	{
		
		[Test()]
		public void FloatImageRotationTest()
		{
			FloatBitmap bitmap = new FloatBitmap(5, 4);
			
			bitmap[3,1] = FloatBitmap.White;
			
			FloatBitmap rotatedBitmap = bitmap.Rotate90();
			Assert.AreEqual("0, 0, 0, 0, 0, \n0, 1, 0, 0, 0, \n0, 0, 0, 0, 0, \n0, 0, 0, 0, 0, \n",
			                rotatedBitmap.ToString());
			
			
		}
	}
}
