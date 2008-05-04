// created on 02/01/2006 at 12:55

using System;
using System.Threading;
using System.Collections.Generic;

using MathTextLibrary;
using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;
using MathTextLibrary.Databases;
using MathTextLibrary.Databases.Characteristic;
using MathTextLibrary.BitmapSegmenters;
using MathTextLibrary.Projection;

using MathTextLibrary.Controllers;

using MathTextRecognizer.Stages.Nodes;

namespace MathTextRecognizer.Controllers
{
	/// <summary>
	/// La clase SegmentingAndSymbolMatchingController realiza las funciones de 
	/// control de los procesos de segmentación de imagenes y asignación
	/// de las mismas a un simbolo matematico determinado.
	/// </summary>
	public class OCRController : BaseController
	{			
		
		//La base de datos que usaremos para reconocer los caracteres.
		private List<MathTextDatabase> databases;
		
		
		
		/// <summary>
		/// Evento usado para notificar a la interfaz de que se ha comenzado
		/// a trabajar con un nueva pieza de la imagen.
		/// </summary>
		public event BitmapProcessedHandler BitmapProcessedByDatabase;
		
		//La imagen raiz que contiene la formula completa que deseamos reconocer.
		private SegmentedNode startNode;
		
		private List<BitmapSegmenter> segmenters;
		
		
		
		private bool searchDatabase;
		
