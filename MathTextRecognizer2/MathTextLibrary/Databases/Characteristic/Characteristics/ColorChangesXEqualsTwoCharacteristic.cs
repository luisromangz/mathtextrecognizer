using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de cambios blanco-negro
	/// en el eje X de la imagen es 2.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountColorChangesHelper"/>
	public class ColorChangesXEqualsTwoCharacteristic:IBinaryCharacteristic
	{
		public ColorChangesXEqualsTwoCharacteristic()
		{
			priority=170;
		}

		public override bool Apply(FloatBitmap image)
		{
			if(CountColorChangesHelper.NumColorChangesRow(image, 
			                                              image.Width/2) == 2) 
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
