using System;
using System.Collections.Generic;

namespace Game
{
	public class GameWorld
	{
		public bool Paused = false;

		public List<Player> Players { get; private set; }

		public GameWorld()
		{
			Players = new List<Player>();
		}

		public void Tick(Keyboard keyboard)
		{
			foreach (var player in Players)
			{
				var command = keyboard.DequeueCommand(player);
				
				if(command != null)
					player.Process(command);


			}
		}

		public void Render(Frame frame)
		{

		}
	}
}
