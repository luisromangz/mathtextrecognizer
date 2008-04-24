
using System;

using Gtk;


namespace MathTextCustomWidgets.Widgets.Logger
{
	
	/// <summary>
	/// Esta clase implementa el diálogo usado para guardar los registros mostrados
	/// en la ventana de log.
	/// </summary>
	public class LogSaveDialog
	{
#region Glade Widgets
		[Glade.WidgetAttribute]
		private FileChooserDialog logSaveDialog = null;
		
		[Glade.WidgetAttribute]
		private Button btnSave = null;
		
		[Glade.WidgetAttribute]
		private Button btnCancel = null;
		
#endregion Glade Widgets
		
	
		/// <summary>
		/// Constructor de la clase LogSaveDialog. Crea y muestra el diálogo.
		/// </summary>
		public LogSaveDialog()
		{
			Glade.XML gxml = new Glade.XML (null,"gui.glade", "logSaveDialog", null);
            gxml.Autoconnect (this);
            
            FileFilter logFilter = new FileFilter();
            logFilter.Name = "Archivo de registro";
            logFilter.AddPattern("*.log");
            logFilter.AddPattern("*.LOG");
            
            FileFilter txtFilter = new FileFilter();
            txtFilter.Name = "Archivo de texto";
            txtFilter.AddPattern("*.txt");
            txtFilter.AddPattern("*.TXT");
            
            FileFilter allFilter = new FileFilter();
            allFilter.Name = "Todos los archivos";
            allFilter.AddPattern("*.*");          
            
            logSaveDialog.AddFilter(logFilter);
            logSaveDialog.AddFilter(txtFilter);
            logSaveDialog.AddFilter(allFilter);
            
            logSaveDialog.AddActionWidget(btnSave,ResponseType.Ok);
            logSaveDialog.AddActionWidget(btnCancel,ResponseType.Cancel); 
		}
		
#region Propiedades
		
		/// <value>
		/// Esta propiedad permite recuperar el nombre de archivo seleccionado para guardar.
		/// </value>
		public string Filename
		{
			get
			{
				return logSaveDialog.Filename;
			}
		}
		
#endregion Propiedades
		
#region Métodos públicos
		
		/// <summary>
		/// Este método destruye el cuadro de diálogo, cerrandolo.
		/// </summary>
		public void Destroy()
		{
			logSaveDialog.Destroy();
		}
		
		/// <summary>
		/// Este método permite esperar al diálogo hasta que sea cerrado o emita 
		/// una respuesta.
		/// </summary>
		public ResponseType Run()
		{
			return (ResponseType)(logSaveDialog.Run());
		}		
		
#endregion
		
		
	}
}
