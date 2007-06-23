using System;
using System.Collections.Generic;

using System.Xml.Serialization;
using MathTextLibrary.Databases.Caracteristic.Caracteristics;

namespace MathTextLibrary.Databases.Caracteristic
{	
	/// <summary>
	/// La clase <c>BinaryCaracteristicNode</c> representa un nodo de un arbol
	/// binario de caracteristicas.
	/// </summary>
	public class BinaryCaracteristicNode
	{
		//La rama en la que una determinada caracteristica se cumple.
		private BinaryCaracteristicNode trueTree;
		
		//La rama en la que una determinada caracteristica se incumple.
		private BinaryCaracteristicNode falseTree;
		
		//El simbolo asociado al nodo, solo en el caso de que sea una hoja.
		private MathSymbol symbol;		
		
		/// <summary>
		/// El constructor de <c>BinaryCaracteristicNode</c>.
		/// </summary>
		public BinaryCaracteristicNode()
		{
		}

		/// <summary>
		/// Esta propiedad permite recupera y establecer el simbolo asociado
		/// al nodo.
		/// </summary>
		public MathSymbol Symbol
		{
			get
			{				
				return symbol;
			}
			
			set
			{			
				if(symbol==null)
				{
					symbol=value;					
				}
				else
				{
					throw new ExistingSymbolException("Ya hay un caracter reconocido aqui :S",symbol);
				}				
			}				
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
				
				if(symbol!=null)
					res.Add(symbol);

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
		public BinaryCaracteristicNode TrueTree
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
		public BinaryCaracteristicNode FalseTree
		{
			get{				
				return falseTree;			
			}			
			set{
				falseTree=value;	
			}
		}
	}
	
	/// <summary>
	/// Una excepcion que se lanzara cuando queremos guardar en la 
	/// base de datos un caracter cuyas informacion coincide con otro
	/// ya almacenado.
	/// </summary>
	public class ExistingSymbolException : Exception
	{
		private MathSymbol existing;
		
		/// <summary>
		/// Constructor de ExistingSymbolException.
		/// </summary>
		/// <param name="existing">
		/// El simbolo que esta presente en la base de datos y coincide.
		/// </param>
		public ExistingSymbolException(MathSymbol existing)
			: this("",existing)
		{
		
		}
		
		/// <summary>
		/// Constructor de ExistingSymbolException.
		/// </summary>
		/// <param name="msg">
		/// Un mensaje a enviar junto con la excepcion.
		/// </param>
		/// <param name="existing">
		/// El simbolo que esta presente en la base de datos y coincide.
		/// </param>
		public ExistingSymbolException(string msg,MathSymbol existing)
			: base(msg)
		{
			this.existing=existing;
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
