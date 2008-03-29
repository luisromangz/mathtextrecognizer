using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de pixeles negros en la mitad
	/// superior de la imagen es mayor al numero de pixeles negros en la mitad
	/// inferior.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Caracteristics.Helpers.CountPixelsHelper"/>
	public class PixelsTopHalfCaracteristic:IBinaryCaracteristic
	{
		public PixelsTopHalfCaracteristic()
		{
			priority=40;
		}

		public override bool Apply(MathTextBitmap image)
		{
			int npixelsTop=CountPixelsHelper.NumBlackPixelsInHalf(image, Half.Top);
			int npixelsBottom=CountPixelsHelper.NumBlackPixelsInHalf(image, Half.Bottom);
			
			return npixelsTop > npixelsBottom;
		}
	}
}
