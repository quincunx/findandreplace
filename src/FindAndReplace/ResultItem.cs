using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FindAndReplace
{
	abstract public class ResultItem
	{
		public string FileName { get; set; }
		public string FilePath { get; set; }
		public string FileRelativePath { get; set; }
		public Encoding FileEncoding { get; set; }
		public int NumMatches { get; set; }
		public MatchCollection Matches { get; set; }
		public bool IsSuccess { get; set; }
		public bool IsBinaryFile { get; set; }
		public bool FailedToOpen { get; set; }
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
}
