using System;

namespace Game.Worms
{
	public class WormsHost : Player
	{
		protected GameWorld world;

		public MenuAction Action { get; private set; } = MenuAction.None;


		public WormsHost(GameWorld world)
		{
			this.world = world;
		}


		public override void Process(Command command)
		{
			if(command == Command.Escape)
				Action = MenuAction.Quit;
			else if(command == Command.Enter)
			{
				// get menu
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
