//Creado por: Luis Román Gutiérrez a las 19:38 de 06/07/2007

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using Gtk;

using MathTextCustomWidgets.Dialogs;

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
		private VBox optionsVB;
		
		[Glade.WidgetAttribute]
		private VBox typeRootWidget;
				
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
				new Glade.XML(null,"databaseAssistant.glade","typeRootWidget",null);
				
			gxml.Autoconnect(this);
			
			SetRootWidget(typeRootWidget);
			
			databaseTypeMap = new Dictionary<Gtk.RadioButton,System.Type>();
			
			InitializeWidgets();
		}
		
#endregion Constructor
		
#region Propiedades
		
		/// <value>
		/// Permite recuperar los procesos selecionados.
		/// </value>
		public DatabaseBase Database
		{
			get
			{
				DatabaseBase res = null;
				
				res = (DatabaseBase)selectedType.GetConstructor(new Type[0]).Invoke(null);
				
				return res;
			}
		}
		
#endregion Propiedades
		

		
#region Metodos privados
		
		/// <summary>
		/// Inicializa los controles del paso del asistente.
		/// </summary>
		private void InitializeWidgets()
		{
			RadioButton group = new RadioButton("group");	
			
			List<Type> databaseTypes = RetrieveDatabaseTypes();
			
			foreach(Type t in databaseTypes)
			{
				RadioButton databaseRadio =  
					new RadioButton(group, RetrieveDescription(t));
				
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
		
		/// <summary>
		/// Carga las clases que son bases de datos de caracteres, es decir
		/// heredan de <c>DatabaseBase</c>.
		/// </summary>
		/// <returns>
		/// Una lista con los tipos que cumplen dicha propiedad.
		/// </returns>
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
		
		/// <summary>
		/// Extrae la descripción asociada a un tipo de base de datos.
		/// </summary>
		/// <param name="t">
		/// El tipo de la base de datos de caracteres.
		/// </param>
		/// <returns>
		/// Una cadena con la descripcion del tipo.
		/// </returns>
		private string RetrieveDescription(Type t)
		{		
			object[] attributes = t.GetCustomAttributes(typeof(DatabaseTypeInfo),
			                                            true);
			
			DatabaseTypeInfo info = (DatabaseTypeInfo)attributes[0];
			return info.Description;
		}
		
		/// <summary>
		/// Calcula los errores del paso del asistente.
		/// </summary>
		protected override void ComputeErrors ()
		{
			errors = "";			
				
			if(selectedType == null)
			{
				errors += "· Debes seleccionar el tipo de la base de datos a crear.";
			}
		}

		 
#endregion Metodos privados
	}
}
