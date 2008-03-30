using System;
using System.Xml.Serialization;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;

using MathTextLibrary.Databases;
using MathTextLibrary.Databases.Characteristic.Characteristics;

namespace MathTextLibrary.Databases.Characteristic
{	
	/// <summary>
	/// La clase <c>BinaryCharacteristicNode</c> representa un nodo de un arbol
	/// binario de caracteristicas.
	/// </summary>
	public class CharacteristicNode
	{
		// La rama en la que una determinada caracteristica se cumple.
		private CharacteristicNode trueTree;
		
		// La rama en la que una determinada caracteristica se incumple.
		private CharacteristicNode falseTree;
		
		// Los simbolos asociados al nodo, en el caso de que el simbolo sea 
		// una hoja.
		private List<MathSymbol> symbols;		
		
		/// <summary>
		/// El constructor de <c>BinaryCharacteristicNode</c>.
		/// </summary>
		public CharacteristicNode()
		{
		}

		/// <summary>
		/// Esta propiedad permite asignar y recuperar los simbolos asociados
		/// a un nodo.
		/// </summary>
		public List<MathSymbol> Symbols
		{
			get
			{				
				return symbols;
			}
			set
			{
				symbols = value;
			}
		}
		
		/// <summary>
		/// Permite saber si el nodo tiene simbolos asociados.
		/// </summary>
		public bool HasSymbols
		{
			get
			{
				return symbols!=null && symbols.Count > 0;
			}
		}

		/// <summary>
		/// Permite añadir un simbolo al nodo.
		/// </summary>
		/// <param name="symbol">
		/// El simbolo a añadir
		/// </param>
		public void AddSymbol(MathSymbol symbol)
		{
			if(symbols==null)
				symbols = new List<MathSymbol>();
			
			if (!this.symbols.Contains(symbol))
				this.symbols.Add(symbol);
			else
				throw new DuplicateSymbolException(symbol);
		}
		
		
		/// <summary>
		/// Esta propiedad nos permite recuperar el conjunto de simbolos que se encuentran
		/// por debajo del nodo sobre el que se invoca, de forma que podemos obtener
		/// un conjunto de simbolos similares segun comprobemos caracteristicas.
		/// </summary>
		[XmlIgnoreAttribute]
		public List<MathSymbol> ChildrenSymbols
		{
			get
			{
				List<MathSymbol> res=new List<MathSymbol>();
				
				if(symbols!=null)
					res.AddRange(symbols);

				if(trueTree!=null)
					res.AddRange(trueTree.ChildrenSymbols);
				
				if(falseTree!=null)
					res.AddRange(falseTree.ChildrenSymbols);

				return res;
			}
		}
		
		/// <summary>
		/// Esta propiedad permite establecer y recuperar la rama del arbol
		/// en la que una determinada caracteristica binaria se cumple.
		/// </summary>
		public CharacteristicNode TrueTree
		{
			get{				
				return trueTree;			
			}			
			set{
				trueTree=value;	
			}
		}
		
		/// <summary>
		/// Esta propiedad permite establecer y recuperar la rama del arbol
		/// en la que una determinada caracteristica binaria se cumple.
		/// </summary>
		public CharacteristicNode FalseTree
		{
			get{				
				return falseTree;			
			}			
			set{
				falseTree=value;	
			}
		}
	}
}
