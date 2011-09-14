using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CommandLine;

namespace FindAndReplace
{
	//from http://www.rootsilver.com/2007/08/how-to-create-a-consolewindow
	static class Program
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool FreeConsole();

		[DllImport("kernel32", SetLastError = true)]
		static extern bool AttachConsole(int dwProcessId);

		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (args.Length!=0 && args[0]=="--cl")
			{

				//Get a pointer to the forground window.  The idea here is that
				//IF the user is starting our application from an existing console
				//shell, that shell will be the uppermost window.  We'll get it
				//and attach to it
				IntPtr ptr = GetForegroundWindow();

				int u;

				GetWindowThreadProcessId(ptr, out u);

				Process process = Process.GetProcessById(u);

				if (process.ProcessName == "cmd")    //Is the uppermost window a cmd process?
				{
					AttachConsole(process.Id);
				}
				else
				{
					//no console AND we're in console mode ... create a new console.

					AllocConsole();
				}

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
					replacer.IsCaseSensitive = options.IsCaseSensitive;
					replacer.IncludeSubDirectories = options.IncludeSubDirectories;

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
					finder.IncludeSubDirectories = options.IncludeSubDirectories;

					var result = finder.Find();
					DisplayFindResult(result);
				}
				System.Console.ReadLine();

				FreeConsole();
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainForm());
				
			}
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
