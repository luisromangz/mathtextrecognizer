
using System;
using Gdk;

namespace CustomGtkWidgets
{
	
	/// <summary>
	/// This class serves as a repository for the icons and images used
	/// in all the interfaces.
	/// </summary>
	public class ImageResources
	{		
		
		public static Pixbuf ImageLoadIcon22
		{
			get
			{
				return Pixbuf.LoadFromResource("insert-image22.png");
			}
		}	
		
		public static Pixbuf ImageLoadIcon16
		{
			get
			{
				return Pixbuf.LoadFromResource("insert-image16.png");
			}
		}	
		
		public static Pixbuf DatabaseIcon22
		{
			get
			{
				return Pixbuf.LoadFromResource("database22.png");
			}
		}	
		
		public static Pixbuf DatabaseIcon16
		{
			get
			{
				return Pixbuf.LoadFromResource("database16.png");
			}
		}	
	}
}
