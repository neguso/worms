using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Game
{
	public class Sprite : Frame
	{
		public Point Location;
		public int ZIndex;
		public bool Visible;
		public bool Enabled;


		public Sprite(Point location, Size size) : base(size)
		{
			Location = location;
			Visible = true;
			Enabled = true;
		}


		public void Draw(Frame frame)
		{
			if(Enabled && Visible)
				frame.Load(this, Location);
		}

		public virtual List<Point> GetBody()
		{
			var list = new List<Point>(buffer.GetLength(0) * buffer.GetLength(1));
			for(int x = 0; x < buffer.GetLength(0); x++)
				for(int y = 0; y < buffer.GetLength(1); y++)
					list.Add(new Point(Location.X + x, Location.Y + y));
			return list;
		}

		public virtual List<Point> Collisions(Element element)
		{
			return element.GetBody().Intersect(GetBody(), PointEqualityComparer.EqualityComparer).ToList();
		}



		private sealed class PointEqualityComparer : IEqualityComparer<Point>
		{
			public bool Equals(Point a, Point b)
			{
				if(ReferenceEquals(a, b)) return true;
				if(ReferenceEquals(a, null)) return false;
				if(ReferenceEquals(b, null)) return false;
				if(a.GetType() != b.GetType()) return false;
				return a.X == b.X && a.Y == b.Y;
			}

			public int GetHashCode([DisallowNull] Point obj)
			{
				return obj.X + obj.Y * 100000;
			}

			private static readonly PointEqualityComparer eqc = new PointEqualityComparer();
			public static PointEqualityComparer EqualityComparer => eqc;
		}
	}



	public class Element : Sprite
	{
		protected Queue<Command> Commands { get; private set; }
		protected DateTime LastUpdated = DateTime.MinValue;

		// players commanding this element
		public List<Player> Players { get; private set; }
		public int UpdateInterval = 100;


		public Element(Point location, Size size) : base(location, size)
		{
			Commands = new Queue<Command>();
			Players = new List<Player>();
		}


		public bool UpdatePending
		{
			get { return (DateTime.Now - LastUpdated).TotalMilliseconds > UpdateInterval; }
		}

		public virtual void Process(Command command)
		{
			if(Enabled)
				Commands.Enqueue(command);
		}

		public virtual void Update()
		{
			if(UpdatePending)
			{
				if(Enabled)
					UpdateCore();
				LastUpdated = DateTime.Now;
			}
		}

		protected virtual void UpdateCore() { }
	}



	public class Slideshow
	{
		public List<Brick[,]> Slides { get; private set; }
		public int Active;


		public Slideshow()
		{
			Slides = new List<Brick[,]>();
			Active = -1;
		}


		// draw active slide into frame
		public void Draw(Frame frame, Point location)
		{
			if(Slides.Count > 0)
				frame.Load(Slides[Active], location);
		}

		// advance to next slide
		public void Next()
		{
			if(Slides.Count > 0)
			{
				Active++;
				if(Active >= Slides.Count)
					Active = 0;
			}
		}
	}



	public abstract class AnimatedElement : Element
	{
		//Slideshow

		public AnimatedElement(Point location, Size size) : base(location, size)
		{

		}
	}



	public class ScrollingText : Element
	{
		protected Queue<char> queue;

		public ScrollingText(string label, Point location, int width) : base(location, new Size(width, 1))
		{
			Label = label;
			Step = 1;
			UpdateInterval = 0;

			queue = new Queue<char>(label.ToCharArray());
			while(queue.Count < Size.Width)
				queue.Enqueue(' ');
		}


		public string Label { get; set; }
		public int Step { get; set; }


		protected override void UpdateCore()
		{
			var text = new String(queue.ToArray());
			Text(Point.Empty, text.Substring(0, Math.Min(text.Length, Size.Width)));

			// shift text
			for(int i = 0; i < Step; i++)
				queue.Enqueue(queue.Dequeue());
		}
	}



	public class ScoreBox : Element
	{
		protected Player player;

		public ScoreBox(Player player, Point location, int width) : base(location, new Size(width, 1))
		{
			this.player = player;
		}

		protected override void UpdateCore()
		{
			Text(Point.Empty, player.Name, player.Color);
			var lives = "";
			for(int i = 0; i < player.Lives; i++)
				lives += ColorConsole.CharMap['♥'];
			Text(new Point(player.Name.Length + 1, 0), lives, ConsoleColor.Red);
			var deaths = "";
			for(int i = player.Lives; i < 5; i++)
				deaths += ColorConsole.CharMap['♥'];
			Text(new Point(player.Name.Length + 1 + player.Lives, 0), deaths, ConsoleColor.DarkGray);
			Text(new Point(player.Name.Length + 7, 0), ColorConsole.CharMap['►'] + " " + player.Score.ToString().PadLeft(4, '0'), player.Color);
		}
	}


	public class LevelBox : Element
	{
		protected WormsWorld world;

		public LevelBox(WormsWorld world, Point location, int width) : base(location, new Size(width, 1))
		{
			this.world = world;
		}

		protected override void UpdateCore()
		{
			Text(Point.Empty, "Level " + world.Level.Level);
		}
	}



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



	public class Worm : Element
	{
		public Point StartLocation;
		public MovingDirection StartDirection;
		public MovingDirection Direction;
		public List<Point> Body { get; private set; }
		public int Grow;


		public Worm(Player player, Size size, Point start, MovingDirection direction) : base(Point.Empty, size)
		{
			Players.Add(player);
			StartLocation = start;
			Direction = StartDirection = direction;
			Body = new List<Point>() { StartLocation };
			Grow = 2;
		}


		public Player Player => Players[0];


		public void Reset()
		{
			Body.Clear();
			Body.Add(StartLocation);
			Direction = StartDirection;
			Grow = 2;
		}

		public override List<Point> GetBody()
		{
			return Body.Select(p => new Point(Location.X + p.X, Location.Y + p.Y)).ToList();
		}

		public void Move()
		{
			var head = Body[0];
			var tail = Body[Body.Count - 1];

			if(Grow > 0)
				Grow--;
			else
				Body.Remove(tail);

			switch(Direction)
			{
				case MovingDirection.Left: Body.Insert(0, new Point(head.X - 1, head.Y)); break;
				case MovingDirection.Right: Body.Insert(0, new Point(head.X + 1, head.Y)); break;
				case MovingDirection.Up: Body.Insert(0, new Point(head.X, head.Y - 1)); break;
				case MovingDirection.Down: Body.Insert(0, new Point(head.X, head.Y + 1)); break;
			}
		}

		public void Steer(MovingDirection direction)
		{
			switch(direction)
			{
				case MovingDirection.Left: if(Direction != MovingDirection.Right) Direction = direction; break;
				case MovingDirection.Right: if(Direction != MovingDirection.Left) Direction = direction; break;
				case MovingDirection.Up: if(Direction != MovingDirection.Down) Direction = direction; break;
				case MovingDirection.Down: if(Direction != MovingDirection.Up) Direction = direction; break;
			}
		}

		protected override void UpdateCore()
		{
			if(Commands.Count > 0)
			{
				// get next command
				var command = Commands.Dequeue();

				if(command == Command.Left)
					Steer(MovingDirection.Left);
				else if(command == Command.Right)
					Steer(MovingDirection.Right);
				else if(command == Command.Up)
					Steer(MovingDirection.Up);
				else if(command == Command.Down)
					Steer(MovingDirection.Down);
			}

			Move();

			Clear();
			for(int i = 0; i < Body.Count; i++)
				SetBrick(Body[i], Brick.From(ColorConsole.CharMap['▓'], Player.Color));
		}
	}


	public enum MovingDirection
	{
		Left,
		Right,
		Up,
		Down
	}
}
