// TokenizingController.cs created with MonoDevelop
// User: luis at 17:43 27/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;


using MathTextLibrary.Analisys.Lexical;

namespace MathTextLibrary.Controllers
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
		
#region Properties
		/// <value>
		/// Contains the tokens processed by the controller.
		/// </value>
		public List<Token> Tokens
		{
			get
			{
				return tokens;
			}
			set
			{
				tokens = value;
			}
		}
#endregion Properties
		
#region Public methods		
		
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
			List<Token> result = new List<Token>();
			
			List<Token> discarded = new List<Token>();
			
			bool found = false;
			while(sequence.Count > 0 && !found)
			{
				Token foundToken = null;				
				foreach (LexicalRule rule in lexicalRules) 
				{
					foundToken = rule.Match(sequence);
					if(foundToken!=null)
					{
						// We search no more.
						break;
					}
				}
				
				// We check if a token was found
				if(foundToken == null)
				{
					// We remove the token from the input sequence and add it
					// to the discarded set.
					int lastIndex = sequence.Count -1;
					discarded.Add(sequence[lastIndex]);
					sequence.RemoveAt(lastIndex);
				}
				else
				{
					// We found a token, so we stop searching and add
					// the token to the result.
					found = true;
					result.Add(foundToken);
				}
			}
			
			if(found && discarded.Count > 0)
			{
				// We try to match the discarded tokens.
				// It has to be reversed because its elements were
				// added from right to left.
				discarded.Reverse(); 
				result.AddRange(MatchTokens(discarded));
			}
			else if(!found)
			{
				// Houston, we have a lexical input problem.
				throw new Exception("No rules were able to match the input tokens.");
			}
			
			return result;
		}

		/// <summary>
		/// Sets the intial tokens to be processed.
		/// </summary>
		/// <param name="initialTokens">
		/// The inital tokens.
		/// </param>
		private void SetInitialTokens(List<Token> initialTokens)
		{
			tokens = initialTokens;
		}
		
#endregion Non-public methods
	}
}
