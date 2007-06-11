
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
					
			
			//assistant.AddStep(new DatabaseTypeStep(assistant));
			//assistant.AddStep(new FileSelectionStep(assistant));
			assistant.AddStep(new BitmapProcessesStep(assistant));
			                                          
			
			assistant.Run();
			assistant.Destroy();
			Application.Run();		
			
		}
	}
}
