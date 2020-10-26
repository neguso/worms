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

			var keyboard = new Keyboard();

			var screen = new Screen();
			var frame = new Frame(new Size(100, 30));
frame.SetBrick(new Point(10, 10), Brick.From('X'));
frame.HLine(new Point(0, 5), 10, Brick.From('-'));
frame.HLine(new Point(0, 6), 10, Brick.From('-'), new Brick[] { Brick.From('a'), Brick.From('b') });
frame.HLine(new Point(0, 7), 3, Brick.From('-'), new Brick[] { Brick.From('a'), Brick.From('b') });
frame.HLine(new Point(0, 8), 2, Brick.From('-'), new Brick[] { Brick.From('a'), Brick.From('b') });
frame.HLine(new Point(0, 9), 1, Brick.From('-'), new Brick[] { Brick.From('a'), Brick.From('b') });

frame.VLine(new Point(20, 5), 10, Brick.From('|'));
frame.VLine(new Point(21, 5), 10, Brick.From('|'), new Brick[] { Brick.From('a'), Brick.From('b') });
frame.VLine(new Point(22, 5), 3, Brick.From('|'), new Brick[] { Brick.From('a'), Brick.From('b') });
frame.VLine(new Point(23, 5), 2, Brick.From('|'), new Brick[] { Brick.From('a'), Brick.From('b') });
frame.VLine(new Point(24, 5), 1, Brick.From('|'), new Brick[] { Brick.From('a'), Brick.From('b') });

frame.Rectangle(new Point(40, 0), new Size(5, 5), new Brick[] { Brick.From('a'), Brick.From('b'), Brick.From('c'), Brick.From('d') }, new Brick[] { Brick.From('1'), Brick.From('2'), Brick.From('3'), Brick.From('4') });
frame.Rectangle(new Point(40, 10), new Size(3, 3), new Brick[] { Brick.From('a'), Brick.From('b'), Brick.From('c'), Brick.From('d') }, new Brick[] { Brick.From('1'), Brick.From('2'), Brick.From('3'), Brick.From('4') });
frame.Rectangle(new Point(40, 15), new Size(2, 2), new Brick[] { Brick.From('a'), Brick.From('b'), Brick.From('c'), Brick.From('d') }, new Brick[] { Brick.From('1'), Brick.From('2'), Brick.From('3'), Brick.From('4') });
frame.Rectangle(new Point(40, 18), new Size(1, 1), new Brick[] { Brick.From('a'), Brick.From('b'), Brick.From('c'), Brick.From('d') }, new Brick[] { Brick.From('1'), Brick.From('2'), Brick.From('3'), Brick.From('4') });

frame.Area(new Point(50, 10), 4, 3, Brick.From('X'));

screen.Update(frame, new Point(1, 1));
			do
			{
				keyboard.Read();

				gameWorld.Tick(keyboard);

				gameWorld.Render(frame);

				//screen.Update(frame, new Point(5, 5));

				keyboard.Clear();
			}
			while(!host.Quit);


		}
	}
}
