using System;

using MathTextLibrary;
using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de agujeros grandes
	/// es igual a 0.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Caracteristics.Helpers.CountNumberOfHolesHelper"/>
	public class NumberBigHolesEqualsZeroCaracteristic:IBinaryCaracteristic
	{
		public NumberBigHolesEqualsZeroCaracteristic()
		{
			priority=60;
		}
		
		public override bool Apply(MathTextBitmap image)
		{
			return (CountNumberOfHolesHelper.NumWhiteZones(image,true)-1) == 0;	
		}
	}
}
