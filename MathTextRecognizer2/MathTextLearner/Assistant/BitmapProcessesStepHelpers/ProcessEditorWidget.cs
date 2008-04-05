//Creado por: Luis Román Gutiérrez a las 17:36 de 06/10/2007

using System;
using System.Reflection;

using Gtk;

using MathTextLibrary.BitmapProcesses;

namespace MathTextLearner.Assistant.BitmapProcessesStepHelpers
{
	/// <summary>
	/// Este widget permite editar los valores de un parametro de un
	/// algorimo de procesado de imagenes, generando el control adecuado
	/// a cada caso.
	/// </summary>
	public class ProcessEditorWidget : Alignment
	{
		private Widget widget;
		private PropertyInfo info;
		private BitmapProcess process;
		private HBox layout;
		
		/// <summary>
		/// Constructor de la clase <c>ProcessEditorWidget</c>, es privado 
		/// porque la usamos con una factoria estatica.
		/// </summary>
		/// <param name="process">
		/// El algoritmo de proceso de imagenes al que pertenece la propiedad.
		/// </param>
		/// <param name="info">
		/// El parametro del algoritmo que el control editará.
		/// </param>
		private ProcessEditorWidget(BitmapProcess process, PropertyInfo info) 
			: base(0f,0.5f,0f,1.0f)
		{
			this.info = info;
			this.process = process;		
		}
		
		/// <summary>
		/// Inicializa el control.
		/// </summary>
		/// <returns>
		/// <c>true</c> si el parametro a editar tiene descripcion,
		/// <c>false</c> en caso contrario.
		/// </returns>
		private bool InitializeWidget()
		{
			layout = new HBox();
			layout.Spacing = 4;		
			
			object [] attr; 
			attr =
				info.GetCustomAttributes(
				                         typeof(BitmapProcessPropertyDescription),
				                         true);
			
			BitmapProcessPropertyDescription desc = null;
			try
			{ 
				// Aquí fallará si la propiedad no tiene el atributo
				desc = (BitmapProcessPropertyDescription) attr[0]; 
			}
			catch(Exception)
			{
				return false;
			}
			
			if(desc != null)
			{							           
				// Si no ha fallado, entra por aquí
				if(info.PropertyType == typeof(int))
				{
					CreateIntWidget(desc);						
				}
				else if(info.PropertyType == typeof(bool))
				{
					CreateBoolWidget(desc);					
				}
				
				layout.Add(widget);
				
				this.Add(layout);	
				
				this.ShowAll();
				
				return true;
				
			}
			
			return  false;
			
		}
		
		/// <summary>
		/// Aplica el cambio en la propiedad, estableciendo el valor
		/// establecido en el control.
		/// </summary>
		public void Apply()
		{
			if(info.PropertyType ==  typeof(int))
			{
				SpinButton spin = widget as SpinButton;
				
				info.SetValue(process, (int)spin.Value,null);
			}
			else if (info.PropertyType == typeof(bool))
			{
				CheckButton check = widget as CheckButton;
				
				info.SetValue(process, check.Active,null);
			}
		}
			
		/// <summary>
		/// Crea un control para editar una propiedad de un algoritmo de
		/// procesado de imagenes.
		/// </summary>
		/// <param name="process">
		/// El algoritmo al que pertenece la propiedad a editar.
		/// </param>
		/// <param name="info">
		/// Informacion acerca de la propiedad a editar.
		/// </param>
		/// <returns>
		/// El control, si la información del parametro contenia su descripcion,
		/// y este pudo ser creado, <c>null</c> en otro caso.
		/// </returns>
		public static ProcessEditorWidget Create(BitmapProcess process,
		                                         PropertyInfo info)
		{
			ProcessEditorWidget widget = new ProcessEditorWidget(process, info);
			if(widget.InitializeWidget())
				return widget;
			else
				return null;
		}
		
		/// <summary>
		/// Crea el control para los parametros de tipo <c>bool</c>.
		/// </summary>
		/// <param name="desc">
		/// La descripcion del parametro.
		/// </param>
		private void CreateBoolWidget(BitmapProcessPropertyDescription desc)
		{
			CheckButton check = new CheckButton(desc.Description);
					
			check.Active = (bool)info.GetValue(process,null);
					
			widget = check;
					
		}
		   
		/// <summary>
		/// Crea el control para los parametros de tipo <c>int</c>.
		/// </summary>
		/// <param name="desc">
		/// A <see cref="BitmapProcessPropertyDescription"/>
		/// </param>
		private void CreateIntWidget(BitmapProcessPropertyDescription desc)
		{
			layout.Add(new Label(desc.Description+":"));		
			
			
					
			SpinButton spin = new Gtk.SpinButton(0,1000,1);
			spin.Numeric = false;
			
			if(desc.Min != -1 && desc.Max != -1)
			{
				spin.SetRange(desc.Min,desc.Max);
			}
			
			
			int val = (int)info.GetValue(process,null);
			
			spin.Value=val;
			
			widget = spin;
			
		}
	
					                           
	}
}
