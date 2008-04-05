
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
		
		/// <value>
		/// Permite recuperar el asistente al que pertenece el panel.
		/// </value>
		public PanelAssistant Assistant
		{
			get
			{
				return parent;
			}
		}
		
		/// <value>
		/// Permite recuperar los errores de validación del paso del asistente.
		/// </value>
		public string Errors
		{
			get
			{
				return errors;
			}
		}
		
		/// <value>
		/// Contiene un valor que indica si hay errores de validacion.
		/// </value>
		public bool HasErrors
		{
			get
			{
				ComputeErrors();
				return errors.Length > 0;
			}
		}
		
		/// <value>
		/// El widget que muestra los controles del «paso» del 
		/// asistente.
		/// </value>
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
		
		/// <summary>
		/// Calcula los errores del paso del asistente.
		/// </summary>
		protected abstract void ComputeErrors();
		
		#endregion Metodos protegidos
		
		#region Metodos publicos
		
		
			
		
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
