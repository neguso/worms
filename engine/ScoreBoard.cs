﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Game
{
	[Serializable]
	public class ScoreBoard
	{
		public Score[] Scores { get; set; }

		
		public ScoreBoard()
		{
			Scores = new Score[10];
			Init();
		}


		public void Init()
		{
			for(int i = 0; i < Scores.Length; i++)
				Scores[i] = new Score();
		}

		public static ScoreBoard Load(string filename)
		{
			var serializer = new XmlSerializer(typeof(ScoreBoard));
			using(var stream = new FileStream(filename, FileMode.Open))
			{
				var obj = serializer.Deserialize(stream) as ScoreBoard;
				stream.Close();
				return obj;
			}
		}

		public void Save(string filename)
		{
			var serializer = new XmlSerializer(GetType());
			using(var stream = new StreamWriter(filename))
			{
				serializer.Serialize(stream, this);
				stream.Close();
			}
		}

		public void Record(string player, int value)
		{
			var list = Scores.ToList();
			if(value >= list.Min(s => s.Value))
			{
				list.Add(new Score { Player = player, Value = value, Date = DateTime.Today });
				list.Sort((a, b) => a.Value < b.Value ? 1 : (a.Value > b.Value ? -1 : 0));
				Scores = list.Take(10).ToArray();
			}
		}



		public class Score
		{
			public Score()
			{
				Player = "AAA";
				Value = 0;
				Date = DateTime.MinValue;
			}

			public string Player { get; set; }
			public int Value { get; set; }
			public DateTime Date { get; set; }
		}
	}
}
