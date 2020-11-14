using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
	public class StaticText : Element
	{
		public StaticText(string label, Point location, int width = 50) : base(location, new Size(width, 1))
		{
			Label = label;
			Foreground = ConsoleColor.White;
			Background = ConsoleColor.Black;
		}


		public string Label { get; set; }

		public ConsoleColor Foreground { get; set; }

		public ConsoleColor Background { get; set; }

		protected override void UpdateCore()
		{
			Text(Point.Empty, Label, Foreground, Background);
		}
	}
}
