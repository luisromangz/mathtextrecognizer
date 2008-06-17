// BlackboardStageWidget.cs created with MonoDevelop
// User: luis at 12:37 04/06/2008

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

using Gtk;
using Glade;


using MathTextCustomWidgets.Widgets.ImageArea;
using MathTextCustomWidgets.Widgets.HandWriting;
using MathTextCustomWidgets.Dialogs;

using MathTextLibrary.Symbol;
using MathTextLibrary.Analisys;
using MathTextLibrary.Bitmap;
using MathTextLibrary.Controllers;

using MathTextRecognizer.Controllers;
using MathTextRecognizer.Controllers.Nodes;

using MathTextRecognizer.Stages.Dialogs;

namespace MathTextRecognizer.Stages
{
	
	/// <summary>
	/// This class implements a widget used for the processing 
	/// of handwritteng formulas in an unassisted fashion.
	/// </summary>
	public class BlackboardStageWidget : RecognizingStageWidget
	{
		
#region Widgets
		[Widget]
		private Alignment unassistedStageWidgetBase = null;
		
		[Widget]
		private Button unassistedShowOutputBtn = null;
		
		[Widget]
		private ProgressBar unassistedTaskProgressBar = null;
		
		[Widget]
		private ProgressBar unassistedGlobalProgressBar = null;
		
		[Widget]
		private HButtonBox unassistedControlHBB = null;
		
		[Widget]
		private Label unassistedTaskNameLabel = null;
		
		
		[Widget]
		private Button unassistedProcessBtn = null;
		
		[Widget]
		private Alignment unassistedImagePlaceholder = null;
		
		[Widget]
		private Menu blackboardMenu = null;
		
		
#endregion Widgets
		
#region Fields
		
		private HandWritingArea handwritingArea;
		
		private NodeStore segmentationStore;
		
		private OCRController ocrController;
		private TokenizingController tokenizingController;
		private ParsingController parsingController;
		
		private bool tokenizingFinished;
		
		private Gdk.Pixbuf originalImage;
		
#endregion Fields
		
#region Constructors
		
		public BlackboardStageWidget(MainRecognizerWindow parent)
			: base(parent)
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",
			                             "unassistedStageWidgetBase");
			
			gladeXml.Autoconnect(this);
			
			gladeXml = new XML("mathtextrecognizer.glade",
			                   "blackboardMenu");
			
			gladeXml.Autoconnect(this);
			
			this.Add(unassistedStageWidgetBase);
			
			ocrController = new OCRController();
			ocrController.MessageLogSent += 
				new MessageLogSentHandler(OnControllerMessageLogSent);
			
			ocrController.ProcessFinished += 
				new EventHandler(OnOCRControllerProcessFinished);
			ocrController.NodeBeingProcessed += 
				new NodeBeingProcessedHandler(OnControllerStepDone);
					
			tokenizingController = new TokenizingController();
			tokenizingController.MessageLogSent += 
				new MessageLogSentHandler(OnControllerMessageLogSent);
			tokenizingController.ProcessFinished += 
				new EventHandler(OnTokenizingControllerProcessFinished);
			tokenizingController.NodeBeingProcessed += 
				new NodeBeingProcessedHandler(OnControllerStepDone);
			
			parsingController = new ParsingController();
			parsingController.MessageLogSent += 
				new MessageLogSentHandler(OnControllerMessageLogSent);
			parsingController.Matching += 
				new MatchingHandler(OnControllerStepDone);
			
			parsingController.ProcessFinished +=
				new EventHandler(OnParsingControllerProcessFinished);
			
			InitializeWidgets();
			
			tokenizingFinished = false;
			
			this.ShowAll();
		}
		
		static BlackboardStageWidget()
		{
			widgetLabel =  "Pizarra virtual";
		}
		
#endregion Constructors
		
#region Public methods
		
	
		
		public override void Abort()
		{
			ocrController.Abort();
			tokenizingController.Abort();
			parsingController.Abort();
		}
		
