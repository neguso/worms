using System;

namespace Game
{
	public class Player
	{
		public int Index;
		public string Name;
		public ConsoleColor Color;
		public int Lives;
		public int Score;
		public KeyboardKeyMap[] KeyMap;

		public virtual void Process(Command command) { }
	}



	public class WormsHost : Player
	{
		private GenericWorld world;

		public MenuAction Action = MenuAction.None;


		public WormsHost(GenericWorld world)
		{
			this.world = world;
		}


		public override void Process(Command command)
		{
			if(command == Command.Escape)
				Action = MenuAction.Quit;
			else if(command == Command.Enter)
			{
				// search for menu
				var menu = world.Elements.Find(e => e.GetType().Equals(typeof(WormsMenu))) as WormsMenu;
				switch(menu.Selected.Id)
				{
					case "new_game_1": Action = MenuAction.NewGame1; break;
					case "new_game_2": Action = MenuAction.NewGame2; break;
					case "quit_game": Action = MenuAction.Quit; break;
				}
			}
		}


		public enum MenuAction
		{
			None,
			Quit,
			NewGame1,
			NewGame2
		}
	}
}
