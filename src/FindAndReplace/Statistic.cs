using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindAndReplace
{
	public class Statistic
	{
		public int TotalFilesCount { get; set; }

		public int FilesWithMathesCount { get; set; }

		public int FailedToOpen { get; set; }

		public int FailedToWrite { get; set; }

		public int TotalMathes { get; set; }

		public int TotalReplaces { get; set; }
	}
}
