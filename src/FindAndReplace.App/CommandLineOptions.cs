using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace FindAndReplace.App
{
	public class CommandLineOptions
	{
		#region Standard Option Attribute

		[Option("cl", HelpText = "Required to run on command line.")]
		public bool UseCommandLine { get; set; } 

		[Option("find", Required = true, HelpText = "Text to find.")]
		public string FindText { get; set; }

		[Option("replace", HelpText = "Replacement text.")]
		public string ReplaceText { get; set; }

		[Option("caseSensitive", HelpText = "Case-sensitive.")]
		public bool IsCaseSensitive { get; set; }

		[Option("useRegEx", HelpText = "Find text has Regular Expression.")]
		public bool IsFindTextHasRegEx { get; set; }

		[Option("dir", Required = true, HelpText = "Directory path.")]
		public string Dir { get; set; }

		[Option("fileMask", Required = true, HelpText = "File mask.")]
		public string FileMask { get; set; }

		[Option("excludeFileMask",  HelpText = "Exclude file mask.")]
		public string ExcludeFileMask { get; set; }

		[Option("includeSubDirectories", HelpText = "Include files in SubDirectories.")]
		public bool IncludeSubDirectories { get; set; }

		[Option("showEncoding", HelpText = "Display detected encoding information for each fle.")]
		public bool ShowEncoding { get; set; }

		[Option("silent", HelpText = "Supress the command window output.")]
		public bool Silent { get; set; }

		[Option("logFile", DefaultValue = null, HelpText = "Path to log file where to save command output.")]
		public string LogFile { get; set; }
		
		#endregion

		#region Specialized Option Attribute

		[HelpOption("help", HelpText = "Display this help screen.")]
		public string GetUsage()
		{
			var help = new HelpText("Find And Replace");

			help.Copyright = new CopyrightInfo("Entech Solutions", 2011);
			//HandleParsingErrorsInHelp();
			help.AddPreOptionsLine("Usage: \n\nfnr.exe --cl --find \"Text To Find\" --replace \"Text To Replace\"  --caseSensitive  --dir \"Directory Path\" --fileMask \"*.*\"  --includeSubDirectories --useRegEx");
			help.AddPreOptionsLine("\n");
			help.AddPreOptionsLine("Mask new line and quote characters using \\n and \\\".");


			help.AddOptions(this);

			return help;
		}


		//private void HandleParsingErrorsInHelp()
		//{
		//	if (this.LastPostParsingState.Errors.Count > 0)
		//	{
		//		var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
		//		if (!string.IsNullOrEmpty(errors))
		//		{
		//			help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
		//			help.AddPreOptionsLine(errors);
		//		}
		//	}
		//}

		#endregion
	}
}
