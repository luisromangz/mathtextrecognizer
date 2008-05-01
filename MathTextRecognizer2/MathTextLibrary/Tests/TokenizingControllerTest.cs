// TokenizingControllerTest.cs created with MonoDevelop
// User: luis at 20:47 01/05/2008

using System;
using System.Collections.Generic;
using NUnit.Framework;


using MathTextLibrary.Bitmap;
using MathTextLibrary.Analisys.Lexical;
using MathTextLibrary.Controllers;

namespace MathTextLibrary.Tests
{
	
	
	[TestFixture()]
	public class TokenizingControllerTest
	{
		
		[Test()]
		public void ControlerTest()
		{
			TokenizingController controller = new TokenizingController();
			
			List<LexicalRule> rules = new List<LexicalRule>();
			
			LexicalRule rule = new LexicalRule();
			rule.Name = "NUMBER";
			rule.LexicalExpressions.Add(@"[0-9]+");
			rule.LexicalExpressions.Add(@"[0-9]+\.[0-9]+");
			
			rules.Add(rule);
			
			rule = new LexicalRule();
			rule.Name = "VAR";
			rule.LexicalExpressions.Add(@"[A-Za-z]+");
			
			rules.Add(rule);
			
			controller.SetLexicalRules(rules);
			
			List<Token> tokens = new List<Token>();
			
			int xd = 0;
			int size = 20;
			foreach (char c in "-") 
			{
				tokens.Add(new Token(c.ToString(),
				                     xd,
				                     0,
				                     new FloatBitmap(size,size)));
				
				xd +=size;
			}
			
			controller.Tokens = tokens;
			
			controller.Next(ControllerStepMode.UntilEnd);
			
			Assert.AreEqual(2, 
			                controller.Tokens.Count, 
			                "La lista de tokens no tiene el tamaño correcot");
			
			Assert.AreEqual("NUMBER", controller.Tokens[0].Type,
			                "El primer token no es correcto");
			Assert.AreEqual("200", controller.Tokens[0].Text,
			                "El texto del primer token no es correcto");
			
			
			Assert.AreEqual("VAR", controller.Tokens[1].Type,
			                "El segundo token no es correcto");
			Assert.AreEqual("x", controller.Tokens[1].Text,
			                "El texto del segundo token token no es correcto");
			
		}
	}
}
