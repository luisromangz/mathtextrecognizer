using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de puntos finales (aquellos
	/// con solo un vecino) es igual a 3.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountPixelsWithXNeighboursHelper"/>
	public class EndingPointsEqualsThreeCharacteristic:BinaryCharacteristic
	{
		public EndingPointsEqualsThreeCharacteristic()
		{
			priority=270;
		}
		
		public override bool Apply(FloatBitmap image)
		{
			return CountPixelsWithXNeighboursHelper.CountPixelsXNeighbours(image, 1)==3;
		}
	}
}
