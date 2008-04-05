
using System;
using System.IO;
using System.Collections.Generic;

using Gtk;

using CustomGtkWidgets.CommonDialogs;

using MathTextLibrary.Utils;

namespace MathTextLearner.Assistant
{
	/// <summary>
	/// Esta clase implementa el panel que permite seleccionar
	/// imagenes en el asistente de creación de bases de datos.
	/// </summary>
	public class FileSelectionStep : PanelAssistantStep
	{
	
#region Controles de Glade
		
		[Glade.WidgetAttribute]
		private VBox fileRootWidget;
		
		[Glade.WidgetAttribute]
		private Button removeButton;
		
		[Glade.WidgetAttribute]
		private IconView filesIconView;

		
#endregion Controles de Glade
		
#region Atributos
		
		private ListStore fileStore;
		
#endregion Atributos
		
#region Constructor
		
		/// <summary>
		/// Crea una instancia del panel de seleccion de ficheros del
		/// asistente de creacion de nuevas basesd de datos de reconocimiento.
		/// </summary>
		/// <param name="assistant">
		/// El asistente al que pertenece el nuevo panel.
		/// </param>
		public FileSelectionStep(PanelAssistant assistant) 
			: base(assistant)
		{
			Glade.XML gxml =
				new Glade.XML(null,"databaseAssistant.glade","fileRootWidget",null);
				
			gxml.Autoconnect(this);
			
			SetRootWidget(fileRootWidget);
			
			InitializeWidgets();
		}
		
#endregion Constructor
		
	
#region Propiedades
		
		/// <value>
		/// Permite recuperar el contenedor de información acerca de las
		/// imagenes.
		/// </value>
		public ListStore ImagesStore
		{
			get
			{
				return fileStore;
			}
		}
		
		/// <value>
		/// Permite recuperar las imagenes añadidas.
		/// </value>
		public List<Gdk.Pixbuf> Images
		{
			get
			{
				List<Gdk.Pixbuf> res = new List<Gdk.Pixbuf>();
				foreach(object[] values in fileStore)
				{
					// Extremos la cadena de la ruta del modelo del iconview.
					string path = (string) (values[2]);
					
					Gdk.Pixbuf p = new Gdk.Pixbuf(path);
					
					res.Add(p);
				}
				
				return res;
			}
		}
		
#endregion Propiedades
		
#region Metodos privados
		/// <summary>
		/// Añade un icono representando un archivo de imagen a la vista
		/// de iconos del panel.
		/// </summary>
		/// <param name="fileName">
		/// La ruta del archivo que se añade al conjunto de archivos.
		/// </param>
		private void AddIcon(string fileName)
		{
			// Aqui añadimos un icono al IconView.
			
			// Creamos la imagen.
			Gdk.Pixbuf image = new Gdk.Pixbuf(fileName);
						
			image = ImageUtils.MakeThumbnail(image, 48);
			
			TreeIter iter = 
				fileStore.AppendValues(
					image, Path.GetFileName(fileName), fileName);
			
			filesIconView.ScrollToPath(fileStore.GetPath(iter));
			
			filesIconView.SelectPath(fileStore.GetPath(iter));
		}
		
		/// <summary>
		/// Muestra una venta de información para el archivo representado 
		/// por un icono de la vista de archivos.
		/// </summary>
		private void FilesIconViewItemActivated()
		{
			if(filesIconView.SelectedItems.Length > 0)
			{ 
				// Recuperamos el elemento del IconView que se ha seleccionado
				TreeIter iter;
				fileStore.GetIter(out iter, filesIconView.SelectedItems[0]);
				
				// Recuperamos el archivo asociado al icono
				string filepath = fileStore.GetValue(iter,2) as string;
				Gdk.Pixbuf p = new Gdk.Pixbuf(filepath);
				
				// Mostramos la ventana de informacióndel archivo
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
		
		/// <summary>
		/// Inicializa algunos controles del panel del asistente.
		/// </summary>
		private void InitializeWidgets()
		{
			// El control de vista de icono mostrará
			// La imagen, el nombre del fichero y tambien almacenará 
			// la ruta completa.
			// La asociación de estos campos en el IconView se define en
			// el propio archivo de Glade.
			fileStore = new ListStore(typeof(Gdk.Pixbuf), 
			                          typeof(string), 
			                          typeof(string));
				
			filesIconView.Model = fileStore;		
		}
				
		/// <summary>
		/// Maneja el uso del boton de añidir imagen.
		/// </summary>
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
		
		/// <summary>
		/// Maneja el uso del boton de añadir las imagenes de una carpeta.
		/// </summary>
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
		
		/// <summary>
		/// Maneja el evento de activación (doble click) de un icono.
		/// </summary>
		private void OnFilesIconViewItemActivated(object o, ItemActivatedArgs a)
		{
			FilesIconViewItemActivated();
		}
		
		/// <summary>
		/// Maneja el evento de cambio de la selección de un icono de la vista
		/// de iconos
		/// </summary>
		private void OnFilesIconViewSelectionChanged(object o, EventArgs a)
		{
			bool hasSelection = filesIconView.SelectedItems.Length > 0;
			removeButton.Sensitive = hasSelection;
		}
		
		/// <summary>
		/// Maneja el evento de uso del boton de eliminar un icono de la vista
		/// de iconos.
		/// </summary>
		private void OnRemoveButtonClicked(object o, EventArgs a)
		{
			TreeIter iter;
			fileStore.GetIter(out iter, filesIconView.SelectedItems[0]);
			
			// Primero pedimos confirmación
			if(ConfirmDialog.Show(
					this.Assistant.Window,
					"¿Desea eliminar la imagen «{0}» de la lista?",
					fileStore.GetValue(iter,1))
				== ResponseType.Yes)
			{
				fileStore.Remove(ref iter);
			}
		}
		
		/// <summary>
		/// Calcula los errores del paso del asistente;
		/// </summary>
		protected override void ComputeErrors ()
		{
			errors = "";
				
			if(fileStore.IterNChildren() == 0)
			{
				errors += "· Tienes que añadir archivos de imagen para ser procesados.\n";
			}
		}

		
#endregion Metodos privados
	}
}
