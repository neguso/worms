using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace Game
{
	public class Screen
	{
		public Screen()
		{
			Console.CursorVisible = false;
		}


		public bool WaitForRefresh = true;

		private DateTime lastRefresh = DateTime.MinValue;

		public void WaitRefresh(int duration = 50)
		{
			if (WaitForRefresh)
			{
				double elapsed = (DateTime.Now - lastRefresh).TotalMilliseconds;
				if (elapsed < duration)
					System.Threading.Thread.Sleep(duration - (int)elapsed);
				lastRefresh = DateTime.Now;
			}
		}

		public Size Size
		{
			get { return new Size(Console.WindowWidth, Console.WindowHeight - 1); }
			set
			{
				Console.BufferWidth = Console.WindowWidth = value.Width;
				Console.BufferHeight = Console.WindowHeight = value.Height + 1;
			}
		}

		public string Title
		{
			get => Console.Title;
			set { Console.Title = value; }
		}

		public ConsoleColor ForegroundColor
		{
			get { return Console.ForegroundColor; }
			set { Console.ForegroundColor = value; }
		}

		public ConsoleColor BackgroundColor
		{
			get { return Console.BackgroundColor; }
			set { Console.BackgroundColor = value; }
		}


		public void Clear()
		{
			Console.Clear();
		}

		public void Draw(Frame frame, Point origin)
		{
			var w = new Stopwatch();
			w.Start();

			var sb = new StringBuilder();

			var line = int.MinValue;
			var fcolor = ConsoleColor.Black;
			var bcolor = ConsoleColor.Black;

			frame.Render((brick, point) =>
			{
				if (origin.X + point.X < 0 || origin.X + point.X >= Console.WindowWidth || origin.Y + point.Y < 0 || origin.Y + point.Y >= Console.WindowHeight - 1)
					return;

				if (line != point.Y)
					sb.Append(Escape.Location(origin.X + point.X, origin.Y + (line = point.Y)));

				if (brick == null || brick == Brick.Empty)
				{
					sb.Append(Escape.MoveRight(1));
				}
				else
				{
					if (fcolor != brick.ForeColor || bcolor != brick.BackColor)
						sb.Append(Escape.Color(fcolor = brick.ForeColor, bcolor = brick.BackColor));

					sb.Append(brick.Char);
				}
			});

			ColorConsole.Write(sb.ToString());

			w.Stop();
			var duration = w.ElapsedMilliseconds;
		}

		public void Draw(Frame frame)
		{
			Draw(frame, Point.Empty);
		}
	}
}
