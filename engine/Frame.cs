using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Game
{
	/// <summary>
	/// A memory buffer containing characters representing images that can be displayed in a console.
	/// </summary>
	public class Frame
	{
		protected Brick[,] buffer;


		public Frame(Size size)
		{
			buffer = new Brick[size.Width, size.Height];
		}


		public Size Size
		{
			get => new Size(buffer.GetLength(0), buffer.GetLength(1));
		}


		/// <summary>
		/// Iterate through frame characters horizontally then vertically.
		/// </summary>
		public void Scan(Action<Brick, int, int> action, bool skipEmpty = true)
		{
			for(int y = 0; y < buffer.GetLength(1); y++)
				for(int x = 0; x < buffer.GetLength(0); x++)
					if(skipEmpty)
					{ 
						if(buffer[x, y] != null)
							action(buffer[x, y], x, y);
					}
					else
						action(buffer[x, y], x, y);
		}

		/// <summary>
		/// Load supplied frame into current frame.
		/// </summary>
		public void Load(Frame frame, Point location)
		{
			frame.Scan((brick, x, y) => {
				SetBrick(new Point(location.X + x, location.Y + y), brick);
			});
		}

		/// <summary>
		/// Load supplied buffer into current frame.
		/// </summary>
		public void Load(Brick[,] buffer, Point location)
		{
			for(int y = 0; y < buffer.GetLength(1); y++)
				for(int x = 0; x < buffer.GetLength(0); x++)
				{
					int xx = location.X + x, yy = location.Y + y;
					if(xx >= 0 && xx < this.buffer.GetLength(0) && yy >= 0 && yy < this.buffer.GetLength(1))
						this.buffer[xx, yy] = buffer[x, y];
				}
		}

		/// <summary>
		/// Load file content into current frame.
		/// </summary>
		public void Load(string file, Point location, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
		{
			var lines = File.ReadLines(file).Take(buffer.GetLength(1)).Select(l => l.Substring(0, Math.Min(buffer.GetLength(0), l.Length))).ToArray();
			for(int y = 0; y < lines.Length; y++)
			{
				var ary = lines[y].ToCharArray();
				for(int x = 0; x < ary.Length; x++)
				{
					int xx = location.X + x, yy = location.Y + y;
					if(xx >= 0 && xx < buffer.GetLength(0) && yy >= 0 && yy < buffer.GetLength(1))
						buffer[xx, yy] = ary[x] == ' ' ? null : Brick.From(ColorConsole.CharMap[ary[x]], foreground, background);
				}
			}
		}

		/// <summary>
		/// Load stream content into current frame.
		/// </summary>
		public void Load(Stream stream, Point location, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
		{
			var reader = new StreamReader(stream);
			var lines = reader.ReadToEnd().Split("\r\n").Take(buffer.GetLength(1)).Select(l => l.Substring(0, Math.Min(buffer.GetLength(0), l.Length))).ToArray();
			for(int y = 0; y < lines.Length; y++)
			{
				var ary = lines[y].ToCharArray();
				for(int x = 0; x < ary.Length; x++)
				{
					int xx = location.X + x, yy = location.Y + y;
					if(xx >= 0 && xx < buffer.GetLength(0) && yy >= 0 && yy < buffer.GetLength(1))
						buffer[xx, yy] = ary[x] == ' ' ? null : Brick.From(ColorConsole.CharMap[ary[x]], foreground, background);
				}
			}
		}

		/// <summary>
		/// Fill frame buffer with specified brick.
		/// </summary>
		public void Fill(Brick brick)
		{
			for(int y = 0; y < buffer.GetLength(1); y++)
				for(int x = 0; x < buffer.GetLength(0); x++)
					buffer[x, y] = brick;
		}

		public void Clear()
		{
			Fill(null);
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
				return null;
			if(point.Y < 0 || point.Y >= buffer.GetLength(1))
				return null;

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
			// draw rectangle counter clockwise:  left, bottom, right, top
			VLine(new Point(location.X, location.Y + 1), size.Height - 2, bricks[3]);
			HLine(new Point(location.X, location.Y + size.Height - 1), size.Width, bricks[2], new Brick[] { corners[3], corners[2] });
			VLine(new Point(location.X + size.Width - 1, location.Y + 1), size.Height - 2, bricks[1]);
			HLine(location, size.Width, bricks[0], new Brick[] { corners[0], corners[1] });
			if(fill != null)
				Area(new Point(location.X + 1, location.Y + 1), new Size(size.Width - 2, size.Height - 2), fill);
		}

		public void Area(Point location, Size size, Brick brick)
		{
			for(int x = 0; x < size.Width; x++)
				for(int y = 0; y < size.Height; y++)
					SetBrick(new Point(location.X + x, location.Y + y), brick);
		}

		public void Text(Point location, string text, ConsoleColor foreground = ConsoleColor.White, ConsoleColor backgound = ConsoleColor.Black)
		{
			if(string.IsNullOrEmpty(text)) return;
			var chars = text.ToCharArray();
			for(int i = 0; i < chars.Length; i++)
				SetBrick(new Point(location.X + i, location.Y), Brick.From(chars[i], foreground, backgound));
		}
	}
}
