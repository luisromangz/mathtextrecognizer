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
			LexicalRule rule = new LexicalRule();
			rule.Name = "NUMBER";
			rule.LexicalExpressions.Add(@"[0-9]+");
			rule.LexicalExpressions.Add(@"[0-9]+\.[0-9]+");
			
			// Should accept
			string number= "290";
			
			Token token = TestToken(number, rule);
			
			Assert.IsNotNull(token, "El token es nulo para {0}",number);
			Assert.AreEqual("NUMBER", token.Type, 
			                "El tipo del token no es correcto para {0}",number);
			Assert.AreEqual(number, token.Text, 
			                "El texto del token no es correcto para {0}",number); 
			
			// Should accept
			number= "290.23";
			
			token = TestToken(number, rule);
			
			Assert.IsNotNull(token, "El token es nulo para {0}",number);
			Assert.AreEqual("NUMBER", token.Type, 
			                "El tipo del token no es correcto para {0}",number);
			Assert.AreEqual(number, token.Text, 
			                "El texto del token no es correcto para {0}",number); 
			
			// Should fail
			number= "2fdsf90.23";
			
			token = TestToken(number, rule);
			
			Assert.IsNull(token, "El token no es nulo para {0}",number);
			
		}
		
		private Token TestToken(string text, LexicalRule rule)
		{
			TokenSequence tokens = new TokenSequence();
			foreach (char c in  text)
			{
				tokens.Append(new Token(c.ToString(), 0,0, new FloatBitmap(2,2)));
			}
			
			Token res;
			if(!rule.Match(tokens, out res))
				return null;
			
			return res;
		}
	}
}
