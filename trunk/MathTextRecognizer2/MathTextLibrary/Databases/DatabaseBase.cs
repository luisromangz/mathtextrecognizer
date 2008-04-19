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
	public abstract class DatabaseBase
	{	


#region Eventos 
		
		/// <summary>
		/// Este evento se lanza para indicar que se completado un paso
		/// mientras se esta aprendiendo o reconociendo un caracter en la 
		/// base de datos.
		/// </summary>
		public event ProcessingStepDoneHandler StepDone;
		
		
		
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
		/// </param>
		/// <returns>
		/// If the symbol was learned or there was a conflict.
		/// </returns>
		public abstract bool Learn(MathTextBitmap bitmap,MathSymbol symbol);
				
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
		public abstract List<MathSymbol> Match(MathTextBitmap image);
		
		/// <value>
		/// Contiene los simbolos almacenados en la base de datos.
		/// </value>
		[XmlIgnoreAttribute]
		public abstract List<MathSymbol> SymbolsContained
		{
			get;
		}
		
#endregion Metodos publicos
		
#region Metodos protegidos
		
		/// <summary>
		/// Launches the <c>StepDone</c> event.
		/// </summary>		
		protected void StepDoneInvoker(StepDoneArgs arg)
		{
			if(StepDone != null)
			{
				StepDone(this,arg);
			}		
		}
		
	
		
#endregion Metodos protegidos
	
	}
	
	
	
}
