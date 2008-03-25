
using System;
using System.Collections.Generic;

namespace MathTextLibrary.Bitmap
{
	public delegate void MathTextBitmapChildrenAddedEventHandler(object sender, MathTextBitmapChildrenAddedEventArgs arg);

	public delegate void MathTextBitmapSymbolChangedEventHandler(object sender, EventArgs arg);
	
	/// <summary>
	/// Esta clase se usa para pasar un array de <c>MathTextBitmap</c> como
	/// argumento en el evento <c>MathTextBitmapSymbolChangedEventHandler</c>.
	/// </summary>
	public class MathTextBitmapChildrenAddedEventArgs : EventArgs
	{
		private List<MathTextBitmap> children;

		public MathTextBitmapChildrenAddedEventArgs(List<MathTextBitmap> children)
			: base()
		{
			this.children = children;           		
		}

		public List<MathTextBitmap> Children
		{
			get{
				return children;  
			}
		}
	}	
	
	
}
