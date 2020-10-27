using System;
using System.Diagnostics;
using System.Drawing;

namespace Game
{
	internal class Program
	{
		static void Main(string[] args)
		{
			using (var stdout = Console.OpenStandardOutput(3000))
			{
					// 
					var b = new byte[3000];
					for (int i = 0; i < 3000; i++)
					{
							b[i] = 55;
					}

var w = new Stopwatch();
			w.Start();

					stdout.Write(b, 0, b.Length);
					stdout.Write()
					
					w.Stop();
			var d = w.ElapsedMilliseconds;
			}
			Console.ReadLine();
return;

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
					//new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false), Command.Up),
					//new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false), Command.Down),
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
			el.Players.Add(playerOne);
			

			var keyboard = new Keyboard();

			var screen = new Screen();
			screen.Size = new Size(100, 40);
			screen.Title = "The Worms Game";
			screen.Clear();

			var frame = new Frame(screen.Size);

			do
			{
				keyboard.Read();

				gameWorld.Tick(keyboard);
				frame.Clear();
				gameWorld.Render(frame);

				screen.WaitRefresh();
				screen.Draw(frame, new Point(0, 0));

				keyboard.Clear();
			}
			while(!host.Quit);
		}
	}
}
