// DatabaseManager.cs created with MonoDevelop
// User: luis at 16:10 03/04/2008

using System;
using System.Collections.Generic;

using Gtk;
using Gdk;
using Glade;

using MathTextLibrary.Databases;

namespace MathTextRecognizer.DatabaseManager
{
	
	/// <summary>
	/// Esta clase representa el dialogo en el que se gestionan las bases de
	/// datos que se usarán en el reconocimiento.
	/// </summary>
	public class DatabaseManagerDialog
	{		
		private List<DatabaseFileInfo> databaseFilesInfo; 
		
		public DatabaseManagerDialog()
		{
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
#endregion Metodos publicos
	
	
		
#region Metodos privados
		
		
		
#endregion Metodos publicos
	}
}
