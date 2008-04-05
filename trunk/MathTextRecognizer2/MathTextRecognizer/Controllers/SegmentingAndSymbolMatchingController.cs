// created on 02/01/2006 at 12:55

using System;
using System.Threading;
using System.Collections.Generic;

using MathTextLibrary;
using MathTextLibrary.Bitmap;
using MathTextLibrary.Symbol;
using MathTextLibrary.Databases;
using MathTextLibrary.Databases.Characteristic;

using MathTextLibrary.Controllers;

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
		public event ProcessFinishedHandler RecognizeProcessFinished;
		
		/// <summary>
		/// Evento usado para notificar a la interfaz de que se ha comenzado
		/// a trabajar con un nueva pieza de la imagen.
		/// </summary>
		public event BitmapBeingRecognizedHandler BitmapBeingRecognized;
		
		//La imagen raiz que contiene la formula completa que deseamos reconocer.
		private FormulaNode startNode;
		
		/// <summary>
		/// Constructor de la clase MathTextRecognizerController, debe ser invocado
		/// en las posibles implementaciones distintas de la interfaz de usuario del
		/// reconocedor.
		/// </summary>
		public SegmentingAndSymbolMatchingController()
		{						
			databases = new List<MathTextDatabase>();						
		}
	
		/// <summary>
		/// Envolvemos el lanzamiento del evento BitmapBeingRecognized, por comodidad.
		/// </summary>
		/// <param name="bitmap">
		/// La imagen que hemos comenzado a reconocer, que sera enviada como
		/// argumentod del evento.
		/// </param>		
		protected void OnBitmapBeingRecognized(MathTextBitmap bitmap)
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
		protected void OnMessageLogSent(string msg, params object [] args)
		{
			if(MessageLogSent!=null)
			{
				MessageLogSent(this,new MessageLogSentArgs(String.Format(msg,args)));
			}
		}
		
		/// <summary>
		/// Envolvemos el lanzamiento del evento RecognizeProcessFinished, por comodidad.
		/// </summary>		
		protected void OnRecognizeProcessFinished()
		{
			if(RecognizeProcessFinished!=null)
			{
				RecognizeProcessFinished(this,EventArgs.Empty);
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
			OnMessageLogSent("{0}: {1}",args.Process.GetType(), args.Result);
			string similar="";	
			if(args.SimilarSymbols!=null){
				foreach(MathSymbol ms in args.SimilarSymbols){
					similar += String.Format("«{0}»,", ms.Text);
				}				
				
				OnMessageLogSent("Caracteres similares: {}",
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
		public FormulaNode StartNode
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
		   	RecognizerTreeBuild(startNode);
		   	OnRecognizeProcessFinished();
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
		private void RecognizerTreeBuild(FormulaNode node)
		{			
			MathTextBitmap bitmap = node.MathTextBitmap;
			OnMessageLogSent("Tratando la subimagen situada a partir de {0}",
			                 bitmap.Position);
						
			//Si no logramos reconocer nada, es el simbolo nulo, tambien sera
			//el simbolo nulo aunque hayamos podido crearle hijos.
			MathSymbol associatedSymbol;
			
			// Lanzamos el reconocedor de caracteres para cada una de
			// las bases de datos.
			List<MathSymbol> associatedSymbols = new List<MathSymbol>();
			foreach(MathTextDatabase database in databases)
			{
				bitmap.ProcessImage(database.Processes);
				OnBitmapBeingRecognized(bitmap);
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
			associatedSymbol = ChooseSymbol(associatedSymbols);
						
			// Asociamos el símbolo al nodo.
			bitmap.Symbol=associatedSymbol;	
			
			//Si no hemos reconocido nada, pues intentaremos segmentar el caracter.
			if(associatedSymbol.SymbolType == MathSymbolType.NotRecognized)
			{			
				OnMessageLogSent("La imagen no pudo ser reconocida como un "
				                 + "simbolo por la base de datos");
				
				List<MathTextBitmap> children = CreateChildren(bitmap);
				
				if(children.Count > 1)
				{
					OnMessageLogSent("La imagen se ha segmentado correctamente");
					
					//Si solo conseguimos un hijo, es la propia imagen, asi que nada
					foreach(MathTextBitmap child in children)
					{
						FormulaNode childNode = node.AddChild(child);
						RecognizerTreeBuild(childNode);						
					}
				}
				else
				{
					OnMessageLogSent("La imagen no pudo ser segmentada, el "
					                 + "símbolo queda sin reconocer");
				}
			}
			else
			{
				OnMessageLogSent("Símbolo reconocido por la base de datos como «{0}»",
				                 associatedSymbol.Text);
			}
		}
		
	
		/// <summary>
		/// Permite al usuario elegir entre varios caracteres que se hayan 
		/// reconocido para una imagen.
		/// </summary>
		/// <param name="symbols">
		/// A <see cref="List`1"/>
		/// </param>
		/// <returns>
		/// A <see cref="MathSymbol"/>
		/// </returns>
		private MathSymbol ChooseSymbol(List<MathSymbol> symbols)
		{
			if(symbols.Count == 0)
			{
				//TODO Aprender caracteres no reconocidos
				return MathSymbol.NullSymbol;
			}
			else if(symbols.Count==1)
			{
				return symbols[0];
			}
			else
			{
				//TODO Seleccion entre varios caracteres
				throw new NotImplementedException("TODO seleccion entre varios caracteres");
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
			return new List<MathTextBitmap>();
		}
	}
}
