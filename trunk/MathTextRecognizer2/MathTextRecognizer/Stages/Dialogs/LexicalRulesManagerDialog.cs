// LexicalRulesManagerDialog.cs created with MonoDevelop
// User: luis at 19:10 28/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

using Gtk;
using Glade;

using MathTextCustomWidgets.Dialogs;
using MathTextLibrary.Analisys.Lexical;

namespace MathTextRecognizer
{
	
	/// <summary>
	/// This class implements the dialog which provides access to the set of
	/// lexical rules.
	/// </summary>
	public class LexicalRulesManagerDialog
	{
#region Glade widgets
		[WidgetAttribute]
		private Dialog lexicalRulesManagerDialog = null;
		
		[WidgetAttribute]
		private TreeView rulesTV = null;
		
		[WidgetAttribute]
		private Button upBtn = null;
		
		[WidgetAttribute]
		private Button downBtn = null;
		
		[WidgetAttribute]
		private Button editBtn = null;
		
		[WidgetAttribute]
		private Button removeBtn = null;
		
		
		
#endregion Glade widgets
		
#region Other fields
		
		private ListStore rulesStore;
		
#endregion Other fields
		
		/// <summary>
		/// <c>LexicalRulesManagerDialog</c>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// The dialog's parente window.
		/// </param>
		public LexicalRulesManagerDialog(Window parent)
		{
			XML gxml = new XML(null, 
			                   "mathtextrecognizer.glade",
			                   "lexicalRulesManagerDialog",
			                   null);
			
			gxml.Autoconnect(this);
			
			lexicalRulesManagerDialog.TransientFor = parent;
			
			InitializeWidgets();
			
		}
		
#region Properties
		/// <value>
		/// Contains the rules used in the lexical analisys.
		/// </value>
		public List<LexicalRule> LexicalRules
		{			
			get
			{
				// We extract the rules from the treeview's model.
				List<LexicalRule> rules = new List<LexicalRule>();
				foreach (object[] values in rulesStore) 
				{
					rules.Add((LexicalRule)(values[2]));
				}
				
				return rules;
			}
			
			set
			{
				// Inserts the rules in the model.
				foreach (LexicalRule rule in value) 
				{
					AddRule(rule);
				}
			}
		}
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Shows the dialog and waits for it to respond.
		/// </summary>
		/// <returns>
		/// The dialog's response type.
		/// </returns>
		public ResponseType Show()
		{
			return (ResponseType)(lexicalRulesManagerDialog.Run());
		}
		
		/// <summary>
		/// Destroys the dialog's resources  and hides it.
		/// </summary>
		public void Destroy()
		{
			lexicalRulesManagerDialog.Destroy();
		}
		
#endregion Public methods
		
#region Non-public methods
		
		/// <summary>
		/// Adds a new rule to the list.
		/// </summary>
		/// <param name="rule">
		/// The rule which will be added.
		/// </param>
		private void AddRule(LexicalRule rule)
		{
			string expression =  
				String.Join(" | ", rule.LexicalExpressions.ToArray());
			
			TreeIter newIter = 	rulesStore.AppendValues(rule.RuleName,
			                                            expression,
			                                            rule);
			
			// We select the new row and scroll to it.
			rulesTV.Selection.SelectIter(newIter);
			rulesTV.ScrollToCell(rulesTV.Selection.GetSelectedRows()[0],
			                     rulesTV.Columns[0],
			                     true, 1.0f, 0);
		}
		
		/// <summary>
		/// Initializes the dialog's widgets;
		/// </summary>
		private void InitializeWidgets()
		{
			// We create a model for the tree view.
			rulesStore = new ListStore(typeof(string),
			                           typeof(string),
			                           typeof(LexicalRule));
			
			rulesTV.Model = rulesStore;
			
			// We add the columns to the view.
			rulesTV.AppendColumn("Regla", new CellRendererText(),"text", 0);
			rulesTV.AppendColumn("Expresión", new CellRendererText(),"text", 1);			
			
			rulesTV.Selection.Changed += 
				new EventHandler(OnRulesTVSelectionChanged);
		}
		
