using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using System.Reflection;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;
using MathTextLibrary.Databases.Characteristic.Characteristics;


namespace MathTextLibrary.Databases.Characteristic
{	
	/// <summary>
	/// Esta clase se encarga de mantener una base de datos de caracteristicas
	/// binarias para los caracteres aprendidos, asi como de realizar el añadido
	/// de nuevos caracteres en la misma y realizar busquedas.
	/// </summary>
	[DatabaseTypeInfo("Base de datos basada en características binarias",
	                  "Características binarias")]	
	[XmlInclude(typeof(MathSymbol))]
	[XmlInclude(typeof(CharacteristicNode))]
	public class CharacteristicDatabase : DatabaseBase
	{
		#region Atributos
		
		// Tabla hash para el metodo alternativo de reconocimiento de caracteres
		// basado en distancia.
		private Dictionary<List<bool>,CharacteristicNode> characteristicHash;
		
		// Lista de caracteristicas binarias que se aplican sobre las imagenes.
		private static List<IBinaryCharacteristic> characteristics;
		
		// El nodo raiz del arbol binario de caracteristicas binarias en 
		/// el que guardamos la informacion de caracteristicas.	
		private CharacteristicNode rootNode;
		
		#endregion Atributos
				
		#region Métodos públicos
		
		/// <summary>
		/// Constructor de <c>CharacteristicDatabase</c>. Crea una base de datos
		/// vacia, sin ningun simbolo aprendido.
		/// </summary>
		public CharacteristicDatabase() : base()
		{	
           
          
			rootNode=new CharacteristicNode();
			
			characteristicHash = 
				new Dictionary<List<bool>,CharacteristicNode>();
		}
		
		/// <summary>
		/// Con este metodos almacenamos un nuevo simbolo en la base de
		/// datos.
		/// 
		/// Lanza <c>DuplicateSymbolException</c> si ya existe un simbolo
		/// con la misma etiqueta y caracteristicas binarias en la base de datos.
		/// </summary>
		/// <param name="bitmap">
		/// La imagen cuyas caracteristicas aprenderemos.
		/// </param>
		/// <param name="symbol">
		/// El simbolo que representa a la imagen.
		///</param>
		public override void Learn(MathTextBitmap bitmap,MathSymbol symbol)
		{
			if(characteristics == null)
				characteristics=CharacteristicFactory.CreateCharacteristicList();
			
			CharacteristicNode node=rootNode;
			bool characteristicValue;	
			
			// Recorremos las caracteristicas, y vamos creando el arbol segun
			// vamos necesitando nodos.
			foreach(IBinaryCharacteristic bc in characteristics)
			{					
				if(characteristicValue=bc.Apply(bitmap))
				{
					if(node.TrueTree==null)
					{
						node.TrueTree=new CharacteristicNode();						
					}
					
					node=node.TrueTree;					
				}
				else
				{
					if(node.FalseTree==null)
					{
						node.FalseTree=new CharacteristicNode();						
					}					
					node=node.FalseTree;					
				}	
				
				ProcessingStepDoneArgs a = new ProcessingStepDoneArgs(bc,
				                                                      bitmap,
				                                                      characteristicValue);
					
				this.OnLearningStepDoneInvoke(a);
			}	
			
			node.AddSymbol(symbol);					
			OnSymbolLearnedInvoke();
		}
		
		
		
		/// <summary>
		/// Este metodo intenta recuperar los simbolos de la base de datos 
		/// correspondiente a una imagen.
		/// </summary>
		/// <param name="image">
		/// La imagen cuyos simbolos asociados queremos encontrar.
		/// </param>
		/// <returns>
		/// Los simbolos que se puedieron asociar con la imagen.
		/// </returns>
		public override List<MathSymbol> Recognize(MathTextBitmap image)
		{
			if(characteristics == null)
				characteristics=CharacteristicFactory.CreateCharacteristicList();
			
			List<MathSymbol> res = new List<MathSymbol>();
			CharacteristicNode nodo=rootNode;
			IBinaryCharacteristic bc;
			
			bool exists=true; 
			bool characteristicValue;
			
			List<bool> vector=new List<bool>();
			
			for(int i=0;i<characteristics.Count && exists;i++)
			{
				bc=(IBinaryCharacteristic)(characteristics[i]);				

				if(characteristicValue=bc.Apply(image))
				{
				
						if(nodo.TrueTree==null)
						{
							exists=false;
						}					
						nodo=nodo.TrueTree;
			
				}
				else
				{
				
						 if(nodo.FalseTree==null)
						 {
							 exists=false;
						 }					
						 nodo=nodo.FalseTree;
				 }		
				
				 ProcessingStepDoneArgs args; 
				 //Avisamos de que hemos dado un paso
				 if(nodo!=null)
				 {
					args =new ProcessingStepDoneArgs(bc,
					                                 image,
					                                 characteristicValue,
					                                 nodo.ChildrenSymbols);
				 }
				 else
				 {
				 	args =new ProcessingStepDoneArgs(bc,
					                                 image,
					                                 characteristicValue);
				 }
				
				 OnRecognizingStepDoneInvoke(args);
			}
			
			if(exists)
			{
				res=nodo.Symbols;
			}			
			
			return res;

		}
		
		
		
