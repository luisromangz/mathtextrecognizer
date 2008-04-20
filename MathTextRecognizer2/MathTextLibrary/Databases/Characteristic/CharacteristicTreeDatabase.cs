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
	[DatabaseTypeInfo("Base de datos de árboles de características binarias (no recomendado)",
	                  "Árbol de características binarias")]	
	[XmlInclude(typeof(MathSymbol))]
	[XmlInclude(typeof(CharacteristicNode))]
	public class CharacteristicTreeDatabase : DatabaseBase
	{
#region Atributos
		
		
		// Lista de caracteristicas binarias que se aplican sobre las imagenes.
		private static List<IBinaryCharacteristic> characteristics;
		
		// El nodo raiz del arbol binario de caracteristicas binarias en 
		/// el que guardamos la informacion de caracteristicas.	
		private CharacteristicNode rootNode;
		
#endregion Atributos
		
#region Propiedades
		/// <value>
		/// Contiene los simbolos almacenados en la base de datos.
		/// </value>
		[XmlIgnoreAttribute]
		public override List<MathSymbol> SymbolsContained 
		{
			get
			{
				return this.rootNode.ChildrenSymbols;
			}
		}	

		
#endregion Propiedades
				
#region Métodos públicos
		
		/// <summary>
		/// Constructor de <c>CharacteristicDatabase</c>. Crea una base de datos
		/// vacia, sin ningun simbolo aprendido.
		/// </summary>
		public CharacteristicTreeDatabase() : base()
		{	
			rootNode=new CharacteristicNode();
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
		public override bool Learn(MathTextBitmap bitmap,MathSymbol symbol)
		{
			if(characteristics == null)
				characteristics=CharacteristicFactory.CreateCharacteristicList();
			
			CharacteristicNode node=rootNode;
			bool characteristicValue;	
			
			FloatBitmap processedBitmap = bitmap.LastProcessedImage;
			
			// Recorremos las caracteristicas, y vamos creando el arbol segun
			// vamos necesitando nodos.
			foreach(IBinaryCharacteristic bc in characteristics)
			{					
				if(characteristicValue=bc.Apply(processedBitmap))
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
				
				StepDoneArgs a = 
						CreateStepDoneArgs(bc,characteristicValue,null);
					
				this.StepDoneInvoker(a);
			}	
			
			return node.AddSymbol(symbol);				
			
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
		public override List<MathSymbol> Match(MathTextBitmap image)
		{
			if(characteristics == null)
				characteristics=CharacteristicFactory.CreateCharacteristicList();
			
			List<MathSymbol> res = new List<MathSymbol>();
			CharacteristicNode nodo=rootNode;
			IBinaryCharacteristic bc;
			
			bool exists=true; 
			bool characteristicValue;
			
			List<bool> vector=new List<bool>();
			
			FloatBitmap processedImage = image.LastProcessedImage;
			
			for(int i=0;i<characteristics.Count && exists;i++)
			{
				bc=(IBinaryCharacteristic)(characteristics[i]);				

				if(characteristicValue=bc.Apply(processedImage))
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
				
				 StepDoneArgs args; 
				 //Avisamos de que hemos dado un paso
				 if(nodo!=null)
				 {
					args =CreateStepDoneArgs(bc,
					                         characteristicValue,
					                         nodo.ChildrenSymbols);
				 }
				 else
				 {
				 	args =CreateStepDoneArgs(bc,
					                         characteristicValue,
					                         null);
				 }
				
				 StepDoneInvoker(args);
			}
			
			if(exists)
			{
				res=nodo.Symbols;
			}			
			
			return res;

		}
		
		/// <value>
		/// Contiene el nodo raiz de la base de datos de caracteristicas.
		/// </value>
		public CharacteristicNode RootNode 
		{
			get {
				return rootNode;
			}
			set
			{
				rootNode = value;
			}
		}
		
	
		
#endregion Métodos públicos
		
		/// <summary>
		/// Helps to create a <c>ProcessingStepDoneArgs</c> instance.
		/// </summary>
		/// <param name="bc">
		/// The binary characteristic used.
		/// </param>
		/// <param name="value">
		/// The value of the binary characteristic applied.
		/// </param>
		/// <param name="symilarSymbols">
		/// Similar simbols found.
		/// </param>
		/// <returns>
		/// The <c>ProcessingStepDoneArgs</c> instance created.
		/// </returns>
		private StepDoneArgs CreateStepDoneArgs(IBinaryCharacteristic bc,
		                                        bool value,
		                                        List<MathSymbol> similarSymbols)
		{
			String res = String.Format("Comprobando {0}: {1}", bc.GetType(), value);
			string similar="";	
			if(similarSymbols!=null)
			{
				foreach(MathSymbol ms in similarSymbols)
				{
					similar += String.Format("«{0}», ", ms.Text);
				}				
				
				res +=String.Format("\nCaracteres similares: {0}", 
				                    similar.TrimEnd(',',' '));
			}
			
			
			
			return new StepDoneArgs(res);
		}
		
	}
}