		/// <summary>
		/// Constructor de la clase MathTextRecognizerController, debe ser invocado
		/// en las posibles implementaciones distintas de la interfaz de usuario del
		/// reconocedor.
		/// </summary>
		public OCRController() : base()
		{						
			databases = new List<MathTextDatabase>();	
			
			segmenters = new List<BitmapSegmenter>();
			
			// Añadimos los segmentadores a la lista, en orden de preferencia.
			segmenters.Add(new AllHolesProjectionSegmenter(ProjectionMode.Horizontal));
			segmenters.Add(new AllHolesProjectionSegmenter(ProjectionMode.Vertical));
			
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.RightToLeft));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.BottomToTop));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.TopToBottom));
			segmenters.Add(new WaterfallSegmenter(WaterfallSegmenterMode.LeftToRight));
			
			
			
			searchDatabase = true;
			               
		}
	
		/// <summary>
		/// Envolvemos el lanzamiento del evento BitmapBeingRecognized, por comodidad.
		/// </summary>
		/// <param name="bitmap">
		/// La imagen que hemos comenzado a reconocer, que sera enviada como
		/// argumentod del evento.
		/// </param>		
		protected void BitmapProcessedByDatabaseInvoker(MathTextBitmap bitmap)
		{
			if(BitmapProcessedByDatabase!=null)
			{
				BitmapProcessedByDatabase(this,new BitmapProcessedArgs(bitmap));
			}
		}			
		
	
		
	
		
		/// <summary>
		/// Manejador del evento RecognizingCharacteristicChecked de la base de datos de caracteres.
		/// </summary>
		/// <param name="sender">El objeto que envio el evento.</param>
		/// <param name="args">Los argumentos del evento.</param>
		private void OnProcessingStepDone(object sender,
		                                  StepDoneArgs args)
		{			
			MessageLogSentInvoker(args.Message);
			
			SuspendByStep();
			
		}
		
		/// <summary>
		/// Cargamos la base de datos que vamos a utilizar para intentar 
		/// reconocer las imagenes como caracteres.
		/// </summary>
		/// <param name="path">
		/// La ruta del fichero donde esta la base de datos.
		/// </param>
		public void LoadDatabase(string path)
		{
			
			MathTextDatabase database = MathTextDatabase.Load(path);
			
			database.StepDone+=
				new ProcessingStepDoneHandler(OnProcessingStepDone);
			
			databases.Add(database);
		}
		
		/// <value>
		/// Contiene la imagen de inicio que
		/// contiene la formula que deseamos reconocer.
		/// </value>
		public SegmentedNode StartNode
		{
			get
			{				
				return startNode; 			
			}
			set
			{
				startNode=value;
			}
		}

		/// <value>
		/// Contiene las bases de datos que usa el controlador para reconocer.
		/// </value>
		public List<MathTextDatabase> Databases 
		{
			get
			{
				return databases;
			}
			
			set
			{
				databases = value;
				foreach(MathTextDatabase database in databases)
				{
					database.StepDone+=
						new ProcessingStepDoneHandler(OnProcessingStepDone);
				}
			}
		}

	

		/// <value>
		/// Contains a boolean value indicating if the database should be
		/// searched or we should segment directly.
		/// </value>
		public bool SearchDatabase 
		{
			get 
			{
				return searchDatabase;
			}
			set 
			{
				searchDatabase = value;
			}
		}
		
		/// <summary>
		/// Metodo que realiza el procesado de las imagenes
		/// </summary>
		protected override void Process()
		{
			MessageLogSentInvoker("=======================================");
			MessageLogSentInvoker(" Comenzando proceso de segmentado");
			MessageLogSentInvoker("=======================================");
			
		   	RecognizerTreeBuild(startNode);
		   	ProcessFinishedInvoker();
		}
		
		/// <summary>
		/// Con este metodo cramos un arbol de imagenes, de forma recursiva.
		/// Primero intentamos reconocer la imagen como un caracter, si no es 
		/// posible, la intentamos segmentar. Si ninguno de estos procesos es 
		/// posible, la imagen nopudo ser reconocida.
		/// </summary>
		/// <param name="node">
		/// El nodo donde esta la imagen sobre la que trabajaremos.
		/// </param>
		private void RecognizerTreeBuild(SegmentedNode node)
		{			
			// Seleccionamos el nodo.
			
			NodeBeingProcessedInvoker();
			SuspendByNode();
		
			node.Select();
			
			while(node.ChildCount>0)
				node.RemoveChild((SegmentedNode)node[0]);
			
			MathTextBitmap bitmap = node.MathTextBitmap;
			MessageLogSentInvoker("=======================================");
			MessageLogSentInvoker("Tratando la subimagen «{0}»",
			                      node.Name);
			
			// Lanzamos el reconocedor de caracteres para cada una de
			// las bases de datos.
			List<MathSymbol> associatedSymbols = new List<MathSymbol>();
			if(searchDatabase)
			{
				foreach(MathTextDatabase database in databases)
				{
					MessageLogSentInvoker("---------- «{0}» ------------",
					                      database.ShortDescription);
					                      
					
					bitmap.ProcessImage(database.Processes);
					BitmapProcessedByDatabaseInvoker(bitmap);
					
					// Añadimos los caracteres reconocidos por la base de datos
					foreach (MathSymbol symbol in database.Recognize(bitmap))
					{
						if(!associatedSymbols.Contains(symbol))
						{
							// Solo añadimos si no esta ya el simbolo.
							associatedSymbols.Add(symbol);
						}
					}
				}		
						
			
			}
		
						
			// We associate all symbols to the node, so we can postargate
			// the decission step.
			node.Symbols = associatedSymbols;
			
			//Si no hemos reconocido nada, pues intentaremos segmentar el caracter.
			if(associatedSymbols.Count == 0)
			{	
				if(searchDatabase)
				{
					MessageLogSentInvoker("La imagen no pudo ser reconocida como un "
				                      + "simbolo por la base de datos, se tratará de "
				                      + "segmentar");
				}
				else
				{
					MessageLogSentInvoker("Se procede directamente a segmentar la imagen");
					this.SearchDatabase = true;
				}
					
				List<MathTextBitmap> children = CreateChildren(bitmap);
				
				if(children.Count > 1)
				{
					MessageLogSentInvoker("La imagen se ha segmentado correctamente");
					
					//Si solo conseguimos un hijo, es la propia imagen, asi que nada
					
					List<SegmentedNode> nodes = new List<SegmentedNode>();
					
					foreach(MathTextBitmap child in children)
					{
						SegmentedNode childNode = new SegmentedNode(child, node.View);	
						node.AddSegmentedChild(childNode);
						nodes.Add(childNode);
					}
					
					foreach(SegmentedNode childNode in nodes)
					{
						RecognizerTreeBuild(childNode);	
					}
				}
				else
				{
					MessageLogSentInvoker("La imagen no pudo ser segmentada, el "
					                      + "símbolo queda sin reconocer");
				}
			}
			else
			{
				// We prepare the string.
				string text ="";
				foreach(MathSymbol s in associatedSymbols)
				{
					text += String.Format("«{0}», ", s.Text);
				}
				
				text = text.TrimEnd(',',' ');
				MessageLogSentInvoker("Símbolo reconocido por la base de datos como {0}.",
				                      text);
			}
		}
		
		/// <summary>
		/// Segmenta una imagen.
		/// </summary>
		/// <param name="image">
		/// La imagen a segmentar.
		/// </param>
		/// <returns>
		/// Una lista con las imagenes que se han obtenido.
		/// </returns>
		private List<MathTextBitmap> CreateChildren(MathTextBitmap image)
		{
			List<MathTextBitmap> children = new List<MathTextBitmap>();
			
			bool segmented = false;
			for(int i=0; i< segmenters.Count && !segmented; i++)
			{
				// Intentamos segmentar.
				children = segmenters[i].Segment(image);
				
				if(children.Count > 1)
					segmented = true;
				else if(children.Count == 1)
				{
					throw new Exception("MathTextBitmap incorrectly segmented");
				}
			}
			
			return children;
		}
		
	
	}
	
	/// <summary>
	/// Delegado para los manejadores de los eventos enviados por los controladores 
	/// cuando desean notificar que han comenzado a procesar una nueva imagen.
	/// </summary>
	public delegate void BitmapProcessedHandler(object sender,
	                                            BitmapProcessedArgs arg);
	
	/// <summary>
	/// Esta clase encapsula los argumentos enviados en los eventos manejados por
	/// BitmapBeingRecognizedHandler.
	/// </summary>
	public class BitmapProcessedArgs : EventArgs
	{
		private MathTextBitmap b;
		
		/// <summary>
		/// Constructor de la clase.
		/// </summary>
		/// <param name="bitmap">La imagen que se ha comenzado a reconocer.</param>
		public BitmapProcessedArgs(MathTextBitmap bitmap)
			:base()
	    {
			b=bitmap;
		}
		
		/// <value>
		/// Contiene la imagen que pasamos como argumento del evento.
		/// </value>
		public MathTextBitmap MathTextBitmap
		{
			get
			{
				return b;
			}
		}
	}
	
}
