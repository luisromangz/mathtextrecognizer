// UtilsTests.cs created with MonoDevelop
// User: luis at 19:18 25/03/2008
//

using System;
using NUnit.Framework;

using MathTextLibrary.Utils;

namespace MathTextLibrary.Tests
{
	
	
	[TestFixture()]
	public class UtilsTests
	{
		
		[Test()]
		public void LuisLinuxConfigFilePathTest()
		{
			string path = ConfigFileUtils.GetConfigFilePath("MathTextLearner");
			
			Assert.AreEqual("/home/luis/.config/mathtextlearner",
			                path,
			                "La ruta del archivo no es la esperada");
		}
	}
}
