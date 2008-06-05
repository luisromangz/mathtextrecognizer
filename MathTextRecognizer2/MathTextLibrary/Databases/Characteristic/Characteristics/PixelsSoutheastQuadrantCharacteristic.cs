using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de pixeles negros en el
	/// cuadrante sureste es mayor al numero de pixeles negros en cada uno
	/// de los demas cuadrantes.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountPixelsHelper"/>
	public class PixelsSoutheastQuadrantCharacteristic:BinaryCharacteristic
	{
		private const float epsilon = 0.01f;
		
		public PixelsSoutheastQuadrantCharacteristic()
		{
			priority=140;
		}

		public override bool Apply(FloatBitmap image)
		{
			int npixelsNW=CountPixelsHelper.NumBlackPixelsInQuadrant(image, Quadrant.NW);
			int npixelsNE=CountPixelsHelper.NumBlackPixelsInQuadrant(image, Quadrant.NE);
			int npixelsSW=CountPixelsHelper.NumBlackPixelsInQuadrant(image, Quadrant.SW);
			int npixelsSE=CountPixelsHelper.NumBlackPixelsInQuadrant(image, Quadrant.SE);

			int tolerance = (int)((image.Width * image.Height)*epsilon);
			
			return (npixelsSE>npixelsNE + tolerance) 
				&& (npixelsSE>npixelsNW + tolerance)
				&& (npixelsSE>npixelsSW + tolerance);
		}
	}
}
