// created on 02/01/2006 at 13:20
using System;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Controllers
{

	/// <summary>
	/// Delegado para los manejadores de eventos que se envian cuando el controlador
	/// desea enviar un mensaje para que sea mostrada en la interfaz.
	/// </summary>
	public delegate void MessageLogSentHandler(object sender,
	                                           MessageLogSentArgs logMsg);

	/// <summary>
	/// Delegado para los manejadores de eventos que se envian cuando un controlador
	/// desea notificar que ha finalizado un determinado proceso.
	/// </summary>
	public delegate void ProcessFinishedHandler(object sender, EventArgs arg);
	
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
	
	
}
