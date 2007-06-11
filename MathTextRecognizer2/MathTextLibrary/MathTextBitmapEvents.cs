
using System;

namespace MathTextLibrary
{
	public delegate void MathTextBitmapChildrenAddedEventHandler(object sender, MathTextBitmapChildrenAddedEventArgs arg);

	public delegate void MathTextBitmapSymbolChangedEventHandler(object sender, EventArgs arg);
	
	/// <summary>
	/// Esta clase se usa para pasar un array de <c>MathTextBitmap</c> como
	/// argumento en el evento <c>MathTextBitmapSymbolChangedEventHandler</c>.
	/// </summary>
	public class MathTextBitmapChildrenAddedEventArgs : EventArgs
	{
		private MathTextBitmap [] children;

		public MathTextBitmapChildrenAddedEventArgs(MathTextBitmap [] children)
			: base()
		{
			this.children = children;           		
		}

		public MathTextBitmap[] Children
		{
			get{
				return children;  
			}
		}
	}	
	
	
}
