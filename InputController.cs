using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
  public abstract class InputController<KeyType>
  {
    public InputController()
    {
      Buffer = new List<KeyType>();
    }

    public List<KeyType> Buffer { get; protected set; }

    public abstract void Read();
    public void Clear()
    {
      Buffer.Clear();
    }

    public abstract Command DequeueCommand(Player player);
  }



  public class Keyboard : InputController<ConsoleKeyInfo>
  {
    public override void Read()
    {
      while (Console.KeyAvailable)
        Buffer.Add(Console.ReadKey(true));
    }

    public override Command DequeueCommand(Player player)
    {
      var key = Buffer.FirstOrDefault(key => player.KeyMap.FirstOrDefault(map => map.Key.Key == key.Key) != null);
      if (key.Key != 0)
      {
        Buffer.Remove(key);
        return player.KeyMap.First(map => map.Key.Key == key.Key).Command;
      }
      return null;
    }
  }
}
