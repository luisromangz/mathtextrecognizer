// FormulaMatchingWidget.cs created with MonoDevelop
// User: luis at 12:47 09/05/2008

using System;
using System.Threading;
using System.Collections.Generic;

using Gtk;
using Glade;

using MathTextCustomWidgets.Dialogs;
using MathTextCustomWidgets.Widgets.ImageArea;

using MathTextLibrary.Analisys;
using MathTextLibrary.Controllers;
using MathTextLibrary.Utils;

using MathTextRecognizer.Controllers;
using MathTextRecognizer.Controllers.Nodes;

namespace MathTextRecognizer.Stages
{

	
	/// <summary>
	/// This class implements a widget to be used as interface to show 
	/// the progress in the formula matching process.
	/// </summary>
	public class ParsingStageWidget : RecognizingStageWidget
	{		
			
#region Glade widgets
		[Widget]
		private VBox parsingStageBaseWidget = null;
		
		[Widget]
		private ScrolledWindow syntacticalTreePlaceholder = null;
		
		[Widget]
		private Notebook parsingButtonsNB = null;
		
		[Widget]
		private Button parsingProcessBtn = null;
		
		[Widget]
		private Button parsingShowOutputBtn = null;
		
		[Widget]
		private Alignment parsingNextButtonsAlign = null;
		
		[Widget]
		private IconView remainingItemsIconView = null;
		
		[Widget]
		private Alignment synImageOriginalPlaceholder = null;
	
#endregion Glade widgets
	
		
#region Fields
		private ParsingController controller;
		
		private NodeView syntacticalCoverTree;
		private NodeStore syntacticalCoverModel;
		
		private SyntacticalCoverNode currentNode;
		
		private ListStore remainingItemsStore;
		
		private TreeIter selectedRemainingItem;
		
		private ImageArea originalImageArea;
		
		private Gdk.Pixbuf originalImage;
		
		
#endregion Fields
		
		/// <summary>
		/// <see cref="ParsingStageWidget"/>'s constructor.
		/// </summary>
		/// <param name="window">
		/// The <see cref="MainRecognizerWindow"/> that contains the widget.
		/// </param>
		public ParsingStageWidget(MainRecognizerWindow window) : base(window)
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "parsingStageBaseWidget");
			
			gladeXml.Autoconnect(this);
			
			this.Add(parsingStageBaseWidget);
			
			
			
			InitializeWidgets();
			
			controller = new ParsingController();
			controller.MessageLogSent += 
				new MessageLogSentHandler(OnControllerMessageLogSent);
			
			controller.ProcessFinished += OnControllerProcessFinishedHandler;
			
			controller.Matching += 
				new MatchingHandler(OnControllerMatching);
			
			controller.MatchingFinished += 
				new MatchingFinishedHandler(OnControllerMatchingFinished);
			
			controller.StepDone += 
				new EventHandler(OnControllerStepDone);
			
			controller.TokenMatching +=
				new TokenMatchingHandler(OnControllerTokenMatching);
			
			controller.TokenMatchingFinished+=
				new TokenMatchingFinishedHandler(OnControllerTokenMatchingFinished);
			
			controller.RuleSequenceRestored += 
				new SequenceSetHandler(OnControllerSequenceRestored);
			
			controller.RelatedSequenceSet += 
				new SequenceSetHandler(OnControllerRelatedSequenceSet);
			
		
			this.ShowAll();
		}
		
		/// <summary>
		/// <c>FormulaMatchingStageWidget</c>'s static fields initializer.
		/// </summary>
		static ParsingStageWidget()
		{
			widgetLabel = "Análisis sintáctico";
		}
		

		
#region Public methods
			/// <summary>
		/// Set the widget to its initial state.
		/// </summary>
		public override void ResetState ()
		{
			parsingButtonsNB.Page = 0;
			
			parsingNextButtonsAlign.Sensitive = true;
			parsingProcessBtn.Sensitive = true;
			
			parsingShowOutputBtn.Sensitive = false;
			
			syntacticalCoverModel.Clear();
			remainingItemsStore.Clear();
		}
		
			
		public override void Abort ()
		{
			controller.Abort();
		}
		
		public override void SetInitialData()			
		{
			originalImage = MainRecognizerWindow.OCRWidget.StartImage;
			this.originalImageArea.Image = originalImage;
			SetRemainingTokens(MainRecognizerWindow.TokenizingWidget.ResultTokens);
		}
		
		
