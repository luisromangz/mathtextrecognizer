
using System;

using Gtk;

namespace MathTextCustomWidgets.Dialogs
{
	/// <summary>
	/// Esta clase implementa un diálogo para seleccionar carpetas que es
	/// usado en las aplicaciones del proyecto.
	/// </summary>
	public class FolderOpenDialog
	{
		#region Atributos de glade
		
		[Glade.WidgetAttribute]
		private FileChooserDialog folderOpenDialog = null;
		
		[Glade.WidgetAttribute]
		private Button okButton = null;
		
		[Glade.WidgetAttribute]
		private Button cancelButton = null;
		
		#endregion Atributos de glade
	
		/// <summary>
		/// El constructor de la clase <c>ImageLoadDialog</c>.
		/// </summary>
		private FolderOpenDialog(Window parent)
		{
			Glade.XML gxml =
				new Glade.XML(null,"gui.glade","folderOpenDialog",null);
				
			gxml.Autoconnect(this);
			
			folderOpenDialog.TransientFor = parent;
			folderOpenDialog.Modal = true;
			
			folderOpenDialog.AddActionWidget(okButton, ResponseType.Ok);
			folderOpenDialog.AddActionWidget(cancelButton, ResponseType.Cancel);
		}	
		
		#region Métodos públicos
		
		/// <summary>
		/// Este método muestra un cuadro de diálogo de apertura de imagenes.
		/// </summary>
		/// <param name = "parent">
		/// La ventana desde la que se muestra el diálogo.
		/// </param>
		/// <param name = "filename">
		/// El nombre del archivo seleccionado.
		/// </param>
		public static ResponseType Show(Window parent, out string filename)
		{
			FolderOpenDialog dialog = new FolderOpenDialog(parent);
			ResponseType res = (ResponseType)dialog.folderOpenDialog.Run();
			filename = dialog.folderOpenDialog.Filename;
			dialog.folderOpenDialog.Destroy();
			
			return res;
		}
		
		#endregion Métodos públicos
	}
}