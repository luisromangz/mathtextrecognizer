using System;

using MathTextLibrary;
using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de agujeros grandes
	/// es igual a 1.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.CountNumberOfHolesHelper"/>
	public class NumberBigHolesEqualsOneCharacteristic:IBinaryCharacteristic
	{
		public NumberBigHolesEqualsOneCharacteristic()
		{
			priority=70;
		}
		
		public override bool Apply(FloatBitmap image)
		{
			return (CountNumberOfHolesHelper.NumWhiteZones(image,true)-1) == 1;	
		}
	}
}
