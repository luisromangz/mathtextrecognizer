//Creado por: Luis Román Gutiérrez a las 13:48 de 06/07/2007

using System;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;
using MathTextLibrary.Databases;

namespace MathTextLibrary.Databases.Momentum
{
	
	[DatabaseInfo("Base de datos basada en los momentos de los caracteres",
	              UsedTypes = 
	              new Type[]{typeof(MathSymbol)})]
	public class MomentumDatabase : DatabaseBase
	{
		
		public MomentumDatabase() : base()
		{
		}	
		
		public override void Learn(MathTextBitmap image, MathSymbol symbol)
		{
			
		}
		                          
			
		public override List<MathSymbol> Recognize (MathTextBitmap image)
		{
			return null;
		}
	}
}
