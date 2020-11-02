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

    public void WaitRefresh(double duration = 16.66667)
    {
      if (WaitForRefresh)
      {
        double elapsed = (DateTime.Now - lastRefresh).TotalMilliseconds;
        if (elapsed < duration)
          System.Threading.Thread.Sleep((int)(duration - elapsed));
        lastRefresh = DateTime.Now;
      }
    }

    private Size _size;
    public Size Size
    {
      get { return _size; }
      set
      {
        Console.BufferWidth = Console.WindowWidth = value.Width;
        Console.BufferHeight = Console.WindowHeight = value.Height + 1;
        _size = new Size(value.Width, value.Height + 1);
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
        if (origin.X + point.X < 0 || origin.X + point.X > Size.Width - 1 || origin.Y + point.Y < 0 || origin.Y + point.Y > Size.Height - 1)
          return;

        if (line != point.Y)
          sb.Append(Escape.Location(origin.X + point.X, origin.Y + (line = point.Y)));

        if (brick == null || brick == Brick.Empty)
        {
          sb.Append(Escape.MoveRight(1));
          //sb.Append(' ');
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
