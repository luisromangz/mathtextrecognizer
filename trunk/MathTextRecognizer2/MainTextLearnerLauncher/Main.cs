// Main.cs created with MonoDevelop
// User: luis at 12:52 24/04/2008
using System;

namespace MainTextLearnerLauncher
{
	
	class MainClass
	{
		public static void Main(string[] args)
		{
			Application.Init();		
			new MainLearnerWindow(null, null);	
			Application.Run();
		
		}
	}
}