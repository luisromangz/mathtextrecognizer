// CharacteristicVector.cs created with MonoDevelop
// User: luis at 13:44 19/04/2008

using System;
using System.Collections.Generic;

namespace MathTextLibrary.Databases.Characteristic
{
	
	/// <summary>
	/// This class implements the index used to store the characteristic
	/// values vectors and compare them.
	/// </summary>
	public class CharacteristicVector
	{
		private List<bool> values;
		
		/// <summary>
		/// <c>CharacteristicVector</c>'s constructor.
		/// </summary>
		public CharacteristicVector()
		{
			values = new List<bool>();
		}
		
		
		/// <value>
		/// Contains the vector's length.
		/// </value>
		public int Length
		{
			get
			{
				return values.Count;
			}
		}
		
		/// <summary>
		/// Contains the value of the vector placed in the i-st position.
		/// </summary>
		/// <param name="value">
		/// A <see cref="System.Boolean"/>
		/// </param>
		public bool this[int i]
		{
			get
			{
				return values[i]; 
			}
		}
		
		/// <summary>
		/// Adds a value to the vector.
		/// </summary>
		/// <param name="value">
		/// A <see cref="System.Boolean"/>
		/// </param>
		public void AddValue(bool value)
		{
			values.Add(value);
		}
		
		/// <summary>
		/// Calculates the distance between <c>CharacteristicVector</c>s
		/// instances.
		/// </summary>
		/// <param name="vector">
		/// The <c>CharacteristicVector</c> instance the invoking is going to 
		/// be compared to.
		/// </param>
		/// <returns>
		/// The distance between vectors, as the number of differences between 
		/// them.
		/// </returns>
		public int Distance(CharacteristicVector vector)
		{
			int count=0;
			
			if(values.Count != vector.Length)
			{
				throw new ArgumentException("Vectors' length aren't equal");
			}
			
			for(int i=0;i<values.Count; i++)
			{
				if(this[i]!=vector[i])
				{
					count++;
				}			
			}
			
			return count;
		}
	}
}
