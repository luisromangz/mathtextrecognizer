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
			
			buttonsNB.Page = 0;
			
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
				Gdk.Pixbuf thumbnail = 
					ImageUtils.MakeThumbnail(token.Image.CreatePixbuf(),
					                         48);
				symbolsModel.AppendValues(thumbnail,
				                          token.Text);
			}
			
			// We don't want vertical scrolling.
			symbolsIV.Columns = tokens.Count;
			
			// We stablish the controller initial tokens.
			controller.SetInitialTokens(tokens);
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
			                             typeof(string));
			
			symbolsIV.Model = symbolsModel;
			
			symbolsIV.TextColumn = 1;
			symbolsIV.PixbufColumn =0;
			
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
				              +"debes solucionar los siguentes problemas:\n\n{0}",
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
		
#endregion Non-public methods
	}
}
