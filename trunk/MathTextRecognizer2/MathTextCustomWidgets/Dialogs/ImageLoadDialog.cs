
using System;

using Gtk;

namespace MathTextCustomWidgets.Dialogs
{
	/// <summary>
	/// Esta clase implementa un diálogo para cargar imágenes que es
	/// usado en todas las aplicaciones del proyecto.
	/// </summary>
	public class ImageLoadDialog
	{
		#region Atributos de glade
		
		[Glade.WidgetAttribute]
		private FileChooserDialog imageLoadDialog;
		
		[Glade.WidgetAttribute]
		private Button okButton;
		
		[Glade.WidgetAttribute]
		private Button cancelButton;
		
		#endregion Atributos de glade
	
		/// <summary>
		/// El constructor de la clase <c>ImageLoadDialog</c>.
		/// </summary>
		private ImageLoadDialog(Window parent)
		{
			Glade.XML gxml =
				new Glade.XML(null,"gui.glade","imageLoadDialog",null);
				
			gxml.Autoconnect(this);
			
			imageLoadDialog.TransientFor = parent;
			imageLoadDialog.Modal = true;
			
			imageLoadDialog.AddActionWidget(okButton, ResponseType.Ok);
			imageLoadDialog.AddActionWidget(cancelButton, ResponseType.Cancel);
			
			FileFilter filter0=new FileFilter();
			filter0.AddPattern("*.png");
			filter0.AddPattern("*.PNG");
			filter0.Name="Imagen PNG";
			
			FileFilter filter1=new FileFilter();
			filter1.AddPattern("*.jpg");
			filter1.AddPattern("*.JPG");
			filter1.Name="Imagen JPG";
			
			FileFilter filter2 = new FileFilter();
			filter2.AddPattern("*.*");
			filter2.Name = "Todos los archivos";
			
			FileFilter filter4=new FileFilter();
			filter4.AddPattern("*.png");
			filter4.AddPattern("*.PNG");
			filter4.AddPattern("*.jpg");
			filter4.AddPattern("*.JPG");
			filter4.Name="Imágenes soportadas";
				
			imageLoadDialog.AddFilter(filter4);	
			imageLoadDialog.AddFilter(filter0);
			imageLoadDialog.AddFilter(filter1);
			imageLoadDialog.AddFilter(filter2);			
			
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
			ImageLoadDialog dialog = new ImageLoadDialog(parent);
			ResponseType res = (ResponseType)dialog.imageLoadDialog.Run();
			filename = dialog.imageLoadDialog.Filename;
			dialog.imageLoadDialog.Destroy();
			
			return res;
		}
		
		#endregion Métodos públicos
	}
}