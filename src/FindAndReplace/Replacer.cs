using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FindAndReplace
{
	public class ReplacerEventArgs : EventArgs
	{
		public Replacer.ReplaceResultItem ResultItem { get; set; }
		public Stats Stats { get; set; }

		public ReplacerEventArgs(Replacer.ReplaceResultItem resultItem, Stats stats)
		{
			ResultItem = resultItem;
			Stats = stats;
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
		public bool FindTextHasRegEx { get; set; }
		
		public class ReplaceResultItem 
		{
			public string FileName { get; set; }
			public string FilePath { get; set; }
			public string FileRelativePath { get; set; }
			public int NumMatches { get; set; }
			public bool IsSuccess { get; set; }
			public string ErrorMessage { get; set; }
			public MatchCollection Matches { get; set; }
			public bool FailedToOpen { get; set; }
			public bool FailedToWrite { get; set; }
		}

		public class ReplaceResult
		{
			public List<ReplaceResultItem> ResultItems { get; set; }

			public Stats Stats { get; set; }
		}

		public ReplaceResult Replace()
		{
			Verify.Argument.IsNotEmpty(Dir, "Dir");
			Verify.Argument.IsNotEmpty(FileMask, "FileMask");
			Verify.Argument.IsNotEmpty(FindText, "FindText");
			Verify.Argument.IsNotEmpty(FindText, "ReplaceText");

			string[] filesInDirectory = Utils.GetFilesInDirectory(Dir, FileMask, IncludeSubDirectories);

			var resultItems = new List<ReplaceResultItem>();
			var stats = new Stats();
			stats.TotalFiles = filesInDirectory.Length;

			foreach (string filePath in filesInDirectory)
			{
				var resultItem = ReplaceTextInFile(filePath);
				stats.ProcessedFiles++;
				stats.TotalMatches += resultItem.NumMatches;

				if (resultItem.IsSuccess)
				{
					if (resultItem.NumMatches > 0)
					{
						stats.FilesWithMatches++;
						stats.TotalReplaces += resultItem.NumMatches;
					}
					else
					{
						stats.FilesWithoutMatches++;
					}
				}
				else
				{
					if (resultItem.FailedToOpen)
					{
						stats.FailedToOpen++;
					}

					if (resultItem.FailedToWrite)
					{
						stats.FailedToWrite++;
					}
				}
				
				if (resultItem.NumMatches > 0 || !resultItem.IsSuccess)
					resultItems.Add(resultItem);
				
				OnFileProcessed(new ReplacerEventArgs(resultItem, stats));
			}

			if (filesInDirectory.Length == 0) 
				OnFileProcessed(new ReplacerEventArgs(new ReplaceResultItem(), stats));

			return new ReplaceResult() {ResultItems = resultItems, Stats = stats};
		}
		
		private ReplaceResultItem ReplaceTextInFile(string filePath)
		{
			string content = string.Empty;

			var resultItem = new ReplaceResultItem();
			resultItem.IsSuccess = true;
			resultItem.FileName = Path.GetFileName(filePath);
			resultItem.FilePath = filePath;
			resultItem.FileRelativePath = "." + filePath.Substring(Dir.Length);

			try
			{
				using (StreamReader sr = new StreamReader(filePath))
				{
					content = sr.ReadToEnd();
				}
			}
			catch (Exception exception)
			{
				resultItem.IsSuccess = false;
				resultItem.FailedToOpen = true;
				resultItem.ErrorMessage = exception.Message;
				
				return resultItem;
			}
			
			
			RegexOptions regexOptions = Utils.GetRegExOptions(IsCaseSensitive);

			var finderText = FindTextHasRegEx ? FindText : Regex.Escape(FindText);
			MatchCollection matches;

			if (!FindTextHasRegEx)
			{
				matches= Regex.Matches(content, Regex.Escape(FindText), Utils.GetRegExOptions(IsCaseSensitive));
			}
			else
			{
				matches = Regex.Matches(content, finderText, regexOptions);
			}

			
			resultItem.NumMatches = matches.Count;
			resultItem.Matches = matches;
		
			if (matches.Count > 0)
			{
				try
				{
					string newContent = Regex.Replace(content, finderText, ReplaceText, regexOptions);

					using (var sw = new StreamWriter(filePath))
					{
						sw.Write(newContent);
					}
				}
				catch (Exception ex)
				{
					resultItem.IsSuccess = false;
					resultItem.FailedToWrite = true;
					resultItem.ErrorMessage = ex.Message;
				}
			}
			

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
