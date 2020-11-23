using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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

		/// <summary>
		/// Retrieve the element body points used for collision detection.
		/// Default implementation return non empty bricks.
		/// </summary>
		public virtual List<Point> GetBody()
		{
			var list = new List<Point>();
			Scan((brick, x, y) => { list.Add(new Point(Location.X + x, Location.Y + y)); });
			return list;
		}

		/// <summary>
		/// Calculate collisions with supplied element.
		/// </summary>
		public virtual List<Point> Collisions(Element element)
		{
			return element.GetBody().Intersect(GetBody(), PointEqualityComparer.EqualityComparer).ToList();
		}



		private sealed class PointEqualityComparer : IEqualityComparer<Point>
		{
			public bool Equals(Point a, Point b)
			{
				if(ReferenceEquals(a, b)) return true;
				if(a == null) return false;
				if(b == null) return false;
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
		

		public Element(Point location, Size size) : base(location, size)
		{
			Commands = new Queue<Command>();
			Players = new List<Player>();
			InputProcess = InputProcessMode.KeyPress;

			UpdateTimer = new Timer(100);
		}


		public string Id { get; set; }
		public InputProcessMode InputProcess { get; protected set; }


		/// <summary>
		/// Players controlling this element.
		/// </summary>
		public List<Player> Players { get; private set; }

		/// <summary>
		/// Timer used for update interval.
		/// </summary>
		public Timer UpdateTimer { get; protected set; }


		/// <summary>
		/// Process players commands. Default implementation store commands in a queue.
		/// </summary>
		public virtual void Process(Command command)
		{
			if(Enabled)
				Commands.Enqueue(command);
		}

		/// <summary>
		/// Update element.
		/// </summary>
		public virtual void Update()
		{
			if(!Enabled) return;

			if(UpdateTimer.Passed)
			{
				UpdateCore();
				UpdateTimer.Reset();
			}
		}

		/// <summary>
		/// Override this in derived classes to update element.
		/// </summary>
		protected virtual void UpdateCore() { }
	}



	public enum InputProcessMode
	{
		KeyPress,
		KeyDown
	}
}
