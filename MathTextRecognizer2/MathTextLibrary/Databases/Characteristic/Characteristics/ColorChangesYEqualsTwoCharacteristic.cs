using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de cambios blanco-negro
	/// en el eje Y de la imagen es 2.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountColorChangesHelper"/>
	public class ColorChangesYEqualsTwoCharacteristic:IBinaryCharacteristic
	{
		public ColorChangesYEqualsTwoCharacteristic()
		{
			priority=210;
		}

		public override bool Apply(MathTextBitmap image)
		{
			if(CountColorChangesHelper.NumColorChangesColumn(image,
			                                                 image.ProcessedImage.GetLength(1)/2) == 2) 
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
