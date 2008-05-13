// SyntacticalRulesManager.cs created with MonoDevelop
// User: luis at 16:38 13/05/2008

using System;
using System.Collections.Generic;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This class implements a manager of syntactical rules as a 
	/// singleton.
	/// </summary>
	public class SyntacticalRulesManager
	{
		private Dictionary<string, SyntacticalRule> rules;
		
		private static SyntacticalRulesManager instance;
		
		
		/// <summary>
		/// <see cref="SyntacticalRulesManager"/>'s constructor.
		/// </summary>
		private SyntacticalRulesManager()
		{
			rules = new Dictionary<string,SyntacticalRule>();
		}
		
		
#region Properties
		
		/// <value>
		/// Contains the rule named like the parameter.
		/// </value>
		public SyntacticalRule this[string ruleName]
		{
			get
			{
				return rules[ruleName];
			}
		}
		
		
		/// <summary>
		/// Contains the only instance allowed for this class.
		/// </summary>
		public static SyntacticalRulesManager Instance
		{
			get
			{
				if(instance == null)
				{
					instance = new SyntacticalRulesManager();
				}
				
				return instance;
			}
		}
		
#endregion Properties
		
#region Public methods
		
		/// <summary>
		/// Adds a rule to the manager
		/// </summary>
		/// <param name="rule">
		/// The <see cref="SyntacticalRule"/> that will be managed.
		/// </param>
		public void AddRule(SyntacticalRule rule)
		{
			this.rules.Add(rule.Name, rule);
		}
		
#endregion Public methods
	}
}
