using System;

using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de cambios blanco-negro
	/// en el eje X de la imagen es 4.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Caracteristics.Helpers.CountColorChangesHelper"/>
	public class ColorChangesXEqualsFourCaracteristic:IBinaryCaracteristic
	{
		public ColorChangesXEqualsFourCaracteristic()
		{
			priority=180;
		}

		public override bool Apply(MathTextBitmap image)
		{
			if(CountColorChangesHelper.NumColorChangesRow(image, image.ProcessedImageSize/2) == 4) 
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
