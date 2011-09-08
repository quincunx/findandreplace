using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FindAndReplace
{
	public class Finder
	{
		public string Dir { get; set; }
		public string FileMask { get; set; }
		public string FindText { get; set; }
		public bool IsCaseSensitive { get; set; }
	
		public class FindResultItem
		{
			public string FileName { get; set; }
			public string FilePath { get; set; }
			public int NumMatches { get; set; }
		}

		public List<FindResultItem> Find()
		{
			//Get all txt files in the directory
			string[] filesInDirectory = Directory.GetFiles(Dir, FileMask, SearchOption.AllDirectories);

			var resultItems = new List<FindResultItem>();
			//Analyze each file in the directory
			foreach (string filePath in filesInDirectory)
			{
				var resultItem = new FindResultItem();

				resultItem.FileName = Path.GetFileName(filePath);
				resultItem.FilePath = filePath;
				resultItem.NumMatches = GetNumMatches(filePath);

				resultItems.Add(resultItem);
			}

			return resultItems;
		}

		private int GetNumMatches(string filePath)
		{

			string content = string.Empty;

			//Create a new object to read a file	
			using (var sr = new StreamReader(filePath))
			{
				//Read the file into the string variable.
				content = sr.ReadToEnd();
			}

			return Regex.Matches(content, FindText, GetRegExOptions()).Count;
		}


		private RegexOptions GetRegExOptions()
		{
			//Create a new option
			var options = new RegexOptions();

			//Is the match case check box checked
			if (!IsCaseSensitive)
				options |= RegexOptions.IgnoreCase;

			//Return the options
			return options;
		}

	}
}
