
using System;
using Gtk;
using Glade;

namespace MathTextCustomWidgets.Dialogs
{
	
	
	/// <summary>
	/// Esta clase implementa un diálogo "Acerca de..." para las
	/// aplicaciones.
	/// </summary>
	public class AppInfoDialog
	{
		[WidgetAttribute]
		private Gtk.AboutDialog appInfoDialog = null;	
		
		private AppInfoDialog(Window parent, string title, string msg, 
		                      string logoResource)
		{
			Glade.XML gxml = new Glade.XML (null, "gui.glade", "appInfoDialog", null);
			gxml.Autoconnect (this);
		    
		    appInfoDialog.TransientFor = parent;
		    appInfoDialog.Modal = true;
			
			appInfoDialog.Icon = parent.Icon;
		    
		    appInfoDialog.Name = title;
		    appInfoDialog.Comments = msg + 
		    	"Desarrollado como proyecto de fin de carrera por:\n\n"+
		    	"Luis Román Gutiérrez";
			
			appInfoDialog.Logo = ImageResources.LoadPixbuf(logoResource);
			
		}
		
		/// <summary>
		/// Muestra el diálogo de información de la aplicación.
		/// </summary>
		/// <param name = "parent">
		/// La ventana desde la que se muestra el diálogo.
		/// </param>
		/// <param name = "title">
		/// El título de la aplicación que se mostrará.
		/// </param>
		/// <param name = "msg">
		/// El mensaje que aparecerá para describir la aplicación.
		/// </param>		
		public static void Show(Window parent, string title, string msg, 
		                        string logoResource)
		{
			AppInfoDialog dialog = 
				new AppInfoDialog(parent, title, msg, logoResource);
			
			dialog.appInfoDialog.Run();
			dialog.appInfoDialog.Destroy();
		}
		
	}
}
