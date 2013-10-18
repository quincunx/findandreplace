using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FindAndReplace
{
    public class FileGetter
    {

        public string DirPath { get; set; }
        public List<string> FileMasks { get; set; }
        public List<string> ExcludeFileMasks { get; set; }
        public List<string> ExcludeFileMasksRegEx { get; set; }

        public SearchOption SearchOption { get; set; }

        public bool IsCancelled { get; set; }
        public Task _task;

        public BlockingCollection<string> FileQueue = new BlockingCollection<string>();


        public void RunAsync()
        {
            _task = Task.Factory.StartNew(Run);
        }

        private void Run()
        {
            IsCancelled = false;

            foreach (var fileMask in FileMasks)
            {
                var files = Directory.EnumerateFiles(DirPath, fileMask, SearchOption);


                foreach (string filePath in files)
                {
                    if (!IsMatchWithExcludeFileMasks(filePath))
                        FileQueue.Add(filePath);

                    if (IsCancelled)
                        break;
                }
            }

            FileQueue.CompleteAdding();
        }


        public List<string> RunSync()
        {
            RunAsync();

            var filePathes = new List<string>();
            
            while (true)
            {
                string filePath;

                try
                {
                    filePath = FileQueue.Take();
                    filePathes.Add(filePath);
                }
                catch (InvalidOperationException)
                {
                    if (FileQueue.IsCompleted)
                        break;

                    throw;
                }
            }

            return filePathes;
        }


        private bool IsMatchWithExcludeFileMasks(string filePath)
        {
            if (ExcludeFileMasks == null || ExcludeFileMasks.Count == 0)
                return false;

            if (ExcludeFileMasksRegEx == null)
                ExcludeFileMasksRegEx = ExcludeFileMasks.Select(Utils.WildcardToRegex).ToList();


            foreach (string pattern in ExcludeFileMasksRegEx)
            {
                if (Regex.IsMatch(filePath, pattern))
                    return true;
            }

            return false;
        }


        public void Cancel()
        {
            IsCancelled = true;
        }
    }
}
