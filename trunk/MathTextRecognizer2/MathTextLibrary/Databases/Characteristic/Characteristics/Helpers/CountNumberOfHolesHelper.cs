using System;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Databases.Characteristic.Characteristics.Helpers
{
	/// <summary>
	/// Esta clase cuenta el numero de agujeros en una imagen.
	/// </summary>
	/// <remarks>
	/// Se distingue entre agujeros grandes y pequeños. Un agujero se
	/// considera grande si supera un cierto numero de pixeles.
	/// </remarks>
	public class CountNumberOfHolesHelper
	{
		//Se considera agujero grande al que tenga mas de pixelLimit pixeles
		private const int pixelLimit=4; 
		
		public CountNumberOfHolesHelper()
		{
		}
		
		/// <summary>
		/// Cuenta el numero de zonas blancas no conexas en la imagen.
		/// </summary>
		/// <remarks>
		/// El numero de agujeros en la imagen sera el numero de zonas blancas
		/// menos uno (el fondo blanco sobre el que esta el caracter).
		/// </remarks>
		/// <param name="image">Imagen sobre la que se trabaja</param>
		/// <param name="bigholes">Si <c>true</c> solo se cuentan los
		/// agujeros grandes</param>
		/// <returns>Numero de zonas blancas en la imagen</returns>
		public static int NumWhiteZones(FloatBitmap image, bool bigholes)
		{
			int sizeC = image.Height;
			int sizeR = image.Width;
			
			//Contiene las etiquetas de cada pixel
			int[,] label = new int[sizeR,sizeC];
			//Contiene las listas con las equivalencias entre etiquetas
			Dictionary<int, List<int>> equiv = new Dictionary<int, List<int>> ();
						
			int aux=1;
			
			/*Se analiza el primer elemento de la primera fila*/
			if (image[0,0] == FloatBitmap.White)
			{
				label[0,0]=aux;
			}
			
			
			
			/*Se analiza el resto de la primera fila*/
			for(int i=1;i<sizeR;i++)
			{
				if (image[i,0]==FloatBitmap.White && image[i-1,0]==FloatBitmap.White)
				{
					label[i,0]=aux;
				}
				else if (image[i,0]==FloatBitmap.White && image[i-1,0]!=FloatBitmap.White)
				{
					label[i,0]=++aux;
				}
			}
			
			
			
			/*Se analiza el resto de la primera columna*/
			for(int i=1;i<sizeC;i++)
			{
				if (image[0,i]==FloatBitmap.White && image[0,i-1]==FloatBitmap.White)
				{
					label[0,i]=aux;
				}
				else if (image[0,i]==FloatBitmap.White && image[0,i-1]!=FloatBitmap.White)
				{
					label[0,i]=++aux;
				}
			}

			/*Finalmente el resto*/
			for(int i=1;i<sizeR;i++)
				for(int j=1;j<sizeC;j++)
				{
					if(image[i,j]==FloatBitmap.White)
					{
						if(image[i-1,j]==FloatBitmap.White 
						   && image[i,j-1]==FloatBitmap.White)
						{
							label[i,j]=label[i-1,j];
							AddLabelsToEquiv(equiv,label[i-1,j],label[i,j-1]);
						}
						else if(image[i-1,j]==FloatBitmap.White)
						{
							label[i,j]=label[i-1,j];
						}
						else if (image[i,j-1]==FloatBitmap.White)
						{
							label[i,j]=label[i,j-1];
						}
						else 
						{
							label[i,j]=++aux;
						}
					}
				}
				
			

			/*Ahora hay que tratar las equivalencias*/
			for(int i=0;i<sizeR;i++)
				for(int j=0;j<sizeC;j++)
					label[i,j]=MinLabel(equiv,label[i,j]);
			
			int numetiq=0;
			
			if(bigholes)
			{
				// Contamos el número de píxeles de cada agujero */
				Dictionary<int, int> bigHolesAux = new Dictionary<int, int>();
				
				for(int i=0;i<sizeR;i++)
				{
					for(int j=0;j<sizeC;j++)
					{
						if(bigHolesAux.ContainsKey(label[i,j]))
						{
							int n=(int)bigHolesAux[label[i,j]];
							n++;
							bigHolesAux[label[i,j]]=n;
						}
						else
						{
							bigHolesAux.Add(label[i,j],1);
						}
					}
				}
				
				// Contamos cuantos agujeros gordos hay
				foreach(int i in bigHolesAux.Values)
				{
					if(i>pixelLimit) 
						numetiq++;
				}
			}
			else
			{
				/* Contamos el número de etiquetas de TODOS los agujeros */
				List<int> differentLabels = new List<int>();
				for(int i=0;i<sizeR;i++)
					for(int j=0;j<sizeC;j++) 
					{
						if(!differentLabels.Contains(label[i,j])) 
						{
							differentLabels.Add(label[i,j]);
							numetiq++;
						}
					}
			}
				
			// no contamos la etiqueta 0 de los pxeles negros
			return numetiq-1;
		}
		
		/// <summary>
		/// Añade la equivalencia label1=label2 en la tabla de equivalencias.
		/// </summary>
		/// <param name="equiv">Equivalencias de etiquetas</param>
		/// <param name="label1">Etiqueta</param>
		/// <param name="label2">Etiqueta a añadir</param>
		private static void AddLabelsToEquiv(Dictionary<int, List<int>> equiv, 
			                                 int label1, int label2)
		{
			List<int> aux=new List<int>();
			
			AddLabelsToEquivAux(equiv, aux, label1, label2);
			AddLabelsToEquivAux(equiv, aux, label2, label1);
			
			foreach(int i in aux)
			{
				equiv[i]=aux;
			}
		}
		
		/// <summary>
		/// Añade adecuadamente las etiquetas label1 y label2
		/// en la tabla de equivalencias.
		/// </summary>
		/// <param name="equiv">Equivalencias de etiquetas</param>
		/// <param name="aux">Lista auxiliar creada en <c>AddLabelsToEquiv()</c></param>
		/// <param name="label1">Etiqueta</param>
		/// <param name="label2">Etiqueta a añadir</param>
		private static void AddLabelsToEquivAux(Dictionary<int, List<int>>  equiv, 
			                                    List<int> aux, 
			                                    int label1, int label2)
		{
			aux.Add(label2);
			if(equiv.ContainsKey(label1))
			{
				List<int> l = equiv[label1];
				foreach(int i in l)
				{
					if(!aux.Contains(i))
						aux.Add(i);
				}
			}
			else
			{
				List<int> l=new List<int>();
				l.Add(label2);
				equiv.Add(label1, l);
			}
		}
		
		/// <summary>
		/// Encuentra la mínima etiqueta en el mapa de equivalencias que
		/// sea equivalente a <c>label</c>.
		/// </summary>
		/// <param name="equiv">Equivalencias de etiquetas</param>
		/// <param name="label">Etiqueta</param>
		/// <returns>Mínima etiqueta equivalente a <c>label</c></returns>
		private static int MinLabel(Dictionary<int, List<int>>  equiv, int label)
		{
			int min=int.MaxValue;
			
			if(equiv.ContainsKey(label))
			{
				List<int> l = equiv[label];
				foreach(int etiq in l)
				{
					if (etiq < min)
					{
						min = etiq;
					}
				}
			}
			else
				min=label;
			
			return min;
		}
	}
}
