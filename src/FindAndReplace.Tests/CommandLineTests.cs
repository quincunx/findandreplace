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
		public void TryDecodeTwoBackslahes_ReturnCorrectvalue()
		{
			var argValue = @"\\";
			
			var cmdText = String.Format("--testVal {0}", argValue);
			System.Diagnostics.Process.Start(_applicationExePath, cmdText);

			//wait for FindAndReplace.Tests.CommandLine.exe finish
			Thread.Sleep(1000);
			
			var decodedValue = GetValueFromOutput();

			Assert.AreEqual(decodedValue, argValue);
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
	}
}
