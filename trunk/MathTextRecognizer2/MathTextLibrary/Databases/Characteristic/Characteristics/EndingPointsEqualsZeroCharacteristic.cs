using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de puntos finales (aquellos
	/// con solo un vecino) es igual a 0.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountPixelsWithXNeighboursHelper"/>
	public class EndingPointsEqualsZeroCharacteristic:BinaryCharacteristic
	{
		public EndingPointsEqualsZeroCharacteristic()
		{
			priority=240;
		}
		
		public override bool Apply(FloatBitmap image)
		{
			return CountPixelsWithXNeighboursHelper.CountPixelsXNeighbours(image, 1)==0;
		}	
	}
}
