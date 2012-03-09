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
			public MatchCollection Matches { get; set; }
			public bool IsSuccess { get; set; }
			public bool IsBinaryFile { get; set; }
			public bool FailedToOpen { get; set; }
			public bool FailedToWrite { get; set; }
			public string ErrorMessage { get; set; }
			public List<MatchPreviewLineNumber> LineNumbers { get; set; }

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
			Verify.Argument.IsNotNull(ReplaceText, "ReplaceText");

			var startTime = DateTime.Now;
			string[] filesInDirectory = Utils.GetFilesInDirectory(Dir, FileMask, IncludeSubDirectories);

			var resultItems = new List<ReplaceResultItem>();
			var stats = new Stats();
			stats.Files.Total = filesInDirectory.Length;

			var startTimeProcessingFiles = DateTime.Now;
			
			foreach (string filePath in filesInDirectory)
			{
				var resultItem = ReplaceTextInFile(filePath);
				stats.Files.Processed++;
				stats.Matches.Found += resultItem.NumMatches;

				if (resultItem.IsSuccess)
				{
					if (resultItem.NumMatches > 0)
					{
						stats.Files.WithMatches++;
						stats.Matches.Replaced += resultItem.NumMatches;
					}
					else
					{
						stats.Files.WithoutMatches++;
					}
				}
				else
				{
					if (resultItem.FailedToOpen)
						stats.Files.FailedToRead++;
		
					if (resultItem.IsBinaryFile)
						stats.Files.Binary++;

					if (resultItem.FailedToWrite)
						stats.Files.FailedToWrite++;
				}
				
				if (resultItem.IncludeInResultsList)
					resultItems.Add(resultItem);

				stats.UpdateTime(startTime, startTimeProcessingFiles);
				
				OnFileProcessed(new ReplacerEventArgs(resultItem, stats));
			}

			if (filesInDirectory.Length == 0) 
				OnFileProcessed(new ReplacerEventArgs(new ReplaceResultItem(), stats));

			
			
			return new ReplaceResult() {ResultItems = resultItems, Stats = stats};
		}
		
		private ReplaceResultItem ReplaceTextInFile(string filePath)
		{
			string fileContent = string.Empty;

			var resultItem = new ReplaceResultItem();
			resultItem.IsSuccess = true;
			resultItem.FileName = Path.GetFileName(filePath);
			resultItem.FilePath = filePath;
			resultItem.FileRelativePath = "." + filePath.Substring(Dir.Length);

			try
			{
				using (StreamReader sr = new StreamReader(filePath))
				{
					fileContent = sr.ReadToEnd();
				}
			}
			catch (Exception exception)
			{
				resultItem.IsSuccess = false;
				resultItem.FailedToOpen = true;
				resultItem.ErrorMessage = exception.Message;
				
				return resultItem;
			}
			
			
			if (Utils.IsBinaryFile(fileContent))
			{
				resultItem.IsSuccess = false;
				resultItem.IsBinaryFile = true;
				return resultItem;
			}

			RegexOptions regexOptions = Utils.GetRegExOptions(IsCaseSensitive);

			var finderText = FindTextHasRegEx ? FindText : Regex.Escape(FindText);
			MatchCollection matches;

			if (!FindTextHasRegEx)
			{
				matches= Regex.Matches(fileContent, Regex.Escape(FindText), Utils.GetRegExOptions(IsCaseSensitive));
			}
			else
			{
				matches = Regex.Matches(fileContent, finderText, regexOptions);
			}

			
			resultItem.NumMatches = matches.Count;
			resultItem.Matches = matches;
		
			if (matches.Count > 0)
			{
				try
				{
					string newContent = Regex.Replace(fileContent, finderText, ReplaceText, regexOptions);

					using (var sw = new StreamWriter(filePath))
					{
						sw.Write(newContent);
					}

					resultItem.LineNumbers = Utils.GetLineNumbersForMatchesPreview(filePath, matches);
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
