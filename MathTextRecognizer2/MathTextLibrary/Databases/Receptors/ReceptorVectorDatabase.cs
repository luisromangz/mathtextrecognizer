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
using MathTextLibrary.Databases;



namespace MathTextLibrary.Databases.Receptors
{	
	/// <summary>
	/// Esta clase se encarga de mantener una base de datos de caracteristicas
	/// binarias para los caracteres aprendidos, asi como de realizar el añadido
	/// de nuevos caracteres en la misma y realizar busquedas.
	/// </summary>
	[DatabaseTypeInfo("Base de datos de vectores de receptores en la imagen",
	                  "Vectores de receptores")]	
	
	[XmlInclude(typeof(CheckVector))]
	public class ReceptorVectorDatabase : Database
	{
#region Atributos
		
		private const float epsilon = 0.1f;  
		
		private List<Receptor> receptors;
		
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
				
#region Métodos públicos
		
		/// <summary>
		/// <c>ReceptorVectorDatabase</c>'s constructor.
		/// </summary>
		public ReceptorVectorDatabase() : base()
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
			List<MathSymbol> res = new List<MathSymbol>();
			
			FloatBitmap processedImage = image.LastProcessedImage;
			
			CheckVector vector = CreateVector(processedImage);
			
			// We have this image vector, now we will compare with the stored ones.
			
			// We consider a threshold.
			int threshold = (int)(receptors.Count * epsilon); 
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
			// We create the receptors list.
			if(receptors == null)
			{
				receptors = Receptor.GenerateList(50);
			}
			
			CheckVector vector = new CheckVector();
			bool checkValue;
			
			int		width = image.Width;
			int		height = image.Height;
			
			for(int i = 0; i<receptors.Count ;i++)
			{
				// We initialize the new check vector
				vector.Values.Add(false);
			}

			Receptor receptor;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					// Check for black pixel
					if (image[x, y] == FloatBitmap.Black)
					{
						for (int i = 0; i < receptors.Count; i++)
						{
							// Skip already activated receptors
							if (vector.Values[i])
								continue;

							receptor = receptors[i];
							checkValue =
								receptor.GetReceptorState(x, y, width, height);
							vector.Values[i]= checkValue;
							
							
							// We inform the controller.
							StepDoneArgs args = 
								new StepDoneArgs(String.Format("Comprobando receptor {0} en el pixel {1}: {2}", 
								                               String.Format("({0}, {1}) -> ({2}, {3})",
								                                             receptor.X0,receptor.Y0,
								                                             receptor.X1, receptor.Y1),
								                               String.Format("({0}, {1})" ,x, y),
								                               checkValue));
				
							StepDoneInvoker(args);
						}
					}
				}
			}
			return vector;
		}
		
#endregion Private methods

		
	}
}
