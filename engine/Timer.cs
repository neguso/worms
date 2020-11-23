using System;

namespace Game
{
	/// <summary>
	/// Represent a timer used to check if a specified number of milliseconds have passed.
	/// </summary>
	public sealed class Timer
	{
		private DateTime start;


		public Timer(int interval)
		{
			Interval = interval;
			start = DateTime.Now; //.AddMilliseconds(-Interval);
		}


		public int Interval { get; private set; }

		public bool Passed
		{
			get { return (DateTime.Now - start).TotalMilliseconds >= Interval; }
		}

		public int Count
		{
			get { return (int)Math.Floor((DateTime.Now - start).TotalMilliseconds / Interval); }
		}

		public void Reset()
		{
			start = DateTime.Now;
		}

		public void Reset(int interval)
		{
			Interval = interval;
			start = DateTime.Now;//.AddMilliseconds(-Interval);
		}
	}
}
