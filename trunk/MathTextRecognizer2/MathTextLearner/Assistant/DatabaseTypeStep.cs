//Creado por: Luis Román Gutiérrez a las 19:38 de 06/07/2007

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using Gtk;

using CustomGtkWidgets.CommonDialogs;

using MathTextLibrary.Databases;

namespace MathTextLearner.Assistant
{
	/// <summary>
	/// Esta clase implementa el panel que permite seleccionar el 
	/// tipo de base de datos a crear en el asistente.
	/// </summary>
	public class DatabaseTypeStep : PanelAssistantStep
	{
	
		#region Controles de Glade
		
		[Glade.WidgetAttribute]
		private Frame databaseTypeFrame;		
		
		[Glade.WidgetAttribute]
		private VBox optionsVB;
				
		#endregion Controles de Glade
		
		#region Atributos
		
		private ListStore fileStore;
		
		private Type selectedType;
		
		private Dictionary<RadioButton,Type> databaseTypeMap;
		
		#endregion Atributos
		
		#region Constructor
		
		public DatabaseTypeStep(PanelAssistant assistant) 
			: base(assistant)
		{
			Glade.XML gxml =
				new Glade.XML(null,"databaseAssistant.glade","databaseTypeFrame",null);
				
			gxml.Autoconnect(this);
			
			SetRootWidget(databaseTypeFrame);
			
			databaseTypeMap = new Dictionary<Gtk.RadioButton,System.Type>();
			
			InitializeWidgets();
		}
		
		#endregion Constructor
		
		#region Metodos públicos
		
		public override bool HasErrors ()
		{
			errors = "";			
			
			if(selectedType == null)
			{
				errors += "· Debe seleccionar el tipo de la base de datos a crear.";
			}
			
			return errors.Length > 0;
		}
		
		#endregion Metodos públicos
		
		#region Metodos privados
		
		private void InitializeWidgets()
		{
			RadioButton group = new RadioButton("group");	
			
			List<Type> databaseTypes = RetrieveDatabaseTypes();
			
			foreach(Type t in databaseTypes)
			{
				RadioButton databaseRadio =  
					new RadioButton(
					                group,
					                RetrieveDescription(t));
				
				databaseRadio.Clicked += OnDatabaseTypeSelected; 

				databaseTypeMap.Add(databaseRadio,t);
				
				optionsVB.Add(databaseRadio);
			}
		}
		
		private void OnDatabaseTypeSelected(object sender, EventArgs arg)
		{
			RadioButton button = (RadioButton) sender;
			selectedType = databaseTypeMap[button];
		}
		
		private List<Type> RetrieveDatabaseTypes()
		{
			// Recuperamos el ensamblado donde esta DatabaseBase
			Assembly ass = Assembly.GetAssembly(typeof(DatabaseBase));
			List<Type> databaseTypes = new List<Type>();
			
			// Procesamos las clases que extienden DatabaseBase
			foreach(Type t in ass.GetTypes())
			{
				if(t.BaseType == typeof(DatabaseBase))
				{
					databaseTypes.Add(t);					
				}
			}
			
			
			return databaseTypes;
		}
		
		private string RetrieveDescription(Type t)
		{		
			object[] attributes = t.GetCustomAttributes(typeof(DatabaseInfo),
			                                            true);
			
			DatabaseInfo info = (DatabaseInfo)attributes[0];
			return info.Description;
		}
		
		 
		
		#endregion Metodos privados
	}
}
