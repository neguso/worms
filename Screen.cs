using System;
using System.Drawing;

namespace Game
{
	public class Screen
	{
		public Screen()
		{
		}


		public bool WaitForRefresh = true;

		public void WaitRefresh()
		{
			if(WaitForRefresh)
				System.Threading.Thread.Sleep(50);
		}

		public Size Size
		{
			get { return new Size(Console.WindowWidth, Console.WindowHeight); }
			set
			{
				Console.BufferWidth = Console.WindowWidth = value.Width;
				Console.BufferHeight = Console.WindowHeight = value.Height;
			}
		}

		public string Title
		{
			get => Console.Title;
			set { Console.Title = value; }
		}


		public void Draw(Frame frame, Point origin)
		{
			frame.Render((brick, point) =>
			{
				if(brick == null || brick == Brick.Empty) return;

				var oldFore = Console.ForegroundColor;
				var oldBack = Console.BackgroundColor;
				var oldPos = new Point(Console.CursorLeft, Console.CursorTop);

				Console.ForegroundColor = brick.ForeColor;
				Console.BackgroundColor = brick.BackColor;
				Console.SetCursorPosition(origin.X + point.X, origin.Y + point.Y);
				Console.Write(brick.Char);

				Console.ForegroundColor = oldFore;
				Console.BackgroundColor = oldBack;
				Console.SetCursorPosition(oldPos.X, oldPos.Y);
			});
		}

		public void Draw(Frame frame)
		{
			Draw(frame, Point.Empty);
		}
	}
}
