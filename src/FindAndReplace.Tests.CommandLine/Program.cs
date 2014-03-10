using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

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
				result = options.TestValue;
			}

			using (var outfile = new StreamWriter("output.log"))
			{
				outfile.WriteLine(result);
			} 
		}
	}
}
