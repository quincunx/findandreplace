using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using href.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace FindAndReplace
{

	public class FinderEventArgs : EventArgs
	{
		public Finder.FindResultItem ResultItem { get; set; }

		public Stats Stats { get; set; }

		public Status Status { get; set; }

		public bool Silent { get; set; }

		public FinderEventArgs(Finder.FindResultItem resultItem, Stats stats, Status status, bool silent = false)
		{
			ResultItem = resultItem;
			Stats = stats;
			Status = status;
			Silent = silent;
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
		public string ExcludeFileMask { get; set; }
		public bool IsCancelRequested { get; set; }
		public bool Silent { get; set; }
		public int NumThreads { get; set; }

		public class FindResultItem : ResultItem
		{
		}

		public class FindResult
		{
			public List<FindResultItem> Items { get; set; }
			public Stats Stats { get; set; }
		}


		public Finder()
		{
			NumThreads = 1;
		}


		public FindResult Find()
		{
			Verify.Argument.IsNotEmpty(Dir, "Dir");
			Verify.Argument.IsNotEmpty(FileMask, "FileMask");
			Verify.Argument.IsNotEmpty(FindText, "FindText");

			Status status = Status.Processing;
			
			StopWatch.Start("GetFilesInDirectory");

			//time
			var startTime = DateTime.Now;
			
			string[] filesInDirectory = Utils.GetFilesInDirectory(Dir, FileMask, IncludeSubDirectories, ExcludeFileMask);

			var resultItems = new List<FindResultItem>();
			var stats = new Stats();
			stats.Files.Total = filesInDirectory.Length;

			StopWatch.Stop("GetFilesInDirectory");


			var startTimeProcessingFiles = DateTime.Now;

			//Old code
			foreach (string filePath in filesInDirectory)


				//Analyze each file in the directory
				//Parallel.ForEach(filesInDirectory,
				//				 new ParallelOptions {MaxDegreeOfParallelism = NumThreads},
				//				 (filePath, state) =>
			{
				stats.Files.Processed++;

				
				var resultItem = FindInFile(filePath);

				
				
				//Update stats
				if (resultItem.IsBinaryFile)
					stats.Files.Binary++;

				if (resultItem.IsSuccess)
				{
					stats.Matches.Found += resultItem.Matches.Count;

					if (resultItem.Matches.Count > 0)
						stats.Files.WithMatches++;
					else
						stats.Files.WithoutMatches++;
				}

				stats.UpdateTime(startTime, startTimeProcessingFiles);


				//Update status
				if (IsCancelRequested)
					status = Status.Cancelled;

				if (stats.Files.Total == stats.Files.Processed)
					status = Status.Completed;


				//Skip files that don't have matches
				if (String.IsNullOrEmpty(resultItem.ErrorMessage) || resultItem.NumMatches > 0)
					resultItems.Add(resultItem);

				//Handle event
				OnFileProcessed(new FinderEventArgs(resultItem, stats, status, Silent));

			
				if (status == Status.Cancelled)
					break;
				//state.Break();
				//});
			}
			
		
			if (filesInDirectory.Length == 0)
			{
				status = Status.Completed;
				OnFileProcessed(new FinderEventArgs(new FindResultItem(), stats, status, Silent));
			}

			return new FindResult {Items = resultItems, Stats = stats};
		}

		private FindResultItem FindInFile(string filePath)
		{
			var resultItem = new FindResultItem();
			resultItem.IsSuccess = true;

			resultItem.FileName = Path.GetFileName(filePath);
			resultItem.FilePath = filePath;
			resultItem.FileRelativePath = "." + filePath.Substring(Dir.Length);

			StopWatch.Start("CheckIfBinary");

			//Load 1KB or 10KB of data and check for /0/0/0/0
			CheckIfBinary(filePath, resultItem);


			StopWatch.Stop("CheckIfBinary");

			if (resultItem.IsSuccess)
			{
				//StopWatch.Start("DetectFileEncoding");

				Encoding encoding = Utils.DetectFileEncoding(filePath);
				resultItem.FileEncoding = encoding;

				//StopWatch.Stop("DetectFileEncoding");

				StopWatch.Start("ReadFileContent");

				string fileContent;
				using (var sr = new StreamReader(filePath, encoding))
				{
					fileContent = sr.ReadToEnd();
				}

				StopWatch.Stop("ReadFileContent");

				StopWatch.Start("FindMatches");
				resultItem.Matches = GetMatches(fileContent);
				StopWatch.Stop("FindMatches");

				StopWatch.Start("GetLineNumbersForMatchesPreview");
				resultItem.LineNumbers = Utils.GetLineNumbersForMatchesPreview(filePath, resultItem.Matches);
				StopWatch.Stop("GetLineNumbersForMatchesPreview");
			
				resultItem.NumMatches = resultItem.Matches.Count;
			}

			return resultItem;
		}

		public void CancelFind()
		{
			IsCancelRequested = true;
		}

		private void CheckIfBinary(string filePath, FindResultItem resultItem)
		{
			string shortContent = string.Empty;

			//Check if can read first
			try
			{
				shortContent = Utils.GetFileContentSample(filePath);
			}
			catch (Exception exception)
			{
				resultItem.IsSuccess = false;
				resultItem.FailedToOpen = true;
				resultItem.ErrorMessage = exception.Message;
			}


			if (resultItem.IsSuccess)
			{
				// check for /0/0/0/0
				if (Utils.IsBinaryFile(shortContent))
				{
					resultItem.IsSuccess = false;
					resultItem.IsBinaryFile = true;
				}
			}
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
