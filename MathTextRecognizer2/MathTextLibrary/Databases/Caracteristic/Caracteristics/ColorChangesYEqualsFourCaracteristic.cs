using System;

using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de cambios blanco-negro
	/// en el eje Y de la imagen es 4.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Caracteristics.Helpers.CountColorChangesHelper"/>
	public class ColorChangesYEqualsFourCaracteristic:IBinaryCaracteristic
	{
		public ColorChangesYEqualsFourCaracteristic()
		{
			priority=220;
		}

		public override bool Apply(MathTextBitmap image)
		{
			if(CountColorChangesHelper.NumColorChangesColumn(image,
			                                                 image.ProcessedImage.GetLength(1)/2) == 4) 
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
