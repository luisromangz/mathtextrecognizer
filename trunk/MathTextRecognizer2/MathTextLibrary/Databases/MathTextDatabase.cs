using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Generic;

using MathTextLibrary.BitmapProcesses;

namespace MathTextLibrary.Databases
{
	
	/// <summary>
	/// Esta clase es la clase base para las distintas implementaciones de bases 
	/// de datos en las que podemos reconocer caracteres matemáticos.
	/// </summary>	
	public class MathTextDatabase
	{	
#region Eventos 
		
		/// <summary>
		/// Este evento se lanza para indicar que se completado un paso
		/// mientras se esta aprendiendo un caracter en la base de datos.
		/// </summary>
		public event ProcessingStepDoneEventHandler LearningStepDone;
		
		/// <summary>
		/// Este evento se lanza para indicar que se completado un paso 
		/// mientras se esta reconociendo un caracter en la base de datos.
		/// </summary>
		public event ProcessingStepDoneEventHandler RecognizingStepDone;
		
		/// <summary>
		/// Este evento se lanza cuando se ha aprendindo un nuevo simbolo en la
		/// base de datos.
		/// </summary>
		public event SymbolLearnedEventHandler SymbolLearned;		
		
#endregion Eventos
		
#region Atributos
		
		private List<BitmapProcess> processes;
		
		private DatabaseBase database;
		
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
		
		/// <summary>
		/// Esta propiedad permite establecer y recuperar la lista de los
		/// procesos de imagenes usados en la base de datos.
		/// </summary>
		public List<BitmapProcess> Processes 
		{
			get {
				return processes;
			}
			
			set{
				processes = value;
			}
		}		

		public DatabaseBase Database
		{
			get {
				return database;
			}
			
			set {
				SetDatabase(value);				
			}
		}
		
		public bool StepByStep
		{
			get
			{
				return database.StepByStep;
			}
			set
			{
				database.StepByStep = value;
			}
		}
		
		
#endregion Propiedades
		
#region Metodos publicos

		/// <summary>
		/// Con este metodos almacenamos un nuevo simbolo en la base de
		/// datos.
		/// </summary>
		/// <param name="bitmap">
		/// La imagen que aprenderemos.
		/// </param>
		/// <param name="symbol">
		/// El simbolo que representa a la imagen.
		///</param>
		public void Learn(MathTextBitmap bitmap,MathSymbol symbol)
		{
			database.Learn(bitmap,symbol);
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
			//Cargamos el archivo deserializando el contenido.

			List<Type> types = new List<Type>();
			
			foreach(Type databaseType in RetrieveDatabaseTypes())
			{
				types.AddRange(RetrieveDatabaseUsedTypes(databaseType));
			}
			
			types.AddRange(RetrieveProcessesTypes());
			
			Type[] usedTypes = new Type[types.Count];
			
			types.CopyTo(usedTypes);
			
			XmlSerializer serializer = new XmlSerializer(typeof(MathTextDatabase),			  
			                                             usedTypes);
			
			MathTextDatabase db = null;
			                                           
			using(StreamReader r = new StreamReader(path))
			{
				db = (MathTextDatabase)serializer.Deserialize(r);
				r.Close();
			}			
			
			return db;
		}
			
		public MathSymbol Recognize(MathTextBitmap image)
		{
			return database.Recognize(image);
		}
		
		/// <summary>
		/// Este metodo almacena la base de datos en el disco duro.
		/// </summary>
		/// <param name="path">
		/// La ruta del archivo en la que se guardara la base de datos.
		/// </param>
		public void Save(string path)
		{		
			// Obtenemos el tipo de la base de datos
			Type databaseType = database.GetType();
			
			List<Type> types = new List<Type>(); 
			types.AddRange(RetrieveDatabaseUsedTypes(databaseType));
			types.AddRange(RetrieveProcessesTypes());
			

			Type[] usedTypes = new Type[types.Count];
			
			types.CopyTo(usedTypes);
			
			
			XmlSerializer serializer=
				new XmlSerializer(databaseType, usedTypes);
			
			using(StreamWriter w=new StreamWriter(path))
			{			
				serializer.Serialize(w,this);
				w.Close();
			}
		}
		
#endregion Metodos publicos
		
#region Metodos protegidos
			
		protected void OnLearningStepDone(object sender,
		                                  ProcessingStepDoneEventArgs arg)
		{
			if(LearningStepDone != null)
			{
				LearningStepDone(this,arg);
			}		
		}
		
		
		protected void OnRecognizingStepDone(object sender,
		                                     ProcessingStepDoneEventArgs arg)
		{
			if(RecognizingStepDone != null)
			{
				RecognizingStepDone(this,arg);
			}		
		}
		
		protected void OnSymbolLearned(object sender,
		                                     EventArgs args)
		{
			if(SymbolLearned != null)
			{
				SymbolLearned(this, EventArgs.Empty);
			}
		}
		
#endregion Metodos protegidos
		
#region Metodos privados
		
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
		
		private static List<Type> RetrieveDatabaseUsedTypes(Type t)
		{
			object[] attrs = t.GetCustomAttributes(typeof(DatabaseInfo),true);
			
			Type[] usedTypes = (Type[])(attrs[0]);
			
			List<Type> types = new List<Type>(usedTypes);
			types.Add(t);
						
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
		
		private void SetDatabase(DatabaseBase database)
		{
			this.database = database;
			
			this.database.LearningStepDone += 
				new ProcessingStepDoneEventHandler(OnLearningStepDone);
			
			this.database.RecognizingStepDone +=
				new ProcessingStepDoneEventHandler(OnRecognizingStepDone);
			
			this.database.SymbolLearned +=
				new SymbolLearnedEventHandler(OnSymbolLearned);
		}
		
		
		
#endregion Metodos privados
	
	}
}
