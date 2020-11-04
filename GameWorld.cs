using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
	public class GenericWorld
	{
		public List<Player> Players { get; private set; }
		public List<Element> Elements { get; private set;}


		public GenericWorld()
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
					var list = Elements.Where(element => element.Players.Any(p => p.Name == player.Name)).ToList();
					list.ForEach(element => element.Process(command));
				}
				Elements.ForEach(element => element.Update());
			});
		}

		public void Render(Frame frame)
		{
			Elements.Sort((a, b) => a.ZIndex < b.ZIndex ? -1 : (a.ZIndex > b.ZIndex ? 1 : 0));
			Elements.ForEach(element => frame.Load(element, element.Location));
		}
	}



	public class WormsWorld : GenericWorld
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
