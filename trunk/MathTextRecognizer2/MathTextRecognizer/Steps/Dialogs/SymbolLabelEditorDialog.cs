// SymbolLabelEditorDialog.cs created with MonoDevelop
// User: luis at 21:47 16/04/2008

using System;
using Gtk;
using Glade;

using MathTextCustomWidgets.Widgets.ImageArea;
using MathTextCustomWidgets.Widgets;

using MathTextLibrary.Symbol;

using MathTextRecognizer.Steps.Nodes;

namespace MathTextRecognizer.Steps.Dialogs
{
	
	/// <summary>
	/// This class implements a dialog used to edit a <c>FormulaNode</c>'s
	/// associated symbol.
	/// </summary>
	public class SymbolLabelEditorDialog
	{
		[WidgetAttribute]
		private Dialog symbolLabelEditorDialog;
		
		[WidgetAttribute]
		private Button closeButton;
		
		[WidgetAttribute]
		private Button okButton;
		
		[WidgetAttribute]
		private VBox symbolEditorPlaceholder;
		
		[WidgetAttribute]
		private Alignment imagePlaceholder;
		
		private SymbolLabelEditorWidget labelEditor;
		
		private ImageArea nodeImage;
		
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
			
			
			labelEditor = new SymbolLabelEditorWidget();
			if(node.Symbols.Count == 1)
				labelEditor.Label = node.Label;	
			else
			{
				// If we have various posibilities, we add radio buttons.
				RadioButton group = new RadioButton("group");
				foreach(MathSymbol symbol in node.Symbols)
				{					
					symbolEditorPlaceholder.Add(new RadioButton(group,
					                                            symbol.Text));
				}
				
				symbolEditorPlaceholder.Add(new RadioButton(group, "Otra"));
				labelEditor.Sensitive = false;
			}
			
			symbolEditorPlaceholder.Add(labelEditor);
			
			nodeImage = new ImageArea();
			nodeImage.Image = node.MathTextBitmap.Pixbuf;
			nodeImage.ImageMode = ImageAreaMode.Zoom;
			
			imagePlaceholder.Add(nodeImage);
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
		
		
	}
}
