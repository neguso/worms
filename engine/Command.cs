using System;

namespace Game
{
	/// <summary>
	/// Predefined commands used by games.
	/// </summary>
	public class Command
	{
		public string Name { get; private set; }


		private Command(string name)
		{
			Name = name;
		}


		// general commands
		private readonly static Command _escape = new Command("Escape");
		public static Command Escape => _escape;

		private readonly static Command _enter = new Command("Enter");
		public static Command Enter => _enter;

		// player commands
		private readonly static Command _left = new Command("Left");
		public static Command Left => _left;

		private readonly static Command _right = new Command("Right");
		public static Command Right => _right;

		private readonly static Command _up = new Command("Up");
		public static Command Up => _up;

		private readonly static Command _down = new Command("Down");
		public static Command Down => _down;

		private readonly static Command _fire = new Command("Fire");
		public static Command Fire => _fire;

		private readonly static Command _jump = new Command("Jump");
		public static Command Jump => _jump;
	}
}
