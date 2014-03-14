using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;
using FindAndReplace;

namespace FindAndReplace.Tests.CommandLine
{
	class Program
	{
		static void Main(string[] args)
		{
			var options = new CommandLineOptions();

			var result = "not decoded";
			if (Parser.Default.ParseArguments(args, options))
			{
				result = CommandLineUtils.DecodeText(options.TestValue, false, options.IsRegex, options.UseEscape);
			}

			using (var outfile = new StreamWriter("output.log"))
			{
				outfile.WriteLine(Regex.Escape(result));
			} 
		}
	}
}
