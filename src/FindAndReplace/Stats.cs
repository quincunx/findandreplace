using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindAndReplace
{
	public class Stats
	{
		public class StatsFiles
		{
			public int Total { get; set; }

			public int Processed { get; set; }

			public int Binary { get; set; }

			public int WithMatches { get; set; }

			public int WithoutMatches { get; set; }

			public int FailedToRead { get; set; }

			public int FailedToWrite { get; set; }
		}

		public class StatsMatches
		{
			public int Found { get; set; }

			public int Replaced { get; set; }
		}

		public class StatsTime
		{
			public TimeSpan Passed { get; set; }

			public TimeSpan Remaining { get; set; }
		}

		public StatsFiles Files { get; set; }

		public StatsMatches Matches { get; set; }

		public StatsTime Time { get; set; }

		public Stats()
		{
			Files = new StatsFiles();

			Matches = new StatsMatches();

			Time = new StatsTime();
		}

		public void UpdateTime(TimeSpan passed)
		{
			Time.Passed += passed;

			var passedSeconds = (int) Time.Passed.TotalSeconds;

			var remainingSeconds  = passedSeconds*Files.Total/Files.Processed;

			Time.Remaining = TimeSpan.FromSeconds(remainingSeconds);
		}
	}
}
