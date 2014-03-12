using CommandLine;

namespace FindAndReplace.Tests.CommandLine
{
	public class CommandLineOptions
	{
		#region Standard Option Attribute

		[Option("testVal", Required = true, HelpText = "Test value to parse.")]
		public string TestValue { get; set; }

		[Option("isRegex", HelpText = "Is Regex.")]
		public bool IsRegex { get; set; }

		[Option("useEscapeChars", HelpText = "Use Escape Chars.")]
		public bool UseEscape { get; set; }

		#endregion
	}
}
