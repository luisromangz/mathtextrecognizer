// LexicalRuleTests.cs created with MonoDevelop
// User: luis at 20:56 28/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

using NUnit.Framework;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Analisys.Lexical;

namespace MathTextLibrary.Tests
{
	
	
	[TestFixture()]
	public class LexicalRuleTests
	{
		
		[Test()]
		public void MatchTextTest()			
		{
			List<Token> tokens = new List<Token>();
			
			string number= "290.839";
			foreach (char c in  number)
			{
				tokens.Add(new Token(c.ToString(), 0,0, new FloatBitmap(2,2)));
			}
			
			LexicalRule rule = new LexicalRule();
			rule.RuleName = "NUMBER";
			rule.LexicalExpressions.Add(@"[0-9]+(\.[0-9]+)?");
			
			Token token = rule.Match(tokens);
			
			Assert.IsNotNull(token, "El token es nulo");
			Assert.AreEqual("NUMBER", token.Type, "El tipo del token no es correcto");
			Assert.AreEqual(number, token.Text, "El texto del token no es correcto"); 
			
		}
	}
}
