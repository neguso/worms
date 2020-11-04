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

		public ActionEnum Action = ActionEnum.None;


		public WormsHost(GenericWorld world)
		{
			menu = world.Elements.Find(e => e.GetType().Equals(typeof(WormsMenu))) as WormsMenu;
		}


		public override void Process(Command command)
		{
			if(command == Command.Escape)
				Action = ActionEnum.Quit;
			else if(command == Command.Enter)
			{
				switch(menu.Selected.Id)
				{
					case "new_game": Action = ActionEnum.NewGame; break;
					case "quit_game": Action = ActionEnum.Quit; break;
				}
			}
		}


		public enum ActionEnum
		{
			None,
			Quit,
			NewGame
		}
	}
}
