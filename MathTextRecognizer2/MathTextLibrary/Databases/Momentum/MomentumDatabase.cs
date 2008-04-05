//Creado por: Luis Román Gutiérrez a las 13:48 de 06/07/2007

using System;
using System.Collections.Generic;

using System.Xml.Serialization;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;
using MathTextLibrary.Databases;

namespace MathTextLibrary.Databases.Momentum
{
	
	[DatabaseTypeInfo("Base de datos basada en los momentos de los caracteres",
	                  "Momentos")]
	[XmlInclude(typeof(MathSymbol))]
	public class MomentumDatabase : DatabaseBase
	{
		
		public MomentumDatabase() : base()
		{
		}	
		
		/// <summary>
		/// Aprende un simbolo en la base de datos.
		/// </summary>
		/// <param name="image">
		/// La imagen que representa al simbolo.
		/// </param>
		/// <param name="symbol">
		/// El simbolo.
		/// </param>
		public override void Learn(MathTextBitmap image, MathSymbol symbol)
		{
			
		}
		                          
		/// <summary>
		/// Este método intenta asignar a una imagen un simbolo segun los parametros
		/// almacenados para la misma.
		/// </summary>
		/// <param name="image">
		/// La imagen que a la que queremos asociar un simbolo.
		/// </param>
		/// <returns>
		/// Una lista de simbolos cuyos parametros coinciden con los de la 
		/// imagen.
		/// </returns>
		public override List<MathSymbol> Match (MathTextBitmap image)
		{
			return null;
		}
		
		/// <value>
		/// Contiene los simbolos almacenados en la base de datos.
		/// </value>
		public override List<MathSymbol> SymbolsContained {
			get 
			{ 
				return null;
			}
		}

	}
}