#endregion Public methods
		
#region Non-public methods		
		
		
	
		/// <summary>
		/// Initializes the child widgets of the widget.
		/// </summary>
		private void InitializeWidgets()
		{
			syntacticalCoverModel = 
				new NodeStore(typeof(SyntacticalCoverNode));
			
			syntacticalCoverTree = new NodeView(syntacticalCoverModel);
			syntacticalCoverTree.RulesHint = true;
			
			syntacticalCoverTree.AppendColumn("Elemento de análisis",
			                                  new CellRendererText(),
			                                  "markup" ,0);
			
			syntacticalCoverTree.RowActivated+= 
				new RowActivatedHandler(OnSyntacticalCoverTreeRowActivated);
			
			syntacticalCoverTree.Selection.Changed +=
				new EventHandler(OnSyntacticalCoverTreeSelectionChanged);
			
			//syntacticalCoverTree.ShowExpanders = false;
			
			syntacticalTreePlaceholder.Add(syntacticalCoverTree);
			
			
			remainingItemsStore = new ListStore(typeof(Gdk.Pixbuf),
			                                    typeof(string),
			                                    typeof(Token));
			remainingItemsIconView.Model = remainingItemsStore;
			
			remainingItemsIconView.PixbufColumn =0;
			remainingItemsIconView.TextColumn = 1;
			
			originalImageArea = new  ImageArea();
			originalImageArea.ImageMode = ImageAreaMode.Zoom;
			
			synImageOriginalPlaceholder.Add(originalImageArea);
			
			originalImageArea.ShowAll();
		}
		
		protected override void NextStep (ControllerStepMode mode)
		{
			parsingNextButtonsAlign.Sensitive = false;
			controller.Next(mode);
		}
		
		private void OnControllerMatchingFinished(object sender, MatchingFinishedArgs args)
		{
			Application.Invoke(sender, args, delegate(object resender, EventArgs a)
			{
				MatchingFinishedArgs _args = a as MatchingFinishedArgs;
				
				if(controller.StepMode != ControllerStepMode.UntilEnd)
				{
					parsingNextButtonsAlign.Sensitive = true;
				}
				
				currentNode.Select();
				currentNode.SetOutput(_args.Output);
				
			});
		}
		
		
		/// <summary>
		/// Handles the end of the syntactical analisys process.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnControllerProcessFinishedHandler(object sender, EventArgs args)
		{
			Application.Invoke(delegate(object resender, EventArgs a)
			{
				if(controller.ParsingResult)
				{
					OkDialog.Show(this.MainRecognizerWindow.Window,
					              MessageType.Info,
					              "¡El proceso de análisis sintáctico fue un éxito!");		
					
					
					
					parsingShowOutputBtn.Sensitive = true;
					parsingProcessBtn.Sensitive = false;
					
					currentNode.SetOutput(controller.Output);
				}
				else
				{
					OkDialog.Show(this.MainRecognizerWindow.Window,
					              MessageType.Warning,
					              "El proceso de análisis sintáctico no tuvo éxito.");	
					
					
					
				}
				
				parsingButtonsNB.Page = 0;
			});
		}
		
		private void OnControllerMatching(object sender, 
		                                  MatchingArgs _args)
		{
			Application.Invoke(sender, 
			                   _args,
			                   delegate(object resender, EventArgs a)
			{
				MatchingArgs args = a as MatchingArgs;
				SyntacticalCoverNode newNode = 
					new SyntacticalCoverNode(args.Matcher,
					                         syntacticalCoverTree);
				
				if(currentNode == null)
				{
					syntacticalCoverModel.AddNode(newNode);
				}
				else
				{
					currentNode.Select();	
					currentNode.AddChild(newNode);
					syntacticalCoverTree.ExpandRow(syntacticalCoverTree.Selection.GetSelectedRows()[0],
					                               false);
					
				}				
				
				currentNode = newNode;			
				
				
				currentNode.Select();
				
							
				
				parsingNextButtonsAlign.Sensitive = 
					controller.StepMode == ControllerStepMode.StepByStep;
			});
		}
		
		
		private void OnControllerRelatedSequenceSet(object sender, 
		                                            SequenceSetArgs a)
		{
			Application.Invoke(sender, a, 
			                   delegate(object resender, EventArgs _a)
			{
				SequenceSetArgs args = _a as SequenceSetArgs;
				
				this.SetRemainingTokens(args.NewSequence.Sequence);
				
				if(controller.StepMode != ControllerStepMode.UntilEnd)
				{
					parsingNextButtonsAlign.Sensitive = true;
				}
					
			});
		}
		
		
		private void OnControllerSequenceRestored(object sender, 
		                                          SequenceSetArgs args)
		{
			
			Application.Invoke(sender, args, delegate(object resender, 
			                                          EventArgs a)
			{
				this.SetRemainingTokens(args.NewSequence.Sequence);
			});
		}
		
		private void OnControllerStepDone(object sender, 
		                                  EventArgs args)
		{
			Application.Invoke(delegate(object resender, EventArgs a)
			{
				if(currentNode.Parent !=null)
				{
					
					currentNode =  currentNode.Parent as SyntacticalCoverNode;	
					currentNode.Select();
					syntacticalCoverTree.CollapseRow(syntacticalCoverTree.Selection.GetSelectedRows()[0]);
					
				}
				
				MarkImage(null);
				
				if(controller.StepMode == ControllerStepMode.StepByStep)
				{
					parsingNextButtonsAlign.Sensitive = true;
				}
			});				
		}
		
		private void OnControllerTokenMatching(object sender,
		                                       TokenMatchingArgs _args)
		{
			Application.Invoke(sender, _args, 
			                   delegate(object resender, EventArgs a)
            {
				currentNode.Select();
				
				TokenMatchingArgs args = a as TokenMatchingArgs;
			
				if(controller.StepMode == ControllerStepMode.StepByStep)
				{
					parsingNextButtonsAlign.Sensitive = true;
				}
			});
		}
		
		
		
		private void OnControllerTokenMatchingFinished(object sender, 
		                                               TokenMatchingFinishedArgs _args)
		{
			Application.Invoke(sender, 
			                   _args, 
			                   delegate(object resender, EventArgs a)
			{
				TokenMatchingFinishedArgs args = a as TokenMatchingFinishedArgs;
			
				if(args.MatchedToken != null)
				{
					int idx = SearchToken(args.MatchedToken);
					remainingItemsStore.IterNthChild(out selectedRemainingItem,
					                                 idx);
					TreePath selectedPath = 
						remainingItemsStore.GetPath(selectedRemainingItem);
				
					remainingItemsIconView.SelectPath(selectedPath);
					remainingItemsIconView.ScrollToPath(selectedPath, 0.5f, 0f);
					
					this.MarkImage(args.MatchedToken);
					
					currentNode.AddMatchedToken(args.MatchedToken);	
					currentNode.Select();
					
					remainingItemsStore.Remove(ref selectedRemainingItem);					
					
				}
				
				if(controller.StepMode != ControllerStepMode.UntilEnd)
				{
					parsingNextButtonsAlign.Sensitive = true;
				}
			});
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
		private void OnParsingNextStepBtnClicked(object sender, EventArgs args)
		{
			MainRecognizerWindow.LogAreaExpanded = true;
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
		private void OnParsingNextNodeBtnClicked(object sender, EventArgs args)
		{
			NextStep(ControllerStepMode.NodeByNode);
		}
		
		/// <summary>
		/// Tells the controller to process until it has finished.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnParsingTillEndBtnClicked(object sender, EventArgs args)
		{
			NextStep(ControllerStepMode.UntilEnd);
		}
		
		private void OnParsingProcessBtnClicked(object sender, EventArgs args)
		{
		
			
			
			
			// We set the tokens from the previous step.
			controller.SetStartTokens(MainRecognizerWindow.TokenizingWidget.ResultTokens);
			
			// We set the rules library.
			SyntacticalRulesLibrary.Instance.ClearRules();
			
			List<SyntacticalRule> rules = 
				MainRecognizerWindow.SyntacticalRulesManager.SyntacticalRules;
			
			foreach (SyntacticalRule rule in  rules) 
			{
				SyntacticalRulesLibrary.Instance.AddRule(rule);
			}
			
			SyntacticalRulesLibrary.Instance.StartRule = rules[0];
			
			parsingButtonsNB.Page = 1;
			
			currentNode = null;
			
			controller.Next(ControllerStepMode.StepByStep);
		}
		
		/// <summary>
		/// Shows the dialog with the output produced by the syntactical
		/// analisys process.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnParsingShowOutputBtnClicked(object sender, EventArgs args)
		{
			// We know the output is correct because if not the button 
			// wouldn't have been sensitivized.
			string output =  controller.Output;
			
			Output.OutputDialog dialog = 
				new Output.OutputDialog(MainRecognizerWindow,output);
			
			dialog.Show();
			dialog.Destroy();
		}
		
		private void OnParsingBackBtnClicked(object sender, EventArgs args)
		{
			if(parsingShowOutputBtn.Sensitive)
			{
				// The parsing finished and was successful
				ResponseType res = ConfirmDialog.Show(MainRecognizerWindow.Window,
				                                      "Si vuelves al paso anterior se perderá el análisis realizado, ¿quieres continuar?");
				
				if(res == ResponseType.No)
					return;
			}
			
			controller.DeregisterEvents();
			PreviousStage();
			
		}
		
		private void OnSyntacticalCoverTreeRowActivated(object sender,
		                                                RowActivatedArgs args)
		{
			if(syntacticalCoverTree.GetRowExpanded(args.Path))			
			{
				syntacticalCoverTree.CollapseRow(args.Path);
			}
			else
			{
				syntacticalCoverTree.ExpandRow(args.Path,true);
			}
			
		}
		
		private void OnSyntacticalCoverTreeSelectionChanged(object sender,
		                                                    EventArgs args)
		{
			
			
		}
		
		/// <summary>
		/// Adds the remaining tokens to the icon view.
		/// </summary>
		/// <param name="remainingTokens">
		/// A <see cref="List'1"/> containing the remaining tokens.
		/// </param>
		private void SetRemainingTokens(List<Token> remainingTokens)
		{
			remainingItemsStore.Clear();
			
			
			remainingItemsIconView.Columns = remainingTokens.Count;
			foreach (Token remainingToken in remainingTokens.ToArray()) 
			{
				Gdk.Pixbuf thumbnail = 
					ImageUtils.MakeThumbnail(remainingToken.Image.CreatePixbuf(),
					                         48);
				remainingItemsStore.AppendValues(thumbnail,
				                                 remainingToken.Text,
				                                 remainingToken);
			}
		}
		
		private void MarkImage(Token t)
		{
			if(t ==null)
			{
				originalImageArea.Image=originalImage;
				return;
			}
			
			Gdk.Pixbuf originalMarked= originalImage.Copy();	
			
			// We tint the copy in red.
			originalMarked = 
				originalMarked.CompositeColorSimple(originalMarked.Width,
				                                    originalMarked.Height,
				                                    Gdk.InterpType.Bilinear,
				                                    100,1,
				                                    0xAAAAAA,0xAAAAAA);
				
			// Over the red tinted copy, we place the piece we want to be
			// normal.
			originalImage.CopyArea(t.Left,
			                       t.Top,
			                       t.Width,
			                       t.Height,
			                       originalMarked,
			                       t.Left,
			                       t.Top);
		
			
			originalImageArea.Image=originalMarked;
			
		}
		
		
		private int SearchToken(Token lookedUpon)
		{
			int i = -1;
			
			foreach (object[] values in remainingItemsStore) 
			{
				i++;
				if(((Token)(values[2]))==lookedUpon)
				{
					return i;
				}
			}			
			
			return i;
		}
		
	
#endregion Non-public methods
	}
}
