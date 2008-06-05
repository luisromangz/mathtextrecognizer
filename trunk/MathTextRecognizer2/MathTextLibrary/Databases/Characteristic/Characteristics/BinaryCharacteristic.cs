using System;
using System.Collections.Generic;
using System.Reflection;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Databases.Characteristic.Characteristics
{
	
	
	/// <summary>
	/// Esta clase abstracta es la base de todas las caracteristicas binarias.
	/// </summary>
	/// <remarks>
	/// <para>Las caracteristicas binarias tienen una prioridad que determina su posicion
	/// en la lista de caracteristicas creada a traves de
	/// <c>CharacteristicFactory.CreateCharacteristicList()</c>.</para>
	/// <para>La prioridad es mayor cuanto menor sea, es decir, una caracteristica
	/// de prioridad 5 aparecera antes en la lista de caracteristicas que otra
	/// de prioridad 10.</para>
	/// </remarks>	
	public abstract class BinaryCharacteristic : IComparable, ISymbolProcess
	{
		protected int priority;
		
		public BinaryCharacteristic()
		{
						
		}
		
		/// <value>
		/// Contiene la prioridad de la caracteristica, que influye en el orden
		/// en que se aplican.
		/// </value>
		public int Priority
		{
			get
			{
				return priority;
			}
		}
		
		/// <summary>
		/// Este metodo aplica la caracteristica binaria a una imagen,
		/// y es implementado por cada clase hija concreta.
		/// </summary>
		/// <returns><c>true</c> si se cumple la caracteristica binaria
		/// en la imagen</returns>
		public abstract bool Apply(FloatBitmap image);
		
		public virtual int CompareTo(object obj){			
			return priority-((BinaryCharacteristic)obj).priority;
		}		
	}
	
}
