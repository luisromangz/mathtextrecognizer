// TokenizingController.cs created with MonoDevelop
// User: luis at 17:43 27/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Threading;
using System.Collections.Generic;

using Gtk;

using MathTextLibrary.Controllers;
using MathTextLibrary.Analisys;

using MathTextRecognizer.Controllers.Nodes;

namespace MathTextRecognizer.Controllers
{
	
	/// <summary>
	/// This class' instances serve as the controllores of the sintactical
	/// analisys.
	/// </summary>
	public class TokenizingController : ControllerBase
	{
		private List<Token> tokens;
		private List<LexicalRule> lexicalRules;
		
		private NodeView view;
		
		private SequenceNode sequenceForTokenizing;
		
		public event SequenceAddedHandler SequenceAdded;
		
		public event TokenCheckedHandler TokenChecked;
		
		public event EventHandler MatchingFailed; 
		
		public event SequenceBeingMatchedHandler SequenceBeingMatched;
		
		/// <summary>
		/// <c>TokenizingController</c>'s constructor.
		/// </summary>
		public TokenizingController() : base()
		{
			
		}
		
#region Properties
		
#endregion Properties
		
#region Public methods		
		
		/// <summary>
		/// Set the rules used for the lexical analisys.
		/// </summary>
		/// <param name="lexicalRules">
		/// The rules defined for the lexical analysis.
		/// </param>
		public void SetLexicalRules(List<LexicalRule> lexicalRules)
		{
			this.lexicalRules = lexicalRules;			
		}
		
		
		/// <summary>
		/// Sets the intial tokens to be processed.
		/// </summary>
		/// <param name="initialTokens">
		/// The inital tokens.
		/// </param>
		public void SetInitialData(List<Token> initialTokens, NodeView view)
		{
			tokens = initialTokens;
			this.view = view;
		}
		
