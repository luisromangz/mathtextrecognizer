
using System;

namespace MathTextLibrary.Projection
{
	
	/// <summary>
	/// This class represents a blank space in a projection.
	/// </summary>
	public class Hole
	{
		private int startPixel;
		private int endPixel;
		
		/// <summary>
		/// <see cref="Hole"/>'s constructor method.
		/// </summary>
		/// <param name="start">
		/// A <see cref="System.Int32"/> marking the hole's start.
		/// </param>
		/// <param name="end">
		/// A <see cref="System.Int32"/> marking the hole's end.
		/// </param>
		public Hole(int start,int end)
		{
			if(end<start || start<0)
			{
				throw new ArgumentException("Los limites del Hole son incorrectos!!");
			
			}
			startPixel=start;
			endPixel=end;				
		}	

		/// <value>
		/// Contains the hole's end point.
		/// </value>
		public int EndPixel
		{
			get
			{
				return endPixel;
			}
		}
		
		/// <value>
		/// Contains the hole's start point.
		/// </value>
		public int StartPixel
		{
			get
			{
				return startPixel;
			}
			
		}

		/// <value>
		/// Contains the hole's size.
		/// </value>
		public int Size
		{
			get
			{
				return EndPixel-StartPixel+1;	
			}	
		}
	}
}
