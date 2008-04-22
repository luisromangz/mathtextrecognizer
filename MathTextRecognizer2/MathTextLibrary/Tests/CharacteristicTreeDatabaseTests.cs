// CharacteristicDatabaseTests.cs created with MonoDevelop
// User: luis at 17:44 27/03/2008

using System;
using System.Collections.Generic;

using NUnit.Framework;

using MathTextLibrary.Symbol;
using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases;
using MathTextLibrary.Databases.Characteristic;

namespace MathTextLibrary.Tests
{
	
	
	[TestFixture()]
	public class CharacteristicTreeDatabaseTests
	{
		
		/// <summary>
		/// Probamos que añadir un simbolo con conteniddo igual a un nodo
		/// no lo añade realmente.
		/// </summary>
		[Test()]
		public void RepeatedSymbolNotAdded()
		{
			CharacteristicTreeDatabase database = new CharacteristicTreeDatabase();
			
			database.RootNode = new CharacteristicNode();
			
			
			database.RootNode.AddSymbol(new MathSymbol("hola"));
			
			int count1 = database.RootNode.Symbols.Count;			
			
			bool learnt = database.RootNode.AddSymbol(new MathSymbol("hola"));
			
			int count2 = database.RootNode.Symbols.Count;
			
			Assert.AreEqual(count1,count2,"Simbolo añadido");
			Assert.IsFalse(learnt,"Simbolo aprendido");
		}
		
		[Test()]
		public void LearningTest()
		{
			MathTextBitmap bitmap = 
				new MathTextBitmap(new FloatBitmap(5,5), new Gdk.Point(0,0));
			
			bitmap.ProcessImage(new List<BitmapProcesses.BitmapProcess>());
			
			CharacteristicTreeDatabase database = 
				new CharacteristicTreeDatabase();
			
			MathSymbol aSymbol = new MathSymbol("a");
			MathSymbol bSymbol = new MathSymbol("b");
			
			database.Learn(bitmap, aSymbol);
			
			List<MathSymbol> symbols = database.Match(bitmap);
			
			bool a1Learned = symbols.Count == 1 
				&& symbols[0] == aSymbol;
			
			bool a2Learned = true;
							
			a2Learned =	database.Learn(bitmap, aSymbol);
			
			
			database.Learn(bitmap, bSymbol);
			symbols = database.Match(bitmap);
			
			bool b1Learned = symbols.Count == 2;
			
			Assert.IsTrue(a1Learned, "Fallo el aprender la primera a");
			Assert.IsFalse(a2Learned, "No se detecto el conflicto de la segunda a");
			Assert.IsTrue(b1Learned, "Fallo el aprender la b");
		}
	}
}
