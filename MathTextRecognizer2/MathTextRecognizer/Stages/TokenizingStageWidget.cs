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
using MathTextLibrary.Analisys;
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
		private Alignment tokenizingStageWidgetBase = null;
		
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
		
		[WidgetAttribute]
		private Label sequencingStepResultLbl = null;
		
		[WidgetAttribute]
		private Label matchingResultLbl = null;
		
		[WidgetAttribute]
		private TreeView tokenizingRulesTV = null;
		
		[WidgetAttribute]
		private Frame sequenceImageFrm = null;
		
		[WidgetAttribute]
		private Menu sequenceNodeMenu = null;
		
		
#endregion Glade widgets
		

#region Fields
		
		private NodeStore sequencesModel;
		private ListStore symbolsModel;
		
		private TokenizingController controller;
		
		private NodeView sequencesNV = null;
		
		private TreePath processedPath = null;
		
		// Indicates if the sequencing process has been completed.
		private bool sequencingFinished = false;
		private bool processFinished = false;
		
		
		private Token lastToken;
		private Token currentToken;
		
		private ImageArea baselineImageArea;
		private ImageArea sequenceMatchingImageArea;
		
		
		private Gdk.Pixbuf sequenceNodeImage;
		
		private SequenceNode selectedNode;
		
#endregion Fields
		
		
#region Constructors
		
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
			                       "tokenizingStageWidgetBase", 
			                       null);
			
			gladeXml.Autoconnect(this);
			
			gladeXml = new XML(null, 
			                   "mathtextrecognizer.glade",
			                   "sequenceNodeMenu", 
			                   null);
			
			gladeXml.Autoconnect(this);
			
			controller = new TokenizingController();
			controller.SequenceAdded+= 
				new SequenceAddedHandler(OnControllerSequenceAdded);
			
			controller.TokenChecked+= 
				new TokenCheckedHandler(OnControllerTokenChecked);
			
			controller.NodeBeingProcessed+=
				new NodeBeingProcessedHandler(OnControllerNodeBeingProcessed);
			
			controller.StepDone += OnControllerStepDone;
			
			controller.MessageLogSent+=
				new MessageLogSentHandler(OnControllerMessageLogSent);
			
			controller.ProcessFinished += OnControllerProcessFinished;
			controller.SequenceBeingMatched +=
				new SequenceBeingMatchedHandler(OnControllerSequenceBeingMatched);
			
			controller.MatchingFailed += OnControllerMatchingFailed;
			
			
		
			
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
		
#endregion Constructors
		
#region Properties
		
		/// <value>
		/// Contains the tokens returned by the lexical analisys.
		/// </value>
		public List<Token> ResultTokens
		{
			get
			{
				return controller.Result;
			}
		}
		
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
			tokenizingStepsNB.Page = 0;
			
			processBtnLbl.Text = "Secuenciar";
			
			sequencingFinished = false;
			
			processBtn.Sensitive = true;
			nextStageBtn.Sensitive = false;
			
			tokenizingNextButtonsAlign.Sensitive = true;
			
			matchingResultLbl.Markup = "-";
			sequencingStepResultLbl.Markup="-";
			
			baselineImageArea.Image = null;
			sequenceMatchingImageArea.Image =null;
		}
		
		
		
		public override void Abort ()
		{
			controller.Abort();
		}

		public override void SetInitialData ()
		{
			this.SetStartSymbols(MainRecognizerWindow.OCRWidget.LeafNodes);	
		}

		
#endregion Public methods
		
