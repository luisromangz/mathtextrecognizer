using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;
using MathTextLibrary.BitmapProcesses;

namespace MathTextLibrary.Databases
{
	
	/// <summary>
	/// Esta clase encapsula las distintas implementaciones de bases de datos
	/// de caracteres, definiendo además los parámetros de procesado de imagenes
	/// que se usaron al definir dicha base de datos, y que por lo tanto es necesario
	/// aplicar a las imagenes a reconocer para obtener resultados consistentes.
	/// </summary>	
	public class MathTextDatabase
	{	
#region Eventos 
		
		/// <summary>
		/// Este evento se lanza para indicar que se completado un paso
		/// mientras se esta aprendiendo un caracter en la base de datos.
		/// </summary>
		public event ProcessingStepDoneHandler StepDone;
		
	
		
#endregion Eventos
		
#region Atributos
		
		private List<BitmapProcess> processes;
		
		private DatabaseBase database;
		
		private string description;
		private string shortDescription;
		
#endregion Atributos
		
#region Constructores
		public MathTextDatabase()
		{
			
		}	
		
		public MathTextDatabase(DatabaseBase database)
		{
			SetDatabase(database);
		}	
		
#endregion Constructores;
		
				
#region Propiedades
		
		/// <value>
		/// Contiene la lista de los
		/// procesos de imagenes usados en la base de datos.
		/// </value>
		public List<BitmapProcess> Processes 
		{
			get {
				return processes;
			}
			
			set{
				processes = value;
			}
		}		

		/// <value>
		/// Contiene la base de datos de información de
		/// caracteres asociada a este objeto.
		/// </value>
		public DatabaseBase Database
		{
			get {
				return database;
			}
			
			set {
				SetDatabase(value);				
			}
		}

		/// <value>
		/// Contiene la descripcion de la base de datos.
		/// </value>
		public string Description 
		{
			get 
			{
				return description;
			}
			set
			{
				description = value;
			}
		}
		
		/// <value>
		/// Contiene la descripcion del tipo de base de datos usada.
		/// </value>
		public string  DatabaseTypeDescription
		{
			get
			{
				Type databaseType = database.GetType();
				object[] attributes = 
					databaseType.GetCustomAttributes(typeof(DatabaseTypeInfo),
					                                 true);
			
				DatabaseTypeInfo info = (DatabaseTypeInfo)attributes[0];
				return info.Description;
			}
		}
		
		/// <value>
		/// Contiene la descripción corta del tipo de base de datos usada.
		/// </value>
		public string  DatabaseTypeShortDescription
		{
			get
			{
				Type databaseType = database.GetType();
				object[] attributes = 
					databaseType.GetCustomAttributes(typeof(DatabaseTypeInfo),
					                                 true);
			
				DatabaseTypeInfo info = (DatabaseTypeInfo)attributes[0];
				return info.ShortDescription;
			}
		}
		
		/// <value>
		/// Contiene los simbolos almacenados en la base de datos.
		/// </value>
		[XmlIgnoreAttribute]
		public List<MathSymbol> SymbolsContained
		{
			get
			{
				return database.SymbolsContained;
			}
		}

		/// <value>
		/// Contains a short description for the database.
		/// </value>
		public string ShortDescription 
		{
			get 
			{
				return shortDescription;
			}
			set 
			{
				shortDescription = value;
			}
		}
	
		
#endregion Propiedades
		
#region Metodos publicos

		/// <summary>
		/// Con este metodos almacenamos un nuevo simbolo en la base de
		/// datos.
		/// 
		/// Lanza <c>DuplicateSymbolException</c> si ya hay un simbolo con las
		/// mismas propiedades y etiqueta en la base de datos.
		/// </summary>
		/// <param name="bitmap">
		/// La imagen que queremos añadir a la base de datos.
		/// </param>
		/// <param name="symbol">
		/// El simbolo que representa a la imagen.
		/// </param>
		public bool Learn(MathTextBitmap bitmap,MathSymbol symbol)
		{
			return database.Learn(bitmap,symbol);
		}
		
