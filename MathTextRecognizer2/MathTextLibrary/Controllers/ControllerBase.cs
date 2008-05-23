// BaseController.cs created with MonoDevelop
// User: luis at 16:01 26/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Threading;

namespace MathTextLibrary.Controllers
{
	
	/// <summary>
	/// This clases is used as the basis to build all the controllers used in the
	/// application.
	/// </summary>
	public abstract class ControllerBase
	{

#region Events
		/// <summary>
		/// Evento usado para enviar un mensaje de informacion a la interfaz.
		/// </summary>
		public event MessageLogSentHandler MessageLogSent;
		
		/// <summary>
		/// Evento usado para notificar a la interfaz de que se ha terminado de
		/// realizar un proceso.
		/// </summary>
		public event EventHandler ProcessFinished;
		
		public event NodeBeingProcessedHandler NodeBeingProcessed;
		
		public event EventHandler StepDone;
		
#endregion Events
		
#region Attributes

		private Thread processThread;		
		private ControllerStepMode stepMode;
		
#endregion Attributes
		public ControllerBase()
		{
			stepMode = ControllerStepMode.UntilEnd;
		}
		
#region Properties
		/// <value>
		/// Contains the mode for the actual step.
		/// </value>
		public ControllerStepMode StepMode 
		{
			get 
			{
				return stepMode;
			}
			set 
			{
				stepMode = value;
			}
		}
#endregion Properties
		
		
#region Public methods
		
		/// <summary>
		/// Runs the next step of the process being controlled.
		/// </summary>
		/// <param name="step">
		/// The step mode for the new step.
		/// </param>
		public void Next(ControllerStepMode step)
		{
			stepMode = step;
			
			if(processThread == null || !processThread.IsAlive)
			{
				processThread = new Thread(new ThreadStart(Process));
				processThread.Start();				
			}
			else if (processThread.ThreadState == ThreadState.Suspended)
			{
				processThread.Resume();				
			}
		}
		
		/// <summary>
		/// Aborts the processing thread.
		/// </summary>
		public void TryAbort()
		{
			if(processThread!=null)
				processThread.Abort();
		}
#endregion Public methods
		
#region Protected methods
		
		
		/// <summary>
		/// This is the method which will be controlled.
		/// </summary>
		protected abstract void Process();
		
		/// <summary>
		/// This method suspends the controller's thread after each step done.
		/// </summary>
		protected void SuspendByStep()
		{
			if(StepMode == ControllerStepMode.StepByStep)
			{
				processThread.Suspend();
			}
		}
		
		/// <summary>
		/// Suspends the controller's thread after each node processing 
		/// has finished.
		/// </summary>
		protected void SuspendByNode()
		{
			if(stepMode != ControllerStepMode.UntilEnd)
			{				
				processThread.Suspend();	
				
			}

		}
		
		/// <summary>
		/// Suspends the thread inconditionally.
		/// </summary>
		protected void Suspend()
		{
			processThread.Suspend();
		}
		
		/// <summary>
		/// Envolvemos el lanzamiento del evento RecognizeProcessFinished, por comodidad.
		/// </summary>		
		protected void ProcessFinishedInvoker()
		{
			if(ProcessFinished!=null)
			{
				ProcessFinished(this,EventArgs.Empty);
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
		/// Invokes the <c>NodeBeingProcessed</c> event.
		/// </summary> 
		protected void NodeBeingProcessedInvoker(Gtk.ITreeNode node)
		{
			if(NodeBeingProcessed !=null)
				NodeBeingProcessed(this, new NodeBeingProcessedArgs(node));
		}
		
		
		/// <summary>
		/// Invokes the <c>StepDone</c> event.
		/// </summary>
		protected void StepDoneInvoker()
		{
			if(StepDone!=null)
				StepDone(this, EventArgs.Empty);
		}
		
#endregion Protected methods
	}
}
