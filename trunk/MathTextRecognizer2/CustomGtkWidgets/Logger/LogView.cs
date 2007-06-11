
using System;
using System.IO;

using Gtk;
using Glade;

namespace CustomGtkWidgets.Logger
{

	
	/// <summary> 
	/// Esta clase implementa un control en el que mostrar mensajes de depuración
	/// o que informen del progreso de una operación.
	/// </summary>
	public class LogView : Alignment
	{
	     #region Widgets
        [WidgetAttribute]
        private Window logViewWidget;
        
        [WidgetAttribute]
        private VBox rightBox;
        
        [WidgetAttribute]
        private HBox mainBox;        
        
        [WidgetAttribute]
        private TextView txtLog;   
        
        [WidgetAttribute]
        private ScrolledWindow txtLogScroll;     
        
        #endregion Widgets
		
		
		/// <summary>
		/// Constructor de la clase LogView.
		/// </summary>
		public LogView()
		    : base(0,0,1,1)
		{
		    // Importamos no a partir de una ventana, sino a parter de uno 
		    // de sus objetos hijos, porque lo que queremos crear es un widget.
		    Glade.XML gxml = new Glade.XML (null,"gui.glade", "mainBox", null);
            gxml.Autoconnect (this);
            
            
            this.Add(mainBox);
            
            txtLog.Buffer.Changed += OnTxtLogChanged;
        }
        
        #region Público
        
        /// <summary>
		/// Añade un mensaje en una nueva línea tras el último mensaje añadido.
		/// </summary>
		/// <param name = "message">
		/// El mensaje a añadir.
		/// </param>
        public void LogLine(string message, params object [] args)
		{	
						
			txtLog.Buffer.Insert(
				txtLog.Buffer.EndIter,
				(String.Format(message,args))+"\n");	
			
			txtLogScroll.Vadjustment.Value = txtLogScroll.Vadjustment.Upper;
		}
		
		/// <summary>
		/// Limpia el registro de mensajes.
		/// </summary>
		public void ClearLog()
		{
			txtLog.Buffer.Clear();
		}
		
		#endregion Público
		
		#region Manejadores de eventos
		
		private void OnBtnClearClicked(object sender, EventArgs a)
		{
		    ClearLog();
		}
		
		private void OnBtnSaveAsClicked(object sender, EventArgs a)
		{
		    LogSaveDialog lsd = new LogSaveDialog();
		    if(lsd.Run() == ResponseType.Ok)
		    {
		    	string filename = lsd.Filename;
		    	Console.WriteLine("Archivo: "+lsd.Filename);
		    	
		    	// Si no tiene extensión se la añadimos.
		    	
		    	if(!System.IO.Path.HasExtension(filename))
		    	{
		    		filename += ".log";
		    	}
		    	
		    	// Guardamos
		    	
		    	using(FileStream file = new FileStream(filename, FileMode.Create))
		    	{
		    		StreamWriter writer = new StreamWriter(file);
		    		writer.Write(txtLog.Buffer.Text);
		    		writer.Close();
		    	}
		    	
		    }
		    
		    lsd.Destroy();
		}
		
		private void OnTxtLogChanged(object sender, EventArgs a)
		{
		    // Activamos los botones de la derecha cuando hay texto.
		    rightBox.Sensitive = txtLog.Buffer.Text.Trim() != ""; 
		        
		}
		
		#endregion Manejadores de eventos

	}
}
