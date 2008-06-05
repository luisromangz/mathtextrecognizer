using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de puntos de arbol (aquellos
	/// con tres o mas vecinos) es igual a 1.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountPixelsWithXNeighboursHelper"/>
	public class TreePointsEqualsOneCharacteristic:BinaryCharacteristic
	{
		public TreePointsEqualsOneCharacteristic()
		{
			priority=290;
		}
				
		public override bool Apply(FloatBitmap image)
		{
			return CountPixelsWithXNeighboursHelper.CountPixelsXOrMoreNeighbours(image, 3)==1;
		}
	}
}
