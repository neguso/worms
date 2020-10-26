using System;

namespace Game
{
	public class Player
	{
		public string Name;
		public KeyboardKeyMap[] KeyMap;

		public virtual void Process(Command command)
		{

		}
	}



	public class Host : Player
	{
		public bool Quit;

		public Host()
		{
				Name = "Host";
				Quit = false;
		}

		public override void Process(Command command)
		{
			Quit = command == Command.Escape;
		}
	}
}
