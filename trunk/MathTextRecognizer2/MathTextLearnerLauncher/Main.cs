// Main.cs created with MonoDevelop
// User: luis at 12:54 24/04/2008
using System;

using Gtk;

using MathTextLearner;


namespace MathTextLearnerLauncher
{
	public class MainClass
	{
		public static void Main(string[] args)
		{
			Application.Init();		
			new MainLearnerWindow();	
			Application.Run();
		
		}
	}
}