// DatabaseFileInfo.cs created with MonoDevelop
// User: luis at 16:21 03/04/2008

using System;

using MathTextLibrary.Databases;

namespace MathTextRecognizer.DatabaseManager
{
	
	
	public class DatabaseFileInfo
	{
		private string path;
		private MathTextDatabase database;
		
		
		
		public DatabaseFileInfo()
		{
		}
		
		/// <value>
		/// Contiene la base de datos del archivo de 
		/// base de datos representado por la instancia de <c>DatabaseFileInfo</c>
		/// </value>		
		public MathTextDatabase Database 
		{
			get 
			{
				return database;
			}
			set
			{
				database = value;
			}
		}

		/// <value>
		/// Contiene la ruta de la base de datos.
		/// </value>
		public string Path
		{
			get
			{
				return path;
			}
			set
			{
				path = value;
			}
		}
	}
}
