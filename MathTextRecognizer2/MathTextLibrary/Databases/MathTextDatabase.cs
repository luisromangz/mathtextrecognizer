using System;
using System.Collections.Generic;

using MathTextLibrary.BitmapProcesses;

namespace MathTextLibrary.Databases
{
	
	/// <summary>
	/// Esta clase es la clase base para las distintas implementaciones de bases 
	/// de datos en las que podemos reconocer caracteres matemáticos.
	/// </summary>
	[DatabaseDescription("Descripción por defecto")]
	public abstract class MathTextDatabase
	{	
		private List<BitmapProcess> processes;
		
		
		
		public MathTextDatabase()
		{
		}		
		
		/// <summary>
		/// Esta propiedad permite establecer y recuperar la lista de los
		/// procesos de imagenes usados en la base de datos.
		/// </summary>
		public List<BitmapProcess> Processes 
		{
			get {
				return processes;
			}
			
			set{
				processes = value;
			}
		}
	
	}
	
	
	/// <summary>
	/// Esta clase define un atributo para ser usado como descripción para las bases 
	/// de datos de caracteres matemáticos.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true) ] 
	public class DatabaseDescription : Attribute
	{
		private string _description;
		
		public DatabaseDescription(string description)
		{
			_description = description;
		}
		
		public string Description
		{
			get
			{
				return _description;
			}
		}
	}
}
