using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Invaders
{
	public class InvadersGame : GenericGame
	{
		private InvadersGame()
		{
			World = new InvadersWorld(Screen.Default.Size);
		}


		public static void Launch()
		{
			new InvadersGame().Run();
		}
	}
}
