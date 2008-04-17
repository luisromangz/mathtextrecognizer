
using System;
using Gtk;

namespace MathTextCustomWidgets.Dialogs
{
	
	/// <summary>
	/// Esta clase hereda de <c>MessageDialog</c>, ofreciendo un 
	/// cuadro de diálogo con un único botón.
	/// </summary>
	public class OkDialog : MessageDialog
	{
		/// <summary>
		/// El constructor de <c>OkDialog</c>.
		/// </summary>
		/// <param name = "parent">
		/// La ventana que usa el diálogo.
		/// </param>
		/// <param name = "type">
		/// El tipo del mensaje a mostrar.
		/// </param>
		/// <param name = "message">
		/// El mensaje que se mostrará en el diálogo.
		/// </param>
		public OkDialog(Window parent, MessageType type, string message, params object[] args)
			: base(	parent,
					DialogFlags.DestroyWithParent,
					type,
					ButtonsType.Ok,
					message, args)
		{
			this.Modal = true;
			this.TransientFor = parent;
			this.Icon = parent.Icon;
			
			switch(type)
			{
				case MessageType.Error:
					this.Title = "Error";
					break;
				case MessageType.Info:				
					this.Title = "Información";
					break;
				case MessageType.Warning:
					this.Title = "Advertencia";
					break;
				case MessageType.Question:
					throw new ArgumentException(
						"Don't use this class for a question dialog");		
				
			}
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
			return (ResponseType)(base.Run());		
		}
		
		
		/// <summary>
		/// Muestra un diálogo del tipo <c>OkDialog</c>-
		/// </summary>
		/// <param name = "parent">
		/// La ventana que usa el diálogo.
		/// </param>
		/// <param name = "type">
		/// El tipo del mensaje a mostrar.
		/// </param>
		/// <param name = "message">
		/// El mensaje que se mostrará en el diálogo.
		/// </param>
		public static void Show(Window parent, MessageType type, string message, params object[] args)
		{			
			OkDialog dialog = new OkDialog(parent,type,message, args);
			dialog.Run();
			dialog.Destroy();
		}
		
	}
}
