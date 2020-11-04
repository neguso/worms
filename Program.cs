using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

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

			Game.Launch();

			//restore size
			ColorConsole.Disable();
			//restore title
		}

/*
    public static void _Main(string[] args)
    {
      Console.Clear();
      ColorConsole.Enable();
			



      var gameWorld = new GenericWorld();

      var host = new WormsHost()
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
			//
			var playerTwo = new Player()
      {
        Name = "Player Two",
        KeyMap = new KeyboardKeyMap[]
        {
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.A, false, false, false), Command.Left),
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.D, false, false, false), Command.Right),
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.W, false, false, false), Command.Up),
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.S, false, false, false), Command.Down)
        }
      };
      gameWorld.Players.Add(playerTwo);


      var el11 = new TestElement();
      gameWorld.Elements.Add(el11);
      el11.Players.Add(playerOne);

			var el12 = new TestElement();
      gameWorld.Elements.Add(el12);
      el12.Players.Add(playerTwo);

			var el2 = new ScrollingText("Quick brown fox jumps over the lazy dog!", new Point(20, 10), 100);
			gameWorld.Elements.Add(el2);

      var keyboard = new Keyboard();

      var screen = new Screen();
      screen.Clear();

      var frame = new Frame(screen.Size);


			frame.Load(Path.Combine(Environment.CurrentDirectory, @"resources\banner.txt"), new Point(10, 3));
			screen.Draw(frame, new Point(0, 0));
			Console.ReadKey();

      do
      {
        // get user input
        keyboard.Clear();
        keyboard.Read();

        // process world
        gameWorld.Tick(keyboard);

        // render frame
        frame.Fill(Brick.Blank);
        gameWorld.Render(frame);

        // draw frame on screen
        screen.WaitRefresh();
        screen.Draw(frame, new Point(0, 0));
      }
      while (!host.Quit);

			ColorConsole.Disable();
			Console.Clear();
    }
*/

		

		public static void RunGame()
		{
			
		}

  }


	public class Game
	{
		protected Keyboard keyboard;
		protected Screen screen;
		protected Frame frame;

		protected GenericWorld menuWorld;
		protected WormsHost host;



		private Game()
		{
			keyboard = new Keyboard();
      screen = new Screen();
      frame = new Frame(screen.Size);
		}


		public static void Launch()
		{
			new Game().Run();
		}


		public void Run()
		{
			RunIntro();

			SetupMenu();
			RunMenu();

		}


		public void RunIntro()
		{
			frame.Load(Path.Combine(Environment.CurrentDirectory, @"resources\banner.txt"), new Point(screen.Size.Width / 2 - 24, 5));
			screen.Draw(frame, new Point(0, 0));
			Console.ReadKey();
			screen.Clear();
		}

		public void SetupMenu()
		{
			menuWorld = new GenericWorld();

			host = new WormsHost(menuWorld)
      {
        KeyMap = new KeyboardKeyMap[]
        {
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false), Command.Escape),
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false), Command.Enter),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false), Command.Up),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false), Command.Down)
				}
      };
			//
      menuWorld.Players.Add(host);

			var menu = new WormsMenu(new Point(10, 10));
      menu.Players.Add(host);
			//
      menuWorld.Elements.Add(menu);
		}

		public void RunMenu()
		{
			do
      {
        // get user input
        keyboard.Clear();
        keyboard.Read();

        // process world
        menuWorld.Tick(keyboard);

        // render frame
        frame.Clear();
        menuWorld.Render(frame);

        // draw frame on screen
        screen.WaitRefresh();
        screen.Draw(frame);
      }
      while (host.Action == WormsHost.ActionEnum.None);
		}

	}

}
