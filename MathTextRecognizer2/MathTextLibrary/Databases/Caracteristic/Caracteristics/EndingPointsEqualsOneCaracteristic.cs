using System;

using MathTextLibrary;
using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de puntos finales (aquellos
	/// con solo un vecino) es igual a 1.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Caracteristics.Helpers.CountPixelsWithXNeighboursHelper"/>
	public class EndingPointsEqualsOneCaracteristic:IBinaryCaracteristic
	{
		public EndingPointsEqualsOneCaracteristic()
		{
			priority=250;
		}
		
		public override bool Apply(MathTextBitmap image){
			return CountPixelsWithXNeighboursHelper.numPixelsXNeighbours(image, 1)==1;
		}		
	}
}
