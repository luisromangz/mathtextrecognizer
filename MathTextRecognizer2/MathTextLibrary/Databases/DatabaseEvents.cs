// created on 27/12/2005 at 21:50
using System;
using System.Collections.Generic;
using MathTextLibrary.Databases.Caracteristic.Caracteristics;

namespace MathTextLibrary.Databases
{

	/// <summary>
	/// Delegado que establece el tipo de los metodos que manejaran los eventos
	/// que informan de la comprobacion de caracteristicas binarias al aprender
	/// o recuperar un simbolo de la base de datos de caracteristicas.
	/// </summary>
	public delegate void ProcessingStepDoneEventHandler(
		object sender,
	    ProcessingStepDoneEventArgs arg);
	
	
	/// <summary>
	/// Delegado que establece el tipo de los metodos que manejaran el evento que
	/// informa de que se ha aprendido un simbolo en la base de datos de caracteres.
	/// </summary>
	public delegate void SymbolLearnedEventHandler(object sender,EventArgs arg);
		
	/// <summary>
	/// Clase que especializa <c>EventArgs</c> para ofrecer informacion adicional
	/// a los manejadores de los eventos encargados de escuchar cuando se ha dado
	/// un paso en el reconocimiento de un caracter. 
	/// </summary>
	public class ProcessingStepDoneEventArgs: EventArgs
	{	
	
		private ISymbolProcess process;
		private MathTextBitmap image;
		private bool result;
		private List<MathSymbol> similarSymbols;  	
	
		/// <summary>
		/// Constructor de la clase <c>ProcessingStepDoneEventArgs</c>.
		/// </summary>
		/// <param name="caracteristic">
		/// El proceso que se ha completado.
		/// </param>
		/// <param name="image">
		/// La imagen a la que se le ha aplicado el proceso.
		/// </param>
		/// <param name="result">
		/// El resultado del proceso.
		/// </param>
		/// <param name="similarSymbols">
		/// La lista de simbolos similares a respecto al proceso realizado.
		/// </param>
		public ProcessingStepDoneEventArgs(ISymbolProcess process,
		                                   MathTextBitmap image,
		                                   bool result,	
		                                   List<MathSymbol> similarSymbols) : base()
		{			
			this.image = image;
			this.process = process;
			this.result = result;		
			this.similarSymbols = similarSymbols;
		}
		
		/// <summary>
		/// Constructor de la clase <c>ProcessingStepDoneEventArgs</c>.
		/// </summary>
		/// <param name="caracteristic">El proceso que se ha completado.</param>
		/// <param name="image">La imagen a la que se le ha aplicado el proceso.</param>
		/// <param name="result">El resultado del proceso.</param>
		public ProcessingStepDoneEventArgs(ISymbolProcess process,
		                                   MathTextBitmap image,	
		                                   bool result)			
			: this(process, image, result, null){
		
		}
		
		/// <summary>
		/// Propiedad de solo lectura que permite obtener la lista de simbolos similares
		/// respecto a la propiedad comprobada (y anteriores).
		/// </summary>
		public List<MathSymbol> SimilarSymbols
		{
			get
			{				
				return similarSymbols;
			}
		}
		
		/// <summary>
		/// Propiedad de solo lectura que permite obtener el proceso realizado.
		/// </summary>
		public ISymbolProcess Process
		{
			get
			{
				return process;
			}			
		}
		
		
		/// <summary>
		/// Propiedad de solo lectura que permite obtener el resultado de la comprobacion
		/// de la caracteristica binaria.
		/// </summary>
		public bool Result
		{
			get
			{
				return result;
			}
		
		}
		
		/// <summary>
		/// Propiedad de solo lectura que permite obtner la imagen a la que se le ha aplicado
		/// la caracteristica binaria.
		/// </summary>
		public MathTextBitmap Image
		{
			get 
			{
				return image;
			}	
		}		
		
	}
	
}
	
