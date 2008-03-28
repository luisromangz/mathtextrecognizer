// LearnerConfig.cs created with MonoDevelop
// User: luis at 18:44 28/03/2008

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;

using MathTextLibrary.Utils;
using MathTextLibrary.BitmapProcesses;

namespace MathTextLearner.Config
{
	
	/// <summary>
	/// Esta clase representa la estructura donde se almacena la configuracion
	/// de la aplicacion.
	/// </summary>
	public class LearnerConfig
	{
		private List<BitmapProcess> processes;
		
		private static LearnerConfig config;
		
		public LearnerConfig()
		{
			processes = new List<BitmapProcess>();
		}
		
		/// <summary>
		/// Permite recuperar y establecer los algoritmos de procesado por
		/// defecto de la aplicacion.
		/// </summary>		
		public List<BitmapProcess> DefaultProcesses
		{
			get
			{
				return processes;
			}
			
			set
			{
				processes = value;
			}
		}
		
		
		
		/// <summary>
		/// Permite recuperar la instancia de configuración.
		/// </summary>
		public static LearnerConfig Instance
		{
			get
			{
				if(config==null)
					Load();
				return config;
			}
		}
		
		/// <summary>
		/// Crea la configuración por defecto para el programa 
		/// </summary>
		private static void CreateDefaultConfig()
		{
			// TODO Crear la configuracion inicial, de verdad.
			config = new LearnerConfig();
			
			config.DefaultProcesses.Add(new BitmapFixedThresholder());
			config.Save();
		}
		
		/// <summary>
		/// Carga la configuracion desde el archivo de configuracion
		/// de la aplicacion.
		/// </summary>
		private static void Load()
		{
						
			XmlSerializer serializer = new XmlSerializer(typeof(LearnerConfig),
			                                              GetSerializationOverrides());	
			
			string path = ConfigFileUtils.GetConfigFilePath("MathTextLearner");
			
			if(File.Exists(path))
			{			                                           
				using(StreamReader r = new StreamReader(path))
				{
					config = (LearnerConfig)serializer.Deserialize(r);
					r.Close();
				}
			}
			else
			{
				CreateDefaultConfig();
			}
		}
		
		/// <summary>
		/// Guarda la configuracion.
		/// </summary>
		public void Save()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(LearnerConfig),
			                                              GetSerializationOverrides());	
			string path = ConfigFileUtils.GetConfigFilePath("MathTextLearner");
			
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
			XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
			XmlAttributes attrs = new XmlAttributes();
			
			XmlElementAttribute attr;
							
			Assembly ass = Assembly.GetAssembly(typeof(BitmapProcess));
			foreach(Type t in ass.GetTypes())
			{
				if(t.BaseType == typeof(BitmapProcess))
				{
					attr = new XmlElementAttribute();
					attr.ElementName = t.Name;
					attr.Type = t;
					
					attrs.XmlElements.Add(attr);	
					 
				}
			}
			attrOverrides.Add(typeof(LearnerConfig), "DefaultProcesses", attrs);
			
			return attrOverrides;
		}
		
	}
}
