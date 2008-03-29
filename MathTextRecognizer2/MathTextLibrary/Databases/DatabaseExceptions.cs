// DatabaseExceptions.cs created with MonoDevelop
// User: luis at 20:35 29/03/2008

using System;

using MathTextLibrary.Symbol;

namespace MathTextLibrary.Databases
{
	
	/// <summary>
	/// Esta excepcion se lanzara cuando una base de datos
	/// intente aprender un simbolo con las mismas propiedades y etiqueta que 
	/// uno ya existente. 
	/// </summary>
	public class DuplicateSymbolException : Exception
	{
		
		private MathSymbol duplicatedSymbol;
		
		public DuplicateSymbolException(MathSymbol symbol)
		{
			duplicatedSymbol = symbol;
		}

		/// <summary>
		/// Permite recuperar el simbolo duplicado.
		/// </summary>
		public MathSymbol DuplicatedSymbol 
		{
			get 
			{
				return duplicatedSymbol;
			}
		}
		
		
	}
}
