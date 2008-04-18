using System;

using MathTextLibrary;
using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de puntos finales (aquellos
	/// con solo un vecino) es igual a 1.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountPixelsWithXNeighboursHelper"/>
	public class EndingPointsEqualsOneCharacteristic:IBinaryCharacteristic
	{
		public EndingPointsEqualsOneCharacteristic()
		{
			priority=250;
		}
		
		public override bool Apply(FloatBitmap image){
			return CountPixelsWithXNeighboursHelper.CountPixelsXNeighbours(image, 1)==1;
		}		
	}
}
