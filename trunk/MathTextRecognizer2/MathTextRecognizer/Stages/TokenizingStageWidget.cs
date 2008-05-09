// TokenizingStageWidget.cs created with MonoDevelop
// User: luis at 16:19 26/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

using Gtk;
using Glade;

using MathTextCustomWidgets.Dialogs;
using MathTextCustomWidgets.Widgets.ImageArea;

using MathTextLibrary.Utils;
using MathTextLibrary.Bitmap;
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
		private Notebook tokenizingButtonsNB = null;
		
		[WidgetAttribute]
		private Alignment tokenizingNextButtonsAlign = null;
		
		[WidgetAttribute]
		private Button processBtn = null;
		
		[WidgetAttribute]
		private Label processBtnLbl = null;
		
		[WidgetAttribute]
		private Button nextStageBtn = null;
		
		[WidgetAttribute]
		private Notebook tokenizingStepsNB = null;
		
		[WidgetAttribute]
		private Frame currentImageFrm = null;
		
#endregion Glade widgets
		
#region Fields
		
		private NodeStore sequencesModel;
		private ListStore symbolsModel;
		
		private TokenizingController controller;
		
		private NodeView sequencesNV = null;
		
		private TreePath processedPath = null;
		
		// Indicates if the sequencing process has been completed.
		private bool sequencingFinished = false;
		
		
		private Token lastToken;
		
		private ImageArea baselineImageArea;
	
		
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
				new MessageLogSentHandler(OnControllerMessageLogSent);
			
			controller.ProcessFinished += 
				new ProcessFinishedHandler(OnControllerProcessFinished);
			
			
			InitializeWidgets();
			
			this.ShowAll();
		}
		
		/// <summary>
		/// <c>TokenizingStageWidget</c>'s static fields initializer.
		/// </summary>
		static TokenizingStageWidget()
		{
			widgetLabel = "Análisis léxico";
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
			
			tokenizingButtonsNB.Page = 0;
			
			processBtnLbl.Text = "Secuenciar";
			
			sequencingFinished = false;
			
			processBtn.Sensitive = true;
			nextStageBtn.Sensitive = false;
			
			tokenizingNextButtonsAlign.Sensitive = true;
			
			baselineImageArea.Image = null;
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
				Gdk.Pixbuf thumbnail = 
					ImageUtils.MakeThumbnail(token.Image.CreatePixbuf(),
					                         48);
				symbolsModel.AppendValues(thumbnail,
				                          token.Text,
				                          token);
			}
			
			// We don't want vertical scrolling.
			symbolsIV.Columns = tokens.Count;
			
			// We stablish the controller initial tokens.
			controller.SetInitialData(tokens, sequencesNV);
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
			
			sequencesNV.ShowExpanders = true;
			sequencesNV.RulesHint = true;
			sequencesNVPlaceholder.Add(sequencesNV);
			
			sequencesNV.AppendColumn("Secuencia", 
			                         new CellRendererText(), 
			                         "text",0);
			
			sequencesNV.AppendColumn("Símbolos", 
			                         new CellRendererText(), 
			                         "text",1);
			
			sequencesNV.AppendColumn("Token",
			                         new CellRendererText(),
			                         "text",2);
			
			foreach (TreeViewColumn column in sequencesNV.Columns) 
			{
				column.Sizing = TreeViewColumnSizing.Autosize;								
			}
						
			symbolsModel = new ListStore(typeof(Gdk.Pixbuf),
			                             typeof(string),
			                             typeof(Token));
			
			symbolsIV.Model = symbolsModel;
			
			symbolsIV.TextColumn = 1;
			symbolsIV.PixbufColumn =0;
			
			baselineImageArea = new ImageArea();
			baselineImageArea.ImageMode = ImageAreaMode.Zoom;
			currentImageFrm.Add(baselineImageArea);
			
			tokenizingStageWidget.ShowAll();
		}
		
		/// <summary>
		/// Tells the controller to process a new step.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnBtnNextStepClicked(object sender, EventArgs args)
		{
			LogAreaExpanded = true;	
			tokenizingNextButtonsAlign.Sensitive=false;
			NextStep(ControllerStepMode.StepByStep);
		}
		
		/// <summary>
		/// Tells the controller to process a new node.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnBtnNextNodeClicked(object sender, EventArgs args)
		{
			tokenizingNextButtonsAlign.Sensitive=false;
			LogAreaExpanded = false;				
			NextStep(ControllerStepMode.NodeByNode);
			
		}
		
		/// <summary>
		/// Tells the controller to process until the finish of the current
		/// process.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnBtnTilEndClicked(object sender, EventArgs args)
		{			
			LogAreaExpanded=false;
			tokenizingNextButtonsAlign.Sensitive=false;
			NextStep(ControllerStepMode.UntilEnd);
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
			Application.Invoke(sender, args, OnControllerSequenceAddedInThread);
		}
		
		private void OnControllerSequenceAddedInThread(object sender, 
		                                               EventArgs args)
		{
			SequenceAddedArgs a = args as SequenceAddedArgs;
			
			int number = 1;
			foreach (SequenceNode node in sequencesModel) 
			{
				number++;
			}
			
			a.Sequence.NodeName = String.Format("{0}", 
			                                    number);
			sequencesModel.AddNode(a.Sequence);
			//sequencesNV.ColumnsAutosize();
			sequencesNV.NodeSelection.SelectNode(a.Sequence);
			sequencesNV.ScrollToCell(sequencesNV.Selection.GetSelectedRows()[0],
			                         sequencesNV.Columns[0],
			                         true,
			                         1,0);
			
			sequencesNV.ColumnsAutosize();
		}
		
		/// <summary>
		/// Handles the start of processing of a new node.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnControllerNodeBeingProcessed(object sender, 
		                                            EventArgs args)
		{
			Application.Invoke(OnControllerNodeBeingProcessedInThread);
		}
		
		private void OnControllerNodeBeingProcessedInThread(object sender,
		                                                  EventArgs args)
		{
			
			if(!sequencingFinished)
			{
				// We are sequencing.
				
				processedPath.Next();
				
				// Selects the new first.			
				symbolsIV.SelectPath(processedPath);
				symbolsIV.ScrollToPath(processedPath, 0,0);
				
				// We get the token.
				TreeIter selectedIter;
				if(symbolsModel.GetIter(out selectedIter, processedPath))
				{
					Token processedToken = 
					symbolsModel.GetValue(selectedIter, 2) as Token;
				
					FloatBitmap sequenceImage;
					if(this.lastToken != null)
					{
						TokenSequence seq = new TokenSequence();
						seq.Append(lastToken);
						seq.Append(processedToken);
						Token joinedToken =Token.Join(seq, "");
						sequenceImage = joinedToken.Image;
					}
					else
					{
						sequenceImage = processedToken.Image;					
					}
					
					baselineImageArea.Image = sequenceImage.CreatePixbuf();
					
					lastToken = processedToken;
				}
				
				
			}
			
			tokenizingNextButtonsAlign.Sensitive = true;
			
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
			if(!sequencingFinished)
			{
				sequencesModel.Clear();
		
				TreeIter first;
				symbolsModel.GetIterFirst(out first);
			
				lastToken = symbolsModel.GetValue(first, 2) as Token;
				
				processedPath = symbolsModel.GetPath(first);
				
					
				symbolsIV.SelectPath(processedPath);
				symbolsIV.ScrollToPath(processedPath, 0, 0);
					
				tokenizingButtonsNB.Page = 1;
				
				
				controller.SetLexicalRules(MainWindow.LexicalRulesManager.LexicalRules);
			}
			else
			{
				
			}
			
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
			// We have finished.
			nextStageBtn.Sensitive = sequencingFinished; 
			processBtn.Sensitive = !sequencingFinished;
			
			
			
			// The state has changed.
			if(!sequencingFinished)
				sequencingFinished = true;
			
			
			
			
			if(sequencingFinished)
				processBtnLbl.Text = "Extraer tokens";
			
			// We change to the first page
			tokenizingButtonsNB.Page = 0;		
			
		}
		
		/// <summary>
		/// Checks if the tokeninzing result is correct, and if so, advances 
		/// to the next 
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnNextStageBtnClicked(object sender, EventArgs arg)
		{
			List<string> errors = new List<string>();
			foreach (SequenceNode rootNode in sequencesModel) 
			{
				errors.AddRange(CheckNodeErrors(rootNode));
			}
			
			if(errors.Count > 0)
			{
				// There were errors.
				string errorss = String.Join("\n", errors.ToArray());
				
				OkDialog.Show(this.MainWindow.Window,
				              MessageType.Info,
				              "Para continuar a la siguente fase de procesado,"
				              + "debes solucionar los siguentes problemas:\n\n{0}",
				              errorss);
			}
		}
		
		/// <summary>
		/// Checks a node for errors.
		/// </summary>
		/// <param name="node">
		/// The node to be checked.
		/// </param>
		/// <returns>
		/// A list with the errors causes.
		/// </returns>
		private List<string> CheckNodeErrors(SequenceNode node)
		{
			List<string> res = new List<string>();
			if(node.ChildCount > 0)
			{
				// If the node has children, we check them.
				for(int i=0; i<node.ChildCount; i++)
				{
					res.AddRange(CheckNodeErrors(node[i] as SequenceNode));
				}
			}
			else
			{
				if(node.FoundToken == null)
				{
					res.Add(String.Format("· La secuencia {0} no encajó con ninguna regla léxica definida.",
					                      node.NodeName));
				}
			}
			
			return res;
		}
		
		protected override void NextStep (ControllerStepMode mode)
		{
			controller.Next(mode);
		}

		
#endregion Non-public methods
	}
}
