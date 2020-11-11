using System;

namespace Game
{
	/// <summary>
	/// General commands used by games
	/// </summary>
	public class Command
	{
		private Command(string name)
		{
			this.Name = name;
		}

		public string Name;


		// general commands
		private static Command _escape = new Command("Escape");
		public static Command Escape => _escape;

		private static Command _enter = new Command("Enter");
		public static Command Enter => _enter;

		// player commands
		private static Command _left = new Command("Left");
		public static Command Left => _left;

		private static Command _right = new Command("Right");
		public static Command Right => _right;

		private static Command _up = new Command("Up");
		public static Command Up => _up;

		private static Command _down = new Command("Down");
		public static Command Down => _down;

		private static Command _fire = new Command("Fire");
		public static Command Fire => _fire;
	}
}
