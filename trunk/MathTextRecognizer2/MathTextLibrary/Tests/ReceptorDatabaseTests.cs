// ReceptorDatabaseTests.cs created with MonoDevelop
// User: luis at 19:38 08/05/2008

using System;
using NUnit.Framework;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Receptors;

namespace MathTextLibrary.Tests
{
	
	
	[TestFixture()]
	public class ReceptorDatabaseTests
	{
		
		[Test()]
		public void ReceptorSimpleTest()
		{
			Receptor receptor = new Receptor(0.5f, 0.5f, 0.99f, 0.99f);
			
			bool state = receptor.GetReceptorState(26,26,50,50);
			
			Assert.IsTrue(state, "The receptor doesn't detected the point");
		}
	}
}
