
using System;

using Gtk;

namespace CustomGtkWidgets.CommonDialogs
{
	/// <summary>
	/// Esta clase es la base de aquellas que implementan
	/// los paneles del asistente de creación de nuevas bases
	/// de datos.
	/// </summary>
	public abstract class PanelAssistantStep
	{
		#region Atributos
		
		private Widget rootWidget;
				
		private PanelAssistant parent;
		
		protected string errors; // Debe ser creada al ejecutar HasErrors();
		
		#endregion Atributos
		
		public PanelAssistantStep(PanelAssistant parent)
		{
			this.parent = parent;
		}
		
		#region Propiedades
		
		/// <summary>
		/// Permite recuperar el asistente al que pertenece el panel.
		/// </summary>
		public PanelAssistant Assistant
		{
			get
			{
				return parent;
			}
		}
		
		/// <summary>
		/// Permite recuperar los errores de validación del paso del asistente.
		/// </summary>
		public string Errors
		{
			get
			{
				return errors;
			}
		}
		
		/// <summary>
		/// El widget que muestra los controles del «paso» del 
		/// asistente.
		/// </summary>
		public Widget StepWidget
		{
			get
			{
				return rootWidget;
			}
		}
		
	
		
		#endregion Propiedades
		
		#region Metodos protegidos
		
		protected void SetRootWidget(Widget rootWidget)
		{
			this.rootWidget = rootWidget; 
		}
		
		#endregion Metodos protegidos
		
		#region Metodos publicos
		
		/// <summary>
		/// Permite comprobar si los datos del paso del asistente son
		/// completos o correctos.
		/// </summary>
		/// <returns>
		/// <c>true</c> si los datos son correctos según el criterio definido.
		/// <c>false</c> en caso contrario.
		/// </returns>
		public abstract bool HasErrors();	
		
			
		
		/// <summary>
		/// Muestra el panel.
		/// </summary>
		public void Show()
		{
			rootWidget.ShowAll();
		}
		
		/// <summary>
		/// Oculta el panel.
		/// </summary>
		public void Hide()
		{
			rootWidget.Hide();
		}
		
		#endregion
		
	}
}
