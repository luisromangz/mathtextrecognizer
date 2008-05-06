// SymbolLabelEditorDialog.cs created with MonoDevelop
// User: luis at 21:47 16/04/2008

using System;
using Gtk;
using Glade;

using MathTextCustomWidgets.Widgets.ImageArea;
using MathTextCustomWidgets.Widgets;

using MathTextLibrary.Symbol;

using MathTextRecognizer.Controllers.Nodes;

namespace MathTextRecognizer.Stages.Dialogs
{
	
	/// <summary>
	/// This class implements a dialog used to edit a <c>FormulaNode</c>'s
	/// associated symbol.
	/// </summary>
	public class SymbolLabelEditorDialog
	{
		
#region Glade widgets
		
		[WidgetAttribute]
		private Dialog symbolLabelEditorDialog = null;
		
		[WidgetAttribute]
		private Button okButton = null;
		
		[WidgetAttribute]
		private VBox symbolEditorPlaceholder = null;
		
		[WidgetAttribute]
		private Alignment imagePlaceholder = null;
		
		[WidgetAttribute]
		private Label imageNameLabel = null;
		
#endregion Glade widgets
		
		private SymbolLabelEditorWidget labelEditor;
		
		private ImageArea nodeImage;
		
		private RadioButton otherLabelRB;
		
		/// <summary>
		/// <c>SymbolLabelEditorDialog</c>'s constructor method.
		/// </summary>
		/// <param name="parent">
		/// The new dialog's parent window.
		/// </param>
		/// <param name="node">
		/// The node which label we want to edit.
		/// </param>
		public SymbolLabelEditorDialog(Window parent, SegmentedNode node)
		{
			XML gxml = new XML("mathtextrecognizer.glade",
			                   "symbolLabelEditorDialog");
			
			gxml.Autoconnect(this);
			
			symbolLabelEditorDialog.TransientFor = parent;
			
			symbolLabelEditorDialog.AddActionWidget(okButton,
			                                        ResponseType.Ok);
			
			InitializeWidgets(node);
			
			
			symbolLabelEditorDialog.ShowAll();
		}
		
		/// <value>
		/// Contains the new label.
		/// </value>
		public string Label
		{
			get
			{
				return labelEditor.Label;
			}
		}
		
		/// <summary>
		/// Shows the dialog.
		/// </summary>
		/// <returns>
		/// The dialog's response.
		/// </returns>
		public ResponseType Show()
		{
			return (ResponseType)(symbolLabelEditorDialog.Run());
		}
		
		/// <summary>
		/// Destroys the dialog's resources.
		/// </summary>
		public void Destroy()
		{
			symbolLabelEditorDialog.Destroy();
		}
		
#region Private methods
	
		/// <summary>
		/// Initializes the dialogs' widgets.
		/// </summary>
		/// <param name="node">
		/// A <see cref="SegmentedNode"/>
		/// </param>
		private void InitializeWidgets(SegmentedNode node)
		{			
			imageNameLabel.Text = String.Format(imageNameLabel.Text, node.Name);
			
			labelEditor = new SymbolLabelEditorWidget();
			if(node.Symbols.Count ==0)
			{
				labelEditor.Label = node.Label;
			}
			else
			{
				// If we have various posibilities, we add radio buttons.
				RadioButton group = new RadioButton("group");
				foreach(MathSymbol symbol in node.Symbols)
				{		
					RadioButton rb = new RadioButton(group, symbol.Text);
					Alignment rbAlign = new Alignment(0,0.5f,0,0);
					rbAlign.Add(rb);
					symbolEditorPlaceholder.Add(rbAlign);
					
					rb.Clicked += new EventHandler(OnLabelOptionClicked);
					
					
				}
				
				Alignment rbOtherAlign = new Alignment(0,0.5f,0,0);
				otherLabelRB = new RadioButton(group, "Otra:");
				otherLabelRB.Clicked += new EventHandler(OnLabelOptionClicked);
				
				rbOtherAlign.Add(otherLabelRB);
				symbolEditorPlaceholder.Add(rbOtherAlign);
				labelEditor.Sensitive = false;
			}
			
			symbolEditorPlaceholder.Add(labelEditor);
			
			nodeImage = new ImageArea();
			nodeImage.Image = node.MathTextBitmap.Pixbuf;
			nodeImage.ImageMode = ImageAreaMode.Zoom;
			
			imagePlaceholder.Add(nodeImage);
		}
			
		
		/// <summary>
		/// Handles the click in one of the options.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnLabelOptionClicked(object sender, EventArgs args)
		{
			RadioButton selected = (RadioButton)sender;
			
			if(selected.Label.Equals(otherLabelRB.Label))
			{
				// If it's the "other" radio button, we activate the manual 
				// editor of the label.
				labelEditor.Sensitive=true;
				labelEditor.IsFocus = true;
			}
			else
			{
				labelEditor.Label = selected.Label;
				labelEditor.Sensitive=false;
			
			}
				
		}
		
#endregion Private methods
		
	}
}