		/// <summary>
		/// Opens the new rule dialog, and adds the rule if that dialog response
		/// is ok.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnAddBtnClicked(object sender, EventArgs args)
		{
			
			LexicalRuleEditorDialog dialog = 
				new LexicalRuleEditorDialog(lexicalRulesManagerDialog);
			
			ResponseType res = dialog.Show();
			
			if(res == ResponseType.Ok)
			{
				AddRule(dialog.Rule);				
			}
			
			dialog.Destroy();
			
		}

		/// <summary>
		/// Removes the selected rule from the rule-list, if the user
		/// gives its confirmation.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnRemoveBtnClicked(object sender, EventArgs args)
		{			
			TreeIter selected;
			rulesTV.Selection.GetSelected(out selected);
			
			// We retrieve the selected row's rule name.
			string ruleName = (string)(rulesStore.GetValue(selected,0));
			
			ResponseType res;
			res = ConfirmDialog.Show(lexicalRulesManagerDialog,
			                         "¿Realmente quieres eliminar la regla «{0}»?",
			                         ruleName);
			
			if(res == ResponseType.Yes)
			{
				rulesStore.Remove(ref selected);
			}
		}	
		
		/// <summary>
		/// Moves the selected rule one positon higher.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnUpBtnClicked(object sender, EventArgs args)
		{
			TreePath selectedPath =  rulesTV.Selection.GetSelectedRows()[0];
			TreeIter selected;
			rulesTV.Selection.GetSelected(out selected);
			
			TreePath previousPath = rulesTV.Selection.GetSelectedRows()[0];
			previousPath.Prev();
			TreeIter previous;
			rulesStore.GetIter(out previous, previousPath);
			
			rulesStore.MoveBefore(selected, previous);
			
			rulesTV.Selection.UnselectAll();
			rulesTV.Selection.SelectPath(previousPath);
			rulesTV.ScrollToCell(previousPath, rulesTV.Columns[0], true, 0, 0);
		}
		
		/// <summary>
		/// Moves the selected rule one postion lower.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnDownBtnClicked(object sender, EventArgs args)
		{
			TreePath selectedPath =  rulesTV.Selection.GetSelectedRows()[0];
			TreeIter selected;
			rulesTV.Selection.GetSelected(out selected);
			
			TreePath nextPath = rulesTV.Selection.GetSelectedRows()[0];
			nextPath.Next();
			TreeIter next;
			rulesStore.GetIter(out next, nextPath);
			
			rulesStore.MoveBefore(selected, next);
			rulesTV.Selection.UnselectAll();
			rulesTV.Selection.SelectPath(nextPath);
			rulesTV.ScrollToCell(nextPath, rulesTV.Columns[0], true, 1.0f, 0);
		}
		
		/// <summary>
		/// Opens the lexical rule editor dialog for the selected rule.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnEditBtnClicked(object sender, EventArgs args)
		{
			// TODO Edit rule;
		}
		
		/// <summary>
		/// If the user agrees, saves the rule-list.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnSaveBtnClicked(object sender, EventArgs args)
		{
			
		}
		
		/// <summary>
		/// Activates or deactivates the buttons based on the selected rows.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		public void OnRulesTVSelectionChanged(object sender, EventArgs args)
		{
			bool rowSelected = rulesTV.Selection.CountSelectedRows() > 0;
			
			int idx = -1;
			if(rowSelected)
			{
				idx = rulesTV.Selection.GetSelectedRows()[0].Indices[0];
			}
			
	
			upBtn.Sensitive = rowSelected && idx > 0;
			downBtn.Sensitive = 
				rowSelected && idx < rulesStore.IterNChildren() - 1;
			
			editBtn.Sensitive = rowSelected;
			removeBtn.Sensitive = rowSelected;
		}
		
#endregion Non-public methods
	}
}
