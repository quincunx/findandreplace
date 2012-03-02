using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class TestBase
	{
		protected string _tempDir;

		[SetUp] 
		public void SetUp()
		{
			_tempDir = Path.GetTempPath() + "\\FindAndReplaceTests";
			Directory.CreateDirectory(_tempDir);

			FileStream fs = new FileStream(_tempDir + "\\test1.test", FileMode.Create);
			StreamWriter sr = new StreamWriter(fs);

			sr.Write("The licenses for most software and other practical");
			sr.Write(" works are designed to take away your freedom to share");
			sr.Write(" and change the works. By contrast, the GNU General Public");
			sr.Write(" License is intended to guarantee your freedom to share and ");
			sr.Write("change all versions of a program--to make sure it remains free");
			sr.Write(" software for all its users. We, the Free Software Foundation, use");
			sr.Write(" the GNU General Public License for most of our software; it applies");
			sr.Write(" also to any other work released this way by its authors. You can apply it to your programs, too.");

			sr.Close();
			fs.Close();

			fs = new FileStream(_tempDir + "\\test2.test", FileMode.Create);
			sr = new StreamWriter(fs);

			sr.WriteLine("1234567890");
			sr.WriteLine("abcdefghijk");
			sr.WriteLine("aabbccddee");
			sr.WriteLine("mail@mail.com");

			sr.Close();
			fs.Close();

			Directory.CreateDirectory(_tempDir + "\\subDir");
			File.Copy(_tempDir + "\\test1.test", _tempDir + "\\subDir\\test1.test", true);
			File.Copy(_tempDir + "\\test2.test", _tempDir + "\\subDir\\test2.test", true);
		}

		[TearDown]
		public void TearDown()
		{
			var tempDir = Path.GetTempPath() + "\\FindAndReplaceTests";
			Directory.Delete(tempDir, true);
		}

	}
}
