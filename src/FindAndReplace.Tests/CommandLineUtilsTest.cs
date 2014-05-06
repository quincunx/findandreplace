using System.Linq;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class CommandLineUtilsTest
	{

		//From https://findandreplace.codeplex.com/workitem/17
		[Test]
		public void Encode_Decode_FromWorkItem17_ReturnsSameValue()
		{
			string original = "\\r(?!\\n)";
			string encoded = CommandLineUtils.EncodeText(original, true);
			string decoded = CommandLineUtils.DecodeText(encoded, false, true);

			Assert.AreEqual(original, decoded);
		}



		[Test]
		public void Encode_Decode_FromDiscussions541024_ReturnsSameValue()
		{
			string original = "<TargetFrameworkVersion>v[24].0</TargetFrameworkVersion>";
			string encoded = CommandLineUtils.EncodeText(original, true);
			string decoded = CommandLineUtils.DecodeText(encoded, true, true);

			Assert.AreEqual(original, decoded);
		}

	}
}
