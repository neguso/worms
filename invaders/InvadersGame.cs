using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Invaders
{
	public class InvadersGame : GenericGame
	{
		private InvadersGame()
		{
			World = new InvadersWorld(screen.Size);
		}


		// overriden to get keys currently down
		public override void Run()
		{
			do
			{
				// get user input
				var keys = keyboard.ReadKeyDown();

				// process world
				World.Tick(keys);

				// render frame
				frame.Clear();
				World.Render(frame);

				// draw frame on screen
				screen.WaitRefresh();
				screen.Draw(frame);
			}
			while(World.Active);
		}


		public static void Launch()
		{
			new InvadersGame().Run();
		}
	}
}
