using System;
using System.Collections.Generic;
using System.Drawing;
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


		// draw active slide into frame
		public void Draw(Frame frame, Point location)
		{
			if(Slides.Count > 0)
				frame.Load(Slides[Active], location);
		}

		// advance to next slide
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
