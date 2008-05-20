// SyntacticalExpressionWidget.cs created with MonoDevelop
// User: luis at 18:21 16/05/2008

using System;
using System.Collections.Generic;

using Gtk;
using Glade;

using MathTextLibrary.Analisys;
using MathTextCustomWidgets.Dialogs;

namespace MathTextRecognizer.SyntacticalRulesManager
{
	
	/// <summary>
	/// This class implements a widget to edit a lexical expression.
	/// </summary>
	public class SyntacticalExpressionWidget : Alignment, IExpressionItemContainer
	{
#region Glade widgets
		
		[Widget]
		private Alignment syntacticalExpressionWidgetBase = null;
				
		[Widget]
		private Label expEdOrLbl =null;
		
		[Widget]
		private Entry expFormatEntry = null;
		
		[Widget]
		private Button expUpBtn =null;
		
		[Widget]
		private Button expDownBtn = null;
		
		[Widget]
		private HBox  expItemsBox =null;
		
		[Widget]
		private ScrolledWindow expItemsScroller = null;
		
		[Widget]
		private HSeparator expSeparator = null;
		
		[Widget]
		private Label expLabel = null;
		
#endregion Glade widgets
		
#region Fields
		
		private SyntacticalRuleEditorDialog dialog;
		
		private AddSubItemMenu addSubItemMenu;
		
		
		
#endregion Fields
		


#region Constructors
		
		/// <summary>
		/// <see cref="SyntacticalExpressionWidget"/>'s constructor.
		/// </summary>
		public SyntacticalExpressionWidget(SyntacticalRuleEditorDialog dialog) 
			: base(0, 0, 1, 1)			
		{
			Glade.XML gladeXml = new XML("mathtextrecognizer.glade",			     
			                             "syntacticalExpressionWidgetBase");
			
			gladeXml.Autoconnect(this);
			
			this.dialog =  dialog;			
			
			this.Add(syntacticalExpressionWidgetBase);
			
			InitializeWidgets();
			
			this.CanFocus = true;
			
			this.ShowAll();
		}
		
#endregion Constructors
	
#region Properties
		
		/// <value>
		/// Contains the expression's number of items.
		/// </value>
		public int ItemCount 
		{
			get 
			{
				return expItemsBox.Children.Length;
			}
		}
		
		/// <summary>
		/// Contains the <see cref="BoxChild"/> associated to a 
		/// given widget.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public new Gtk.Box.BoxChild this[Widget w]
		{
			get
			{
				return expItemsBox[w] as Gtk.Box.BoxChild;
			}
		}
		
		/// <value>
		/// Contains the container's window.
		/// </value>
		public Window Window
		{
			get
			{
				return dialog.Window;
			}
		}
		
		public SyntacticalExpression Expression
		{
			get
			{
				SyntacticalExpression expression = new SyntacticalExpression();
				expression.FormatString = expFormatEntry.Text.Trim();
				
				foreach (ExpressionItemWidget childWidget in 
				         expItemsBox.Children) 
				{
					expression.Items.Add(childWidget.ExpressionItem);
				}
				
				return expression;
				
			}
			set
			{
				expFormatEntry.Text = value.FormatString;
				
				foreach (ExpressionItem item in value.Items) 
				{
					this.AddItem(ExpressionItemWidget.CreateWidget(item, this));
				}
			}
		}
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Adds an item to the items container.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void AddItem (ExpressionItemWidget widget)
		{
			expItemsBox.Add(widget);
			
			foreach (ExpressionItemWidget w in expItemsBox.Children) 
			{
				w.CheckPosition();
			}
			
			
			widget.HeightRequestChanged +=
				new EventHandler(OnChildWidgetHeightRequestChanged);
			
			// We expand the scroller is it's necessary to show the whole widget.
			if(widget.HeightRequest + 20 > expItemsScroller.HeightRequest)
				expItemsScroller.HeightRequest = widget.HeightRequest + 20;
			
			expItemsScroller.Hadjustment.Value =
				expItemsScroller.Hadjustment.Upper;
		}
		
		/// <summary>
		/// Removes a widget from the container.
		/// </summary>
		/// <param name="widget">
		/// The <see cref="ExpressionItemWidget"/> that will be removed
		/// </param>
		public void RemoveItem(ExpressionItemWidget widget)
		{
			expItemsBox.Remove(widget);
				
			
			if(expItemsBox.Children.Length ==0)
			{
				expItemsScroller.HeightRequest = 85;
			}
			else
			{
				int maxRequest = -1;
				
				foreach (ExpressionItemWidget childWidget in expItemsBox.Children) 
				{
					if (maxRequest < childWidget.HeightRequest)
						maxRequest = childWidget.HeightRequest;
					
					childWidget.CheckPosition();
				}
			
				
				expItemsScroller.HeightRequest = maxRequest + 20;
			}
		}
		
		/// <summary>
		/// Moves the item towards the expression's end.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void MoveItemFordwards(ExpressionItemWidget widget)
		{
			int position = (expItemsBox[widget] as Gtk.Box.BoxChild).Position;
			expItemsBox.ReorderChild(widget, position+1);
			widget.CheckPosition();
			
			(expItemsBox.Children[position] as ExpressionItemWidget).CheckPosition();
		}
		
