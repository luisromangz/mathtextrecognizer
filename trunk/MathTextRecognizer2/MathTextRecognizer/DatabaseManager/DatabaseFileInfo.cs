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
		/// Permite recuperar la base de datos contenida en el archivo de 
		/// base de datos representado por la instancia de <c>DatabaseFileInfo</c>
		/// </value>		
		public MathTextDatabase Database 
		{
			get 
			{
				return database;
			}
		}

		/// <value>
		/// Permite establecer y recuperar la ruta de la base de datos.
		/// </value>
		public string Path
		{
			get
			{
				return path;
			}
		}
	}
}
