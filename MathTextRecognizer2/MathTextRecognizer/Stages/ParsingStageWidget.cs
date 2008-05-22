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
		}
	
#endregion Non-public methods
	}
}
