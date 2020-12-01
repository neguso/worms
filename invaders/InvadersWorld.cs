using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Game.Invaders
{
	public class InvadersWorld : GameWorld
	{
		private const string hiscores = "invaders.xml";

		public InvadersWorld(Size size) : base(size)
		{
			LoadScores(hiscores);

			PostMessage(new WorldMessage { Name = MessageName.ShowIntro });
		}


		public int TotalPlayers { get; private set; }
		public int CurrentPlayer { get; private set; }
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
				
				case MessageName.ReadyGame:
					TotalPlayers = ((ReadyGameMessage)message).Players;
					CurrentPlayer = 1;
					LoadLevel(new ReadyLevel(this, CurrentPlayer));
					break;
				
				case MessageName.StartGame:
					LoadLevel(new GameLevel(this, 1, new GameLevel.LevelConfig(@"invaders\resources\levels")));
					break;
				
				case MessageName.ShowHelp: LoadLevel(new HelpLevel(this)); break;
				//case MessageName.NextLevel: NextGameLevel(); break;
				
				case MessageName.NextPlayer:
					if(CurrentPlayer < TotalPlayers)
					{
						CurrentPlayer++;
						LoadLevel(new ReadyLevel(this, CurrentPlayer));
					}
					else
						LoadLevel(new MenuLevel(this));
					break;

				case MessageName.GameOver:
					HighScores.Record(((GameFinished)message).Player, ((GameFinished)message).Score);
					LoadLevel(new LostLevel(this, CurrentPlayer, ((GameFinished)message).Score));
					break;
				
				case MessageName.GameCompleted:
					HighScores.Record(((GameFinished)message).Player, ((GameFinished)message).Score);
					LoadLevel(new WinLevel(this, CurrentPlayer, ((GameFinished)message).Score));
					break;
				
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

		//protected void NextGameLevel()
		//{
		//	var level = Level as InvadersGameLevel;
		//	if(level != null)
		//	{
		//		if(level.Index < 3)
		//			LoadLevel(new InvadersGameLevel(this, level.Index + 1, new InvadersGameLevel.LevelConfig(@"invaders\resources\levels")));
		//		else
		//			LoadLevel(new WinLevel(this, CurrentPlayer));
		//	}
		//}

		protected void Quit()
		{
			Level.Uninstall();
			Level = null;
			Active = false;

			SaveScores(hiscores);
		}
	}



	public class IntroLevel : WorldLevel
	{
		public IntroLevel(GameWorld world) : base(world)
		{ }

		public override void Install()
		{
			var logo = new Element(new Point(2, 5), new Size(World.Size.Width, 18));
			logo.Load(GetType().Assembly.GetManifestResourceStream("worms.invaders.resources.banner.txt"), Point.Empty, ConsoleColor.DarkGreen);
			logo.Scan((brick, x, y) => {
				if(brick.Char == '#')
					brick.ForeColor = ConsoleColor.Green;
			});
			//
			World.Elements.Add(logo);

			World.Elements.Add(new ScrollingText("Protect the Earth from space invaders  \x4", new Point(31, 20), 42) { Foreground = ConsoleColor.Green });

			World.Elements.Add(new StaticText("Press any key to start", new Point(39, 32)));
		}

		public override void Uninstall()
		{
			World.Elements.Clear();
		}

		public override void Tick(IEnumerable<ConsoleKey> keys)
		{
			if(keys.Any())
				World.PostMessage(new WorldMessage { Name = MessageName.ShowMenu });
		}
	}



	public class MenuLevel : WorldLevel
	{
		public MenuLevel(InvadersWorld world) : base(world)
		{
		}


		public override void Install()
		{
			var host = new InvadersHost(World)
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

			var logo = new Element(new Point(2, 5), new Size(World.Size.Width, 18));
			logo.Load(GetType().Assembly.GetManifestResourceStream("worms.invaders.resources.banner.txt"), Point.Empty, ConsoleColor.DarkGreen);
			//
			World.Elements.Add(logo);

			var menu = new InvadersMenu(new Point(20, 28));
			menu.Players.Add(host);
			//
			World.Elements.Add(menu);

			var board = new ScoreBoard(new Point(60, 25), World.HighScores);
			World.Elements.Add(board);
		}

		public override void Uninstall()
		{
			World.Players.Clear();
			World.Elements.Clear();
		}

		public override void Tick(IEnumerable<ConsoleKey> keys)
		{
			var host = World.Players[0] as InvadersHost;

			if(host.Action == InvadersHost.MenuAction.Quit)
				World.PostMessage(new WorldMessage { Name = MessageName.Quit });
			else if(host.Action == InvadersHost.MenuAction.Help)
				World.PostMessage(new WorldMessage { Name = MessageName.ShowHelp });
			else if(host.Action == InvadersHost.MenuAction.NewGame1)
				World.PostMessage(new ReadyGameMessage { Name = MessageName.ReadyGame, Players = 1 });
			else if(host.Action == InvadersHost.MenuAction.NewGame2)
				World.PostMessage(new ReadyGameMessage { Name = MessageName.ReadyGame, Players = 2 });
		}
	}



	public class HelpLevel : WorldLevel
	{
		public HelpLevel(InvadersWorld world) : base(world)
		{ }


		public override void Install()
		{
			World.Elements.Add(new StaticText("S P A C E   I N V A D E R S", new Point(35, 5)));

			var ufo = new InvaderShipUFO(new Point(30, 11), World.Size, false);
			World.Elements.Add(ufo);
			World.Elements.Add(new StaticText($".......... {ufo.Score} POINTS", new Point(47, 12)));

			var squid = new InvaderShipSquid(new Point(32, 15), World.Size);
			World.Elements.Add(squid);
			World.Elements.Add(new StaticText($"............. {squid.Score} POINTS", new Point(45, 17)));

			var crab = new InvaderShipCrab(new Point(33, 20), World.Size);
			World.Elements.Add(crab);
			World.Elements.Add(new StaticText($".............. {crab.Score} POINTS", new Point(44, 22)));

			var octopus = new InvaderShipOctopus(new Point(34, 25), World.Size);
			World.Elements.Add(octopus);
			World.Elements.Add(new StaticText($"............... {octopus.Score} POINTS", new Point(43, 27)));

			World.Elements.Add(new StaticText("Press any key to return to menu.", new Point(35, 35)) { Foreground = ConsoleColor.Gray });
		}

		public override void Uninstall()
		{
			World.Elements.Clear();
		}

		public override void Tick(IEnumerable<ConsoleKey> keys)
		{
			if(keys.Any())
				World.PostMessage(new WorldMessage { Name = MessageName.ShowMenu });
		}
	}



	public class ReadyLevel : WorldLevel
	{
		protected int player;
		protected Timer textTimer;


		public ReadyLevel(GameWorld world, int player) : base(world)
		{
			this.player = player;
			textTimer = new Timer(200);
		}


		private void AnimateText(StaticText text)
		{
			if(text.Foreground == ConsoleColor.White)
				text.Foreground = ConsoleColor.DarkGray;
			else
				text.Foreground = ConsoleColor.White;
		}


		public override void Install()
		{
			World.Elements.Add(new StaticText($"R E A D Y  P L A Y E R  {player}", new Point(38, 17)));
			World.Elements.Add(new StaticText("press any key to start", new Point(39, 32)));
		}

		public override void Uninstall()
		{
			World.Elements.Clear();
		}

		public override void Tick(IEnumerable<ConsoleKey> keys)
		{
			if(keys.Any())
				World.PostMessage(new WorldMessage { Name = MessageName.StartGame });

			if(textTimer.Passed)
			{
				AnimateText(World.Elements[0] as StaticText);
				textTimer.Reset();
			}
		}
	}



	public class GameLevel : WorldLevel
	{
		protected LevelConfig Config;

		protected Arena Arena;
		protected DefenderShip defender;
		protected InvadersController fleet;


		public GameLevel(GameWorld world, int level, LevelConfig config) : base(world)
		{
			Index = level;
			Name = level.ToString();
			Config = config;
		}


		public override void Install()
		{
			// create player
			var player = new Player()
			{
				Index = 1,
				Name = $"Player {((InvadersWorld)World).CurrentPlayer}",
				Color = ConsoleColor.White,
				Lives = 1,
				Score = 0,
				KeyMap = new KeyboardKeyMap[]
				{
					new KeyboardKeyMap(ConsoleKey.LeftArrow, Command.Left),
					new KeyboardKeyMap(ConsoleKey.RightArrow, Command.Right),
					new KeyboardKeyMap(ConsoleKey.Spacebar, Command.Fire)
				}
			};
			World.Players.Add(player);

			// create arena
			Arena = new Arena(Config.DataFolder, new Point(0, 0), new Size(World.Size.Width, World.Size.Height - 2));
			World.Elements.Add(Arena);
			
			// create defender ship
			defender = new DefenderShip(player, new Point(10, 35), Arena.Size);
			World.Elements.Add(defender);

			// create barriers
			for(int b = 0; b < 4; b++)
				World.Elements.Add(new Barrier(new Point(9 + b * 23, 32)));

			// create invaders ships
			var invaders = new List<InvaderShip>();
			//
			for(int row = 0; row < 1; row++)
				for(int col = 0; col < 5; col++)
					invaders.Add(new InvaderShipSquid(new Point(col * 14 + 1, 3 + row * 5), Arena.Size));
			//
			for(int row = 1; row < 2; row++)
				for(int col = 0; col < 6; col++)
					invaders.Add(new InvaderShipCrab(new Point(col * 12, 3 + row * 5), Arena.Size));
			//
			for(int row = 2; row < 4; row++)
				for(int col = 0; col < 7; col++)
					invaders.Add(new InvaderShipOctopus(new Point(col * 10 + 1, 3 + row * 5 + 1), Arena.Size));
			World.Elements.AddRange(invaders);
			//
			World.Elements.Add(new InvaderShipUFO(new Point(-16, 0), Arena.Size));

			// create invaders fleet controller
			fleet = new InvadersController(invaders, Arena.Size);

			// lives
			World.Elements.Add(new LivesBox(player, new Point(2, World.Size.Height - 2)));

			// score
			World.Elements.Add(new ScoreBox(player, new Point(83, World.Size.Height - 2), 20));
		}

		public override void Uninstall()
		{
			World.Players.Clear();
			World.Elements.Clear();
		}

		public override void Tick(IEnumerable<ConsoleKey> keys)
		{
			// check missiles collisions
			foreach(var missile in World.Elements.OfType<Projectile>().Where(m => m.State == Projectile.MissileState.Lauched))
			{
				// with barriers
				foreach(var barrier in World.Elements.OfType<Barrier>())
				{
					var collisions = missile.Collisions(barrier);
					if(collisions.Count > 0)
					{
						missile.Explode();
						barrier.Hit(new Point(collisions[0].X - barrier.Location.X, collisions[0].Y - barrier.Location.Y));
					}
				}

				// with defender
				if(defender.Status != DefenderShip.ShipStatus.Exploding)
				{
					if(missile.GetType() == typeof(Bomb))
					{
						var collisions = missile.Collisions(defender);
						if(collisions.Count > 0)
						{
							missile.Explode();
							defender.Player.Lives--;
							if(defender.Player.Lives > 0)
								defender.Hit(collisions[0]);
							else
								World.PostMessage(new GameFinished { Name = MessageName.GameOver, Player = World.Players[0].Name, Score = World.Players[0].Score });
						}
					}
				}

				// with invaders
				if(missile.GetType() == typeof(Missile))
				{
					foreach(var invader in World.Elements.OfType<InvaderShip>().Where(e => !e.IsDead))
					{
						var collisions = missile.Collisions(invader);
						if(collisions.Count > 0)
						{
							missile.Explode();
							invader.Hit(new Point(collisions[0].X - invader.Location.X, collisions[0].Y - invader.Location.Y));
							World.Players[0].Score += invader.Score;
						}
					}
				}
			}

			// check defender collisons
			//TODO

			// remove out of range missiles
			World.Elements.RemoveAll(e => e is Projectile m && m.State == Projectile.MissileState.OutOfRange);

			// remove dead invaders
			World.Elements.RemoveAll(e => e is InvaderShip i && i.Status == InvaderShip.ShipStatus.Dead);
			fleet.Invaders.RemoveAll(i => i.Status == InvaderShip.ShipStatus.Dead);
			if(!World.Elements.Any(e => e is InvaderShip))
				World.PostMessage(new GameFinished { Name = MessageName.GameCompleted, Player = World.Players[0].Name, Score = World.Players[0].Score });

			// coordinate invaders fleet
			fleet.Move();

			// add missiles and bombs into world
			World.Elements.AddRange(defender.GetMissiles());
			World.Elements.OfType<InvaderShip>().ToList().ForEach(i => World.Elements.AddRange(i.GetMissiles()));

			if(keys.Any(k => k == ConsoleKey.Escape))
				World.PostMessage(new WorldMessage { Name = MessageName.ShowMenu });
		}


		/// <summary>
		/// Control movement of the invaders fleet.
		/// </summary>
		public class InvadersController
		{
			public List<InvaderShip> Invaders { get; private set; }
			public MovingDirection Direction;
			public Timer UpdateTimer;
			public Size Range;


			public InvadersController(List<InvaderShip> invaders, Size range)
			{
				Invaders = invaders;
				Direction = MovingDirection.Right;
				Range = range;
				UpdateTimer = new Timer(750);
			}


			public void Move()
			{
				if(Invaders.Count == 0) return;

				if(UpdateTimer.Passed)
				{
					// calculate moving distance and direction
					var distance = new Size(0, 0);
					var lowest = Invaders.Max(i => i.Location.Y + i.Size.Height);
					switch(Direction)
					{
						case MovingDirection.Left:
							var leftmost = Invaders.Min(i => i.Location.X);
							if(leftmost == 0)
							{
								Direction = MovingDirection.Right;
								distance.Width = +1;
								distance.Height = lowest < Range.Height ? 1 : 0;
							}
							else
								distance.Width = -1;
							break;

						case MovingDirection.Right:
							var righttmost = Invaders.Max(i => i.Location.X + i.Size.Width);
							if(righttmost == Range.Width)
							{
								Direction = MovingDirection.Left;
								distance.Width = -1;
								distance.Height = lowest < Range.Height ? 1 : 0;
							}
							else
								distance.Width = +1;
							break;
					}
					foreach(var invader in Invaders)
						invader.Move(distance);

					UpdateTimer.Reset();
				}
			}


			public enum MovingDirection
			{
				Left,
				Right
			}
		}


		public class LevelConfig
		{
			public string DataFolder;
			public int Crabs;
			public int Octopuses;
			public int UFOs;


			public LevelConfig() { }

			public LevelConfig(string path)
			{
				DataFolder = path;
			}
		}
	}


	
	public class LostLevel : WorldLevel
	{
		protected int player;
		protected int score;
		protected Timer timer;


		public LostLevel(GameWorld world, int player, int score) : base(world)
		{
			this.player = player;
			this.score = score;
			timer = new Timer(1000);
		}


		public override void Install()
		{
			var banner = new Element(new Point(World.Size.Width / 2 - 39, 10), new Size(76, 7));
			banner.Load(Path.Combine(Environment.CurrentDirectory, @"invaders\resources\lose.txt"), Point.Empty, ConsoleColor.DarkRed);
			//
			World.Elements.Add(banner);

			World.Elements.Add(new StaticText($"Player {player} score {score}", new Point(40, 30)));
		}

		public override void Uninstall()
		{
			World.Elements.Clear();
		}

		public override void Tick(IEnumerable<ConsoleKey> keys)
		{
			if(timer.Passed && keys.Any())
				World.PostMessage(new WorldMessage { Name = MessageName.NextPlayer });
		}
	}



	public class WinLevel : WorldLevel
	{
		protected int player;
		protected int score;
		protected Timer timer;


		public WinLevel(GameWorld world, int player, int score) : base(world)
		{
			this.player = player;
			this.score = score;
			timer = new Timer(1000);
		}


		public override void Install()
		{
			var banner = new Element(new Point(World.Size.Width / 2 - 29, 10), new Size(76, 7));
			banner.Load(Path.Combine(Environment.CurrentDirectory, @"invaders\resources\win.txt"), Point.Empty, ConsoleColor.DarkGreen);
			//
			World.Elements.Add(banner);

			World.Elements.Add(new StaticText($"Player {player} score {score}", new Point(40, 30)));
		}

		public override void Uninstall()
		{
			World.Elements.Clear();
		}

		public override void Tick(IEnumerable<ConsoleKey> keys)
		{
			if(timer.Passed &&    keys.Any())
				World.PostMessage(new WorldMessage { Name = MessageName.NextPlayer });
		}
	}



	public static class MessageName
	{
		public const string ShowIntro = "show_intro";
		public const string ShowMenu = "show_menu";
		public const string ReadyGame = "ready_game";
		public const string StartGame = "start_game";
		public const string ShowHelp = "show_help";
		public const string Quit = "quit";
		public const string NextLevel = "next_level";
		public const string NextPlayer = "next_player";
		public const string GameOver = "game_over";
		public const string GameCompleted = "game_completed";
	}

	public class ReadyGameMessage : WorldMessage
	{
		public int Players;
	}

	public class GameFinished : WorldMessage
	{
		public string Player;
		public int Score;
	}
}
