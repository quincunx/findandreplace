using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FindAndReplace.Tests;

namespace FindAndReplace.ConsoleTests
{
	class Program
	{
		static void Main(string[] args)
		{
			var test = new DetectEncodingSpeedTest();

			test.SetUp();
			test.MLang_Real_Dir_MultiThreaded();
			test.TearDown();

			Console.WriteLine("");
			Console.WriteLine("");

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey(true);
		}
	}
}
