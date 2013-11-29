using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class RegexReplacerTests
	{
		[Test]
		public void Replace_WhenReplaceTextIsRegExpr_ReplacesText()
		{
			string text = "A x@x.com letter --- alphabetical ---- missing ---- lack release penchant slack acryllic laundry A x@x.com hh --- cease";
			string findPattern = @"A x@x.com (.*?) ---";
			string replacePattern = "---/-B-";

			var result = Regex.Replace(text, findPattern, new MatchEvaluator(delegate(Match match) { return WordScrambler(match, replacePattern ); }));

			Assert.IsTrue(result == "A x@x.com letter -B- alphabetical ---- missing ---- lack release penchant slack acryllic laundry A x@x.com hh -B- cease");
		}

		public static string WordScrambler(Match match, string replacePattern)
		{
			var splittedPattern = replacePattern.Split('/');

			var findText = splittedPattern[0];

			var replaceText = splittedPattern[1];

			var text = match.Value;

			return Regex.Replace(text, findText, replaceText);
		}
	}
}
