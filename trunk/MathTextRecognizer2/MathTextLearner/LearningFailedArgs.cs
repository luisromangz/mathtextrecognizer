
using System;

using MathTextLibrary;
using MathTextLibrary.Symbol;

namespace MathTextLearner
{
	/// <summary>
	/// Esta clase representa los argumentos necesarios para el delegado encargado
	/// de informar del fallo del proceso de aprendizaje.
	/// </summary>
	class LearningFailedArgs : EventArgs
	{
		private MathSymbol existing;
	
		/// <summary>
		/// El constructor de <c>LearningFailedArgs</c>
		/// </summary>
		/// <param name = "existingSymbol">
		/// El símbolo que presenta un conflicto con el que se intentaba 
		/// aprender.
		/// </param>
		public LearningFailedArgs(MathSymbol existingSymbol)
			:base()
		{
			this.existing=existingSymbol;		
		}
		
		/// <summary>
		/// Propiedad de solo lectura que nos permite recuperar el simbolo
		/// coincidente de la base de datos.
		/// </summary>
		public MathSymbol ExistingSymbol
		{
			get
			{
				return existing;
			}
		}
	
	}
}
