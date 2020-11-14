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

		public virtual void Process(Command command) { }
	}
}
