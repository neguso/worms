using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Game
{
	public abstract class GenericWorld
	{
		public Size Size { get; private set; }
		public List<Player> Players { get; private set; }
		public List<Element> Elements { get; private set; }


		public GenericWorld(Size size)
		{
			Size = size;
			Players = new List<Player>();
			Elements = new List<Element>();
		}


		private string _status = "new";
		public string Status
		{
			get => _status;
			set
			{
				if(value != _status)
				{
					var old = _status;
					_status = value;
					OnStateChange(old, _status);
				}
			}
		}

		protected abstract void OnStateChange(string oldStatus, string newStatus);

		public virtual bool Alive => Players.Any(p => p.Lives > 0);

		public virtual bool Completed => false; //TODO: replace with status

		public virtual void Tick(Keyboard keyboard)
		{
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

		public virtual void Render(Frame frame)
		{
			Elements.Sort((a, b) => a.ZIndex < b.ZIndex ? -1 : (a.ZIndex > b.ZIndex ? 1 : 0));
			Elements.ForEach(element => element.Draw(frame));
		}
	}



	public class WormsWorld : GenericWorld
	{
		public WorldConfig Configuration { get; private set; }


		public WormsWorld(Size size) : base(size)
		{
			Configuration = new WorldConfig();
		}


		public GenericLevel Level { get; protected set; }


		protected override void OnStateChange(string oldStatus, string newStatus)
		{
			switch(Status)
			{
				case "intro": OnIntro(); break;
				case "menu": OnMenu(); break;
				case "level1": OnLevel(1); break;
				case "level2": OnLevel(2); break;
				case "level3": OnLevel(3); break;
				case "level4": OnWin(); break;
			}
		}

		public override void Tick(Keyboard keyboard)
		{
			if(Status == "new")
				Status = "intro";

			base.Tick(keyboard);
			if(Level != null) Level.Tick(keyboard);
		}


		protected void OnIntro()
		{
			if(Level != null)
				Level.Uninstall();

			Level = new IntroLevel(this);
			Level.Install();
		}

		protected void OnMenu()
		{
			if(Level != null)
				Level.Uninstall();

			Level = new MenuLevel(this);
			Level.Install();
		}

		protected void OnLevel(int level)
		{
			if(Level != null)
				Level.Uninstall();

			Level = new WorldLevel(this, level, new WorldLevel.LevelConfig(@"resources\level" + level));
			Level.Install();
		}

		protected void OnLost()
		{

		}

		protected void OnWin()
		{

		}


		[Obsolete]
		protected void Setup(int players)
		{
			//Levels = new List<WorldLevel>();
			//CurrentLevel = -1;

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
			Players.Add(player1);
			//
			var score1 = new ScoreBox(player1, new Point(0, 0), player1.Name.Length + 15);
			Elements.Add(score1);

			if(players > 1)
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
				Players.Add(player2);
				//
				var score2 = new ScoreBox(player2, new Point(Size.Width - 23, 0), player2.Name.Length + 15);
				Elements.Add(score2);
			}
			if(players > 2)
			{
				//TODO
			}

			var level = new LevelBox(this, new Point(Size.Width / 2 - 4, 0), 7);
			Elements.Add(level);


			// levels
			//Levels.Add(new WorldLevel(this, 1, new WorldLevel.LevelConfig(@"resources\level1")));
			//Levels.Add(new WorldLevel(this, 2, new WorldLevel.LevelConfig(@"resources\level2")));
			//Levels.Add(new WorldLevel(this, 3, new WorldLevel.LevelConfig(@"resources\level3")));

			NextLevel();
		}

		[Obsolete]
		public void NextLevel()
		{
			if(Level != null)
				Level.Uninstall();

			//CurrentLevel++;

			if(Level != null)
				Level.Install();
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
			}

			public override void Uninstall()
			{
				World.Elements.Clear();
			}

			public override void Tick(Keyboard keyboard)
			{
				if(keyboard.Buffer.Count > 0)
					World.Status = "menu";
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
					World.Status = null;
				else if(host.Action == WormsHost.MenuAction.NewGame1)
				{
					((WormsWorld)World).Configuration.Players = 1;
					World.Status = "level1";
				}
				else if(host.Action == WormsHost.MenuAction.NewGame2)
				{
					((WormsWorld)World).Configuration.Players = 2;
					World.Status = "level1";
				}
			}
		}


		public class WorldLevel : GenericLevel
		{
			protected int Level;
			protected LevelConfig Config;

			protected Arena Arena;
			protected List<Worm> Worms;
			protected int FoodStock;
			protected Food Food;


			public WorldLevel(GenericWorld world, int level, LevelConfig config) : base(world)
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
					//TODO
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
				foreach(var worm in Worms.Where(w => w.Enabled))
				{
					if(worm.Collisions(Food).Count > 0)
					{
						// if worm collide with food
						worm.Player.Score += worm.Body.Count;
						if(FoodStock == 0)
							World.Status = "level" + Level; xxxxxxxxx need to send a message that is processed after tick
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

						// if worm collide with himself
						//TODO:

						// if worm colide with other worms
						//TODO:

					}
				}
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

		public class WorldConfig
		{
			public int Players;
		}





		public class _WorldLevel
		{
			public WormsWorld World { get; private set; }
			public int Level { get; private set; }
			public LevelConfig Config;


			protected Arena Arena;
			protected List<Worm> Worms;
			protected int FoodStock;
			protected Food Food;


			public _WorldLevel(WormsWorld world, int level, LevelConfig config)
			{
				World = world;
				Level = level;
				Config = config;
				FoodStock = config.Food + 1;
				Worms = new List<Worm>();
			}


			public bool Finished { get { return FoodStock == 0; } }

			public void Install()
			{
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

			public void Uninstall()
			{
				World.Elements.Remove(Arena);

				foreach(var worm in Worms)
					World.Elements.Remove(worm);
				Worms.Clear();

				World.Elements.Remove(Food);
				Food = null;
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

			public virtual void Tick()
			{
				foreach(var worm in Worms.Where(w => w.Enabled))
				{
					if(worm.Collisions(Food).Count > 0)
					{
						// if worm collide with food
						worm.Player.Score += worm.Body.Count;
						worm.Grow += Food.Weight;
						PlaceFood();
					}
					else if(worm.Collisions(Arena).Count > 0)
					{
						// if worm collide with arena
						worm.Player.Lives--;
						if(worm.Player.Lives > 0)
							worm.Reset();
						else
							worm.Enabled = false;

						// if worm collide with himself
						//TODO:

						// if worm colide with other worms
						//TODO:

					}
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
