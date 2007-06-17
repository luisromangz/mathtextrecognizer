
using System;
using System.Collections.Generic;

using Gtk;

using CustomGtkWidgets.CommonDialogs;

using MathTextLibrary.Databases;

namespace MathTextLearner.Assistant
{
	/// <summary>
	/// Esta clase contiene el punto de entrada principal de la aplicación
	/// <c>MathTextBatchLearner</c>.
	/// </summary>
	public class NewDatabaseAsisstant : PanelAssistant
	{				
		
		private DatabaseTypeStep databaseStep;
		private FileSelectionStep fileStep;
		private BitmapProcessesStep processesStep;
		
		public NewDatabaseAsisstant(Window parent) : base(parent, "Asistente de nueva base de dato de caracteres")
		{
			databaseStep = new DatabaseTypeStep(this);
			fileStep = new FileSelectionStep(this);
			processesStep = new BitmapProcessesStep(this, fileStep.ImagesStore);
			
			this.AddStep(databaseStep);		
			this.AddStep(fileStep);
			this.AddStep(processesStep);
			
		}
		
#region Propiedades 
		
		public MathTextDatabase Database
		{
			get{
				return CreateDatabase();
			}
		}
		
		public List<Gdk.Pixbuf> Images{
			get{
				return RetrieveImages();
			}
		}
#endregion
		
#region Metodos privados
		
		private MathTextDatabase CreateDatabase()
		{
			// TODO Implementar la creacion de bases de datos con el asistente.
			return null;
		}
		
		private List<Gdk.Pixbuf> RetrieveImages()
		{
			return null;
		}

#endregion Metodos privados
	}
}
