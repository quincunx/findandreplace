using System.Linq;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class FinderTest : TestBase
	{
		[Test]
		public void Find_WhenSearchTextIsLicenseNoRegExpr_FindsTextInOne()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "license";

			var resultItems = finder.Find().Items.Where(r=>r.NumMatches>0).ToList();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			Assert.AreEqual(1, resultItems.Count);
			Assert.AreEqual("test1.test", resultItems[0].FileName);
			Assert.AreEqual(3, resultItems[0].NumMatches);
		}

		[Test]
		public void Find_WhenSearchTextIsEENoRegExpr_FindsTextInBoth()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "ee";

			var resultItems = finder.Find().Items;

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
		public void Find_WhenSearchTextIsNewYorkNoRegExpr_NoFindsText()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "New York";

			var resultItems = finder.Find().Items.Where(f=>f.NumMatches>0).ToList();
			Assert.AreEqual(0, resultItems.Count);
		}

		[Test]
		public void Find_WhenSearchMaskIsTxtOnlNoRegExpry_NoFindsText()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.txt";
			finder.FindText = "a";

			var resultItems = finder.Find().Items;

			Assert.AreEqual(0, resultItems.Count);
		}

		[Test]
		public void Find_WhenSearchMaskIsTest1NoRegExpr_FindsTextInOne()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "test1.*";
			finder.FindText = "ee";

			var resultItems = finder.Find().Items;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(1, matchedResultItems.Count);

			Assert.AreEqual(5, matchedResultItems[0].NumMatches);

			Assert.AreEqual("test1.test", matchedResultItems[0].FileName);
		}

		[Test]
		public void Find_WhenSearchTextIsSoAndCaseSensitiveNoRegExpr_FindsTextInOne()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "So";
			finder.IsCaseSensitive = true;

			var resultItems = finder.Find().Items.Where(f=>f.NumMatches>0).ToList();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			Assert.AreEqual(1, resultItems.Count);
			Assert.AreEqual("test1.test", resultItems[0].FileName);
			Assert.AreEqual(1, resultItems[0].NumMatches);
		}

		[Test]
		public void Find_WhenSearchTextIsEEAndUseSubDirNoRegExpr_FindsTextInFourFiles()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "ee";
			finder.IncludeSubDirectories = true;

			var resultItems = finder.Find().Items.Where(f=>f.NumMatches>0).ToList();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");


			Assert.AreEqual(4, resultItems.Count);

			var firstFile = resultItems.Where(ri => ri.FileName == "test1.test").ToList();

			Assert.AreEqual(2, firstFile.Count);
			Assert.AreEqual(5, firstFile[0].NumMatches);
			Assert.AreEqual(5, firstFile[1].NumMatches);

			var secondFile = resultItems.Where(ri => ri.FileName == "test2.test").ToList();

			Assert.AreEqual(2, secondFile.Count);
			Assert.AreEqual(1, secondFile[0].NumMatches);
			Assert.AreEqual(1, secondFile[1].NumMatches);
		}

		[Test]
		public void Find_WhenSearchTextIsEmailPatternRegularExpression_FindsTextInOne()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = @"\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b"; //email pattern
			finder.FindTextHasRegEx = true;

			var resultItems = finder.Find().Items.Where(r=>r.NumMatches>0).ToList();

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			Assert.AreEqual(1, resultItems.Count);
			Assert.AreEqual("test2.test", resultItems[0].FileName);
			Assert.AreEqual(1, resultItems[0].NumMatches);
		}

		[Test]
		public void Find_WhenSearchStartManyTimes_FindsTextInFourFiles()
		{
			Finder finder = new Finder();

			finder.Dir = _tempDir;
			finder.FileMask = "*.*";
			finder.FindText = "a+";
			finder.IncludeSubDirectories = true;
			finder.FindTextHasRegEx = true;

			Finder.FindResult result = null;
			
			for(int i = 0; i<10; i++)
			{
				result = finder.Find();
			}

			Assert.IsNotNull(result);
		}
	}
}
