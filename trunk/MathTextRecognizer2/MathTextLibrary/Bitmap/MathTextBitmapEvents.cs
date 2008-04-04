
using System;
using System.Collections.Generic;

namespace MathTextLibrary.Bitmap
{
	public delegate void ChildrenAddedHandler(object sender, ChildrenAddedArgs arg);

	public delegate void SymbolChangedHandler(object sender, EventArgs arg);
	
	/// <summary>
	/// Esta clase se usa para pasar un array de <c>MathTextBitmap</c> como
	/// argumento en el evento <c>SymbolChangedEventHandler</c>.
	/// </summary>
	public class ChildrenAddedArgs : EventArgs
	{
		private List<MathTextBitmap> children;

		public ChildrenAddedArgs(List<MathTextBitmap> children)
			: base()
		{
			this.children = children;           		
		}

		/// <value>
		/// Contiene las subimagenes en que se dividió la imagen.
		/// </value>
		public List<MathTextBitmap> Children
		{
			get{
				return children;  
			}
		}
	}	
	
	
}
