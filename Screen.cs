using System;
using System.Diagnostics;
using System.Drawing;

namespace Game
{
	public class Screen
	{
		public Screen()
		{
			Console.CursorVisible = false;
		}


		public bool WaitForRefresh = true;

		public void WaitRefresh()
		{
			if(WaitForRefresh)
				System.Threading.Thread.Sleep(50);
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

			var oldFore = Console.ForegroundColor;
			var oldBack = Console.BackgroundColor;
			var oldPos = new Point(Console.CursorLeft, Console.CursorTop);

			frame.Render((brick, point) =>
			{
				Console.SetCursorPosition(origin.X + point.X, origin.Y + point.Y);

				if(brick == null || brick == Brick.Empty)
				{
					//Console.ForegroundColor = ForegroundColor;
					//Console.BackgroundColor = BackgroundColor;
					Console.Write(' ');
				}
				else
				{
					Console.ForegroundColor = brick.ForeColor;
					Console.BackgroundColor = brick.BackColor;
					Console.Write(brick.Char);
				}
			});

			Console.ForegroundColor = oldFore;
			Console.BackgroundColor = oldBack;
			Console.SetCursorPosition(oldPos.X, oldPos.Y);

			w.Stop();
			var d = w.ElapsedMilliseconds;
		}

		public void Draw(Frame frame)
		{
			Draw(frame, Point.Empty);
		}
	}
}
