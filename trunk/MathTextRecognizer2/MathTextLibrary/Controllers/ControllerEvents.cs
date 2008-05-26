// created on 02/01/2006 at 13:20
using System;

using System.Collections.Generic;

using Gtk;

using MathTextLibrary.Bitmap;

using MathTextLibrary.Analisys;

namespace MathTextLibrary.Controllers
{

	/// <summary>
	/// Delegado para los manejadores de eventos que se envian cuando el controlador
	/// desea enviar un mensaje para que sea mostrada en la interfaz.
	/// </summary>
	public delegate void MessageLogSentHandler(object sender,
	                                           MessageLogSentArgs logMsg);

	/// <summary>
	/// Clase que implementa los argumentos necesarios en la transimisión de un
	/// mensaje indicando que un paso del reconocimiento se ha producido.
	/// </summary>
	public class MessageLogSentArgs : EventArgs
	{
	    private string message;
	    
	    /// <summary>
		/// Constructor de MessageLogSentEventArgs.
		/// </summary>
		/// <param name = "message">
		/// El mensaje que se transmite.
		/// </param>
	    public MessageLogSentArgs(string message)
	        : base()
	    {
	        this.message=message;    
	    }	
	
	    /// <value>
		/// Contiene el mensaje enviado.
		/// </value>
	    public string Message
	    {
	        get
	        {
	            return this.message;
	        }
	    }
	}
	
	public delegate void NodeBeingProcessedHandler(object sender,
	                                               NodeBeingProcessedArgs args);
	
	public class NodeBeingProcessedArgs : EventArgs
	{
		private ITreeNode node;
		
		public NodeBeingProcessedArgs(ITreeNode node)
		{
			this.node = node;
		}
		
		public ITreeNode Node
		{
			get
			{
				return node;
			}
		}
	}
	
	public delegate void MatchingHandler (object sender, MatchingArgs args);
	
	public class MatchingArgs : EventArgs
	{
		private SyntacticalMatcher matcher;
		
		
		public MatchingArgs (SyntacticalMatcher matcher)
		{
			this.matcher = matcher;			
		}
		
		public SyntacticalMatcher Matcher 
		{
			get 
			{
				return matcher;
			}
		}
		
		
	}
	
	
}
