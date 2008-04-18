using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de cambios blanco-negro
	/// en el eje Y de la imagen es 4.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountColorChangesHelper"/>
	public class ColorChangesYEqualsFourCharacteristic:IBinaryCharacteristic
	{
		public ColorChangesYEqualsFourCharacteristic()
		{
			priority=220;
		}

		public override bool Apply(FloatBitmap image)
		{
			if(CountColorChangesHelper.NumColorChangesColumn(image,
			                                                 image.Height/2) == 4) 
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
