using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Game
{
  internal class Program
  {
    public static void Main(string[] args)
    {
      Console.Clear();
      Console.Title = "The Worms Game";
      ColorConsole.Enable();
      ColorConsole.Size = new Size(100, 40);

      WormsGame.Launch();

      //restore size
      ColorConsole.Disable();
      //restore title
    }
  }


  public class WormsGame
  {
    protected Keyboard keyboard;
    protected Screen screen;
    protected Frame frame;

    protected GenericWorld menuWorld;
    protected WormsWorld gameWorld;
    protected WormsHost host;


    private WormsGame()
    {
      keyboard = new Keyboard();
      screen = new Screen();
      frame = new Frame(screen.Size);
    }


    public static void Launch()
    {
      new WormsGame().Run();
    }


    public void Run()
    {
      RunIntro();

      SetupMenu();
      do
      {
        RunMenu();

        switch (host.Action)
        {
					case WormsHost.ActionType.NewGame1:
          case WormsHost.ActionType.NewGame2:
            SetupGame(host.Action == WormsHost.ActionType.NewGame1 ? 1 : 2);
            RunGame();

            if (gameWorld.Alive)
            {
              if (gameWorld.Completed)
              {
                // game finished
              }
            }
            else
            {
              // game over
            }

            break;

        }
      }
      while (host.Action != WormsHost.ActionType.Quit);
    }


    public void RunIntro()
    {
      frame.Load(Path.Combine(Environment.CurrentDirectory, @"resources\banner.txt"), new Point(screen.Size.Width / 2 - 24, 10));
      frame.Text(new Point(38, 20), "Awesome game with worms.");
      frame.Text(new Point(39, 32), "Press any key to start");
      screen.Draw(frame, new Point(0, 0));
      Console.ReadKey();
      screen.Clear();
    }

    public void SetupMenu()
    {
      menuWorld = new GenericWorld();

      host = new WormsHost(menuWorld)
      {
        KeyMap = new KeyboardKeyMap[]
        {
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.Escape, false, false, false), Command.Escape),
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.Enter, false, false, false), Command.Enter),
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false), Command.Up),
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false), Command.Down)
        }
      };
      //
      menuWorld.Players.Add(host);

      var logo = new Element(new Point(screen.Size.Width / 2 - 24, 10), new Size(48, 9));
      logo.Load(Path.Combine(Environment.CurrentDirectory, @"resources\banner.txt"), Point.Empty);
      //
      menuWorld.Elements.Add(logo);

      var menu = new WormsMenu(new Point(screen.Size.Width / 2 - 8, 25));
      menu.Players.Add(host);
      //
      menuWorld.Elements.Add(menu);
    }

    public void RunMenu()
    {
      host.Action = WormsHost.ActionType.None;

      do
      {
        // get user input
        keyboard.Clear();
        keyboard.Read();

        // process world
        menuWorld.Tick(keyboard);

        // render frame
        frame.Clear();
        menuWorld.Render(frame);

        // draw frame on screen
        screen.WaitRefresh();
        screen.Draw(frame);
      }
      while (host.Action == WormsHost.ActionType.None);

      screen.Clear();
    }

    public void SetupGame(int players)
    {
      gameWorld = new WormsWorld(screen.Size, players);
    }

    public void RunGame()
    {
      do
      {
        // get user input
        keyboard.Clear();
        keyboard.Read();

        // process world
        gameWorld.Tick(keyboard);

        // render frame
        frame.Clear();
        gameWorld.Render(frame);

        // draw frame on screen
        screen.WaitRefresh();
        screen.Draw(frame);
      }
      while (gameWorld.Alive && !gameWorld.Completed);
    }

  }
}
