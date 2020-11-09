using System;

namespace Game
{
  abstract public class KeyMap<KeyType>
  {
    public KeyMap(KeyType key, Command command)
    {
      Key = key;
      Command = command;
    }

    public KeyType Key { get; private set; }
    public Command Command { get; private set; }
  }



  public class KeyboardKeyMap : KeyMap<ConsoleKeyInfo>
  {
    public KeyboardKeyMap(ConsoleKeyInfo key, Command command) : base(key, command)
    { }
  }
}
