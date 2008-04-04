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
	                                               ProcessingStepDoneArgs arg);
	
	
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
	public class ProcessingStepDoneArgs: EventArgs
	{	
	
		private ISymbolProcess process;
		private MathTextBitmap image;
		private object result;
		private List<MathSymbol> similarSymbols;  	
	
		/// <summary>
		/// Constructor de la clase <c>ProcessingStepDoneArgs</c>.
		/// </summary>
		/// <param name="process">
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
		public ProcessingStepDoneArgs(ISymbolProcess process,
		                                   MathTextBitmap image,
		                                   object result,	
		                                   List<MathSymbol> similarSymbols) : base()
		{			
			this.image = image;
			this.process = process;
			this.result = result;		
			this.similarSymbols = similarSymbols;
		}
		
		/// <summary>
		/// Constructor de la clase <c>ProcessingStepDoneArgs</c>.
		/// </summary>
		/// <param name="process">El proceso que se ha completado.</param>
		/// <param name="image">La imagen a la que se le ha aplicado el proceso.</param>
		/// <param name="result">El resultado del proceso.</param>
		public ProcessingStepDoneArgs(ISymbolProcess process,
		                              MathTextBitmap image,	
		                              bool result)			
			: this(process, image, result, null){
		
		}
		
		/// <value>
		/// Contiene la lista de simbolos similares
		/// respecto a la propiedad comprobada (y anteriores).
		/// </value>
		public List<MathSymbol> SimilarSymbols
		{
			get
			{				
				return similarSymbols;
			}
		}
		
		/// <value>
		/// Contiene la instancia del algoritmo de procesado utilizado.
		/// </value>
		public ISymbolProcess Process
		{
			get
			{
				return process;
			}			
		}
		
		
		/// <value>
		/// Contiener el resultado de la aplicacion del paso del algoritmo.
		/// </value>
		public object Result
		{
			get
			{
				return result;
			}		
		}
		
		/// <value>
		/// Contiene la imagen a la que se le ha aplicado el algoritmo.
		/// </value>
		public MathTextBitmap Image
		{
			get 
			{
				return image;
			}	
		}		
		
	}
	
}
	
