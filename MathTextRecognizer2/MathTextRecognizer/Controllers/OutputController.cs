/*
 * Created by SharpDevelop.
 * User: Luis
 * Date: 07/01/2006
 * Time: 12:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Threading;

using MathTextLibrary;
using MathTextLibrary.Bitmap;

using MathTextLibrary.Controllers;

namespace MathTextRecognizer.Controllers
{
	/// <summary>
	/// Esta clase realiza funciones de controlador para el proceso de generar la salida
	/// MathML y LaTeX, ofreciendo una fachada a la interfaz de usuario, y proporcionando
	/// abstracion de los procesos subyacentes a la misma.
	/// </summary>
	public class OutputController
	{
				
		//La imagen original que contiene la formula que hemos reconocido/segmentado,
		//con todos sus hijos, siendo, en efecto, la raiz de un arbol de imagenes.
		private MathTextBitmap startImage;
		
		/// <summary>
		/// Este evento se lanza al terminar uno de los distintos pasos en los que
		/// se divide el procesado del arbol de imagenes para ofrecer la salida textual.
		/// </summary>
		public event ProcessFinishedHandler StepFinished;
		
		/// <summary>
		/// Este evento se lanza al terminar el procesado del arbol de imagenes, 
		/// y haberse obtenido la representacion LaTeX y MathML.
		/// </summary>
		public event ProcessFinishedHandler OutputCreated;
		
		/// <summary>
		/// Constructor de la clase MathTextOutputController, sera invocado
		/// dese la interfaz de usuario desde la que se quiera ofrecer la 
		/// salida en texto.
		/// </summary>
		public OutputController()
		{
			
		}	
		
		/// <summary>
		/// Metodo para lanzar el evento StepFinished con mayor comodidad.
		/// </summary>
		protected void OnStepFinished()
		{
			if(StepFinished!=null)
			{
				StepFinished(this,EventArgs.Empty);
			}			
		}
		
		/// <summary>
		/// Metodo para lanzar el evento OutputCreated con mayor comodidad.
		/// </summary>
		protected void OnOutputCreated()
		{
			if(OutputCreated!=null)
			{
				OutputCreated(this,EventArgs.Empty);
			}
		}
		
		/// <value>
		/// Propiedad que nos permite establecer y recuperar la imagen que contiene la
		/// formula, una vez procesada y siendo la raiz de un arbol de imagenes.
		/// </value>
		public MathTextBitmap StartImage
		{
			get
			{
				return startImage;
			}
			set
			{					
				startImage=value;
			}			
		}		
		
		/// <summary>
		/// Este metodo es el encargado de llamar a los distintos procesados de arboles,
		/// y generar las salidas LaTeX y MathML.
		/// </summary>
		public void MakeOutput()
		{			
			
		}
	}
}
