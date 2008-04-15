
using System;
using System.Collections.Generic;

using Gtk;

using MathTextCustomWidgets;
using MathTextCustomWidgets.CommonDialogs;

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
		
		public NewDatabaseAsisstant(Window parent) : 
			base(parent, "Asistente de nueva base de datos de caracteres")
		{
			databaseStep = new DatabaseTypeStep(this);
			fileStep = new FileSelectionStep(this);
			processesStep = new BitmapProcessesStep(this, fileStep.ImagesStore);
			
			this.AddStep(databaseStep);		
			this.AddStep(fileStep);
			this.AddStep(processesStep);
			
			
			foreach(PanelAssistantStep panel in Steps)
			{
				panel.StepWidget.SetSizeRequest(500,250);
			}
			
			this.Window.Icon = ImageResources.LoadPixbuf("database-new16");
		}
		
#region Propiedades 
		
		/// <value>
		/// Contiene la base de datos creada por el asistente.
		/// </value>
		public MathTextDatabase Database
		{
			get{
				return CreateDatabase();
			}
		}
		
		/// <value>
		/// Contiene las imagenes para aprender seleccionadas en 
		/// el asistente.
		/// </value>
		public List<Gdk.Pixbuf> Images
		{
			get{
				return RetrieveImages();
			}
		}
#endregion
		
#region Metodos privados
		
		private MathTextDatabase CreateDatabase()
		{
			MathTextDatabase mtd = new MathTextDatabase();
			
			mtd.Database = databaseStep.Database;
			mtd.Processes = processesStep.Processes;
			
			return mtd;
		}
		
		private List<Gdk.Pixbuf> RetrieveImages()
		{
			
			return fileStep.Images;
		}

#endregion Metodos privados
	}
}
