// CharacteristicVector.cs created with MonoDevelop
// User: luis at 13:44 19/04/2008

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using MathTextLibrary.Symbol;

namespace MathTextLibrary.Databases.Characteristic
{
	
	/// <summary>
	/// This class implements the index used to store the characteristic
	/// values vectors and compare them.
	/// </summary>
	[XmlInclude(typeof(MathSymbol))]
	public class CharacteristicVector
	{
		private List<bool> values;
		private List<MathSymbol> symbols;
		
		/// <summary>
		/// <c>CharacteristicVector</c>'s constructor.
		/// </summary>
		public CharacteristicVector()
		{
			values = new List<bool>();
			symbols = new List<MathSymbol>();
		}
		
#region Properties
		
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
		
		
				/// <value>
		/// Contains the binary values.
		/// </value>
		public List<bool> Values
		{
			get
			{
				return values;
			}
			set
			{
				values = value;
			}
		}

		/// <value>
		/// Contains the symbols stored in the instance.
		/// </value>
		public List<MathSymbol> Symbols 
		{
			get 
			{
				return symbols;
			}
			set
			{
				symbols = value;
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
		
#endregion Properties
		
#region Public methods
		
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
				throw new ArgumentException("Vector lengths aren't equal");
			}
			
			for(int i=0;i<values.Count; i++)
			{
				if(this.values[i]!=vector.values[i])
				{
					count++;
				}			
			}
			
			return count;
		}
		
		/// <summary>
		/// Equals method.
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public override bool Equals (object o)
		{
			if(o.GetType() != this.GetType())
				return false;
			
			CharacteristicVector vector = (CharacteristicVector) o;
			int distance = this.Distance(vector);
			
			// The vectors are the same if the distance is zero
			return distance == 0;
		}
		
		public override int GetHashCode ()
		{
			string res = "";
			foreach (bool value in values)
			{
				res += (value?1:0).ToString();
			}
			
			int resint;
			
			int.TryParse(res,out resint);
			
			return resint;
		}

		
		/// <summary>
		/// ToString method.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public override string ToString ()
		{
			string res = "[";
			foreach (bool value in values)
			{
				res += (value?1:0) +", ";
			}
			
			res = res.TrimEnd(',',' ');
			res+="]";
			return res;
		}
		
#endregion Public methods


	}
}