#endregion Public methods
		
#region Non-public methods
		
		private void InitializeWidgets()
		{
			unassistedProcessBtn.Sensitive = true;
			
			handwritingArea = new RasterHandWritingArea();
			handwritingArea.SmoothingMode = SmoothingMode.AntiAlias;
			handwritingArea.LineStyle = new Pen(Color.Black, 3);
			
			handwritingArea.ButtonPressEvent += 
				new ButtonPressEventHandler(OnHandwritingAreaButtonPress);
			
			unassistedImagePlaceholder.Add(handwritingArea);
			
			segmentationStore = new NodeStore(typeof(SegmentedNode));
		}	
	
		
#endregion Non-public methods
		
#region Event handlers
		
		private void OnOCRControllerProcessFinished(object sender, EventArgs args)
		{
			Application.Invoke(delegate(object resender, EventArgs a)
			{
				unassistedGlobalProgressBar.Fraction = 0.33;
				
				unassistedTaskNameLabel.Text = "Secuenciación";
				
				List<Token> startTokens = new List<Token>();
				foreach (SegmentedNode node in ocrController.Result) 
				{
					if(node.Symbols.Count != 1)
					{
						SymbolLabelEditorDialog dialog = 
							new SymbolLabelEditorDialog(this.MainRecognizerWindow.Window, node);
							
						ResponseType res = dialog.Show();
						string label = dialog.Label;
						dialog.Destroy();
						
						if(res == ResponseType.Ok)
						{
							node.Symbols.Clear();					
							node.Symbols.Add(new MathSymbol(label));
							node.SetLabels();
						}
						else
						{
							OkDialog.Show(this.MainRecognizerWindow.Window,
							              MessageType.Error,
							              "La fase de reconocimiento y segementado de imágenes falló.");
							
							unassistedControlHBB.Sensitive = true;
							return;
						}
						
						
						
					}
					
					
					startTokens.Add(new Token(node.Label,
					                          node.MathTextBitmap.Position.X,
					                          node.MathTextBitmap.Position.Y,
					                          node.MathTextBitmap.FloatImage));
				}
				
				
				tokenizingFinished = false;
				tokenizingController.SetLexicalRules(Config.RecognizerConfig.Instance.LexicalRules);
				tokenizingController.SetInitialData(startTokens, null);
				
				tokenizingController.Next(ControllerStepMode.UntilEnd);
			});
		}
		
		private void OnControllerMessageLogSent(object sender, MessageLogSentArgs _args)
		{
			Application.Invoke(sender, _args, 
			                   delegate(object resender, EventArgs a)
			{
				MessageLogSentArgs args = a as MessageLogSentArgs;
				
				MainRecognizerWindow.Log(args.Message);
			});
		}
		
		private void OnUnassistedBackBtnClicked(object sender, EventArgs args)
		{
			
			parsingController.DeregisterEvents();
			this.PreviousStage();
		}
		
		private void OnUnassistedProcessBtnClicked(object sender, EventArgs args)
		{
			// We try to free memory.
			GC.Collect();
			segmentationStore.Clear();
			
			
			MainRecognizerWindow.ProcessItemsSensitive = true;
			unassistedControlHBB.Sensitive = false;
			unassistedTaskNameLabel.Text = "Segmentación y OCR";
			
			unassistedGlobalProgressBar.Fraction = 0;
			
			// We create the image to be recognized.
			
			Bitmap  bitmap = handwritingArea.Bitmap;
			
			FloatBitmap floatBitmap = 
				new FloatBitmap(bitmap.Width, bitmap.Height);
			
			for(int i=0; i< bitmap.Width; i++)
			{
				for(int j = 0; j < bitmap.Height; j++)
				{
					floatBitmap[i,j] = bitmap.GetPixel(i,j).GetBrightness();
				}
			}
			
			// The original image is set.
			originalImage = floatBitmap.CreatePixbuf();
			
			MathTextBitmap mtb = new MathTextBitmap(originalImage);
			
			SegmentedNode node = new SegmentedNode("Raíz",
			                                       mtb,
			                                       null);
			    
			segmentationStore.AddNode(node);
			
			ocrController.StartNode = node;
			ocrController.SearchDatabase = false;
			ocrController.Databases =  
				Config.RecognizerConfig.Instance.Databases;
			ocrController.Next(ControllerStepMode.UntilEnd);
		}
		
		private void OnUnassistedShowOutputBtnClicked(object sender, EventArgs args)
		{
			
			MathTextRecognizer.Output.OutputDialog dialog = 
				new MathTextRecognizer.Output.OutputDialog(MainRecognizerWindow,
				                                           parsingController.Output,
				                                           originalImage);
			dialog.Show();
			dialog.Destroy();
		}
		
		private void OnControllerStepDone(object sender, EventArgs args)
		{
			Application.Invoke(delegate(object resender, EventArgs a)
			{
				unassistedTaskProgressBar.Pulse();
			});
			
		}
		
		private void OnTokenizingControllerProcessFinished(object sender, EventArgs args)
		{
			Application.Invoke(delegate(object resender, EventArgs a)
			{
				if(!tokenizingFinished)
				{					
					tokenizingFinished = true;
					unassistedTaskNameLabel.Text = "Análisis léxico";
					unassistedGlobalProgressBar.Fraction = 0.5;
					tokenizingController.Next(ControllerStepMode.UntilEnd);
				}
				else
				{
					unassistedTaskNameLabel.Text = "Análisis sintáctico";
					unassistedGlobalProgressBar.Fraction = 0.66;
					
					List<Token> result = tokenizingController.Result;
					
					SyntacticalRulesLibrary.Instance.ClearRules();
					foreach (SyntacticalRule rule in  
					         Config.RecognizerConfig.Instance.SyntacticalRules) 
					{
						SyntacticalRulesLibrary.Instance.AddRule(rule);
					}
					
					SyntacticalRulesLibrary.Instance.StartRule = 
						Config.RecognizerConfig.Instance.SyntacticalRules[0];
					parsingController.SetStartTokens(result);
					parsingController.Next(ControllerStepMode.UntilEnd);
				}
			});
		}
		
		
		
		private void OnParsingControllerProcessFinished(object sender,
		                                                EventArgs args)
		{
			Application.Invoke(delegate(object resender, EventArgs a)
			{
				unassistedGlobalProgressBar.Fraction =1;
				unassistedTaskProgressBar.Fraction = 0;
				
				unassistedTaskNameLabel.Text = "-";
				MainRecognizerWindow.ProcessItemsSensitive = true;
				
				if(parsingController.ParsingResult)
				{
					OkDialog.Show(MainRecognizerWindow.Window,
					              MessageType.Info,
					              "¡El proceso de reconocimiento tuvo éxito!");
					
					
					
					unassistedShowOutputBtn.Sensitive = true;
					
				}
				else
				{
					OkDialog.Show(MainRecognizerWindow.Window,
					              MessageType.Warning,
					              "¡El proceso de reconocimiento no tuvo éxito!");
				}
				unassistedControlHBB.Sensitive = true;
			});
		}
		
		[GLib.ConnectBefore]
		private void OnHandwritingAreaButtonPress(object sender,
		                                          ButtonPressEventArgs args)
		{
			if(args.Event.Button == 3)
			{
				blackboardMenu.Popup();
			}
		}
		
		private void OnBlackboardUndoItemActivate(object sender, EventArgs args)
		{
			handwritingArea.UndoLastStroke();
		}
		
		private void OnBlackboardClearItemActivate(object sender, EventArgs args)
		{
			ResponseType res = 
				ConfirmDialog.Show(this.MainRecognizerWindow.Window,
				                   "Se borrará toda la imagen, ¿quieres continuar?");
			
			if(res==ResponseType.Yes)
			{
				handwritingArea.Clear();
			}
		}
		
#endregion Event handlers
	}
}