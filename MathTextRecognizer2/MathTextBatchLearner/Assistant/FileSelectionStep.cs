
using System;
using System.IO;

using Gtk;

using CustomGtkWidgets.CommonDialogs;

using MathTextBatchLearner.Assistant;

namespace MathTextBatchLearner.Assistant
{
	/// <summary>
	/// Esta clase implementa el panel que permite seleccionar
	/// imagenes en el asistente de creación de bases de datos.
	/// </summary>
	public class FileSelectionStep : PanelAssistantStep
	{
	
		#region Controles de Glade
		
		[Glade.WidgetAttribute]
		private Frame fileSelectionStepFrame;
		
		[Glade.WidgetAttribute]
		private Button removeButton;
		
		[Glade.WidgetAttribute]
		private IconView filesIconView;			
		
		#endregion Controles de Glade
		
		#region Atributos
		
		private ListStore fileStore;
		
		#endregion Atributos
		
		#region Constructor
		
		public FileSelectionStep(PanelAssistant assistant) 
			: base(assistant)
		{
			Glade.XML gxml =
				new Glade.XML(null,"gui.glade","fileSelectionStepFrame",null);
				
			gxml.Autoconnect(this);
			
			SetRootWidget(fileSelectionStepFrame);
			
			InitializeWidgets();
		}
		
		#endregion Constructor
		
		#region Metodos públicos
		
		public override bool HasErrors ()
		{
			errors = "";
			
			if(fileStore.IterNChildren() == 0)
			{
				errors += "· Debe añadir archivos de imagen para ser procesados.\n";
			}
			
			
			return errors.Length > 0;
		}
		
		#endregion Metodos públicos
		
		#region Metodos privados
		
		private void AddIcon(string fileName)
		{
			// Aqui añadimos un icono al IconView.
			
			// Creamos la imagen.
			Gdk.Pixbuf image = new Gdk.Pixbuf(fileName);
			float scaleX, scaleY;
			
			// La escalamos para que no se distorsione.
			if(image.Width > image.Height)
			{
				scaleX = 1;
				scaleY = (float)(image.Height)/image.Width;
			}
			else
			{
				scaleX = (float)(image.Width)/image.Height;
				scaleY = 1;
			}
			
			image = image.ScaleSimple(
				(int)(48*scaleX), (int)(48*scaleY), Gdk.InterpType.Bilinear);
			
			TreeIter iter = 
				fileStore.AppendValues(
					image, Path.GetFileName(fileName), fileName);
			
			filesIconView.ScrollToPath(fileStore.GetPath(iter));
			
			filesIconView.SelectPath(fileStore.GetPath(iter));
		}
		
		private void FilesIconViewItemActivated()
		{
			if(filesIconView.SelectedItems.Length > 0)
			{ 
				TreeIter iter;
				fileStore.GetIter(out iter, filesIconView.SelectedItems[0]);
				
				string filepath = fileStore.GetValue(iter,2) as string;
				Gdk.Pixbuf p = new Gdk.Pixbuf(filepath);
				
				
				
				OkDialog.Show(
					this.Assistant.Window,
					MessageType.Info,
					"Información de «{0}»:\n\n"
					+"· Ruta del archivo: {1}\n"
					+". Tamaño de la imagen: "+p.Width+" x "+p.Height+"\n",
					fileStore.GetValue(iter,1) as string,
					Path.GetDirectoryName(filepath));
			}
		}
		
		private void InitializeWidgets()
		{
			// El control de vista de icono mostrará
			// La imagen, el nombre del fichero y tambien almacenará 
			// la ruta completa.
			fileStore = 
				new ListStore(typeof(Gdk.Pixbuf), typeof(string), typeof(string));
				
			filesIconView.Model = fileStore;
			filesIconView.PixbufColumn = 0;
			filesIconView.TextColumn = 1;
			
		}
				
		private void OnAddImageButtonClicked(object o, EventArgs a)
		{
			// Añadiremos una imagen a la lista.
			string filename;
			if(ImageLoadDialog.Show(Assistant.Window, out filename)
				== ResponseType.Ok)
			{
				AddIcon(filename);
			}
		}
		
		private void OnAddFolderButtonClicked(object o, EventArgs a)
		{
			// Selccionamos la carpeta
			string folderPath;
			if(FolderOpenDialog.Show(this.Assistant.Window, out folderPath)
				== ResponseType.Ok)
			{
				string [] files = Directory.GetFiles(folderPath);
				
				int added = 0;
				
				// Recorremos los ficheros que hay en la carpeta
				foreach(string file in files)
				{
					// Lo ponemos en minuscula para facilitar la comparacion.
					string ext = Path.GetExtension(file).ToLower();
					if(	ext == ".png"
						|| ext == ".jpg")
					{
						// Si es png o jpg intentamos añadirlo.
						try
						{
							AddIcon(file);
							added++;
						}
						catch(Exception)
						{
							// Si peta, el fichero tenia una extensión que no
							// hacia honor a su contenido.
						}
					}
				} 
				
				if(added > 0)
				{
					// Decimos el número de archivos que hemos añadido.
					OkDialog.Show(
						this.Assistant.Window,
						MessageType.Info,
						"Se añidieron {0} archivos(s) de imagen.",
						added);
				}
				else
				{
					// Nos quejamos si no pudimos añadir ningún fichero.
					OkDialog.Show(
						this.Assistant.Window,
						MessageType.Warning,
						"No se encotró ningún archivo de imagen válido en la"
						+" carpeta seleccionada",
						added);
				}
			}			
		}
		
		private void OnFilesIconViewItemActivated(object o, ItemActivatedArgs a)
		{
			FilesIconViewItemActivated();
		}
		
		private void OnFilesIconViewSelectionChanged(object o, EventArgs a)
		{
			removeButton.Sensitive = filesIconView.SelectedItems.Length > 0;
		}
		
		private void OnRemoveButtonClicked(object o, EventArgs a)
		{
			TreeIter iter;
			fileStore.GetIter(out iter, filesIconView.SelectedItems[0]);
			
			// Primero pedimos confirmación
			if(ConfirmDialog.Show(
					this.Assistant.Window,
					"¿Desea eliminar de la lista la imagen\n«{0}»?",
					fileStore.GetValue(iter,2))
				== ResponseType.Yes)
			{
				fileStore.Remove(ref iter);
			}
		}
		
		#endregion Metodos privados
	}
}
