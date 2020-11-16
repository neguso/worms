using System;
using System.Drawing;

using Game;
using Game.Invaders;
using Game.Worms;

namespace MyGame
{
	internal class Program
	{
		public static void Main1(string[] args)
		{
			Console.Clear();
			Console.Title = "Space Invaders";
			ColorConsole.Enable();
			ColorConsole.Size = new Size(100, 40);

			InvadersGame.Launch();

			//restore size
			ColorConsole.Disable();
			//restore title
		}

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
