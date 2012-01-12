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

		public FinderEventArgs(Finder.FindResultItem resultItem, int fileCount)
		{
			ResultItem = resultItem;
			TotalFilesCount = fileCount;
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
			public string ToolTip { get; set; }
		}

		public List<FindResultItem> Find()
		{
			Verify.Argument.IsNotEmpty(Dir, "Dir");
			Verify.Argument.IsNotEmpty(FileMask, "FileMask");
			Verify.Argument.IsNotEmpty(FindText, "FindText");

			string[] filesInDirectory = Utils.GetFilesInDirectory(Dir, FileMask, IncludeSubDirectories);

			var resultItems = new List<FindResultItem>();

			//Analyze each file in the directory
			foreach (string filePath in filesInDirectory)
			{
				var resultItem = new FindResultItem();

				resultItem.FileName = Path.GetFileName(filePath);
				resultItem.FilePath = filePath;
				resultItem.FileRelativePath = "." + filePath.Substring(Dir.Length);
				resultItem.Matches = GetMatches(filePath);
				resultItem.NumMatches = resultItem.Matches.Count;




				//Skip files that don't have matches
				if (resultItem.NumMatches > 0)
				{
					var linesToTooltip = new List<int>();

					foreach (Match match in resultItem.Matches)
					{
						linesToTooltip.AddRange(GetLineNumbersForTooltip(filePath, match));
					}

					resultItem.ToolTip = GenerateToolTip(filePath, linesToTooltip);
					resultItems.Add(resultItem);
				}

				OnFileProcessed(new FinderEventArgs(resultItem, filesInDirectory.Length));
			}

			if (filesInDirectory.Length == 0) OnFileProcessed(new FinderEventArgs(new FindResultItem(), filesInDirectory.Length));

			return resultItems;
		}


		public event FileProcessedEventHandler FileProcessed;

		protected virtual void OnFileProcessed(FinderEventArgs e)
		{
			if (FileProcessed != null)
				FileProcessed(this, e);
		}

		private int GetNumMatches(string filePath)
		{
			string content = string.Empty;

			using (var sr = new StreamReader(filePath))
			{
				content = sr.ReadToEnd();
			}

			var result = Regex.Matches(content, FindText, Utils.GetRegExOptions(IsCaseSensitive));



			return result.Count;
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

		private List<int> GetLineNumbersForTooltip(string filePath, Match match)
		{
			string content = string.Empty;

			using (var sr = new StreamReader(filePath))
			{
				content = sr.ReadToEnd();
			}

			var separator = "\r\n";

			var lines = content.Split(separator.ToCharArray());
			
			var clearLines = new List<string>();
			for (int i = 0; i < lines.Count(); i++)
				if (i % 2 == 0) clearLines.Add(lines[i]);

			var lineIndex = DetectMatchLine(clearLines.ToArray(), match.Index);

			var result = new List<int>();

			for (int i = lineIndex - 2; i <= lineIndex + 2; i++)
			{
				if (i >= 0 && i < clearLines.Count())
					result.Add(i);
			}

			return result;

		}

		private int DetectMatchLine(string[] lines, int position)
		{
			var separator = "/r/n";
			int i = 0;
			int charsCount = lines[0].Length + separator.Length;

			while (charsCount < position)
			{
				i++;
				charsCount += lines[i].Length + separator.Length;
			}

			return i;
		}

		private string GenerateToolTip(string filePath, List<int> rowNumbers)
		{
			string content = string.Empty;

			using (var sr = new StreamReader(filePath))
			{
				content = sr.ReadToEnd();
			}

			var separator = "\r\n";

			var lines = content.Split(separator.ToCharArray());
			lines = lines.Where(s => !String.IsNullOrEmpty(s)).ToArray();

			StringBuilder stringBuilder=new StringBuilder();

			rowNumbers = rowNumbers.Distinct().OrderBy(r=>r).ToList();
			var prevLineIndex = 0;
			foreach (var rowNumber in rowNumbers)
			{
				if (rowNumber-prevLineIndex >1 ) stringBuilder.AppendLine("");
				stringBuilder.AppendLine(lines[rowNumber]);
				prevLineIndex = rowNumber;
			}

			return stringBuilder.ToString();
		}

	}
}
