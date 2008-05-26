// OutputSettingsDialog.cs created with MonoDevelop
// User: luis at 12:54 25/05/2008

using System;
using System.Collections.Generic;

using Gtk;
using Glade;

using MathTextCustomWidgets.Dialogs;

using MathTextRecognizer.Config;

namespace MathTextRecognizer.Output
{
	
	/// <summary>
	/// This class implements a dialog to be able to configure the settings
	/// of the output dialog.
	/// </summary>
	public class OutputSettingsDialog
	{
		
#region Glade widgets
		[Widget]
		private Dialog outputSettingsDialog = null;
		
		[Widget]
		private CheckButton outSettingsShowOutputCheck = null;
		
		[Widget]
		private Frame outSettingsCommandFrame = null;
		
		[Widget]
		private Entry outSettingsCommandEntry = null;
		
		
		
#endregion Glade widgets
		
		/// <summary>
		/// <see cref="OutputSettingsDialog"/>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// The <see cref="Window"/> this dialog is modal to.
		/// </param>
		public OutputSettingsDialog(Window parent)
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "outputSettingsDialog");
			
			gladeXml.Autoconnect(this);
			
			this.outputSettingsDialog.TransientFor =  parent;
			
			InitializeWidgets();
			
		}
		
#region Public methods
		
		/// <summary>
		/// Shows the dialog and waits for a valid reponse.
		/// </summary>
		/// <returns>
		/// A <see cref="ResponseType"/>
		/// </returns>
		public ResponseType Show()
		{
			ResponseType res;
			do
			{
				res = (ResponseType)(outputSettingsDialog.Run());
			}
			while(res == ResponseType.None);
			
			return res;
		}
		
		/// <summary>
		/// Hides the dialog and frees its allocated resources.
		/// </summary>
		public void Destroy()
		{
			outputSettingsDialog.Destroy();
		}
		
#endregion Public methods
		
#region Non-public methods
		
		/// <summary>
		/// Initializes the dialog's widgets.
		/// </summary>
		private void InitializeWidgets()
		{
			
			Config.RecognizerConfig config = Config.RecognizerConfig.Instance;
			
			this.outSettingsShowOutputCheck.Active = 
				config.ShowOutputConversion;
			
			if(config.ShowOutputConversion)
			{
				this.outSettingsCommandEntry.Text = 
					config.OutputConversionCommand;
			}
		}
		
		/// <summary>
		/// Checks the dialog's widget for validation errors and shows a
		/// message if errors are found.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private bool HasErrors()
		{
			List<string> errors = new List<string>();
			
			if(outSettingsShowOutputCheck.Active)
			{
				// We have to check the command.
				
				string command = outSettingsCommandEntry.Text.Trim();
				if(String.IsNullOrEmpty(command))
				{
					errors.Add("· No se especificó un comando para generar una imagen.");
				}
				else if(!command.Contains("{0}")
				        || !command.Contains("{1}"))
				{
					errors.Add("· No se encontraron en el comando especificado los dos elementos por los que se reemplazarán los archivos temporales necesarios para generar la imagen ({0} y {1})");
				}
				
				try
				{
					String.Format(command, "input", "output");
				}
				catch (Exception)
				{
					errors.Add("· El comando especificado no tiene un formato válido.");
				}
			}
			
			if(errors.Count > 0)
			{
				OkDialog.Show(this.outputSettingsDialog,
				              MessageType.Warning,
				              "Para continuar, debes solucionar los siguientes errores:\n\n{0}",
				              String.Join("\n", errors.ToArray()));
				return true;
			}
			
			return false;
		}
		
#endregion Non-public methods
		
		
#region Event handlers
		
		/// <summary>
		/// Checks for validation defects, and responds Ok if everything is ok.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		[GLib.ConnectBefore]
		private void OnOutSettingsOkBtnClicked(object sender, EventArgs args)
		{
			if(HasErrors())
			{
				outputSettingsDialog.Respond(ResponseType.None);
			}
			else
			{
				
				Config.RecognizerConfig config = Config.RecognizerConfig.Instance;
				
				config.ShowOutputConversion= outSettingsShowOutputCheck.Active;
				
				config.OutputConversionCommand=
					config.ShowOutputConversion?this.outSettingsCommandEntry.Text.Trim():"";
				
				config.Save();
				
				outputSettingsDialog.Respond(ResponseType.Ok);
			}
		}
		
		/// <summary>
		/// Sensitivizes the command frame depending on the state of the checkbutton.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnOutSettingsShowOutputCheckToggled(object sender,
		                                                 EventArgs args)
		{
			outSettingsCommandFrame.Sensitive =
				outSettingsShowOutputCheck.Active;
		}
		
		/// <summary>
		/// Allows the user to search the command within a dialog.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnOutSettingsSearchCommandBtnClicked(object sender,
		                                                  EventArgs args)
		{
			
		}
		
#endregion Event handlers
	}
}
