using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class CommandLineTests
	{
		private string _applicationExePath = @"FindAndReplace.Tests.CommandLine.exe";

		[Test]
		public void TryDecode2Backslahes_UseEscapeChars_Return4Backslashes()
		{
			var argValue = @"\\";
			var isRegex = false;
			var useChars = true;

			var cmdText = GenCommandLine(argValue, isRegex, useChars);

			System.Diagnostics.Process.Start(_applicationExePath, cmdText);

			//wait for FindAndReplace.Tests.CommandLine.exe finish
			Thread.Sleep(1000);
			
			var decodedValue = GetValueFromOutput();

			Assert.AreEqual(@"\\\\", decodedValue);
		}

		[Test]
		public void TryDecode2Backslahes_Return2Backslashes()
		{
			var argValue = @"\\";
			var isRegex = false;
			var useChars = false;

			var cmdText = GenCommandLine(argValue, isRegex, useChars);

			System.Diagnostics.Process.Start(_applicationExePath, cmdText);

			//wait for FindAndReplace.Tests.CommandLine.exe finish
			Thread.Sleep(1000);

			var decodedValue = GetValueFromOutput();

			Assert.AreEqual(@"\\", decodedValue);
		}

		[Test]
		public void TryDecodeTwoBackslahes_IsRegex_Return4Backslashes()
		{
			var argValue = @"\\";
			var isRegex = true;
			var useChars = false;

			var cmdText = GenCommandLine(argValue, isRegex, useChars);

			System.Diagnostics.Process.Start(_applicationExePath, cmdText);

			//wait for FindAndReplace.Tests.CommandLine.exe finish
			Thread.Sleep(1000);

			var decodedValue = GetValueFromOutput();

			Assert.AreEqual(@"\\\\", decodedValue);
		}

		[Test]
		public void TryDecode4Backslahes_UseEscapeChars_Return8Backslashes()
		{
			var argValue = @"\\\\";
			var isRegex = false;
			var useChars = true;

			var cmdText = GenCommandLine(argValue, isRegex, useChars);

			System.Diagnostics.Process.Start(_applicationExePath, cmdText);

			//wait for FindAndReplace.Tests.CommandLine.exe finish
			Thread.Sleep(1000);

			var decodedValue = GetValueFromOutput();

			Assert.AreEqual(@"\\\\\\\\", decodedValue);
		}

		private string GetValueFromOutput()
		{
			string result = String.Empty;

			var filename = "output.log";

			if (File.Exists(filename))
			{
				using (var outfile = new StreamReader(filename))
				{
					result = outfile.ReadLine();
				}
			}

			return result;
		}

		private string GenCommandLine(string value, bool isRegex, bool useChars)
		{
			return String.Format("--testVal \"{0}\"{1}{2}",
			                     value,
			                     isRegex ? " --isRegex" : "",
			                     useChars ? " --useEscapeChars" : "");
		}
	}
}
