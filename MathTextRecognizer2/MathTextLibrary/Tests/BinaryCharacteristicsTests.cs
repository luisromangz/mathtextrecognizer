// BinaryCharacteristicsTests.cs created with MonoDevelop
// User: luis at 16:21 18/04/2008

using System;
using NUnit.Framework;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary
{
	
	
	[TestFixture()]
	public class BinaryCharacteristicsTests
	{
		
		[Test()]
		public void CountBlackNeighboursHelperTest()
		{
			
			FloatBitmap bitmap = new FloatBitmap(5,5);
			bitmap[1,1] = FloatBitmap.Black;
			bitmap[1,2] = FloatBitmap.Black;
			bitmap[2,3] = FloatBitmap.Black;
			
			int res11 = CountBlackNeighboursHelper.BlackNeighbours(bitmap,1,1);
			int res12 = CountBlackNeighboursHelper.BlackNeighbours(bitmap,1,2);
			
			Assert.AreEqual(1,res11, "Pixel (1,1)");
			Assert.AreEqual(2,res12, "Pixel (1,2)");
		}
		
		
		[Test()]
		public void CountColorChangesHelperTest()
		{
			
			FloatBitmap bitmap = new FloatBitmap(5,5);			
			bitmap[1,2] = FloatBitmap.Black;
			bitmap[3,2] = FloatBitmap.Black;
			
			int resC1 = CountColorChangesHelper.NumColorChangesColumn(bitmap,1);
			int resC2 = CountColorChangesHelper.NumColorChangesColumn(bitmap,2);			
			
			int resR1 = CountColorChangesHelper.NumColorChangesRow(bitmap,1);
			int resR2 = CountColorChangesHelper.NumColorChangesRow(bitmap,2);
			
			Assert.AreEqual(2,resC1,"Columna 1");
			Assert.AreEqual(0,resC2,"Columna 2");			
			Assert.AreEqual(0,resR1, "Fila 1");
			Assert.AreEqual(4,resR2, "Fila 2");

		}
		
		[Test()]
		public void CountNumberOfBlackPixelsHelperTest()
		{
			
			FloatBitmap bitmap = new FloatBitmap(5,5);			
			bitmap[1,2] = FloatBitmap.Black;
			bitmap[3,2] = FloatBitmap.Black;
			
			int resC1 = CountNumberOfBlackPixelsHelper.NumBlackPixelsColumn(bitmap,1);
			int resC2 = CountNumberOfBlackPixelsHelper.NumBlackPixelsColumn(bitmap,2);			
			
			int resR1 = CountNumberOfBlackPixelsHelper.NumBlackPixelsRow(bitmap,1);
			int resR2 = CountNumberOfBlackPixelsHelper.NumBlackPixelsRow(bitmap,2);
			
			Assert.AreEqual(1,resC1,"Columna 1");
			Assert.AreEqual(0,resC2,"Columna 2");			
			Assert.AreEqual(0,resR1, "Fila 1");
			Assert.AreEqual(2,resR2, "Fila 2");
			

		}
		
		[Test()]
		public void CountNumberOfHolesHelperTest()
		{
			
			FloatBitmap bitmap = new FloatBitmap(5,5);			
			bitmap[1,2] = FloatBitmap.Black;
			bitmap[3,2] = FloatBitmap.Black;
			
			int resC1 = CountNumberOfBlackPixelsHelper.NumBlackPixelsColumn(bitmap,1);
			int resC2 = CountNumberOfBlackPixelsHelper.NumBlackPixelsColumn(bitmap,2);			
			
			int resR1 = CountNumberOfBlackPixelsHelper.NumBlackPixelsRow(bitmap,1);
			int resR2 = CountNumberOfBlackPixelsHelper.NumBlackPixelsRow(bitmap,2);
			
			Assert.AreEqual(1,resC1,"Columna 1");
			Assert.AreEqual(0,resC2,"Columna 2");			
			Assert.AreEqual(0,resR1, "Fila 1");
			Assert.AreEqual(2,resR2, "Fila 2");
			

		}
	}
}
