using System;

using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics
{
	/// <summary>
	/// Esta caracteristica determina si el numero de cambios blanco-negro
	/// en el eje Y de la imagen es menor estricto que 2.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Caracteristics.Helpers.CountColorChangesHelper"/>
	public class ColorChangesYBelowTwoCaracteristic:IBinaryCaracteristic
	{
		public ColorChangesYBelowTwoCaracteristic()
		{
			priority=200;
		}

		public override bool Apply(MathTextBitmap image)
		{
			if(CountColorChangesHelper.NumColorChangesColumn(image, image.ProcessedImageSize/2) < 2) 
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
