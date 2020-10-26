using System;

namespace Game
{
	public class Brick
	{
		public char Char;
		public ConsoleColor ForeColor;
		public ConsoleColor BackColor;


		public static Brick _empty = new Brick() { Char = Char.MinValue, ForeColor = ConsoleColor.White, BackColor = ConsoleColor.Black };
		public static Brick Empty { get => _empty; }

		public static Brick From(char c) => new Brick() { Char = c, ForeColor = ConsoleColor.White, BackColor = ConsoleColor.Black };
		public static Brick From(char c, ConsoleColor foregroundColor, ConsoleColor backgroundColor) => new Brick() { Char = c, ForeColor = foregroundColor, BackColor = backgroundColor };
	}
}
