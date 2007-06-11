// created on 27/12/2005 at 21:50
using System;
using System.Collections;
using MathTextLibrary.Databases.Caracteristic.Caracteristics;

namespace MathTextLibrary.Databases.Caracteristic
{

	/// <summary>
	/// Delegado que establece el tipo de los metodos que manejaran los eventos
	/// que informan de la comprobacion de caracteristicas binarias al aprender
	/// o recuperar un simbolo de la base de datos de caracteristicas.
	/// </summary>
	public delegate void BinaryCaracteristicCheckedEventHandler(
		object sender,
	    BinaryCaracteristicCheckedEventArgs arg);
	
	
	/// <summary>
	/// Delegado que establece el tipo de los metodos que manejaran el evento que
	/// informa de que se ha aprendido un simbolo en la base de datos de caracteristicas.
	/// </summary>
	public delegate void SymbolLearnedEventHandler(
		object sender,EventArgs arg);
		
	/// <summary>
	/// Clase que especializa <c>EventArgs</c> para ofrecer informacion adicional
	/// a los manejadores de los eventos de comprobacion de caracteristicas binarias 
	/// generados en la base de datos.
	/// </summary>
	public class BinaryCaracteristicCheckedEventArgs
		: EventArgs
	{	
	
		private IBinaryCaracteristic caracteristic;
		private MathTextBitmap image;
		private bool result;
		private IList similarSymbols;  	
	
		/// <summary>
		/// Constructor de la clase <c>BinaryCaracteristicCheckedEventArgs</c>.
		/// </summary>
		/// <param name="caracteristic">La caracteristica que se ha comprobado.</param>
		/// <param name="image">La imagen a la que se le ha aplicado la comprobacion.</param>
		/// <param name="result">El resultado de la comprobacion.</param>
		/// <param name="similarSymbols">
		/// La lista de simbolos similares a respecto a la caracteristica comprobada.
		/// </param>
		public BinaryCaracteristicCheckedEventArgs(
				IBinaryCaracteristic caracteristic,MathTextBitmap image,bool result,	IList similarSymbols)
			: base()
		{
			
			this.image=image;
			this.caracteristic=caracteristic;
			this.result=result;		
			this.similarSymbols=similarSymbols;
		}
		
		/// <summary>
		/// Constructor de la clase <c>BinaryCaracteristicCheckedEventArgs</c>.
		/// </summary>
		/// <param name="caracteristic">La caracteristica que se ha comprobado.</param>
		/// <param name="image">La imagen a la que se le ha aplicado la comprobacion.</param>
		/// <param name="result">El resultado de la comprobacion.</param>
		public BinaryCaracteristicCheckedEventArgs(
			IBinaryCaracteristic caracteristic,
			MathTextBitmap image,	bool result)
			
			: this(caracteristic, image, result, null){
		
		}
		
		/// <summary>
		/// Propiedad de solo lectura que permite obtener la lista de simbolos similares
		/// respecto a la propiedad comprobada (y anteriores).
		/// </summary>
		public IList SimilarSymbols
		{
			get
			{				
				return similarSymbols;
			}
		}
		
		/// <summary>
		/// Propiedad de solo lectura que permite obtener la caracteristica que se ha
		///	comprobado.
		/// </summary>
		public IBinaryCaracteristic Caracteristic
		{
			get
			{
				return caracteristic;
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
	
