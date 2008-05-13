
using System;
using System.IO;

using Gtk;
using Glade;

namespace MathTextCustomWidgets.Widgets.Logger
{

	
	/// <summary> 
	/// Esta clase implementa un control en el que mostrar mensajes de
	/// depuración o que informen del progreso de una operación.
	/// </summary>
	public class LogView : Alignment
	{
	     #region Widgets

        [WidgetAttribute]
        private HBox mainBox = null;        
        
        [WidgetAttribute]
        private TextView txtLog = null;   
        
        [WidgetAttribute]
        private ScrolledWindow txtLogScroll = null;     
        
        #endregion Widgets
		
		private const int MAX_LINES=80;
		
		private bool follow;
		
		
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
			
			follow = false;
        }
        
#region Público
		
		/// <value>
		/// Contains a value indicating if the textview will be scrolled to the output.
		/// </value>
		public bool Follow 
		{
			get 
			{
				return follow;
			}
			set
			{
				follow = value;
			}
		}
        
        /// <summary>
		/// Añade un mensaje en una nueva línea tras el último mensaje añadido.
		/// </summary>
		/// <param name = "message">
		/// El mensaje a añadir.
		/// </param>
        public void LogLine(string message, params object [] args)
		{	
					
			if(txtLog.Buffer.LineCount > MAX_LINES)
			{
				TextIter start =txtLog.Buffer.GetIterAtLine(0);
				TextIter end =txtLog.Buffer.GetIterAtLine(1);
				
				txtLog.Buffer.Delete(ref start, ref end);
			}
			
			TextIter endIter = txtLog.Buffer.EndIter;
			txtLog.Buffer.Insert(ref endIter,
			                     String.Format(message+"\n",args));
			
			
			if(follow)
			{
				txtLogScroll.Vadjustment.Value = txtLogScroll.Vadjustment.Upper;
			}
				
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
		
		
#endregion Manejadores de eventos

	}
}
