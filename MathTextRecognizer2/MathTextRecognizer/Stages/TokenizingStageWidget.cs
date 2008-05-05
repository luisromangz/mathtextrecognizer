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
		private ScrolledWindow sequencesNVPlaceholder = null;
		
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
		
		private NodeStore sequencesModel;
		private ListStore symbolsModel;
		
		private TokenizingController controller;
		
		private NodeView sequencesNV = null;
		
		private TreePath processedPath = null;
		
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
			controller.SequenceAdded+= 
				new SequenceAddedHandler(OnControllerSequenceAdded);
			
			controller.NodeBeingProcessed+= 
				new EventHandler(OnControllerNodeBeingProcessed);
			
			controller.MessageLogSent+=
				new MessageLogSentHandler(OnMessageLogSent);
			
			
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
			this.Add(tokenizingStageWidget);
			
			sequencesModel = new NodeStore(typeof(SequenceNode));
			
			sequencesNV = new NodeView(sequencesModel);
			sequencesNV.RulesHint = true;
			sequencesNVPlaceholder.Add(sequencesNV);
			
			sequencesNV.AppendColumn("Secuencia", 
			                         new CellRendererText(), 
			                         "text",0);
			
			sequencesNV.AppendColumn("Tokens",
			                         new CellRendererText(),
			                         "text",1);
			
			symbolsModel = new ListStore(typeof(Gdk.Pixbuf),
			                             typeof(string));
			
			symbolsIV.Model = symbolsModel;
			
			symbolsIV.TextColumn = 1;
			symbolsIV.PixbufColumn =0;
			
		}
	
		/// <summary>
		/// Adds a node to repressent the new sequence.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="SequenceAddedArgs"/>
		/// </param>
		private void OnControllerSequenceAdded(object sender, 
		                                       SequenceAddedArgs args)
		{
			Application.Invoke(sender, args, OnControllerSequenceAddedThread);
		}
		
		private void OnControllerSequenceAddedThread(object sender,
		                                             EventArgs args)
		{
			SequenceAddedArgs a = args as SequenceAddedArgs;
			
			SequenceNode node = new SequenceNode(a.Sequence);			
			sequencesModel.AddNode(node);
		}
		
		/// <summary>
		/// Handles the start of processing of a new node.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="ntArgs">
		/// A <see cref="Eve"/>
		/// </param>
		private void OnControllerNodeBeingProcessed(object sender, 
		                                            EventArgs args)
		{
			Application.Invoke(OnControllerNodeBeingProcessedThread);
		}
		
		private void OnControllerNodeBeingProcessedThread(object sender,
		                                                  EventArgs args)
		{
			// Remove the first item, and the selects the new first.
			if(processedPath ==null)
			{
				TreeIter first;
				symbolsModel.GetIterFirst(out first);
				
				processedPath = symbolsModel.GetPath(first);
			}
			
			symbolsIV.SelectPath(processedPath);
			symbolsIV.ScrollToPath(processedPath, 0,0);
			
			processedPath.Next();
		}
		
		/// <summary>
		/// Writes a log message.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="MessageLogSentArgs">
		/// A <see cref="MessageLogSentArgs"/>
		/// </param>		
		private void OnMessageLogSent(object sender,MessageLogSentArgs a)
		{
		    // Llamamos a través de invoke para que funcione bien.			
			Application.Invoke(sender, a,OnMessageLogSentThread);
		}
		
		private void OnMessageLogSentThread(object sender, EventArgs a)
		{		   
		    Log(((MessageLogSentArgs)a).Message);
		}
				
		/// <summary>
		/// Launches the sequencing process.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnProcessBtnClicked(object sender, EventArgs args)
		{
			controller.SetLexicalRules(MainWindow.LexicalRulesManager.LexicalRules);
			controller.Next(ControllerStepMode.StepByStep);
			
			buttonsNB.Page = 1;
			
			TreeIter first;
			symbolsModel.GetIterFirst(out first);
			
			symbolsIV.SelectPath(symbolsModel.GetPath(first));
		}
				

		
#endregion Non-public methods
	}
	
	/// <summary>
	/// This class implements a node for the sequence treeview.
	/// </summary>
	class SequenceNode : TreeNode
	{
		TokenSequence sequence;
		TokenSequence tokens;
		
		string sequenceLabel;
		string tokensLabel;
		
		public SequenceNode(TokenSequence sequence)
		{
			this.sequence = sequence;
			this.tokens = new TokenSequence();
			
			sequence.Changed += new EventHandler(OnSequenceItemAdded);
			tokens.Changed += new EventHandler(OnTokensChanged);
		}
		
#region Properties
		
		/// <value>
		/// Contains the node sequence column text.
		/// </value>
		[TreeNodeValue(Column =0)]
		public string SequenceText
		{
			get
			{
				return sequenceLabel;
			}
		}
	
		/// <value>
		/// Contains the node tokens column text.
		/// </value>
		[TreeNodeValue(Column =1)]
		public string TokensText
		{
			get
			{
				return tokensLabel;
			}
		}

		/// <value>
		/// Contains the tokens assigned to this node's token sequence.
		/// </value>
		public TokenSequence Tokens 
		{
			get 
			{
				return tokens;
			}
			set 
			{
				tokens = value;
				tokens.Changed += new EventHandler(OnTokensChanged);
			}
		}
		
#endregion Properties
		
#region Event handlers	
		/// <summary>
		/// Creates the label for the sequence column when the sequence 
		/// is modified.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnSequenceItemAdded(object sender, EventArgs args)
		{
			Application.Invoke(OnSequenceItemAddedThread);
		}
		
		private void OnSequenceItemAddedThread(object sender, EventArgs args)
		{
			List<string> res = new List<string>();
			
			foreach (Token t in sequence) 
			{
				res.Add(t.Text);
			}
			
			sequenceLabel =  String.Join(", ", res.ToArray());
			Console.WriteLine("Sequence {0}", sequenceLabel);
		}

		
		/// <summary>
		/// Creates the label for the sequence column when the sequence 
		/// is modified.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnTokensChanged(object sender, EventArgs args)
		{
			Application.Invoke(OnTokensChangedThread);
		}
		
		private void OnTokensChangedThread(object sender, EventArgs args)
		{
			List<string> res = new List<string>();
			
			foreach (Token t in tokens) 
			{
				res.Add(t.Type);
			}
			
			tokensLabel =  String.Join(", ", res.ToArray());
		
		}
		
#endregion Event handlers
	}
}
