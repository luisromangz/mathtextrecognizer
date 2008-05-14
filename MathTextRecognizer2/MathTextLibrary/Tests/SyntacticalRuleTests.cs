// SyntacticalRuleTests.cs created with MonoDevelop
// User: luis at 21:39 13/05/2008

using System;
using NUnit.Framework;

using MathTextLibrary.Analisys;

namespace MathTextLibrary.Tests
{
	
	
	[TestFixture()]
	public class SyntacticalRuleTests
	{
		
		[Test()]
		public void SimpeTestCase()
		{
			TokenSequence sequence= new TokenSequence();
			
			Token t = new Token("NUMBER");
			t.Text="200.9";
			sequence.Append(t);
			
			t = new Token("ADD");
			t.Text="+";
			sequence.Append(t);
			
			t = new Token("NUMBER");
			t.Text="14";
			sequence.Append(t);
			
			SyntacticalRule rule = new SyntacticalRule("formula");
			
			SyntacticalExpression exp = new SyntacticalExpression();
			exp.FormatString="{0}{1}";
			
			ExpressionTokenItem eti = new ExpressionTokenItem();
			eti.TokenType = "NUMBER";
			exp.Items.Add(eti);
			
			ExpressionGroupItem group = new ExpressionGroupItem();
			
			eti = new ExpressionTokenItem();
			eti.TokenType = "ADD";
			group.ChildrenItems.Add(eti);
			
			eti = new ExpressionTokenItem();
			eti.TokenType = "NUMBER";
			group.ChildrenItems.Add(eti);
			
			
			group.FormatString=" + {1}";
			exp.Items.Add(group);
			rule.Expressions.Add(exp);
			
			string output;
			bool res = rule.Match(sequence, out output);
			
			Assert.IsTrue(res, "Matching wasn't succesfull");
			Assert.AreEqual("200.9 + 14", output, "Output isn't correct.'");
		}
	}
}
