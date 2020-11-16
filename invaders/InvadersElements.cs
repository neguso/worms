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

		public override List<Point> GetBody()
		{
			var list = new List<Point>();
			for(int x = 0; x < buffer.GetLength(0); x++)
				for(int y = 0; y < buffer.GetLength(1); y++)
					if(buffer[x, y] != null)
						list.Add(new Point(Location.X + x, Location.Y + y));
			return list;
		}
	}



	public class DefenderShip : Element
	{
		protected List<Missile> missiles;
		protected int fireInterval;
		protected DateTime lastFired;

		public Size Range { get; private set; }


		public DefenderShip(Player player, Point location, Size range) : base(location, new Size(7, 2))
		{
			Players.Add(player);
			Range = range;
			missiles = new List<Missile>();
			Load(@"invaders\resources\defender1.txt", Point.Empty);
			UpdateInterval = 50;
			fireInterval = 300;
			lastFired = DateTime.MinValue;
		}


		public void Move(int delta)
		{
			if(Location.X + delta < 0 || Location.X + delta + 7 > Range.Width) return;
			Location.X += delta;
		}

		public void Fire()
		{
			var now = DateTime.Now;
			var duration = (now - lastFired).TotalMilliseconds;
			if(duration < fireInterval) return;

			missiles.Add(new Missile(new Point(Location.X + 3, Location.Y - 1), -1, Range.Height));
			lastFired = now;
		}

		public List<Missile> GetMissiles()
		{
			var list = missiles.ToList();
			missiles.Clear();
			return list;
		}

		protected override void UpdateCore()
		{
			// commands containts all pressed keys

			if(Commands.Contains(Command.Left))
				Move(-1);
			if(Commands.Contains(Command.Right))
				Move(+1);
			if(Commands.Contains(Command.Fire))
				Fire();
			Commands.Clear();
		}
	}



	public abstract class InvaderShip : Element
	{
		protected List<Missile> bombs;

		public ShipStatus Status { get; protected set; }


		public InvaderShip(Point location, Size size) : base(location, size)
		{
			bombs = new List<Missile>();

			Status = ShipStatus.Normal;
			UpdateInterval = 50;
		}


		public void Move(Size delta)
		{
			Location.X += delta.Width;
			Location.Y += delta.Height;
		}

		public void Fire()
		{
			//bombs.Add(new Bomb(new Point(Location.X + 3, Location.Y - 1), -1, Range.Height));
		}


		public enum ShipStatus
		{
			Normal,
			Alerted,
			Exploding
		}
	}



	public class InvaderShipOne : InvaderShip
	{
		public InvaderShipOne(Point location) : base(location, new Size(10, 5))
		{
			Load(@"invaders\resources\alien1.txt", Point.Empty);
		}
	}


	public class InvaderShipTwo : InvaderShip
	{
		public InvaderShipTwo(Point location) : base(location, new Size(8, 4))
		{
			Load(@"invaders\resources\alien2.txt", Point.Empty);
		}
	}



	public class Missile : Element
	{
		protected char[] explosion = new char[] { '∙', '☼' };


		public int Direction { get; private set; }
		public int Range { get; private set; }
		public MissileState State { get; protected set; }


		public Missile(Point location, int direction, int range) : base(location, new Size(1, 1))
		{
			State = MissileState.Lauched;
			Direction = direction;
			Range = range;

			UpdateInterval = 50;

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



	public class Bomb : Missile
	{
		public Bomb(Point location, int range) : base(location, +1, range)
		{
			SetBrick(Point.Empty, Brick.From('¡', ConsoleColor.Red));
		}
	}



	public class Barrier : Element
	{
		private readonly char[] stages = new char[] {'\xb0', '\xb1', '\xb2', '\xdb', };

		public Barrier(Point location) : base(location, new Size(16, 2))
		{
			Load(@"invaders\resources\barrier.txt", Point.Empty);
		}


		public override List<Point> GetBody()
		{
			var list = new List<Point>();
			for(int x = 0; x < buffer.GetLength(0); x++)
				for(int y = 0; y < buffer.GetLength(1); y++)
				{
					var brick = buffer[x, y];
					if(brick != null)
						list.Add(new Point(Location.X + x, Location.Y + y));
				}
			return list;
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
