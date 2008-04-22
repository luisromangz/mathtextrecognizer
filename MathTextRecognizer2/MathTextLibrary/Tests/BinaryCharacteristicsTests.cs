// BinaryCharacteristicsTests.cs created with MonoDevelop
// User: luis at 16:21 18/04/2008

using System;
using NUnit.Framework;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Tests
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
		public void CountPixelsHelperHalfsTest()
		{
			
			FloatBitmap bitmap = new FloatBitmap(5,5);	
			bitmap[1,0] = FloatBitmap.Black;
			bitmap[1,1] = FloatBitmap.Black;
			bitmap[3,2] = FloatBitmap.Black;
			
			int ct1 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Top);
			int cb1 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Bottom);
			int cl1 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Left);
			int cr1 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Right);
			
			Assert.AreEqual(2, ct1,"Arriba 1");
			Assert.AreEqual(1, cb1,"Abajo 1");
			Assert.AreEqual(2, cl1,"Izquierda 1");
			Assert.AreEqual(1, cr1,"Derecha 1");
			
			bitmap = bitmap.Rotate90();
			
			int ct2 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Top);
			int cb2 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Bottom);
			int cl2 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Left);
			int cr2 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Right);
			
			Assert.AreEqual(1, ct2,"Arriba 2");
			Assert.AreEqual(2, cb2,"Abajo 2");
			Assert.AreEqual(2, cl2,"Izquierda 2 ");
			Assert.AreEqual(1, cr2,"Derecha 2");
			
			bitmap = bitmap.Rotate90();
			
			int ct3 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Top);
			int cb3 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Bottom);
			int cl3 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Left);
			int cr3 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Right);
			
			Assert.AreEqual(0, ct3,"Arriba 3");
			Assert.AreEqual(3, cb3,"Abajo 3");
			Assert.AreEqual(1, cl3,"Izquierda 3");
			Assert.AreEqual(2, cr3,"Derecha 3");
			
			bitmap = bitmap.Rotate90();
			
			int ct4 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Top);
			int cb4 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Bottom);
			int cl4 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Left);
			int cr4 = CountPixelsHelper.NumBlackPixelsInHalf(bitmap, Half.Right);
			
			Assert.AreEqual(2, ct4,"Arriba 4");
			Assert.AreEqual(1, cb4,"Abajo 4");
			Assert.AreEqual(0, cl4,"Izquierda 4");
			Assert.AreEqual(3, cr4,"Derecha 4");
		}
		
		[Test()]
		public void CountPixelsHelperQuartersTest()
		{
			
			FloatBitmap bitmap = new FloatBitmap(5,5);	
			bitmap[1,0] = FloatBitmap.Black;
			bitmap[1,1] = FloatBitmap.Black;
			bitmap[3,2] = FloatBitmap.Black;
			
			int ct1 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.NW);
			int cb1 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.SW);
			int cl1 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.SE);
			int cr1 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.NE);
			
			Assert.AreEqual(2, ct1,"NW 1");
			Assert.AreEqual(0, cb1,"SW 1");
			Assert.AreEqual(1, cl1,"SE 1");
			Assert.AreEqual(0, cr1,"NE 1");
			
			bitmap = bitmap.Rotate90();
			
			int ct2 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.NW);
			int cb2 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.SW);
			int cl2 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.SE);
			int cr2 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.NE);
			
			Assert.AreEqual(0, ct2,"NW 2");
			Assert.AreEqual(2, cb2,"SW 2");
			Assert.AreEqual(0, cl2,"SE 2");
			Assert.AreEqual(1, cr2,"NE 2");
			
			bitmap = bitmap.Rotate90();
			
			int ct3 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.NW);
			int cb3 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.SW);
			int cl3 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.SE);
			int cr3 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.NE);
			
			Assert.AreEqual(0, ct3,"NW 3");
			Assert.AreEqual(1, cb3,"SW 3");
			Assert.AreEqual(2, cl3,"SE 3");
			Assert.AreEqual(0, cr3,"NE 3");
			
			bitmap = bitmap.Rotate90();
			
			int ct4 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.NW);
			int cb4 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.SW);
			int cl4 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.SE);
			int cr4 = 
				CountPixelsHelper.NumBlackPixelsInQuadrant(bitmap, Quadrant.NE);
			
			Assert.AreEqual(0, ct4,"NW 4");
			Assert.AreEqual(0, cb4,"SW 4");
			Assert.AreEqual(1, cl4,"SE 4");
			Assert.AreEqual(2, cr4,"NE 4");
		}
	}
}
