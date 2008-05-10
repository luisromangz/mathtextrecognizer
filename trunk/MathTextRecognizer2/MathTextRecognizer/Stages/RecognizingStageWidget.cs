// IRecognizingStepWidget.cs created with MonoDevelop
// User: luis at 20:51 14/04/2008

using System;
using System.Collections.Generic;

using MathTextLibrary.Databases;
using MathTextLibrary.Controllers;

namespace MathTextRecognizer.Stages
{
	
	/// <summary>
	/// Base class for the <c>MathTextLearner</c>'s widgets used for
	/// showing the recognizing steps.
	/// </summary>
	public abstract class RecognizingStageWidget : Gtk.Alignment
	{
		private MainRecognizerWindow mainWindow;
		
		protected static string widgetLabel;
		
		/// <summary>
		/// <c>RecognizingStepWidget</c>'s constructor.
		/// </summary>
		/// <param name="window">
		/// The <c>MathTextRecognizer</c>'s main window instance.
		/// </param>
		public RecognizingStageWidget(MainRecognizerWindow window)
			: base(0.5f,0.5f,1.0f,1.0f)
		{
			this.mainWindow = window;
		}
		
		/// <value>
		/// Contains the recognizing widget's label;
		/// </value>
		public static string WidgetLabel
		{
			get
			{
				return widgetLabel;
			}
		}

		/// <value>
		/// Contains the app's main window's <c>MainWindow</c> instance.
		/// </value>
		protected MainRecognizerWindow MainWindow
		{
			get
			{
				return mainWindow;
			}
		}
		
		/// <value>
		/// Contains the selected databases.
		/// </value>
		protected List<MathTextDatabase> Databases
		{
			get
			{
				return mainWindow.DatabaseManager.Databases;
			}
		}
		
		/// <value>
		/// Contains the <c>MainRecognizerWindow</c>'s log area expansion state.
		/// </value>
		protected bool LogAreaExpanded
		{
			get
			{
				return mainWindow.LogAreaExpanded;
			}
			set
			{
				mainWindow.LogAreaExpanded = value;				
			}
		}
		
		/// <summary>
		/// Writes in the <c>MathTextRecognizer</c>'s log widget;
		/// </summary>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="pars">
		/// A <see cref="System.String"/>
		/// </param>
		protected void Log(string format, params string[] pars)
		{
			mainWindow.Log(format, pars);
		}
		
		/// <summary>
		/// Resets the widget's children widget to a state suitable for a 
		/// new recognizement session.
		/// </summary>
		public abstract void ResetState();
		
		/// <summary>
		/// Selects the next recognizing step.
		/// </summary>
		/// <returns>
		/// The new stage.
		/// </returns>
		public RecognizingStageWidget NextStage()			
		{
			Gtk.Notebook parentNB = (Gtk.Notebook)(this.Parent);
			parentNB.NextPage();
			
			return (RecognizingStageWidget)(parentNB.GetNthPage(parentNB.Page));
		}
		
		
		/// <summary>
		/// Shows a message sent by the controller in the log area.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="MessageLogSentArgs"/>
		/// </param>
		protected void OnControllerMessageLogSent(object sender, 
		                                        MessageLogSentArgs args)
		{
		    // Llamamos a través de invoke para que funcione bien.			
			Gtk.Application.Invoke(sender, 
			                       args,
			                       OnControllerMessageLogSentInThread);
		}
		
		private void OnControllerMessageLogSentInThread(object sender, 
		                                                EventArgs a)
		{		   
		    Log(((MessageLogSentArgs)a).Message);
		}
		
		/// <summary>
		/// Makes the controller process more data.
		/// </summary>
		/// <param name="mode">
		/// A <see cref="ControllerStepMode"/> indicating when the controller 
		/// should stop.
		/// </param>
		protected abstract void NextStep(ControllerStepMode mode);
		
		/// <summary>
		/// Abort this widget's controller thread.
		/// </summary>
		public abstract void Abort();
		
	}
}
