// ISyntacticMatcher.cs created with MonoDevelop
// User: luis at 14:34 12/05/2008

using System;
using System.Collections.Generic;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This interface must be implemented by classes which performs 
	/// the task of syntactical matching.
	/// </summary>
	public abstract class SyntacticalMatcher
	{
		public abstract bool Match(TokenSequence sequence, out string text);
		
		public abstract string Label
		{
			get;
		}
	}
}
