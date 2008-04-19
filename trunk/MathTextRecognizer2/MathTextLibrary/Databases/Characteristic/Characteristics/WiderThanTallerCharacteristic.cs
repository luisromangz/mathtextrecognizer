using System;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Databases.Characteristic.Characteristics.Helpers;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	/// <summary>
	/// Esta caracteristica determina si el simbolo contenido en la imagen
	/// es como mas ancho que alto.
	/// </summary>
	/// <seealso cref="MathTextLibrary.Characteristics.Helpers.ImageBoxerHelper"/>
	public class WiderThanTallerCharacteristic:IBinaryCharacteristic
	{
		private const float epsilon = 0.05f;
		public WiderThanTallerCharacteristic()
		{
			priority=10;
		}

		public override bool Apply(FloatBitmap image)
		{
			int x1,y1,x2,y2;
			
			try
			{
				ImageBoxerHelper.BoxImage(image,out x1,out y1,out x2,out y2);
			} 
			catch(ApplicationException)
			{
				return false;
			}
			
			int width=(x2-x1+1);
			int height=(y2-y1+1);
			
			int tolerance = (int) (image.Height * epsilon);
			
			return width >= height + tolerance;
		}
	}
}
