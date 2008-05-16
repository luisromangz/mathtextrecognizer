// LexicalExpressionWidget.cs created with MonoDevelop
// User: luis at 18:18 16/05/2008

using System;

using Gtk;
using Glade;

using MathTextCustomWidgets.Dialogs;

namespace MathTextRecognizer.LexicalRulesManager
{
	
	
	/// <summary>
	/// This class implements the expression editor widget.
	/// </summary>
	public class LexicalExpressionWidget : Alignment
	{
		[WidgetAttribute]
		private Entry expressionEntry = null;
		
		[WidgetAttribute]
		private Alignment expressionAlignment = null;
		
		[WidgetAttribute]
		private Button lexUpBtn = null;
		
		[WidgetAttribute]
		private Button lexDownBtn = null;
		
		[WidgetAttribute]
		private Label orLabel = null;
		
		private Window parentWindow;
		private VBox container;
		
		/// <summary>
		/// <c>LexicalExpressionWidget</c>'s constructor.
		/// </summary>
		public LexicalExpressionWidget(Window parentWindow, VBox container) 
			: base(0, 0.5f, 0, 0)
		{
			XML gxml = new XML(null, 
			                   "mathtextrecognizer.glade",
			                   "expressionAlignment",
			                   null);
			
			gxml.Autoconnect(this);
			
			this.parentWindow = parentWindow;
			this.container = container;
			
			this.Add(expressionAlignment);
			
			container.Added += new AddedHandler(OnContainerAdded);
			container.Removed += new RemovedHandler(OnContainerRemoved);
			
			this.ShowAll();
		}
		
		
		/// <value>
		/// Contains the widget's expression.
		/// </value>
		public string Expression
		{				
			get
			{
				return expressionEntry.Text.Trim();					
			}
			set
			{
				expressionEntry.Text = value;
			}
		}
		
		/// <summary>
		/// Checks the position of the control and sets the sensitiveness
		/// of the buttons (and other things) accordingly.
		/// </summary>
		public void CheckPosition()
		{
			int position = (container[this] as Gtk.Box.BoxChild).Position;
			bool notLast = position < container.Children.Length -1;
			bool notFirst = position > 0;
			lexDownBtn.Sensitive = notLast;
			lexUpBtn.Sensitive = notFirst;				
			orLabel.Text = notFirst?"|":" ";	
				
		}
		
		/// <summary>
		/// Moves the widget one position before the actual.
		/// </summary>
		private void OnUpBtnClicked(object sender, EventArgs args)
		{
			int position = (container[this] as Gtk.Box.BoxChild).Position;
			container.ReorderChild(this, position-1);
			(container.Children[position] as LexicalExpressionWidget).CheckPosition();
			CheckPosition();
		}
		
		/// <summary>
		/// Handles the addition of a sibling.
		/// </summary>
		private void OnContainerAdded(object sender, AddedArgs args) 
		{				
			CheckPosition();
		}
		
		/// <summary>
		/// Handles the deletion of a sibling.
		/// </summary>
		private void OnContainerRemoved(object sender, RemovedArgs args)
		{			
			CheckPosition();
		}
		
		/// <summary>
		/// Moves the widget one position after the actual.
		/// </summary>
		private void OnDownBtnClicked(object sender, EventArgs args)
		{
			int position = (container[this] as Gtk.Box.BoxChild).Position;
			container.ReorderChild(this, position+1);
			(container.Children[position] as LexicalExpressionWidget).CheckPosition();
			CheckPosition();
		}
		
		/// <summary>
		/// Removes the widget from the list.
		/// </summary>
		private void OnRemoveBtnClicked(object sender, EventArgs args)
		{
			
			ResponseType res = ResponseType.Yes;
			
			// If the widget isn't empty, we have to ask.
			if(!String.IsNullOrEmpty(this.Expression.Trim()))
			{
				res = ConfirmDialog.Show(parentWindow,
				                         "¿Quieres eliminar la expresión?");
			}
			
			
			if(res == ResponseType.Yes)
			{
				this.Destroy();
			}
		}
	}
}
