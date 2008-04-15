// RecognizingNodeActionDialog.cs created with MonoDevelop
// User: luis at 0:46 15/04/2008

using System;
using Gtk;

namespace MathTextRecognizer.Steps
{
	
	/// <summary>
	/// Esta clase implementa el dialogo usado para preguntar a un usuario
	/// la acción adecuada al activar un nodo en la vista de arbol de
	/// <c>SegmentingAndMatchingStepWidget</c>.
	/// </summary>
	public class FormulaNodeActionDialog
	{
		[Glade.WidgetAttribute]
		private Dialog formulaNodeActionDialog;
		
		[Glade.WidgetAttribute]
		private Button okButton;
		
		[Glade.WidgetAttribute]
		private Button cancelButton;
		
		[Glade.WidgetAttribute]
		private RadioButton learnImageRB;
		
		[Glade.WidgetAttribute]
		private RadioButton saveImageRB;
		
		[Glade.WidgetAttribute]
		private RadioButton editLabelRB;

#region Metodos publicos
		
		/// <summary>
		/// Constructor de la clase <c>FormulaNodeActionDialog</c>.
		/// </summary>
		/// <param name="parent">
		/// La ventana de la que el dialogo es modal.
		/// </param>
		/// <param name="showLearnOption">
		/// Indica si debe mostrarse o no la opción de aprender la imagen 
		/// del nodo.
		/// </param>
		public FormulaNodeActionDialog(Window parent, bool showLearnOption)
		{
			Glade.XML gxml = new Glade.XML("mathtextrecognizer.glade",
			                               "formulaNodeActionDialog");
			
			gxml.Autoconnect(this);
			
			formulaNodeActionDialog.TransientFor = parent;
			
			formulaNodeActionDialog.AddActionWidget(okButton,
			                                        ResponseType.Ok);
			
			formulaNodeActionDialog.AddActionWidget(cancelButton,
			                                        ResponseType.Cancel);
			
			learnImageRB.Visible = showLearnOption;
		}
		
		/// <summary>
		/// Muestra el dialogo.
		/// </summary>
		/// <returns>
		/// La respuesta del dialogo.
		/// </returns>
		public ResponseType Show()
		{
			return (ResponseType) (formulaNodeActionDialog.Run());
		}
		
		/// <summary>
		/// Libera los recursos del dialogo.
		/// </summary>
		public void Destroy()			
		{
			formulaNodeActionDialog.Destroy();
		}
		
#endregion Metodos publicos
		
#region Metodos privados
		
		/// <summary>
		/// Maneja el evento provocado al hacer click en el boton de cancelar
		/// del dialogo.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnCancelButtonClicked(object sender, EventArgs arg)
		{
			formulaNodeActionDialog.Respond(ResponseType.Cancel);
		}
		
		/// <summary>
		/// Maneja el evento provocado al hacer click en el boton de aceptar
		/// del dialogo.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnOkButtonClicked(object sender, EventArgs arg)
		{
			formulaNodeActionDialog.Respond(ResponseType.Ok);
		}
#endregion Metodos privados
	}
}
