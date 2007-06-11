
using System;

namespace MathTextLibrary.Projection
{
	
	/// <summary>
	/// La clase <c>Hole</c> representa un hueco (espacio de pixeles
	/// en blanco) en una proyeccion de una imagen.
	/// </summary>
	public class Hole
	{
		private int startPixel;
		private int endPixel;
		
		/// <summary>
		/// Constructor de <c>Hole</c>.
		/// </summary>
		/// <param name="start">El pixel de inicio del hueco.</param>
		/// <param name="end">El pixel de fin del hueco.</param>
		public Hole(int start,int end)
		{
			if(end<start || start<0)
			{
				throw new ArgumentException("Los limites del Hole son incorrectos!!");
			
			}
			startPixel=start;
			endPixel=end;				
		}	

		/// <summary>
		/// Esta propiedad de solo lectura permite recuperar el pixel final del hueco.
		/// </summary>
		public int EndPixel
		{
			get
			{
				return endPixel;
			}
		}
		
		/// <summary>
		/// Esta propiedad de solo lectura permite recuperar el pixel de inicio del hueco.
		/// </summary>
		public int StartPixel
		{
			get
			{
				return startPixel;
			}
			
		}

		/// <summary>
		/// Esta propiedad de solo lectura permite recuperar el pixel de inicio del hueco.
		/// </summary>
		public int Size
		{
			get
			{
				return EndPixel-StartPixel+1;	
			}	
		}
	}
}
