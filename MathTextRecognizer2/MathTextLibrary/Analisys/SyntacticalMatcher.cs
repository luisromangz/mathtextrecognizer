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
		
		/// <value>
		/// This event is launched when the matcher starts matching tokens.
		/// </value>
		public event EventHandler Matching;
		
		public abstract string Label
		{
			get;
		}
		
		/// <summary>
		/// Launches the <see cref="Matching"/> event.
		/// </summary>
		protected void MatchingInvoker()
		{
			if(Matching!=null)
				Matching(this, EventArgs.Empty);
		}
	}
}
