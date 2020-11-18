using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
	/// <summary>
	/// Represent a static text.
	/// </summary>
	public class StaticText : Element
	{
		public StaticText(string label, Point location, Size size) : base(location, size)
		{
			Label = label;
			Foreground = ConsoleColor.White;
			Background = ConsoleColor.Black;
			TextAlign = TextAlignment.Left;
			VerticalAlign = VerticalAlignement.Top;
		}
		public StaticText(string label, Point location, int width = 50) : this(label, location, new Size(width, 1))
		{ }



		public string Label { get; set; }

		public ConsoleColor Foreground { get; set; }

		public ConsoleColor Background { get; set; }

		public TextAlignment TextAlign { get; set; }
		public VerticalAlignement VerticalAlign { get; set; }

		protected override void UpdateCore()
		{
			var x = TextAlign == TextAlignment.Left ? 0 : (TextAlign == TextAlignment.Right ? Size.Width - Label.Length : Size.Width / 2 - Label.Length / 2);
			var y = VerticalAlign == VerticalAlignement.Top ? 0 : (VerticalAlign == VerticalAlignement.Bottom ? Size.Height - 1 : (Size.Height - 1) / 2);
			Text(new Point(x, y), Label, Foreground, Background);
		}


		public enum TextAlignment
		{
			Left,
			Middle,
			Right
		}

		public enum VerticalAlignement
		{
			Top,
			Middle,
			Bottom
		}
	}
}
