using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de pixeles negros en el
	/// cuadrante noreste es mayor al numero de pixeles negros en cada uno
	/// de los demas cuadrantes.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountPixelsHelper"/>
	public class PixelsNortheastQuadrantCharacteristic:IBinaryCharacteristic
	{
		public PixelsNortheastQuadrantCharacteristic()
		{
			priority=120;
		}

		public override bool Apply(FloatBitmap image)
		{
			int npixelsNW=CountPixelsHelper.NumBlackPixelsInQuadrant(image, Quadrant.NW);
			int npixelsNE=CountPixelsHelper.NumBlackPixelsInQuadrant(image, Quadrant.NE);
			int npixelsSW=CountPixelsHelper.NumBlackPixelsInQuadrant(image, Quadrant.SW);
			int npixelsSE=CountPixelsHelper.NumBlackPixelsInQuadrant(image, Quadrant.SE);
			
			return (npixelsNE>npixelsNW) && (npixelsNE>npixelsSW)
				&& (npixelsNE>npixelsSE);
		}
	}
}
