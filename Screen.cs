using System;
using System.Drawing;

namespace Game
{
	public class Screen
	{
		public Screen()
		{
			Size = new Size(Console.WindowWidth, Console.WindowHeight);
		}

		public Screen(Size size)
		{
			Size = size;
			Console.WindowWidth = Size.Width;
			Console.WindowHeight = Size.Height;
		}

		public string Title
		{
			get => Console.Title;
			set { Console.Title = value; }
		}

		public Size Size { get; }


		public void Update(Frame frame, Point origin)
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

		public void Update(Frame frame)
		{
			Update(frame, Point.Empty);
		}
	}
}