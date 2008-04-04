// DatabaseFileInfo.cs created with MonoDevelop
// User: luis at 16:21 03/04/2008

using System;

using System.Xml.Serialization;

using MathTextLibrary.Databases;

namespace MathTextRecognizer.DatabaseManager
{
	
	/// <summary>
	/// Esta clase almacena la información con la ruta de una base de datos,
	/// para poder almacenarla como parte de la configuración de la aplicacion
	/// de reconocimiento.
	/// </summary>
	public class DatabaseFileInfo
	{
		private string path;
		private MathTextDatabase database;
		
		
		/// <summary>
		/// Constructor de <c>DatabaseFileInfo</c>.
		/// </summary>		
		public DatabaseFileInfo()
		{
			
		}
		
		/// <value>
		/// Contiene la base de datos del archivo de 
		/// base de datos representado por la instancia de <c>DatabaseFileInfo</c>
		/// </value>
		[XmlIgnoreAttribute]
		public MathTextDatabase Database 
		{
			get 
			{
				if (database == null)
				{
					// No tenemos base de datos, la cargamos
					database = MathTextDatabase.Load(this.path);
				}
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
		
		/// <summary>
		/// Implementa el comparador para las instancias de la clase.
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public override bool Equals (object o)
		{
			DatabaseFileInfo info = o as DatabaseFileInfo;
			if (info == null)
				return false;
			else
				return info.path == this.path;
		}

			
	}
}
