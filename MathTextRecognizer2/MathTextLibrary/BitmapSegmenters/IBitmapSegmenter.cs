// created on 28/12/2005 at 13:23
using System.Collections.Generic;

namespace MathTextLibrary.BitmapSegmenters{
	/// <summary>
	/// Esta interfaz define los metodos comunes a todas las
	/// clases que implementan metodos para descomponer 
	/// formulas en sus distintas partes.
	/// </summary>
	public interface IBitmapSegmenter
	{
		
		/// <summary>
		/// El metodo que sera invocado para intentar descomponer
		/// una imagen.
		/// </summary>
		/// <param name="mtb">La imagen que queremos descomponer.</param>
		/// <returns>
		/// Un array con las distintas partes en que se
		/// ha descompuesto la imagen.
		/// </returns>
		List<MathTextBitmap> Segment(MathTextBitmap mtb);
	}

	
}
