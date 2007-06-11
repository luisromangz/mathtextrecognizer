// created on 05/01/2006 at 14:31

using System;
using System.IO;
using System.Threading;

using Gtk;
using Glade;

using MathTextLibrary;
using MathTextLibrary.Controllers;
using MathTextLibrary.Databases.Caracteristic.Caracteristics;


namespace MathTextRecognizerGUI
{
	/// <summary>
	/// Esta clase representa la venta que muestra las salidas
	/// LaTeX y MathML de la interfaz.
	/// </summary>
	public class OutputDialog
	{
		[WidgetAttribute]
		private Dialog outputDialog;
		
		[WidgetAttribute]
		private TextView textviewOutput;		
		
		[WidgetAttribute]
		private ComboBox comboOutputType;
		
		private MathTextOutputController controller;
		
		private FileChooserDialog fileSaveDialog;
		
		/// <summary>
		/// Constructor de <code>OutputWindow</code>.
		/// </summary>
		/// <param name="rootBitmap">
		/// El <code>MathTextBitmap</code> reconocido para generar la salida a partir de el.
		/// </param>
		public OutputDialog(MathTextBitmap rootBitmap)
		{
			Glade.XML gxml = new Glade.XML (null, "mathtextrecognizer.glade",
			     "outputDialog", null);
			
			gxml.Autoconnect (this);			
			
			controller=new MathTextOutputController();
			controller.StartImage=rootBitmap;
			controller.OutputCreated+=new ControllerProcessFinishedEventHandler(OnOutputCreated);
						
			controller.MakeOutput();
		}
		
		/// <summary>
		/// Para lanzar la ventana desde la ventana principal de la aplicacion.
		/// </summary>
		public void Run()
		{
			outputDialog.Run();			
			outputDialog.Hide();
		}		
		
		/// <summary>
		/// Manejo del evento provocado al pulsar el boton "Guardar".
		/// </summary>
		private void OnBtnSaveClicked(object sender, EventArgs args)
		{
			
			fileSaveDialog=new FileChooserDialog("Guardar salida",
			                                     outputDialog,
			                                     FileChooserAction.Save,
			                                     "Cancelar",ResponseType.Cancel,
			     	                             "Guardar",ResponseType.Ok);
			
			FileFilter filter=new FileFilter();
			
			switch(comboOutputType.Active)
			{
				case(0):
					//Latex
					filter.Name="Archivo de LaTeX";
					filter.AddPattern("*.tex");
					filter.AddPattern("*.TEX");
					break;
				case(1):
					//MathML
					filter.Name="Archivo MathML";
					filter.AddPattern("*.mathml");
					filter.AddPattern("*.MATHML");
					break;			
			}
			fileSaveDialog.AddFilter(filter);
			fileSaveDialog.Response += new ResponseHandler(OnSaveDialogResponse);
			fileSaveDialog.Modal=true;
			outputDialog.Visible=false;
			fileSaveDialog.Run();
			fileSaveDialog.Hide();
			outputDialog.Run();
		}
		
		/// <summary>
		/// Manejo del evento que ocurre al cerrarse el cuadro de dialogo de 
		/// guardar.
		/// </summary>
		private void OnSaveDialogResponse(object sender, ResponseArgs arg)
		{
			string path=fileSaveDialog.Filename;
			
			StreamWriter stream=new StreamWriter(path);
			
			stream.WriteLine(textviewOutput.Buffer.Text);
			
			stream.Close();
			
		}
		
		/// <summary>
		/// Metodo que maneja el evento del controlador que indica que se ha
		/// generado la salida.
		/// </summary>
		private void OnOutputCreated(object sender, EventArgs args)
		{			
			textviewOutput.Sensitive=true;
		}
		
		/// <summary>
		/// Metodo que maneja el cambio de seleccion del tipo de salida
		/// en la lista desplegable.
		/// </summary>
		private void OnComboOutputTypeChanged(object sender, EventArgs args)
		{
			switch(comboOutputType.Active)
			{
				case(0):
					textviewOutput.Buffer.Text=controller.LaTeXOutput;
					break;
				case(1):
					textviewOutput.Buffer.Text=controller.MathMLOutput;
					break;			
			}

		}
	}
}
