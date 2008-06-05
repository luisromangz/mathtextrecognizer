// TristateCheckVector.cs created with MonoDevelop
// User: luis at 15:51 05/06/2008

using System;
using System.Collections.Generic;

using MathTextLibrary.Symbol;

namespace MathTextLibrary.Databases
{
	
	public enum TristateValue
	{
		True,
		False,
		DontCare
	}
	
	public class TristateCheckVector
	{
		List<TristateValue> values;
		private List<MathSymbol> symbols;
		
		public TristateCheckVector()
		{
			values = new List<TristateValue>();
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
		/// Contains the number of positions that shouldn't be considered
		/// in the vector.
		/// </value>
		public int DontCares
			
		{
			get
			{
				int count = 0;
				foreach (TristateValue val in values) 
				{
					if(val == TristateValue.DontCare)
					{
						count ++;
					}
				}
				
				return count;
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
		public TristateValue this[int i]
		{
			get
			{
				return values[i]; 
			}
			
			set
			{
				values[i] = value;
			}
		}
		
		/// <value>
		/// Contains the underlying collection used to store the values.
		/// </value>
		public List<TristateValue> Values
		{
			get
			{
				return values;
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
		public int Distance(TristateCheckVector vector)
		{
			int count=0;
			
			if(values.Count != vector.Length)
			{
				throw new ArgumentException("Vector lengths aren't equal");
			}
			
			for(int i=0;i<values.Count; i++)
			{				
				
				if(this.values[i] != TristateValue.DontCare
				   && this.values[i]!=vector.values[i])
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
			return this.GetHashCode() == o.GetHashCode();
		}
		
		public override int GetHashCode ()
		{
			return this.ToString().GetHashCode();
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
			foreach (TristateValue value in values)
			{
				switch(value)
				{
					case TristateValue.True:
						res += "1,";
						break;
					case TristateValue.False:
						res += "0,";
						break;
					case TristateValue.DontCare:
						res += "?,";
						break;
				}
				
			}
			
			res = res.TrimEnd(',',' ');
			res+="]";
			return res;
		}
		
		
#endregion Public methods
	}
}
