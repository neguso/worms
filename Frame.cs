using System;
using System.Drawing;

namespace Game
{
	public class Frame
	{
		protected Brick[,] buffer;

		public Frame(Size size)
		{
			buffer = new Brick[size.Width, size.Height];
		}


		public void Render(Action<Brick, Point> action)
		{
			for (int x = 0; x < buffer.GetLength(0); x++)
				for (int y = 0; y < buffer.GetLength(1); y++)
					action(buffer[x, y], new Point(x, y));
    }

		public void SetBrick(Point point, Brick brick)
		{
			if(point.X < 0 || point.X >= buffer.GetLength(0))
				return;
			if(point.Y < 0 || point.Y >= buffer.GetLength(1))
				return;

			buffer[point.X, point.Y] = brick;
		}

		public Brick GetBrick(Point point)
		{
			if(point.X < 0 || point.X >= buffer.GetLength(0))
				return Brick.Empty;
			if(point.Y < 0 || point.Y >= buffer.GetLength(1))
				return Brick.Empty;

			return buffer[point.X, point.Y];
		}

		public void HLine(Point location, int width, Brick brick, Brick[] ends = null)
		{
			if(ends == null)
				ends = new Brick[] { brick, brick };
			for(int i = 0; i < width; i++)
				SetBrick(new Point(location.X + i, location.Y), i == 0 ? ends[0] : (i == width - 1 ? ends[1] : brick));
		}

		public void VLine(Point location, int height, Brick brick, Brick[] ends = null)
		{
			if(ends == null)
				ends = new Brick[] { brick, brick };
			for(int i = 0; i < height; i++)
				SetBrick(new Point(location.X, location.Y + i), i == 0 ? ends[0] : (i == height - 1 ? ends[1] : brick));
		}

		public void Rectangle(Point location, Size size, Brick[] bricks, Brick[] corners = null, Brick fill = null)
		{
			if(corners == null)
				corners = new Brick[] { bricks[0], bricks[0], bricks[0], bricks[0] };
			// counter clockwise
			VLine(new Point(location.X, location.Y + 1), size.Height - 2, bricks[3]);
			HLine(new Point(location.X, location.Y + size.Height - 1), size.Width, bricks[2], new Brick[] { corners[3], corners[2] });
			VLine(new Point(location.X + size.Width - 1, location.Y + 1), size.Height - 2, bricks[1]);
			HLine(location, size.Width, bricks[0], new Brick[] { corners[0], corners[1] });
		}

		public void Area(Point a, int width, int height, Brick brick)
		{
			for(int x = 0; x < width; x++)
				for(int y = 0; y < height; y++)
					SetBrick(new Point(a.X + x, a.Y + y), brick);
		}

		public void Draw(Frame frame, Point location, Size size)
		{
			//TODO
		}

	}
}
