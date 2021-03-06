﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
	/// <summary>
	///  Represe
	/// </summary>
	public class ScrollingText : Element
	{
		protected Queue<char> queue;


		public ScrollingText(string label, Point location, int width) : base(location, new Size(width, 1))
		{
			Label = label;
			Foreground = ConsoleColor.White;
			Background = ConsoleColor.Black;
			Step = 1;
			UpdateTimer.Reset(100);

			queue = new Queue<char>(label.ToCharArray());
			while(queue.Count < Size.Width)
				queue.Enqueue(' ');
		}


		public string Label { get; set; }

		public ConsoleColor Foreground { get; set; }

		public ConsoleColor Background { get; set; }

		public int Step { get; set; }


		protected override void UpdateCore()
		{
			var text = new string(queue.ToArray());
			Text(Point.Empty, text.Substring(0, Math.Min(text.Length, Size.Width)), Foreground, Background);

			// shift text
			for(int i = 0; i < Step; i++)
				queue.Enqueue(queue.Dequeue());
		}
	}
}
