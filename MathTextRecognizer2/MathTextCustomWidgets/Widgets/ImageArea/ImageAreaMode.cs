
using System;

namespace MathTextCustomWidgets.Widgets.ImageArea
{
	
	
	/// <summary>
    /// Enumerado que establece el modo en que la imagen se ajustará 
    /// al control en el que se visualizará.
    /// </summary>
	public enum ImageAreaMode
	{
	    /// <summary>
	    /// La imagen se ajustará a los bordes del control, pudiendo perder
	    /// la proporción original.
	    /// </summary>	    
		Strecht,
		
		/// <summary>
		/// La imagen se ajustará a los bordes del control, respetando las
		/// proporciones originales.
		/// </summary>
		Zoom
	}
}
