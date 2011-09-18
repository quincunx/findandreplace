using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class FinderTest:TestBase
	{
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

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(1, matchedResultItems.Count);
			Assert.AreEqual("test1.test", matchedResultItems[0].FileName);
			Assert.AreEqual(3, matchedResultItems[0].NumMatches);

			var notNatchedResultItems = resultItems.Where(ri => ri.NumMatches == 0).ToList();
			Assert.AreEqual(1, notNatchedResultItems.Count);
			Assert.AreEqual("test2.test", notNatchedResultItems[0].FileName);
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

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(2, matchedResultItems.Count);

			var firstFile = resultItems.Where(ri => ri.FileName == "test1.test").ToList();

			Assert.AreEqual(1, firstFile.Count);
			Assert.AreEqual(5, firstFile[0].NumMatches);

			var secondFile = resultItems.Where(ri => ri.FileName == "test2.test").ToList();

			Assert.AreEqual(1, secondFile.Count);
			Assert.AreEqual(1, secondFile[0].NumMatches);
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

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(0, matchedResultItems.Count);
		}

		[Test]
		public void Find_WhenSearchMaskIsTxtOnly_NoFindsText()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.txt";
			finder.FindText = "a";

			var resultItems = finder.Find();

			Assert.AreEqual(0, resultItems.Count);
		}

		[Test]
		public void Find_WhenSearchMaskIsTest1_FindsTextInOne()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "test1.*";
			finder.FindText = "ee";

			var resultItems = finder.Find();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(1, matchedResultItems.Count);

			Assert.AreEqual(5, matchedResultItems[0].NumMatches);

			Assert.AreEqual("test1.test", matchedResultItems[0].FileName);
		}

		[Test]
		public void Find_WhenSearchTextIsSoAndCaseSensitive_FindsTextInOne()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "So";
			finder.IsCaseSensitive = true;

			var resultItems = finder.Find();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(1, matchedResultItems.Count);
			Assert.AreEqual("test1.test", matchedResultItems[0].FileName);
			Assert.AreEqual(1, matchedResultItems[0].NumMatches);

			var notmatchedResultItems = resultItems.Where(ri => ri.NumMatches == 0).ToList();

			Assert.AreEqual(1, notmatchedResultItems.Count);
			Assert.AreEqual("test2.test", notmatchedResultItems[0].FileName);
		}

		[Test]
		public void Find_WhenSearchTextIsEEAndUseSubDir_FindsTextInFourFiles()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "ee";
			finder.IncludeSubDirectories = true;

			var resultItems = finder.Find();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(4, matchedResultItems.Count);

			var firstFile = resultItems.Where(ri => ri.FileName == "test1.test").ToList();

			Assert.AreEqual(2, firstFile.Count);
			Assert.AreEqual(5, firstFile[0].NumMatches);
			Assert.AreEqual(5, firstFile[1].NumMatches);

			var secondFile = resultItems.Where(ri => ri.FileName == "test2.test").ToList();

			Assert.AreEqual(2, secondFile.Count);
			Assert.AreEqual(1, secondFile[0].NumMatches);
			Assert.AreEqual(1, secondFile[1].NumMatches);
		}
	}
}
