// RecognizerConfig.cs created with MonoDevelop
// User: luis at 17:44 05/04/2008

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;

using MathTextLibrary.Utils;

using MathTextRecognizer.DatabaseManager;

namespace MathTextRecognizer.Config
{
	
	/// <summary>
	/// Esta clase almacena la configuracion de la aplicacion de reconocimiento.
	/// </summary>
	public class RecognizerConfig
	{
		private List<DatabaseFileInfo> databaseFilesInfo;
		
		private static RecognizerConfig config;
		
		public RecognizerConfig()
		{
			databaseFilesInfo = new List<DatabaseFileInfo>();
		}
		
		/// <value>
		/// Contiene la informacion para persistir de las bases de datos.
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
		
		/// <value>
		/// Permite recuperar la instancia de configuración.
		/// </value>
		public static RecognizerConfig Instance
		{
			get
			{
				if(config==null)
					Load();
				return config;
			}
		}
		
		/// <summary>
		/// Carga la configuracion desde el archivo de configuracion
		/// de la aplicacion.
		/// </summary>
		private static void Load()
		{
						
			XmlSerializer serializer = new XmlSerializer(typeof(RecognizerConfig),
			                                              GetSerializationOverrides());	
			
			string path = PathUtils.GetConfigFilePath("MathTextRecognizer");
			
			Stream configStream;
			
			if(File.Exists(path))
			{			                                           
				configStream = new FileStream(path,FileMode.Open);
			}
			else
			{
				// Cargamos la configuración por defecto, cargandola desde
				// un archivo de recursos.				
				Assembly runningAssembly =Assembly.GetExecutingAssembly();
				configStream = 
					runningAssembly.GetManifestResourceStream("defaultConfig");			
				
			}
			
			// Deserializamos
			config = (RecognizerConfig)serializer.Deserialize(configStream);					
			configStream.Close();
			
		}
		
		/// <summary>
		/// Guarda la configuracion.
		/// </summary>
		public void Save()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(RecognizerConfig),
			                                              GetSerializationOverrides());	
			string path = PathUtils.GetConfigFilePath("MathTextRecognizer");
			
			using(StreamWriter w = new StreamWriter(path,false))
			{
				serializer.Serialize(w,this);
				w.Close();
			}
		}
		
		/// <summary>
		/// Crea el perfil para poder serializar la configuracion.
		/// </summary>
		/// <returns>
		/// A <see cref="XmlSerializationOverrides"/>
		/// </returns>
		private static XmlAttributeOverrides GetSerializationOverrides()
		{
			// En principio nada.
			XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
		
			
			return attrOverrides;
		}
	}
}
