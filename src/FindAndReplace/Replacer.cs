using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FindAndReplace
{
	public class Replacer
	{
		public string Dir { get; set; }
		public string FileMask { get; set; }
		public string FindText { get; set; }
		public string ReplaceText { get; set; }
		public bool IsCaseSensitive { get; set; }
	

		public class ReplaceResultItem 
		{
			public string FileName { get; set; }
			public string FilePath { get; set; }
			public int NumMatches { get; set; }
			public bool IsSuccess { get; set; }
			public string ErrorMessage { get; set; }
		}

		public List<ReplaceResultItem> Replace()
		{
			string[] filesInDirectory = Directory.GetFiles(Dir, FileMask, SearchOption.AllDirectories);

			var resultItems = new List<ReplaceResultItem>();

			foreach (string filePath in filesInDirectory)
			{
				var resultItem = ReplaceTextInFile(filePath);
				resultItems.Add(resultItem);
			}

			return resultItems;
		}


		private ReplaceResultItem ReplaceTextInFile(string filePath)
		{
			//holds the content of the file
			string content = string.Empty;

			//Create a new object to read a file	
			using (StreamReader sr = new StreamReader(filePath))
			{
				//Read the file into the string variable.
				content = sr.ReadToEnd();
			}


			//Look for a match
			int matchCount = Regex.Matches(content, FindText, GetRegExOptions()).Count;
			if (matchCount  > 0)
			{
				//Replace the text
				string newContent = Regex.Replace(content, FindText, ReplaceText, GetRegExOptions());

				//Create a new object to write a file
				using (var sw = new StreamWriter(filePath))
				{
					//Write the updated file
					sw.Write(newContent);
				}

				//A match was found and replaced
			}


			var resultItem = new ReplaceResultItem();

			resultItem.FileName = Path.GetFileName(filePath);
			resultItem.FilePath = filePath;
			resultItem.NumMatches = matchCount;
			resultItem.IsSuccess = matchCount > 0;

			return resultItem;
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
