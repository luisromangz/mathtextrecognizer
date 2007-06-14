
using System;

using Gtk;


using CustomGtkWidgets.CommonDialogs;
using MathTextBatchLearner.Assistant;


namespace MathTextBatchLearner
{
	/// <summary>
	/// Esta clase contiene el punto de entrada principal de la aplicación
	/// <c>MathTextBatchLearner</c>.
	/// </summary>
	public class MainClass
	{		
		public static void Main(string [] args)
		{
			Application.Init();
			PanelAssistant assistant =
				new PanelAssistant(
					null,
					"Crear nueva base de datos de caracteres matemáticos");
					
			
			assistant.AddStep(new DatabaseTypeStep(assistant));
			FileSelectionStep fileSelection = new FileSelectionStep(assistant);
			assistant.AddStep(fileSelection);
			assistant.AddStep(new BitmapProcessesStep(assistant,fileSelection.ImagesStore));
			                                          
			
			assistant.Run();
			assistant.Destroy();
			Application.Run();		
			
		}
	}
}
