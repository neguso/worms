using System;
using System.Drawing;

namespace Game
{
	internal class Program
	{
		static void Main(string[] args)
		{
			// menu world //
			//TODO


			// setup game world //
			var gameWorld = new GameWorld();

			var host = new Host()
			{
				KeyMap = new KeyboardKeyMap[]
				{
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false), Command.Escape),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false), Command.Enter),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false), Command.Up),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false), Command.Down),
				}
			};
			gameWorld.Players.Add(host);

			var playerOne = new Player()
			{
				Name = "Player One",
				KeyMap = new KeyboardKeyMap[]
				{
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false), Command.Left),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false), Command.Right),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false), Command.Up),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false), Command.Down)
				}
			};
			gameWorld.Players.Add(playerOne);


			var el = new TestElement();
			gameWorld.Elements.Add(el);


			var keyboard = new Keyboard();

			var screen = new Screen();
			screen.Size = new Size(100, 40);
			screen.Title = "The Worms Game";

			var frame = new Frame(screen.Size);

			do
			{
				keyboard.Read();

				gameWorld.Tick(keyboard);
				gameWorld.Render(frame);

				screen.WaitRefresh();
				screen.Draw(frame, new Point(5, 5));

				keyboard.Clear();
			}
			while(!host.Quit);


		}
	}
}
