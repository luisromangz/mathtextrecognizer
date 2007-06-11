using System;

namespace MathTextLibrary.TreeMaker.Steps
{
	/// <summary>
	/// Interfaz comun a los pasos de transformacion del arbol intermedio.
	/// </summary>
	interface ITreeMakerStep
	{
		/// <summary>
		/// Aplica el paso al nodo <c>tree</c>.
		/// </summary>
		/// <returns>Nuevo arbol resultante</returns>
		RecognizedTreeNode ApplyStep(RecognizedTreeNode tree);
	}
}
