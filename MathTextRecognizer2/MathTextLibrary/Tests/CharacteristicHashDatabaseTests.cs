// CharacteristicHashDatabaseTests.cs created with MonoDevelop
// User: luis at 17:18 19/04/2008

using System;
using System.Collections.Generic;

using NUnit.Framework;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;
using MathTextLibrary.Databases;
using MathTextLibrary.Databases.Characteristic;

namespace MathTextLibrary.Tests
{
	
	
	[TestFixture()]
	public class CharacteristicHashDatabaseTests
	{
		
		[Test()]
		public void LearningTest()
		{
			MathTextBitmap bitmap = 
				new MathTextBitmap(new FloatBitmap(5,5), new Gdk.Point(0,0));
			
			bitmap.ProcessImage(new List<BitmapProcesses.BitmapProcess>());
			
			CharacteristicHashDatabase database = 
				new CharacteristicHashDatabase();
			
			MathSymbol aSymbol = new MathSymbol("a");
			MathSymbol bSymbol = new MathSymbol("b");
			
			database.Learn(bitmap, aSymbol);
			
			List<MathSymbol> symbols = database.Match(bitmap);
			
			bool a1Learned = symbols.Count == 1 
				&& symbols[0] == aSymbol;
			
			Assert.IsTrue(a1Learned, "Fallo el aprender la primera a");
			
			bool a2Learned = database.Learn(bitmap, aSymbol);
			Assert.IsFalse(a2Learned, "No se detecto el conflicto de la segunda a");
			
			database.Learn(bitmap, bSymbol);
			symbols = database.Match(bitmap);
			
			bool b1Learned = symbols.Count == 2;
			
			
			
			Assert.IsTrue(b1Learned, "Fallo el aprender la b");
		}
	}
}
