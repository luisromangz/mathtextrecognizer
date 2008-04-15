// IRecognizingStepWidget.cs created with MonoDevelop
// User: luis at 20:51 14/04/2008

using System;

namespace MathTextRecognizer.Steps
{
	
	/// <summary>
	/// Base para los 
	/// </summary>
	public abstract class RecognizingStepWidget
	{
		public abstract void ResetState();
		
		public abstract Gtk.Widget Widget
		{
			get;
		}
	}
}
