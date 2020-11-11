using System;

namespace Game
{
	/// <summary>
	/// Represent a character with foreground and background color.
	/// </summary>
	public class Brick
	{
		public char Char;
		public ConsoleColor ForeColor;
		public ConsoleColor BackColor;


		public static Brick _empty = new Brick() { Char = Char.MinValue, ForeColor = ConsoleColor.White, BackColor = ConsoleColor.Black };
		public static Brick Empty => _empty;

		public static Brick _blank = new Brick() { Char = ' ', ForeColor = ConsoleColor.White, BackColor = ConsoleColor.Black };
		public static Brick Blank => _blank;

		public static Brick From(char c, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black) => new Brick() { Char = c, ForeColor = foregroundColor, BackColor = backgroundColor };
	}
}
