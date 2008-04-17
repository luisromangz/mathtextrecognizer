
using System;

using Gtk;

namespace MathTextCustomWidgets.CommonDialogs
{
	
	/// <summary>
	/// Esta clase implementa un cuadro de diálogo que se usa usarse para abrir
	/// un archivo que contenga una base de datos de reconocimiento de caracteres.
	/// </summary>
	public class DatabaseOpenDialog
	{
		#region Glade widgets
		
		[Glade.WidgetAttribute]
		private FileChooserDialog databaseOpenDialog;
		
		[Glade.WidgetAttribute]
		private Button btnOpen;
		
		[Glade.WidgetAttribute]
		private Button btnCancel;
		
		#endregion Glade widgets
		
		private DatabaseOpenDialog(Window parent)
		{
			Glade.XML xml = new Glade.XML(null,"gui.glade","databaseOpenDialog",null);
			xml.Autoconnect(this);
			
			databaseOpenDialog.Icon = parent.Icon;
			
			// Conectamos las acciones de los botones del diálogo.
			databaseOpenDialog.AddActionWidget(btnOpen,ResponseType.Ok);
			databaseOpenDialog.AddActionWidget(btnCancel,ResponseType.Cancel);			
			
			
			// Añadimos los archivos de filtros soportados
			FileFilter filter1=new FileFilter();
			filter1.Name="Archivo XML";
			filter1.AddPattern("*.xml");
			filter1.AddPattern("*.XML");
			
			FileFilter filter2=new FileFilter();
			filter2.Name="Base de datos de reconocimiento";
			filter2.AddPattern("*.jilfml");
			filter2.AddPattern("*.JILFML");
			
			FileFilter filter3=new FileFilter();
			filter3.Name="Todos los archivos";
			filter3.AddPattern("*.*");			
			
			databaseOpenDialog.AddFilter(filter2);		
			databaseOpenDialog.AddFilter(filter1);				
			databaseOpenDialog.AddFilter(filter3);	
		}
		
		#region Metodos públicos
		
		/// <summary>
		/// Este método muestra un cuadro de diálogo de apertura de base de
		/// datos de caracteres.
		/// </summary>
		/// <param name = "parent">
		/// La ventana desde la que se muestra el diálogo.
		/// </param>
		/// <param name = "filename">
		/// El nombre del archivo seleccionado.
		/// </param>
		public static ResponseType Show(Window parent, out string filename)
		{
			DatabaseOpenDialog dialog = new DatabaseOpenDialog(parent);
			ResponseType res = (ResponseType)dialog.databaseOpenDialog.Run();
			filename = dialog.databaseOpenDialog.Filename;
			dialog.databaseOpenDialog.Destroy();
			
			return res;
		}
		
		
		#endregion Métodos públicos
	}

}
