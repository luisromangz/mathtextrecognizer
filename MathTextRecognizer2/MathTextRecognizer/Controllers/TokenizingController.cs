// TokenizingController.cs created with MonoDevelop
// User: luis at 17:43 27/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

using Gtk;

using MathTextLibrary.Controllers;
using MathTextLibrary.Analisys.Lexical;

using MathTextRecognizer.Controllers.Nodes;

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
		
		public event SequenceAddedHandler SequenceAdded;
		
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
			
			List<SequenceNode> tokenSequences = GetTokenSequences();
			
			ProcessFinishedInvoker();
			
			foreach (SequenceNode sequence in tokenSequences) 
			{
				MatchTokens(sequence);
			}
			
			ProcessFinishedInvoker();
		}
		
		/// <summary>
		/// Processes the tokens given as an input to the controller.
		/// </summary>
		/// <returns>
		/// The list of sequences of contiguous tokens.
		/// </returns>
		private List<SequenceNode> GetTokenSequences()
		{
			List<SequenceNode> tokenSequences = new List<SequenceNode>();
			TokenSequence sequence = new TokenSequence();
			// We add a first sequence to the list.
			SequenceNode node = new SequenceNode(sequence);
			tokenSequences.Add(node);
			MessageLogSentInvoker("===== Secuencia añadida =====");
			SequenceAddedInvoker(node);
			
			NodeBeingProcessedInvoker();
			//SuspendByStep();
			/// We add the first token to the first sequence.
			Token lastToken = tokens[0];
			sequence.Append(lastToken);
			MessageLogSentInvoker("Símbolo «{0}» añadido a la secuencia", 
				                  lastToken.Text);
			tokens.RemoveAt(0);
			
			// All the tokens must be in one sequence.
			while(this.tokens.Count > 0)
			{	
				NodeBeingProcessedInvoker();
				//SuspendByStep();
				
				Token firstToken = tokens[0];
				if(!firstToken.CloseFollows(lastToken))
				{
					// If the symbols aren't contiguous, a new sequence has
					// commenced.
					sequence = new TokenSequence();	
					node = new SequenceNode(sequence);
					tokenSequences.Add(node);
					SequenceAddedInvoker(node);
					MessageLogSentInvoker("===== Secuencia añadida =====");
				}
				
				// We add the token to the current sequence, and remove it
				// from the inital token list.
				sequence.Append(firstToken);
				MessageLogSentInvoker("Símbolo «{0}» añadido a la secuencia", 
				                      firstToken.Text);
				
				
				
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
		/// The token sequence (possibly) containing tokens.
		/// </param>
		private void MatchTokens(SequenceNode node)
		{			
			TokenSequence discarded = new TokenSequence();
			SequenceNode discardedNode = new SequenceNode(discarded);
			TokenSequence sequence = node.Sequence;
			
			node.Select();
			NodeBeingProcessedInvoker();
			
			
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
					// at the beggining of the discarded set.
					int lastIndex = sequence.Count -1;
					if(node.ChildCount==0)
					{
						// If we haven't done so, we add the discarded sequence.
						node.AddSequenceChild(discardedNode);
					}
						
					discarded.Prepend(sequence[lastIndex]);
					
					sequence.RemoveAt(lastIndex);
				}
				else
				{
					// We found a token, so we stop searching and add
					// the token to the result.
					found = true;
					node.AppendToken(foundToken);
				}
			}
			
			if(found && discarded.Count > 0)
			{
				// We follow the recursive path.
				MatchTokens(discardedNode);
			}
			else
			{
				node.RemoveSequenceChildren();
			}
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
		
#region Event invokers
		
		private void SequenceAddedInvoker(SequenceNode sequence)
		{
			if(SequenceAdded !=null)
				SequenceAdded(this, new SequenceAddedArgs(sequence));
		}
		
#endregion Event invokers
	}
}
