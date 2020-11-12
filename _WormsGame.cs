using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Game
{
	public abstract class GenericGame
	{
		protected Keyboard keyboard;
		protected Screen screen;
		protected Frame frame;


		protected GenericGame()
		{
			keyboard = new Keyboard();
			screen = new Screen();
			frame = new Frame(screen.Size);
		}


		public GenericWorld World { get; protected set; }


		public void Run()
		{
			do
			{
				// get user input
				keyboard.Clear();
				keyboard.Read();

				// process world
				World.Tick(keyboard);

				// render frame
				frame.Clear();
				World.Render(frame);

				// draw frame on screen
				screen.WaitRefresh();
				screen.Draw(frame);
			}
			while(World.Status != null);
		}
	}


	public class WormsGame : GenericGame
	{
		private WormsGame()
		{
			World = new WormsWorld(screen.Size);
		}


		public static void Launch()
		{
			new WormsGame().Run();
		}
	}



	public class _WormsGame
	{
		protected Keyboard keyboard;
		protected Screen screen;
		protected Frame frame;

		protected GenericWorld menuWorld;
		protected WormsWorld gameWorld;
		protected WormsHost host;


		private _WormsGame()
		{
			keyboard = new Keyboard();
			screen = new Screen();
			frame = new Frame(screen.Size);
		}


		public static void Launch()
		{
			new _WormsGame().Run();
		}


		public void Run()
		{
			RunIntro();

			SetupMenu();
			do
			{
				RunMenu();

				switch(host.Action)
				{
					case WormsHost.MenuAction.NewGame1:
					case WormsHost.MenuAction.NewGame2:
						SetupGame(host.Action == WormsHost.MenuAction.NewGame1 ? 1 : 2);
						RunGame();

						if(gameWorld.Alive)
						{
							//if(gameWorld.Completed)
							{
								// game finished
								frame.Clear();
								frame.Load(Path.Combine(Environment.CurrentDirectory, @"resources\win.txt"), new Point(0, 13), ConsoleColor.DarkRed);
								screen.Draw(frame, new Point(0, 0));
								Console.ReadKey();
							}
						}
						else
						{
							// game over
							frame.Clear();
							frame.Load(Path.Combine(Environment.CurrentDirectory, @"resources\lose.txt"), new Point(screen.Size.Width / 2 - 39, 10), ConsoleColor.DarkRed);
							screen.Draw(frame, new Point(0, 0));
							Console.ReadKey();
						}

						break;

				}
			}
			while(host.Action != WormsHost.MenuAction.Quit);
		}


		public void RunIntro()
		{
			frame.Load(Path.Combine(Environment.CurrentDirectory, @"resources\banner.txt"), new Point(screen.Size.Width / 2 - 24, 10), ConsoleColor.DarkGreen);
			frame.Text(new Point(38, 20), "Awesome game with worms.", ConsoleColor.Green);
			frame.Text(new Point(39, 32), "Press any key to start");
			screen.Draw(frame, new Point(0, 0));
			Console.ReadKey();
		}

		public void SetupMenu()
		{
			//menuWorld = new GenericWorld();

			host = new WormsHost(menuWorld)
			{
				KeyMap = new KeyboardKeyMap[]
				{
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false), Command.Escape),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.Enter, false, false, false), Command.Enter),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false), Command.Up),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false), Command.Down)
				}
			};
			//
			menuWorld.Players.Add(host);

			var logo = new Element(new Point(screen.Size.Width / 2 - 24, 10), new Size(48, 9));
			logo.Load(Path.Combine(Environment.CurrentDirectory, @"resources\banner.txt"), Point.Empty);
			//
			menuWorld.Elements.Add(logo);

			var menu = new WormsMenu(new Point(screen.Size.Width / 2 - 8, 25));
			menu.Players.Add(host);
			//
			menuWorld.Elements.Add(menu);
		}

		public void RunMenu()
		{
			host.Action = WormsHost.MenuAction.None;

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
			while(host.Action == WormsHost.MenuAction.None);

			screen.Clear();
		}

		public void SetupGame(int players)
		{
			//gameWorld = new WormsWorld(screen.Size, players);
		}

		public void RunGame()
		{
			do
			{
				// get user input
				keyboard.Clear();
				keyboard.Read();

				// process world
				gameWorld.Tick(keyboard);

				// render frame
				frame.Clear();
				gameWorld.Render(frame);

				// draw frame on screen
				screen.WaitRefresh();
				screen.Draw(frame);
			}
			while(gameWorld.Alive);// && !gameWorld.Completed);
		}

	}
}