using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
	public abstract class InputController<KeyType>
	{
		public List<KeyType> Buffer { get; protected set; }


		public InputController()
		{
			Buffer = new List<KeyType>();
		}


		public abstract void Read();

		public void Clear()
		{
			Buffer.Clear();
		}

		public abstract Command DequeueCommand(Player player);
	}



	public class Keyboard : InputController<ConsoleKeyInfo>
	{
		/// <summary>
		/// Read all keys from keyboard buffer.
		/// </summary>
		public override void Read()
		{
			while(Console.KeyAvailable)
				Buffer.Add(Console.ReadKey(true));
		}

		/// <summary>
		/// Get next command for player.
		/// </summary>
		public override Command DequeueCommand(Player player)
		{
			var key = Buffer.FirstOrDefault(key => player.KeyMap.FirstOrDefault(map => map.Key.Key == key.Key) != null);
			if(key.Key != 0)
			{
				Buffer.Remove(key);
				return player.KeyMap.First(map => map.Key.Key == key.Key).Command;
			}
			return null;
		}
	}
}
