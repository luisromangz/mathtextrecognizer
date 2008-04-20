// DatabasePropertiesStep.cs created with MonoDevelop
// User: luis at 1:05 05/04/2008

using System;

using MathTextCustomWidgets.Dialogs;
using MathTextCustomWidgets.Widgets;

namespace MathTextLearner.Assistant
{
	
	/// <summary>
	/// Esta clase implementa el paso del asistente que establece las 
	/// propiedades de la base de datos como su descripcion, etc.
	/// </summary>
	public class DatabasePropertiesStep : PanelAssistantStep
	{
		
		private DatabaseDescritpionEditorWidget descriptionWidget;
		
			
		/// <summary>
		/// Constructor del panel de propiedades de la base de datos.
		/// </summary>
		/// <param name="parent">
		/// El asistente al que pertenece el panel.
		/// </param>
		public DatabasePropertiesStep(PanelAssistant parent) :
			base(parent)
		{
			descriptionWidget = new DatabaseDescritpionEditorWidget();
			SetRootWidget(descriptionWidget);
		}
		
#region Propiedades
		
		/// <value>
		/// Contiene la descripcion para la base de datos recien creada.
		/// </value>
		public string ShortDescription 
		{
			get
			{
				return descriptionWidget.ShortDescription;
			}
		}
		
		/// <value>
		/// Contiene la descripcion larga para la base de datos recien creada.
		/// </value>
		public string LongDescription 
		{
			get
			{
				return descriptionWidget.LongDescription;
			}
		}
#endregion Propiedades
		
#region Metodos no publicos
		
		protected override void ComputeErrors ()
		{
			if(String.IsNullOrEmpty(ShortDescription))
			{
				errors.Add("· No has escrito la descripción corta de la base de datos");
			}
			
			if(String.IsNullOrEmpty(LongDescription))
			{
				errors.Add("· No has escrito la descripción larga de la base de datos");
			}		
			
		}

#endregion Metodos no publicos
	}
}
