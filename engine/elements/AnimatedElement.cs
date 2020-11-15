using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Game
{
	public abstract class AnimatedElement : Element
	{
		public Slideshow Show { get; private set; }

		public AnimatedElement(Point location, Size size) : base(location, size)
		{
			Show = new Slideshow();
		}

		protected override void UpdateCore()
		{
			Show.Next();
			Show.Draw(this, Point.Empty);
		}
	}



	public class Slideshow
	{
		public List<Brick[,]> Slides { get; private set; }
		public int Active;


		public Slideshow()
		{
			Slides = new List<Brick[,]>();
			Active = -1;
		}


		public void Load(string file, Size size, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
		{
			var lines = File.ReadLines(file).Select(l => l.Substring(0, Math.Min(size.Width, l.Length))).ToArray();

			var slide = new Brick[0,0];
			for(int y = 0; y < lines.Count(); y++)
			{
				if(y % size.Height == 0)
				{
					slide = new Brick[size.Width, size.Height];
					Slides.Add(slide);
				}

				var ary = lines[y].ToCharArray();
				for(int x = 0; x < ary.Length; x++)
					slide[x, y % size.Height] = ary[x] == ' ' ? null : Brick.From(ColorConsole.CharMap[ary[x]], foreground, background);
			}
		}


		/// <summary>
		/// Draw active slide into frame.
		/// </summary>
		public void Draw(Frame frame, Point location)
		{
			if(Slides.Count > 0)
				frame.Load(Slides[Active], location);
		}

		/// <summary>
		/// Advance to next slide.
		/// </summary>
		public void Next()
		{
			if(Slides.Count > 0)
			{
				Active++;
				if(Active >= Slides.Count)
					Active = 0;
			}
		}
	}
}
