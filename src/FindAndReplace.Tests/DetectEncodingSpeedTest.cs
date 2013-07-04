using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using href.Utils;

namespace FindAndReplace.Tests
{
	[TestFixture]
	public class DetectEncodingSpeedTest : TestBase
	{

		[SetUp]
		public override void SetUp()
		{
			CreateTestDir();
			CreateSpeedDir();

			WriteFiles(100000);
		}


		[TearDown]
		public override void TearDown()
		{
			DeleteTestDir();
		}


		private void WriteFiles(int fileSize)
		{
			string fileContent = GetFileContent(fileSize);

			CreateTestFile(fileContent, Encoding.ASCII);
			CreateTestFile(fileContent, Encoding.Unicode);
			CreateTestFile(fileContent, Encoding.BigEndianUnicode);
			CreateTestFile(fileContent, Encoding.UTF32);
			CreateTestFile(fileContent, Encoding.UTF7);
			CreateTestFile(fileContent, Encoding.UTF8);

		}

		private void CreateTestFile(string fileContent, Encoding encoding)
		{
			string filePath = _speedDir + "\\" + encoding.EncodingName + ".txt";
			File.WriteAllText(filePath, fileContent, encoding);
		}

		[Test]
		public void KlerkSoft_Bom_NewFiles()
		{
			RunTest(EncodingDetector.Options.KlerkSoftBom, _speedDir);
		}


		[Test]
		public void KlerkSoft_Heuristics_NewFiles()
		{
			RunTest(EncodingDetector.Options.KlerkSoftHeuristics, _speedDir);
		}


		[Test]
		public void MLang_NewFiles()
		{
			RunTest(EncodingDetector.Options.MLang, _speedDir);
		}

		[Test]
		public void KlerkSoft_Bom_Real_Dir()
		{
			RunTest(EncodingDetector.Options.KlerkSoftBom, Dir_StyleSalt);
		}

		[Test]
		public void KlerkSoft_Bom_And_Heuristics_Real_Dir()
		{
			RunTest(EncodingDetector.Options.KlerkSoftBom | EncodingDetector.Options.KlerkSoftHeuristics, Dir_StyleSalt);
		}


		[Test]
		public void MLang_Real_Dir()
		{
			RunTest(EncodingDetector.Options.MLang, Dir_StyleSalt);
		}

		[Test]
		public void KlerkSoft_Bom_And_MLang_Real_Dir()
		{
			RunTest(EncodingDetector.Options.MLang, Dir_StyleSalt);
		}

		private void RunTest(EncodingDetector.Options encDetectorOptions, string dir)
		{
			int totalFiles =0;
			int numFoundEncodings = 0;

			StopWatch stopWatch = new StopWatch();
			stopWatch.Start();
			
			string detectorName = encDetectorOptions.ToString();
			foreach (string filePath in Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories))
			{
				StopWatch.Start("ReadFileSample");

				var sampleBytes = Utils.ReadFileContentSample(filePath);

				StopWatch.Stop("ReadFileSample");



				StopWatch.Start("IsBinaryFile");
				
				if (Utils.IsBinaryFile(sampleBytes))
				{
					StopWatch.Stop("IsBinaryFile");
					continue;
				}
				
				StopWatch.Stop("IsBinaryFile");


				StopWatch.Start(detectorName);

			
				//First try BOM detection and Unicode detection using Klerks Soft encoder
				//stream.Seek(0, SeekOrigin.Begin);

				Encoding encoding = EncodingDetector.Detect(sampleBytes, encDetectorOptions);
				totalFiles++;

				if (encoding != null)
					numFoundEncodings++;

				//Console.WriteLine(filePath + ": " + encoding);
				StopWatch.Stop(detectorName);
			}

			Console.WriteLine("Found Encoding in:" + numFoundEncodings + " out of " + totalFiles);

			stopWatch.Stop();

			StopWatch.PrintCollection(stopWatch.Milliseconds);
			StopWatch.Collection.Clear();
		}

	}
}
