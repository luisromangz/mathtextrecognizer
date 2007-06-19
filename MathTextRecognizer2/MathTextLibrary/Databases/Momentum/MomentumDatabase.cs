//Creado por: Luis Román Gutiérrez a las 13:48 de 06/07/2007

using System;

using MathTextLibrary.Databases;

namespace MathTextLibrary.Databases.Momentum
{
	
	[DatabaseDescription("Base de datos basada en los momentos de los caracteres")]
	public class MomentumDatabase : DatabaseBase
	{
		
		public MomentumDatabase() : base()
		{
		}	
		
		public override void Learn(MathTextBitmap image, MathSymbol symbol)
		{
			
		}
		                          
			
		public override MathSymbol Recognize (MathTextBitmap image)
		{
			return null;
		}
	}
}
