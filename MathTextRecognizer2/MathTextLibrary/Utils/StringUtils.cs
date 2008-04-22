// StringUtils.cs created with MonoDevelop
// User: luis at 14:55 22/04/2008

using System;
using System.Collections.Generic;

namespace MathTextLibrary.Utils
{
	
	/// <summary>
	/// Class that implements some useful string methods.
	/// </summary>
	public class StringUtils
	{
		/// <summary>
		/// Joins a set of strings using a given string.
		/// </summary>
		/// <param name="joint">
		/// The string used to glue the pieces together.
		/// </param>
		/// <param name="pieces">
		/// The pieces to be joined.
		/// </param>
		/// <returns>
		/// The joined string.
		/// </returns>
		public static string Join(string joint, List<string> pieces)
		{
			String res ="";
			for(int i =0; i<pieces.Count-1; i++)
			{
				res+= pieces+joint;
			}
			
			res+= joint;
			return res;
		}
	}
}
