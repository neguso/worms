using System;
using System.Drawing;
using System.Linq;

namespace Game.Worms
{
	public class WormsMenu : Menu<MenuItem>
	{
		public WormsMenu(Point location) : base(location, new Size(25, 5))
		{
			UpdateTimer.Interval = 10;

			Items.AddRange(new MenuItem[] {
				new MenuItem("new_game_1", "1 Player"),
				new MenuItem("new_game_2", "2 Players"),
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
