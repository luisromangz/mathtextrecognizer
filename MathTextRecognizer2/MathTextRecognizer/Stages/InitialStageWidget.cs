// InitialStageWidget.cs created with MonoDevelop
// User: luis at 15:17 01/06/2008

using System;
using System.Collections.Generic;

using Gtk;
using Glade;

using MathTextCustomWidgets;

namespace MathTextRecognizer.Stages
{
	public class InitialStageWidget : RecognizingStageWidget
	{
		[Widget]
		private Alignment initialStageWidgetBase = null;
		
		[Widget]
		private Image blackboardImage = null;
		
		public InitialStageWidget(MainRecognizerWindow window)
			: base(window)
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "initialStageWidgetBase");
			
			gladeXml.Autoconnect(this);
			
			
			this.Add(initialStageWidgetBase);
			
			blackboardImage.Pixbuf = ImageResources.LoadPixbuf("edu_miscellaneous32");
		}
		
		static InitialStageWidget()
		{
			InitialStageWidget.widgetLabel = "Página de inicio";
		}
		
#region Public methods
		
		

		
#endregion Public methods
		
#region Non-public methods
		
		

		
#endregion Non-public methods
		
#region Event handlers
		
		private void OnEducationalModeBtnClicked(object sender, EventArgs args)
		{
			MainRecognizerWindow.CreateOCRWidget();	
			
			NextStage();
		}
		
		
#endregion Event handlers
	}
}
