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
									.Aggregate(filesInDirectory, (current, excludeMaskRegEx) => current.Where(f => !Regex.IsMatch(f, excludeMaskRegEx)).ToList());
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

		//from http://www.roelvanlisdonk.nl/?p=259
		private static string WildcardToRegex(string pattern)
		{
			return string.Format("^{0}$", Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", "."));
		}

		public static string GetFileContentSample(string filePath, int size = 10240)
		{
			var buffer = new char[10240];
			string shortContent;

			using (var sr = new StreamReader(filePath))
			{
				int k = sr.Read(buffer, 0, 10240);

				shortContent = new string(buffer, 0, k);
				//shortContent = sr.ReadToEnd();
			}

			return shortContent;
		}


		public static Encoding DetectFileEncoding(string filePath)
		{
			Encoding encoding = null;
			string method = "Default";

			using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
			{
				//First read only what we need for BOM detection
				stream.Seek(0, SeekOrigin.Begin);
				encoding = DetectEncodingUsingBom(stream);
				if (encoding != null)
					method = "BOM";

				if (encoding == null)
				{
					stream.Seek(0, SeekOrigin.Begin);
					encoding = DetectEncodingUsingMLang(stream);

					if (encoding != null)
						method = "MLang";
				}	
			}
			

			if (encoding == null)
				encoding = Encoding.UTF8; //Use UTF8 by default
			
			Console.WriteLine(method.PadRight(10, ' ') + " | "+  filePath.PadRight(100, ' ') + " | " + encoding.EncodingName);
			return encoding;
		}

		//From http://www.architectshack.com/TextFileEncodingDetector.ashx
		private static Encoding DetectEncodingUsingBom(Stream fileStream)
		{
			byte[] bomBytes = new byte[fileStream.Length > 4 ? 4 : fileStream.Length];
			fileStream.Read(bomBytes, 0, bomBytes.Length);

			Encoding encoding = DetectBOMBytes(bomBytes);
			return encoding;
		}


		public static Encoding DetectBOMBytes(byte[] BOMBytes)
		{
			if (BOMBytes == null)
				throw new ArgumentNullException("BOMBytes");

			if (BOMBytes.Length < 2)
				return null;

			if (BOMBytes[0] == 0xff
				&& BOMBytes[1] == 0xfe
				&& (BOMBytes.Length < 4
					|| BOMBytes[2] != 0
					|| BOMBytes[3] != 0
					)
				)
				return Encoding.Unicode;

			if (BOMBytes[0] == 0xfe
				&& BOMBytes[1] == 0xff
				)
				return Encoding.BigEndianUnicode;

			if (BOMBytes.Length < 3)
				return null;

			if (BOMBytes[0] == 0xef && BOMBytes[1] == 0xbb && BOMBytes[2] == 0xbf)
				return Encoding.UTF8;

			if (BOMBytes[0] == 0x2b && BOMBytes[1] == 0x2f && BOMBytes[2] == 0x76)
				return Encoding.UTF7;

			if (BOMBytes.Length < 4)
				return null;

			if (BOMBytes[0] == 0xff && BOMBytes[1] == 0xfe && BOMBytes[2] == 0 && BOMBytes[3] == 0)
				return Encoding.UTF32;

			if (BOMBytes[0] == 0 && BOMBytes[1] == 0 && BOMBytes[2] == 0xfe && BOMBytes[3] == 0xff)
				return Encoding.GetEncoding(12001);

			return null;
		}



		private static Encoding DetectEncodingUsingMLang(Stream fileStream)
		{
			long length = fileStream.Length;
			var buf = new byte[length];
			fileStream.Read(buf, 0, buf.Length);

			try
			{
				Encoding[] detected = EncodingTools.DetectInputCodepages(buf, 1);
				if (detected.Length > 0)
				{
					return detected[0];
				}
			}
			catch (COMException)
			{
				// return default codepage on error
			}

			return null;
		}
	}
}
