// IRecognizingStepWidget.cs created with MonoDevelop
// User: luis at 20:51 14/04/2008

using System;
using System.Collections.Generic;

using MathTextLibrary.Databases;

namespace MathTextRecognizer.Steps
{
	
	/// <summary>
	/// Base class for the <c>MathTextLearner</c>'s widgets used for
	/// showing the recognizing steps.
	/// </summary>
	public abstract class RecognizingStepWidget : Gtk.Alignment
	{
		private MainRecognizerWindow mainWindow;
		
		/// <summary>
		/// <c>RecognizingStepWidget</c>'s constructor.
		/// </summary>
		/// <param name="window">
		/// The <c>MathTextRecognizer</c>'s main window instance.
		/// </param>
		public RecognizingStepWidget(MainRecognizerWindow window)
			: base(0.5f,0.5f,1.0f,1.0f)
		{
			this.mainWindow = window;
			
			
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
		
		
	}
}
