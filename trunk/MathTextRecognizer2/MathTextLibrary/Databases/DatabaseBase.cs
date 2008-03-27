//Creado por: Luis Román Gutiérrez a las 13:08 de 06/19/2007

using System;
using System.Threading;
using System.Xml.Serialization;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;
using MathTextLibrary.BitmapProcesses;

namespace MathTextLibrary.Databases
{
	
	/// <summary>
	/// Esta clase es la clase base para las distintas implementaciones de bases 
	/// de datos en las que podemos reconocer caracteres matemáticos.
	/// </summary>
	[DatabaseInfo("Descripción por defecto")]
	public abstract class DatabaseBase
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
		/// Busca una imagen en la base de datos.
		/// </summary>
		/// <param name="image">
		/// La imagen que se procesará y buscará en la base de datos.
		/// </param>
		/// <returns>
		/// Una lista con todos los simbolos que coincidan con los parametros
		/// asociados a la imagen.
		/// </returns>
		public abstract List<MathSymbol> Recognize(MathTextBitmap image);
		
#endregion Metodos publicos
		
#region Metodos protegidos
		
		/// <summary>
		/// Para lanzar el evento <c>LearningCaracteristicChecked</c> con
		/// comodidad.
		/// </summary>		
		protected void OnLearningStepDoneInvoke(
			ProcessingStepDoneEventArgs arg)
		{
			if(LearningStepDone != null)
			{
				LearningStepDone(this,arg);
			}		
		}
		
		/// <summary>
		/// Para lanzar el evento <c>RecognizingCaracteristicChecked</c> con
		/// comodidad.	
		/// </summary>
		/// <param name="arg">
		/// Los argumentos pasados al manejador del evento.
		/// </param>
		protected void OnRecognizingStepDoneInvoke(
			ProcessingStepDoneEventArgs arg)
		{
			if(RecognizingStepDone != null)
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
	
	
	
}