		/// <summary>
		/// Permite abrir un fichero xml en el que esta almacenada la base de
		/// datos.
		/// </summary>
		/// <param name="path">
		/// La ruta del archivo que queremos cargar.
		/// </param>
		public static MathTextDatabase Load(string path)
		{
			XmlSerializer serializer=
				new XmlSerializer(typeof(MathTextDatabase),
				                  MathTextDatabase.GetXmlAttributeOverrides());	
			
			MathTextDatabase db = null;
			StreamReader r = null;
			
		
			try
			{
				r = new StreamReader(path);
			}
			catch(Exception)
			{
				return null;
			}
				
			                                           
			try
			{
				db = (MathTextDatabase)serializer.Deserialize(r);
				
			}
			catch(System.Xml.XmlException)
			{
					// Nada.
			}
			
			r.Close();
			
			return db;
		}
			
		/// <summary>
		/// Reconoce una imagen dada una imagen.
		/// </summary>
		/// <param name="image">
		/// La imagen a intentar asociar al caracter.
		/// </param>
		/// <returns>
		/// Los símbolos que se han podido asociar a la imagen.
		/// </returns>
		public List<MathSymbol> Recognize(MathTextBitmap image)
		{
			return database.Match(image);
		}
		
		
		/// <summary>
		/// Este metodo almacena la base de datos en el disco duro.
		/// </summary>
		/// <param name="path">
		/// La ruta del archivo en la que se guardara la base de datos.
		/// </param>
		public void Save(string path)
		{					
			
			
			XmlSerializer serializer=
				new XmlSerializer(typeof(MathTextDatabase),
				                  MathTextDatabase.GetXmlAttributeOverrides());			
			
			using(StreamWriter w=new StreamWriter(path))
			{			
				serializer.Serialize(w,this);
				w.Close();
			}
		}
		
#endregion Metodos publicos
		
#region Metodos protegidos
			
		/// <summary>
		/// This method relauches the children database's <c>StepDone</c> event.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="ProcessingStepDoneArgs"/>
		/// </param>
		protected void OnStepDone(object sender, StepDoneArgs arg)
		{
			if(StepDone != null)
			{
				StepDone(this,arg);
			}		
		}
	
#endregion Metodos protegidos
		
#region Metodos privados
		
		/// <summary>
		/// Recupera los tipos de bases de datos disponibles.
		/// </summary>
		/// <returns>
		/// Una lista con los tipos de bases de datos.
		/// </returns>
		private static List<Type> RetrieveDatabaseTypes()
		{
			List<Type> types = new List<Type>();
			Assembly ass = Assembly.GetAssembly(typeof(DatabaseBase));
			
			foreach(Type bpt in ass.GetTypes())
			{
				if(bpt.BaseType == typeof(DatabaseBase))
				{
					types.Add(bpt);
				}
			}
			
			return types;
		}
				
		private static List<Type> RetrieveProcessesTypes()
		{
			List<Type> types = new List<Type>();
			Assembly ass = Assembly.GetAssembly(typeof(BitmapProcess));
			
			foreach(Type bpt in ass.GetTypes())
			{
				if(bpt.BaseType == typeof(BitmapProcess))
				{
					types.Add(bpt);
				}
			}
			
			return types;
		}		
		
		/// <summary>
		/// Establece la base de datos, asignando los eventos convenientemente.
		/// </summary>
		/// <param name="database">
		/// La base de datos que contiene este objeto.
		/// </param>
		private void SetDatabase(DatabaseBase database)
		{
			this.database = database;
			
			this.database.StepDone += 
				new ProcessingStepDoneHandler(OnStepDone);
			
		}
		
		/// <summary>
		/// Genera los atributos de serialización necesarios para que funcione
		/// bien.
		/// </summary>
		/// <returns>
		/// Una instancia de <see cref="XmlAttributeOverrides"/>.
		/// </returns>
		private static XmlAttributeOverrides GetXmlAttributeOverrides()
		{
			XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
			XmlAttributes attrs = new XmlAttributes();
        
			
			Assembly ass = Assembly.GetAssembly(typeof(DatabaseBase));
			
			XmlElementAttribute attr;
			
			foreach(Type t in ass.GetTypes())
			{
				if(t.BaseType == typeof(DatabaseBase))
				{
					attr = new XmlElementAttribute();
					attr.ElementName = t.Name;
					attr.Type = t;
					
					attrs.XmlElements.Add(attr);
				}
			}			

			attrOverrides.Add(typeof(MathTextDatabase), "Database", attrs);
			
			attrs = new XmlAttributes();
			
			ass = Assembly.GetAssembly(typeof(BitmapProcess));
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
			attrOverrides.Add(typeof(MathTextDatabase), "Processes", attrs);
			
			return attrOverrides;
		}
		
		
		
#endregion Metodos privados
	
	}
}
