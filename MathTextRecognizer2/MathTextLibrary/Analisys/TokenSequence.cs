// TokenSequence.cs created with MonoDevelop
// User: luis at 21:58 03/05/2008

using System;
using System.Collections;
using System.Collections.Generic;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This class implements a sequence made of <c>Token</c> instances.
	/// </summary>
	public class TokenSequence : IEnumerable<Token>
	{
		private List<Token> sequence;
		
		public event EventHandler Changed;
		public event EventHandler ItemAdded;
		
		/// <summary>
		/// <c>TokenSequence</c>'s constructor.
		/// </summary>
		public TokenSequence()
		{
			sequence = new List<Token>();
		}
		
		public TokenSequence(List<Token> tokens)
		{
			sequence = tokens;
		}
		
		public TokenSequence(TokenSequence source) : this()
		{
			foreach (Token t in source) 
			{
				sequence.Add(t);
			}
		}
		
#region Properties
		
		/// <value>
		/// Contains the element count of the sequence.
		/// </value>
		public int Count
		{
			get
			{
				return sequence.Count;
			}
		}
		
		/// <value>
		/// Contains the Token in the position i of the sequence.
		/// </value>
		public Token this[int i]
		{
			get
			{
				return  sequence[i];
			}
		}
		
		/// <value>
		/// Contains the last token of the sequence.
		/// </value>
		public Token Last
		{
			get
			{
				if(this.Count>0)
					return this[Count-1];
				else
					return null;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Appends a token to the sequence.
		/// </summary>
		/// <param name="item">
		/// The <c>Token</c> instance being added.
		/// </param>
		public void Append(Token item)
		{
			sequence.Add(item);
			ItemAddedInvoker();
			ChangedInvoker();			
		}
		
		/// <summary>
		/// Adds a token at the sequence's start.
		/// </summary>
		/// <param name="item">
		/// The token being added.
		/// </param>
		public void Prepend(Token item)
		{
			sequence.Insert(0, item);
			ItemAddedInvoker();
			ChangedInvoker();
			
		}
		
		/// <summary>
		/// Removes the token in a given position, and returns it.
		/// </summary>
		/// <param name="position">
		/// The index of the removed token.
		/// </param>
		/// <returns>
		/// The removed token instance.
		/// </returns>
		public Token RemoveAt(int position)
		{
			Token removed = sequence[position];
			sequence.RemoveAt(position);
			ChangedInvoker();
			return removed;
		}
		
		/// <summary>
		/// Returns an enumerator for this token sequence.
		/// </summary>
		/// <returns>
		/// A <see cref="IEnumerator`1"/>
		/// </returns>
		public IEnumerator<Token> GetEnumerator ()
		{
			return sequence.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{			
			return sequence.GetEnumerator();
		}
		
		/// <summary>
		/// Shortcut for launching the Changed event.
		/// </summary>
		protected void ChangedInvoker()
		{			
			if(this.Changed !=null)
			{
				Changed(this, EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Shortcut for launching the ItemAdded event.
		/// </summary>
		protected void ItemAddedInvoker()
		{
			if(this.ItemAdded != null)
			{
				ItemAdded(this, EventArgs.Empty);
			}
				
		}
		
		/// <summary>
		/// Creates a string representation of the sequence.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public override string ToString ()
		{
			List<string> res = new List<string>();
			
			foreach (Token t in sequence) 
			{
				res.Add(String.Format("«{0}»",t.Text));
			}
			
			return String.Join(", ", res.ToArray());			
		}
		
		
		/// <summary>
		/// Searchs for a token in the sequence, and retrieves the tallest 
		/// (and thus more important) instance.
		/// </summary>
		/// <param name="type">
		/// The searched token's type name.
		/// </param>
		/// <returns>
		/// The position of the tallest found token.
		/// </returns>
		public int SearchToken(string type)
		{
			int idx = -1;
			int maxHeight = -1;
			Token testedToken;
			for(int i=0; i< sequence.Count; i++)
			{
				testedToken = sequence[i];
				if(testedToken.Height > maxHeight)
				{
					maxHeight = testedToken.Height;
					
					if(testedToken.Type == type 
					   && !ShadedByPreviousToken(testedToken))
					{
						idx = i;
					}
					
				}
			}
			
			if(idx >=0 && sequence[idx].Height < maxHeight)
			{
				// The symbol found wasnt the heightst, so it nos the main one.
				idx = -1;
			}
			
			return idx;
		}
		
		/// <summary>
		/// Tests if there isn't a path from every pixel of the token's left
		/// side to the sequence beginning.
		/// </summary>
		/// <param name="tested">
		/// A <see cref="Token"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private bool ShadedByPreviousToken(Token tested)
		{
			for(int i =0 ; i<this.Count && sequence[i] != tested; i++)
			{
				Token shading =  sequence[i];
				if(shading.Y > tested.Y 
				   || shading.Y+  shading.Height < tested.Y + tested.Height )
				{
					return true;
				}
			}
			
			return false;
		}

		
#endregion Public methods
		
	}
}
