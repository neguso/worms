using System;

namespace Game
{
	/// <summary>
	/// Represent a timer used to check if a specified number of milliseconds have passed.
	/// </summary>
	public sealed class Timer
	{
		private DateTime lastCheck;

		public int Interval { get; set; }


		public Timer(int interval = 50)
		{
			Interval = interval;
		}


		public void Reset()
		{
			lastCheck = DateTime.Now;
		}

		public bool Passed()
		{
			return (DateTime.Now - lastCheck).TotalMilliseconds > Interval;
		}

		public bool Passed(int interval)
		{
			return (DateTime.Now - lastCheck).TotalMilliseconds > interval;
		}
	}
}
