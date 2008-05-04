// TokenizingStageWidget.cs created with MonoDevelop
// User: luis at 16:19 26/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

using Gtk;
using Glade;

using MathTextLibrary.Utils;
using MathTextLibrary.Analisys.Lexical;
using MathTextLibrary.Controllers;

using MathTextRecognizer.Stages.Nodes; 

namespace MathTextRecognizer.Stages
{
	
	/// <summary>
	/// This class implements a widget for showing the tokenizing of the 
	/// processed nodes.
	/// </summary>
	public class TokenizingStageWidget : RecognizingStageWidget
	{
		
#region Glade widgets
		
		[WidgetAttribute]
		private Alignment tokenizingStageWidget = null;
		
		[WidgetAttribute]
		private IconView symbolsIV = null;
		
		[WidgetAttribute]
		private TreeView sequencesTV = null;
		
		[WidgetAttribute]
		private Notebook buttonsNB = null;
		
		[WidgetAttribute]
		private Alignment alignNextButtons = null;
		
		[WidgetAttribute]
		private Button processBtn = null;
		
		[WidgetAttribute]
		private Button nextStageBtn = null;
		
#endregion Glade widgets
		
#region Fields
		
		private ListStore sequencesModel;
		private ListStore symbolsModel;
		
		private TokenizingController controller;
		
#endregion Fields
		
		/// <summary>
		/// <c>TokenizingStageWidget</c>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// A <see cref="Window"/>
		/// </param>
		public TokenizingStageWidget(MainRecognizerWindow parent) : base(parent)
		{
			XML gladeXml = new XML(null,
			                       "mathtextrecognizer.glade" ,
			                       "tokenizingStageWidget", 
			                       null);
			
			gladeXml.Autoconnect(this);
			
			controller = new TokenizingController();
			
			this.Add(tokenizingStageWidget);
			
			InitializeWidgets();
			
			this.ShowAll();
		}
		
#region Properties
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Sets the controls to their initial state.
		/// </summary>
		public override void ResetState ()
		{
			symbolsModel.Clear();
		}
		
		/// <summary>
		/// Sets the product of the segmentation stage as the start point
		/// for the tokenizing stage.
		/// </summary>
		/// <param name="segmentationResult">
		/// A list made with the leaf of the segmentation tree.
		/// </param>
		public void SetStartSymbols(List<SegmentedNode> segmentationResult)
		{
			// Transforms the segmented nodes to tokens.
			List<Token> tokens = new List<Token>();
			foreach(SegmentedNode symbolNode in segmentationResult)
			{
				tokens.Add(new Token(symbolNode.Label, 
				                     symbolNode.MathTextBitmap.Position.X,
				                     symbolNode.MathTextBitmap.Position.Y,
				                     symbolNode.MathTextBitmap.FloatImage));
			}				
			
			tokens.Sort();
			
			// We add the symbols to the gui.
			foreach (Token token in tokens) 
			{
				symbolsModel.AppendValues(ImageUtils.MakeThumbnail(token.Image.CreatePixbuf(),
				                                                   48),
				                          token.Text);
			}
			
			symbolsIV.Columns = tokens.Count;
			
			// We stablish the controller initial tokens.
			controller.Tokens = tokens;
		}

		
#endregion Public methods
		
#region Non-public methods
		
		/// <summary>
		/// Initialize the widget's children widgets.
		/// </summary>
		private void InitializeWidgets()
		{
			sequencesModel = new ListStore(typeof(string),
			                               typeof(string),
			                               typeof(TokenSequence));
			
			sequencesTV.Model = sequencesModel;
			
			sequencesTV.AppendColumn("Secuencia", 
			                         new CellRendererText(), 
			                         "text",0);
			
			sequencesTV.AppendColumn("Tokens",
			                         new CellRendererText(),
			                         "text",1);
			
			symbolsModel = new ListStore(typeof(Gdk.Pixbuf),
			                             typeof(string));
			
			symbolsIV.Model = symbolsModel;
			
			symbolsIV.TextColumn = 1;
			symbolsIV.PixbufColumn =0;
			
		}
	
	

		
#endregion Non-public methods
	}
}
