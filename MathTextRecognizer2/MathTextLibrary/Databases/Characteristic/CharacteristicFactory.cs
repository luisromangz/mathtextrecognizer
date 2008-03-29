//Creado por: Luis Román Gutiérrez a las 20:26 de 06/07/2007
using System;
using System.Collections.Generic;
using System.Reflection;

using MathTextLibrary.Databases.Caracteristic.Caracteristics;

namespace MathTextLibrary.Databases.Caracteristic
{
	
	
	/// <summary>
	/// Esta clase es una fabrica de caracteristicas para centralizar la
	/// responsabilidad de crear instancias de las mismas.
	/// </summary>
	/// <remarks>
	/// Las caracteristicas se crean por reflexion: se obtienen todas las
	/// clases hijas de <c>IBinaryCaracteristic</c> disponibles, de forma
	/// dinamica.
	/// </remarks>
	public class CaracteristicFactory
	{		
		/// <summary>
		/// Crea una nueva caracteristica del tipo indicado.
		/// </summary>
		/// <param name="t">Tipo de la caracteristica deseada</param>
		/// <returns>Caracteristica del tipo deseado</returns>
		public static IBinaryCaracteristic CreateCaracteristic(Type t)
		{
			return (IBinaryCaracteristic) (t.GetConstructor(new Type[]{}).Invoke(null));
		}

		/// <summary>
		/// Crea una lista con todas las caracteristicas disponibles en
		/// el sistema.
		/// </summary>
		/// <returns>Lista de todas las clases que implementan
		/// <c>IBinaryCaracteristic</c></returns>
		public static List<IBinaryCaracteristic> CreateCaracteristicList() 
		{
			List<IBinaryCaracteristic> caracteristics=new  List<IBinaryCaracteristic>();
			
			Assembly a=Assembly.GetAssembly(typeof(IBinaryCaracteristic));
			
			foreach(Type t in a.GetTypes())
			{
				if(t.BaseType == typeof(IBinaryCaracteristic))
				{
					caracteristics.Add(CaracteristicFactory.CreateCaracteristic(t));														
				}				
			}
			
			caracteristics.Sort();		

			return caracteristics;
		}
	}
}
