// RecognizingControllerEvents.cs created with MonoDevelop
// User: luis at 23:07 06/05/2008

using System;

using MathTextLibrary.Analisys.Lexical;
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
		private Token lastToken;
		private Token currentToken;
		
		
		
		public TokenCheckedArgs (Token lastToken, Token currentToken)
			: base ()
		{
			this.lastToken = lastToken;
			this.currentToken= currentToken;
		}
		
		/// <value>
		/// Contains the last token in the sequence the current token is being
		/// checked with.
		/// </value>
		public Token LastToken 
		{
			get {
				return lastToken;
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
}
