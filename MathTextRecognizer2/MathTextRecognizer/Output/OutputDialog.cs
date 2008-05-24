// created on 05/01/2006 at 14:31

using System;
using System.IO;
using System.Threading;

using Gtk;
using Glade;

using MathTextLibrary;
using MathTextLibrary.Bitmap;
using MathTextLibrary.Controllers;
using MathTextLibrary.Databases.Characteristic.Characteristics;

using MathTextRecognizer.Controllers;

namespace MathTextRecognizer.Output
{
	/// <summary>
	/// Esta clase representa la venta que muestra las salidas
	/// LaTeX y MathML de la interfaz.
	/// </summary>
	public class OutputDialog
	{
		
#region Glade widgets
		[Widget]
		private TextView textviewOutput = null;	
		
		[Widget]
		private Dialog outputDialog = null;
		
#endregion Glade widgets
		
		private string output;
		
		/// <summary>
		/// Constructor de <code>OutputWindow</code>.
		/// </summary>
		/// <param name="rootBitmap">
		/// El <code>MathTextBitmap</code> reconocido para generar la salida a partir de el.
		/// </param>
		public OutputDialog(Window parent, string output)
		{
			Glade.XML gxml = new Glade.XML ("mathtextrecognizer.glade",
			                                "outputDialog");
			
			gxml.Autoconnect (this);			
			
			this.output = output;
			
			textviewOutput.Buffer.Text = output;
			
		}
	
#region Public methods		
		/// <summary>
		/// Shows the dialog, and waits for its response.
		/// </summary>
		public ResponseType Show()
		{
			ResponseType res;
			do
			{
				res = (ResponseType) (outputDialog.Run());
			}
			while(res == ResponseType.None);
			return res;
		}		
		
		/// <summary>
		/// Hides the dialog, and frees its resources.
		/// </summary>
		public void Destroy()
		{
			outputDialog.Destroy();
		}
		
#endregion Public methods
		
		/// <summary>
		/// Manejo del evento provocado al pulsar el boton "Guardar".
		/// </summary>
		private void OnBtnSaveClicked(object sender, EventArgs args)
		{
			
			FileChooserDialog fileSaveDialog=
				new FileChooserDialog("Elija el fichero donde se guardará el resultado",
				                      outputDialog,
				                      FileChooserAction.Save,
				                      "Cancelar",ResponseType.Cancel,
				                      "Guardar",ResponseType.Ok);
			
			FileFilter filter=new FileFilter();
			
			
			//Latex
			filter.Name="Archivo de LaTeX";
			filter.AddPattern("*.tex");
			filter.AddPattern("*.TEX");
		
			fileSaveDialog.AddFilter(filter);
			fileSaveDialog.Modal=true;
			fileSaveDialog.TransientFor = outputDialog;
			outputDialog.Visible=false;
			ResponseType res =(ResponseType) (fileSaveDialog.Run());
			
			if(res == ResponseType.Ok)
			{
				string path=fileSaveDialog.Filename;
			
				StreamWriter stream=new StreamWriter(path);
				
				stream.Write(textviewOutput.Buffer.Text.Trim(' ','\n'));
			
				stream.Close();
			}
			
			fileSaveDialog.Destroy();
			
		}
		
	}
}
