// ISyntacticMatcher.cs created with MonoDevelop
// User: luis at 14:34 12/05/2008

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using MathTextLibrary.Controllers;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This interface must be implemented by classes which performs 
	/// the task of syntactical matching.
	/// </summary>
	public abstract class SyntacticalMatcher
	{
		
		
		public static event MatchingHandler Matching;
		
		public static event MatchingFinishedHandler MatchingFinished;
		
		public static event MessageLogSentHandler LogSent;
	
		
#region Properties
		/// <value>
		/// Contains a label for the matcher's type.
		/// </value>
		public abstract string Type
		{
			get;
		}
		
		/// <value>
		/// Contains a label for the matcher.
		/// </value>
		public abstract string Label
		{
			get;
		}
		
		/// <value>
		/// Contains a list with the names of the rules used by this matcher
		/// or/and its children machers.
		/// </value>
		[XmlIgnore]
		public abstract List<string> RulesUsed
		{
			get;				
		}
			
		
#endregion Properties
		
#region Public methods
		
		public abstract bool Match(ref TokenSequence sequence, out string text);
		
#endregion Public methods
		
#region Non-public methods
		/// <summary>
		/// Launches the <see cref="Matching"/> event.
		/// </summary>
		protected void MatchingInvoker()
		{
			if(Matching!=null)
				Matching(this, new MatchingArgs(this));
		}
		
		/// <summary>
		/// Launches the <see cref="MatchingFinished"/> event.
		/// </summary>
		protected void MatchingFinishedInvoker(string output)
		{
			if(MatchingFinished!=null)
			{
				MatchingFinished(this, new MatchingFinishedArgs(output));
			}	
		}
		
		protected void LogSentInvoker(string format, params object [] args)
		{
			if(LogSent != null)
			{
				if(args.Length > 0)
					LogSent(this, new MessageLogSentArgs(String.Format(format, args)));
				else
					LogSent(this, new MessageLogSentArgs(format));
			}
		}
#endregion Non-public methods		
	
	}
}
