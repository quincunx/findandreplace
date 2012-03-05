using System.Linq;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class ReplacerTest:TestBase
	{
		[Test]
		public void Replace_WhenSearchTextIsLicenseNoRegExpr_ReplacesTextInOne()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "license";
			replacer.ReplaceText = "aggrement";

			var resultItems = replacer.Replace().ResultItems;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			Assert.AreEqual(1, resultItems.Count);
			Assert.AreEqual("test1.test", resultItems[0].FileName);
			Assert.AreEqual(3, resultItems[0].NumMatches);
			Assert.IsTrue(resultItems[0].IsSuccess);

			resultItems = replacer.Replace().ResultItems;

			resultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();
			Assert.AreEqual(0, resultItems.Count);
		}
		
		[Test]
		public void Replace_WhenSearchTextIsEENoRegExpr_ReplacesTextInBoth()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "ee";
			replacer.ReplaceText = "!!";


			var resultItems = replacer.Replace().ResultItems;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();
			Assert.AreEqual(2, matchedResultItems.Count);

			var firstFile = resultItems.Where(ri => ri.FileName == "test1.test").ToList();

			Assert.AreEqual(1, firstFile.Count);
			Assert.AreEqual(5, firstFile[0].NumMatches);
			Assert.IsTrue(firstFile[0].IsSuccess);

			var secondFile = resultItems.Where(ri => ri.FileName == "test2.test").ToList();

			Assert.AreEqual(1, secondFile.Count);
			Assert.AreEqual(1, secondFile[0].NumMatches);
			Assert.IsTrue(secondFile[0].IsSuccess);
		}
		
		[Test]
		public void Replace_WhenSearchTextIsNewYorkNoRegExpr_NoReplacesText()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "New York";
			replacer.ReplaceText = "Moscow";

			var resultItems = replacer.Replace().ResultItems;

			Assert.AreEqual(0, resultItems.Count);
		}
		
		[Test]
		public void Replace_WhenSearchMaskIsTxtOnlyNoRegExpr_NoRepacesText()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.txt";
			replacer.FindText = "a";
			replacer.ReplaceText = "b";

			var resultItems = replacer.Replace().ResultItems;

			Assert.AreEqual(0, resultItems.Count);
		}

		[Test]
		public void Replace_WhenSearchMaskIsTest1NoRegExpr_ReplacesTextInOne()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "test1.*";
			replacer.FindText = "ee";
			replacer.ReplaceText = "!!";

			var resultItems = replacer.Replace().ResultItems;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(1, matchedResultItems.Count);
			Assert.AreEqual(5, matchedResultItems[0].NumMatches);
			Assert.AreEqual("test1.test", matchedResultItems[0].FileName);
			Assert.IsTrue(matchedResultItems[0].IsSuccess);

			resultItems = replacer.Replace().ResultItems;

			matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(0, matchedResultItems.Count);
		}

		[Test]
		public void Replace_WhenSearchTextIsSoAndCaseSensitiveNoRegExpr_ReplacesTextInOne()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "So";
			replacer.IsCaseSensitive = true;
			replacer.ReplaceText = "Su";

			var resultItems = replacer.Replace().ResultItems;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			Assert.AreEqual(1, resultItems.Count);
			Assert.AreEqual("test1.test", resultItems[0].FileName);
			Assert.AreEqual(1, resultItems[0].NumMatches);
			Assert.IsTrue(resultItems[0].IsSuccess);

		
			resultItems = replacer.Replace().ResultItems;
			Assert.AreEqual(0, resultItems.Count);
		}
		
		[Test]
		public void Replace_WhenSearchTextIsEEAndUseSubDirNoRegExpr_ReplacesTextInFour()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = "ee";
			replacer.ReplaceText = "!!";
			replacer.IncludeSubDirectories = true;

			var resultItems = replacer.Replace().ResultItems;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			var matchedResultItems = resultItems.Where(ri => ri.NumMatches != 0).ToList();

			Assert.AreEqual(4, matchedResultItems.Count);

			var firstFile = resultItems.Where(ri => ri.FileName == "test1.test").ToList();

			Assert.AreEqual(2, firstFile.Count);
			Assert.AreEqual(5, firstFile[0].NumMatches);
			Assert.AreEqual(5, firstFile[1].NumMatches);
			Assert.IsTrue(firstFile[0].IsSuccess);
			Assert.IsTrue(firstFile[1].IsSuccess);

			var secondFile = resultItems.Where(ri => ri.FileName == "test2.test").ToList();

			Assert.AreEqual(2, secondFile.Count);
			Assert.AreEqual(1, secondFile[0].NumMatches);
			Assert.IsTrue(secondFile[0].IsSuccess);
			Assert.AreEqual(1, secondFile[1].NumMatches);
			Assert.IsTrue(secondFile[1].IsSuccess);
		}

		[Test]
		public void Replace_WhenSearchTextIsRegularExpression_ReplacesTextInOne()
		{
			Replacer replacer = new Replacer();

			replacer.Dir = _tempDir;
			replacer.FileMask = "*.*";
			replacer.FindText = @"\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b"; //email pattern
			replacer.ReplaceText = "Email was here..";
			replacer.FindTextHasRegEx = true;

			var resultItems = replacer.Replace().ResultItems;

			if (resultItems == null || resultItems.Count == 0)
				Assert.Fail("Cant find test files");

			Assert.AreEqual(1, resultItems.Count);
			Assert.AreEqual("test2.test", resultItems[0].FileName);
			Assert.AreEqual(1, resultItems[0].NumMatches);
		}
	}
}
