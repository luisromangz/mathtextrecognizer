// SyntacticalRulesManager.cs created with MonoDevelop
// User: luis at 16:38 13/05/2008

using System;
using System.Collections.Generic;

namespace MathTextLibrary.Analisys
{
	
	/// <summary>
	/// This class implements a library of syntactical rules as a 
	/// singleton.
	/// </summary>
	public class SyntacticalRulesLibrary
	{
		private Dictionary<string, SyntacticalRule> rules;
		
		private static SyntacticalRulesLibrary instance;
		
		private SyntacticalRule startRule;
		
		
		/// <summary>
		/// <see cref=""/>'s constructor.
		//SyntacticalRulesLibrary/ </summary>
		private SyntacticalRulesLibrary()
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
		public static SyntacticalRulesLibrary Instance
		{
			get
			{
				if(instance == null)
				{
					instance = new SyntacticalRulesLibrary();
				}
				
				return instance;
			}
		}
		
		/// <value>
		/// Contains the start rule.
		/// </value>
		public SyntacticalRule StartRule
		{
			get
			{
				return startRule; 
			}
			set
			{
				startRule = value;
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
		
		/// <summary>
		/// Removes all rules from the mananager.
		/// </summary>
		public void ClearRules()
		{
			this.rules.Clear();
		}
		
#endregion Public methods
	}
}
