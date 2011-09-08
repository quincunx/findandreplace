using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class ReplacerTest
	{
		private string _tempDir
		{
			get { return Path.GetTempPath() + "\\FindAndReplaceTests"; }
		}
	}
}
