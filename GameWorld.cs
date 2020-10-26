using System;
using System.Collections.Generic;

namespace Game
{
	public class GameWorld
	{
		public List<Player> Players { get; private set; }
		public List<Element> Elements { get; private set;}


		public GameWorld()
		{
			Players = new List<Player>();
			Elements = new List<Element>();
		}


		public void Tick(Keyboard keyboard)
		{
			Players.ForEach(player =>
			{
				var command = keyboard.DequeueCommand(player);
				if(command != null)
				{
					player.Process(command);
					Elements.ForEach(element => element.Process(command));
				}
				Elements.ForEach(element => element.Update());
			});
		}

		public void Render(Frame frame)
		{
			Elements.Sort((a, b) => a.ZIndex < b.ZIndex ? -1 : (a.ZIndex > b.ZIndex ? 1 : 0));
			Elements.ForEach(element => frame.Draw(element, element.Location));
		}
	}



	public class WormsWorld : GameWorld
	{
		public WormsWorld()
		{
			this.Initialize();
		}


		protected void Initialize()
		{
			// 
		}

	}
}
