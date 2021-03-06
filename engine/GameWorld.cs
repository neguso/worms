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
			HighScores = new ScoreTable();
		}


		public bool Active { get; set; }
		public Size Size { get; private set; }
		public List<Player> Players { get; private set; }
		public List<Element> Elements { get; private set; }
		public ScoreTable HighScores { get; private set; }


		public virtual void PostMessage(WorldMessage message)
		{
			messages.Enqueue(message);
		}

		public virtual void Tick(IEnumerable<ConsoleKey> keysPress, IEnumerable<ConsoleKey> keysDown)
		{
			// process world messages
			if(messages.Count > 0)
				ProcessMessage(messages.Dequeue());

			// process keys press
			if(keysPress.Any())
			{
				Players.ForEach(player =>
				{
					// get player commands
					var commands = player.Decode(keysPress);
					foreach(var command in commands)
					{
						// send commands to player
						player.Process(command);

						// send commands to player elements
						var elements = Elements.Where(e => e.Players.Any(p => p.Name == player.Name) && e.InputProcess == InputProcessMode.KeyPress).ToList();
						elements.ForEach(e => e.Process(command));
					}
				});
			}

			// process keys down
			if(keysDown.Any())
			{
				Players.ForEach(player =>
				{
					// get player commands
					var commands = player.Decode(keysDown);
					foreach(var command in commands)
					{
						// send commands to player elements
						var elements = Elements.Where(e => e.Players.Any(p => p.Name == player.Name) && e.InputProcess == InputProcessMode.KeyDown).ToList();
						elements.ForEach(e => e.Process(command));
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

		public virtual bool LoadScores(string filename)
		{
			if(!File.Exists(filename)) return false;

			HighScores = ScoreTable.Load(filename);
			return true;
		}

		public virtual void SaveScores(string filename)
		{
			HighScores.Save(filename);
		}
	}



	/// <summary>
	/// Represent a generic message that can be sent to a world.
	/// </summary>
	public class WorldMessage
	{
		public string Name;
	}



	/// <summary>
	/// Represent a abstract game level in a world.
	/// </summary>
	public abstract class WorldLevel
	{
		public int Index { get; set; }
		public string Name { get; set; }
		public GameWorld World { get; protected set; }


		public WorldLevel(GameWorld world, string name = "undefined", int index = 0)
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
