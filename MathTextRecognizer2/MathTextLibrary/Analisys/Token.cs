// Token.cs created with MonoDevelop
// User: luis at 13:01 27/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

using MathTextLibrary.Bitmap;

namespace MathTextLibrary.Analisys
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
		
		private const string DESCENDERS = "yjgpq()";
		private const string ASCENDERS = "dtijñ()";
		private const string PUNCTUATION = ".,";	
		
		
		private static Token empty = new Token("");
		
		private int baseline;
		private int bodyline;
		
		private int percent033line;
		private int percent066line;
		private int percent075line;
		private int percent120line;
		
		public Token()
		{
			baseline = -1;
			bodyline = -1;
			percent033line = -1;
			percent066line = -1;
			percent120line = -1;
			percent075line = -1;
		}
			
		/// <summary>
		/// <see cref="Token"/>'s contructor accepting the token's type.
		/// </summary>
		/// <param name="tokenType">
		/// A <see cref="System.String"/>
		/// </param>
		public Token(string tokenType) 
			: this()
		{
			this.type = tokenType;
		}
		
		
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
			: this()
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
		public int Left 
		{
			get 
			{
				return x;
			}
		}

		/// <value>
		/// Contains the Y coordinate of the image the token is symbolizing.
		/// </value>
		public int Top
		{
			get 
			{
				return y;
			}
		}
		
		/// <value>
		/// Contains the token's bottom.
		/// </value>
		public int Bottom
		{
			get
			{
				return y + this.Height;
			}
		}
		
		/// <value>
		/// Contains the token's right pixel.
		/// </value>
		public int Right
		{
			get
			{
				return x + this.Width;
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
		
		/// <value>
		/// Contains the token's baseline.
		/// </value>
		public int Baseline
		{
			get
			{
				if(baseline== -1)
				{
					baseline = this.Height;
				
					bool isAscender = (ASCENDERS.Contains(this.text) 
					                   || char.IsUpper(this.Text[0])
					                   || char.IsNumber(this.text[0])
					                   || this.type == "INT");
					
					bool isDescender = (DESCENDERS.Contains(this.text)
					                    ||this.type == "INT");
					
					
					if(isDescender && !isAscender)
					{
						baseline = (int)(baseline*0.66f);
					}
					else if(isDescender && isAscender)
					{
						baseline = (int)(baseline*0.87f);
					}
					
					baseline = this.y + baseline;
					
				}
				
				return baseline;				
			}
		}
		
		/// <value>
		/// Contains the token's body line.
		/// </value>
		public int Bodyline
		{
			get
			{
				if(bodyline == -1)
				{
					bodyline = this.y;
				
					bool isAscender = (ASCENDERS.Contains(this.text) 
					                   || char.IsUpper(this.Text[0])
					                   || char.IsNumber(this.text[0])
					                   || this.type == "INT");
					
					bool isDescender = (DESCENDERS.Contains(this.text)
					                    ||this.type == "INT");
					
					if(isAscender
					   && !isDescender)
					{
						bodyline += (int)(this.Height*0.33f);
					}
					else if(isAscender
					        && isDescender)
					{
						bodyline += (int)(this.Height*0.13f);
					}
				}
				
				return bodyline;
			}
		}
		
		/// <value>
		/// This property contains the height of the <see cref="Token"/>'s
		/// body portion.
		/// </value>
		public int BodyHeight
		{
			
			get
			{
				return this.Baseline - this.Bodyline;
			}
		}
		
		public int Percent0_33Line
		{
			get
			{
				if(percent033line==-1)
				{
					percent033line = Bodyline +(int)(BodyHeight*0.33f);
				}
				
				return percent033line;
			}
		}
		
		
		public int Percent0_66Line {
			get {
				if(percent066line==-1)
				{
					percent066line = Bodyline +(int)(BodyHeight*0.66f);
				}
				
				return percent066line;
			}
		}
		
		public int Percent0_75Line {
			get {
				if(percent075line==-1)
				{
					percent075line = Bodyline +(int)(BodyHeight*0.75f);
				}
				
				return percent075line;
			}
		}

		public int Percent1_20Line {
			get {
				if(percent120line==-1)
				{
					percent120line = Bodyline +(int)(BodyHeight*1.20f);
				}
				
				return percent120line;
			}
		}


		/// <value>
		/// Contains an empty token.
		/// </value>
		public static Token Empty 
		{
			get 
			{
				return empty;
			}
		}
		
		/// <value>
		/// Contains the x position of the topmost left first black.
		/// </value>
		public int TopmostX
		{
			get
			{
				for(int j=0;j<image.Height; j++)
				{
					for(int i =0;i<image.Width; i++)
					{
						if(image[i,j]!=FloatBitmap.White)
						{
							return i + x;
						}
					}
				}
				
				return -1;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Joins several tokens in one token.
		/// </summary>
		/// <param name="tokens">
		/// A <see cref="TokenSequence"/>
		/// </param>
		/// <param name="tokenType">
		/// The new token's type.
		/// </param>
		/// <returns>
		/// A <see cref="Token"/>
		/// </returns>
		public static Token Join(TokenSequence tokens, string tokenType)
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
			foreach(Token t in tokens)
			{
				for(int i = 0; i < t.image.Width; i++)
				{
					for(int j=0; j < t.image.Height; j++)
					{
						// We transform the coordinates so we place the 
						// pixel correctly on the new image.
						int x = i + t.Left - minX;
						int y =  j + t.Top - minY;
						image[x, y] = t.image[i, j];
					}
				}
			}
			
			Token newToken = new Token(newText, minX, minY, image);
			newToken.type = tokenType;			
			
			// We are going to suppose that we are joining a sequence that
			// shares baseline and bodyline, because all other joined tokens
			// won't be used.
			newToken.baseline = tokens.Last.Baseline;
			newToken.bodyline = tokens.Last.Bodyline;
			
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
			return this.Left - other.Left;
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
		public bool CloseFollows(Token previous)
		{
			
			
			int horizontalDistance = this.x - (previous.x+ previous.Width);
			if(horizontalDistance > (this.Width + previous.Width)/4
			   || horizontalDistance <0)
			{
				return false;
			}
				
				
			int range = (int)(epsilon*(Math.Max(this.Height, previous.Height)));
			int baseDiff = Math.Abs(this.Baseline - previous.Baseline);
			
			return baseDiff <=range;
			
			
		}

	
		
#endregion Public methods
	}
}
