// FormulaMatchingController.cs created with MonoDevelop
// User: luis at 12:57 09/05/2008

using System;
using System.Collections.Generic;

using System.Threading;

using Gtk;

using MathTextLibrary.Analisys;
using MathTextLibrary.Controllers;

using MathTextRecognizer.Controllers.Nodes;

namespace MathTextRecognizer.Controllers
{
	
	/// <summary>
	/// This class implements the controller for the formula construction 
	/// process.
	/// </summary>
	public class ParsingController : ControllerBase
	{
		private List<Token> startTokens;
		
		public event MatchingFinishedHandler MatchingFinished;
		public event MatchingHandler Matching;
		public event TokenMatchingHandler TokenMatching;
		public event TokenMatchingFinishedHandler TokenMatchingFinished;
		public event SequenceSetHandler RuleSequenceRestored;
		public event SequenceSetHandler RelatedSequenceSet;
		
		private string output;
		private bool parsingResult;
		
		/// <summary>
		/// <c>FormulaMatchingController</c>'s contructor.
		/// </summary>
		public ParsingController() : base()
		{
			SyntacticalMatcher.Matching += 
				new MatchingHandler(OnMatcherMatching);
			
			SyntacticalMatcher.MatchingFinished += 
				new MatchingFinishedHandler(OnMatcherMatchingFinished);
			
			SyntacticalRule.SequenceRestored += 
				new SequenceSetHandler(OnSyntacticalRuleSequenceRestored);
			
			ExpressionTokenItem.TokenMatching += 
				new TokenMatchingHandler(OnTokenItemMatching);
			
			ExpressionTokenItem.TokenMatchingFinished +=
				new TokenMatchingFinishedHandler(OnTokenItemMatchingFinished);
			
			ExpressionTokenItem.RelatedSequenceSet+=
				new SequenceSetHandler(OnTokenItemRelatedSequenceSet);
			
			SyntacticalMatcher.LogSent+= 
				new MessageLogSentHandler(OnMatcherLogSent);
		}
		
#region Properties
		/// <value>
		/// Contains the output of the rules.
		/// </value>
		public string Output
		{
			get 
			{
				return output;
			}
		}

		/// <value>
		/// Contains a value that indicates if the syntactical analisys
		/// was successful.
		/// </value>
		public bool ParsingResult
		{
			get 
			{
				return parsingResult;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Sets the tokens used as data for the process.
		/// </summary>
		/// <param name="startTokens">
		/// A <see cref="List'1"/> containing the <see cref="Token"/> 
		/// instances generated by the lexical analisys stage.
		/// </param>
		public void SetStartTokens(List<Token> startTokens)
		{
			this.startTokens = startTokens;
		}
		
		/// <summary>
		/// Call point to start the processing.
		/// </summary>
		protected override void Process ()
		{
			MessageLogSentInvoker("===========================================");
			MessageLogSentInvoker(" Comenzando proceso de análisis sintáctico");
			MessageLogSentInvoker("===========================================");
			
			SyntacticalRule startRule = 
				SyntacticalRulesLibrary.Instance.StartRule;
		
					
			OnMatcherMatching(this,
			                  new MatchingArgs(SyntacticalRulesLibrary.Instance.StartRule));
			
			SuspendByStep();
			
			TokenSequence inputTokens = new TokenSequence(startTokens);
			parsingResult = startRule.Match(ref inputTokens, out output);
			
			if(inputTokens.Count > 0)
			{
				// Not all tokens were matched, so the parsing process
				// was unsuccessfull.
				parsingResult = false;
			}
			
			ProcessFinishedInvoker();
		}
		
		/// <summary>
		/// Deregisters the static event handlers, so they don't get
		/// handled several times.
		/// </summary>
		public void DeregisterEvents()
		{
			SyntacticalMatcher.Matching -= 
				new MatchingHandler(OnMatcherMatching);
			
			SyntacticalMatcher.MatchingFinished -= 
				new MatchingFinishedHandler(OnMatcherMatchingFinished);
			
			SyntacticalRule.SequenceRestored -= 
				new SequenceSetHandler(OnSyntacticalRuleSequenceRestored);
			
			ExpressionTokenItem.TokenMatching -= 
				new TokenMatchingHandler(OnTokenItemMatching);
			
			ExpressionTokenItem.TokenMatchingFinished -=
				new TokenMatchingFinishedHandler(OnTokenItemMatchingFinished);
			
			ExpressionTokenItem.RelatedSequenceSet -=
				new SequenceSetHandler(OnTokenItemRelatedSequenceSet);
			
			SyntacticalMatcher.LogSent -= 
				new MessageLogSentHandler(OnMatcherLogSent);
		}

		
#endregion Public methods
		
#region Private methods
		
	
		private void OnMatcherMatching(object sender, MatchingArgs args)
		{
			if(this.Matching!=null)
			{
				Matching(this, args);
			}
			
			Thread.Sleep(50);
			SuspendByStep();
		}
		
		private void OnMatcherLogSent(object sender, MessageLogSentArgs args)
		{
			this.MessageLogSentInvoker(args.Message);
		}
		
		private void OnMatcherMatchingFinished(object sender,
		                                       MatchingFinishedArgs args)
		{
			if(MatchingFinished !=null)
			{
				MatchingFinished(this, args);
			}
			SuspendByNode();	
			
			StepDoneInvoker();
			SuspendByStep();
		}
		
		private void OnTokenItemMatching(object sender, TokenMatchingArgs args)
		{
			if(TokenMatching !=null)
			{
				TokenMatching(this, args);
			}
			SuspendByStep();
		}
		
		private void OnTokenItemMatchingFinished(object sender, TokenMatchingFinishedArgs args)
		{
			
			if(TokenMatchingFinished!=null)
			{
				TokenMatchingFinished(this, args);
			}
			SuspendByStep();
		
		}
		private void OnTokenItemRelatedSequenceSet(object sender, SequenceSetArgs args)
		{
			if(RelatedSequenceSet != null)
			{
				RelatedSequenceSet(this, args);
			}
			SuspendByNode();
		}
		
		
		private void OnSyntacticalRuleSequenceRestored(object sender, 
		                                               SequenceSetArgs args)
		{
			if(this.RuleSequenceRestored!=null)
			{
				RuleSequenceRestored(this, args);
			}
		}
		
				
		
#endregion Private methods
		
	}
}
