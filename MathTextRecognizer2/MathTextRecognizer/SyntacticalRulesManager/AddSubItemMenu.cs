// AddSubItemMenu.cs created with MonoDevelop
// User: luis at 16:41 17/05/2008

using System;

using Gtk;
using Glade;

namespace MathTextRecognizer.SyntacticalRulesManager
{
	
	/// <summary>
	/// This class holds a menu that is used to create new items in expressions
	/// and syntactical groups.
	/// </summary>
	public class AddSubItemMenu
	{
	
#region Fields
		private IExpressionItemContainer container;
		
		[Widget]
		private Menu addExpressionItemMenu = null;
		
#endregion Fields
		
#region Constructors
		
		/// <summary>
		/// <see cref="AddSubItemMenu"/>'s constructor.
		/// </summary>
		/// <param name="container">
		/// A <see cref="IExpressionItemContainer"/> 
		/// Where the elements created by the menu items
		/// will be stored.
		/// </param>
		public AddSubItemMenu(IExpressionItemContainer container)
		{
			Glade.XML gladeXml =new XML("mathtextrecognizer.glade",
			                            "addExpressionItemMenu");
			
			gladeXml.Autoconnect(this);				
			
			this.container = container;
		}
		
#endregion Constructors
	
#region Public methods
		
		/// <summary>
		/// Shows the menu.
		/// </summary>
		public void Popup()
		{
			addExpressionItemMenu.Popup();
		}
		
#endregion Public methods
		
#region Non-public methods
		
		/// <summary>
		/// Adds a <see cref="ItemWidget"/> to the container.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnAddTokenItmActivate(object sender, EventArgs args)
		{
			container.AddItem(new ExpressionTokenWidget(container));
		}
		
		/// <summary>
		/// Adds a <see cref="GroupWidget"/> to the container
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnAddGroupItmActivate(object sender, EventArgs args)
		{
			container.AddItem(new ExpressionGroupWidget(container));
		}
		
		/// <summary>
		/// Adds a <see cref="SubexpressionWidget"/> to the container.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// A <see cref="EventArgs"/>
		/// </param>
		private void OnAddSubexpressionItmActivate(object sender, EventArgs args)
		{
			container.AddItem(new ExpressionRuleCallWidget(container));
		}
		
#endregion Non-public methods
	}
}
