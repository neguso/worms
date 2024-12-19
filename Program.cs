using System;
using System.Drawing;

using Game;
using Game.Invaders;
using Game.Worms;

namespace MyGame
{
	internal class Program
	{
		public static void Main()
		{
			ColorConsole.SaveStatus();
			ColorConsole.Enable();

			Console.CursorVisible = false;
			Console.Clear();
			Console.Title = "Invaders Game";
			ColorConsole.Size = new Size(100, 40);

			InvadersGame.Launch();

			ColorConsole.Disable();
			ColorConsole.RestoreStatus();
		}

		public static void Main2()
		{
			ColorConsole.SaveStatus();
			ColorConsole.Enable();

			Console.CursorVisible = false;
			Console.Clear();
			Console.Title = "Worms Game";
			ColorConsole.Size = new Size(100, 40);

			WormsGame.Launch();

			ColorConsole.Disable();
			ColorConsole.RestoreStatus();
		}
	}
}
