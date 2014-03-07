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
		    string encoded = CommandLineUtils.EncodeText(original);
		    string decoded = CommandLineUtils.DecodeText(encoded, true);

            Assert.AreEqual(original, decoded);
		}

	
	}
}
