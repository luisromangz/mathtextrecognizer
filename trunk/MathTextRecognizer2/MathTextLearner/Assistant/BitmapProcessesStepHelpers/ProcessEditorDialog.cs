//Creado por: Luis Román Gutiérrez a las 16:31 de 06/10/2007

using System;
using System.Reflection;
using System.Collections.Generic;

using Gtk;

using MathTextCustomWidgets.CommonDialogs;

using MathTextLearner.Assistant;

using MathTextLibrary.BitmapProcesses;

namespace MathTextLearner.Assistant.BitmapProcessesStepHelpers
{
	/// <summary>
	/// Esta clase representa el diálogo encargado de editar las propiedades de
	/// los procesos de imágenes.
	/// </summary>	
	public class ProcessEditorDialog
	{
#region Atributos de interfaz de usuario

		[Glade.WidgetAttribute]
		private Dialog processEditorDialog;
		
		[Glade.WidgetAttribute]
		private VBox editVB;
		
		[Glade.WidgetAttribute]
		private Label infoLbl;
		
#endregion Atributos de interfaz de usuario
		
#region Atributos

		private BitmapProcess process;
		
		private List<ProcessEditorWidget> editorWidgets;
		
#endregion Atributos
		
		private ProcessEditorDialog(Window parent,
		                            BitmapProcess process,
		                            string desc)
		{
			Glade.XML xml =
				new Glade.XML(null,"databaseAssistant.glade","processEditorDialog",null);
			
			xml.Autoconnect(this);
			
			processEditorDialog.Modal = true;
			processEditorDialog.TransientFor = parent;
			
			this.process = process;
			
			editorWidgets =  new List<ProcessEditorWidget>();
			
			InitializeWidgets(desc);
		}
		
#region Metodos publicos
		
		public static ResponseType Show(Window parent,
		                                BitmapProcess process,
		                                string desc)
		{
			ProcessEditorDialog dlg = new
				ProcessEditorDialog(parent, process,desc);
			
			ResponseType res = dlg.Run();
			dlg.Destroy();
			
			return res;
			
		}
		
		public ResponseType Run()
		{
			ResponseType res = ResponseType.None;
			while(res == ResponseType.None)
				res= (ResponseType)processEditorDialog.Run();
			
			return res;
			
		}
		
		public void Destroy()
		{
			processEditorDialog.Destroy();
		}
		
#endregion Metodos privados
		
#region Metodos privados

		private void AddPropertyWidget(BitmapProcess p, PropertyInfo info)
		{
			ProcessEditorWidget widget = ProcessEditorWidget.Create(p,info);
			
			if(widget != null)
			{
				// Si la propiedad es una de las que configuracion del procesado
				editVB.Add(widget);
				editorWidgets.Add(widget);
			}
		}
		
		
		private void InitializeWidgets(string desc)			
		{
			infoLbl.Text = infoLbl.Text.Replace("%0",desc);
			
			foreach(PropertyInfo pinf in process.GetType().GetProperties())
			{
				AddPropertyWidget(process, pinf);
			}
		}
		
		private void OnOkBtnClicked(object e , EventArgs a)
		{
			foreach(ProcessEditorWidget w in editorWidgets)
			{
				w.Apply();
			}
			
			processEditorDialog.Respond(ResponseType.Ok);
		}
		
		private void OnResetBtnClicked(object e, EventArgs a)
		{
			OkDialog.Show(processEditorDialog,
			              MessageType.Info,"No implementado");
			processEditorDialog.Respond(ResponseType.None);
		}
				
#endregion Metodos privados
	}
}
