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


		public override void Run()
		{
			do
			{
				// get users input
				//keyboard.Clear();
				//keyboard.Read();
				var a = keyboard.GetStatus();
				if(a.Length > 0)
				{
					Console.Write(a.Length);
				}

				// process world
				World.Tick(keyboard);

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
