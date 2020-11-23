using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Game.Worms
{
	public class WormsWorld : GameWorld
	{
		public WorldConfig Configuration { get; private set; }


		public WormsWorld(Size size) : base(size)
		{
			Configuration = new WorldConfig();

			PostMessage(new WorldMessage { Name = MessageName.ShowIntro });
		}


		public WorldLevel Level { get; protected set; }


		public override void Tick(IEnumerable<ConsoleKey> keysPress, IEnumerable<ConsoleKey> keysDown)
		{
			base.Tick(keysPress, keysDown);
			if(Level != null) Level.Tick(keysPress);
		}

		public override void ProcessMessage(WorldMessage message)
		{
			switch(message.Name)
			{
				case MessageName.ShowIntro: LoadLevel(new IntroLevel(this)); break;
				case MessageName.ShowMenu: LoadLevel(new MenuLevel(this)); break;
				case MessageName.StartGame: LoadLevel(new WormsGameLevel(this, 1, new WormsGameLevel.LevelConfig(@"worms\resources\level1"))); break;
				case MessageName.NextLevel: NextGameLevel(); break;
				case MessageName.GameOver: LoadLevel(new LostLevel(this)); break;
				case MessageName.Quit: Quit(); break;
			}
		}

		protected void LoadLevel(WorldLevel level)
		{
			if(Level != null)
				Level.Uninstall();
			Level = level;
			Level.Install();
		}

		protected void NextGameLevel()
		{
			var level = Level as WormsGameLevel;
			if(level != null)
			{
				if(level.Index < 3)
					LoadLevel(new WormsGameLevel(this, level.Index + 1, new WormsGameLevel.LevelConfig(@"worms\resources\level" + (level.Index + 1))));
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



	public class IntroLevel : WorldLevel
	{
		public IntroLevel(GameWorld world) : base(world)
		{ }

		public override void Install()
		{
			var logo = new Element(new Point(World.Size.Width / 2 - 24, 10), new Size(48, 9));
			logo.Load(Path.Combine(Environment.CurrentDirectory, @"worms\resources\banner.txt"), Point.Empty, ConsoleColor.DarkGreen);
			//
			World.Elements.Add(logo);

			World.Elements.Add(new StaticText("Awesome game with worms.", new Point(38, 20)) { Foreground = ConsoleColor.DarkGreen });

			World.Elements.Add(new StaticText("Press any key to start", new Point(39, 32)));
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


	public class MenuLevel : WorldLevel
	{
		public MenuLevel(GameWorld world) : base(world)
		{ }

		public override void Install()
		{
			var host = new WormsHost(World)
			{
				KeyMap = new KeyboardKeyMap[]
				{
						new KeyboardKeyMap(ConsoleKey.Escape, Command.Escape),
						new KeyboardKeyMap(ConsoleKey.Enter, Command.Enter),
						new KeyboardKeyMap(ConsoleKey.UpArrow, Command.Up),
						new KeyboardKeyMap(ConsoleKey.DownArrow, Command.Down)
				}
			};
			//
			World.Players.Add(host);

			var logo = new Element(new Point(World.Size.Width / 2 - 24, 10), new Size(48, 9));
			logo.Load(Path.Combine(Environment.CurrentDirectory, @"worms\resources\banner.txt"), Point.Empty);
			//
			World.Elements.Add(logo);

			var menu = new WormsMenu(new Point(30, 25));
			menu.Players.Add(host);
			//
			World.Elements.Add(menu);
		}

		public override void Uninstall()
		{
			World.Players.Clear();
			World.Elements.Clear();
		}

		public override void Tick(IEnumerable<ConsoleKey> keys)
		{
			var host = World.Players[0] as WormsHost;

			if(host.Action == WormsHost.MenuAction.Quit)
				World.PostMessage(new WorldMessage { Name = MessageName.Quit });
			else if(host.Action == WormsHost.MenuAction.NewGame1)
			{
				((WormsWorld)World).Configuration.Players = 1;
				World.PostMessage(new WorldMessage { Name = MessageName.StartGame });
			}
			else if(host.Action == WormsHost.MenuAction.NewGame2)
			{
				((WormsWorld)World).Configuration.Players = 2;
				World.PostMessage(new WorldMessage { Name = MessageName.StartGame });
			}
		}
	}


	public class WormsGameLevel : WorldLevel
	{
		protected LevelConfig Config;

		protected Arena Arena;
		protected List<Worm> Worms;
		protected int FoodStock;
		protected Food Food;


		public WormsGameLevel(GameWorld world, int level, LevelConfig config) : base(world)
		{
			Index = level;
			Name = level.ToString();
			Config = config;
			FoodStock = config.Food + 1;
			Worms = new List<Worm>();
		}

		public override void Install()
		{
			var player1 = new Player()
			{
				Index = 1,
				Name = "Player One",
				Color = ConsoleColor.Blue,
				Lives = 5,
				Score = 0,
				KeyMap = new KeyboardKeyMap[]
			{
					new KeyboardKeyMap(ConsoleKey.LeftArrow, Command.Left),
					new KeyboardKeyMap(ConsoleKey.RightArrow, Command.Right),
					new KeyboardKeyMap(ConsoleKey.UpArrow, Command.Up),
					new KeyboardKeyMap(ConsoleKey.DownArrow, Command.Down)
			}
			};
			World.Players.Add(player1);
			//
			var score1 = new ScoreBox(player1, new Point(0, 0), player1.Name.Length + 15);
			World.Elements.Add(score1);

			if(((WormsWorld)World).Configuration.Players > 1)
			{
				var player2 = new Player()
				{
					Index = 2,
					Name = "Player Two",
					Color = ConsoleColor.Green,
					Lives = 5,
					Score = 0,
					KeyMap = new KeyboardKeyMap[]
					{
							new KeyboardKeyMap(ConsoleKey.A, Command.Left),
							new KeyboardKeyMap(ConsoleKey.D, Command.Right),
							new KeyboardKeyMap(ConsoleKey.W, Command.Up),
							new KeyboardKeyMap(ConsoleKey.S, Command.Down)
					}
				};
				World.Players.Add(player2);
				//
				var score2 = new ScoreBox(player2, new Point(World.Size.Width - 23, 0), player2.Name.Length + 15);
				World.Elements.Add(score2);
			}
			if(((WormsWorld)World).Configuration.Players > 2)
			{
				//TODO: add more players
			}

			var level = new LevelBox((WormsWorld)World, new Point(World.Size.Width / 2 - 4, 0), 7);
			World.Elements.Add(level);

			Arena = new Arena(Config.DataFolder, new Point(0, 1), new Size(World.Size.Width, World.Size.Height - 1));
			World.Elements.Add(Arena);

			foreach(var player in World.Players)
			{
				var playerConfig = Config.Players[World.Players.IndexOf(player)];
				Worms.Add(new Worm(player, World.Size, playerConfig.Start, playerConfig.Direction));
			}
			World.Elements.AddRange(Worms);

			Food = new Food(Point.Empty);
			World.Elements.Add(Food);
			PlaceFood();
		}

		public override void Uninstall()
		{
			World.Players.Clear();
			World.Elements.Clear();
		}

		public override void Tick(IEnumerable<ConsoleKey> keys)
		{
			// check worms collisions
			foreach(var worm in Worms.Where(w => w.Enabled))
			{
				if(worm.Collisions(Food).Count > 0)
				{
					// if worm collide with food
					worm.Player.Score += worm.Body.Count;
					if(FoodStock == 0)
						World.PostMessage(new WorldMessage { Name = MessageName.NextLevel });
					else
					{
						worm.Grow += Food.Length;
						worm.UpdateTimer.Reset(worm.UpdateTimer.Interval - Food.Speed);
						PlaceFood();
					}
				}
				else if(worm.Collisions(Arena).Count > 0)
				{
					// if worm collide with arena
					worm.Player.Lives--;
					if(worm.Player.Lives > 0)
						worm.Reset();
					else
						worm.Enabled = false;
				}

				// if worm collide with himself
				//TODO:

				// if worm colide with other worms
				//TODO:
			}
			if(Worms.Count(w => w.Enabled) == 0)
				World.PostMessage(new WorldMessage { Name = MessageName.GameOver });

			if(keys.Any(k => k == ConsoleKey.Escape))
				World.PostMessage(new WorldMessage { Name = MessageName.ShowMenu });
		}


		public Point RandomLocation()
		{
			var list = new List<Point>();
			for(int x = Arena.Location.X; x < Arena.Size.Width; x++)
				for(int y = Arena.Location.Y; y < Arena.Size.Height; y++)
					if(World.Elements.All(e => e.GetBrick(new Point(x - e.Location.X, y - e.Location.Y)) == null))
						list.Add(new Point(x, y));
			var rnd = new Random();
			return list[rnd.Next(list.Count)];
		}

		public void PlaceFood()
		{
			if(FoodStock > 0)
			{
				Food.Location = RandomLocation();
				FoodStock--;
			}
		}


		public class LevelConfig
		{
			public string DataFolder;
			public int Food;
			public PlayerConfig[] Players;

			public LevelConfig() { }

			public LevelConfig(string path)
			{
				DataFolder = path;
				Players = new PlayerConfig[] { new PlayerConfig(), new PlayerConfig() };

				File.ReadAllLines(Path.Combine(path, "settings.txt")).Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#")).ToList().ForEach(line =>
				{
					var ary = line.Split("=");
					string option = ary[0].Trim(), value = ary[1].Trim();
					if(option == "food")
						Food = int.Parse(value);
					else if(option.StartsWith("player_1_"))
					{
						if(option == "player_1_start")
						{
							var p = value.Split(",");
							Players[0].Start = new Point(int.Parse(p[0]), int.Parse(p[1]));
						}
						else if(option == "player_1_direction")
							Players[0].Direction = Enum.Parse<MovingDirection>(value);
					}
					else if(option.StartsWith("player_2_"))
					{
						if(Players.Length < 1)
						{
							Array.Resize(ref Players, 2);
							Players[1] = new PlayerConfig();
						}
						if(option == "player_2_start")
						{
							var p = value.Split(",");
							Players[1].Start = new Point(int.Parse(p[0]), int.Parse(p[1]));
						}
						else if(option == "player_2_direction")
							Players[1].Direction = Enum.Parse<MovingDirection>(value);
					}
				});
			}
		}

		public class PlayerConfig
		{
			public Point Start;
			public MovingDirection Direction;
		}
	}


	public class LostLevel : WorldLevel
	{
		public LostLevel(GameWorld world) : base(world)
		{
			Name = "lost";
		}

		public override void Install()
		{
			var banner = new Element(new Point(World.Size.Width / 2 - 39, 10), new Size(76, 7));
			banner.Load(Path.Combine(Environment.CurrentDirectory, @"worms\resources\lose.txt"), Point.Empty, ConsoleColor.DarkRed);
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


	public class WinLevel : WorldLevel
	{
		public WinLevel(GameWorld world) : base(world)
		{
			Name = "win";
		}

		public override void Install()
		{
			var banner = new Element(new Point(World.Size.Width / 2 - 29, 10), new Size(76, 7));
			banner.Load(Path.Combine(Environment.CurrentDirectory, @"worms\resources\win.txt"), Point.Empty, ConsoleColor.DarkGreen);
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


	public class WorldConfig
	{
		public int Players;
	}
}
