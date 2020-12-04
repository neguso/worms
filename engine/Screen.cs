using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace Game
{
	/// <summary>
	/// Represent the screen where frames are drawn.
	/// </summary>
	public class Screen
	{
		private DateTime lastRefresh = DateTime.MinValue;


		private Screen() { }


		public Size Size => ColorConsole.Size;


		public void WaitRefresh(double interval = 16.66667) // 60Hz
		{
			var elapsed = (DateTime.Now - lastRefresh).TotalMilliseconds;
			if(elapsed < interval)
				System.Threading.Thread.Sleep((int)(interval - elapsed));
			lastRefresh = DateTime.Now;
		}

		public void Draw(Frame frame, Point origin)
		{
			//var w = new Stopwatch();
			//w.Start();

			var sb = new StringBuilder();

			var line = int.MinValue;
			var fcolor = ConsoleColor.White;
			var bcolor = ConsoleColor.Black;

			frame.Scan((brick, x, y) =>
			{
				if(origin.X + x < 0 || origin.X + x > Size.Width - 1 || origin.Y + y < 0 || origin.Y + y > Size.Height - 1)
					return;

				if(line != y)
					sb.Append(ColorConsole.Escape.Location(origin.X + x, origin.Y + (line = y)));

				if(brick == null)
				{
					if(fcolor != ConsoleColor.White || bcolor != ConsoleColor.Black)
						sb.Append(ColorConsole.Escape.Color(fcolor = ConsoleColor.White, bcolor = ConsoleColor.Black));
					sb.Append(' ');
				}
				else
				{
					if(fcolor != brick.ForeColor || bcolor != brick.BackColor)
						sb.Append(ColorConsole.Escape.Color(fcolor = brick.ForeColor, bcolor = brick.BackColor));

					sb.Append(brick.Char);
				}
			}, false);

			ColorConsole.Write(sb.ToString());

			//w.Stop();
			//var duration = w.ElapsedMilliseconds;
		}

		public void Draw(Frame frame)
		{
			Draw(frame, Point.Empty);
		}


		private static Screen _default = new Screen();
		public static Screen Default
		{
			get => _default;
		}
	}
}
