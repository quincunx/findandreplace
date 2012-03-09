using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CommandLine;



namespace FindAndReplace.App
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
			// from http://blogs.msdn.com/b/microsoft_press/archive/2010/02/03/jeffrey-richter-excerpt-2-from-clr-via-c-third-edition.aspx
			AppDomain.CurrentDomain.AssemblyResolve += ResolveEventHandler;
			
			if (args.Length != 0 && args[0] == "--cl")
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

				CommandLineRunner.Run(args);

				FreeConsole();
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainForm());

			}
		}
		
		public static void PrintFinderResultRow(Finder.FindResultItem item, Stats stats)
		{
			PrintNameValuePair("File", item.FileRelativePath);
				
			if (item.IsSuccess)
				PrintNameValuePair("Matches", item.NumMatches.ToString());
			else
				PrintNameValuePair("Error", item.ErrorMessage);

			var passedSecondes = stats.Time.Passed.TotalSeconds;
			if (passedSecondes >= 1) PrintNameValuePair("Time Passed", Utils.FormatTimeSpan(stats.Time.Passed));

			Console.WriteLine();

		}

		public static void PrintReplacerResultRow(Replacer.ReplaceResultItem item, Stats stats)
		{
			PrintNameValuePair("File", item.FileRelativePath);
				
			if (!item.FailedToOpen)
				PrintNameValuePair("Matches", item.NumMatches.ToString());

			PrintNameValuePair("Replaced", item.IsSuccess ? "Yes" : "No");

			if (!item.IsSuccess)
				PrintNameValuePair("Error", item.ErrorMessage);

			var passedSecondes = stats.Time.Passed.TotalSeconds;
			if (passedSecondes >= 1) PrintNameValuePair("Time Passed", Utils.FormatTimeSpan(stats.Time.Passed));

			Console.WriteLine();
		}

		static void PrintNameValuePair(string name, string value)
		{
			string label = name + ":";
			label = label.PadRight(10);
			Console.WriteLine(label + value);
		}

		public static void PrintStatistics(Stats stats, bool isReplacerStats=false)
		{
			Console.WriteLine("");

			Console.WriteLine("====================================");
			Console.WriteLine("Stats");
			Console.WriteLine("");
			Console.WriteLine("Files:");
			Console.WriteLine("- Total: " + stats.Files.Total);
			Console.WriteLine("- Binary: " + stats.Files.Binary + " (skipped)");
			Console.WriteLine("- With Matches: " + stats.Files.WithMatches);
			Console.WriteLine("- Without Matches: " + stats.Files.WithoutMatches);
			Console.WriteLine("- Failed to Open: " + stats.Files.FailedToRead);

			if (isReplacerStats)
				Console.WriteLine("- Failed to Write: " + stats.Files.FailedToWrite);

			Console.WriteLine("");
			Console.WriteLine("Matches:");
			Console.WriteLine("- Found: " + stats.Matches.Found);

			if (isReplacerStats)
				Console.WriteLine("- Replaced: " + stats.Matches.Replaced);
			
			Console.WriteLine("");
			double secs = Math.Round(stats.Time.Passed.TotalSeconds, 3);
			Console.WriteLine("Duration: " + secs.ToString() + " secs");
			
			Console.WriteLine("====================================");
		}

		private static Assembly ResolveEventHandler(Object sender, ResolveEventArgs args)
		{
			String dllName = new AssemblyName(args.Name).Name + ".dll";

			var assem = Assembly.GetExecutingAssembly();

			String resourceName = assem.GetManifestResourceNames().FirstOrDefault(rn => rn.EndsWith(dllName));

			if (resourceName == null) return null; // Not found, maybe another handler will find it

			using (var stream = assem.GetManifestResourceStream(resourceName))
			{
				Byte[] assemblyData = new Byte[stream.Length];

				stream.Read(assemblyData, 0, assemblyData.Length);

				return Assembly.Load(assemblyData);

			}
		}
	}

	static class CommandLineRunner
	{
		public static void Run(string[] args)
		{
			var options = new CommandLineOptions();
			ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(System.Console.Error));
			if (!parser.ParseArguments(args, options, System.Console.Error))
			{
				Console.ReadKey();
				Environment.Exit(1);
			}

			var validationResultList = new List<ValidationResult>();

			validationResultList.Add(ValidationUtils.IsDirValid(options.Dir, "dir"));
			validationResultList.Add(ValidationUtils.IsNotEmpty(options.FileMask, "fileMask"));
			validationResultList.Add(ValidationUtils.IsNotEmpty(options.FindText, "find"));

			if (!String.IsNullOrEmpty(options.ReplaceText))
				validationResultList.Add(ValidationUtils.IsNotEmpty(options.ReplaceText, "replace"));

			if (validationResultList.Any(vr => !vr.IsSuccess))
			{
				Console.WriteLine("");
				foreach (var validationResult in validationResultList)
				{
					if (!validationResult.IsSuccess)
						Console.WriteLine(String.Format("{0}: {1}", validationResult.FieldName, validationResult.ErrorMessage));
				}
				Console.WriteLine("");
			}
			else
			{
				if (!String.IsNullOrEmpty(options.ReplaceText))
				{
					var replacer = new Replacer();
					replacer.Dir = options.Dir;
					replacer.FileMask = options.FileMask;
					replacer.IncludeSubDirectories = options.IncludeSubDirectories;

					replacer.FindText = CommandLineUtils.DecodeText(options.FindText);
					replacer.ReplaceText = CommandLineUtils.DecodeText(options.ReplaceText);
					replacer.IsCaseSensitive = options.IsCaseSensitive;
					replacer.FindTextHasRegEx = options.IsFindTextHasRegEx;
					replacer.FileProcessed += OnReplacerFileProcessed;

					replacer.Replace();
				}
				else
				{
					var finder = new Finder();
					finder.Dir = options.Dir;
					finder.FileMask = options.FileMask;
					finder.IncludeSubDirectories = options.IncludeSubDirectories;

					finder.FindText = CommandLineUtils.DecodeText(options.FindText);
					finder.IsCaseSensitive = options.IsCaseSensitive;
					finder.FindTextHasRegEx = options.IsFindTextHasRegEx;
					finder.FileProcessed += OnFinderFileProcessed;
					finder.Find();

				}
			}

			#if (DEBUG)
				Console.ReadLine();
			#endif
		}

		private static void OnFinderFileProcessed(object sender, FinderEventArgs e)
		{
			if (e.ResultItem.IncludeInResultsList)
				Program.PrintFinderResultRow(e.ResultItem, e.Stats);

			if (e.Stats.Files.Processed == e.Stats.Files.Total)
				Program.PrintStatistics(e.Stats);
			
		}

		private static void OnReplacerFileProcessed(object sender, ReplacerEventArgs e)
		{
			if (e.ResultItem.IncludeInResultsList)
				Program.PrintReplacerResultRow(e.ResultItem, e.Stats);

			if (e.Stats.Files.Processed == e.Stats.Files.Total)
				Program.PrintStatistics(e.Stats, true);

		}
	}
}
