using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
	public abstract class GenericGame
	{
		protected Keyboard keyboard;
		protected Screen screen;
		protected Frame frame;


		protected GenericGame()
		{
			keyboard = new Keyboard();
			screen = new Screen();
			frame = new Frame(screen.Size);
		}


		public GameWorld World { get; protected set; }


		public virtual void Run()
		{
			do
			{
				// get user input
				var keysPress = keyboard.ReadKeyPress();
				var keysDown = keyboard.ReadKeyDown();

				// process world
				World.Tick(keysPress, keysDown);

				// render frame
				frame.Clear();
				World.Render(frame);

				// draw frame on screen
				screen.WaitRefresh();
				screen.Draw(frame);
			}
			while(World.Active);
		}
	}
}
