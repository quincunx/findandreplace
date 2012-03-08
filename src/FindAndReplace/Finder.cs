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
			public bool IsBinaryFile { get; set; }
			public string ErrorMessage { get; set; }

			public bool IncludeInResultsList
			{
				get
				{
					if (IsSuccess && NumMatches > 0)
						return true;

					if (!IsSuccess && !String.IsNullOrEmpty(ErrorMessage))
						return true;

					return false;
				}
			}
		}

		public class FindResult
		{
			public List<FindResultItem> Items { get; set; }
			public Stats Stats { get; set; }
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
						fileContent = sr.ReadToEnd();
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
					if (!Utils.IsBinaryFile(fileContent))
					{
						resultItem.Matches = GetMatches(fileContent);
						resultItem.NumMatches = resultItem.Matches.Count;

						stats.TotalMatches += resultItem.Matches.Count;

						if (resultItem.Matches.Count > 0)
							stats.FilesWithMatches++;
						else
							stats.FilesWithoutMatches++;
					}
					else
					{
						resultItem.IsSuccess = false;
						stats.BinaryFiles++;
					}
				}


				//Skip files that don't have matches
				if (String.IsNullOrEmpty(resultItem.ErrorMessage) || resultItem.NumMatches > 0)
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
	}
}
