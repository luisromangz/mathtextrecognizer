// SyntacticalRulesManagerDialog.cs created with MonoDevelop
// User: luis at 11:24 16/05/2008

using System;
using System.Collections.Generic;

using Gtk;
using Glade;
using MathTextLibrary.Analisys;
using MathTextCustomWidgets.Dialogs;

using MathTextRecognizer.Config;

namespace MathTextRecognizer.SyntacticalRulesManager
{
	/// <summary>
	/// This class implements a dialog that serve to manage the rules used
	/// in the syntactical analysis.
	/// </summary>
	public class SyntacticalRulesManagerDialog
	{
#region Glade Widgets
		[Widget]
		private Dialog syntacticalRulesManagerDialog = null;
		
		[Widget]
		private TreeView synRulesTree = null;
		
		[Widget]
		private Menu synRuleMenu = null;
		
		[Widget]
		private Button rmSynRuleBtn = null;
		
		[Widget]
		private Button synRuleEditBtn = null;
		
#endregion Glade widgets
		
#region Fields
		private ListStore synRulesModel;
		
		private TreeIter selectedRule;
#endregion Fields
		
#region Constructors	
		/// <summary>
		/// <see cref="SyntacticalRulesManagerDialog"/>'s constructor.
		/// </summary>
		/// <param name="parent">
		/// A <see cref="Window"/>
		/// </param>
		public SyntacticalRulesManagerDialog(Window parent)
		{
			XML gladeXml = new XML("mathtextrecognizer.glade",
			                       "syntacticalRulesManagerDialog");
			
			gladeXml.Autoconnect(this);
			
			gladeXml = new XML ("mathtextrecognizer.glade",
			                    "synRuleMenu");
			
			gladeXml.Autoconnect (this);
			
			syntacticalRulesManagerDialog.TransientFor = parent;
			
			// We remove the close button, because we aren't able to check the 
			// dialog for validation errors.
			syntacticalRulesManagerDialog.Deletable = false;
			
			
			InitializeWidgets();
			
			this.SyntacticalRules = 
				Config.RecognizerConfig.Instance.SyntacticalRules;
		}
		
		/// <value>
		/// Contains the rules managed by the dialog.
		/// </value>
		public List<SyntacticalRule> SyntacticalRules
		{
			get
			{
				List<SyntacticalRule> rules = new List<SyntacticalRule>();
				
				foreach (object [] values in synRulesModel) 
				{
					rules.Add((SyntacticalRule)values[2]);
				}
				
				return rules;
			}
			set
			{
				foreach (SyntacticalRule rule in value) 
				{
					AddRule(rule);
				}
				
				synRulesTree.Selection.UnselectAll();
				synRulesTree.ScrollToPoint(0,0);
			}
		}
		
#endregion Constructors
		
#region Properties
		
