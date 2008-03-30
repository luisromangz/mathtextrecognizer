using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de puntos de arbol (aquellos
	/// con tres o mas vecinos) es igual a 0.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountPixelsWithXNeighboursHelper"/>
	public class TreePointsEqualsZeroCharacteristic:IBinaryCharacteristic
	{
		public TreePointsEqualsZeroCharacteristic()
		{
			priority=280;
		}
				
		public override bool Apply(MathTextBitmap image)
		{
			return CountPixelsWithXNeighboursHelper.numPixelsXOrMoreNeighbours(image, 3)==0;
		}
	}
}
