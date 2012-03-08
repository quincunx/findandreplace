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

		public Stats Stats { get; set; }

		public FinderEventArgs(Finder.FindResultItem resultItem, Stats stats)
		{
			ResultItem = resultItem;
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
		public bool FindTextHasRegEx { get; set; }

		public class FindResultItem
		{
			public string FileName { get; set; }
			public string FilePath { get; set; }
			public string FileRelativePath { get; set; }
			public int NumMatches { get; set; }
			public MatchCollection Matches { get; set; }
			public bool IsSuccess { get; set; }
			public bool FailedToOpen { get; set; }
			public string ErrorMessage { get; set; }
			public List<TooltipLineNumber> LineNumbers { get; set; }
		}

		public class FindResult
		{
			public List<FindResultItem> Items { get; set; }
			public Stats Stats { get; set; }
		}

		public class TooltipLineNumber
		{
			public int LineNumber { get; set; }

			public bool HasMatch { get; set; }
		}

		public FindResult Find()
		{
			Verify.Argument.IsNotEmpty(Dir, "Dir");
			Verify.Argument.IsNotEmpty(FileMask, "FileMask");
			Verify.Argument.IsNotEmpty(FindText, "FindText");

			string[] filesInDirectory = Utils.GetFilesInDirectory(Dir, FileMask, IncludeSubDirectories);

			var resultItems = new List<FindResultItem>();
			var stats = new Stats();
			stats.TotalFiles = filesInDirectory.Length;

			//Analyze each file in the directory
			foreach (string filePath in filesInDirectory)
			{
				var resultItem = new FindResultItem();
				resultItem.IsSuccess = true;

				resultItem.FileName = Path.GetFileName(filePath);
				resultItem.FilePath = filePath;
				resultItem.FileRelativePath = "." + filePath.Substring(Dir.Length);

				stats.ProcessedFiles++;

				string fileContent = string.Empty;
				
				try
				{
					using (var sr = new StreamReader(filePath))
					{
					resultItem.LineNumbers = GetLineNumbersForMatchesPreview(filePath, resultItem.Matches);
					}

				}
				catch (Exception exception)
				{
					resultItem.IsSuccess = false;
					resultItem.FailedToOpen = true;
					resultItem.ErrorMessage = exception.Message;

					stats.FailedToOpen++;
				}


				if (!resultItem.FailedToOpen)
				{
					resultItem.Matches = GetMatches(fileContent);
					resultItem.NumMatches = resultItem.Matches.Count;

					stats.TotalMatches += resultItem.Matches.Count;

					if (resultItem.Matches.Count > 0)
						stats.FilesWithMatches++;
					else
						stats.FilesWithoutMatches++;
				}


				//Skip files that don't have matches
				if (resultItem.NumMatches > 0 || !resultItem.IsSuccess)
					resultItems.Add(resultItem);
				
				OnFileProcessed(new FinderEventArgs(resultItem, stats));
			}

			if (filesInDirectory.Length == 0)
				OnFileProcessed(new FinderEventArgs(new FindResultItem(), stats));

			return new FindResult() {Items = resultItems, Stats = stats};
		}

		public event FileProcessedEventHandler FileProcessed;

		protected virtual void OnFileProcessed(FinderEventArgs e)
		{
			if (FileProcessed != null)
				FileProcessed(this, e);
		}

		private MatchCollection GetMatches(string fileContent)
		{
			if (!FindTextHasRegEx)
				return Regex.Matches(fileContent, Regex.Escape(FindText), Utils.GetRegExOptions(IsCaseSensitive));

			var exp = new Regex(FindText, Utils.GetRegExOptions(IsCaseSensitive));

			return exp.Matches(fileContent);
		}

		private List<TooltipLineNumber> GetLineNumbersForMatchesPreview(string filePath, MatchCollection matches)
		{
			string content = string.Empty;

			using (var sr = new StreamReader(filePath))
			{
				content = sr.ReadToEnd();
			}
			
			var separator = Environment.NewLine;
			var lines = content.Split(new string[] { separator }, StringSplitOptions.None);

			var result = new List<TooltipLineNumber>();
			var temp = new List<TooltipLineNumber>();

			foreach (Match match in matches)
			{
				var lineIndexStart = DetectMatchLine(lines.ToArray(), match.Index);
				var lineIndexEnd = DetectMatchLine(lines.ToArray(), match.Index + match.Length);

				for (int i = lineIndexStart - 2; i <= lineIndexEnd + 2; i++)
				{
					if (i >= 0 && i < lines.Count())
					{
						var lineNumber = new TooltipLineNumber();
						lineNumber.LineNumber = i;
						lineNumber.HasMatch = (i >= lineIndexStart && i <= lineIndexEnd) ? true : false;
						temp.Add(lineNumber);
					}
				}
			}

			result.AddRange(temp.Where(ln=>ln.HasMatch).Distinct(new LineNumberComparer()));

			result.AddRange(temp.Where(ln=>!ln.HasMatch && !result.Select(l=>l.LineNumber).Contains(ln.LineNumber)).Distinct(new LineNumberComparer()));

			return result.OrderBy(ln=>ln.LineNumber).ToList();
		}

		private int DetectMatchLine(string[] lines, int position)
		{
			var separatorLength = 2;
			int i = 0;
			int charsCount = lines[0].Length + separatorLength;

			while (charsCount <= position)
			{
				i++;
				charsCount += lines[i].Length + separatorLength;
			}

			return i;
		}
	}

	public class LineNumberComparer : IEqualityComparer<Finder.TooltipLineNumber>
	{
		public bool Equals(Finder.TooltipLineNumber x, Finder.TooltipLineNumber y)
		{
			return x.LineNumber == y.LineNumber;
		}

		public int GetHashCode(Finder.TooltipLineNumber obj)
		{
			return obj.LineNumber.GetHashCode();
		}
	}
}
