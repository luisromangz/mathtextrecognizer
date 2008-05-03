// Token.cs created with MonoDevelop
// User: luis at 13:01 27/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Analisys.Lexical
{
	
	/// <summary>
	/// This class represents a product of lexical analisys.
	/// </summary>
	public class Token : IComparable<Token>
	{
		private string text;		
		private string type;
		
		private int x;
		private int y;
	
		private FloatBitmap image;
		
		private const float epsilon = 0.05f;
		
		/// <summary>
		/// <c>Token</c>'s constructor.
		/// </summary>
		/// <param name="text">
		/// The new token's text.
		/// </param>
		/// <param name="x">
		/// The new token's upper left corner x coordinate.
		/// </param>
		/// <param name="y">
		/// The new token's upper left corner y coordinate.
		/// </param>
		/// <param name="image">
		/// The image which represents the new token.
		/// </param>
		public Token(string text, int x, int y, FloatBitmap image)
		{			
			this.text = text;
			this.x = x;
			this.y = y;
			
			this.image = image;
		}
		
#region Properties
		
		/// <value>
		/// Contains the token's text.
		/// </value>
		public string Text
		{
			get 
			{
				return text;
			}
			set 
			{
				text = value;
			}
		}
		
		/// <value>
		/// Contains the token's type.
		/// </value>
		public string Type 
		{
			get 
			{
				return type;
			}
			set 
			{
				type = value;
			}
		}

		/// <value>
		/// Contains the image the token is symbolizing.
		/// </value>
		public FloatBitmap Image 
		{
			get {
				return image;
			}
		}

		/// <value>
		/// Contains the X coordinate of the imagen the token is symbolizing.
		/// </value>
		public int X 
		{
			get 
			{
				return x;
			}
		}

		/// <value>
		/// Contains the Y coordinate of the image the token is symbolizing.
		/// </value>
		public int Y
		{
			get 
			{
				return y;
			}
		}
		
		/// <value>
		/// Contains the height of the token's image.
		/// </value>
		public int Height
		{
			get
			{
				return image.Height;
			}
		}
		
		/// <value>
		/// Contains the width of the token's image.
		/// </value>
		public int Width
		{
			get
			{
				return image.Width;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Joins several tokens in one token.
		/// </summary>
		/// <param name="tokens">
		/// A <see cref="List`1"/>
		/// </param>
		/// <param name="tokenType">
		/// The new token's type.
		/// </param>
		/// <returns>
		/// A <see cref="Token"/>
		/// </returns>
		public static Token Join(List<Token> tokens, string tokenType)
		{
			int maxY = -1;
			int maxX = -1;
			int minY = int.MaxValue;
			int minX = int.MaxValue;
			string newText ="";
			
			// We have to calculate the new image's bounds, and join the 
			// texts.
			foreach(Token t in tokens)
			{
				if(t.y < minY)
				{
					minY = t.y;
				}
				
				if(t.y + t.Height > maxY)
				{
					maxY = t.y + t.Height;
				}
				
				if(t.x < minX)
				{
					minX = t.x;
				}
				
				if(t.x + t.Width> maxX)
				{
					maxX = t.x + t.Width;
				}
				
				newText+=t.Text;
			}
			
			int height = maxY - minY;
			int width = maxX - minX;
			
			FloatBitmap image = new FloatBitmap(width, height);
			
			// We copy the images in the result image.
			
			//TODO: Revisar este algoritmo.
			foreach(Token t in tokens)
			{
				for(int i = 0; i < t.image.Width; i++)
				{
					for(int j=0; j < t.image.Height; j++)
					{
						// We transform the coordinates so we place the 
						// pixel correctly on the new image.
						int x = i + t.X - minX;
						int y =  j + t.Y - minY;
						image[x, y] = t.image[i, j];
					}
				}
			}
			
			Token newToken = new Token(newText, minX, minY, image);
			newToken.type = tokenType;
			
			
			return newToken;
			
		}
		
		/// <summary>
		/// Compares with another <c>Token</c>  instance.
		/// </summary>
		/// <param name="other">
		/// A <see cref="Token"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		public int CompareTo (Token other)
		{
			return this.X - other.X;
		}
		
		/// <summary>
		/// Checks if the invoking token is next to another.
		/// </summary>
		/// <param name="previous">
		/// The token to check with.
		/// </param>
		/// <returns>
		/// <c>true</c> if the invoking token is next to the one used as
		/// parameter.
		/// </returns>
		public bool IsNextTo(Token previous)
		{
			
			
			int horizontalDistance = this.x - previous.x+ previous.Width;
			if(horizontalDistance > (Math.Min(this.Width, previous.Width))
			   || horizontalDistance <0)
			{
				return false;
			}
				
				
			
			int previousCenterY = previous.y+ previous.Height/2;
			int thisCenterY = this.y + this.Height /2;
			
			int mediumHeight = (previous.Height + this.Height)/2;
			
			int range = (int)(mediumHeight * epsilon);
			
			if(Math.Abs(previousCenterY-thisCenterY) > range)
			{
				return false;
			}
			
			
			
			
			return true;
			
		}

		
#endregion Public methods
	}
}
