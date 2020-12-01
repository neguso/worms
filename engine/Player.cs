using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game
{
	public class Player
	{
		public int Index;
		public string Name;
		public ConsoleColor Color = ConsoleColor.White;
		public int Lives;
		public int Score;
		public KeyboardKeyMap[] KeyMap;


		/// <summary>
		/// Decode keys in player commands.
		/// </summary>
		public IEnumerable<Command> Decode(IEnumerable<ConsoleKey> keys)
		{
			return keys.Join(KeyMap, k => k, m => m.Key, (k, m) => m.Command);
		}

		public virtual void Process(Command command) { }
	}
}
