// FormulaMatchingWidget.cs created with MonoDevelop
// User: luis at 12:47 09/05/2008

using System;

using Gtk;
using Glade;

using MathTextCustomWidgets.Dialogs;
using MathTextLibrary.Controllers;

using MathTextRecognizer.Controllers;

namespace MathTextRecognizer.Stages
{

	
	/// <summary>
	/// This class implements a widget to be used as interface to show 
	/// the progress in the formula matching process.
	/// </summary>
	public class ParsingStageWidget : RecognizingStageWidget
	{		
			
#region Glade widgets
		[Widget]
		private HBox parsingStageBaseWidget = null;
	
#endregion Glade widgets
	
		
#region Fields
		ParsingController controller;
		
#endregion Fields
		
		/// <summary>
		/// <see cref="ParsingStageWidget"/>'s constructor.
		/// </summary>
		/// <param name="window">
		/// The <see cref="MainRecognizerWindow"/> that contains the widget.
		/// </param>
		public ParsingStageWidget(MainRecognizerWindow window) : base(window)
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "parsingStageBaseWidget");
			
			gladeXml.Autoconnect(this);
			
			this.Add(parsingStageBaseWidget);
			
			controller = new ParsingController();
			controller.MessageLogSent += 
				new MessageLogSentHandler(OnControllerMessageLogSent);
			
			this.ShowAll();
		}
		
		/// <summary>
		/// <c>FormulaMatchingStageWidget</c>'s static fields initializer.
		/// </summary>
		static ParsingStageWidget()
		{
			widgetLabel = "Construcción de fórmulas";
		}
		
#region Public methods
			/// <summary>
		/// Set the widget to its initial state.
		/// </summary>
		public override void ResetState ()
		{
			
		}
		
			
		public override void Abort ()
		{
			controller.TryAbort();
		}
		
		
#endregion Public methods
		
#region Non-public methods		
	
		
		protected override void NextStep (ControllerStepMode mode)
		{
			controller.Next(mode);
		}
	
#endregion Non-public methods
	}
}
