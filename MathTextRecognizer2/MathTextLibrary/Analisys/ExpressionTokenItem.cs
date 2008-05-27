// ExpressionTokenItem.cs created with MonoDevelop
// User: luis at 12:57 13/05/2008

using System;
using System.Collections.Generic;

using MathTextLibrary.Controllers;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This class implements a expression item to be matched by 
	/// a given <see cref="Token"/> type.
	/// </summary>
	public class ExpressionTokenItem : ExpressionItem
	{
		private string tokenType;	
		
		private bool forceTokenSearch;
		
		private List<ExpressionItem> relatedItems;
		
		private string formatString;
		
		
		public static event TokenMatchingHandler TokenMatching;
		
		public static event TokenMatchingFinishedHandler TokenMatchingFinished;
		
		/// <summary>
		/// <see cref="ExpressionTokenItem"/>'s constructor.
		/// </summary>
		public ExpressionTokenItem() : base()
		{
			forceTokenSearch = false;
			
			relatedItems = new List<ExpressionItem>();		
		}
		
#region Properties
		
		/// <value>
		/// Contains the token type that matches this expression item.
		/// </value>
		public string TokenType 
		{
			get 
			{
				return tokenType;
			}
			set 
			{
				tokenType = value;
			}
		}

		/// <value>
		/// Contains a value indicating if the token must be searched instead
		/// just taking the first token in the sequence.
		/// </value>
		public bool ForceTokenSearch 
		{
			get 
			{
				return forceTokenSearch;
			}
			set
			{
				forceTokenSearch = value;
			}
		}
		
		
		/// <value>
		/// Contains the list of items placed in a special position 
		/// relative to this item.
		/// </value>
		public List<ExpressionItem> RelatedItems
		{
			get
			{
				return relatedItems;
			}
			set
			{
				relatedItems = value;
			}
		}

		/// <value>
		/// Contains the format string for the token in case it has 
		/// related items.
		/// </value>
		public string FormatString
		{
			get 
			{
				return formatString;
			}
			set
			{
				formatString = value;
			}
		}
		
		/// <value>
		/// Contains the label shown by the item.
		/// </value>
		public override string Label {
			get { return this.ToString(); }
		}

		public override string Type {
			get { return "Item"; }
		}

		
#endregion Properties
		
#region Non-public methods
		
		protected override bool MatchSequence (TokenSequence sequence, 
		                                       out string output)
		{
			
			output ="";
			int idx = 0;
			if(forceTokenSearch)
			{
				idx = sequence.SearchToken(new Token(this.tokenType));
			}		
			
			// We tell the controller we are trying to match this token.
			TokenMatchingInvoker(idx);
			
			// By default, we say we had a success
			bool res = true;
			Token matched = null;
			if(idx==-1 || this.tokenType != sequence[idx].Type)
			{
				
				res = !IsCompulsory;
			}
			else
			{
				output= sequence[idx].Text;
				matched = sequence.RemoveAt(idx);
			}
						
			// We tell the controller we finished matching the token.
			TokenMatchingFinishedInvoker(matched);
			
			return res;
			
		}

		

		protected override string ToStringAux ()
		{
			
			
			if(relatedItems.Count == 0)
			{
				return this.tokenType;
			}
			else
			{
				string res="{" + this.TokenType;
				
				foreach (ExpressionItem item in relatedItems)
				{
					switch(item.Position)
					{
						case ExpressionItemPosition.Above:
							res+= "↑";
							break;
						case ExpressionItemPosition.Below:
							res+="";
							break;
						case ExpressionItemPosition.Inside:
							res+="↶";
							break;
						case ExpressionItemPosition.RootIndex:
							res+="↖";
							break;						
						case ExpressionItemPosition.SubIndex:
							res+="↘";
							break;						
						case ExpressionItemPosition.SuperIndex:
							res+="↗";
							break;
							
					}
					
					res+= item.ToString();
				}
				
				res +="}";
				
				return res;
			}
			
		}
		
		/// <summary>
		/// Tries to match the tokens related items.
		/// </summary>
		/// <param name="index">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="sequence">
		/// A <see cref="TokenSequence"/>
		/// </param>
		private void MatchRelatedItems(int index, TokenSequence sequence)
		{
			
		}
		
		/// <summary>
		/// Launches the <see cref="TokenMatching"/> event.
		/// </summary>
		/// <param name="idx">
		/// The index in the sequence of the "first" <see cref="Token"/>
		/// being considered by the <see cref="TokenItem"/> for matching.
		/// </param>
		protected void TokenMatchingInvoker(int idx)
		{
			if(TokenMatching != null)
			{
				TokenMatching(this, new TokenMatchingArgs(idx));
			}
		}
		
		/// <summary>
		/// Launches the <see cref="TokenMatchingFinished"/> event.
		/// </summary>
		/// <param name="t">
		/// The token matched, or <c>null</c> if the matching was
		/// unsuccesful.
		/// </param>
		protected void TokenMatchingFinishedInvoker(Token t)
		{
			if(TokenMatchingFinished !=null)
			{
				TokenMatchingFinished(this, 
				                      new TokenMatchingFinishedArgs(t));
			}
		}

		
#endregion Non-public methods
	}
}
