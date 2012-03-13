using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace FindAndReplace
{
	public static class Utils
	{
		public static RegexOptions GetRegExOptions(bool isCaseSensitive)
		{
			//Create a new option
			var options = new RegexOptions();

			//Is the match case check box checked
			if (!isCaseSensitive)
				options |= RegexOptions.IgnoreCase;

			//Return the options
			return options;
		}

		public static string[] GetFilesInDirectory(string dir, string fileMask, bool includeSubDirectories)
		{
			SearchOption searchOption = includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			var filesInDirectory = new List<string>();
			var fileMasks = fileMask.Split(',');
			foreach (var mask in fileMasks)
			{
				filesInDirectory.AddRange(Directory.GetFiles(dir, mask.Trim(), searchOption));
			}

			return filesInDirectory.Distinct().ToArray();
		}

		public static bool IsBinaryFile(string fileContent)
		{
			//http://stackoverflow.com/questions/910873/how-can-i-determine-if-a-file-is-binary-or-text-in-c
			if (fileContent.Contains("\0\0\0\0"))
				return true;

			return false;
		}

		public static List<MatchPreviewLineNumber> GetLineNumbersForMatchesPreview(string filePath, MatchCollection matches)
		{
			string content = string.Empty;

			using (var sr = new StreamReader(filePath))
			{
				content = sr.ReadToEnd();
			}

			var separator = Environment.NewLine;
			var lines = content.Split(new string[] { separator }, StringSplitOptions.None);

			var result = new List<MatchPreviewLineNumber>();
			var temp = new List<MatchPreviewLineNumber>();

			foreach (Match match in matches)
			{
				var lineIndexStart = DetectMatchLine(lines.ToArray(), match.Index);
				var lineIndexEnd = DetectMatchLine(lines.ToArray(), match.Index + match.Length);

				for (int i = lineIndexStart - 2; i <= lineIndexEnd + 2; i++)
				{
					if (i >= 0 && i < lines.Count())
					{
						var lineNumber = new MatchPreviewLineNumber();
						lineNumber.LineNumber = i;
						lineNumber.HasMatch = (i >= lineIndexStart && i <= lineIndexEnd) ? true : false;
						temp.Add(lineNumber);
					}
				}
			}

			result.AddRange(temp.Where(ln => ln.HasMatch).Distinct(new LineNumberComparer()));

			result.AddRange(temp.Where(ln => !ln.HasMatch && !result.Select(l => l.LineNumber).Contains(ln.LineNumber)).Distinct(new LineNumberComparer()));

			return result.OrderBy(ln => ln.LineNumber).ToList();
		}

		public static string FormatTimeSpan(TimeSpan timeSpan)
		{
			string result = String.Empty;

			int h = timeSpan.Hours;
			int m = timeSpan.Minutes;
			int s = timeSpan.Seconds;

			if (h > 0)
			{
				result += String.Format("{0}h ", h);

				if (m > 0)
				{
					result += String.Format("{0}m ", m);

					if (s > 0) result += String.Format("{0}s ", s);
				}
				else
				{
					if (s > 0)
					{
						result += String.Format("{0}m ", m);

						result += String.Format("{0}s ", s);
					}
				}

			}
			else
			{
				if (m > 0) result += String.Format("{0}m ", m);

				if (s > 0) result += String.Format("{0}s ", s);
			}

			return result;
		}

		private static int DetectMatchLine(string[] lines, int position)
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
}
