using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FindAndReplace.Tests
{
    [TestFixture]
    public class FileGetterTest : TestBase
    {

        [Test]
        public void RunAsync_UsingBlockingCollection_TryTake_Works()
        {
            var fileGetter = new FileGetter
                {
                    DirPath = _tempDir,
                    FileMasks = new List<string>{"*.*"},
                    SearchOption = SearchOption.AllDirectories
                };

            fileGetter.RunAsync();

            // Consume bc
            while (true)
            {
                string filePath;
                bool success = fileGetter.FileQueue.TryTake(out filePath);

                if (success)
                {
                    Console.WriteLine(filePath);
                }
                else
                {
                    if (fileGetter.FileQueue.IsCompleted)
                        break;

                    Console.WriteLine("Blocking Collection empty");
                }
            }
        }


        [Test]
        public void RunAsync_UsingBlockingCollection_Take_Works()
        {
            var fileGetter = new FileGetter
            {
                DirPath = _tempDir,
                FileMasks = new List<string> { "*.*" },
                SearchOption = SearchOption.AllDirectories
            };

            fileGetter.RunAsync();

            // Consume bc
            while (true)
            {
                string filePath;

                try
                {
                    filePath = fileGetter.FileQueue.Take();
                    Console.WriteLine(filePath);
                }
                catch (InvalidOperationException)
                {
                    if (fileGetter.FileQueue.IsCompleted)
                        break;

                    throw;
                }
            }
        }


        [Test]
        public void Run_WhenFileMaskIsTest1_Works()
        {
            var fileGetter = new FileGetter
            {
                DirPath = _tempDir,
                FileMasks = new List<string> { "test1.*" },
                SearchOption = SearchOption.AllDirectories
            };

            List<string> files = fileGetter.RunSync();
            Assert.AreEqual(2, files.Count);
        }

        [Test]
        public void Run_WhenFileMaskIsTest1AndTest2_Works()
        {
            var fileGetter = new FileGetter
            {
                DirPath = _tempDir,
                FileMasks = new List<string> { "test1.txt", "test2.*" },
                SearchOption = SearchOption.AllDirectories
            };

            List<string> files = fileGetter.RunSync();
            Assert.AreEqual(4, files.Count);
        }


        [Test]
        public void Run_WhenExcludeFileMaskIsTxt_Works()
        {
            var fileGetter = new FileGetter
            {
                DirPath = _tempDir,
                FileMasks = new List<string> { "*.*" },
                ExcludeFileMasks = new List<string> { "*.txt" },
                SearchOption = SearchOption.AllDirectories
            };

            List<string> files = fileGetter.RunSync();
            Assert.AreEqual(1, files.Count);
        }

        [Test]
        public void Run_WhenExcludeFileMaskIsTxtAndDll_Works()
        {
            var fileGetter = new FileGetter
            {
                DirPath = _tempDir,
                FileMasks = new List<string> { "*.*" },
                ExcludeFileMasks = new List<string> { "*.txt", "*.dll" },
                SearchOption = SearchOption.AllDirectories
            };

            List<string> files = fileGetter.RunSync();
            Assert.AreEqual(0, files.Count);
        }
    }
}
