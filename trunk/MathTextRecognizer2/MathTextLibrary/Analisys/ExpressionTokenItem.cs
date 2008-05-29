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
		public override string Label 
		{
			get 
			{ 
				return this.ToString(); 
			}
		}

		/// <value>
		/// Contains the type of the node's matcher element.
		/// </value>
		public override string Type 
		{
			get 
			{ 
				return "Item"; 
			}
		}

		
#endregion Properties
		
#region Non-public methods
		
		/// <summary>
		/// Tries to match the expetect token with one from the given sequence.
		/// </summary>
		/// <param name="sequence">
		/// A <see cref="TokenSequence"/> containing the items not yet matched.
		/// </param>
		/// <param name="output">
		/// The output in a <see cref="System.String"/>.
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/> that tells if the matching process
		/// was successful.
		/// </returns>
		protected override bool MatchSequence (ref TokenSequence sequence, 
		                                       out string output)
		{
			
			output ="";
			int idx = 0;
			
			// We tell the controller we are trying to match this token.			
			TokenMatchingInvoker(this.TokenType);
			
			if(forceTokenSearch)
			{
				idx = sequence.SearchToken(this.tokenType);
				Console.WriteLine("Forzada búsqueda de {0}, posición {1}",
				                  this.tokenType,
				                  idx);
			}		
			
			
			// By default, we say we had a success
			bool res = true;
			Token matched = null;
			if(idx==-1 || this.tokenType != sequence[idx].Type)
			{
				
				Console.WriteLine("Not matched: {0}", this.tokenType);
				res = !IsCompulsory;
			}
			else
			{
				
				matched = sequence.RemoveAt(idx);				
				if(this.relatedItems.Count ==0)
				{
					output= matched.Text;
				}
				else
				{
					res = MatchRelatedItems(matched, sequence, out output);
				}
			}
			
			
						
			// We tell the controller we finished matching the token.
			TokenMatchingFinishedInvoker(matched);
			
			return res;
			
		}

		

		protected override string SpecificToString ()
		{
			
			if(relatedItems.Count == 0)
			{
				string res="";
				
				res+=this.tokenType;
				return res;
			}
			else
			{
				
				List<string> itemTexts = new List<string>();
				itemTexts.Add(this.tokenType);
				foreach (ExpressionItem item in relatedItems)
				{
					itemTexts.Add(item.ToString());
				}
				
				return "{"+String.Join(" ", itemTexts.ToArray()) +"}";
			}
			
			
		}
		
		/// <summary>
		/// Tries to match the token's related items.
		/// </summary>
		/// <param name="matched">
		/// The <see cref="Token"/> matched token.  
		/// </param>
		/// <param name="sequence">
		/// The remaining tokens.
		/// </param>
		/// <param name="output">
		/// A <see cref="System.String"/> containing the output.
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating if the matching was 
		/// successfull.
		/// </returns>
		private bool MatchRelatedItems(Token matched, 
		                               TokenSequence sequence, 
		                               out string output)
		{
			output = "";
			
			// We return true unless we found a matching error in one of the
			// related items.
			bool res = true;
			
			// We have to create a list of outputs, so we can apply the 
			// output format string later.
			List<string> outputs = new List<string>();
			
			// We add the matched token own text as the first element
			outputs.Add(matched.Text); 
			
			foreach(ExpressionItem relatedItem in this.relatedItems)
			{
				string relatedItemOutput;
				TokenSequence relatedRemnant = 
					GetRelatedItems(matched,sequence,relatedItem.Position);

				Console.WriteLine("Reconociendo items ({0}) con el elemento «{1}»",
				                  relatedRemnant,
				                  relatedItem.ToString());
				
				if(relatedItem.Match(ref relatedRemnant, 
				                     out relatedItemOutput))
				{
					outputs.Add(relatedItemOutput);
				}
				else
				{
					res = false;
					break;
				}
			}				
			
			if(res)
			{
				output = String.Format(this.formatString, outputs.ToArray());
			}
			
			return res;
		}
		
		/// <summary>
		/// Launches the <see cref="TokenMatching"/> event.
		/// </summary>
		/// <param name="matchableType">
		/// The "first" <see cref="Token"/>
		/// being considered by the <see cref="TokenItem"/> for matching.
		/// </param>
		protected void TokenMatchingInvoker(String matchableType)
		{
			if(TokenMatching != null)
			{
				TokenMatching(this, new TokenMatchingArgs(matchableType));
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
		
		/// <summary>
		/// Retrives the related items for a token from the remaining items list.
		/// </summary>
		/// <param name="matched">
		/// The <see cref="Token"/> the items we are looking for are related to.
		/// </param>
		/// <param name="remainingItems">
		/// A <see cref="TokenSequence"/> containing the yet to be matched items.
		/// </param>
		/// <param name="position">
		/// A <see cref="ExpressionItemPosition"/> the position of the related item.
		/// </param>
		/// <returns>
		/// A <see cref="TokenSequence"/> containing the items related to the
		/// matched item found in the given position.
		/// </returns>
		protected TokenSequence GetRelatedItems(Token matched,
		                                        TokenSequence remainingItems, 
		                                        ExpressionItemPosition position)
		{
			TokenSequence sequence = new TokenSequence();
			
			string remainingItemsString = remainingItems.ToString();
			
			int i= 0;
			while (i < remainingItems.Count)
			{
				Token checkedItem = remainingItems[i];
				
				if(CheckInRelatedItemSequence(matched, checkedItem, position))
				{
					sequence.Append(checkedItem);
					remainingItems.RemoveAt(i);
				}
				else if(!SpecialPosition(matched, checkedItem))
				{
					Console.WriteLine("Encontrado {0}, cancelando la creación de la secuencia de items {1} de {2}",
					                  checkedItem.Type,
					                  position,
					                  matched.Type);
					break;
				}
				else
				{
					i++;
				}
			}
			
			Console.WriteLine("Extraida la secuencia ({0}) en posicion {1} de entre los elementos de ({2})",
			                  sequence,
			                  position,
			                  remainingItemsString);
			
			return sequence;
		}
		
		/// <summary>
		/// Checks if a token is in a given position related to the matched token.
		/// </summary>
		/// <param name="matched">
		/// A <see cref="Token"/> which the checked token will be checked to.
		/// </param>
		/// <param name="checkedItem">
		/// A <see cref="Token"/> that we want to know if it is in the related 
		/// position especified respect to the matched token.
		/// </param>
		/// <param name="position">
		/// A <see cref="ExpressionItemPosition"/> specifiying the position the 
		/// checked item should be.
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating if the checked item is in the
		/// given position related to the matched token.
		/// </returns>
		protected bool CheckInRelatedItemSequence(Token matched, 
		                                          Token checkedItem, 
		                                          ExpressionItemPosition position)
		{
			
			bool res =  false;
			
			int matchedBottom = matched.Y + matched.Height;
			int checkedBottom = checkedItem.Y + checkedItem.Height;
			int matchedRight = matched.X + matched.Width;
			int checkedRight = checkedItem.X + checkedItem.Width;
			switch(position)
			{
				case ExpressionItemPosition.Above:
					res = checkedBottom< matched.Y;
					break;
				case ExpressionItemPosition.Below:
					res = checkedItem.Y > matchedBottom;
					break;
				case ExpressionItemPosition.Inside:
					res = ((checkedItem.X >= matched.TopmostX )
						&& (checkedRight < matchedRight)
						&& (checkedItem.Y> matched.Y)
						&& (checkedBottom < matchedBottom));
							
					break;
				case ExpressionItemPosition.RootIndex:
					int line06 = matched.Y +(int)( matched.Height*0.66f);
					res = ((checkedItem.X < matched.TopmostX)
					       && (checkedBottom < line06));
					break;
				case ExpressionItemPosition.SubIndex:
					break;					
				case ExpressionItemPosition.SuperIndex:
					break;
			}
			
			
			
			return res;
			
		}
		
		/// <summary>
		/// Checks if a token is placed in a special position regarding other token.
		/// </summary>
		/// <param name="referenceToken">
		/// A <see cref="Token"/>
		/// </param>
		/// <param name="checkedToken">
		/// A <see cref="Token"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		protected bool SpecialPosition(Token referenceToken, Token checkedToken)
		{
			if(checkedToken.X < referenceToken.X)
				return true;
			
			
			int line066 = referenceToken.Y + (int)(referenceToken.Height * 0.66f);
			if(checkedToken.Y + checkedToken.Height < line066)
				return true;
			
			int line033 = referenceToken.Y + (int)(referenceToken.Height * 0.33f);
			if(checkedToken.Y > line033)
				return true;
			
			return false;
				
		}
#endregion Non-public methods
	}
}
