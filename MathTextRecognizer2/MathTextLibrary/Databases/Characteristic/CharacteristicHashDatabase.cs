// CharacteristicHash.cs created with MonoDevelop
// User: luis at 13:37 19/04/2008

using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using System.Reflection;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;
using MathTextLibrary.Databases.Characteristic.Characteristics;


namespace MathTextLibrary.Databases.Characteristic
{	
	/// <summary>
	/// Esta clase se encarga de mantener una base de datos de caracteristicas
	/// binarias para los caracteres aprendidos, asi como de realizar el añadido
	/// de nuevos caracteres en la misma y realizar busquedas.
	/// </summary>
	[DatabaseTypeInfo("Base de datos basada en el uso de un mapa de características binarias",
	                  "Arbol de características binarias")]	
	[XmlInclude(typeof(MathSymbol))]
	[XmlInclude(typeof(CharacteristicVector))]
	public class CharacteristicHashDatabase : DatabaseBase
	{
#region Atributos
		
		private const float epsilon = 0.15f;  
			
		// Lista de caracteristicas binarias que se aplican sobre las imagenes.
		private static List<IBinaryCharacteristic> characteristics;
		
		private Dictionary<CharacteristicVector, List<MathSymbol>> symbolsDict;
		
#endregion Atributos
		
#region Propiedades
		/// <value>
		/// Contiene los simbolos almacenados en la base de datos.
		/// </value>
		[XmlIgnoreAttribute]
		public override List<MathSymbol> SymbolsContained 
		{
			get
			{
				List<MathSymbol> res = new List<MathSymbol>();
				foreach(List<MathSymbol> symbols in symbolsDict.Values)
				{
					foreach(MathSymbol symbol in symbols)
					{
						// We add the symbols that aren't already present.
						if (!res.Contains(symbol))
						{
							res.Add(symbol);
						}
					}
				}
				
				return res;
			}
		}	

		
#endregion Propiedades
				
#region Métodos públicos
		
		/// <summary>
		/// Constructor de <c>CharacteristicDatabase</c>. Crea una base de datos
		/// vacia, sin ningun simbolo aprendido.
		/// </summary>
		public CharacteristicHashDatabase() : base()
		{	
			symbolsDict = new Dictionary<CharacteristicVector,List<MathSymbol>>();
		}
		
		/// <summary>
		/// Con este metodos almacenamos un nuevo simbolo en la base de
		/// datos.
		/// 
		/// Lanza <c>DuplicateSymbolException</c> si ya existe un simbolo
		/// con la misma etiqueta y caracteristicas binarias en la base de datos.
		/// </summary>
		/// <param name="bitmap">
		/// La imagen cuyas caracteristicas aprenderemos.
		/// </param>
		/// <param name="symbol">
		/// El simbolo que representa a la imagen.
		///</param>
		public override void Learn(MathTextBitmap bitmap,MathSymbol symbol)
		{
			if(characteristics == null)
				characteristics=CharacteristicFactory.CreateCharacteristicList();
			
			bool characteristicValue;	
			
			FloatBitmap processedBitmap = bitmap.LastProcessedImage;
			CharacteristicVector vector = new CharacteristicVector();
			
			// Recorremos las caracteristicas, y vamos creando el arbol segun
			// vamos necesitando nodos.
			foreach(IBinaryCharacteristic bc in characteristics)
			{					
				characteristicValue = bc.Apply(processedBitmap);
				
				vector.AddValue(characteristicValue);
				
				StepDoneArgs args = 
					new StepDoneArgs(String.Format("{0}: {1}", 
					                               bc.GetType(), 
					                               characteristicValue));
					
				this.StepDoneInvoker(args);
			}	
			
							
			SymbolLearnedInvoke();
		}
		
		
		
		/// <summary>
		/// Este metodo intenta recuperar los simbolos de la base de datos 
		/// correspondiente a una imagen.
		/// </summary>
		/// <param name="image">
		/// La imagen cuyos simbolos asociados queremos encontrar.
		/// </param>
		/// <returns>
		/// Los simbolos que se puedieron asociar con la imagen.
		/// </returns>
		public override List<MathSymbol> Match(MathTextBitmap image)
		{
			if(characteristics == null)
				characteristics=CharacteristicFactory.CreateCharacteristicList();
			
			List<MathSymbol> res = new List<MathSymbol>();
			
			bool exists=true; 
			bool characteristicValue;
			
			CharacteristicVector vector = new CharacteristicVector();
			
			FloatBitmap processedImage = image.LastProcessedImage;
			
			foreach(IBinaryCharacteristic bc in characteristics)
			{
				characteristicValue = bc.Apply(processedImage);
				
				vector.AddValue(characteristicValue);
				
				StepDoneArgs args = 
					new StepDoneArgs(String.Format("{0}: {1}", 
					                               bc.GetType(), 
					                               characteristicValue));
				
				StepDoneInvoker(args);
			}
			
		
			return res;

		}
		
		
		
#endregion Métodos públicos
		

		
	}
}