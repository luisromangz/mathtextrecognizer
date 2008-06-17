// DatabaseManager.cs created with MonoDevelop
// User: luis at 16:10 03/04/2008

using System;
using System.IO;
using System.Collections.Generic;

using Gtk;

using MathTextCustomWidgets;
using MathTextCustomWidgets.Dialogs;

using MathTextLibrary.Databases;

using MathTextRecognizer.Config;

namespace MathTextRecognizer.DatabaseManager
{
	
	/// <summary>
	/// Esta clase representa el dialogo en el que se gestionan las bases de
	/// datos que se usarán en el reconocimiento.
	/// </summary>
	public class DatabaseManagerDialog
	{		
		
#region Widgets Glade
		[Glade.WidgetAttribute]
		private Dialog databaseManagerDialog = null;
		
		[Glade.WidgetAttribute]
		private Button removeBtn = null;
		
		[Glade.WidgetAttribute]
		private Button propertiesBtn = null;
		
		[Glade.WidgetAttribute]
		private TreeView databasesTV = null;
		
		[Glade.WidgetAttribute]
		private Button closeBtn = null;
		
		
#endregion Widgets Glade
		
		private List<DatabaseFileInfo> databaseFilesInfo; 
		
		private ListStore databasesLS;
		
		public event EventHandler DatabaseListChanged;
		
		public DatabaseManagerDialog(Window parentWindow)
		{
			InitializeWidgets();
			
			databaseManagerDialog.TransientFor = parentWindow;
			
			databaseFilesInfo =  new List<DatabaseFileInfo>();
			
			databasesLS.RowDeleted += 
				new RowDeletedHandler(OnDatabasesLSRowsChanged);
			databasesLS.RowInserted +=
				new RowInsertedHandler(OnDatabasesLSRowsChanged);
			
			
			this.DatabaseFilesInfo =
				Config.RecognizerConfig.Instance.DatabaseFilesInfo;
		}
		
#region Propiedades
		
		/// <value>
		/// Contiene la información sobre las bases de datos
		/// gestionadas en el manager.
		/// </value>
		public List<DatabaseFileInfo> DatabaseFilesInfo
		{			
			get
			{
				return databaseFilesInfo;
			}
			set
			{
				databaseFilesInfo.Clear();
				foreach(DatabaseFileInfo info in value)
				{
					AddDatabaseInfo(info);
				}
			}
		}
		
		
		
	
#endregion Propiedades
		
#region Metodos publicos
		
		
		
		/// <summary>
		/// Muestra el dialogo, esperando a terminar la funcion a que se devuelva
		/// un valor.
		/// </summary>
		/// <returns>
		/// La respuesta del dialogo.
		/// </returns>
		public ResponseType Show()
		{
			return (ResponseType)(this.databaseManagerDialog.Run());
		}
		
		
		public void Destroy()
		{
			databaseManagerDialog.Destroy();
		}
		
	
		
#endregion Metodos publicos
	
	
		
#region Metodos privados

		/// <summary>
		/// Añade una base de datos a la lista.
		/// </summary>
		/// <param name="databasePath">
		/// La ruta de la base de datos.
		/// </param>
		private void AddDatabase(string databasePath)
		{
			
			MathTextDatabase database = MathTextDatabase.Load(databasePath);
			if(database == null)
			{
				// No se abrio un archivo de base de datos, informamos.
				OkDialog.Show(this.databaseManagerDialog,
				              MessageType.Warning,
				              "El archivo «{0}» no contiene una base de datos correcta, y no se pudo abrir.",
				              databasePath);
				return;
			}
			
			DatabaseFileInfo databaseInfo = new DatabaseFileInfo();
			databaseInfo.Database = database;
			databaseInfo.Path = databasePath;
			
			if(!databaseFilesInfo.Contains(databaseInfo))
			{
				
				// Lo añadimos a la coleccion.
				databaseFilesInfo.Add(databaseInfo);
				
				TreeIter newItem =
					databasesLS.AppendValues(Path.GetFileName(databasePath),
					                         database.DatabaseTypeShortDescription,
					                         databasePath,
					                         databaseInfo);
				
				// Seleccionamos la fila añadida.
				databasesTV.Selection.SelectIter(newItem);
				TreePath newPath = databasesLS.GetPath(newItem);
				databasesTV.ScrollToCell(newPath,
				                         databasesTV.Columns[0],
				                         true,
				                         1.0f,
				                         0.0f);
				
				
			}
			else
			{
				OkDialog.Show(this.databaseManagerDialog,
				              MessageType.Info,
				              "La base de datos del fichero «{0}» ya se encuentra en la lista y no se añadirá de nuevo.",
				              Path.GetFileName(databasePath));
			}
		}
		
