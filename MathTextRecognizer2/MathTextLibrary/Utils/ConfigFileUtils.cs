// ConfigFileUtils.cs created with MonoDevelop
// User: luis at 18:21 25/03/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.IO;

namespace MathTextLibrary.Utils
{
	
	/// <summary>
	/// Contiene métodos para manejar el uso de archivos de configuracion
	/// </summary>
	public class ConfigFileUtils
	{
		/// <summary>
		/// Este método permite recuperar la ruta del archivo de configuración
		/// de una aplicación dada.
		/// </summary>
		/// <param name="appName">
		/// El nombre de la aplicación.
		/// </param>
		/// <returns>
		/// La cadena que contiene la ruta del archivo de configuración de la
		/// aplicación.
		/// </returns>
		public static string GetConfigFilePath(string appName)
		{
			string path = 
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			
			return String.Format("{0}{1}{2}", 
			                     path, 
			                     Path.DirectorySeparatorChar,
			                     appName.ToLowerInvariant());
		}
	}
}
