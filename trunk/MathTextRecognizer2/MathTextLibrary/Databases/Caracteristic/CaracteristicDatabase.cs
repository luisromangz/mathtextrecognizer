using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Xml.Serialization;

using MathTextLibrary.Databases.Caracteristic.Caracteristics;


namespace MathTextLibrary.Databases.Caracteristic
{	
	/// <summary>
	/// Esta clase se encarga de mantener una base de datos de caracteristicas
	/// binarias para los caracteres aprendidos, asi como de realizar el añadido
	/// de nuevos caracteres en la misma y realizar busquedas.
	/// </summary>
	[DatabaseDescription("Base de datos basada en características binarias")]
	public class CaracteristicDatabase
		: MathTextDatabase
	{
		#region Atributos
		
		// Tabla hash para el metodo alternativo de reconocimiento de caracteres
		// basado en distancia.
		private Hashtable caracteristicHash;
		
		// Lista de caracteristicas binarias que se aplican sobre las imagenes.
		private IList caracteristics;
		
		// El nodo raiz del arbol binario de caracteristicas binarias en 
		/// el que guardamos la informacion de caracteristicas.
		private BinaryCaracteristicNode rootNode;
		
		// Para ver si tenemos que ir paso a paso
		private bool stepByStep; 

		// Para garantizar la exclusion mutua al usar hilos sobre esta clase.
		private Mutex stepByStepMutex;			
		
		#endregion Atributos
		
		#region Eventos 
		
		/// <summary>
		/// Este evento se lanza para indicar que se ha comprobado una 
		/// caracteristica binaria mientras se esta aprendiendo un caracter en 
		/// la base de datos.
		/// </summary>
		public event BinaryCaracteristicCheckedEventHandler LearningCaracteristicChecked;
		
		/// <summary>
		/// Este evento se lanza para indicar que se ha comprobado una 
		/// caracteristica binaria mientras se esta intentando reconocer un
		/// caracter en la base de datos.
		/// </summary>
		public event BinaryCaracteristicCheckedEventHandler RecognizingCaracteristicChecked;
		
		/// <summary>
		/// Este evento se lanza cuando se ha aprendindo un nuevo simbolo en la
		/// base de datos.
		/// </summary>
		public event SymbolLearnedEventHandler SymbolLearned;		
		
		#endregion Eventos
		
		#region Propiedades
		
		/// <summary>
		/// Propiedad que permite establecer y recuperar el modo de ejecucion 
		/// para el proceso de busqueda o aprendizaje en la base de datos.
		/// </summary>
		public bool StepByStep
		{
			get
			{
				return stepByStep;
			}
			set
			{
				lock(stepByStepMutex)
				{
					stepByStep=value;
				}
			}
		}		
		
		#endregion Propiedades
		
		#region Métodos públicos
		
		/// <summary>
		/// Constructor de <c>CaracteristicDatabase</c>. Crea una base de datos
		/// vacia, sin ningun simbolo aprendido.
		/// </summary>
		public CaracteristicDatabase()
		{	
            caracteristics=CaracteristicFactory.CreateCaracteristicList();
          
			rootNode=new BinaryCaracteristicNode();

			stepByStep=false;
			stepByStepMutex=new Mutex();
			
			caracteristicHash=new Hashtable();
		}
		
		/// <summary>
		/// Con este metodos almacenamos un nuevo simbolo en la base de
		/// datos.
		/// </summary>
		/// <param name="bitmap">
		/// La imagen cuyas caracteristicas aprenderemos.
		/// </param>
		/// <param name="symbol">
		/// El simbolo que representa a la imagen.
		///</param>
		public void Learn(MathTextBitmap bitmap,MathSymbol symbol)
		{
			BinaryCaracteristicNode nodo=rootNode;
			bool caracteristicValue;	
			
			// Recorremos las caracteristicas, y vamos creando el arbol segun
			// vamos necesitando nodos.
			foreach(IBinaryCaracteristic bc in caracteristics)
			{					
				if(caracteristicValue=bc.Apply(bitmap))
				{
					if(nodo.TrueTree==null)
					{
						nodo.TrueTree=new BinaryCaracteristicNode();						
					}
					
					nodo=nodo.TrueTree;					
				}
				else
				{
					if(nodo.FalseTree==null)
					{
						nodo.FalseTree=new BinaryCaracteristicNode();						
					}					
					nodo=nodo.FalseTree;					
				}	
				OnLearningCaracteristicChecked(
				     new BinaryCaracteristicCheckedEventArgs(
				     		bc,bitmap,caracteristicValue));
				
				bool aux;
				lock(stepByStepMutex)
				{
					aux=stepByStep;
				}
				
				if(aux)
				{						
					Thread.CurrentThread.Suspend();
				}
									
			}			
		
			
			nodo.Symbol=symbol;					
			OnSymbolLearned();
		}
		
		/// <summary>
		/// Permite abrir un fichero xml en el que tenemos almacenada la base de
		/// datos de caracteristicas binarias.
		/// </summary>
		/// <param name="path">
		/// La ruta del archivo que queremos cargar.
		/// </param>
		public void LoadXml(string path)
		{
			//Cargamos el archivo deserializando el contenido.
			XmlSerializer serializer=new XmlSerializer(
				typeof(BinaryCaracteristicNode),new Type[]{typeof(MathSymbol)});
			
			using(StreamReader r=new StreamReader(path))
			{
				rootNode= (BinaryCaracteristicNode)serializer.Deserialize(r);
				r.Close();
			}
			
			CreateHashTable();
			
		}
		
		/// <summary>
		/// Este metodo intenta recuperar el simbolo que representa a una imagen,
		/// a partir de la informacion almacenada en la base de datos.
		/// </summary>
		/// <param name="image">
		/// La imagen cuyo simbolo queremos encontrar.
		/// </param>
		/// <returns>
		/// El simbolo asociado a la imagen, si fue encontrada. Si no, un 
		/// <c>MathSymbol.NullSymbol</c>.
		/// </returns>
		public MathSymbol Recognize(MathTextBitmap image)
		{
			MathSymbol res=MathSymbol.NullSymbol;
			BinaryCaracteristicNode nodo=rootNode;
			IBinaryCaracteristic bc;
			
			bool existe=true; 
			bool caracteristicValue;
			
			ArrayList vector=new ArrayList();
			
			for(int i=0;i<caracteristics.Count && existe;i++)
			{
				bc=(IBinaryCaracteristic)(caracteristics[i]);				

				if(caracteristicValue=bc.Apply(image))
				{
				//	if(existe){
						if(nodo.TrueTree==null)
						{
							existe=false;
						}					
						nodo=nodo.TrueTree;
				//	}
				//	vector.Add(true);
				}
				else
				{
				//	if(existe){
						 if(nodo.FalseTree==null)
						 {
							 existe=false;
						 }					
						 nodo=nodo.FalseTree;
				//	 }
				//	 vector.Add(false);
				 }		
				
				 //Avisamos de que hemos dado un paso
				 if(nodo!=null)
				 {
					OnRecognizingCaracteristicChecked(new BinaryCaracteristicCheckedEventArgs(bc,image,caracteristicValue,nodo.ChildrenSymbols));
				 }
				 else
				 {
				 	OnRecognizingCaracteristicChecked(new BinaryCaracteristicCheckedEventArgs(bc,image,caracteristicValue));
				 }
				
				 
				 bool aux;
				lock(stepByStepMutex)
				{
					aux=stepByStep;
				}
				if(aux)
				{						
					Thread.CurrentThread.Suspend();
				}	
			}
			
			if(existe)
			{
				res=nodo.Symbol;
			}
			//Descomentar para usar el la busqueda en la tabla hash si se falla al buscar en el arbol.
			/*else{
				res=NearestSymbol(vector);
				
			}*/
			
			return res;

		}
		
		/// <summary>
		/// Permite guardar un fichero xml con la base de datos de
		/// caracteristicas binarias.
		/// </summary>
		/// <param name="path">
		/// La ruta en la que queremos guardar la base de datos.
		/// </param>
		public void XmlSave(string path){			
			// Usamos serializacion xml para generar el xml a partir del arbol
			// de caracteristicas.
			XmlSerializer serializer=new XmlSerializer(
				typeof(BinaryCaracteristicNode),new Type[]{typeof(MathSymbol)});
			
			using(StreamWriter w=new StreamWriter(path))
			{			
				serializer.Serialize(w,rootNode);
				w.Close();
			}
			
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
		private int BoolVectorDistance(IList vector1, IList vector2)
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
			caracteristicHash=new Hashtable();
			Console.WriteLine("Creando hash");
			CreateHashTableAux(rootNode,new bool[]{});
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
		private void CreateHashTableAux(BinaryCaracteristicNode node, IList vector)
		{
			
			ArrayList newVector; 
			
			if(node.Symbol!=null)
			{
				// Hemos llegado a una hoja, añadimos una entrada en la tabla.
				Console.WriteLine(vector.Count);
				caracteristicHash.Add(vector,node.Symbol);
			}
			else
			{				
				if(node.FalseTree!=null)
				{
					newVector = new ArrayList(vector);
					newVector.Add(false);
					CreateHashTableAux(node.FalseTree,newVector);			
				}
				
				if(node.TrueTree!=null)
				{
					newVector = new ArrayList(vector);
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
		/// El simbolo mas cercano que tenemos en la base de datos.
		/// </returns>
		private MathSymbol NearestSymbol(IList vector)
		{
			
			int minDiff=Int32.MaxValue;
			MathSymbol res;
			IList key=null;
			
			foreach(IList s in caracteristicHash.Keys)
			{
				if(BoolVectorDistance(vector,s)<minDiff)
				{
					minDiff=BoolVectorDistance(vector,s);
					key=s;
				}
			}		
			
			if(key!=null && minDiff < 3)
			{
				
				res=(MathSymbol) (caracteristicHash[key]);
			}
			else
			{
				res=MathSymbol.NullSymbol;
			}
			return res;
		
		}
		
		/// <summary>
		/// Para lanzar el evento <c>LearningCaracteristicChecked</c> con
		/// comodidad.
		/// </summary>		
		protected void OnLearningCaracteristicChecked(
			BinaryCaracteristicCheckedEventArgs arg)
		{
			if(LearningCaracteristicChecked!=null)
			{
				LearningCaracteristicChecked(this,arg);
			}		
		}
		
		/// <summary>
		/// Para lanzar el evento <c>RecognizingCaracteristicChecked</c> con
		/// comodidad.
		/// </summary>		
		protected void OnRecognizingCaracteristicChecked(
			BinaryCaracteristicCheckedEventArgs arg)
		{
			if(RecognizingCaracteristicChecked!=null)
			{
				RecognizingCaracteristicChecked(this,arg);
			}		
		}
		
		/// <summary>
		/// Para lanzar el evento <c>SymbolLearned</c> con comodidad.
		/// </summary>		
		protected void OnSymbolLearned()
		{
			if(SymbolLearned!=null)
				SymbolLearned(this,EventArgs.Empty);
			   
		}
		
		#endregion Métodos no públicos
		
		
		
	}
}