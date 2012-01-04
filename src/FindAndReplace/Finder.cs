using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

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
				resultItem.FileRelativePath = "." + filePath.Substring(Dir.Length );
				resultItem.NumMatches = GetNumMatches(filePath);

				OnFileProcessed(new FinderEventArgs(resultItem, filesInDirectory.Length));
	
				//Skip files that don't have matches
				if (resultItem.NumMatches > 0)
					resultItems.Add(resultItem);
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

			return Regex.Matches(content, FindText, Utils.GetRegExOptions(IsCaseSensitive)).Count;
		}

		

	}
}
