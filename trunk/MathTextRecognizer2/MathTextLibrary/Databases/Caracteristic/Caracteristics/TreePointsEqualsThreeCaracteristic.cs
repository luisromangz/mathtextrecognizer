using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de puntos de arbol (aquellos
	/// con tres o mas vecinos) es igual a 3.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Caracteristics.Helpers.CountPixelsWithXNeighboursHelper"/>
	public class TreePointsEqualsThreeCaracteristic:IBinaryCaracteristic
	{
		public TreePointsEqualsThreeCaracteristic()
		{
			priority=310;
		}
				
		public override bool Apply(MathTextBitmap image)
		{
			return CountPixelsWithXNeighboursHelper.numPixelsXOrMoreNeighbours(image, 3)==3;
		}	
	}
}