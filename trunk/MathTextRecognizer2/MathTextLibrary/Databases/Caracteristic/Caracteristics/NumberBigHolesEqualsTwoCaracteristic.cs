using System;

using MathTextLibrary;
using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de agujeros grandes
	/// es igual a 2.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Caracteristics.Helpers.CountNumberOfHolesHelper"/>
	public class NumberBigHolesEqualsTwoCaracteristic:IBinaryCaracteristic
	{
		public NumberBigHolesEqualsTwoCaracteristic()
		{
			priority=80;
		}
		
		public override bool Apply(MathTextBitmap image)
		{
			return (CountNumberOfHolesHelper.NumWhiteZones(image,true)-1) == 2;	
		}
	}
}
