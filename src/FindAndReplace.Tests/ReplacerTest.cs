using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class ReplacerTest:TestBase
	{
		[Test]
		public void Replace_WhenSearchTextIsLicense_ReplacesTextInOne()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "license";
			replacer.ReplaceText = "aggrement";

			var resultItems = replacer.Replace();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var machedResult = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(1, machedResult.Count, "Must be 1 mached file");

			Assert.AreEqual("test1.test", machedResult[0].FileName, "Mached filename must be test1.test");

			Assert.AreEqual(3, machedResult[0].NumMatches, "Must be 1 mache in file");

			Assert.IsTrue(machedResult[0].IsSuccess, "Must be success replace result");

			var notMachedResult = resultItems.Where(ri => ri.NumMatches == 0).ToList();

			Assert.AreEqual(1, notMachedResult.Count, "Must be 1 not mached file");

			Assert.AreEqual("test2.test", notMachedResult[0].FileName, "Not mached filename must be test2.test");

			resultItems = replacer.Replace();

			machedResult = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(0, machedResult.Count, "Must be 0 mached file after replace");
		}
		
		[Test]
		public void Replace_WhenSearchTextIsEE_ReplacesTextInBoth()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "ee";
			replacer.ReplaceText = "!!";


			var resultItems = replacer.Replace();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var machedResult = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(2, machedResult.Count, "Must be 2 mached file");

			var firstFile = resultItems.Where(ri => ri.FileName == "test1.test").ToList();

			Assert.AreEqual(1, firstFile.Count, "test1.test must be in result list");

			Assert.AreEqual(5, firstFile[0].NumMatches, "Must be 5 maches in test1.test");

			Assert.IsTrue(firstFile[0].IsSuccess, "Must be success replace in test1.test");

			var secondFile = resultItems.Where(ri => ri.FileName == "test2.test").ToList();

			Assert.AreEqual(1, secondFile.Count, "test2.test must be in result list");

			Assert.AreEqual(1, secondFile[0].NumMatches, "Must be 1 maches in test2.test");

			Assert.IsTrue(secondFile[0].IsSuccess, "Must be success replace in test2.test");
		}
		
		[Test]
		public void Replace_WhenSearchTextIsNewYork_NoReplacesText()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "New York";
			replacer.ReplaceText = "Moscow";

			var resultItems = replacer.Replace();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var machedResult = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(0, machedResult.Count, "Must be 0 mached file");
		}
		
		[Test]
		public void Replace_WhenSearchMaskIsTxtOnly_NoRepacesText()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.txt";
			replacer.FindText = "a";
			replacer.ReplaceText = "b";

			var resultItems = replacer.Replace();

			Assert.AreEqual(0, resultItems.Count, "Must be 0 mached file");
		}

		
		[Test]
		public void Replace_WhenSearchMaskIsTest1_ReplacesTextInOne()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "test1.*";
			replacer.FindText = "ee";
			replacer.ReplaceText = "!!";

			var resultItems = replacer.Replace();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var machedResult = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(1, machedResult.Count, "Must be 1 mached file");

			Assert.AreEqual(5, machedResult[0].NumMatches, "Must be 5 maches in test1.test");

			Assert.AreEqual("test1.test", machedResult[0].FileName, "mached filename must be test1.test");

			Assert.IsTrue(machedResult[0].IsSuccess, "Must be success replace result");

			resultItems = replacer.Replace();

			machedResult = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(0, machedResult.Count, "Must be 0 mached file after replace");
		}

		
		[Test]
		public void Replace_WhenSearchTextIsSoAndCaseSensitive_ReplacesTextInOne()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "So";
			replacer.IsCaseSensitive = true;
			replacer.ReplaceText = "Su";

			var resultItems = replacer.Replace();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var machedResult = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(1, machedResult.Count, "Must be 1 mached file");

			Assert.AreEqual("test1.test", machedResult[0].FileName, "Mached filename must be test1.test");

			Assert.AreEqual(1, machedResult[0].NumMatches, "Must be 1 mache in file");

			Assert.IsTrue(machedResult[0].IsSuccess, "Must be success replace");

			var notMachedResult = resultItems.Where(ri => ri.NumMatches == 0).ToList();

			Assert.AreEqual(1, notMachedResult.Count, "Must be 1 not mached file");

			Assert.AreEqual("test2.test", notMachedResult[0].FileName, "Not mached filename must be test2.test");

			resultItems = replacer.Replace();

			machedResult = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(0, machedResult.Count, "Must be 0 mached file after replace");
		}
		
	}
}
