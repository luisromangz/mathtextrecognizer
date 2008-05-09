// FormulaMatchingController.cs created with MonoDevelop
// User: luis at 12:57 09/05/2008

using System;
using System.Collections.Generic;

using MathTextLibrary.Analisys.Lexical;
using MathTextLibrary.Controllers;

namespace MathTextRecognizer.Controllers
{
	
	/// <summary>
	/// This class implements the controller for the formula construction 
	/// process.
	/// </summary>
	public class FormulaMatchingController : ControllerBase
	{
		/// <summary>
		/// <c>FormulaMatchingController</c>'s contructor.
		/// </summary>
		public FormulaMatchingController() : base()
		{
		}
		
#region Public methods
		
		/// <summary>
		/// Sets the tokens used as data for the process.
		/// </summary>
		/// <param name="startTokens">
		/// A <see cref="List`1"/> containing the <c>Token</c> instances generated
		/// by the previous stage.
		/// </param>
		public void SetStartTokens(List<Token> startTokens)
		{
			
		}
		
		/// <summary>
		/// Call point to start the processing.
		/// </summary>
		protected override void Process ()
		{
			throw new NotImplementedException ();
		}

		
#endregion Public methods
		
#region Private methods
		
#endregion Private methods
		
	}
}
