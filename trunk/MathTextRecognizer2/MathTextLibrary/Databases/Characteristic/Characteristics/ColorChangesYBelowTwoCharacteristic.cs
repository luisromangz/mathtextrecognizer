using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de cambios blanco-negro
	/// en el eje Y de la imagen es menor estricto que 2.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountColorChangesHelper"/>
	public class ColorChangesYBelowTwoCharacteristic:IBinaryCharacteristic
	{
		public ColorChangesYBelowTwoCharacteristic()
		{
			priority=200;
		}

		public override bool Apply(FloatBitmap image)
		{
			if(CountColorChangesHelper.NumColorChangesColumn(image, 
			                                                 image.Height/2) < 2) 
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
