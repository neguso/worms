using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Invaders
{
	public class InvadersHost : Player
	{
		protected GameWorld world;


		public InvadersHost(GameWorld world)
		{
			this.world = world;
		}


		public MenuAction Action { get; private set; } = MenuAction.None;


		public override void Process(Command command)
		{
			if(command == Command.Escape)
				Action = MenuAction.Quit;
			else if(command == Command.Enter)
			{
				// get menu
				var menu = world.Elements.Find(e => e.Id == "MainMenu") as InvadersMenu;
				switch(menu.Selected.Id)
				{
					case "new_game_1": Action = MenuAction.NewGame1; break;
					case "new_game_2": Action = MenuAction.NewGame2; break;
					case "show_help": Action = MenuAction.Help; break;
					case "quit_game": Action = MenuAction.Quit; break;
				}
			}
		}


		public enum MenuAction
		{
			None,
			Quit,
			NewGame1,
			NewGame2,
			Help
		}
	}
}
