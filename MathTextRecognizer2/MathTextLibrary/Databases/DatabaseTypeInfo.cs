//Creado por: Luis Román Gutiérrez a las 13:09 de 06/19/2007

using System;

namespace MathTextLibrary.Databases
{
	/// <summary>
	/// Esta clase define un atributo para ser usado como descripción para las
	/// bases de datos de caracteres matemáticos.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true) ] 
	public class DatabaseTypeInfo : Attribute
	{
		private string description;
		private Type[] usedTypes;
		
		public DatabaseTypeInfo(string description)
		{
			this.description = description;
		}
		
		
		/// <value>
		/// Contiene la descripcion de una base de datos de caracteres
		/// matematicos.
		/// </value>
		public string Description
		{
			get
			{
				return description;
			}
			
			set
			{
				description = value;
			}
		}

		/// <value>
		/// Contiene los tipos usados por la base de datos.
		/// </value>
		public virtual System.Type[] UsedTypes {
			get {
				return usedTypes;
			}
			set{
				usedTypes=value;
			}
		}
		
		
		
	}
}