		/// <summary>
		/// Sets a <see cref="SequenceNode"/> that will be analyzed lexically.
		/// </summary>
		/// <param name="node">
		/// A <see cref="SequenceNode"/>
		/// </param>
		public void SetSequenceForTokenizing(SequenceNode node)
		{
			sequenceForTokenizing = node;
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
			
			List<SequenceNode> tokenSequences;
			if(sequenceForTokenizing ==null)
			{
				Suspend();
			
				MessageLogSentInvoker("========== Secuenciación ==========");
				
				tokenSequences = GetTokenSequences();
				
				ProcessFinishedInvoker();
			}
			else
			{
				tokenSequences = new List<SequenceNode>();
				tokenSequences.Add(sequenceForTokenizing);
			}
			
			Suspend();
			
			MessageLogSentInvoker("========== Itemización de secuencias ==========");
			
			
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
		
			SequenceNode node = null;
			Token lastToken = null;
			Token currentToken = null;
			// All the tokens must be in one sequence.
			while(this.tokens.Count > 0)
			{	
				TokenSequence foundSequence = null;
				
				currentToken = tokens[0];
				
				NodeBeingProcessedInvoker(null);					
				
				if(tokenSequences.Count ==0)
					TokenCheckedInvoker(null, currentToken);
				
				foreach (SequenceNode storedNode in tokenSequences) 
				{					
					// We search the stored sequences so we may find one that
					// has the same baseline as the one being checked.
					storedNode.Select();
					
					this.MessageLogSentInvoker("Comprobando si «{0}» puede formar parte de la secuencia {1}",
					                           currentToken.Text,
					                           storedNode.NodeName);
					
					lastToken = storedNode.Sequence.Last;
					
					TokenCheckedInvoker(new TokenSequence(storedNode.Sequence),
					                    currentToken);
					if(tokenSequences.Count>1)
						SuspendByStep();
					
					if(currentToken.CloseFollows(lastToken))
					{
						foundSequence = storedNode.Sequence;
						break;
					}
					
					Thread.Sleep(150);
					
				}
				
				
				
				if(foundSequence == null)
				{
					// If the symbols aren't contiguous, a new sequence has
					// commenced.
					foundSequence = new TokenSequence();	
					node = new SequenceNode(foundSequence, view);
					tokenSequences.Add(node);
					node.NodeName = tokenSequences.Count.ToString();
					SequenceAddedInvoker(node);
					MessageLogSentInvoker("===== Secuencia {0} añadida =====", 
					                      node.NodeName);
				}
				
				
				
				// We add the token to the current sequence, and remove it
				// from the inital token list.
				foundSequence.Append(currentToken);
				MessageLogSentInvoker("Símbolo «{0}» añadido a la secuencia {1}", 
				                      currentToken.Text,
				                      node.NodeName);	
				
				StepDoneInvoker();				
				SuspendByNode();
				
				
				tokens.RemoveAt(0);
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
			
			TokenSequence accepted = new TokenSequence();
			SequenceNode acceptedNode = new SequenceNode(accepted, view);
			SequenceNode discardedNode = new SequenceNode(discarded, view);
			TokenSequence sequence = node.Sequence;
			
			node.AddChildSequence(acceptedNode);
			
			foreach (Token t in sequence) 
			{
				accepted.Append(t);
			}
			
			node.Select();
			NodeBeingProcessedInvoker(node);
			SuspendByStep();
			
			bool found = false;
			Token foundToken = null;		
			
			bool discardedNodeAdded = false;
			
			MessageLogSentInvoker("===== Tratando la secuencia {0} =====", 
			                      node.NodeName);
			
			while(accepted.Count > 0 && !found)
			{
				foreach (LexicalRule rule in lexicalRules) 
				{
					found = rule.Match(accepted, out foundToken);
					
					MessageLogSentInvoker("¿La regla «{0}» acepta la secuencia ({1})?: {2}", 
					                      rule.Name,
					                      accepted.ToString(),
					                      found?"Sí":"No");
					SequenceBeingMatchedInvoker(foundToken, rule, found);
					SuspendByStep();
					
					if(found)
					{
						// We search no more.
						break;
					}				
					Thread.Sleep(50);
				}
				
				// We check if a token was found
				if(!found)
				{
					// We remove the token from the input sequence and add it
					// at the beggining of the discarded set.
					int lastIndex = accepted.Count -1;
					if(!discardedNodeAdded)
					{
						// If we haven't done so, we add the discarded sequence.
						node.AddChildSequence(discardedNode);
						discardedNodeAdded = true;
					}
					
					this.MatchingFailedInvoker();
					
					MessageLogSentInvoker("Se elimina el último símbolo de la secuencia {0} para seguir probando.",
					                      accepted.ToString());
						
					discarded.Prepend(accepted[lastIndex]);
					
					accepted.RemoveAt(lastIndex);
					
					
				}
				else
				{
					// We found a token, so we stop searching and add
					// the token to the result.
					acceptedNode.FoundToken = foundToken;
					
					
				}
			}
			
		
			
			if(found && discarded.Count > 0)
			{
				MessageLogSentInvoker("Se tratará la secuencia de símbolos descartados.");
				// We follow the recursive path.
				MatchTokens(discardedNode);
			}
			else if(found && discarded.Count ==0)
			{
				// Only one token was found, we assimilate the acceptedNode
				// with its parent.
				node.FoundToken = foundToken;
				node.RemoveSequenceChildren();
			}
			else
			{
				// If nothing was found, we remove the children.
				node.RemoveSequenceChildren();
				
				MessageLogSentInvoker("No se pudo reconocer la secuencia {0}.",
					                   node.Sequence.ToString());
			}
			
			StepDoneInvoker();
			SuspendByNode();
		}

#endregion Non-public methods
		
#region Event invokers
		
		private void SequenceAddedInvoker(SequenceNode sequence)
		{
			if(SequenceAdded !=null)
				SequenceAdded(this, new SequenceAddedArgs(sequence));
		}
		
		private void TokenCheckedInvoker(TokenSequence lastSequence, Token currentToken)
		{
			if(TokenChecked !=null)
			{
				TokenChecked(this, 
				             new TokenCheckedArgs(lastSequence, currentToken));
			}
				
		}
		
		/// <summary>
		/// Launches the <see cref="SequenceBeingMatched"/> event.
		/// </summary>
		/// <param name="joinedToken">
		/// A <see cref="Token"/>
		/// </param>
		/// <param name="matcherRule">
		/// A <see cref="LexicalRule"/>
		/// </param>
		/// <param name="found">
		/// If the token is a valid token.
		/// </param>
		protected void SequenceBeingMatchedInvoker(Token joinedToken,
		                                         LexicalRule matcherRule,
		                                         bool found)
		{
			if(this.SequenceBeingMatched !=null)
			{
				SequenceBeingMatchedArgs args = 
					new SequenceBeingMatchedArgs(joinedToken, matcherRule, found);
				
				SequenceBeingMatched(this, args);
			}
		}
		
		/// <summary>
		/// Launches the MatchingFailed event.
		/// </summary>
		protected void MatchingFailedInvoker()
		{
			if(MatchingFailed!=null)
				MatchingFailed(this, EventArgs.Empty);
			
		}
		
#endregion Event invokers
	}
}
