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
	
	[XmlInclude(typeof(CheckVector))]
	public class CharacteristicHashDatabase : Database
	{
#region Atributos
		
		private const float epsilon = 0.05f;  
			
		// Lista de caracteristicas binarias que se aplican sobre las imagenes.
		private static List<BinaryCharacteristic> characteristics;
		
		private List<CheckVector> symbolsDict;
		
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
				foreach(CheckVector vector in symbolsDict)
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
		public List<CheckVector> Vectors
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
			symbolsDict = new List<CheckVector>();
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
		
			FloatBitmap processedBitmap = bitmap.LastProcessedImage;
			CheckVector vector = CreateVector(processedBitmap);
			
			
			int position = symbolsDict.IndexOf(vector);
			if(position >=0)
			{
				// The vector exists in the list
			
				List<MathSymbol> symbols = symbolsDict[position].Symbols;
				if(symbols.Contains(symbol))
				{
					return false;
				}
				else
				{
					symbols.Add(symbol);
				}
			}
			else
			{
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
			
			CheckVector vector = CreateVector(processedImage);
			
			// We have this image vector, now we will compare with the stored ones.
			
			// We consider a threshold.
			int threshold = (int)(characteristics.Count * epsilon); 
			foreach(CheckVector storedVector in symbolsDict)
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
		private CheckVector CreateVector(FloatBitmap image)
		{
			CheckVector vector = new CheckVector();
			bool characteristicValue;
			
			foreach(BinaryCharacteristic bc in characteristics)
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
