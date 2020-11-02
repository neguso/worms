using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Game
{
  public class FastConsole
  {
    [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern SafeFileHandle CreateFile(
      string fileName,
      [MarshalAs(UnmanagedType.U4)] uint fileAccess,
      [MarshalAs(UnmanagedType.U4)] uint fileShare,
      IntPtr securityAttributes,
      [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
      [MarshalAs(UnmanagedType.U4)] int flags,
      IntPtr template);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteConsoleOutput(
      SafeFileHandle hConsoleOutput,
      CharInfo[] lpBuffer,
      Coord dwBufferSize,
      Coord dwBufferCoord,
      ref SmallRect lpWriteRegion);

    [StructLayout(LayoutKind.Sequential)]
    struct Coord
    {
      public short X;
      public short Y;
    };

    [StructLayout(LayoutKind.Explicit)]
    struct CharUnion
    {
      [FieldOffset(0)] public char UnicodeChar;
      [FieldOffset(0)] public byte AsciiChar;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct CharInfo
    {
      [FieldOffset(0)] public CharUnion Char;
      [FieldOffset(2)] public short Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SmallRect
    {
      public short Left;
      public short Top;
      public short Right;
      public short Bottom;
    }

    public static void WriteChar(char c, short left, short top)
    {
      var buffer = new CharInfo[] { new CharInfo { Attributes = 0x0004, Char = new CharUnion { UnicodeChar = c } } };
      var rect = new SmallRect() { Left = left, Top = top, Right = left, Bottom = top };
      using (var handle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero))
      {
        var result = WriteConsoleOutput(handle, buffer, new Coord() { X = 1, Y = 1 }, new Coord() { X = 0, Y = 0 }, ref rect);
      }
    }
  }


	[Flags]
	public enum FastColor
	{
		Blue = 0x0001,
		LightBlue = 0x0001 | 0x0008,
		Green,
		Red
	}
}