		/// <summary>
		/// Añade la informacion de una base de datos a la lista.
		/// </summary>
		/// <param name="databaseInfo">
		/// La informacion de base de datos a añadir.
		/// </param>
		private void AddDatabaseInfo(DatabaseFileInfo databaseInfo)
		{
			if (databaseInfo.Database == null)
			{
				// No se abrio un archivo de base de datos, informamos.
				OkDialog.Show(this.databaseManagerDialog,
				              MessageType.Warning,
				              "El archivo «{0}» no existe o no contiene una base de datos correcta, y no se pudo abrir.",
				              databaseInfo.Path);
				return;
			}
			
			databaseFilesInfo.Add(databaseInfo);
			databasesLS.AppendValues(Path.GetFileName(databaseInfo.Path),
			                         databaseInfo.Database.DatabaseTypeShortDescription,
			                         databaseInfo.Path,
			                         databaseInfo);  
			
			
		}
		
		
		
		
		/// <summary>
		/// Inicializa los controles del dialogo.
		/// </summary>
		private void InitializeWidgets()
		{
			// Nos traemos el Glade
			Glade.XML gladeXML = 
				Glade.XML.FromAssembly("mathtextrecognizer.glade",
				                       "databaseManagerDialog",
				                       null);
			
			gladeXML.Autoconnect(this);
			
			databaseManagerDialog.AddActionWidget(closeBtn, ResponseType.Close);	
			
			// Creamos el modelo de la lista de bases de datos,
			// para que contenga el tipo de la base de datos, el archivo que
			// la contiene, y el objecto DatabaseFileInfo asociado.
			databasesLS =  new ListStore(typeof(string),
			                             typeof(string), 
			                             typeof(string),
			                             typeof(DatabaseFileInfo));
			databasesTV.Model = databasesLS;
			
			
			databasesTV.AppendColumn ("Archivo", 
			                          new CellRendererText (), 
			                          "text", 
			                          0);
			
			databasesTV.AppendColumn ("Tipo", 
			                          new CellRendererText (), 
			                          "text", 
			                          1);			
			
			databasesTV.Columns[0].Sizing = TreeViewColumnSizing.Autosize;
			
			databasesTV.Selection.Changed += 
				new EventHandler(OnDatabasesTVSelectionChanged);
			
			databaseManagerDialog.Icon = ImageResources.LoadPixbuf("database16");			
		}
		
		/// <summary>
		/// Maneja el evento producido al pulsar el boton de añadir 
		/// una base de datos.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnAddBtnClicked(object sender, EventArgs args)
		{
			string filename;	
			
			if(DatabaseOpenDialog.Show(this.databaseManagerDialog, out filename)
				== ResponseType.Ok)
			{				
				// Añadimos la base de datos a la lista.
				AddDatabase(filename);
			}
		}
		
		/// <summary>
		/// Maneja el evento producido al pulsar el boton de cerrar el dialogo.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnCloseBtnClicked(object sender, EventArgs args)
		{
			RecognizerConfig.Instance.DatabaseFilesInfo=DatabaseFilesInfo;
			RecognizerConfig.Instance.Save();
			
			databaseManagerDialog.Respond(ResponseType.Ok);
		}
		
		/// Maneja el evento producido al pulsar el boton de ver las propiedades 
		/// una base de datos.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnPropertiesBtnClicked(object sender, EventArgs args)
		{
			DatabaseInfoDialog dialog =
				new DatabaseInfoDialog(this.databaseManagerDialog);
			
			// We get the selected database and show its properties.
			TreeIter selected;
			databasesTV.Selection.GetSelected(out selected);
			DatabaseFileInfo info = 
				(DatabaseFileInfo)(databasesLS.GetValue(selected, 3));
			dialog.SetDatabase(info.Database);
			dialog.Show();
			dialog.Destroy();
		}
		
		/// Maneja el evento producido al pulsar el boton de añadir 
		/// una base de datos.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnRemoveBtnClicked(object sender, EventArgs args)
		{
			TreeIter selectedIter;
			databasesTV.Selection.GetSelected(out selectedIter);

			// Recuperamos la informacion y la eliminamos de la lista y el modelo.
			DatabaseFileInfo databaseInfo = 
				(DatabaseFileInfo) (databasesLS.GetValue(selectedIter, 3));

			databaseFilesInfo.Remove(databaseInfo);
			databasesLS.Remove(ref selectedIter);
			
		}
		
		
		/// <summary>
		/// Maneja el cambio de las filas de la lista de bases de datos.
		/// </summary>
		/// <param name="?">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnDatabasesLSRowsChanged(object sender, EventArgs args)
		{
			if(DatabaseListChanged!=null)
				DatabaseListChanged(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Maneja el cambio en la seleccion de la lista.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnDatabasesTVSelectionChanged(object sender, EventArgs args)
		{
			// Cambiamos la sensitividad de los botones.
			bool buttonsSensitive = databasesTV.Selection.CountSelectedRows() > 0;
			removeBtn.Sensitive = buttonsSensitive;
			propertiesBtn.Sensitive = buttonsSensitive;
		}
			
		
#endregion Metodos privados
	}
}
