//Creado por: Luis Román Gutiérrez a las 13:49 de 06/07/2007

using System;

namespace MathTextLibrary.Databases
{
	/// <summary>
	/// Enumerado que representa la prioridad dada a las bases de datos
	/// a la hora de reconocer símbolos.
	/// </summary>
	public enum DatabasePriority
	{
		High,
		Medium,
		Low		
	}
	
	/// <summary>
	/// Esta clase se encarga de organizar varias bases de datos para reconocer
	/// un símbolo mátematico en ellas.
	/// Permite definir las prioridades dadas a cada base de datos a la hora de reconocer.
	/// </summary>
	public class DatabaseManager
	{
		
		public DatabaseManager()
		{
		}
	}
}
