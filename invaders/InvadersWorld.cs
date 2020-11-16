using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Game.Invaders
{
	public class InvadersWorld : GameWorld
	{
		public InvadersWorld(Size size) : base(size)
		{
			PostMessage(new WorldMessage { Name = MessageName.ShowIntro });
		}


		public GameLevel Level { get; protected set; }


		public override void Tick(IEnumerable<ConsoleKey> keys)
		{
			base.Tick(keys);
			if(Level != null) Level.Tick(keys);
		}

		public override void ProcessMessage(WorldMessage message)
		{
			switch(message.Name)
			{
				case MessageName.ShowIntro: LoadLevel(new IntroLevel(this)); break;
				//case MessageName.ShowMenu: LoadLevel(new MenuLevel(this)); break;
				case MessageName.StartGame: LoadLevel(new InvadersGameLevel(this, 1, new InvadersGameLevel.LevelConfig(@"invaders\resources\level1"))); break;
				//case MessageName.NextLevel: NextGameLevel(); break;
				//case MessageName.GameOver: LoadLevel(new LostLevel(this)); break;
				case MessageName.Quit: Quit(); break;
			}
		}

		protected void LoadLevel(GameLevel level)
		{
			if(Level != null)
				Level.Uninstall();
			Level = level;
			Level.Install();
		}

		protected void NextGameLevel()
		{
			var level = Level as InvadersGameLevel;
			if(level != null)
			{
				if(level.Index < 3)
					LoadLevel(new InvadersGameLevel(this, level.Index + 1, new InvadersGameLevel.LevelConfig(@"invaders\resources\level" + (level.Index + 1))));
				else
					LoadLevel(new WinLevel(this));
			}
		}

		protected void Quit()
		{
			Level.Uninstall();
			Level = null;
			Active = false;
		}
	}



	public class IntroLevel : GameLevel
	{
		public IntroLevel(GameWorld world) : base(world)
		{ }

		public override void Install()
		{
			var logo = new Element(new Point(2, 5), new Size(World.Size.Width, 18));
			logo.Load(Path.Combine(Environment.CurrentDirectory, @"invaders\resources\banner.txt"), Point.Empty, ConsoleColor.DarkGreen);
			//
			World.Elements.Add(logo);

			World.Elements.Add(new StaticText(" Protect the Earth from space invaders ", new Point(31, 20)) { Foreground = ConsoleColor.Green });

			World.Elements.Add(new StaticText("Press any key to start", new Point(39, 32)));
		}

		public override void Uninstall()
		{
			World.Elements.Clear();
		}

		public override void Tick(IEnumerable<ConsoleKey> keys)
		{
			if(keys.Count() > 0)
				World.PostMessage(new WorldMessage { Name = MessageName.StartGame });
		}
	}


	public class InvadersGameLevel : GameLevel
	{
		protected LevelConfig Config;

		protected Arena Arena;
		protected DefenderShip defender;


		public InvadersGameLevel(GameWorld world, int level, LevelConfig config) : base(world)
		{
			Index = level;
			Name = level.ToString();
			Config = config;
		}


		public override void Install()
		{
			var player = new Player()
			{
				Index = 1,
				Name = "Player",
				Color = ConsoleColor.White,
				Lives = 3,
				Score = 0,
				KeyMap = new KeyboardKeyMap[]
				{
					new KeyboardKeyMap(ConsoleKey.LeftArrow, Command.Left),
					new KeyboardKeyMap(ConsoleKey.RightArrow, Command.Right),
					new KeyboardKeyMap(ConsoleKey.Spacebar, Command.Fire)
				}
			};
			World.Players.Add(player);

			Arena = new Arena(Config.DataFolder, new Point(0, 0), new Size(World.Size.Width, World.Size.Height));
			World.Elements.Add(Arena);

			defender = new DefenderShip(player, new Point(10, 35), World.Size);
			World.Elements.Add(defender);
		}

		public override void Uninstall()
		{
			World.Players.Clear();
			World.Elements.Clear();
		}



		public class LevelConfig
		{
			public string DataFolder;

			public LevelConfig() { }

			public LevelConfig(string path)
			{
				DataFolder = path;
			}
		}
	}


	public class LostLevel : GameLevel
	{
		public LostLevel(GameWorld world) : base(world)
		{
			Name = "lost";
		}

		public override void Install()
		{
			var banner = new Element(new Point(World.Size.Width / 2 - 39, 10), new Size(76, 7));
			banner.Load(Path.Combine(Environment.CurrentDirectory, @"invaders\resources\lose.txt"), Point.Empty, ConsoleColor.DarkRed);
			//
			World.Elements.Add(banner);
		}

		public override void Uninstall()
		{
			World.Elements.Clear();
		}

		public override void Tick(IEnumerable<ConsoleKey> keys)
		{
			if(keys.Count() > 0)
				World.PostMessage(new WorldMessage { Name = MessageName.ShowMenu });
		}
	}


	public class WinLevel : GameLevel
	{
		public WinLevel(GameWorld world) : base(world)
		{
			Name = "win";
		}

		public override void Install()
		{
			var banner = new Element(new Point(World.Size.Width / 2 - 29, 10), new Size(76, 7));
			banner.Load(Path.Combine(Environment.CurrentDirectory, @"invaders\resources\win.txt"), Point.Empty, ConsoleColor.DarkGreen);
			//
			World.Elements.Add(banner);
		}

		public override void Uninstall()
		{
			World.Elements.Clear();
		}

		public override void Tick(IEnumerable<ConsoleKey> keys)
		{
			if(keys.Count() > 0)
				World.PostMessage(new WorldMessage { Name = MessageName.ShowMenu });
		}
	}




	public static class MessageName
	{
		public const string ShowIntro = "show_intro";
		public const string ShowMenu = "show_menu";
		public const string StartGame = "start_game";
		public const string Quit = "quit";
		public const string NextLevel = "next_level";
		public const string GameOver = "game_over";
		public const string GameCompleted = "game_completed";
	}
}
