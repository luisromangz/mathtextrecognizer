
using System;

using Gtk;

namespace MathTextCustomWidgets.Dialogs
{
	
	/// <summary>
	/// Esta clase implementa un cuadro de diálogo que se usa usarse para abrir
	/// un archivo que contenga una base de datos de reconocimiento de caracteres.
	/// </summary>
	public class DatabaseSaveDialog
	{
		#region Glade widgets
		
		[Glade.WidgetAttribute]
		private FileChooserDialog databaseSaveDialog;
		
		[Glade.WidgetAttribute]
		private Button btnSave;
		
		[Glade.WidgetAttribute]
		private Button btnCancel;
		
		#endregion Glade widgets
		
		private DatabaseSaveDialog(Window parent)
		{
			Glade.XML xml = new Glade.XML(null,"gui.glade","databaseSaveDialog",null);
			xml.Autoconnect(this);
			
			databaseSaveDialog.Icon = parent.Icon;
			
			// Conectamos las acciones de los botones del diálogo.
			databaseSaveDialog.AddActionWidget(btnSave,ResponseType.Ok);
			databaseSaveDialog.AddActionWidget(btnCancel,ResponseType.Cancel);			
			
			
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
			
			databaseSaveDialog.AddFilter(filter2);		
			databaseSaveDialog.AddFilter(filter1);				
			databaseSaveDialog.AddFilter(filter3);	
		}
		
		#region Métodos públicos

		/// <summary>
		/// Este método muestra un cuadro de diálogo de guardado de base de
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
			DatabaseSaveDialog dialog = new DatabaseSaveDialog(parent);
			ResponseType res = (ResponseType)dialog.databaseSaveDialog.Run();
			filename = dialog.databaseSaveDialog.Filename;
			dialog.databaseSaveDialog.Destroy();
			
			return res;
		}
		
		
		#endregion Métodos públicos
	}
}
