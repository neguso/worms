using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Game
{
	public class ColorConsole
	{
		const int STD_OUTPUT_HANDLE = -11; //CONOUT$

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

		[Flags]
		private enum ConsoleOutputModes : uint
    {
        ENABLE_PROCESSED_OUTPUT = 0x0001,
        ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
        ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
        DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
        ENABLE_LVB_GRID_WORLDWIDE = 0x0010
    }


		private static Stream StandardOutput;

		public static void Enable()
		{
			var handle = GetStdHandle(STD_OUTPUT_HANDLE);
			uint mode = 0;
			GetConsoleMode(handle, out mode);
			mode |= (uint)ConsoleOutputModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
			SetConsoleMode(handle, mode);

			StandardOutput = Console.OpenStandardOutput();
		}

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