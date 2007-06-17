//Creado por: Luis Román Gutiérrez a las 21:30 de 06/08/2007

using System;
using System.Collections.Generic;

using Gtk;

using CustomGtkWidgets.CommonDialogs;

namespace MathTextLearner.Assistant.BitmapProcessHelpers
{
	/// <summary>
	/// Esta clase implementa el panel que permite seleccionar
	/// imagenes en el asistente de creación de bases de datos.
	/// </summary>
	public class ProcessSelectorDialog
	{
	
		#region Controles de Glade
		
		[Glade.WidgetAttribute]
		private Dialog processSelectorDialog;
		
		[Glade.WidgetAttribute]
		private Button okBtn;
		
		[Glade.WidgetAttribute]
		private ComboBox bitmapProcessCmb;			
		
		#endregion Controles de Glade
		
		#region Atributos
		
		private Type selectedProcess;
		
		Type [] types;
			
		#endregion Atributos
		
		#region Constructor
		
		public ProcessSelectorDialog(Window parent, Dictionary<Type,string> processTypes)			
		{
			Glade.XML gxml =
				new Glade.XML(null,"databaseAssistant.glade","processSelectorDialog",null);
				
			gxml.Autoconnect(this);				
			
			processSelectorDialog.TransientFor = parent;
			processSelectorDialog.Modal = true;
			
			types = new Type[processTypes.Count];
			processTypes.Keys.CopyTo(types,0);
			
			InitializeWidgets(processTypes);
		}
		
		#endregion Constructor
		
		#region Metodos públicos
		
		public void Destroy()
		{
			processSelectorDialog.Destroy();
		}
			
			
		public ResponseType Run()
		{
			// Evitamos responder none.
			ResponseType res = ResponseType.None;
			while(res == ResponseType.None)
				res= (ResponseType)(processSelectorDialog.Run());
			
			return res; 
		}
		
		
		
		#endregion Metodos públicos
			
		#region Propiedades
		
		public System.Type SelectedProcess
		{
			get
			{
				return selectedProcess;
			}
		}
		
		#endregion Propiedades
		
		#region Metodos privados
		
		private void InitializeWidgets(Dictionary<Type,string> processes)
		{	
			
			bitmapProcessCmb.RemoveText(0);				
			
			
			foreach (string desc in processes.Values)
			{
				bitmapProcessCmb.AppendText(desc);				
			}
		}
		
		private void OnBitmapProcessCmbChanged(object e, EventArgs a)
		{
			// El tipo a devolver es el que tiene su indice seleccionado.
			
			selectedProcess = types[bitmapProcessCmb.Active];
		}
		
		private void OnOkBtnClicked(object e, EventArgs a)
		{
			if(selectedProcess != null)
			{
				// Salimos aceptando.				
				processSelectorDialog.Respond(ResponseType.Ok);
			}
			else
			{
				OkDialog.Show(processSelectorDialog,
				              MessageType.Info,
				              "Debe seleccionar el tipo de proceso a añadir.");
				
				processSelectorDialog.Respond(ResponseType.None);
			}
			
		}
		
		#endregion Metodos privados
	}
}
