using System;

using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de puntos de arbol (aquellos
	/// con tres o mas vecinos) es igual a 0.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Caracteristics.Helpers.CountPixelsWithXNeighboursHelper"/>
	public class TreePointsEqualsZeroCaracteristic:IBinaryCaracteristic
	{
		public TreePointsEqualsZeroCaracteristic()
		{
			priority=280;
		}
				
		public override bool Apply(MathTextBitmap image)
		{
			return CountPixelsWithXNeighboursHelper.numPixelsXOrMoreNeighbours(image, 3)==0;
		}
	}
}
