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
	[DatabaseTypeInfo("Base de datos de vectores de características binarias",
	                  "Vectores de características binarias")]	
	
	[XmlInclude(typeof(CharacteristicVector))]
	public class CharacteristicHashDatabase : DatabaseBase
	{
#region Atributos
		
		private const float epsilon = 0.05f;  
			
		// Lista de caracteristicas binarias que se aplican sobre las imagenes.
		private static List<IBinaryCharacteristic> characteristics;
		
		private List<CharacteristicVector> symbolsDict;
		
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
				foreach(CharacteristicVector vector in symbolsDict)
				{
					List<MathSymbol> symbols = vector.Symbols;
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

		/// <value>
		/// Contains the association between value vectors and symbols.
		/// </value>
		public List<CharacteristicVector> Vectors
		{
			get 
			{
				return symbolsDict;
			}
			set
			{
				symbolsDict = value;
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
			symbolsDict = new List<CharacteristicVector>();
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
		/// </param>
		public override bool Learn(MathTextBitmap bitmap,MathSymbol symbol)
		{
			if(characteristics == null)
				characteristics=CharacteristicFactory.CreateCharacteristicList();
			
			bool characteristicValue;	
			
			FloatBitmap processedBitmap = bitmap.LastProcessedImage;
			CharacteristicVector vector = CreateVector(processedBitmap);
			
			Console.WriteLine(vector.ToString());
			
			int position = symbolsDict.IndexOf(vector);
			if(position >=0)
			{
				// The vector exists in the list
				Console.WriteLine("existe");
				List<MathSymbol> symbols = symbolsDict[position].Symbols;
				if(symbols.Contains(symbol))
				{
					Console.WriteLine("conflicto");
					return false;
				}
				else
				{
					Console.WriteLine("no conflicto");
					symbols.Add(symbol);
				}
			}
			else
			{
				Console.WriteLine("no existe");
				vector.Symbols.Add(symbol);
				symbolsDict.Add(vector);
			}
			
			return true;
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
			
			FloatBitmap processedImage = image.LastProcessedImage;
			
			CharacteristicVector vector = CreateVector(processedImage);
			
			// We have this image vector, now we will compare with the stored ones.
			
			// We consider a threshold.
			int threshold = (int)(characteristics.Count * epsilon); 
			foreach(CharacteristicVector storedVector in symbolsDict)
			{
				// If the distance is below the threshold, we consider it valid.
				if(vector.Distance(storedVector) <= threshold)
				{
					foreach(MathSymbol symbol in storedVector.Symbols)
					{
						// We don't want duplicated symbols.
						if(!res.Contains(symbol))
						{
							res.Add(symbol);
						}
					}
					
					string msg = 
						String.Format("Distancia({0}, {1}) <= {2}",
						             vector.ToString(),
						             storedVector.ToString(),
						             threshold);
					
					msg += "\nSe añadieron los símbolos almacenados.";
					
					StepDoneInvoker(new StepDoneArgs(msg));
				}
				else
				{
					string msg = String.Format("Distancia({0}, {1}) > {2}",
					                           vector.ToString(),
					                           storedVector.ToString(),
					                           threshold);
					
					StepDoneInvoker(new StepDoneArgs(msg));
				}
				
				
			}
		
			return res;

		}
		
		
		
#endregion Métodos públicos
		
#region Private methods
		/// <summary>
		/// Creates a <c>CharacteristicVector</c> instance for a given
		/// <c>FloatBitmap</c> object.
		/// </summary>
		/// <param name="image">
		/// A <see cref="FloatBitmap"/>
		/// </param>
		/// <returns>
		/// A <see cref="CharacteristicVector"/>
		/// </returns>
		private CharacteristicVector CreateVector(FloatBitmap image)
		{
			CharacteristicVector vector = new CharacteristicVector();
			bool characteristicValue;
			
			foreach(IBinaryCharacteristic bc in characteristics)
			{
				characteristicValue = bc.Apply(image);
				
				vector.Values.Add(characteristicValue);
				
				StepDoneArgs args = 
					new StepDoneArgs(String.Format("Comprobando {0}: {1}", 
					                               bc.GetType(), 
					                               characteristicValue));
				
				StepDoneInvoker(args);
			}
			
			return vector;
		}
		
#endregion Private methods

		
	}
}