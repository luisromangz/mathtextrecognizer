using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de pixeles negros en la mitad
	/// izquierda de la imagen es mayor al numero de pixeles negros en la mitad
	/// derecha.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountPixelsHelper"/>
	public class PixelsLeftHalfCharacteristic:BinaryCharacteristic
	{

		private const float epsilon = 0.01f;
		
		public PixelsLeftHalfCharacteristic()
		{
			priority=50;
		}

		public override bool Apply(FloatBitmap image)
		{
			int npixelsLeft=CountPixelsHelper.NumBlackPixelsInHalf(image, Half.Left);
			int npixelsRight=CountPixelsHelper.NumBlackPixelsInHalf(image, Half.Right);
			
			int tolerance = (int)((image.Width * image.Height)*epsilon);
			
			return npixelsLeft > npixelsRight + tolerance;
		}
	}
}
