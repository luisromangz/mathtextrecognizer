using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Caracteristic.Caracteristics.Helpers;

namespace MathTextLibrary.Databases.Caracteristic.Caracteristics
{
	/// <summary>
	/// Esta caracteristica determina si el simbolo contenido en la imagen
	/// es como minimo tres veces mas alto que ancho.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Caracteristics.Helpers.ImageBoxerHelper"/>
	public class ThriceTallerThanWiderCaracteristic:IBinaryCaracteristic
	{
		public ThriceTallerThanWiderCaracteristic()
		{
			priority=30;
		}

		public override bool Apply(MathTextBitmap image)
		{
			int x1,y1,x2,y2;
			
			try
			{
				ImageBoxerHelper.BoxImage(image,out x1,out y1,out x2,out y2);
			} catch(ApplicationException)
			{
				return false;
			}
			
			int width=(x2-x1+1);
			int height=(y2-y1+1);
			
			return (3*height) >= width;
		}
	}
}
