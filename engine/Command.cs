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
		private readonly static Command _escape = new("Escape");
		public static Command Escape => _escape;

		private readonly static Command _enter = new("Enter");
		public static Command Enter => _enter;

		// player commands
		private readonly static Command _left = new("Left");
		public static Command Left => _left;

		private readonly static Command _right = new("Right");
		public static Command Right => _right;

		private readonly static Command _up = new("Up");
		public static Command Up => _up;

		private readonly static Command _down = new("Down");
		public static Command Down => _down;

		private readonly static Command _fire = new("Fire");
		public static Command Fire => _fire;

		private readonly static Command _jump = new("Jump");
		public static Command Jump => _jump;
	}
}
