using System;
using System.Threading;
using System.Collections.Generic;

using MathTextLibrary.BitmapProcesses;

namespace MathTextLibrary.Databases
{
	
	/// <summary>
	/// Esta clase es la clase base para las distintas implementaciones de bases 
	/// de datos en las que podemos reconocer caracteres matemáticos.
	/// </summary>
	[DatabaseDescription("Descripción por defecto")]
	public abstract class MathTextDatabase
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
		
		// Para ver si tenemos que ir paso a paso
		protected bool stepByStep; 
		
		// Para garantizar la exclusion mutua al usar hilos 
		protected Mutex stepByStepMutex;	
#endregion Atributos
		
		
		
		
		
		public MathTextDatabase()
		{
			stepByStep=false;
			stepByStepMutex=new Mutex();
		}		
		
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
		
		/// <summary>
		/// Propiedad que permite establecer y recuperar el modo de ejecucion 
		/// para el proceso de busqueda o aprendizaje en la base de datos.
		/// </summary>
		public bool StepByStep
		{
			get
			{
				return stepByStep;
			}
			set
			{
				lock(stepByStepMutex)
				{
					stepByStep=value;
				}
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
		public abstract void Learn(MathTextBitmap bitmap,MathSymbol symbol);
		
		/// <summary>
		/// Permite abrir un fichero xml en el que esta almacenada la base de
		/// datos.
		/// </summary>
		/// <param name="path">
		/// La ruta del archivo que queremos cargar.
		/// </param>
		public abstract void LoadXml(string path);
		
		public abstract MathSymbol Recognize(MathTextBitmap image);
		
		public abstract void XmlSave(string path);
		
#endregion Metodos publicos
		
#region Metodos protegidos

		/// <summary>
		/// Para lanzar el evento <c>LearningCaracteristicChecked</c> con
		/// comodidad.
		/// </summary>		
		protected void OnLearningStepDoneInvoke(
			ProcessingStepDoneEventArgs arg)
		{
			if(LearningStepDone!=null)
			{
				LearningStepDone(this,arg);
			}		
		}
		
		/// <summary>
		/// Para lanzar el evento <c>RecognizingCaracteristicChecked</c> con
		/// comodidad.
		/// </summary>		
		protected void OnRecognizingStepDoneInvoke(
			ProcessingStepDoneEventArgs arg)
		{
			if(RecognizingStepDone!=null)
			{
				RecognizingStepDone(this,arg);
			}		
		}
		
		protected void OnSymbolLearnedInvoke()
		{
			if(SymbolLearned != null)
			{
				SymbolLearned(this, EventArgs.Empty);
			}
		}
		
#endregion Metodos protegidos
	
	}
	
	
	/// <summary>
	/// Esta clase define un atributo para ser usado como descripción para las bases 
	/// de datos de caracteres matemáticos.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true) ] 
	public class DatabaseDescription : Attribute
	{
		private string _description;
		
		public DatabaseDescription(string description)
		{
			_description = description;
		}
		
		public string Description
		{
			get
			{
				return _description;
			}
		}
	}
}
