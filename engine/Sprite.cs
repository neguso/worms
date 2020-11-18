﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;

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
			UpdateTimer = new Timer(100);
		}


		/// <summary>
		/// Players controlling this element.
		/// </summary>
		public List<Player> Players { get; private set; }

		/// <summary>
		/// Timer used for update interval.
		/// </summary>
		public Timer UpdateTimer { get; protected set; }



		public virtual void Process(Command command)
		{
			if(Enabled)
				Commands.Enqueue(command);
		}

		public virtual void Update()
		{
			if(!Enabled) return;

			if(UpdateTimer.Passed)
			{
				UpdateCore();
				UpdateTimer.Reset();
			}
		}

		protected virtual void UpdateCore() { }
	}
}
