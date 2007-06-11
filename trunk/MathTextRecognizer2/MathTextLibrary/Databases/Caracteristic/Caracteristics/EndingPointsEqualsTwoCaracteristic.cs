using System;

using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de puntos finales (aquellos
	/// con solo un vecino) es igual a 2.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Caracteristics.Helpers.CountPixelsWithXNeighboursHelper"/>
	public class EndingPointsEqualsTwoCaracteristic:IBinaryCaracteristic
	{
		public EndingPointsEqualsTwoCaracteristic()
		{
			priority=260;
		}
		
		public override bool Apply(MathTextBitmap image){
			return CountPixelsWithXNeighboursHelper.numPixelsXNeighbours(image, 1)==2;
		}	
	}
}
