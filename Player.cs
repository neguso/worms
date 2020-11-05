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
		protected WormsMenu menu;

		public ActionType Action = ActionType.None;


		public WormsHost(GenericWorld world)
		{
			menu = world.Elements.Find(e => e.GetType().Equals(typeof(WormsMenu))) as WormsMenu;
		}


		public override void Process(Command command)
		{
			if(command == Command.Escape)
				Action = ActionType.Quit;
			else if(command == Command.Enter)
			{
				var menu = world.Elements.Find(e => e.GetType().Equals(typeof(WormsMenu))) as WormsMenu;

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
