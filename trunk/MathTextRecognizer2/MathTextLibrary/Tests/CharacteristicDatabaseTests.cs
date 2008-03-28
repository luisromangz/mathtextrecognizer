// CharacteristicDatabaseTests.cs created with MonoDevelop
// User: luis at 17:44 27/03/2008

using System;
using NUnit.Framework;

using MathTextLibrary.Symbol;
using MathTextLibrary.Databases.Caracteristic;

namespace MathTextLibrary
{
	
	
	[TestFixture()]
	public class CharacteristicDatabaseTests
	{
		
		/// <summary>
		/// Probamos que añadir un simbolo con conteniddo igual a un nodo
		/// no lo añade realmente.
		/// </summary>
		[Test()]
		public void RepeatedSymbolNotAdded()
		{
			CaracteristicDatabase database = new CaracteristicDatabase();
			
			database.CaracteristicNode = new BinaryCaracteristicNode();
			
			database.CaracteristicNode.AddSymbol(new MathSymbol("hola",MathSymbolType.Operator));
			int count1 = database.CaracteristicNode.Symbols.Count;
			
			database.CaracteristicNode.AddSymbol(new MathSymbol("hola",MathSymbolType.Number));
			int count2 = database.CaracteristicNode.Symbols.Count;
			
			Assert.AreEqual(count1, count2,"Se añadio el simbolo repetido");
		}
	}
}
