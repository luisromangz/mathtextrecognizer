// created on 15/12/2005 at 14:43
using System;
using System.Collections.Generic;

using Gdk;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Projection
{
	/// <summary>
	/// This class provides the common ground for bitmap projections.
	/// </summary>
	public abstract class BitmapProjection
	{
		protected int [] projection;	
		
		protected ProjectionMode mode;
		
		protected List<Hole> holes;

        
        /// <summary>
        /// Creates BitmapProjection instances.
        /// </summary>
        /// <param name="mode">
        /// The <see cref="ProjectionMode"/> used for the projection.
        /// </param>
        /// <param name="mtb">
        /// The <see cref="MathTextBitmap"/> to be projected.
        /// </param>
        /// <returns>
        /// The <see cref="BitmapProjection"/> created.
        /// </returns>
		public static BitmapProjection CreateProjection(ProjectionMode mode, 
                                                        MathTextBitmap mtb)
		{
			BitmapProjection res=null;
			
			switch(mode)
			{
				case(ProjectionMode.Horizontal):
					res=new HorizontalBitmapProjection(mtb);
					break;
				case(ProjectionMode.Vertical):
					res=new VerticalBitmapProjection(mtb);
					break;
				default:
					throw new ArgumentException(
						"No puede usar None para crear una nueva projección");				
			}
			return res;
		}
		
		/// <value>
		/// Contains the projection holes.
		/// </value>
		public List<Hole> Holes
		{
			get
			{
				if(holes==null)
				{
					CreateHoles();
				}
				return holes;			
			}		
		}
		
		/// <summary>
		/// Creates the projection's hole list.
		/// </summary>
		private void CreateHoles()
		{
			holes=new List<Hole>();					
			for(int i=0;i<projection.Length;i++)
			{
				if(projection[i]==0)
				{
					int j=0;				
					// Si, esto es asi, con el punto y coma al final.
					for(j=i;j<projection.Length && projection[j]==0;j++);					
					// Cuando acaba el bucle, tenemos el comienzo y 
					// el final del hueco;
					holes.Add(new Hole(i,j-1));					
					i=j;			
				}	
			}		
		}
		
		/// <summary>
		/// <see cref="BitmapProjection"/>'s constructor
		/// </summary>
		/// <param name="image">
		/// The <see cref="MathTextBitmap"/> to be projected.
		/// </param>
		protected BitmapProjection(MathTextBitmap image)
		{		
			CreateProjection(image);
		}
		/// <value>
		/// Contains the projection size.
		/// </value>
		public int Size
		{
			get
			{
				return projection.Length;
			}		
		}

        /// <value>
        /// Contains the projection values.
        /// </value>
		public int[] Values 
        {
			get 
            {
				return projection;
			}
			set 
            {
				projection = value;
			}
		}
			
		/// <summary>
		/// Creates the projection.
		/// </summary>
		/// <param name="image">
		/// The <see cref="MathTextBitmap"/> being projected.
		/// </param>
		protected abstract void CreateProjection(MathTextBitmap image);
	}
}
