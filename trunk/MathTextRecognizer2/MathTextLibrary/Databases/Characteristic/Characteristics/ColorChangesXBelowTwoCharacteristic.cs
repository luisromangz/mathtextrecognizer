using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de cambios blanco-negro
	/// en el eje X de la imagen es menor estricto que 2.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountColorChangesHelper"/>
	public class ColorChangesXBelowTwoCharacteristic:IBinaryCharacteristic
	{
		public ColorChangesXBelowTwoCharacteristic()
		{
			priority=160;
		}

		public override bool Apply(MathTextBitmap image)
		{
			if(CountColorChangesHelper.NumColorChangesRow(image,
			                                              image.ProcessedImage.GetLength(0)/2) < 2) 
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
