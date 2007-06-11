//Creado por: Luis Román Gutiérrez a las 17:36 de 06/10/2007

using System;
using System.Reflection;

using Gtk;

using MathTextLibrary.BitmapProcesses;

namespace MathTextBatchLearner.Assistant.BitmapProcessesHelpers
{
	/// <summary>
	/// Este widget permite editar los valores de una propiedad,
	/// cambiando según el tipo de esta dinámicamente.
	/// </summary>
	public class ProcessEditorWidget : Alignment
	{
		private Widget widget;
		private PropertyInfo info;
		private BitmapProcess process;
		private HBox layout;
		
		private ProcessEditorWidget(BitmapProcess process, PropertyInfo info) 
			: base(0f,0.5f,0f,1.0f)
		{
			this.info = info;
			this.process = process;		
		}
		
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
			else
			{
				return false;
			}
			
		}
		
		public void Apply()
		{
			if(info.PropertyType ==  typeof(int))
			{
				SpinButton spin = widget as SpinButton;
				
				info.SetValue(process, spin.ValueAsInt,null);
			}
			else if (info.PropertyType == typeof(bool))
			{
				CheckButton check = widget as CheckButton;
				
				info.SetValue(process, check.Active,null);
			}
		}
			
		
		public static ProcessEditorWidget Create(BitmapProcess process,
		                                         PropertyInfo info)
		{
			ProcessEditorWidget widget = new ProcessEditorWidget(process, info);
			if(widget.InitializeWidget())
				return widget;
			else
				return null;
		}
		
		private void CreateBoolWidget(BitmapProcessPropertyDescription desc)
		{
			CheckButton check = new CheckButton(desc.Description);
					
			check.Active = (bool)info.GetValue(process,null);
					
			widget = check;
					
		}
		                              
		private void CreateIntWidget(BitmapProcessPropertyDescription desc)
		{
			layout.Add(new Label(desc.Description+":"));
					
			SpinButton spin = new SpinButton(0,1000,1);
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
