
using System;
using Gdk;

namespace MathTextCustomWidgets
{
	
	/// <value>
	/// This class serves as a repository for the icons and images used
	/// in all the interfaces.
	/// </value>
	public class ImageResources
	{		
		
		/// <summary>
		/// Obtains an ImageWidget for a resource icon.
		/// </summary>
		/// <param name="resource">
		/// The wanted resouce's name.
		/// </param>
		/// <returns>
		/// An <c>Image</c> GTK widget.
		/// </returns>
		public static Gtk.Image LoadImage(string resource)
		{
			
			return new Gtk.Image(Pixbuf.LoadFromResource(resource+".png"));
			
		}	
		
		/// <summary>
		/// Obtains an Pixbuf for a resource icon.
		/// </summary>
		/// <param name="resource">
		/// The wanted resouce's name.
		/// </param>
		/// <returns>
		/// An <c>Gdk.Pixbuf</c> image
		/// </returns>
		public static Gdk.Pixbuf LoadPixbuf(string resource)
		{
			
			return Pixbuf.LoadFromResource(resource+".png");
			
		}	
	}
}
