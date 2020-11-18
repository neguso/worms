using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Game
{
	/// <summary>
	/// Represent a abstract game world with players and elements.
	/// </summary>
	public abstract class GameWorld
	{
		protected Queue<WorldMessage> messages;


		public GameWorld(Size size)
		{
			messages = new Queue<WorldMessage>();

			Active = true;
			Size = size;
			Players = new List<Player>();
			Elements = new List<Element>();
		}


		public bool Active { get; set; }
		public Size Size { get; private set; }
		public List<Player> Players { get; private set; }
		public List<Element> Elements { get; private set; }


		public virtual void PostMessage(WorldMessage message)
		{
			messages.Enqueue(message);
		}

		/// <summary>
		/// Decode keys in player commands.
		/// </summary>
		public IEnumerable<Command> Decode(IEnumerable<ConsoleKey> keys, Player player)
		{
			return keys.Join(player.KeyMap, k => k, m => m.Key, (k, m) => m.Command);
		}

		public virtual void Tick(IEnumerable<ConsoleKey> keys)
		{
			// process world messages
			if(messages.Count > 0)
				ProcessMessage(messages.Dequeue());

			if(keys.Count() > 0)
			{
				Players.ForEach(player =>
				{
					// get player commands
					var commands = Decode(keys, player);
					foreach(var command in commands)
					{
						// send commands to player
						player.Process(command);

						// send commands to player elements
						var elements = Elements.Where(element => element.Players.Any(p => p.Name == player.Name)).ToList();
						elements.ForEach(element => element.Process(command));
					}
				});
			}

			// update world elements
			Elements.ForEach(element => element.Update());
		}

		public abstract void ProcessMessage(WorldMessage message);

		public virtual void Render(Frame frame)
		{
			Elements.Sort((a, b) => a.ZIndex < b.ZIndex ? -1 : (a.ZIndex > b.ZIndex ? 1 : 0));
			Elements.ForEach(element => element.Draw(frame));
		}
	}



	/// <summary>
	/// Represent a message that can be sent to a world.
	/// </summary>
	public class WorldMessage
	{
		public string Name;
	}



	/// <summary>
	/// Represent a abstract game level in a world.
	/// </summary>
	public abstract class GameLevel
	{
		public int Index { get; set; }
		public string Name { get; set; }
		public GameWorld World { get; protected set; }


		public GameLevel(GameWorld world, string name = "undefined", int index = 0)
		{
			Name = name;
			Index = index;
			World = world;
		}


		public virtual void Install() { }

		public virtual void Uninstall() { }

		public virtual void Tick(IEnumerable<ConsoleKey> keys) { }
	}
}
