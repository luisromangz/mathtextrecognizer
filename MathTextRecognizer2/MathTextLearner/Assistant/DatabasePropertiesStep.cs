// DatabasePropertiesStep.cs created with MonoDevelop
// User: luis at 1:05 05/04/2008

using System;

using MathTextCustomWidgets.CommonDialogs;

namespace MathTextLearner.Assistant
{
	
	/// <summary>
	/// Esta clase implementa el paso del asistente que establece las 
	/// propiedades de la base de datos como su descripcion, etc.
	/// </summary>
	public class DatabasePropertiesStep : PanelAssistantStep
	{
		
		private string description;
			
		/// <summary>
		/// Constructor del panel de propiedades de la base de datos.
		/// </summary>
		/// <param name="parent">
		/// El asistente al que pertenece el panel.
		/// </param>
		public DatabasePropertiesStep(PanelAssistant parent) :
			base(parent)
		{
		}
		
#region Propiedades
		
		/// <value>
		/// Contiene la descripcion para la base de datos recien creada.
		/// </value>
		public string Description 
		{
			get
			{
				return description;
			}
		}
#endregion Propiedades
		
#region Metodos no publicos
		
		protected override void ComputeErrors ()
		{
			errors = "";
		}

#endregion Metodos no publicos
	}
}