		/// <value>
		/// Contains the dialog's window.
		/// </value>
		public Window Window
		{
			get
			{
				return this.syntacticalRulesManagerDialog;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Shows the dialog and waits for its response.
		/// </summary>
		/// <returns>
		/// The <see cref="ResponseType"/> of the dialog.
		/// </returns>
		public ResponseType Show()
		{
			ResponseType res;
			do
			{
				// We have to wait until the response is useful.
				res = (ResponseType)(syntacticalRulesManagerDialog.Run());
			}while(res== ResponseType.None);
			
			return res;
		}
		
		/// <summary>
		/// Hides the dialog, and frees its resources.
		/// </summary>
		public void Destroy()
		{
			syntacticalRulesManagerDialog.Destroy();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ruleName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool ExistsRuleName(string ruleName)
		{
			foreach (object[] values in synRulesModel) 
			{
				if((values[2] as SyntacticalRule).Name == ruleName)
				{
					// We have found the name.
					return true;
				}
			}
			
			return false;
		}
		
#endregion Public methods
		
#region Non-public methods
		
		private void AddRule(SyntacticalRule rule)
		{
			string [] parts = rule.ToString().Split(':');
			
			TreeIter iter = synRulesModel.AppendValues(parts[0]+" :",
			                                           parts[1],
			                                           rule);
			
			synRulesTree.Selection.SelectIter(iter);
			
			
			synRulesTree.ScrollToCell(synRulesModel.GetPath(iter), 
			                          synRulesTree.Columns[0],
			                          true,0.5f,0);
		}
		
		/// <summary>
		/// Initialize the dialog's children widgets.
		/// </summary>
		private void InitializeWidgets()
		{
			
			// We create the tree's model, and asign it.
			synRulesModel = new ListStore(typeof(string),
			                              typeof(string),
			                              typeof(SyntacticalRule));
			
			synRulesTree.Model = synRulesModel;
			
			// We must handle the selection event of the treeview.
			synRulesTree.Selection.Changed += 
				new EventHandler(OnSynRulesTreeSelectionChanged);
			
			synRulesTree.ButtonPressEvent+= 
				new ButtonPressEventHandler(OnSynRulesTreeButtonPressEvent);
			
			
			// We add the columsn of the treeview.
			
			CellRendererText cellRenderer = new CellRendererText();
			cellRenderer.Yalign = 0;
			synRulesTree.AppendColumn("Regla", 
			                          cellRenderer,
			                          "markup" ,0);
			synRulesTree.Columns[0].Sizing = TreeViewColumnSizing.Autosize;
			
			synRulesTree.AppendColumn("Expresión", 
			                          new CellRendererText(),
			                          "markup" ,1);
			synRulesTree.Columns[1].Sizing = TreeViewColumnSizing.Autosize;
		}
		
		/// <summary>
		/// Adds a new rule to the rule set.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnAddSynRuleBtnClicked(object sender, EventArgs args)
		{
			SyntacticalRuleEditorDialog dialog = 
				new SyntacticalRuleEditorDialog(this);
			
			ResponseType res = dialog.Show();
			if(res == ResponseType.Ok)
			{
				AddRule(dialog.Rule);
			}
			
			dialog.Destroy();
		}	
		
		/// <summary>
		/// Launches the rule editor dialog for the selected rule.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnSynRuleEditBtnClicked(object sender, EventArgs args)
		{
			SyntacticalRuleEditorDialog dialog = 
				new SyntacticalRuleEditorDialog(this);
			
			TreeIter iter;
			synRulesTree.Selection.GetSelected(out iter);
			
			dialog.Rule = synRulesModel.GetValue(iter, 2) as  SyntacticalRule;
			
			ResponseType res = dialog.Show();
			if(res == ResponseType.Ok)
			{			
				SyntacticalRule rule = dialog.Rule;
				string [] parts = rule.ToString().Split(':');
				synRulesModel.SetValue(iter, 0,parts[0] +" :");
				synRulesModel.SetValue(iter, 1,parts[1]);
				synRulesModel.SetValue(iter, 2,rule);
			}
			
			dialog.Destroy();
		}
		
		
		/// <summary>
		/// Makes the selected rule the default one.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnDefaultSynRuleItemActivate(object sender, EventArgs args)
		{
			TreeIter first;
			synRulesModel.GetIterFirst(out first);
			synRulesModel.MoveBefore(selectedRule, first);
			
			synRulesTree.Selection.SelectIter(selectedRule);
			synRulesTree.ScrollToPoint(0,0);
		}
		
		/// <summary>
		/// Removes the selected rule from the list.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnRmSynRuleBtnClicked(object sender, EventArgs args)
		{
			TreeIter selected;
			synRulesTree.Selection.GetSelected(out selected);
			
			// We retrieve the selected rule.
			SyntacticalRule rule = 
				synRulesModel.GetValue(selected, 2) as SyntacticalRule;
			
			ResponseType res = 
				ConfirmDialog.Show(syntacticalRulesManagerDialog,
				                   "¿Realemente quieres eliminar la regla «{0}»?",
				                   rule.Name);
			
			if(res == ResponseType.No)
			{
				// The user doesn't want the removal of the rule, so we abort.
				return;
			}
			
			// The user accepted, so there we go!
			synRulesModel.Remove(ref selected);
		}
		
		/// <summary>
		/// Handles the clicking on the dialog's close button.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		[GLib.ConnectBefore]
		private void OnSynRulesCloseBtnClicked(object sender, EventArgs args)
		{
			syntacticalRulesManagerDialog.Respond(ResponseType.Ok);
			syntacticalRulesManagerDialog.Hide();
		}
		
		/// <summary>
		/// Saves the current rule set as the default one.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnSynMakeDefaultBtnClicked(object sender, EventArgs args)
		{
			ResponseType res =
				ConfirmDialog.Show(syntacticalRulesManagerDialog,
				                   "Esto cambiará la configuración de la aplicación, ¿desea continuar?");
			
			if(res == ResponseType.Yes)
			{
				RecognizerConfig.Instance.SyntacticalRules = SyntacticalRules;
				RecognizerConfig.Instance.Save();
			}
		}
		
		/// <summary>
		/// Shows a message dialog with information about the dialog.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		[GLib.ConnectBefore]
		private void OnSynRulesInfoBtnClicked(object sender, EventArgs args)
		{
			// TODO: Add a useful info message for the SyntacticalRulesManagerDialog.
			OkDialog.Show(this.syntacticalRulesManagerDialog, 
			              MessageType.Info,
			              "Meeeeeeeeh!");
			
			syntacticalRulesManagerDialog.Respond(ResponseType.None);
		}
		
		/// <summary>
		/// Shows the contextual menu for the rules treeview.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="ButtonPressEventArgs"/>
		/// </param>
		[GLib.ConnectBefore]
		private void OnSynRulesTreeButtonPressEvent(object sender, 
		                                            ButtonPressEventArgs args)
		{
			
			// Have the user pressed the right mouse button?
			if(args.Event.Button == 3)
			{
				// Yeah, so let's see if there is something under the mouse pointer.
				TreePath path;
				synRulesTree.GetPathAtPos((int)args.Event.X,
				                          (int)args.Event.Y,
				                          out path);
				
				// There seems to exist a rule there,
				// it is the default one?
				TreeIter iter;
				if(synRulesModel.GetIter(out iter, path))
				{
					selectedRule =  iter;
					
					TreeIter first;
					synRulesModel.GetIterFirst(out first);
					if(synRulesModel.GetPath(iter).ToString() != 
					   synRulesModel.GetPath(first).ToString())
					{
						// We show the contextual menu.
						synRuleMenu.Popup();
					}
					
					
				}
			}
		}
		
		/// <summary>
		/// Handles the selection of a rule in the tree view.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnSynRulesTreeSelectionChanged(object sender,
		                                            EventArgs args)
		{
			// The remove button must be sensitive just if we have a rule
			// selected.
			rmSynRuleBtn.Sensitive = 
				synRulesTree.Selection.CountSelectedRows() > 0;
			
			synRuleEditBtn.Sensitive= 
				synRulesTree.Selection.CountSelectedRows() > 0;
		}
		
		
		
			
#endregion Non-public methods
		
			
	}
}


