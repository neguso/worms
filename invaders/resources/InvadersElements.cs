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
		public Size Range { get; private set; }


		public DefenderShip(Player player, Point location, Size range) : base(location, new Size(7, 2))
		{
			Players.Add(player);
			Range = range;
			Load(@"invaders\resources\defender1.txt", Point.Empty);
		}


		public void Move(int delta)
		{
			if(Location.X + delta < 0 || Location.X + delta + 7 > Range.Width) return;
			Location.X += delta;
		}

		public void Fire()
		{

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

}
