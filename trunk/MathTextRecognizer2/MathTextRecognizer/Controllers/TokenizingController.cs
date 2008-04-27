// TokenizingController.cs created with MonoDevelop
// User: luis at 17:43 27/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;


using MathTextLibrary.Tokenizing;
using MathTextLibrary.Controllers;

namespace MathTextRecognizer.Controllers
{
	
	/// <summary>
	/// This class' instances serve as the controllores of the sintactical
	/// analisys.
	/// </summary>
	public class TokenizingController : BaseController
	{
		private List<Token> tokens;
		
		/// <summary>
		/// <c>TokenizingController</c>'s constructor.
		/// </summary>
		public TokenizingController() : base()
		{
		}
		
#region Public methods		
		
		/// <summary>
		/// Sets the intial tokens to be processed.
		/// </summary>
		/// <param name="initialTokens">
		/// The inital tokens.
		/// </param>
		public void SetInitialTokens(List<Token> initialTokens)
		{
			tokens = initialTokens;
		}
#endregion Public methods	
		
#region Non-public methods
		
		/// <summary>
		/// Groups the tokens in others containing more symbols.
		/// </summary>
		protected override void Process ()
		{
			
		}

		
#endregion Non-public methods
	}
}
