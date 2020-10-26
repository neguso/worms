using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game
{
	public abstract class Sprite : Frame
	{
		public Point Location;
		public int ZIndex;

		public Sprite(Point location, Size size) : base(size)
		{
			Location = location;
		}


		public void Draw(Frame frame)
		{
			Draw(frame, Location);
		}
	}



	public abstract class Element : Sprite
	{
		public List<Player> Players { get; private set; }
		public Queue<Command> Commands { get; private set; }

		public Element(Point location, Size size) : base(location, size)
		{
			Players = new List<Player>();
			Commands = new Queue<Command>();
		}


		protected DateTime LastUpdated = DateTime.MinValue;


		public int UpdateInterval = 100;

		public bool UpdatePending
		{
			get { return (DateTime.Now - LastUpdated).TotalMilliseconds > UpdateInterval; }
		}


		public virtual void Process(Command command)
		{
			this.Commands.Enqueue(command);
		}

		public void Update()
		{
			if(UpdatePending)
			{
				UpdateCore(Commands.Count > 0 ? Commands.Dequeue() : null);
				LastUpdated = DateTime.Now;
			}
		}

		protected abstract void UpdateCore(Command command);
	}



	public class TestElement : Element
	{
		public TestElement() : base(new Point(0, 0), new Size(5, 2))
		{
		}


		protected override void UpdateCore(Command command)
		{
			// react to time and commands

			var rnd = new Random();
			Area(Location, Size, Brick.From((char)((int)'a' + rnd.Next(25))));
		}
	}

}
