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

using MathTextRecognizer.Controllers;
using MathTextRecognizer.Controllers.Nodes; 

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
		
		// Indicates if the sequencing process has been completed.
		private int state = 0;
		
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
			
			controller.ProcessFinished += 
				new ProcessFinishedHandler(OnControllerProcessFinished);
			
			
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
			sequencesModel.Clear();
			
			buttonsNB.Page =0;
			
			processBtn.Label = "Secuenciar";
			
			state = 0;
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
			SequenceAddedArgs a = args as SequenceAddedArgs;
			
			a.Sequence.Widget = sequencesNV;
			sequencesModel.AddNode(a.Sequence);
			sequencesNV.ColumnsAutosize();
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
			Application.Invoke(OnControllerNodeBeingProcessedInThread);
		}
		
		private void OnControllerNodeBeingProcessedInThread(object sender,
		                                                  EventArgs args)
		{
			
			if(state==0)
			{
				// We are sequencing.
				
				// Selects the new first.			
				symbolsIV.SelectPath(processedPath);
				symbolsIV.ScrollToPath(processedPath, 0,0);
				
				processedPath.Next();
			}
			
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
			Application.Invoke(sender, a,OnMessageLogSentInThread);
		}
		
		private void OnMessageLogSentInThread(object sender, EventArgs a)
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
			sequencesModel.Clear();
		
			TreeIter first;
			symbolsModel.GetIterFirst(out first);
			
			processedPath = symbolsModel.GetPath(first);
				
			symbolsIV.SelectPath(processedPath);
			symbolsIV.ScrollToPath(processedPath, 0, 0);
				
			buttonsNB.Page = 1;
			
			
			controller.SetLexicalRules(MainWindow.LexicalRulesManager.LexicalRules);
			controller.Next(ControllerStepMode.StepByStep);		
		}
			
		/// <summary>
		/// Handles the process finished event of the controller.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnControllerProcessFinished(object sender, EventArgs a)
		{
			// The state has changed.
			state = state +1;
			
			// We have finished.
			nextStageBtn.Sensitive = state ==2; // Sensitive if we have finished.
			
			processBtn.Label = state==0?"Secuenciar":"Extraer tokens";
			// We change to the first page
			buttonsNB.Page = 0;		
			
		}
		
#endregion Non-public methods
	}
}
