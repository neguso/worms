using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Game
{
	public abstract class GenericWorld
	{
		protected Queue<GenericMessage> messages;

		public bool Active { get; set; }
		public Size Size { get; private set; }
		public List<Player> Players { get; private set; }
		public List<Element> Elements { get; private set; }


		public GenericWorld(Size size)
		{
			messages = new Queue<GenericMessage>();

			Active = true;
			Size = size;
			Players = new List<Player>();
			Elements = new List<Element>();
		}


		public virtual void PostMessage(GenericMessage message)
		{
			messages.Enqueue(message);
		}

		public virtual void Tick(Keyboard keyboard)
		{
			if(messages.Count > 0)
				ProcessMessage(messages.Dequeue());

			Players.ForEach(player =>
			{
				var command = keyboard.DequeueCommand(player);
				if(command != null)
				{
					player.Process(command);
					var list = Elements.Where(element => element.Players.Any(p => p.Name == player.Name)).ToList();
					list.ForEach(element => element.Process(command));
				}
			});
			Elements.ForEach(element => element.Update());
		}

		public virtual void ProcessMessage(GenericMessage message) { }

		public virtual void Render(Frame frame)
		{
			Elements.Sort((a, b) => a.ZIndex < b.ZIndex ? -1 : (a.ZIndex > b.ZIndex ? 1 : 0));
			Elements.ForEach(element => element.Draw(frame));
		}



		public class GenericMessage
		{
			public string Name;
		}
	}



	public class WormsWorld : GenericWorld
	{
		public WorldConfig Configuration { get; private set; }


		public WormsWorld(Size size) : base(size)
		{
			Configuration = new WorldConfig();

			PostMessage(new GenericMessage { Name = Messages.ShowIntro });
		}


		public GenericLevel Level { get; protected set; }


		public override void Tick(Keyboard keyboard)
		{
			base.Tick(keyboard);
			if(Level != null) Level.Tick(keyboard);
		}

		public override void ProcessMessage(GenericMessage message)
		{
			switch(message.Name)
			{
				case Messages.ShowIntro: LoadLevel(new IntroLevel(this)); break;
				case Messages.ShowMenu: LoadLevel(new MenuLevel(this)); break;
				case Messages.StartGame: LoadLevel(new GameLevel(this, 1, new GameLevel.LevelConfig(@"resources\level1"))); break;
				case Messages.NextLevel: NextGameLevel(); break;
				case Messages.GameOver: LoadLevel(new LostLevel(this)); break;
				case Messages.Quit: Quit(); break;
			}
		}

		protected void LoadLevel(GenericLevel level)
		{
			if(Level != null)
				Level.Uninstall();
			Level = level;
			Level.Install();
		}

		protected void NextGameLevel()
		{
			var level = Level as GameLevel;
			if(level != null)
			{
				if(level.Level < 3)
					LoadLevel(new GameLevel(this, level.Level + 1, new GameLevel.LevelConfig(@"resources\level" + (level.Level + 1))));
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


		public static class Messages
		{
			public const string ShowIntro = "show_intro";
			public const string ShowMenu = "show_menu";
			public const string StartGame = "start_game";
			public const string Quit = "quit";
			public const string NextLevel = "next_level";
			public const string GameOver = "game_over";
			public const string GameCompleted = "game_completed";
		}


		public abstract class GenericLevel
		{
			public GenericWorld World { get; protected set; }
			public string Name;

			public GenericLevel(GenericWorld world)
			{
				World = world;
			}

			public virtual void Install() { }
			public virtual void Uninstall() { }
			public virtual void Tick(Keyboard keyboard) { }
		}


		public class IntroLevel : GenericLevel
		{
			public IntroLevel(GenericWorld world) : base(world)
			{
				Name = "intro";
			}

			public override void Install()
			{
				var logo = new Element(new Point(World.Size.Width / 2 - 24, 10), new Size(48, 9));
				logo.Load(Path.Combine(Environment.CurrentDirectory, @"resources\banner.txt"), Point.Empty, ConsoleColor.DarkGreen);
				//
				World.Elements.Add(logo);

				World.Elements.Add(new StaticText("Awesome game with worms.", new Point(38, 20)) { Foreground = ConsoleColor.DarkGreen });
				World.Elements.Add(new StaticText("Press any key to start", new Point(39, 32)));
			}

			public override void Uninstall()
			{
				World.Elements.Clear();
			}

			public override void Tick(Keyboard keyboard)
			{
				if(keyboard.Buffer.Count > 0)
					World.PostMessage(new GenericMessage { Name = Messages.ShowMenu });
			}
		}


		public class MenuLevel : GenericLevel
		{
			public MenuLevel(GenericWorld world) : base(world)
			{
				Name = "menu";
			}

			public override void Install()
			{
				var host = new WormsHost(World)
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
				World.Players.Add(host);

				var logo = new Element(new Point(World.Size.Width / 2 - 24, 10), new Size(48, 9));
				logo.Load(Path.Combine(Environment.CurrentDirectory, @"resources\banner.txt"), Point.Empty);
				//
				World.Elements.Add(logo);

				var menu = new WormsMenu(new Point(World.Size.Width / 2 - 8, 25));
				menu.Players.Add(host);
				//
				World.Elements.Add(menu);
			}

			public override void Uninstall()
			{
				World.Players.Clear();
				World.Elements.Clear();
			}

			public override void Tick(Keyboard keyboard)
			{
				var host = World.Players[0] as WormsHost;

				if(host.Action == WormsHost.MenuAction.Quit)
					World.PostMessage(new GenericMessage { Name = Messages.Quit });
				else if(host.Action == WormsHost.MenuAction.NewGame1)
				{
					((WormsWorld)World).Configuration.Players = 1;
					World.PostMessage(new GenericMessage { Name = Messages.StartGame });
				}
				else if(host.Action == WormsHost.MenuAction.NewGame2)
				{
					((WormsWorld)World).Configuration.Players = 2;
					World.PostMessage(new GenericMessage { Name = Messages.StartGame });
				}
			}
		}


		public class GameLevel : GenericLevel
		{
			protected LevelConfig Config;

			protected Arena Arena;
			protected List<Worm> Worms;
			protected int FoodStock;
			protected Food Food;

			public int Level { get; private set; }


			public GameLevel(GenericWorld world, int level, LevelConfig config) : base(world)
			{
				Level = level;
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
					Lives = 1,
					Score = 0,
					KeyMap = new KeyboardKeyMap[]
				{
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false), Command.Left),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false), Command.Right),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false), Command.Up),
					new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false), Command.Down)
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
						Lives = 1,
						Score = 0,
						KeyMap = new KeyboardKeyMap[]
						{
							new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.A, false, false, false), Command.Left),
							new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.D, false, false, false), Command.Right),
							new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.W, false, false, false), Command.Up),
							new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.S, false, false, false), Command.Down)
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

				foreach(var player in World.Players.Where(p => p.Lives > 0))
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

			public override void Tick(Keyboard keyboard)
			{
				// check worms collisions
				foreach(var worm in Worms.Where(w => w.Enabled))
				{
					if(worm.Collisions(Food).Count > 0)
					{
						// if worm collide with food
						worm.Player.Score += worm.Body.Count;
						if(FoodStock == 0)
							World.PostMessage(new GenericMessage { Name = Messages.NextLevel });
						else
						{
							worm.Grow += Food.Weight;
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
					World.PostMessage(new GenericMessage { Name = Messages.GameOver });

				if(keyboard.Buffer.ToList().Any(k => k.Key == ConsoleKey.Escape))
					World.PostMessage(new GenericMessage { Name = Messages.ShowMenu });
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


		public class LostLevel : GenericLevel
		{
			public LostLevel(GenericWorld world) : base(world)
			{
				Name = "lost";
			}

			public override void Install()
			{
				var banner = new Element(new Point(World.Size.Width / 2 - 39, 10), new Size(76, 7));
				banner.Load(Path.Combine(Environment.CurrentDirectory, @"resources\lose.txt"), Point.Empty, ConsoleColor.DarkRed);
				//
				World.Elements.Add(banner);
			}

			public override void Uninstall()
			{
				World.Elements.Clear();
			}

			public override void Tick(Keyboard keyboard)
			{
				if(keyboard.Buffer.Count > 0)
					World.PostMessage(new GenericMessage { Name = Messages.ShowMenu });
			}
		}


		public class WinLevel : GenericLevel
		{
			public WinLevel(GenericWorld world) : base(world)
			{
				Name = "win";
			}

			public override void Install()
			{
				var banner = new Element(new Point(World.Size.Width / 2 - 29, 10), new Size(76, 7));
				banner.Load(Path.Combine(Environment.CurrentDirectory, @"resources\win.txt"), Point.Empty, ConsoleColor.DarkGreen);
				//
				World.Elements.Add(banner);
			}

			public override void Uninstall()
			{
				World.Elements.Clear();
			}

			public override void Tick(Keyboard keyboard)
			{
				if(keyboard.Buffer.Count > 0)
					World.PostMessage(new GenericMessage { Name = Messages.ShowMenu });
			}
		}


		public class WorldConfig
		{
			public int Players;
		}


		public class Food : Element
		{
			public int Weight;
			public bool Eated;


			public Food(Point location, int weight = 3) : base(location, new Size(1, 1))
			{
				Weight = weight;
			}


			protected override void UpdateCore()
			{
				SetBrick(Point.Empty, Brick.From(ColorConsole.CharMap['♣'], ConsoleColor.Green));
			}
		}

	}
}
