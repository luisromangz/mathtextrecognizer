// FormulaMatchingWidget.cs created with MonoDevelop
// User: luis at 12:47 09/05/2008

using System;
using System.Collections.Generic;

using Gtk;
using Glade;

using MathTextCustomWidgets.Dialogs;
using MathTextLibrary.Analisys;
using MathTextLibrary.Controllers;
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
		private HPaned parsingStageBaseWidget = null;
		
		[Widget]
		private ScrolledWindow syntacticalTreePlaceholder = null;
		
		[Widget]
		private Notebook parsingButtonsNB = null;
		
		[Widget]
		private Button parsingProcessBtn = null;
		
		[Widget]
		private Button parsingShowOutputBtn = null;
	
#endregion Glade widgets
	
		
#region Fields
		private ParsingController controller;
		
		private NodeView syntacticalCoverTree;
		private NodeStore syntacticalCoverModel;
		
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
			
			controller = new ParsingController();
			controller.MessageLogSent += 
				new MessageLogSentHandler(OnControllerMessageLogSent);
			
			controller.ProcessFinished += OnControllerProcessFinishedHandler;
			
			InitializeWidgets();
			
			this.ShowAll();
		}
		
		/// <summary>
		/// <c>FormulaMatchingStageWidget</c>'s static fields initializer.
		/// </summary>
		static ParsingStageWidget()
		{
			widgetLabel = "Reconstrucción de la fórmula";
		}
		
#region Public methods
			/// <summary>
		/// Set the widget to its initial state.
		/// </summary>
		public override void ResetState ()
		{
			parsingButtonsNB.Page = 0;
			
			parsingProcessBtn.Sensitive = true;
			
			parsingShowOutputBtn.Sensitive = false;
			
			syntacticalCoverModel.Clear();
		}
		
			
		public override void Abort ()
		{
			controller.TryAbort();
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
			
			syntacticalCoverTree.AppendColumn("Elemento que reconoce",
			                                  new CellRendererText(),
			                                  "markup" ,0);
			
			syntacticalCoverTree.Columns[0].Sizing = 
				TreeViewColumnSizing.Autosize;
			
			syntacticalTreePlaceholder.Add(syntacticalCoverTree);
		}
		
		protected override void NextStep (ControllerStepMode mode)
		{
			controller.Next(mode);
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
			Application.Invoke(OnControllerProcessFinishedHandlerInThread);
		}
		
		private void OnControllerProcessFinishedHandlerInThread(object sender, 
		                                                        EventArgs args)
		{
			if(controller.ParsingResult)
			{
				OkDialog.Show(this.MainRecognizerWindow.Window,
				              MessageType.Info,
				              "¡El proceso de análisis sintáctico fue un éxito!");		
				
				parsingButtonsNB.Page = 0;
				
				parsingShowOutputBtn.Sensitive = true;
				parsingProcessBtn.Sensitive = false;
			}
			else
			{
				OkDialog.Show(this.MainRecognizerWindow.Window,
				              MessageType.Warning,
				              "El proceso de análisis sintáctico no tuvo éxito.");		
			}
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
			
			SyntacticalCoverNode node = 
				new SyntacticalCoverNode(rules[0]);
			
			syntacticalCoverModel.AddNode(node);
			
			parsingButtonsNB.Page = 1;
			
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
				new Output.OutputDialog(MainRecognizerWindow.Window,output);
			
			dialog.Show();
			dialog.Destroy();
		}
	
#endregion Non-public methods
	}
}