		#endregion Métodos públicos
		
		#region Métodos no públicos
		
		/// <summary>
		/// Calcula la distancia entre dos vectores, usando para ello el numero 
		/// de diferencias entre los dos vectores.
		/// </summary>
		/// <param name="vector1">
		/// El vector de caracteristicas binarias de un simbolo.
		/// </param>
		/// <param name="vector2">
		/// El vector de caracteristicas binarias de otro simbolo.
		/// </param>
		/// <returns>La «distacia» entre los dos vectores.</returns>
		private int BoolVectorDistance(List<bool> vector1, List<bool> vector2)
		{
			int count=0;
			
			for(int i=0;i<vector1.Count;i++)
			{
				if((bool)vector1[i]!=(bool)vector2[i])
				{
					count++;
				}			
			}
			
			return count;
		}
		
		/// <summary>
		/// Se invoca para crear la tabla hash sobre la informacion aprendida a 
		/// partir de la base de datos arbórea.
		/// </summary>
		private void CreateHashTable()
		{
			characteristicHash = 
				new Dictionary<List<bool>,CharacteristicNode>();
			Console.WriteLine("Creando hash");
			CreateHashTableAux(rootNode,new List<bool>());
			Console.WriteLine("Fin hash");
			
		}
		
		/// <summary>
		/// Rellena recursivamente la tabla hash para la busqueda de 
		/// caracteristicas.
		/// </summary>
		/// <param name="node">
		/// El nodo que tratamos.
		/// </param>
		/// <param name="vector">
		/// El vector de caracteristicas que vamos generando.
		/// </param>
		private void CreateHashTableAux(CharacteristicNode node,
		                                List<bool> vector)
		{
			
			List<bool> newVector; 
			
			if(node.HasSymbols)
			{
				// Hemos llegado a una hoja, añadimos una entrada en la tabla.
				Console.WriteLine(vector.Count);
				characteristicHash.Add(vector,node);
			}
			else
			{				
				if(node.FalseTree!=null)
				{
					newVector = new List<bool>(vector);
					newVector.Add(false);
					CreateHashTableAux(node.FalseTree,newVector);			
				}
				
				if(node.TrueTree!=null)
				{
					newVector = new List<bool>(vector);
					newVector.Add(true);
					CreateHashTableAux(node.TrueTree,newVector);
				}
			}
		}

		
		/// <summary>
		/// Busca el símbolo mas cercano en la tabla hash.
		/// </summary>
		/// <param name="vector">
		/// El vector conteniendo los resultados de las caracterisiticas 
		/// binarias de un simbolo.
		/// </param>
		/// <returns>
		/// El conjunto de simbolos mas cercano que hay en la base de datos.
		/// </returns>
		private List<MathSymbol> NearestSymbols(List<bool> vector)
		{
			
			int minDiff=Int32.MaxValue;
			List<MathSymbol> res = new List<MathSymbol>();
			List<bool> key=null;
			
			foreach(List<bool> s in characteristicHash.Keys)
			{
				if(BoolVectorDistance(vector,s) < minDiff)
				{
					minDiff = BoolVectorDistance(vector,s);
					key=s;
				}
			}		
			
			if(key!=null && minDiff < 3)
			{
				
				res=characteristicHash[key].Symbols;
			}
			
			return res;
		
		}
		#endregion Métodos no públicos
		
		/// <value>
		/// Contiene el nodo raiz de la base de datos de caracteristicas.
		/// </value>
		public virtual CharacteristicNode RootNode 
		{
			get {
				return rootNode;
			}
			set
			{
				rootNode = value;
			}
		}
	}
}