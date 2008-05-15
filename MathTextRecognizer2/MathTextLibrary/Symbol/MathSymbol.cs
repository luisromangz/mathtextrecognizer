using System;
using System.Xml.Serialization;

namespace MathTextLibrary.Symbol
{
	/// <summary>
	/// Esta clase encapsula un símbolo como un texto y el 
	/// tipo al que corresponde.
	/// </summary>	
	
	public class MathSymbol
	{
		// Texto del simbolo
		private string text;
		
		// Uso del simbolo, para propositos estadisticos
		private int usage;
		
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
		public MathSymbol(string text )
		{
			this.text=text; 
		}		
		
		
		/// <value>
		/// La etiqueta del símbolo.
		/// </value>
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

		/// <value>
		/// Permite recuperar el numero de veces que se ha usado el simbolo,
		/// para proporcionar una medida de la probabilidad de que sea este simbolo
		/// en caso de conflicto al reconocer.
		/// </value>
		public int Usage
		{
			get
			{
				return usage;
			}
		}
		
		/// <summary>
		/// Incrementa al uso del simbolo.
		/// </summary>
		public void IncreaseUsage()
		{
			this.usage++;
		}
		
		/// <summary>
		/// Permite imprimir la información del símbolo.
		/// </summary>
		public override string ToString()
		{
			return text;
			
		}
		
		/// <summary>
		/// Compares two mathsymbol.
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public override bool Equals (object o)
		{
			if(o.GetType() != typeof(MathSymbol))
				return false;
			
			MathSymbol symbol = (MathSymbol) o;
			
			return this.text.Equals(symbol.Text);
		}
	}
}
