using System;
using System.Drawing;

namespace Game
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			Console.Clear();
			Console.Title = "The Worms Game";
			ColorConsole.Enable();
			ColorConsole.Size = new Size(100, 40);

			WormsGame.Launch();

			//restore size
			ColorConsole.Disable();
			//restore title
		}
	}
}
