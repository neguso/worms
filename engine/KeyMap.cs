using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game
{
	abstract public class KeyMap<KeyType>
	{
		public KeyType Key { get; private set; }
		public Command Command { get; private set; }

		public KeyMap(KeyType key, Command command)
		{
			Key = key;
			Command = command;
		}
	}



	public class KeyboardKeyMap : KeyMap<ConsoleKey>
	{
		public KeyboardKeyMap(ConsoleKey key, Command command) : base(key, command)
		{ }
	}
}
