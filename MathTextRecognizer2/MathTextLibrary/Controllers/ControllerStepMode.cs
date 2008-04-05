// ControllerStepMode.cs created with MonoDevelop
// User: luis at 16:06 05/04/2008

using System;

namespace MathTextLibrary.Controllers
{
	
	
	/// <summary>
	/// Enumeracion que indica el tipo de paso a paso con el que se
	/// van a realizar los procesos de cada controlador.
	/// </summary>
	public enum ControllerStepMode
	{
		/// <summary>
		/// Representa el modo de nodo a nodo de la imagen, deteniendose
		/// en cada parametro a procesar de la misma.
		/// </summary>
		StepByStep,		
		
		/// <summary>
		/// Representa el modo de paso a paso por nodo segmentado de la imagen.
		/// </summary>
		NodeByNode,
		
		/// <summary>
		/// Representa el modo de ejecucion sin detenerse en pasos intermedios.
		/// </summary>
		UntilEnd
	}
}
