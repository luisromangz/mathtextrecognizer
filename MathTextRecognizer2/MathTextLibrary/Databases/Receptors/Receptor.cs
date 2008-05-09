// Receptor.cs created with MonoDevelop
// User: luis at 18:17 08/05/2008

using System;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Databases.Receptors
{
	
	/// <summary>
	/// This class implements line against to check if a image intersects it.
	/// </summary>
	public class Receptor
	{
		private float x0, y0, x1, y1;
		private float left, top, right, bottom;
		
		private static Random rand = new Random(Environment.TickCount);

		/// <value>
		/// Contains the first defining point x position.
		/// </value>
		public float X0
		{
			get 
			{ 
				return x0;
			}
			set
			{
				x0 = value;
			}
			
		}
		
		/// <value>
		/// Contains the fist defining point y position.
		/// </value>
		public float Y0
		{
			get 
			{ 
				return y0; 
			}
			set
			{
				y0 = value;
			}
		}
		
		/// <value>
		/// Contains the second defining point x position.
		/// </value>
		public float X1
		{
			get 
			{ 
				return x1; 
			}
			
			set
			{
				x1 = value;
			}
				
			
		}
		
		/// <value>
		/// Contains the second defining point y position.
		/// </value>
		public float Y1
		{
			get 
			{ 
				return y1; 
			}
			
			set
			{
				y1 = value;
			}
		}

		/// <value>
		/// Contains the leftmost position of the receptor segment.
		/// </value>
		public float Left 
		{			
			get 
			{
				return left;
			}
			set
			{
				
				left = value;
			}
		}

		/// <value>
		/// Containst the highest position of the receptor segment.
		/// </value>
		public float Top 
		{
			get 
			{
				return top;
			}
			set
			{
				top = value;
			}
		}

		/// <value>
		/// Contains the rightests position of the receptor segment.
		/// </value>
		public float Right 
		{
			get 
			{
				return right;
			}
			set
			{
				right = value;
			}
		}

		/// <value>
		/// Contains the lower position of the receptor segment.
		/// </value>
		public float Bottom 
		{
			get 
			{
				return bottom;
			}
			set
			{
				bottom = value;
			}
		}
		
		/// <summary>
		/// Needed for serialization.
		/// </summary>
		public Receptor()
		{
			
		}

		/// <summary>
		/// <c>Receptor</c>'s constructor used in the generation of new 
		/// lists.
		/// </summary>
		/// <param name="x0">
		/// A <see cref="System.Single"/>
		/// </param>
		/// <param name="y0">
		/// A <see cref="System.Single"/>
		/// </param>
		/// <param name="x1">
		/// A <see cref="System.Single"/>
		/// </param>
		/// <param name="y1">
		/// A <see cref="System.Single"/>
		/// </param>
		public Receptor(float x0, float y0, float x1, float y1)
		{			
			this.x0 = x0;
			this.y0 = y0;
			this.x1 = x1;
			this.y1 = y1;

			left	= Math.Min(x0, x1);
			right	= Math.Max(x0, x1);
			top		= Math.Min(y0, y1);
			bottom	= Math.Max(y0, y1);
		}

		// Check receptor state
		public bool GetReceptorState(int x, int y, int width, int height)
		{			
			// check, if the point is in receptors bounds
			if ((x < left*width) 
			    || (y < top*height) 
			    || (x > right*width) 
			    || (y > bottom*height))
				return false;

			// check for horizontal and vertical receptors
			if ((x1 == x0) || (y1 == y0))
				return true;
			
			int y0n = (int)(y0*height);
			int y1n = (int)(y1*height);
			int x0n = (int)(x0*width);
			int x1n = (int)(x1*width);
				
			/*int a = y0n - y1n;
			int	b = x1n - x0n;
			int	c = y0n * (x0n - x1n) + x0n * (y1n - y0n);
			float	d = (float) Math.Sqrt(a * a + b * b);
			// check if the point is on the receptors line
			if (Math.Abs(a * x + b * y + c) / d < 1)
				return true;*/
			
			float k = (float) (y1n - y0n) / (float) (x1n - x0n);
			float z = (float) y0n - k * x0n;
			
			if ((int)(k * x + z - y) == 0)
				return true;
			
		

			return false;
		}
		
		/// <summary>
		/// Generates a randomized <c>Receptor</c> list.
		/// </summary>
		/// <param name="count">
		/// The number of receptors.
		/// </param>
		/// <returns>
		/// A <see cref="List`1"/> of receptors.
		/// </returns>
		public static List<Receptor> GenerateList(int count)
		{
			List<Receptor> receptors = new List<Receptor>();
			int i = 0;

			while (i < count)
			{
				float x1 = (float)rand.NextDouble();
				float y1 = (float)rand.NextDouble();
				float x2 = (float)rand.NextDouble();
				float y2 = (float)rand.NextDouble();

				float dx = x1 - x2;
				float dy = y1 - y2;
				float length = (float) Math.Sqrt(dx * dx + dy * dy);

				// avoid too short and too long receptors
				if ((length < 0.1f) || (length > 0.5f))
					continue;

				receptors.Add(new Receptor(x1, y1, x2, y2));
				i++;
			}
			
			return receptors;
		}
	
	}
}
