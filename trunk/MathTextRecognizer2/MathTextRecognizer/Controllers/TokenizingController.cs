// TokenizingController.cs created with MonoDevelop
// User: luis at 17:43 27/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;


using MathTextLibrary.Analisys.Lexical;
using MathTextLibrary.Controllers;

namespace MathTextRecognizer.Controllers
{
	
	/// <summary>
	/// This class' instances serve as the controllores of the sintactical
	/// analisys.
	/// </summary>
	public class TokenizingController : BaseController
	{
		private List<Token> tokens;
		private List<LexicalRule> lexicalRules;
		
		/// <summary>
		/// <c>TokenizingController</c>'s constructor.
		/// </summary>
		public TokenizingController() : base()
		{
		}
		
#region Public methods		
		
		/// <summary>
		/// Sets the intial tokens to be processed.
		/// </summary>
		/// <param name="initialTokens">
		/// The inital tokens.
		/// </param>
		public void SetInitialTokens(List<Token> initialTokens)
		{
			tokens = initialTokens;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="lexicalRules">
		/// The rules defined for the lexical analysis.
		/// </param>
		public void SetLexicalRules(List<LexicalRule> lexicalRules)
		{
			this.lexicalRules = lexicalRules;			
		}
		
#endregion Public methods	
		
#region Non-public methods
		
		/// <summary>
		/// Groups the tokens in others containing more symbols.
		/// </summary>
		protected override void Process ()
		{
			MessageLogSentInvoker("========================================");
			MessageLogSentInvoker(" Comenzando proceso de análisis léxico");
			MessageLogSentInvoker("========================================");
			
			List<List<Token>> tokenSequences = GetTokenSequences();
			
			foreach (List<Token> tokenSequence in tokenSequences) 
			{
				List<Token> matchedTokens = MatchTokens(tokenSequence);
				tokens.AddRange(matchedTokens);
			}
			
			
		}
		
		/// <summary>
		/// Processes the tokens given as an input to the controller.
		/// </summary>
		/// <returns>
		/// The list of sequences of contiguous tokens.
		/// </returns>
		private List<List<Token>> GetTokenSequences()
		{
			List<List<Token>> tokenSequences = new List<List<Token>>();
			List<Token> sequence = new List<Token>();
			// We add a first sequence to the list.
			tokenSequences.Add(sequence);
			
			/// We add the first token to the first sequence.
			Token lastToken = tokens[0];
			sequence.Add(lastToken);
			tokens.RemoveAt(0);
			
			// All the tokens must be in one sequence.
			while(this.tokens.Count > 0)
			{
				Token firstToken = tokens[0];
				if(!firstToken.IsNextTo(lastToken))
				{
					// If the symbols aren't contiguous, a new sequence has
					// commenced.
					sequence = new List<Token>();
					tokenSequences.Add(sequence);
				}
				
				// We add the token to the current sequence, and remove it
				// from the inital token list.
				sequence.Add(firstToken);
				tokens.RemoveAt(0);
				
				// We update the token pointer.
				lastToken = firstToken;
			}
			
			return tokenSequences;
			
		}
		
		/// <summary>
		/// Matches a given token sequence into one or more Tokens.
		/// </summary>
		/// <param name="sequence">
		/// A <see cref="List`1"/>
		/// </param>
		/// <returns>
		/// The tokens the 
		/// </returns>
		private List<Token> MatchTokens(List<Token> sequence)
		{
			// TODO Complete the MatchTokens method stub.
			return null;
		}

		
#endregion Non-public methods
	}
}
