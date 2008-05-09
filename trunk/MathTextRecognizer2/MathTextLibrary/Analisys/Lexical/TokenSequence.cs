// TokenSequence.cs created with MonoDevelop
// User: luis at 21:58 03/05/2008

using System;
using System.Collections;
using System.Collections.Generic;

namespace MathTextLibrary.Analisys.Lexical
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

		
#endregion Public methods
		
	}
}
