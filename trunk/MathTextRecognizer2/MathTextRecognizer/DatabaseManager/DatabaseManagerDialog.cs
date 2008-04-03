// DatabaseManager.cs created with MonoDevelop
// User: luis at 16:10 03/04/2008

using System;
using System.Collections.Generic;

using Gtk;

using CustomGtkWidgets.CommonDialogs;

using MathTextLibrary.Databases;

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
		private Dialog databaseManagerDialog;
		
		[Glade.WidgetAttribute]
		private Button removeBtn;
		
		[Glade.WidgetAttribute]
		private Button propertiesBtn;
		
		[Glade.WidgetAttribute]
		private TreeView databasesTV;
		
		[Glade.WidgetAttribute]
		private Button closeBtn;
		
		
#endregion Widgets Glade
		
		private List<DatabaseFileInfo> databaseFilesInfo; 
		
		protected DatabaseManagerDialog(Window parentWindow)
		{
			InitializeWidgets();
			
			databaseManagerDialog.TransientFor = parentWindow;
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
				databaseFilesInfo = value;
			}
		}
		
	
#endregion Propiedades
		
#region Metodos publicos
		
		/// <value>
		/// Permite recuperar las bases de datos referenciadas en el manager.
		/// </value>
		/// <returns>
		/// Una lista con las bases de datos.
		/// </returns>
		public List<MathTextDatabase> Databases
		{
			get
			{
				List<MathTextDatabase> databases = new List<MathTextDatabase>();
				
				foreach (DatabaseFileInfo info in databaseFilesInfo)
				{
					databases.Add(info.Database);
				}
				
				return databases;
			}
		}
		
		/// <summary>
		/// Muestra el manager de bases de datos.
		/// </summary>
		/// <returns>
		/// A <see cref="ResponseType"/>
		/// </returns>
		public static ResponseType Show(Window parentWindow)
		{
			DatabaseManagerDialog dialog =  
				new DatabaseManagerDialog(parentWindow);			
			
			return (ResponseType) (dialog.databaseManagerDialog.Run());
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
			// TODO Añadir la base de datos a la lista.
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
			
			this.databaseManagerDialog.AddActionWidget(closeBtn, 
			                                           ResponseType.Close);			
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
			this.databaseManagerDialog.Hide();
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
			
		}
		
		/// Maneja el evento producido al pulsar el boton guardar la selección
		/// como selección por defecto.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnMakeDefaultBtnClicked(object sender, EventArgs args)
		{
			
		}
		
#endregion Metodos privados
	}
}
