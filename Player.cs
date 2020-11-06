using System;

namespace Game
{
	public class Player
	{
		public string Name;
		public KeyboardKeyMap[] KeyMap;

		public virtual void Process(Command command)
		{
			// do nothing
		}
	}



	public class WormsHost : Player
	{
		private GenericWorld menuWorld;

		public ActionType Action = ActionType.None;


		public WormsHost(GenericWorld world)
		{
			menuWorld = world;
		}


		public override void Process(Command command)
		{
			if(command == Command.Escape)
				Action = ActionType.Quit;
			else if(command == Command.Enter)
			{
				// search for menu
				var menu = menuWorld.Elements.Find(e => e.GetType().Equals(typeof(WormsMenu))) as WormsMenu;
				switch(menu.Selected.Id)
				{
					case "new_game": Action = ActionType.NewGame; break;
					case "quit_game": Action = ActionType.Quit; break;
				}
			}
		}


		public enum ActionType
		{
			None,
			Quit,
			NewGame
		}
	}
}
