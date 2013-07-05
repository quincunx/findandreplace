using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using href.Utils;

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

		public static string[] GetFilesInDirectory(string dir, string fileMask, bool includeSubDirectories, string excludeMask)
		{
			SearchOption searchOption = includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			var filesInDirectory = new List<string>();
			var fileMasks = fileMask.Split(',');
			foreach (var mask in fileMasks)
			{
				filesInDirectory.AddRange(Directory.GetFiles(dir, mask.Trim(), searchOption));
			}

			filesInDirectory = filesInDirectory.Distinct().ToList();

			if (!String.IsNullOrEmpty(excludeMask))
			{
				var excludeFileMasks = excludeMask.Split(',');
				filesInDirectory = excludeFileMasks
					.Select(excludeFileMask => WildcardToRegex(excludeFileMask.Trim()))
					.Aggregate(filesInDirectory,
							   (current, excludeMaskRegEx) => current.Where(f => !Regex.IsMatch(f, excludeMaskRegEx)).ToList());
			}

			filesInDirectory.Sort();
			return filesInDirectory.ToArray();
		}

		public static bool IsBinaryFile(string fileContent)
		{
			//http://stackoverflow.com/questions/910873/how-can-i-determine-if-a-file-is-binary-or-text-in-c
			if (fileContent.Contains("\0\0\0\0"))
				return true;

			return false;
		}


		public static bool IsBinaryFile(byte[] bytes)
		{
			string text = System.Text.Encoding.Default.GetString(bytes);
			return IsBinaryFile(text);
		}


		public static List<MatchPreviewLineNumber> GetLineNumbersForMatchesPreview(string fileContent, MatchCollection matches, int replaceStrLength = 0, bool isReplace = false)
		{
			var separator = Environment.NewLine;
			var lines = fileContent.Split(new string[] { separator }, StringSplitOptions.None);

			var temp = new List<MatchPreviewLineNumber>();

			int replacedTextLength = 0;

			foreach (Match match in matches)
			{
				var lineIndexStart = DetectMatchLine(lines.ToArray(), GetMatchIndex(match.Index, replacedTextLength, isReplace));
				var lineIndexEnd = DetectMatchLine(lines.ToArray(), GetMatchIndex(match.Index + replaceStrLength, replacedTextLength, isReplace));

				replacedTextLength += match.Length;

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

			return temp.Distinct(new LineNumberComparer()).OrderBy(ln => ln.LineNumber).ToList();
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

		//from http://www.roelvanlisdonk.nl/?p=259
		private static string WildcardToRegex(string pattern)
		{
			return string.Format("^{0}$", Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", "."));
		}

		public static byte[] ReadFileContentSample(string filePath, int maxSize = 10240)
		{
			byte[] buffer;
  			using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
			{
				long streamLength = stream.Length;
				long bufferSize = Math.Min(streamLength, maxSize);

				buffer= new byte[bufferSize];

				stream.Read(buffer, 0, (int)bufferSize);
			}

			return buffer;
		}


		private static int GetMatchIndex(int originalIndex, int replacedTextLength, bool isReplace = false)
		{
			if (!isReplace) return originalIndex;

			var newIndex = originalIndex - replacedTextLength;

			return newIndex;
		}
	}
}
