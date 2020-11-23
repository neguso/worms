using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Game.Invaders
{
	public class InvadersMenu : Menu<MenuItem>
	{
		public InvadersMenu(Point location) : base(location, new Size(25, 5))
		{
			Id = "MainMenu";
			UpdateTimer.Reset(50);

			Items.AddRange(new MenuItem[] {
				new MenuItem("new_game_1", "1 Player"),
				new MenuItem("new_game_2", "2 Players"),
				new MenuItem("show_help", "Help"),
				new MenuItem("quit_game", "Quit"),
			});
			Selected = Items[0];
		}




		protected override void Draw()
		{
			var items = Items.Where(i => i.Visible).ToList();
			for(int i = 0; i < items.Count; i++)
			{
				var text = items[i].Text;

				if(items[i] == Selected)
				{
					text = " \x10 " + text;
					text = text.PadRight(Size.Width);
					Text(new Point(0, 0 + i), text, ConsoleColor.Black, ConsoleColor.White);
				}
				else
				{
					text = "   " + text;
					text = text.PadRight(Size.Width);
					Text(new Point(0, 0 + i), text);
				}
			}
		}
	}
}
