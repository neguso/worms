using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game
{
	public abstract class Menu<ItemType> : Element where ItemType : MenuItem
	{
		public List<ItemType> Items { get; private set; }
		public ItemType Selected { get; set; }


		public Menu(Point location, Size size) : base(location, size)
		{
			Items = new List<ItemType>();
		}


		protected void SelectPrev()
		{
			var items = Items.Where(i => i.Visible).ToList();
			if(items.Count > 1)
			{
				var i = items.IndexOf(Selected);
				if(i > 0)
					Selected = items[i - 1];
			}
		}

		protected void SelectNext()
		{
			var items = Items.Where(i => i.Visible).ToList();
			if(items.Count > 1)
			{
				var i = items.IndexOf(Selected);
				if(i < items.Count - 1)
					Selected = items[i + 1];
			}
		}

		protected override void UpdateCore()
		{
			if(Commands.Count > 0)
			{
				// get next command
				var command = Commands.Dequeue();

				// act on command
				if(command == Command.Up)
					SelectPrev();
				else if(command == Command.Down)
					SelectNext();
			}

			// draw menu
			Draw();
		}

		protected virtual void Draw()
		{
			var items = Items.Where(i => i.Visible).ToList();
			for(int i = 0; i < items.Count; i++)
			{
				if(items[i] == Selected)
					Text(new Point(0, 0 + i), items[i].Text, ConsoleColor.Black, ConsoleColor.White);
				else
					Text(new Point(0, 0 + i), items[i].Text);
			}
		}
	}



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
}
