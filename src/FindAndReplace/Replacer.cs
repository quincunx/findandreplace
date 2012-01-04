using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FindAndReplace
{
	public class ReplacerEventArgs : EventArgs
	{
		public Replacer.ReplaceResultItem ResultItem { get; set; }
		public int TotalFilesCount { get; set; }

		public ReplacerEventArgs(Replacer.ReplaceResultItem resultItem, int fileCount)
		{
			ResultItem = resultItem;
			TotalFilesCount = fileCount;
		}
	}

	public delegate void ReplaceFileProcessedEventHandler(object sender, ReplacerEventArgs e);
	
	public class Replacer
	{
		public string Dir { get; set; }
		public string FileMask { get; set; }
		public bool IncludeSubDirectories { get; set; }
		public string FindText { get; set; }
		public string ReplaceText { get; set; }
		public bool IsCaseSensitive { get; set; }
		

		public class ReplaceResultItem 
		{
			public string FileName { get; set; }
			public string FilePath { get; set; }
			public string FileRelativePath { get; set; }
			public int NumMatches { get; set; }
			public bool IsSuccess { get; set; }
			public string ErrorMessage { get; set; }
		}

		public List<ReplaceResultItem> Replace()
		{
			Verify.Argument.IsNotEmpty(Dir, "Dir");
			Verify.Argument.IsNotEmpty(FileMask, "FileMask");
			Verify.Argument.IsNotEmpty(FindText, "FindText");
			Verify.Argument.IsNotEmpty(FindText, "ReplaceText");

			string[] filesInDirectory = Utils.GetFilesInDirectory(Dir, FileMask, IncludeSubDirectories);

			var resultItems = new List<ReplaceResultItem>();

			foreach (string filePath in filesInDirectory)
			{
				var resultItem = ReplaceTextInFile(filePath);

				//Skip files that don't have matches
				if (resultItem.NumMatches > 0)
					resultItems.Add(resultItem);

				OnFileProcessed(new ReplacerEventArgs(resultItem, filesInDirectory.Length));
			}

			if (filesInDirectory.Length == 0) OnFileProcessed(new ReplacerEventArgs(new ReplaceResultItem(), filesInDirectory.Length));

			return resultItems;
		}
		
		private ReplaceResultItem ReplaceTextInFile(string filePath)
		{
			string content = string.Empty;

			using (StreamReader sr = new StreamReader(filePath))
			{
				content = sr.ReadToEnd();
			}

			RegexOptions regexOptions = Utils.GetRegExOptions(IsCaseSensitive);

			int matchCount = Regex.Matches(content, Regex.Escape(FindText), regexOptions).Count;
			if (matchCount  > 0)
			{
				string newContent = Regex.Replace(content, Regex.Escape(FindText), ReplaceText, regexOptions);

				using (var sw = new StreamWriter(filePath))
				{
					sw.Write(newContent);
				}
			}


			var resultItem = new ReplaceResultItem();

			resultItem.FileName = Path.GetFileName(filePath);
			resultItem.FilePath = filePath;
			resultItem.FileRelativePath = "." + filePath.Substring(Dir.Length);
			
			resultItem.NumMatches = matchCount;
			resultItem.IsSuccess = matchCount > 0;

			return resultItem;
		}

		public event ReplaceFileProcessedEventHandler FileProcessed;

		protected virtual void OnFileProcessed(ReplacerEventArgs e)
		{
			if (FileProcessed != null)
				FileProcessed(this, e);
		}
	}
}
