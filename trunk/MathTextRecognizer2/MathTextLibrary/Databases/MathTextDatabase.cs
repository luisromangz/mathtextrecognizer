using System;
using System.IO;
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
			// TODO carga generica de las bases de datos.
			
			//Cargamos el archivo deserializando el contenido.
//			XmlSerializer serializer = 
//				new XmlSerializer(
//				                  typeof(BinaryCaracteristicNode),
//				                  new Type[]{typeof(MathSymbol)});
//			                                           
//			using(StreamReader r = new StreamReader(path))
//			{
//				rootNode= (BinaryCaracteristicNode)serializer.Deserialize(r);
//				r.Close();
//			}			
			
			return null;
		}
			
		public MathSymbol Recognize(MathTextBitmap image)
		{
			return database.Recognize(image);
		}
		
		public void Save(string path)
		{
			// TODO Serializacion de la base de datos generalista
			// Usamos serializacion xml para generar el xml a partir del arbol
			// de caracteristicas.
//			XmlSerializer serializer=
//				new XmlSerializer(typeof(BinaryCaracteristicNode),
//				                  new Type[]{typeof(MathSymbol)});
//			
//			using(StreamWriter w=new StreamWriter(path))
//			{			
//				serializer.Serialize(w,rootNode);
//				w.Close();
//			}
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
