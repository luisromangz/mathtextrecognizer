using System;

using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de puntos de arbol (aquellos
	/// con tres o mas vecinos) es igual a 2.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Caracteristics.Helpers.CountPixelsWithXNeighboursHelper"/>
	public class TreePointsEqualsTwoCaracteristic:IBinaryCaracteristic
	{
		public TreePointsEqualsTwoCaracteristic()
		{
			priority=300;
		}
				
		public override bool Apply(MathTextBitmap image)
		{
			return CountPixelsWithXNeighboursHelper.numPixelsXOrMoreNeighbours(image, 3)==2;
		}
	}
}