#region Non-public methods
		
		/// <summary>
		/// Sets the product of the segmentation stage as the start point
		/// for the tokenizing stage.
		/// </summary>
		/// <param name="segmentationResult">
		/// A list made with the leaf of the segmentation tree.
		/// </param>
		private void SetStartSymbols(List<SegmentedNode> segmentationResult)
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
		
		
		
	
		
	
		
		/// <summary>
		/// Initialize the widget's children widgets.
		/// </summary>
		private void InitializeWidgets()
		{
			this.Add(tokenizingStageWidgetBase);
			
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
			
			sequencesNV.AppendColumn("Item",
			                         new CellRendererText(),
			                         "text",2);
			
			// We tell the treeview's columns to resize automatically.
			foreach (TreeViewColumn column in sequencesNV.Columns) 
			{
				column.Sizing = TreeViewColumnSizing.Autosize;								
			}
			
			// We handle the pressing of the mouse buttons, so we can show
			// the contextual menu.
		
			sequencesNV.Events = Gdk.EventMask.ButtonPressMask;
			sequencesNV.ButtonPressEvent+= 
				new ButtonPressEventHandler(OnSequencesNVButtonPress);
									
			symbolsModel = new ListStore(typeof(Gdk.Pixbuf),
			                             typeof(string),
			                             typeof(Token));
			
			symbolsIV.Model = symbolsModel;
			
			symbolsIV.TextColumn = 1;
			symbolsIV.PixbufColumn =0;
			
			baselineImageArea = new ImageArea();
			baselineImageArea.ImageMode = ImageAreaMode.Zoom;
			currentImageFrm.Add(baselineImageArea);
			
			sequenceMatchingImageArea = new ImageArea();
			sequenceMatchingImageArea.ImageMode = ImageAreaMode.Zoom;
			sequenceImageFrm.Add(sequenceMatchingImageArea);
			
			tokenizingRulesTV.AppendColumn("Item generado", 
			                               new CellRendererText(), 
			                               "text", 0);
			tokenizingRulesTV.AppendColumn("Expresión", 
			                               new CellRendererText(), 
			                               "text", 1);
			
			tokenizingRulesTV.Columns[0].Sizing = TreeViewColumnSizing.Autosize;
			tokenizingRulesTV.Columns[1].Sizing = TreeViewColumnSizing.Autosize;
			
			
				
			
			tokenizingButtonsNB.Page = 0;
			tokenizingStepsNB.Page = 0;
			
			tokenizingStageWidgetBase.ShowAll();
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
		/// Writes a message saying the matching process failed.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnControllerMatchingFailed(object sender, EventArgs a)
		{
			Application.Invoke(delegate(object resender, EventArgs args)
            {
				matchingResultLbl.Markup =
				"<b>La secuencia no pudo ser asociada a ningún item</b>";
			
				if(controller.StepMode != ControllerStepMode.UntilEnd)
					tokenizingNextButtonsAlign.Sensitive = true;
			});
		}
		
		/// <summary>
		/// If sequencing, selects the next symbol of the list.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgas"/>
		/// </param>
		private void OnControllerNodeBeingProcessed(object sender, 
		                                            NodeBeingProcessedArgs _args)
		{
			Application.Invoke(sender, _args, 
			                   delegate(object resender, EventArgs args)
			{
				NodeBeingProcessedArgs a =  args as NodeBeingProcessedArgs;
			
				if(!sequencingFinished)
				{
					// We are sequencing.	
					
					// Selects the new first.			
					symbolsIV.SelectPath(processedPath);
					symbolsIV.ScrollToPath(processedPath,1,0.5f);
					processedPath.Next();
					
				}
				else
				{
					// We are matching
					SequenceNode node = (SequenceNode) a.Node;
					
					node.Select();
					
					Token t = Token.Join(node.Sequence, "");
					
					sequenceNodeImage = t.Image.CreatePixbuf();
					
					sequenceMatchingImageArea.Image = sequenceNodeImage.Copy();
					
					sequenceNodeImage = 
						sequenceNodeImage.CompositeColorSimple(sequenceNodeImage.Width,
						                                    sequenceNodeImage.Height,
						                                    Gdk.InterpType.Nearest, 
						                                    100, 1,  
						                                    0xAAAAAA,0xAAAAAA);
					
					matchingResultLbl.Markup = "-";
					
					tokenizingRulesTV.Selection.UnselectAll();
					
					tokenizingRulesTV.ScrollToPoint(0,0);
					
					
					if(controller.StepMode == ControllerStepMode.StepByStep)
					{
						tokenizingNextButtonsAlign.Sensitive = true;
					}
				}
			});
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
		                                       SequenceAddedArgs _args)
		{
			Application.Invoke(sender, _args, 
			                   delegate(object resender, EventArgs args)
			{
				SequenceAddedArgs a = args as SequenceAddedArgs;
			
				sequencesModel.AddNode(a.Sequence);
				//sequencesNV.ColumnsAutosize();
				sequencesNV.NodeSelection.SelectNode(a.Sequence);
				sequencesNV.ScrollToCell(sequencesNV.Selection.GetSelectedRows()[0],
				                         sequencesNV.Columns[0],
				                         true,
				                         1,0);
				
				if(currentToken!=null)
				{
					sequencingStepResultLbl.Markup=
						String.Format("<b>El símbolo «{0}» no se pudo añadir a ninguna secuencia, se crea una nueva</b>",
						              currentToken.Text);
				}
			
				
				sequencesNV.ColumnsAutosize();
			});
		}
		
		
		/// <summary>
		/// Handles the controller's SequenceBeingMatched event.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="quenceBeingMatchedArgs">
		/// A <see cref="Se"/>
		/// </param>
		private void OnControllerSequenceBeingMatched(object sender, 
		                                              SequenceBeingMatchedArgs args)
		{
			Application.Invoke(sender,
			                   args,
			                   delegate(object resender, EventArgs _args)
			{
				SequenceBeingMatchedArgs a =  _args as SequenceBeingMatchedArgs;
			
			
				Gdk.Pixbuf sequenceImage = a.JoinedToken.Image.CreatePixbuf();
				
				Gdk.Pixbuf drawnImage = sequenceNodeImage.Copy();
				
				sequenceImage.CopyArea(0, 0, 
				                       sequenceImage.Width, sequenceImage.Height,
				                       drawnImage,
				                       0,0);
				
				sequenceMatchingImageArea.Image = drawnImage;
				
				TreeIter iter;
				tokenizingRulesTV.Model.GetIterFirst(out iter);
				
				TreePath path = tokenizingRulesTV.Model.GetPath(iter);
				
				tokenizingRulesTV.Selection.UnselectAll();
					
				string ruleName;
				do
				{
					ruleName = tokenizingRulesTV.Model.GetValue(iter,0) as string;
					
					if(ruleName == a.MatchingRule.Name)
					{
						tokenizingRulesTV.Selection.SelectPath(path);
						tokenizingRulesTV.ScrollToCell(path,
						                               tokenizingRulesTV.Columns[0],
						                               true,
						                               0.5f, 0);
						break;
					}
					
					path.Next();
						
					
				}while(tokenizingRulesTV.Model.GetIter(out iter, path));
				
				if(a.Found)
				{
					matchingResultLbl.Markup=
						String.Format("<b>Sí, se le asigna el item «{0}» a la secuencia actual</b>",
						              a.JoinedToken.Type);
				}
				else
				{
					matchingResultLbl.Markup=
						String.Format("<b>No, la regla actual no concuerda con la secuencia</b>");
				}
				
				// Activate the buttons if necessary.
				if(controller.StepMode == ControllerStepMode.StepByStep)
					tokenizingNextButtonsAlign.Sensitive = true;
			});
		}
		
		
		/// <summary>
		/// Reacts to the controller saying has done a new step.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnControllerStepDone(object sender, EventArgs args)
		{
			Application.Invoke(delegate(object resender, EventArgs _args)
			{
				if(controller.StepMode != ControllerStepMode.UntilEnd)
					tokenizingNextButtonsAlign.Sensitive = true;
			});
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
		private void OnControllerTokenChecked(object sender, 
		                                            TokenCheckedArgs args)
		{
			Application.Invoke(sender, args,
			                   delegate(object resender, EventArgs _args)
			{
				TokenCheckedArgs a = _args as TokenCheckedArgs;
			
				if(!sequencingFinished)
				{
					currentToken = a.CurrentToken;
					
				
					FloatBitmap sequenceImage;
					
					if(a.LastSequence!= null)
					{
						TokenSequence joinSeq = 
							new TokenSequence(a.LastSequence);
						lastToken = joinSeq.Last;	
						joinSeq.Append(currentToken);
						Token joinedToken =Token.Join(joinSeq, "");
						
						sequenceImage = joinedToken.Image;
						
					}
					else
					{
						sequenceImage = currentToken.Image;					
						lastToken = null;
					}
					
					// We add a border to the orginal image.
					
					Gdk.Pixbuf sequencePixbuf = sequenceImage.CreatePixbuf();
					
					Gdk.Pixbuf drawnImage = 
						new Gdk.Pixbuf(sequencePixbuf.Colorspace,false, 8, 
						               sequencePixbuf.Width+10, 
						               sequencePixbuf.Height+10);
					
					drawnImage.Fill(0xFFFFFFFF);
					
					sequencePixbuf.CopyArea(0, 0, 
				                        sequencePixbuf.Width, 
				                        sequencePixbuf.Height,
				                        drawnImage,
				                        5,5);
					
					if(lastToken!=null)
					{
						uint color;
						if(currentToken.CloseFollows(lastToken))
						{
							color = 0x00FF00;
							sequencingStepResultLbl.Markup = 
								String.Format("<b>Sí, el símbolo «{0}» se añadirá a la secuencia actual</b>",
								              currentToken.Text);
							
						}
						else
						{
							color = 0xFF0000;
							sequencingStepResultLbl.Markup = 
								String.Format("<b>No, «{0}» no puede ser considerado parte de la secuencia ({1})</b>",
								              currentToken.Text,
								              a.LastSequence.ToString());
						}
						
						
						Gdk.Pixbuf markedImage = drawnImage.Copy();
						
						// We paint the image of the color
						markedImage = 
							markedImage.CompositeColorSimple(markedImage.Width, 
							                                 markedImage.Height,
							                                 Gdk.InterpType.Nearest,
							                                 100, 1, color, color);
						
						// We are going to mark the image of the to symbols being considered
						// with their baselines.
						int min = int.MaxValue;
						foreach (Token t in a.LastSequence ) 
						{
							if(t.Top < min)
								min =t.Top;
						}
						
						int offset = Math.Min(min, currentToken.Top);
						int lastBaseline = lastToken.Baseline - offset;
						int currentBaseline = currentToken.Baseline - offset;
						
						markedImage.CopyArea(0, lastBaseline, 
						                     markedImage.Width, 5,
						                     drawnImage, 
						                     0,lastBaseline);
						
						markedImage.CopyArea(0, currentBaseline, 
						                     markedImage.Width,5, 
						                     drawnImage, 
						                     0,currentBaseline);
					}
					
							
					baselineImageArea.Image = drawnImage;
				}
				
				if(controller.StepMode == ControllerStepMode.StepByStep)
					tokenizingNextButtonsAlign.Sensitive = true;
			});
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
				symbolsIV.ScrollToPath(processedPath, 1, 0.5f);
			}
			
			tokenizingButtonsNB.Page = 1;
			tokenizingNextButtonsAlign.Sensitive = true;
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
			Application.Invoke(delegate(object resender, EventArgs args)
            {
				// We have finished.
				nextStageBtn.Sensitive = sequencingFinished; 
				processBtn.Sensitive = !sequencingFinished;
				
				// The state has changed.
				if(!sequencingFinished)
				{
					sequencingFinished = true;
					OkDialog.Show(this.MainRecognizerWindow.Window,
					              MessageType.Info,
					              "El proceso de secuenciación de los símbolos a terminado, puede procederse a extraer los items de las secuencias.");
				}
					
				else
				{
					OkDialog.Show(this.MainRecognizerWindow.Window,
					              MessageType.Info,
					              "El proceso de análisis léxico ha terminado, ahora puedes revisar el resultado.");
					
					processBtn.Sensitive = false;
					processFinished = true;
					
					matchingResultLbl.Markup= "-";
					
					tokenizingRulesTV.Selection.UnselectAll();
					tokenizingRulesTV.ScrollToPoint(0,0);
				}
			
			
				if(sequencingFinished)
				{
					// We retrieve the rules.
					controller.SetLexicalRules(MainRecognizerWindow.LexicalRulesManager.LexicalRules);
					
					this.tokenizingRulesTV.Model = 
						MainRecognizerWindow.LexicalRulesManager.RulesStore;
					
					tokenizingStepsNB.Page = 1;				
					processBtnLbl.Text = "_Extraer items";
					processBtnLbl.UseUnderline = true;
				}
				
				
				// We change to the first page
				tokenizingButtonsNB.Page = 0;	
			});
			
		}
		
	
		
		/// <summary>
		/// Ask the user for his permissions to perform a new lexical
		/// analysis on the selected sequence.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="arg">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnForceTokenizingItemActivate(object sender, EventArgs arg)
		{
			
			if(selectedNode.FoundToken !=null || selectedNode.ChildCount >0)
			{
				// We need that the user confirm the operation as, it is desctructive.
				ResponseType res =
					ConfirmDialog.Show(this.MainRecognizerWindow.Window,
					                   "Se van a perder los items identificados en la secuencia seleccionada (o sus hijos), ¿deseas continuar?");
				if(res == ResponseType.No)
					return;
			}
			
			selectedNode.RemoveSequenceChildren();
			selectedNode.FoundToken = null;
			
			// We set again the rules, because the user _should_ have modified them.
			controller.SetLexicalRules(MainRecognizerWindow.LexicalRulesManager.LexicalRules);
			
			controller.SetSequenceForTokenizing(selectedNode);
			
			tokenizingButtonsNB.Page = 1;
			tokenizingNextButtonsAlign.Sensitive = true;
			
			
			// We lauch the tokenizer again.
			NextStep(ControllerStepMode.StepByStep);
			
				
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
				
				OkDialog.Show(this.MainRecognizerWindow.Window,
				              MessageType.Info,
				              "Para continuar a la siguente fase de procesado, debes solucionar los siguentes problemas:\n\n{0}",
				              errorss);
			}
			else
			{
				MainRecognizerWindow.CreateParsingWidget();
				this.NextStage();
			}
		}
		
		/// <summary>
		/// Shows the contextual menu for the sequences' nodes.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="ButtonPressEventArgs"/>
		/// </param>
		[GLib.ConnectBeforeAttribute]
		private void OnSequencesNVButtonPress(object sender,
		                                           ButtonPressEventArgs args)
		{
			if(processFinished && args.Event.Button == 3)
			{
				TreePath path = new TreePath();
                // Obtenemos el treepath con las coordenadas del cursor.
                sequencesNV.GetPathAtPos(System.Convert.ToInt16 (args.Event.X), 
				                      System.Convert.ToInt16 (args.Event.Y),				              
				                      out path);
                
				if( path != null)
				{
					// We try only if a node was found.			
					SequenceNode node =
						(SequenceNode)(sequencesModel.GetNode(path));	
					
					
					selectedNode = node;										
					sequenceNodeMenu.Popup();	
				}
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
		
		private void OnTokenizingBackBtnClicked(object sender, EventArgs args)
		{
			if(processFinished)
			{
				// The parsing finished and was successful
				ResponseType res = 
					ConfirmDialog.Show(MainRecognizerWindow.Window,
					                   "Si regresas al paso anterior se perderá el análisis realizado, ¿quieres continuar?");
				
				if(res == ResponseType.No)
					return;
			}
		
			PreviousStage();
		}
		
		/// <summary>
		/// Auxiliary method tasked with telling the controller to resume the
		/// working process.
		/// </summary>
		/// <param name="mode">
		/// A <see cref="ControllerStepMode"/> that tells the controller
		/// when the next stop shoul be made.
		/// </param>
		protected override void NextStep (ControllerStepMode mode)
		{
			controller.Next(mode);
		}
		
		
#endregion Non-public methods
	}
}
