using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;
using CommandLine;

namespace FindAndReplace.Console
{
	
	class Program
	{
		static void Main(string[] args)
		{
			var options = new CommandLineOptions();
			ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(System.Console.Error));
			if (!parser.ParseArguments(args, options, System.Console.Error))
			{
				System.Console.ReadKey();
				Environment.Exit(1);
			}

			if (!String.IsNullOrEmpty(options.ReplaceText))
			{
				var replacer = new Replacer();
				replacer.Dir = options.Dir;
				replacer.FileMask = options.FileMask;
				replacer.FindText = options.FindText;
				replacer.ReplaceText = options.ReplaceText;

				var result = replacer.Replace();
				DisplayReplaceResult(result);
			}
			else
			{
				var finder = new Finder();
				finder.Dir = options.Dir;
				finder.FileMask = options.FileMask;
				finder.FindText = options.FindText;
				finder.IsCaseSensitive = options.IsCaseSensitive;

				var result = finder.Find();
				DisplayFindResult(result);
			}
			System.Console.ReadLine();
		}

		static void DisplayFindResult(List<Finder.FindResultItem> resultItems)
		{
			PrintLine();
			PrintRow("File Name", "Path", "Matches", "");
			PrintLine();
			foreach (var item in resultItems)
			{
				PrintRow(item.FileName, item.FilePath.Substring(0, 17), item.NumMatches.ToString(), "");
			}
			PrintLine();
		}

		static void DisplayReplaceResult(List<Replacer.ReplaceResultItem> resultItems)
		{
			PrintLine();
			PrintRow("File Name", "Path", "Matches", "Is Success");
			PrintLine();
			foreach (var item in resultItems)
			{
				PrintRow(item.FileName, item.FilePath.Substring(0, 17), item.NumMatches.ToString(), item.IsSuccess.ToString());
			}
			PrintLine();
		}

		static void PrintLine()
		{
			System.Console.WriteLine(new string('-', 73));
		}

		static void PrintRow(string column1, string column2, string column3, string column4)
		{
			System.Console.WriteLine(
				string.Format("|{0}|{1}|{2}|{3}|",
					AlignCentre(column1, 17),
					AlignCentre(column2, 17),
					AlignCentre(column3, 17),
					AlignCentre(column4, 17)));
		}

		static string AlignCentre(string text, int width)
		{
			if (string.IsNullOrEmpty(text))
			{
				return new string(' ', width);
			}
			return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
		}

	}
}
