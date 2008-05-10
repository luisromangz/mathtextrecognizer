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
	public class TokenizingController : ControllerBase
	{
		private List<Token> tokens;
		private List<LexicalRule> lexicalRules;
		
		private NodeView view;
		
		public event SequenceAddedHandler SequenceAdded;
		
		public event TokenCheckedHandler TokenChecked;
		
		public event EventHandler StepFailed; 
		
		
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
			
			Suspend();
			
			List<SequenceNode> tokenSequences = GetTokenSequences();
			
			ProcessFinishedInvoker();
			
			Suspend();
			
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
				
				NodeBeingProcessedInvoker();					
				
				if(tokenSequences.Count ==0)
					TokenCheckedInvoker(null, currentToken);
				
				foreach (SequenceNode storedNode in tokenSequences) 
				{					
					// We search the stored sequences so we may find one that
					// has the same baseline as the 
					storedNode.Select();
					
					this.MessageLogSentInvoker("Comprobando si «{0}» puede formar parte de la secuencia {1}",
					                           currentToken.Text,
					                           storedNode.NodeName);
					
					lastToken = storedNode.Sequence.Last;
					
					TokenCheckedInvoker(lastToken, currentToken);
					if(tokenSequences.Count>1)
						SuspendByStep();
					
					if(currentToken.CloseFollows(lastToken))
					{
						
						foundSequence = storedNode.Sequence;
						break;
					}
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
			NodeBeingProcessedInvoker();
			SuspendByNode();
			
			bool found = false;
			Token foundToken = null;		
			while(accepted.Count > 0 && !found)
			{
						
				foreach (LexicalRule rule in lexicalRules) 
				{
					foundToken = rule.Match(accepted);
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
					int lastIndex = accepted.Count -1;
					if(node.ChildCount==0)
					{
						// If we haven't done so, we add the discarded sequence.
						node.AddChildSequence(discardedNode);
					}
						
					discarded.Prepend(accepted[lastIndex]);
					
					accepted.RemoveAt(lastIndex);
				}
				else
				{
					// We found a token, so we stop searching and add
					// the token to the result.
					found = true;
					acceptedNode.FoundToken = foundToken;
				}
			}
			
			if(found && discarded.Count > 0)
			{
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
			}
		}

#endregion Non-public methods
		
#region Event invokers
		
		private void SequenceAddedInvoker(SequenceNode sequence)
		{
			if(SequenceAdded !=null)
				SequenceAdded(this, new SequenceAddedArgs(sequence));
		}
		
		private void TokenCheckedInvoker(Token lastToken, Token currentToken)
		{
			if(TokenChecked !=null)
			{
				TokenChecked(this, 
				             new TokenCheckedArgs(lastToken, currentToken));
			}
				
		}
		
		private void StepFailedInvoker()
		{
			if(StepFailed!=null)
			{
				StepFailed(this, EventArgs.Empty);
			}
		}
		
#endregion Event invokers
	}
}
