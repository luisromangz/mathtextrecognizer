using System;

using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de pixeles negros en la mitad
	/// izquierda de la imagen es mayor al numero de pixeles negros en la mitad
	/// derecha.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Caracteristics.Helpers.CountPixelsHelper"/>
	public class PixelsLeftHalfCaracteristic:IBinaryCaracteristic
	{
		public PixelsLeftHalfCaracteristic()
		{
			priority=50;
		}

		public override bool Apply(MathTextBitmap image)
		{
			int npixelsLeft=CountPixelsHelper.NumBlackPixelsInHalf(image, Half.Left);
			int npixelsRight=CountPixelsHelper.NumBlackPixelsInHalf(image, Half.Right);
			
			return npixelsLeft > npixelsRight;
		}
	}
}
