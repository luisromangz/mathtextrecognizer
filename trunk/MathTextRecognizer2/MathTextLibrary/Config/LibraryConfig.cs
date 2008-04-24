// SymbolsConfig.cs created with MonoDevelop
// User: luis at 12:15 17/04/2008

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;

using MathTextLibrary.Utils;

namespace MathTextLibrary.Config
{
	
	/// <summary>
	/// Esta clase almacena la configuracion de la aplicacion de reconocimiento.
	/// </summary>
	public class LibraryConfig
	{
		private List<SymbolLabelInfo> symbols;
		
		private static LibraryConfig config;
		
		public LibraryConfig()
		{
			symbols = new List<SymbolLabelInfo>();
		}
		
		/// <value>
		/// Contains the symbols used.
		/// </value>
		public List<SymbolLabelInfo> Symbols
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
		/// Contains the config class' instance.
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
		/// Loads the configuration file.
		/// </summary>
		private static void Load()
		{
						
			XmlSerializer serializer = new XmlSerializer(typeof(LibraryConfig),
			                                              GetSerializationOverrides());	
			
			string path = PathUtils.GetConfigFilePath("MathTextLibrary");
			
			Stream configStream;
			
			if(File.Exists(path))
			{			                                           
				configStream = new FileStream(path,FileMode.Open);
				
				// Deserializamos
				config = (LibraryConfig)serializer.Deserialize(configStream);					
				configStream.Close();
				
				return;
			}
			else
			{
				config = new LibraryConfig();
				
				
				config.Symbols.Add(new SymbolLabelInfo("√", @"\surd"));
				config.Symbols.Add(new SymbolLabelInfo("∑", @"\sum"));
				config.Symbols.Add(new SymbolLabelInfo("∏", @"\prod"));
				config.Symbols.Add(new SymbolLabelInfo("∫", @"\int"));
				config.Symbols.Add(new SymbolLabelInfo("∲", @"\oint"));
				config.Symbols.Add(new SymbolLabelInfo("⋂", @"\bigcap"));
				config.Symbols.Add(new SymbolLabelInfo("∩", @"\cap"));
				config.Symbols.Add(new SymbolLabelInfo("⋃", @"\bigcup"));
				config.Symbols.Add(new SymbolLabelInfo("∪", @"\cup"));			
				config.Symbols.Add(new SymbolLabelInfo("⋀", @"\bigwedge"));
				config.Symbols.Add(new SymbolLabelInfo("∧", @"\wedge"));
				config.Symbols.Add(new SymbolLabelInfo("⋁", @"\bigvee"));
				config.Symbols.Add(new SymbolLabelInfo("∨", @"\vee"));
				config.Symbols.Add(new SymbolLabelInfo("∊", @"\in"));
				config.Symbols.Add(new SymbolLabelInfo("∀", @"\forall"));
				config.Symbols.Add(new SymbolLabelInfo("∃", @"\exists"));
				config.Symbols.Add(new SymbolLabelInfo("∄", @"\nexists"));
				config.Symbols.Add(new SymbolLabelInfo("∞", @"\infty"));
				config.Symbols.Add(new SymbolLabelInfo("→", @"\rightarrow"));
				config.Symbols.Add(new SymbolLabelInfo("≈", @"\simeq"));
					
			
			}
			
			
			
		}
		
		/// <summary>
		/// Save the current configuration.
		/// </summary>
		public void Save()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(LibraryConfig),
			                                              GetSerializationOverrides());	
			string path = PathUtils.GetConfigFilePath("MathTextLibrary");
			
			using(StreamWriter w = new StreamWriter(path,false))
			{
				serializer.Serialize(w,this);
				w.Close();
			}
		}
		
		/// <summary>
		/// Creates a serialization profile.
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
	public class SymbolLabelInfo
	{
		private string label;
		private string symbol;
				
		/// <summary>
		/// <c>SymbolsInfo</c>'s default constructor.
		/// </summary>
		public SymbolLabelInfo()
		{
			
		}
		
		/// <summary>
		/// <c>SymbolsInfo</c>'s parametrized constructor.
		/// </summary>
		/// <param name="symbol">
		/// The symbol string.
		/// </param>
		/// <param name="label">
		/// The label string.
		/// </param>
		public SymbolLabelInfo(string symbol, string label)
		{
			this.symbol = symbol;
			this.label = label;
		}
		
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
