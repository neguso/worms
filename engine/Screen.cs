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

		public bool WaitForRefresh = true;


		public Screen()
		{
			Console.CursorVisible = false;
		}


		public Size Size
		{
			get { return ColorConsole.Size; }
		}


		public void WaitRefresh(double duration = 16.66667) // 60Hz
		{
			if(WaitForRefresh)
			{
				double elapsed = (DateTime.Now - lastRefresh).TotalMilliseconds;
				if(elapsed < duration)
					System.Threading.Thread.Sleep((int)(duration - elapsed));
				lastRefresh = DateTime.Now;
			}
		}

		public void Draw(Frame frame, Point origin)
		{
			//var w = new Stopwatch();
			//w.Start();

			var sb = new StringBuilder();

			var line = int.MinValue;
			var fcolor = ConsoleColor.White;
			var bcolor = ConsoleColor.Black;

			frame.Scan((brick, point) =>
			{
				if(origin.X + point.X < 0 || origin.X + point.X > Size.Width - 1 || origin.Y + point.Y < 0 || origin.Y + point.Y > Size.Height - 1)
					return;

				if(line != point.Y)
					sb.Append(ColorConsole.Escape.Location(origin.X + point.X, origin.Y + (line = point.Y)));

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
			});

			ColorConsole.Write(sb.ToString());

			//w.Stop();
			//var duration = w.ElapsedMilliseconds;
		}

		public void Draw(Frame frame)
		{
			Draw(frame, Point.Empty);
		}
	}
}
