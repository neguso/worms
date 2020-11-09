using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Game
{
  public class GenericWorld
  {
    public List<Player> Players { get; private set; }
    public List<Element> Elements { get; private set; }


    public GenericWorld()
    {
      Players = new List<Player>();
      Elements = new List<Element>();
    }


    public virtual bool Alive => Players.Any(p => p.Lives > 0);

    public virtual bool Completed => false;

    public virtual void Tick(Keyboard keyboard)
    {
      Players.ForEach(player =>
      {
        var command = keyboard.DequeueCommand(player);
        if (command != null)
        {
          player.Process(command);
          var list = Elements.Where(element => element.Players.Any(p => p.Name == player.Name)).ToList();
          list.ForEach(element => element.Process(command));
        }
        Elements.ForEach(element => element.Update());
      });
    }

    public virtual void Render(Frame frame)
    {
      Elements.Sort((a, b) => a.ZIndex < b.ZIndex ? -1 : (a.ZIndex > b.ZIndex ? 1 : 0));
      Elements.ForEach(element => element.Draw(frame));
    }
  }



  public class WormsWorld : GenericWorld
  {
    public List<WorldLevel> Levels { get; private set; }
    public int CurrentLevel;
    public Size Size;


    public WormsWorld(Size size, int players)
    {
      Size = size;

      this.Setup(players);
    }


    public WorldLevel Level
    {
      get
      {
        if (CurrentLevel >= 0 && CurrentLevel < Levels.Count)
          return Levels[CurrentLevel];
        return null;
      }
    }

    public override bool Completed => Level == null;

    public override void Tick(Keyboard keyboard)
    {
      base.Tick(keyboard);

      Level.Tick();
      if (Level.Finished)
        NextLevel();
    }

    protected void Setup(int players)
    {
      Levels = new List<WorldLevel>();
      CurrentLevel = -1;

      var player1 = new Player()
      {
        Index = 1,
        Name = "Player One",
        Color = ConsoleColor.Blue,
        Lives = 5,
        Score = 0,
        KeyMap = new KeyboardKeyMap[]
        {
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false), Command.Left),
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false), Command.Right),
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false), Command.Up),
          new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false), Command.Down)
        }
      };
      Players.Add(player1);
      //
      var score1 = new ScoreBox(player1, new Point(0, 0), player1.Name.Length + 15);
      Elements.Add(score1);

      if (players > 1)
      {
        var player2 = new Player()
        {
          Index = 2,
          Name = "Player Two",
          Color = ConsoleColor.Green,
          Lives = 1,
          Score = 0,
          KeyMap = new KeyboardKeyMap[]
          {
            new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.A, false, false, false), Command.Left),
            new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.D, false, false, false), Command.Right),
            new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.W, false, false, false), Command.Up),
            new KeyboardKeyMap(new ConsoleKeyInfo('\0', ConsoleKey.S, false, false, false), Command.Down)
          }
        };
        Players.Add(player2);
        //
        var score2 = new ScoreBox(player2, new Point(Size.Width - 23, 0), player2.Name.Length + 15);
        Elements.Add(score2);

				var level = new LevelBox(this, new Point(Size.Width / 2 - 4, 0), 7);
				Elements.Add(level);
      }

      Levels.Add(new WorldLevel(this, 1, Path.Combine(Environment.CurrentDirectory, @"resources\level1.txt"), 3));
      Levels.Add(new WorldLevel(this, 2, Path.Combine(Environment.CurrentDirectory, @"resources\level2.txt"), 3));
      //...

      NextLevel();
    }

    public void NextLevel()
    {
      if (Level != null)
        Level.Uninstall();

      CurrentLevel++;

      if (Level != null)
        Level.Install();
    }




    public class WorldLevel
    {
      public WormsWorld World { get; private set; }
      public int Level { get; private set; }


      protected Arena Arena;
      protected List<Worm> Worms;
      protected int FoodStock;
      protected Food Food;


      public WorldLevel(WormsWorld world, int level, string file, int foodStock)
      {
        World = world;
        Level = level;
        FoodStock = foodStock;
        Worms = new List<Worm>();
      }


      public bool Finished { get { return FoodStock == 0; } }

      public void Install()
      {
        Arena = new Arena(Path.Combine(Environment.CurrentDirectory, @$"resources\level{Level}.txt"), new Point(0, 1), new Size(World.Size.Width, World.Size.Height - 1));
        World.Elements.Add(Arena);

        foreach (var player in World.Players.Where(p => p.Lives > 0))
          Worms.Add(new Worm(player, World.Size));
        World.Elements.AddRange(Worms);

        Food = new Food(Point.Empty);
        World.Elements.Add(Food);
        PlaceFood();
      }

      public void Uninstall()
      {
        World.Elements.Remove(Arena);

        foreach (var worm in Worms)
          World.Elements.Remove(worm);
        Worms.Clear();

        World.Elements.Remove(Food);
        Food = null;
      }

      public Point RandomLocation()
      {
        var list = new List<Point>();
        for (int x = 0; x < Arena.Size.Width; x++)
          for (int y = 0; y < Arena.Size.Height; y++)
            if (World.Elements.All(e => e.GetBrick(new Point(x - e.Location.X, y - e.Location.Y)) == null))
              list.Add(new Point(x, y));
        var rnd = new Random();
        return list[rnd.Next(list.Count)];
      }

      public void PlaceFood()
      {
        if (FoodStock > 0)
        {
          Food.Location = RandomLocation();
          FoodStock--;
        }
      }

      public virtual void Tick()
      {
        // if worm collide with food
        foreach (var worm in Worms.Where(w => w.Enabled))
        {
          if (worm.Collisions(Food).Count > 0)
          {
						worm.Player.Score += 1;
            worm.Grow += 5;
            PlaceFood();
          }
          else if (worm.Collisions(Arena).Count > 0)
          {
            // if worm collide with arena
            worm.Player.Lives--;
            if (worm.Player.Lives > 0)
              worm.Reset();
            else
              worm.Enabled = false;

            // if worm collide with himself


            // if worm colide with other worms

          }
        }


      }
    }



    public class Food : Element
    {
      public bool Eated;

      public Food(Point location) : base(location, new Size(1, 1))
      {
      }


      protected override void UpdateCore()
      {
        SetBrick(Point.Empty, Brick.From(ColorConsole.CharMap['♣'], ConsoleColor.Green));
      }
    }

  }
}