		/// <summary>
		/// Moves the widget towards the beginning of the expression.
		/// </summary>
		/// <param name="widget">
		/// A <see cref="ExpressionItemWidget"/>
		/// </param>
		public void MoveItemBackwards(ExpressionItemWidget widget)
		{
			int position = (expItemsBox[widget] as Gtk.Box.BoxChild).Position;
			expItemsBox.ReorderChild(widget, position-1);
			widget.CheckPosition();
			
			(expItemsBox.Children[position] as ExpressionItemWidget).CheckPosition();
		}
		
		/// <summary>
		/// Checks the expression's position and acts accordingly.
		/// </summary>
		public void CheckPosition()
		{
			int position =
				(this.dialog.ExpressionsBox[this] as Gtk.Box.BoxChild).Position;
			
			expEdOrLbl.Text = position>0?"|": " ";
			
			expSeparator.Visible = position < dialog.ExpressionsBox.Children.Length -1;
			expDownBtn.Sensitive = position < dialog.ExpressionsBox.Children.Length -1;
			
			expUpBtn.Sensitive =  position > 0;
			
			expLabel.Text =  String.Format("Expresión {0}", position + 1);
		}
		
		/// <summary>
		/// Checks the errors in the expression.
		/// </summary>
		/// <returns>
		/// A <see cref="List`1"/>
		/// </returns>
		public List<string> CheckErrors()
		{
			List<string> errors = new List<string>();
			
			int position = 
				((Gtk.Box.BoxChild)(this.dialog.ExpressionsBox[this])).Position +1;
			
			if(expItemsBox.Children.Length == 0)
			{
				errors.Add( String.Format("· La expresión {0} no contiene elementos.",
				                          position));
			}
			else
			{
				if(String.IsNullOrEmpty(expFormatEntry.Text.Trim()))
				{
					errors.Add("\t· La cadena de formato de la expresión está vacia.");
				}
				else
				{
					try
					{
						List<string> testList = new List<string>();
						
						for(int i =0; i< expItemsBox.Children.Length; i++)
						{
							testList.Add("test");
						}
						
						// We are going to test the format string.
						String.Format(expFormatEntry.Text.Trim(),
						              testList.ToArray());
					}
					catch(Exception)
					{
						errors.Add("\t· La cadena de formato de la expresión no es válida.");
					}
				}
				
				
				
				foreach (ExpressionItemWidget itemWidget in expItemsBox.Children) 
				{
					List<string> itemErrors = itemWidget.CheckErrors();
					
					foreach (string itemError in itemErrors) 
					{
						errors.Add("\t" + itemError);
					}
					
				}
				
				if(errors.Count > 0)
				{
					errors.Insert(0,String.Format("· La expresión {0} tiene los siguientes errores:",
					                              position));
				
				}
				
				
			}
			
			return errors;
		}
		
#endregion Public methods
		
#region Non-public methods
		
		/// <summary>
		/// Intilialize the widget's children widgets.
		/// </summary>
		private void InitializeWidgets()
		{
			addSubItemMenu = new AddSubItemMenu(this);
			
			expItemsScroller.Hadjustment.ValueChanged+= 
				delegate(object sender, EventArgs args)
			{
				expItemsScroller.QueueDraw();
			};
		}
		
		/// <summary>
		/// Shows a menu used to select the kind of item that will be added to 
		/// the expressión.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnExpAddItemBtnClicked(object sender, EventArgs args)
		{
			addSubItemMenu.Popup();
		}
		
		/// <summary>
		/// Removes the expression from the rule.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnExpRemoveBtnClicked(object sender, EventArgs args)
		{
			ResponseType res =  ResponseType.Yes;
				
			if(expItemsBox.Children.Length >0 || 
			   !String.IsNullOrEmpty(expFormatEntry.Text.Trim()))
			{
				res=ConfirmDialog.Show(dialog.Window,
				                   "¿Realmente quieres eliminar la expresión?");			
			}
				
			if(res == ResponseType.No)
				return;
			
			dialog.RemoveExpression(this);
		}
			
	
		/// <summary>
		/// Changes the height of the container based on the height of its
		/// children, when a height request is detected.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnChildWidgetHeightRequestChanged(object sender,
		                                               EventArgs args)
		{
			if(expItemsBox.Children.Length ==0)
			{
				expItemsBox.HeightRequest = 90;
			}
			else
			{
				int maxRequest = -1;
				foreach (Widget childWidget in  expItemsBox.Children) 
				{
					if (maxRequest < childWidget.HeightRequest)
						maxRequest = childWidget.HeightRequest;
				}
				
				expItemsScroller.HeightRequest = maxRequest + 20;
				
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnExpUpBtnClicked(object sender, EventArgs args)
		{
			int position =
				(dialog.ExpressionsBox[this] as Gtk.Box.BoxChild).Position;
			dialog.ExpressionsBox.ReorderChild(this, position-1);
			this.CheckPosition();
			
			((SyntacticalExpressionWidget)dialog.ExpressionsBox.Children[position]).CheckPosition();
		}
		
		private void OnExpDownBtnClicked(object sender, EventArgs args)
		{
			int position = 
				(dialog.ExpressionsBox[this] as Gtk.Box.BoxChild).Position;
			dialog.ExpressionsBox.ReorderChild(this, position+1);
			this.CheckPosition();
			
			((SyntacticalExpressionWidget)dialog.ExpressionsBox.Children[position]).CheckPosition();
		}
		
#endregion Non-public methods
	}
}
