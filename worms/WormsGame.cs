using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Game.Worms
{
	public class WormsGame : GenericGame
	{
		private WormsGame()
		{
			World = new WormsWorld(screen.Size);
		}


		public static void Launch()
		{
			new WormsGame().Run();
		}
	}
}
