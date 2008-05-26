// created on 05/01/2006 at 14:31

using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;

using Gtk;
using Glade;

using MathTextCustomWidgets.Dialogs;
using MathTextCustomWidgets.Widgets.ImageArea;

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
		
		[Widget]
		private Frame outputOutputFrame = null;
		
		[Widget]
		private Alignment outputOutputPlaceholder = null;
		
		[Widget]
		private Alignment outputOriginalPlaceholder = null;
		
#endregion Glade widgets
		
#region Fields
		private string output;
		
		private ImageArea originalImageArea;
		
		private ImageArea outputImageArea;
		
		private MainRecognizerWindow mainWindow;
		
#endregion Fields
		
		/// <summary>
		/// Constructor de <code>OutputWindow</code>.
		/// </summary>
		/// <param name="rootBitmap">
		/// El <code>MathTextBitmap</code> reconocido para generar la salida a partir de el.
		/// </param>
		public OutputDialog(MainRecognizerWindow parent, string output)
		{
			Glade.XML gxml = new Glade.XML ("mathtextrecognizer.glade",
			                                "outputDialog");
			
			gxml.Autoconnect (this);			
			
			this.output = output;
			
			this.outputDialog.TransientFor = parent.Window;
			mainWindow = parent;
			
			InitializeWidgets();
			
		
			
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
		
#region Private methods
		
		/// <summary>
		/// Refresh the output view with the changes made in the editor.
		/// </summary>
		private void RefreshOutputView()
		{
			string tempOutput = Path.GetTempFileName()+".png";
			string tempInput = Path.GetTempFileName();
			
			StreamWriter writer = new StreamWriter(tempInput, false);
			writer.Write(textviewOutput.Buffer.Text.Trim());
			
			writer.Close();
			
		 
			
			string command = 
				String.Format(Config.RecognizerConfig.Instance.OutputConversionCommand,
				              tempInput,
				              tempOutput);
			
			Console.WriteLine(command);
			Process conversionProcess = null;
			
					
			ProcessStartInfo processInfo = new ProcessStartInfo();
			
			int idx = command.IndexOf(' ');
			
			processInfo.FileName = command.Substring(0, idx);
			processInfo.Arguments= command.Substring(idx);
			processInfo.RedirectStandardError = true;
			processInfo.UseShellExecute = false;
			
			conversionProcess = Process.Start(processInfo);
			
			
			conversionProcess.WaitForExit();
		
			
			
			
			if(conversionProcess.ExitCode ==0)
			{
				Gdk.Pixbuf outPixbuf = new Gdk.Pixbuf(tempOutput);
				this.outputImageArea.Image = outPixbuf;
			}
			else
			{
				string processOutput = conversionProcess.StandardError.ReadToEnd();
				
				mainWindow.Log("===================================================");
				mainWindow.Log(" Error al generar la previsualización de la salida");
				mainWindow.Log("===================================================");
				mainWindow.Log(processOutput);
				OkDialog.Show(this.outputDialog,
			              MessageType.Warning,
				         "Hubo un error al generar la imagen a partir de la salida, puedes encontrar la descripción en la ventana de información de proceso.");
			}
		}
		
		/// <summary>
		/// Initializes the dialog's widgets.
		/// </summary>
		private void InitializeWidgets()
		{	
			textviewOutput.Buffer.Text = output;
			
			
			originalImageArea = new ImageArea();
			originalImageArea.ImageMode = ImageAreaMode.Zoom;
			this.outputOriginalPlaceholder.Add(originalImageArea);
			
			if(Config.RecognizerConfig.Instance.ShowOutputConversion)
			{
				this.outputImageArea = new ImageArea();
				this.outputImageArea.ImageMode = ImageAreaMode.Zoom;
				
				this.outputOutputPlaceholder.Add(outputImageArea);
				
				RefreshOutputView();
			}
			else
			{
				this.outputOutputFrame.Visible=false;
			}
		}
		
#endregion Private methods
		
#region Event handlers
		
		/// <summary>
		/// Refresh
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnOutputRefreshBtnClicked(object sender, EventArgs args)
		{
			RefreshOutputView();
		}
		
		/// <summary>
		/// Asks the user for a place to store the output.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
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
		
#endregion Event handlers
		
	}
}
