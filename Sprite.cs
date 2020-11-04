using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game
{
  public abstract class Sprite : Frame
  {
    public Point Location;
    public int ZIndex;


    public Sprite(Point location, Size size) : base(size)
    {
      Location = location;
    }


    public void Draw(Frame frame)
    {
      Load(frame, Location);
    }
  }



  public abstract class Element : Sprite
  {
    protected Queue<Command> Commands { get; private set; }

    // players commanding this element
    public List<Player> Players { get; private set; }


    public Element(Point location, Size size) : base(location, size)
    {
      Commands = new Queue<Command>();
      Players = new List<Player>();
    }


    protected DateTime LastUpdated = DateTime.MinValue;


    public int UpdateInterval = 100;

    public bool UpdatePending
    {
      get { return (DateTime.Now - LastUpdated).TotalMilliseconds > UpdateInterval; }
    }

    public virtual void Process(Command command)
    {
      this.Commands.Enqueue(command);
    }

    public virtual void Update()
    {
      if (UpdatePending)
      {
        UpdateCore();
        LastUpdated = DateTime.Now;
      }
    }

    protected abstract void UpdateCore();
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



	public abstract class AnimatedElement : Element
	{
		//Slideshow

		public AnimatedElement(Point location, Size size) : base(location, size)
		{
			
		}
	}



  public class ScrollingText : Element
  {
    protected Queue<char> queue;

    public ScrollingText(string label, Point location, int width) : base(location, new Size(width, 1))
    {
      Label = label;
      Step = 1;
      UpdateInterval = 0;

      queue = new Queue<char>(label.ToCharArray());
			while(queue.Count < Size.Width)
				queue.Enqueue(' ');
    }


    public string Label { get; set; }
    public int Step { get; set; }


    protected override void UpdateCore()
    {
			var text = new String(queue.ToArray());
      Text(Point.Empty, text.Substring(0, Math.Min(text.Length, Size.Width)));

			// shift text
      for (int i = 0; i < Step; i++)
        queue.Enqueue(queue.Dequeue());
    }
  }



  public class TestElement : Element
  {
    public TestElement() : base(new Point(0, 0), new Size(20, 8))
    {
    }


    protected override void UpdateCore()
    {
      if (Commands.Count > 0)
      {
        // get next command
        var command = Commands.Dequeue();

        // act on command
        if (command == Command.Up)
          Location.Offset(new Point(0, -1));
        else if (command == Command.Down)
          Location.Offset(new Point(0, 1));
        else if (command == Command.Left)
          Location.Offset(new Point(-1, 0));
        else if (command == Command.Right)
          Location.Offset(new Point(1, 0));
      }

      // update element
      //Area(Point.Empty, Size, Brick.From('\xb1'));
			Rectangle(Point.Empty, Size, new Brick[] { Brick.From('\xb1'), Brick.From('\xb1'), Brick.From('\xb1'), Brick.From('\xb1') });
    }
  }

}
