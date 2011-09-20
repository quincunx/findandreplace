using System.IO;
using System.Text.RegularExpressions;

namespace FindAndReplace
{
	internal static class Utils
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
			string[] filesInDirectory = Directory.GetFiles(dir, fileMask, searchOption);
			return filesInDirectory;
		}
	}
}
