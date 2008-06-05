//Creado por: Luis Román Gutiérrez a las 20:26 de 06/07/2007
using System;
using System.Collections.Generic;
using System.Reflection;

using MathTextLibrary.Databases.Characteristic.Characteristics;

namespace MathTextLibrary.Databases.Characteristic
{
	
	
	/// <summary>
	/// Esta clase es una fabrica de caracteristicas para centralizar la
	/// responsabilidad de crear instancias de las mismas.
	/// </summary>
	/// <remarks>
	/// Las caracteristicas se crean por reflexion: se obtienen todas las
	/// clases hijas de <c>IBinaryCharacteristic</c> disponibles, de forma
	/// dinamica.
	/// </remarks>
	public class CharacteristicFactory
	{		
		/// <summary>
		/// Crea una nueva caracteristica del tipo indicado.
		/// </summary>
		/// <param name="t">Tipo de la caracteristica deseada</param>
		/// <returns>Characteristica del tipo deseado</returns>
		public static BinaryCharacteristic CreateCharacteristic(Type t)
		{
			return (BinaryCharacteristic) (t.GetConstructor(new Type[]{}).Invoke(null));
		}

		/// <summary>
		/// Crea una lista con todas las caracteristicas disponibles en
		/// el sistema.
		/// </summary>
		/// <returns>Lista de todas las clases que implementan
		/// <c>IBinaryCharacteristic</c></returns>
		public static List<BinaryCharacteristic> CreateCharacteristicList() 
		{
			List<BinaryCharacteristic> characteristics=new  List<BinaryCharacteristic>();
			
			Assembly a=Assembly.GetAssembly(typeof(BinaryCharacteristic));
			
			foreach(Type t in a.GetTypes())
			{
				if(t.BaseType == typeof(BinaryCharacteristic))
				{
					characteristics.Add(CharacteristicFactory.CreateCharacteristic(t));														
				}				
			}
			
			characteristics.Sort();		

			return characteristics;
		}
	}
}
