using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

public static class ColorConsole
{
	private static ConsoleStatus savedStatus;

	public static void SaveStatus()
	{
		savedStatus = new ConsoleStatus
		{
			Title = Console.Title,
			//CursorVisible = Console.CursorVisible,
			ForegroundColor = Console.ForegroundColor,
			BackgroundColor = Console.BackgroundColor,
			Size = new Size(Console.WindowWidth, Console.WindowHeight)
		};
	}

	public static void RestoreStatus()
	{
		Console.Title = savedStatus.Title;
		//Console.CursorVisible = savedStatus.CursorVisible;
		Console.ForegroundColor = savedStatus.ForegroundColor;
		Console.BackgroundColor = savedStatus.BackgroundColor;
		Console.BufferWidth = Console.WindowWidth = savedStatus.Size.Width;
		Console.BufferHeight = Console.WindowHeight = savedStatus.Size.Height;
	}

	public static void Enable()
	{
	}

	public static void Disable()
	{
	}

	private static Size _size;
	public static Size Size
	{
		get => _size;
		set
		{
			Console.BufferWidth = Console.WindowWidth = value.Width;
			Console.BufferHeight = Console.WindowHeight = value.Height;
			_size = new Size(value.Width, value.Height);
		}
	}

	public static void Write(string value)
	{
		Console.Write(value);
	}

	public static void WriteLine(string value)
	{
		Console.WriteLine(value);
	}

	public static void WriteLine()
	{
		Console.WriteLine();
	}

	public static void SetCursorPosition(int left, int top)
	{
		Console.SetCursorPosition(left, top);
	}

	public static void Clear()
	{
		Console.Clear();
	}

	public static int WindowWidth => Console.WindowWidth;
	public static int WindowHeight => Console.WindowHeight;
	public static int CursorLeft => Console.CursorLeft;
	public static int CursorTop => Console.CursorTop;

	public static void SetForegroundColor(ConsoleColor color)
	{
		Console.ForegroundColor = color;
	}

	public static void SetBackgroundColor(ConsoleColor color)
	{
		Console.BackgroundColor = color;
	}

	public static void ResetColor()
	{
		Console.ResetColor();
	}

	private static Dictionary<char, char> _charMap;
	public static Dictionary<char, char> CharMap
	{
		get
		{
			if(_charMap == null)
			{
				var map = File.ReadLines(Path.Combine(Environment.CurrentDirectory, @"worms\resources\map.txt"));
				_charMap = new Dictionary<char, char>(map.Select(e => new KeyValuePair<char, char>(e[0], (char)byte.Parse(e.Substring(1), System.Globalization.NumberStyles.HexNumber))));
			}
			return _charMap;
		}
	}

	/// <summary>
		/// Console escape sequences.
		/// </summary>
		public static class Escape
		{
			private readonly static int[] ForegroundColorMap = [30, 34, 32, 36, 31, 35, 33, 37, 90, 94, 92, 96, 91, 95, 93, 97];
			private readonly static int[] BackgroundColorMap = [ 40, 44, 42, 46, 41, 45, 43, 47, 100, 104, 102, 106, 101, 105, 103, 107 ];

			public static string Color(ConsoleColor foreground, ConsoleColor background = ConsoleColor.Black)
			{
				var f = ForegroundColorMap[(int)foreground];
				var b = BackgroundColorMap[(int)background];
				return $"\x1b[{f};{b}m";
			}

			public static string Location(int left, int top)
			{
				//return $"\x1b[{top + 1};{left + 1}H";
				return string.Empty;
			}
		}



	private class ConsoleStatus
	{
		public string Title;
		public bool CursorVisible;
		public ConsoleColor ForegroundColor;
		public ConsoleColor BackgroundColor;
		public Size Size;
	}
}
