using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Game
{
	public class ColorConsole
	{
		private const int STD_OUTPUT_HANDLE = -11; //CONOUT$

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

		[Flags]
		private enum ConsoleOutputModes : uint
    {
        ENABLE_PROCESSED_OUTPUT = 0x0001,
        ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
        ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
        DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
        ENABLE_LVB_GRID_WORLDWIDE = 0x0010
    }


		private static uint _oldMode = 0;
		private static Stream StandardOutput = null;

		public static void Enable()
		{
			var handle = GetStdHandle(STD_OUTPUT_HANDLE);
			GetConsoleMode(handle, out _oldMode);
			uint mode = _oldMode | (uint)ConsoleOutputModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
			SetConsoleMode(handle, mode);

			StandardOutput = Console.OpenStandardOutput();
		}

		public static void Disable()
		{
			var handle = GetStdHandle(STD_OUTPUT_HANDLE);
			SetConsoleMode(handle, _oldMode);

			StandardOutput.Close();
			StandardOutput.Dispose();
		}


		private static Dictionary<char, char> _charMap;
		public static Dictionary<char, char> CharMap
		{
			get
			{
				if(_charMap == null)
				{
					var map = File.ReadLines(Path.Combine(Environment.CurrentDirectory, @"resources\map.txt"));
					_charMap = new Dictionary<char, char>(map.Select(e => new KeyValuePair<char, char>(e[0], (char)byte.Parse(e.Substring(1), System.Globalization.NumberStyles.HexNumber))));
				}
				return _charMap;
			}
		}

		//TODO move Size, Title, etc from Screen

		public static void Write(string text)
		{
			var buffer = text.ToCharArray().Select(c => (byte)c).ToArray();
			StandardOutput.Write(buffer, 0, buffer.Length);
		}

		public static void Write(byte[] buffer)
		{
			StandardOutput.Write(buffer, 0, buffer.Length);
		}
	}
}