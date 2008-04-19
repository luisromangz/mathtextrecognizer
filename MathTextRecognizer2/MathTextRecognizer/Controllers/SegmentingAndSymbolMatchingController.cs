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

using MathTextRecognizer.Steps.Nodes;

namespace MathTextRecognizer.Controllers
{
	/// <summary>
	/// La clase SegmentingAndSymbolMatchingController realiza las funciones de 
	/// control de los procesos de segmentación de imagenes y asignación
	/// de las mismas a un simbolo matematico determinado.
	/// </summary>
	public class SegmentingAndSymbolMatchingController
	{			
		
		//La base de datos que usaremos para reconocer los caracteres.
		private List<MathTextDatabase> databases;
		
		/// <summary>
		/// Evento usado para enviar un mensaje de informacion a la interfaz.
		/// </summary>
		public event MessageLogSentHandler MessageLogSent;
		
		/// <summary>
		/// Evento usado para notificar a la interfaz de que se ha terminado de
		/// realizar un proceso.
		/// </summary>
		public event ProcessFinishedHandler RecognizementProcessFinished;
		
		/// <summary>
		/// Evento usado para notificar a la interfaz de que se ha comenzado
		/// a trabajar con un nueva pieza de la imagen.
		/// </summary>
		public event BitmapBeingRecognizedHandler BitmapBeingRecognized;
		
		//La imagen raiz que contiene la formula completa que deseamos reconocer.
		private SegmentedNode startNode;
		
		private List<BitmapSegmenter> segmenters;
		
		/// <summary>
		/// Constructor de la clase MathTextRecognizerController, debe ser invocado
		/// en las posibles implementaciones distintas de la interfaz de usuario del
		/// reconocedor.
		/// </summary>
		public SegmentingAndSymbolMatchingController()
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
			               
		}
	
		/// <summary>
		/// Envolvemos el lanzamiento del evento BitmapBeingRecognized, por comodidad.
		/// </summary>
		/// <param name="bitmap">
		/// La imagen que hemos comenzado a reconocer, que sera enviada como
		/// argumentod del evento.
		/// </param>		
		protected void BitmapBeingRecognizedInvoker(MathTextBitmap bitmap)
		{
			if(BitmapBeingRecognized!=null)
			{
				BitmapBeingRecognized(this,new BitmapBeingRecognizedArgs(bitmap));
			}
		}			
		
		/// <summary>
		/// Envolvemos el lanzamiento del evento LogMessageSend, por comodidad.
		/// </summary>
		/// <param name="msg">
		/// El mensaje que queremos pasar como argumento al manejador del evento.
		/// </param>		
		protected void MessageLogSentInvoker(string msg, params object [] args)
		{
			if(MessageLogSent!=null)
			{
				MessageLogSent(this,new MessageLogSentArgs(String.Format(msg,args)));
			}
		}
		
		/// <summary>
		/// Envolvemos el lanzamiento del evento RecognizeProcessFinished, por comodidad.
		/// </summary>		
		protected void RecognizementProcessFinishedInvoker()
		{
			if(RecognizementProcessFinished!=null)
			{
				RecognizementProcessFinished(this,EventArgs.Empty);
			}

		}
		
		/// <summary>
		/// Manejador del evento RecognizingCharacteristicChecked de la base de datos de caracteres.
		/// </summary>
		/// <param name="sender">El objeto que envio el evento.</param>
		/// <param name="args">Los argumentos del evento.</param>
		private void OnProcessingStepDone(object sender,
		                                  ProcessingStepDoneArgs args)
		{
			// Lo que hacemos es notificar a la interfaz de que una determinada 
			// caracteristica binaria ha tomado un valor, y que caracteres son
			// similares.
			MessageLogSentInvoker("{0}: {1}",args.Process.GetType(), args.Result);
			string similar="";	
			if(args.SimilarSymbols!=null){
				foreach(MathSymbol ms in args.SimilarSymbols){
					similar += String.Format("«{0}»,", ms.Text);
				}				
				
				MessageLogSentInvoker("Caracteres similares: {0}",
				                 similar.TrimEnd(new char[]{','}));
			}
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
			
			database.RecognizingStepDone+=
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
		public List<MathTextDatabase> Databases {
			get {
				return databases;
			}
			
			set
			{
				databases = value;
				foreach(MathTextDatabase database in value)
				{
					database.RecognizingStepDone+=
						new ProcessingStepDoneHandler(OnProcessingStepDone);
				}
			}
		}
		
		/// <summary>
		/// Metodo que realiza el procesado de las imagenes
		/// </summary>
		public void RecognizeProcess()
		{
			MessageLogSentInvoker("=======================================");
			MessageLogSentInvoker(" Comenzando proceso de segmentado");
			MessageLogSentInvoker("=======================================");
			
		   	RecognizerTreeBuild(startNode);
		   	RecognizementProcessFinishedInvoker();
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
			node.Select();
			
			MathTextBitmap bitmap = node.MathTextBitmap;
			MessageLogSentInvoker("=======================================");
			MessageLogSentInvoker("Tratando la subimagen «{0}»",
			                      node.Name);
						
			// Lanzamos el reconocedor de caracteres para cada una de
			// las bases de datos.
			List<MathSymbol> associatedSymbols = new List<MathSymbol>();
			foreach(MathTextDatabase database in databases)
			{
				MessageLogSentInvoker("---------- «{0}» ------------",
				                      database.Description);
				                      
				
				bitmap.ProcessImage(database.Processes);
				BitmapBeingRecognizedInvoker(bitmap);
				
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
			
			
			
			// Decidimos que símbolo de los  posiblemente devuelto usuaremos.			
			
						
			// We associate all symbols to the node, so we can postargate
			// the decission step.
			node.Symbols = associatedSymbols;
			
			//Si no hemos reconocido nada, pues intentaremos segmentar el caracter.
			if(associatedSymbols.Count ==0)
			{			
				MessageLogSentInvoker("La imagen no pudo ser reconocida como un "
				                      + "simbolo por la base de datos, se tratará de "
				                      + "segmentar");
				
				List<MathTextBitmap> children = CreateChildren(bitmap);
				
				if(children.Count > 1)
				{
					MessageLogSentInvoker("La imagen se ha segmentado correctamente");
					
					//Si solo conseguimos un hijo, es la propia imagen, asi que nada
					foreach(MathTextBitmap child in children)
					{
						SegmentedNode childNode = node.AddChild(child);
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
}
