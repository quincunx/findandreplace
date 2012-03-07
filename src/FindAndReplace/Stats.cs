using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindAndReplace
{
	public class Stats
	{
		public int TotalFiles { get; set; }

		public int ProcessedFiles { get; set; }
		
		public int FilesWithMatches { get; set; }

		public int FilesWithoutMatches { get; set; }

		public int FailedToOpen { get; set; }

		public int FailedToWrite { get; set; }

		public int TotalMatches { get; set; }

		public int TotalReplaces { get; set; }
	}
}
