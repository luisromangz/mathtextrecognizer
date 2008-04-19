// created on 27/12/2005 at 21:50
using System;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;
using MathTextLibrary.Databases.Characteristic.Characteristics;

namespace MathTextLibrary.Databases
{

	/// <summary>
	/// Delegado que establece el tipo de los metodos que manejaran los eventos
	/// que informan de la comprobacion de caracteristicas binarias al aprender
	/// o recuperar un simbolo de la base de datos de caracteristicas.
	/// </summary>
	public delegate void ProcessingStepDoneHandler(object sender,
	                                               StepDoneArgs arg);
	
	
	/// <summary>
	/// Delegado que establece el tipo de los metodos que manejaran el evento que
	/// informa de que se ha aprendido un simbolo en la base de datos de caracteres.
	/// </summary>
	public delegate void SymbolLearnedHandler(object sender,EventArgs arg);
		
	/// <summary>
	/// Clase que especializa <c>EventArgs</c> para ofrecer informacion adicional
	/// a los manejadores de los eventos encargados de escuchar cuando se ha dado
	/// un paso en el reconocimiento de un caracter. 
	/// </summary>
	public class StepDoneArgs: EventArgs
	{	
	
		private string message;
			
		/// <summary>
		/// <c>ProcessingStepDoneArgs</c>'s constructor.
		/// </summary>
		/// <param name="process">
		/// The message to be shown.
		/// </param>		
		public StepDoneArgs(string message, 
		                              params string[] pars) : base()
		{			
			this.message =String.Format(message,pars);
			
		}
		
		/// <value>
		/// Contiene la lista de simbolos similares
		/// respecto a la propiedad comprobada (y anteriores).
		/// </value>
		public string  Message
		{
			get
			{				
				return message;
			}
		}
		
	}
	
}
	
