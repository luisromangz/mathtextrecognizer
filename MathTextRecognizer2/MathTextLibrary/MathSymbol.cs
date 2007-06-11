using System;

namespace MathTextLibrary
{
	/// <summary>
	/// Esta clase encapsula un símbolo como un texto y el 
	/// tipo al que corresponde.
	/// </summary>
	public class MathSymbol
	{
		// Texto del simbolo
		private string text;
		
		// Tipo del simbolo 
		private MathSymbolType type;
		
		// Simbolo nulo
		private static MathSymbol nullSymbol;

		/// <summary>
		/// El constructor por defecto de <c>MathSymbol</c>.
		/// </summary>
		public MathSymbol()
		{
		}

		/// <summary>
		/// El constructor de <c>MathSymbol</c>.
		/// </summary>
		/// <param name = "text">
		/// El texto que representará el símbolo.
		/// </param>
		/// <param name = "type">
		/// El tipo del símbolo.
		/// </param>
		public MathSymbol(string text,MathSymbolType type)
		{
			this.text=text;
			this.type=type;
		}		
		
		/// <summary>
		/// El símbolo que se usa cuando no se ha podido reconocer.
		/// </summary>
		public static MathSymbol NullSymbol
		{
			get
			{
				if(nullSymbol==null)
				{
					nullSymbol = new MathSymbol("Not recognizable symbol",
												MathSymbolType.NotRecognized);
				}

				return nullSymbol;
			}
		}
		
		/// <summary>
		/// La etiqueta del símbolo.
		/// </summary>
		public string Text
		{
			get
			{				
				return text;
			}	
			
			set
			{
				text=value;
			}
		}
		
		/// <summary>
		/// El tipo del símbolo.
		/// </summary>
		public MathSymbolType SymbolType
		{
			get
			{
				return type;
			}
			set
			{
				if(value==MathSymbolType.NotRecognized)
				{
					throw new ArgumentException("No puede usarse NotRecognized como tipo del simbolo");
				}				
				type=value; 
			}
		}
		
		/// <summary>
		/// Permite imprimir la información del símbolo.
		/// </summary>
		public override string ToString()
		{
			return text;
		}
	}
	
	/// <summary>
	/// Tipos de simbolos que usa la aplicacion.
	/// </summary>
	public enum MathSymbolType
	{
		Identifier,
		Number,
		Operator,
		LeftDelimiter,
		RightDelimiter,
		Superindex,
		Subindex,
		Fraction,
		NotRecognized
	}
}
