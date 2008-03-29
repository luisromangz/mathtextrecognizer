using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de cambios blanco-negro
	/// en el eje X de la imagen es mayor estricto que 4.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountColorChangesHelper"/>
	public class ColorChangesXAboveFourCharacteristic :IBinaryCharacteristic
	{
		public ColorChangesXAboveFourCharacteristic()
		{
			priority=190;
		}

		public override bool Apply(MathTextBitmap image)
		{
			if(CountColorChangesHelper.NumColorChangesRow(image,
			                                              image.ProcessedImage.GetLength(0)/2) > 4) 
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
