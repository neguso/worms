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



	public abstract class Menu<ItemType> : Element where ItemType : MenuItem
	{
		public List<ItemType> Items { get; set; }
		public ItemType Selected { get; set; }


		public Menu(Point location, Size size) : base(location, size)
		{
			Items = new List<ItemType>();
		}


		protected void SelectPrev()
		{
			if (Items.Count > 1)
			{
				var i = Items.IndexOf(Selected);
				if (i > 0)
					Selected = Items[i - 1];
			}
		}

		protected void SelectNext()
		{
			if (Items.Count > 1)
			{
				var i = Items.IndexOf(Selected);
				if (i < Items.Count - 1)
					Selected = Items[i + 1];
			}
		}

		protected override void UpdateCore()
		{
			if (Commands.Count > 0)
			{
				// get next command
				var command = Commands.Dequeue();

				// act on command
				if (command == Command.Up)
					SelectPrev();
				else if (command == Command.Down)
					SelectNext();
			}

			// draw menu
			Draw();
		}

		protected virtual void Draw()
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i] == Selected)
					Text(new Point(0, 0 + i), Items[i].Text, ConsoleColor.Black, ConsoleColor.White);
				else
					Text(new Point(0, 0 + i), Items[i].Text);
			}
		}
	}



	public class WormsMenu : Menu<MenuItem>
	{

		public WormsMenu(Point location) : base(location, new Size(20, 5))
		{
			Items.AddRange(new MenuItem[] {
				Selected = new MenuItem("new_game_1", "1 Player"),
				new MenuItem("new_game_2", "2 Players"),
				new MenuItem("quit_game", "Quit"),
			});
		}


		protected override void Draw()
		{
			for (int i = 0; i < Items.Count; i++)
			{
				var text = Items[i].Text;

				if (Items[i] == Selected)
				{
					text = " \xaf " + text + " \xae ";
					text = text.PadLeft(Size.Width / 2 + text.Length / 2).PadRight(Size.Width);
					Text(new Point(0, 0 + i), text, ConsoleColor.Black, ConsoleColor.White);
				}
				else
				{
					text = text.PadLeft(Size.Width / 2 + text.Length / 2).PadRight(Size.Width);
					Text(new Point(0, 0 + i), text);
				}
			}
		}
	}
}
