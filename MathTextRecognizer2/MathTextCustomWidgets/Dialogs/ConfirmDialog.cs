using System;
using Gtk;

namespace MathTextCustomWidgets.Dialogs
{	
	/// <summary>
	/// Esta clase hereda de <c>MessageDialog</c> para especializarse
	/// en mostrar preguntas del tipo "sí o no".
	/// </summary>
	public class ConfirmDialog : MessageDialog
	{
		private Window parent;
		
		/// <summary>
		/// El constructor de <c>ConfirmDialog</c>.
		/// </summary>
		/// <param name = "parent">
		/// La ventana desde la que se usa el diálogo.
		/// </param>
		/// <param name = "question">
		/// La pregunta que será mostrada.
		/// </param>
		public ConfirmDialog(Window parent, string question, params object[] args)
			: base(	parent,
					DialogFlags.DestroyWithParent,
					MessageType.Question, 
                    ButtonsType.YesNo,
                    question, args)
		{
			this.Title = "Pregunta";
			this.Modal = true;
			this.TransientFor = parent;
			this.Icon = parent.Icon;
			this.parent = parent;
		}	
		
		/// <summary>
		/// Permite esperar a que el diálogo sea cerrado o emita una
		/// respuesta, devolviendo esta respuesta
		/// </summary>
		/// <returns>
		/// La respuesta con la que se cerró el diálogo.
		/// </returns>
		public new ResponseType Run()
		{
			// We draw attention towards the app.
			parent.UrgencyHint = true;
			ResponseType res= (ResponseType)(base.Run());		
			parent.UrgencyHint = false;
			return res;
		}
		
		/// <summary>
		/// Este método muestra un diálogo de la clase <c>ConfirmDialog</c>.
		/// </summary>
		/// <param name = "parent">
		/// La ventana que usará el diálogo.
		/// </param>
		/// <param name = "question">
		/// La pregunta que mostrará el diálogo.
		/// </param>
		public static ResponseType Show(Window parent, string question, params object[] args)
		{
			ConfirmDialog dialog = new ConfirmDialog(parent, question, args);
			ResponseType res = dialog.Run();
			dialog.Destroy();
			return res;		
		}
	}
}
