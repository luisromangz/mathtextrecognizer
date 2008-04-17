// SymbolLabelInfoConfigDialog.cs created with MonoDevelop
// User: luis at 14:15 17/04/2008

using System;
using System.Collections.Generic;

using Gtk;

using Glade;

using MathTextLibrary.Config;

using MathTextCustomWidgets.Dialogs;

namespace MathTextCustomWidgets.Dialogs.SymbolLabel
{
	
	/// <summary>
	/// This class implements a dialog to choose the symbol/label correspondence.
	/// </summary>
	public class SymbolLabelDialog
	{
		[WidgetAttribute]
		private Dialog symbolLabelDialog;
		
		[WidgetAttribute]
		private Button editBtn;
		
		[WidgetAttribute]
		private Button removeBtn;
		
		[WidgetAttribute]
		private TreeView symbolLabelsTV;
		
		private ListStore symbolLabelsModel;
		
		bool changes;
		
		/// <summary>
		/// <c>SymbolLabelInfoConfigDialog</c>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// The dialog's parent window.
		/// </param>
		public SymbolLabelDialog(Window parent)
		{
			XML gxml = new XML(null, "gui.glade","symbolLabelDialog",null);
			
			gxml.Autoconnect(this);
			
			symbolLabelDialog.Modal = true;
			symbolLabelDialog.Resizable = false;
			symbolLabelDialog.TransientFor = parent;
			
			CellRendererText cellRenderer = new CellRendererText();			
			
			cellRenderer.Xalign = 0.5f;
			symbolLabelsTV.AppendColumn("Símbolo", cellRenderer,"text",0);
			symbolLabelsTV.AppendColumn("Etiqueta", new CellRendererText(),"text",1);
			
			symbolLabelsModel = new ListStore(typeof(string), 
			                                  typeof(string));
			
			symbolLabelsTV.Model = symbolLabelsModel;
			
			symbolLabelsTV.Selection.Changed += OnSymbolLabelsTVSelectionChanged;
			
			foreach (SymbolLabelInfo info in LibraryConfig.Instance.Symbols)
			{				
				symbolLabelsModel.AppendValues(info.Symbol, info.Label);
			}
			
			changes = false;
		}
		
		/// <summary>
		/// Shows the dialog.
		/// </summary>
		public void Show()
		{
			symbolLabelDialog.Run();
		}
		
		/// <summary>
		/// Destroys the dialog's resources.
		/// </summary>
		public void Destroy()
		{
			symbolLabelDialog.Destroy();
		}
		
#region Private methods
		
		/// <summary>
		/// Edits the currently selected row of the treeview.
		/// </summary>
		/// <param name="iter">
		/// The iter to be modified.
		/// </param>
		private bool EditIter(TreeIter selected)
		{
			SymbolLabelListDialog dialog = 
				new SymbolLabelListDialog(this.symbolLabelDialog);
			
			dialog.Symbol = (string)(symbolLabelsModel.GetValue(selected,0));
			dialog.Label = (string)(symbolLabelsModel.GetValue(selected,1));
			 			
			ResponseType res;
			while((res = dialog.Show()) == ResponseType.Ok
			      && (String.IsNullOrEmpty(dialog.Label)
			          || String.IsNullOrEmpty(dialog.Symbol)))
			{
				OkDialog.Show(this.symbolLabelDialog,
				              MessageType.Warning,
				              "Debe rellenar tanto el símbolo como la etiqueta");
			}
			
			dialog.Destroy();
			
			if(res == ResponseType.Ok)
			{
				symbolLabelsModel.SetValue(selected,0,dialog.Symbol);
				symbolLabelsModel.SetValue(selected,1,dialog.Label);
				
				changes = true;
				return true;
			}
			else
			{
				return false;
			}
			
			
			
		}
		
		/// <summary>
		/// Handles the add button clicked event.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnAddBtnClicked(object sender, EventArgs a)
		{
			TreeIter iter = symbolLabelsModel.Append();		
			
			
			// We select and scroll to the new row.
			symbolLabelsTV.Selection.SelectIter(iter);
			symbolLabelsTV.ScrollToCell(symbolLabelsTV.Selection.GetSelectedRows()[0],
			                            symbolLabelsTV.Columns[0],
			                            true, 1.0f, 0.0f);
			
			TreeIter selected; 
			symbolLabelsTV.Selection.GetSelected(out selected);
			if(!EditIter(selected))
			{
				// If we pressed the cancel button, we remove the new iter.
				symbolLabelsModel.Remove(ref iter);
			}
			
			changes = true;
		}
		
		/// <summary>
		/// Handles the click on the remove button.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnRemoveBtnClicked(object sender, EventArgs a)
		{
			TreeIter iter;
			symbolLabelsTV.Selection.GetSelected(out iter);
			symbolLabelsModel.Remove(ref iter);
			
			changes = true;
		}
		
		/// <summary>
		/// Handles the mouse click on the edit button.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnEditBtnClicked(object sender, EventArgs a)
		{
			TreeIter selected; 
			symbolLabelsTV.Selection.GetSelected(out selected);
			EditIter(selected);
		}
		
		/// <summary>
		/// Handles the close button's clicked event.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="EventArgs"/>
		/// </param>
		[GLib.ConnectBefore]
		private void OnCloseBtnClicked(object sender, EventArgs a)
		{
			if(changes)
			{
				ResponseType res=  ConfirmDialog.Show(symbolLabelDialog,
				                                      "¿Quieres guardar los cambios?");
				
				if (res == ResponseType.Yes)
				{
				
					List<SymbolLabelInfo> symbols = new List<SymbolLabelInfo>();
					
					foreach (object[] row in symbolLabelsModel)
					{
						
						SymbolLabelInfo info = new SymbolLabelInfo();
						info.Symbol = (string)(row[0]);
						info.Label = (string)(row[1]);
						
						symbols.Add(info);
					}
					
					LibraryConfig.Instance.Symbols = symbols;
					LibraryConfig.Instance.Save();
				}
			}
		
		}
		
		/// <summary>
		/// Handles the treeview's row activated event.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="a">
		/// A <see cref="RowActivatedArgs"/>
		/// </param>
		private void OnSymbolLabelTVRowActivated(object sender, 
		                                          RowActivatedArgs a)
		{
			TreeIter selected; 
			symbolLabelsTV.Selection.GetSelected(out selected);
			EditIter(selected);
		}
		
		private void OnSymbolLabelsTVSelectionChanged(object sender,
		                                              EventArgs a)
		{
			bool selection = symbolLabelsTV.Selection.CountSelectedRows() > 0;
			
			editBtn.Sensitive = selection;
			removeBtn.Sensitive = selection;
		}
		
#endregion Private methods
			
	}
}
