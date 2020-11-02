using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Game
{
	public static class Escape
	{
		private static int[] ForegroundColorMap = { 30, 34, 32, 36, 31, 35, 33, 37, 90, 94, 92, 96, 91, 95, 93, 97 };
		private static int[] BackgroundColorMap = { 40, 44, 42, 46, 41, 45, 43, 47, 100, 104, 102, 106, 101, 105, 103, 107 };

		public static string Color(ConsoleColor foreground, ConsoleColor background)
		{
			var f = ForegroundColorMap[(int)foreground];
			var b = BackgroundColorMap[(int)background];
			return $"\x1b[{f};{b}m";
		}

		public static string Location(int left, int top)
		{
			return $"\x1b[{top + 1};{left + 1}H";
		}

		public static string MoveRight(int n)
		{
			return $"\x1b[{n}C";
		}
	}

  internal class Program
  {
    static void Main(string[] args)
    {
			Console.Clear();
			ColorConsole.Enable();
/*
			var buffer = new byte[255];
			for (int i = 0; i < 255; i++)
				buffer[i] = (byte)i;
			ColorConsole.Write(buffer);
			ColorConsole.Write("\xb0");
Console.ReadLine();
return;
*/

/*
			var stdout = Console.OpenStandardOutput();

			var rnd = new Random();
			var sb = new StringBuilder();

			//for (int i = 0; i < 16; i++)
			//	sb.AppendLine($"{i} - {Enum.GetName(typeof(ConsoleColor), i)} - {Color.Make((ConsoleColor)i, ConsoleColor.Black)}text text text text text text text text text text text text text text text text \x1b[39;49m");	
			
			sb.Append(Escape.Location(1, 1));
			for (int y = 0; y < 28; y++)
			{
				for (int x = 0; x < 118; x++)
				{
					sb.Append($"{Escape.Color((ConsoleColor)rnd.Next(16), ConsoleColor.Black)}O");
					//sb.Append("O");
				}
				sb.Append("\x1b[1E\x1b[2G");
			}

			var text = sb.ToString();
			var buffer = text.ToCharArray().Select(c => (byte)c).ToArray();
			
			var w = new Stopwatch();
      w.Start();

			//Console.Write(text);
			stdout.Write(buffer, 0, buffer.Length);

      w.Stop();
      var duration = w.ElapsedMilliseconds;

			Console.Write($"\x1b[{39};{49}m" + duration);

      Console.ReadLine();
      return;
*/

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
        frame.Fill(Brick.Blank);
        gameWorld.Render(frame);

        screen.WaitRefresh();
        screen.Draw(frame, new Point(10, 3));

        keyboard.Clear();
      }
      while (!host.Quit);
    }
  }
}
