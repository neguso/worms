using System;

namespace Game
{
	/// <summary>
	/// Represent a timer used to check if a specified number of milliseconds have passed.
	/// </summary>
	public sealed class Timer
	{
		private DateTime start;

		public int Interval { get; set; }


		public Timer(int interval)
		{
			Interval = interval;
			start = DateTime.MinValue;
		}


		public void Reset()
		{
			start = DateTime.Now;
		}

		public bool Passed
		{
			get { return (DateTime.Now - start).TotalMilliseconds > Interval; }
		}
	}
}
