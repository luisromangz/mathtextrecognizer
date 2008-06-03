// RecognizingControllerEvents.cs created with MonoDevelop
// User: luis at 23:07 06/05/2008

using System;

using MathTextLibrary.Analisys;
using MathTextLibrary.Bitmap;

using MathTextRecognizer.Controllers.Nodes;

namespace MathTextRecognizer.Controllers
{
	
	
	/// <summary>
	/// Delegado para los manejadores de los eventos enviados por los controladores 
	/// cuando desean notificar que han comenzado a procesar una nueva imagen.
	/// </summary>
	public delegate void BitmapProcessedHandler(object sender,
	                                            BitmapProcessedArgs arg);
	
	/// <summary>
	/// Esta clase encapsula los argumentos enviados en los eventos manejados por
	/// BitmapBeingRecognizedHandler.
	/// </summary>
	public class BitmapProcessedArgs : EventArgs
	{
		private MathTextBitmap b;
		
		/// <summary>
		/// Constructor de la clase.
		/// </summary>
		/// <param name="bitmap">La imagen que se ha comenzado a reconocer.</param>
		public BitmapProcessedArgs(MathTextBitmap bitmap)
			:base()
	    {
			b=bitmap;
		}
		
		/// <value>
		/// Contiene la imagen que pasamos como argumento del evento.
		/// </value>
		public MathTextBitmap MathTextBitmap
		{
			get
			{
				return b;
			}
		}
	}
	

	
	
	/// <summary>
	/// Delegate for the sequence added event of <c>TokenizingController</c>.
	/// </summary>
	/// <param name="sender">
	/// A <see cref="System.Object"/>
	/// </param>
	/// <param name="args">
	/// A <see cref="SequenceAddedArgs"/>
	/// </param>
	public delegate void SequenceAddedHandler(object sender, 
	                                          SequenceAddedArgs args);
	
	/// <summary>
	/// Wrapper class for the sequence added event parameters.
	/// </summary>
	public class SequenceAddedArgs : EventArgs
	{
		private SequenceNode sequence;
		public SequenceAddedArgs(SequenceNode sequence)
		{
			this.sequence = sequence;
		}
		
		/// <value>
		/// Contains the sequence which was added.
		/// </value>
		public SequenceNode Sequence
		{
			get
			{
				return sequence;
			}
		}
	}
	
	public delegate void TokenCheckedHandler(object sender, 
	                                         TokenCheckedArgs args);
	
	/// <summary>
	/// Auxiliary class to be used as argument of the <c>TokenChecked</c> event.
	/// </summary>
	public class TokenCheckedArgs : EventArgs
	{
		private Token currentToken;
		private TokenSequence lastSequence;
		
		public TokenCheckedArgs (TokenSequence lastSequence, Token currentToken)
			: base ()
		{
			this.lastSequence = lastSequence;
			this.currentToken= currentToken;
		}
		
		/// <value>
		/// Contains the last token in the sequence the current token is being
		/// checked with.
		/// </value>
		public TokenSequence LastSequence 
		{
			get
			{
				return lastSequence;
			}
		}

		/// <value>
		/// Contains the current event being processed.
		/// </value>
		public Token CurrentToken 
		{
			get {
				return currentToken;
			}
		}
	}
	
	/// <summary>
	/// Delegate for the SequenceBeingMatched event.
	/// </summary>
	/// <param name="sender">
	/// A <see cref="System.Object"/>
	/// </param>
	/// <param name="a">
	/// A <see cref="SequenceBeingMatchedArgs"/>
	/// </param>
	public delegate void SequenceBeingMatchedHandler(object sender,
	                                                 SequenceBeingMatchedArgs a);
	
	
	/// <summary>
	/// Auxiliary class used to pass the arguments of
	/// <see cref="SequenceBeingMatchedHandler"/> type.
	/// </summary>
	public class SequenceBeingMatchedArgs : EventArgs
	{
		private Token joinedToken;
		private LexicalRule matchingRule;
		private bool found;
		
		/// <summary>
		/// <see cref="SequenceBeingMatcchedArgs"/>'s contructor.
		/// </summary>
		/// <param name="joinedToken">
		/// A <see cref="Token"/>
		/// </param>
		/// <param name="matchingRule">
		/// A <see cref="LexicalRule"/>
		/// </param>
		/// <param name="found">
		/// A <see cref="System.Boolean"/>
		/// </param>
		public SequenceBeingMatchedArgs(Token joinedToken, 
		                                LexicalRule matchingRule,
		                                bool found)
		{
			this.joinedToken = joinedToken;
			this.matchingRule = matchingRule;
			this.found = found;
		}
		
		/// <value>
		/// The resulting token of joining the matched sequence's elements.
		/// </value>
		public Token JoinedToken
		{
			get
			{
				return joinedToken;
			}
		}
		
		/// <value>
		/// Contains the rule being used to match the token.
		/// </value>
		public LexicalRule MatchingRule
		{
			get
			{
				return matchingRule;
			}
		}

		/// <value>
		/// <c>true</c> if the found token is a valid one.
		/// </value>
		public bool Found 
		{
			get 
			{
				return found;
			}
		}
	}
	
	
	
}
