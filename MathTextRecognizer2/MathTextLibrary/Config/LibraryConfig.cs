// SymbolsConfig.cs created with MonoDevelop
// User: luis at 12:15 17/04/2008

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;

using MathTextLibrary.Utils;

using MathTextRecognizer.DatabaseManager;

namespace MathTextLibrary.Config
{
	
	/// <summary>
	/// Esta clase almacena la configuracion de la aplicacion de reconocimiento.
	/// </summary>
	public class LibraryConfig
	{
		private List<SymbolInfo> symbols;
		
		private static LibraryConfig config;
		
		public LibraryConfig()
		{
			symbols = new List<SymbolInfo>();
		}
		
		/// <value>
		/// Contiene la informacion para persistir de las bases de datos.
		/// </value>
		public List<SymbolInfo> Symbols
		{
			get
			{
				return symbols;
			}
			set
			{
				symbols = value;
			}
		}
		
		/// <value>
		/// Permite recuperar la instancia de configuración.
		/// </value>
		public static LibraryConfig Instance
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
			
			string path = ConfigFileUtils.GetConfigFilePath("MathTextLibrary");
			
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
			string path = ConfigFileUtils.GetConfigFilePath("MathTextLibrary");
			
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
	
	/// <summary>
	/// This class implements a structure for storage of symbols and its labels.
	/// </summary>
	public class SymbolsInfo
	{
		private string label;
		private string symbol;
		
		/// <value>
		/// Contains a symbols label.
		/// </value>
		public string Label
		{
			get
			{
				return label;
			}
			set
			{
				label = value;
			}
		}
		
		/// <value>
		/// Contains a symbol's symbol string.
		/// </value>
		public string Symbol 
		{
			get 
			{
				return symbol;
			}
			set 
			{
				symbol = value;
			}
		}
	}
}
