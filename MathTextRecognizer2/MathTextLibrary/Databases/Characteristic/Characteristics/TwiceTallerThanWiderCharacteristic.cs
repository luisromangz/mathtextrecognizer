using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el simbolo contenido en la imagen
	/// es como minimo el doble de alto que de ancho.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.ImageBoxerHelper"/>
	public class TwiceTallerThanWiderCharacteristic:IBinaryCharacteristic
	{
		public TwiceTallerThanWiderCharacteristic()
		{
			priority=20;
		}

		public override bool Apply(FloatBitmap image)
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
			
			return (2*height) >= width;
		}
	}
}
