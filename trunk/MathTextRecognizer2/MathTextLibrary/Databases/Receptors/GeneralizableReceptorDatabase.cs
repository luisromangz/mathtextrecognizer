// ReceptorGridDatabase.cs created with MonoDevelop
// User: luis at 16:04 05/06/2008

using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using System.Reflection;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;
using MathTextLibrary.Databases;
using MathTextLibrary.Databases.Characteristic;
using MathTextLibrary.Databases.Characteristic.Characteristics;



namespace MathTextLibrary.Databases.Receptors
{	
	/// <summary>
	/// Esta clase se encarga de mantener una base de datos de caracteristicas
	/// binarias para los caracteres aprendidos, asi como de realizar el añadido
	/// de nuevos caracteres en la misma y realizar busquedas.
	/// </summary>
	[DatabaseTypeInfo("Base de datos de receptores en la imagen con generalizacion",
	                  "Receptores con vectores generalizables")]	
	
	[XmlInclude(typeof(TristateCheckVector))]
	public class GeneralizableReceptorDatabase : Database
	{
#region Fields
		
		private const float epsilon = 0.05f;  
		
		private List<Receptor> receptors;
		
		private List<TristateCheckVector> symbolsDict;
		
		private List<BinaryCharacteristic> characteristics;
		
#endregion Fields
		
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
				foreach(TristateCheckVector vector in symbolsDict)
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
		public List<TristateCheckVector> Vectors
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

		/// <value>
		/// Contains the receptors used by the database.
		/// </value>
		public List<Receptor> Receptors
		{
			get 
			{
				return receptors;
			}
			set
			{
				receptors = value;
			}
		}
	
		
		

		
#endregion Propiedades
				

		
		/// <summary>
		/// <c>ReceptorVectorDatabase</c>'s constructor.
		/// </summary>
		public GeneralizableReceptorDatabase() : base()
		{	
			symbolsDict = new List<TristateCheckVector>();
			
			characteristics = new List<BinaryCharacteristic>();
			
			characteristics.Add(new TallerThanWiderCharacteristic());
			//characteristics.Add(new SegmentableCharacteristic());
		}
		
		
#region Public methods
		/// <summary>
		/// This method is used to add new symbols to the database.
		/// </summary>
		/// <param name="bitmap">
		/// The image containing the symbol.
		/// </param>
		/// <param name="symbol">
		/// The symbol contained in the image.
		/// </param>
		public override bool Learn(MathTextBitmap bitmap,MathSymbol symbol)
		{
			FloatBitmap processedBitmap = bitmap.LastProcessedImage;
			TristateCheckVector newVector = CreateVector(processedBitmap);			
			
			TristateCheckVector found =null;
			foreach (TristateCheckVector vector in symbolsDict) 
			{
				if(vector.Symbols.Contains(symbol))
				{
					found  = vector;
					break;
				}
			}
			
			if(found ==null)
			{
				// The symbol wasnt present in the database.
				newVector.Symbols.Add(symbol);
				symbolsDict.Add(newVector);
			}
			else
			{
				// The symbol is present, we may have to retrain the database.
				if(newVector.Equals(found))
				{
					// It's the same one, so there is a conflict.
					return false;
				}
				else
				{
					// We have to find the differnces, and change them to
					// don't care values.
					for(int i=0; i< found.Length; i++)
					{
						if(found[i] != TristateValue.DontCare
						   && found[i] != newVector[i])
						{
							// If the value is different from what we had learned, 
							// then we make the vector more general in that point.
							found[i] = TristateValue.DontCare;
						}
					}
				}
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
			List<MathSymbol> res = new List<MathSymbol>();
			
			FloatBitmap processedImage = image.LastProcessedImage;
			
			TristateCheckVector vector = CreateVector(processedImage);
			
			// We have this image vector, now we will compare with the stored ones.
			
			
			
			foreach(TristateCheckVector storedVector in symbolsDict)
			{
				// We consider a threshold.
				int threshold = (int)((storedVector.Length - storedVector.DontCares) * epsilon); 
				
				// If the distance is below the threshold, we consider it valid.
				if(storedVector.Distance(vector) <= threshold)
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
		
		
		
#endregion Public methods
		
#region Private methods
		/// <summary>
		/// Creates a <see cref="TristateCheckVector"/> instance for a given
		/// <c>FloatBitmap</c> object.
		/// </summary>
		/// <param name="image">
		/// A <see cref="FloatBitmap"/> for which the vector is created.
		/// </param>
		/// <returns>
		/// The <see cref="TristateCheckVector"/> for the image.
		/// </returns>
		private TristateCheckVector CreateVector(FloatBitmap image)
		{
			// We create the receptors list.
			if(receptors == null)
			{
				receptors = Receptor.GenerateList(40);
			}
			
			TristateCheckVector vector = new TristateCheckVector();
			
			TristateValue checkValue;
			
			foreach (Receptor receptor in receptors) 
			{
				checkValue = 
					receptor.CheckBressard(image)? 
						TristateValue.True:TristateValue.False;
				
				vector.Values.Add(checkValue);
				StepDoneArgs args = 
					new StepDoneArgs(String.Format("Comprobando receptor {0}: {1}", 
					                               String.Format("({0}, {1}) -> ({2}, {3})",
					                                             receptor.X0,receptor.Y0,
					                                             receptor.X1, receptor.Y1),					                               
					                               checkValue));
				
				StepDoneInvoker(args);
				Thread.Sleep(20);
			}
			
			foreach (BinaryCharacteristic characteristic in characteristics) 
			{
				checkValue = 
					characteristic.Apply(image)?
						TristateValue.True
						: TristateValue.False;
				
				vector.Values.Add(checkValue);
				
				StepDoneArgs args = 
					new StepDoneArgs(String.Format("Comprobando característica {0}: {1}", 
					                               characteristic.GetType().ToString(),
					                               checkValue));
				
				StepDoneInvoker(args);
				Thread.Sleep(20);
			}
			
			return vector;
		}
		
#endregion Private methods

		
	}
}

