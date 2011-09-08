using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class FinderTest
	{
	
		private string _tempDir
		{
			get { return Path.GetTempPath() + "\\FindAndReplaceTests"; }
		}

		[Test]
		public void Find_WhenSearchTextIsLicense_FindsTextInOne()
		{
			Finder finder=new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "license";

			var resultItems = finder.Find();

			if (resultItems==null || resultItems.Count==0) 
				Assert.Fail("Cant find test files");

			var machedResult = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(1, machedResult.Count, "Must be 1 mached file");

			Assert.AreEqual("test1.test", machedResult[0].FileName, "Mached filename must be test1.test");
			
			Assert.AreEqual(1, machedResult[0].NumMatches, "Must be 1 mache in file");

			var notMachedResult = resultItems.Where(ri => ri.NumMatches == 0).ToList();

			Assert.AreEqual(1, notMachedResult.Count, "Must be 1 not mached file");

			Assert.AreEqual("test2.test", notMachedResult[0].FileName, "Not mached filename must be test2.test");
		}

		[Test]
		public void Find_WhenSearchTextIsEE_FindsTextInBoth()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "ee";

			var resultItems = finder.Find();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var machedResult = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(2, machedResult.Count, "Must be 2 mached file");

			var firstFile = resultItems.Where(ri => ri.FileName == "test1.test").ToList();

			Assert.AreEqual(1, firstFile.Count, "test1.test must be in result list");

			Assert.AreEqual(5, firstFile[0].NumMatches, "Must be 5 maches in test1.test");

			var secondFile = resultItems.Where(ri => ri.FileName == "test2.test").ToList();

			Assert.AreEqual(1, secondFile.Count, "test2.test must be in result list");

			Assert.AreEqual(1, secondFile[0].NumMatches, "Must be 1 maches in test2.test");
		}

		[Test]
		public void Find_WhenSearchTextIsNewYork_NoFindsText()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "New York";

			var resultItems = finder.Find();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var machedResult = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(0, machedResult.Count, "Must be 0 mached file");
		}

		[Test]
		public void Find_WhenSearchMaskIsTxtOnly_NoFindsText()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.txt";
			finder.FindText = "a";

			var resultItems = finder.Find();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var machedResult = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(0, machedResult.Count, "Must be 0 mached file");
		}
	}
}
