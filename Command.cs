using System;

namespace Game
{
  public class Command
  {
    private Command(string name)
    {
      this.Name = name;
    }

    public string Name;


    // general commands
    private static Command _escape = new Command("Escape");
    public static Command Escape { get => _escape; }

    private static Command _enter = new Command("Enter");
    public static Command Enter { get => _enter; }

    // player commands
    private static Command _left = new Command("Left");
    public static Command Left { get => _left; }

    private static Command _right = new Command("Right");
    public static Command Right { get => _right; }

    private static Command _up = new Command("Up");
    public static Command Up { get => _up; }

    private static Command _down = new Command("Down");
    public static Command Down { get => _down; }
  }
}
