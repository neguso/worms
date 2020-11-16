using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Game.Worms
{
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
			Text(Point.Empty, "Level " + world.Level.Name);
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



	public class Food : Element
	{
		public int Length { get; set; }
		public int Speed { get; set; }
		public bool Eated { get; set; }


		public Food(Point location, int weight = 3, int speed = 10) : base(location, new Size(1, 1))
		{
			Length = weight;
			Speed = speed;
		}


		protected override void UpdateCore()
		{
			SetBrick(Point.Empty, Brick.From(ColorConsole.CharMap['♣'], ConsoleColor.Green));
		}
	}
}
