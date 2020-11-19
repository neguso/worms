using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
	public class ScoreBoard : Element
	{
		public ScoreBoard(Point location, ScoreTable table) : base(location, new Size(23, 12))
		{
			Table = table;

			Text(new Point(7, 0), "HIGH SCORES");
		}


		public ScoreTable Table { get; set; }


		protected override void UpdateCore()
		{
			for(int i = 0; i < Table.Scores.Length; i++)
			{
				Text(new Point(0, i + 2), $"{(i + 1).ToString().PadLeft(2)}. {Table.Scores[i].Player.PadRight(13, '.')}{Table.Scores[i].Value:N0}");
			}
		}
	}
}
