using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace FindAndReplace.Tests.CommandLine
{
	public class CommandLineOptions
	{
		#region Standard Option Attribute

		[Option("testVal", Required = true, HelpText = "Test value to parse.")]
		public string TestValue { get; set; }

		#endregion
	}
}
