/*
 * Created by SharpDevelop.
 * User: Luis
 * Date: 07/01/2006
 * Time: 12:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Threading;

using MathTextLibrary;
using MathTextLibrary.Bitmap;

using MathTextLibrary.TreeMaker;
using MathTextLibrary.TreeMaker.Steps;
using MathTextLibrary.Output;

namespace MathTextLibrary.Controllers
{
	/// <summary>
	/// Esta clase realiza funciones de controlador para el proceso de generar la salida
	/// MathML y LaTeX, ofreciendo una fachada a la interfaz de usuario, y proporcionando
	/// abstracion de los procesos subyacentes a la misma.
	/// </summary>
	public class OutputController
	{
		//La salida en MathML
		private string mathMLPOutput;
		//La salida en LaTeX
		private string latexOutput;
		
		//La imagen original que contiene la formula que hemos reconocido/segmentado,
		//con todos sus hijos, siendo, en efecto, la raiz de un arbol de imagenes.
		private MathTextBitmap startImage;
		
		/// <summary>
		/// Este evento se lanza al terminar uno de los distintos pasos en los que
		/// se divide el procesado del arbol de imagenes para ofrecer la salida textual.
		/// </summary>
		public event ProcessFinishedHandler StepFinished;
		
		/// <summary>
		/// Este evento se lanza al terminar el procesado del arbol de imagenes, 
		/// y haberse obtenido la representacion LaTeX y MathML.
		/// </summary>
		public event ProcessFinishedHandler OutputCreated;
		
		/// <summary>
		/// Constructor de la clase MathTextOutputController, sera invocado
		/// dese la interfaz de usuario desde la que se quiera ofrecer la 
		/// salida en texto.
		/// </summary>
		public OutputController()
		{
			
		}	
		
		/// <summary>
		/// Metodo para lanzar el evento StepFinished con mayor comodidad.
		/// </summary>
		protected void OnStepFinished()
		{
			if(StepFinished!=null)
			{
				StepFinished(this,EventArgs.Empty);
			}			
		}
		
		/// <summary>
		/// Metodo para lanzar el evento OutputCreated con mayor comodidad.
		/// </summary>
		protected void OnOutputCreated()
		{
			if(OutputCreated!=null)
			{
				OutputCreated(this,EventArgs.Empty);
			}
		}
		
		/// <value>
		/// Contiener la salida MathML generada a partir de la formula.
		/// </value>
		public string MathMLOutput
		{			
			get
			{
				return mathMLPOutput;				
			}
		}
		
		/// <value>
		/// Propiedad de solo lectura que nos permite recuperar la salida
		/// LaTeX generada a partir de la formula.
		/// </value>
		public string LaTeXOutput
		{			
			get
			{
				return latexOutput;				
			}
		}
		
		/// <value>
		/// Propiedad que nos permite establecer y recuperar la imagen que contiene la
		/// formula, una vez procesada y siendo la raiz de un arbol de imagenes.
		/// </value>
		public MathTextBitmap StartImage
		{
			get
			{
				return startImage;
			}
			set
			{					
				startImage=value;
			}			
		}		
		
		/// <summary>
		/// Este metodo es el encargado de llamar a los distintos procesados de arboles,
		/// y generar las salidas LaTeX y MathML.
		/// </summary>
		public void MakeOutput()
		{			
		
			//Cada uno de los pasos nos trata el arbol de images de forma que 
			//cambia su estructura, para que sea mas facil generar una salida
			//de forma recursiva.
			MakeInitialTree step0=new MakeInitialTree();			
			RecognizedTreeNode raiz=step0.ApplyStep(startImage);			
			OnStepFinished();
			
		
			SearchFractions step1=new SearchFractions();
			raiz=step1.ApplyStep(raiz);			
			OnStepFinished();
		
			SearchSuperAndSub step2=new SearchSuperAndSub();
			raiz=step2.ApplyStep(raiz);			
			OnStepFinished();
		
			//Una vez hemos transformado el arbol, procedemos a
			//procesarlo recursivamente, para generar la salida textual.
			MathMLGenerator mathmlgen=new MathMLGenerator(raiz);			
			LaTeXGenerator latexgen=new LaTeXGenerator(raiz);			
			mathMLPOutput=mathmlgen.ToString();
			latexOutput=latexgen.ToString();			
			OnOutputCreated();
		}
	}
}
