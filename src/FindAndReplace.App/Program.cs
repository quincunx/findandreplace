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

		public static void PrintFinderResult(List<Finder.FindResultItem> resultItems)
		{
			Console.WriteLine();

			PrintLine();
			PrintFinderResultRow("File", "Matches");
			PrintLine();
			foreach (var item in resultItems)
			{
				PrintFinderResultRow(item.FileRelativePath, item.NumMatches.ToString());
			}
			PrintLine();
		}


		static void PrintFinderResultRow(string path, string matches)
		{
			Console.WriteLine(
				string.Format("{0} | {1}",
							  FormatCell(path, 63),
							  FormatCell(matches, 10)));
		}

		public static void PrintReplacerResult(List<Replacer.ReplaceResultItem> resultItems)
		{
			Console.WriteLine();

			PrintLine();
			PrintReplacerResultRow("File", "Matches", "Success");
			PrintLine();
			foreach (var item in resultItems)
			{
				PrintReplacerResultRow(item.FileRelativePath, item.NumMatches.ToString(), item.IsSuccess.ToString());
			}
			PrintLine();
		}

		static void PrintLine()
		{
			Console.WriteLine(new string('-', 79));
		}

		static void PrintReplacerResultRow(string path, string matches, string success)
		{
			Console.WriteLine(
				string.Format("{0} | {1} | {2}",
					FormatCell(path, 53),
					FormatCell(matches, 10),
					FormatCell(success, 10)));
		}


		static string FormatCell(string text, int width)
		{
			return text.PadRight(width);
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


			try
			{

			
			var validationResult = options.Dir.IsDirValid();

			if (!validationResult.IsSuccess)
			{
				throw new Exception(validationResult.ErrorMessage);
			}

			validationResult = options.FileMask.IsNotEmpty("FileMask");

			if (!validationResult.IsSuccess)
			{
				throw new Exception(validationResult.ErrorMessage);
			}

			validationResult = options.FindText.IsNotEmpty("Find");

			if (!validationResult.IsSuccess)
			{
				throw new Exception(validationResult.ErrorMessage);
			}

			if (!String.IsNullOrEmpty(options.ReplaceText))
			{
				validationResult = options.ReplaceText.IsNotEmpty("Replace");
				if (!validationResult.IsSuccess)
				{
					throw new Exception(validationResult.ErrorMessage);
				}
				
				var replacer = new Replacer();
				replacer.Dir = options.Dir;
				replacer.FileMask = options.FileMask;
				replacer.IncludeSubDirectories = options.IncludeSubDirectories;

				replacer.FindText = CommandLineUtils.DecodeText(options.FindText);
				replacer.ReplaceText = CommandLineUtils.DecodeText(options.ReplaceText);
				replacer.IsCaseSensitive = options.IsCaseSensitive;

				var result = replacer.Replace();
				Program.PrintReplacerResult(result);
			}
			else
			{
				var finder = new Finder();
				finder.Dir = options.Dir;
				finder.FileMask = options.FileMask;
				finder.IncludeSubDirectories = options.IncludeSubDirectories;

				finder.FindText = CommandLineUtils.DecodeText(options.FindText);
				finder.IsCaseSensitive = options.IsCaseSensitive;

				var result = finder.Find();
				Program.PrintFinderResult(result);
			}

			//#if (DEBUG)
			//    Console.ReadLine();
			//#endif
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
			}

		}
	}
}
