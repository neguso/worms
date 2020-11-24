using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Game.Invaders
{
	public class Arena : Element
	{
		public Arena(string path, Point location, Size size) : base(location, size)
		{
			Load(Path.Combine(path, "arena.txt"), Point.Empty);
		}
	}



	public class DefenderShip : Element
	{
		protected List<Missile> missiles;
		protected Timer fireTimer;
		protected Timer explodingTimer;
		protected readonly ConsoleColor[] exploding = new ConsoleColor[] { ConsoleColor.Red, ConsoleColor.DarkRed, ConsoleColor.DarkGray };
		protected int explodingIndex = 0;


		public DefenderShip(Player player, Point location, Size range) : base(location, new Size(7, 2))
		{
			InputProcess = InputProcessMode.KeyDown;
			Players.Add(player);
			Range = range;
			missiles = new List<Missile>();
			Load(@"invaders\resources\defender1.txt", Point.Empty);
			UpdateTimer.Reset(50);
			fireTimer = new Timer(300);
			explodingTimer = new Timer(200);
		}


		public ShipStatus Status { get; protected set; }

		public Size Range { get; private set; }

		public Player Player => Players[0];


		public void Move(int delta)
		{
			if(Location.X + delta < 0 || Location.X + delta + 7 > Range.Width) return;
			Location.X += delta;
		}

		public void Fire()
		{
			if(fireTimer.Passed)
			{
				missiles.Add(new Missile(new Point(Location.X + 3, Location.Y - 1), Range.Height - 3));
				fireTimer.Reset();
			}
		}

		public List<Missile> GetMissiles()
		{
			var list = missiles.ToList();
			missiles.Clear();
			return list;
		}

		public void Hit(Point location)
		{
			Status = ShipStatus.Exploding;
			explodingTimer.Reset();
		}

		protected override void UpdateCore()
		{
			// commands containts all pressed keys
			switch(Status)
			{
				case ShipStatus.Normal:
					if(Commands.Contains(Command.Left))
						Move(-1);
					if(Commands.Contains(Command.Right))
						Move(+1);
					if(Commands.Contains(Command.Fire))
						Fire();
					Commands.Clear();
					break;

				case ShipStatus.Exploding:
					if(explodingTimer.Count < 10)
					{
						// alternate colors
						Scan((brick, x, y) => { brick.ForeColor = explodingTimer.Count % 2 == 0 ? ConsoleColor.White : ConsoleColor.DarkGray ; });
					}
					else
					{
						Status = ShipStatus.Normal;
						Scan((brick, x, y) => { brick.ForeColor = ConsoleColor.White; });
					}
					break;
			}
		}


		public enum ShipStatus
		{
			Normal,
			Exploding
		}
	}



	public abstract class InvaderShip : AnimatedElement
	{
		protected List<Bomb> bombs;
		protected ConsoleColor[] exploding = new ConsoleColor[] {ConsoleColor.Red, ConsoleColor.DarkRed, ConsoleColor.DarkGray };
		protected Timer explodingTimer;


		public InvaderShip(Point location, Size size, Size range) : base(location, size)
		{
			ZIndex = 1; // draw invaders on top of bariers
			bombs = new List<Bomb>();
			explodingTimer = new Timer(UpdateTimer.Interval);
			Range = range;
			Status = ShipStatus.Normal;
		}


		public ShipStatus Status { get; protected set; }
		public Size Range { get; private set; }


		public void Move(Size distance)
		{
			Location.X += distance.Width;
			Location.Y += distance.Height;
		}

		public void Fire()
		{
			var range = Range.Height - Location.Y - Size.Height;
			if(range > 0)
			{
				Random rnd = new Random();
				bombs.Add(new Bomb(new Point(Location.X + rnd.Next(Size.Width), Location.Y + Size.Height), range));
			}
		}

		public List<Bomb> GetMissiles()
		{
			var list = bombs.ToList();
			bombs.Clear();
			return list;
		}

		public void Hit(Point location)
		{
			Status = ShipStatus.Exploding;
			explodingTimer.Reset();
		}

		protected override void UpdateCore()
		{
			base.UpdateCore();

			Random rnd = new Random();
			switch(Status)
			{
				case ShipStatus.Normal:
					if(rnd.Next(100) > 98) Fire();
					break;
				case ShipStatus.Alerted:
					if(rnd.Next(100) > 90) Fire();
					break;
				case ShipStatus.Exploding:
					if(explodingTimer.Count < 3)
						Scan((brick, x, y) => { brick.ForeColor = exploding[explodingTimer.Count % 3]; });
					else
						Status = ShipStatus.Dead;
					break;
			}
		}


		public enum ShipStatus
		{
			Normal,
			Alerted,
			Exploding,
			Dead
		}
	}



	public class InvaderShipSquid : InvaderShip
	{
		public InvaderShipSquid(Point location, Size range) : base(location, new Size(12, 5), range)
		{
			timer.Reset(1250);
			Show.Load(@"invaders\resources\alien_squid.txt", Size, ConsoleColor.Green);
		}
	}



	public class InvaderShipCrab : InvaderShip
	{
		public InvaderShipCrab(Point location, Size range) : base(location, new Size(10, 5), range)
		{
			timer.Reset(750);
			Show.Load(@"invaders\resources\alien_crab.txt", Size, ConsoleColor.Cyan);
		}
	}



	public class InvaderShipOctopus : InvaderShip
	{
		public InvaderShipOctopus(Point location, Size range) : base(location, new Size(8, 4), range)
		{
			timer.Reset(250);
			Show.Load(@"invaders\resources\alien_octopus.txt", Size, ConsoleColor.Magenta);
		}
	}



	public class InvaderShipUFO : InvaderShip
	{
		private int speed;
		private Timer movingTimer;

		public InvaderShipUFO(Point location, Size range) : base(location, new Size(16, 3), range)
		{
			timer.Reset(50);
			Show.Load(@"invaders\resources\alien_ufo.txt", Size, ConsoleColor.Yellow);

			speed = 0;
			movingTimer = new Timer(5000);
			Status = ShipStatus.Alerted;
		}


		protected override void UpdateCore()
		{
			base.UpdateCore();

			if(speed == 0 && movingTimer.Passed)
				speed = Location.X < 0 ? 1 : -1;

			Move(new Size(speed, 0));

			if(speed != 0 && (Location.X <= -Size.Width || Location.X >= Range.Width))
			{
				speed = 0;
				Random rnd = new Random();
				movingTimer = new Timer(3000 + rnd.Next(10000));
			}
		}
	}



	public class Projectile : Element
	{
		protected char[] explosion = new char[] { '∙', '☼' };

		public int Direction { get; private set; }
		public int Range { get; private set; }
		public MissileState State { get; protected set; }


		public Projectile(Point location, int direction, int range) : base(location, new Size(1, 1))
		{
			ZIndex = 2; // draw projectiles on top of invaders
			State = MissileState.Lauched;
			Direction = direction;
			Range = range;

			UpdateTimer.Reset(50);

			SetBrick(Point.Empty, Brick.From('^', ConsoleColor.Red));
		}


		public void Explode()
		{
			State = MissileState.Exploding;
		}

		protected override void UpdateCore()
		{
			switch(State)
			{
				case MissileState.Lauched:
					if(Range > 0)
					{
						Location.Y += Direction;
						Range--;
						if(Range == 0)
							State = MissileState.OutOfRange;
					}
					break;

				case MissileState.Exploding:
					if(explosion.Length > 0)
					{
						buffer[0, 0] = Brick.From(ColorConsole.CharMap[explosion[explosion.Length - 1]]);
						Array.Resize(ref explosion, explosion.Length - 1);
					}
					else
						State = MissileState.OutOfRange;
					break;
			}
		}


		public enum MissileState
		{
			Lauched,
			Exploding,
			OutOfRange
		}
	}



	public class Missile : Projectile
	{
		public Missile(Point location, int range) : base(location, -1, range)
		{
			SetBrick(Point.Empty, Brick.From('!', ConsoleColor.Blue));
		}
	}



	public class Bomb : Projectile
	{
		public Bomb(Point location, int range) : base(location, +1, range)
		{
			UpdateTimer.Reset(100);

			SetBrick(Point.Empty, Brick.From(ColorConsole.CharMap['¥'], ConsoleColor.Red));
		}
	}



	public class Barrier : Element
	{
		private readonly char[] stages = new char[] {'\xb0', '\xb1', '\xb2', '\xdb', };

		public Barrier(Point location) : base(location, new Size(15, 2))
		{
			Load(@"invaders\resources\barrier.txt", Point.Empty);
		}


		public void Hit(Point location)
		{
			var brick = buffer[location.X, location.Y];
			if(brick == null) return;
			var stage = Array.IndexOf(stages, brick.Char);
			if(stage == 0)
				buffer[location.X, location.Y] = null;
			else if(stage > 0)
				brick.Char = stages[stage - 1];
		}
	}
}
