using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game
{
	public class MenuItem
	{
		public string Id;
		public string Text;
		public bool Visible;

		public MenuItem(string id, string text)
		{
			Id = id;
			Text = text;
			Visible = true;
		}
	}



	public abstract class Menu : Element
	{
		public List<MenuItem> Items { get; set; }
		public MenuItem Selected { get; set; }


		public Menu(Point location, Size size) : base(location, size)
		{
			Items = new List<MenuItem>();
		}


		protected override void UpdateCore()
		{
			for (int i = 0; i < Items.Count; i++)
			{
				Text(new Point(0, 0 + i), Items[i].Text);
			}
		}

	}



	public class WormsMenu : Menu
	{
		public WormsMenu(Point location) : base(location, new Size(30, 5))
		{
			Items.AddRange(new MenuItem[] {
				Selected = new MenuItem("new_game", "New Game"),
				new MenuItem("quit_game", "Quit"),
			});
		}

	}

}
