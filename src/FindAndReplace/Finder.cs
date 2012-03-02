using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace FindAndReplace
{

	public class FinderEventArgs : EventArgs
	{
		public Finder.FindResultItem ResultItem { get; set; }

		public int TotalFilesCount { get; set; }

		public Statistic Stats { get; set; }

		public FinderEventArgs(Finder.FindResultItem resultItem, int fileCount, Statistic stats)
		{
			ResultItem = resultItem;
			TotalFilesCount = fileCount;
			Stats = stats;
		}
	}

	public delegate void FileProcessedEventHandler(object sender, FinderEventArgs e);

	public class Finder
	{
		public string Dir { get; set; }
		public bool IncludeSubDirectories { get; set; }

		public string FileMask { get; set; }
		public string FindText { get; set; }
		public bool IsCaseSensitive { get; set; }

		public class FindResultItem
		{
			public string FileName { get; set; }
			public string FilePath { get; set; }
			public string FileRelativePath { get; set; }
			public int NumMatches { get; set; }
			public MatchCollection Matches { get; set; }
			public bool IsSuccessOpen { get; set; }
		}

		public class FindResult
		{
			public List<FindResultItem> FindResults { get; set; }

			public Statistic FindStats { get; set; }
		}

		public FindResult Find()
		{
			Verify.Argument.IsNotEmpty(Dir, "Dir");
			Verify.Argument.IsNotEmpty(FileMask, "FileMask");
			Verify.Argument.IsNotEmpty(FindText, "FindText");

			string[] filesInDirectory = Utils.GetFilesInDirectory(Dir, FileMask, IncludeSubDirectories);

			var resultItems = new List<FindResultItem>();
			var stats = new Statistic();

			//Analyze each file in the directory
			foreach (string filePath in filesInDirectory)
			{
				var resultItem = new FindResultItem();

				resultItem.FileName = Path.GetFileName(filePath);
				resultItem.FilePath = filePath;
				resultItem.FileRelativePath = "." + filePath.Substring(Dir.Length);
				stats.TotalFilesCount++;

				try
				{
					resultItem.Matches = GetMatches(filePath);
					resultItem.IsSuccessOpen = true;
					resultItem.NumMatches = resultItem.Matches.Count;
					stats.TotalMathes += resultItem.Matches.Count;
					if (resultItem.Matches.Count!=0) stats.FilesWithMathesCount++;
				}
				catch(Exception exception)
				{
					resultItem.IsSuccessOpen = false;
					resultItem.NumMatches = 0;
					stats.FailedToOpen++;
				}

				//Skip files that don't have matches
				if (resultItem.NumMatches > 0)
				{
					resultItems.Add(resultItem);
				}

				OnFileProcessed(new FinderEventArgs(resultItem, filesInDirectory.Length, stats));
			}

			if (filesInDirectory.Length == 0) OnFileProcessed(new FinderEventArgs(new FindResultItem(), filesInDirectory.Length, stats));

			return new FindResult() {FindResults = resultItems};
		}

		public event FileProcessedEventHandler FileProcessed;

		protected virtual void OnFileProcessed(FinderEventArgs e)
		{
			if (FileProcessed != null)
				FileProcessed(this, e);
		}

		private MatchCollection GetMatches(string filePath)
		{
			string content = string.Empty;

			using (var sr = new StreamReader(filePath))
			{
				content = sr.ReadToEnd();
			}

			return Regex.Matches(content, FindText, Utils.GetRegExOptions(IsCaseSensitive));

		}
	}
}
