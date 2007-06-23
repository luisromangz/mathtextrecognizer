//Creado por: Luis Román Gutiérrez a las 13:09 de 06/19/2007

using System;

namespace MathTextLibrary.Databases
{
	/// <summary>
	/// Esta clase define un atributo para ser usado como descripción para las
	/// bases de datos de caracteres matemáticos.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true) ] 
	public class DatabaseInfo : Attribute
	{
		private string description;
		private Type[] usedTypes;
		
		public DatabaseInfo(string description)
		{
			this.description = description;
		}
		
		
		/// <summary>
		/// Esta propiedad permite recupera la descripcion de una base 
		/// de datos de caracteres matematicos.
		/// </summary>
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