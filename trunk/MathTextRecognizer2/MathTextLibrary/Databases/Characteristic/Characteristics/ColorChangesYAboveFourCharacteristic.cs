using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de cambios blanco-negro
	/// en el eje Y de la imagen es mayor estricto que 4.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountColorChangesHelper"/>
	public class ColorChangesYAboveFourCharacteristic :IBinaryCharacteristic
	{
		public ColorChangesYAboveFourCharacteristic()
		{
			priority=230;
		}

		public override bool Apply(MathTextBitmap image)
		{
			if(CountColorChangesHelper.NumColorChangesColumn(image,
			                                                 image.LastProcessedImage.Height/2) > 4) 
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
