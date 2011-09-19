using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace FindAndReplace.App
{
	 
	
	public partial class MainForm : Form
	{
		private Finder _finder;

		private Replacer _replace;

		private Thread _currentThread;

		delegate void SetFinderResultCallback(Finder.FindResultItem resultItem, int totalCount);

		delegate void SetReplaceResultCallback(Replacer.ReplaceResultItem resultItem, int totalCount);

		public MainForm()
		{
			InitializeComponent();
		}

		private void btnFindOnly_Click(object sender, EventArgs e)
		{
			PrepareFinderGrid();

			var finder = new Finder();
			finder.Dir = txtDir.Text;
			finder.FileMask = txtFileMask.Text;
			finder.FindText = txtFind.Text;
			finder.IsCaseSensitive = chkBoxCaseSense.Checked;
			finder.IncludeSubDirectories = chkSubDir.Checked;

			CreateListener(finder);

			ShowResultPanel();

			_currentThread = new Thread(new ThreadStart(DoFindWork));
			_currentThread.IsBackground = true;
			_currentThread.Start();
		}

		private void btnReplace_Click(object sender, EventArgs e)
		{
			var replacer = new Replacer();

			replacer.Dir = txtDir.Text;
			replacer.FileMask = txtFileMask.Text;
			replacer.FindText = txtFind.Text;
			replacer.ReplaceText = txtReplace.Text;
			replacer.IsCaseSensitive = chkBoxCaseSense.Checked;
			replacer.IncludeSubDirectories = chkSubDir.Checked;

			//var results = replacer.Replace();

			ShowResultPanel();

			PrepareReplacerGrid();

			CreateListener(replacer);

			//gvResults.Rows.Clear();
			//gvResults.Columns.Clear();
			//gvResults.DataSource = results;

			_currentThread = new Thread(new ThreadStart(DoReplaceWork));
			_currentThread.IsBackground = true;
			_currentThread.Start();
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_currentThread!=null && _currentThread.IsAlive) _currentThread.Abort();
		}

		private void CreateListener(Finder finder)
		{
			_finder = finder;
			_finder.FileProcessed += new FileProcessedEventHandler(FindFileProceed);
		}

		private void CreateListener(Replacer replacer)
		{
			_replace = replacer;
			_replace.FileProcessed += new ReplaceFileProcessedEventHandler(ReplaceFileProceed);
		}

		private void DetachListener()
		{
			_finder.FileProcessed -= new FileProcessedEventHandler(FindFileProceed);
			_finder = null;
		}

		private void FindFileProceed(object sender, FinderEventArgs e)
		{

			if (!this.gvResults.InvokeRequired)
			{

				ShowFindResult(e.ResultItem, e.TotalFilesCount);
			}
			else
			{
				SetFinderResultCallback finderResultCallback=new SetFinderResultCallback(ShowFindResult);
				this.Invoke(finderResultCallback, new object[] {e.ResultItem, e.TotalFilesCount});
			}
		}

		private void PrepareFinderGrid()
		{
			gvResults.DataSource = null;

			gvResults.Rows.Clear();
			gvResults.Columns.Clear();
			gvResults.Columns.Add("Filename", "Filename");
			gvResults.Columns.Add("Path", "Path");
			gvResults.Columns.Add("Num Maches", "Num Maches");

			progressBar1.Value = 0;
		}

		private void PrepareReplacerGrid()
		{
			gvResults.DataSource = null;

			gvResults.Rows.Clear();
			gvResults.Columns.Clear();
			gvResults.Columns.Add("Filename", "Filename");
			gvResults.Columns.Add("Path", "Path");
			gvResults.Columns.Add("Num Maches", "Num Maches");
			gvResults.Columns.Add("Is Success", "IsSuccess");

			progressBar1.Value = 0;
		}

		private void ShowResultPanel()
		{
			if (!label3.Visible)
			{
				label3.Visible = true;
				gvResults.Visible = true;
				progressBar1.Visible = true;

				if (!txtCommandLine.Visible)
				{
					label3.Top -= txtCommandLine.Height;
					gvResults.Top -= txtCommandLine.Height;
					progressBar1.Top -= txtCommandLine.Height;
				}

				this.Height += 200;
			}
		}

		private void ShowCommandLinePanel()
		{
			if (!txtCommandLine.Visible)
			{
				txtCommandLine.Visible = true;

				if (label3.Visible)
				{
					label3.Top += txtCommandLine.Height;
					gvResults.Top += txtCommandLine.Height;
					progressBar1.Top += txtCommandLine.Height;
				}

				this.Height += txtCommandLine.Height + 10;
			}
		}

		private void btnGen_Click(object sender, EventArgs e)
		{
			ShowCommandLinePanel();
			txtCommandLine.Clear();
			
			string s = String.Format("{0} --cl --dir \"{1}\" --fileMask \"{2}\" --find \"{3}\" --replace \"{4}\" {5} {6}",
									 Application.ExecutablePath, txtDir.Text, txtFileMask.Text, ParseText(txtFind.Text), ParseText(txtReplace.Text), chkSubDir.Checked ? "--includeSubDir" : "", chkBoxCaseSense.Checked ? "--caseSensitive" : "");

			txtCommandLine.Text = s;
		}

		private void DoFindWork()
		{
			_finder.FindAsync();
		}

		private string ParseText(string original)
		{
			
			return original.Replace(Environment.NewLine, "\\r\\n").Replace("\"","\\\"");
		}

		private void ShowFindResult(Finder.FindResultItem findResultItem, int totalCount)
		{
			gvResults.Rows.Add();

			int currentRow = gvResults.Rows.Count - 2;

			gvResults.Rows[currentRow].Cells[0].Value = findResultItem.FileName;
			gvResults.Rows[currentRow].Cells[1].Value = findResultItem.FilePath;
			gvResults.Rows[currentRow].Cells[2].Value = findResultItem.NumMatches;

			progressBar1.Maximum = totalCount;
			progressBar1.Value++;
		}

		private void ShowReplaceResult(Replacer.ReplaceResultItem replaceResultItem, int totalCount)
		{
			gvResults.Rows.Add();

			int currentRow = gvResults.Rows.Count - 2;

			gvResults.Rows[currentRow].Cells[0].Value = replaceResultItem.FileName;
			gvResults.Rows[currentRow].Cells[1].Value = replaceResultItem.FilePath;
			gvResults.Rows[currentRow].Cells[2].Value = replaceResultItem.NumMatches;
			gvResults.Rows[currentRow].Cells[3].Value = replaceResultItem.IsSuccess;

			progressBar1.Maximum = totalCount;
			progressBar1.Value++;
		}

		private void DoReplaceWork()
		{
			_replace.ReplaceAsync();
		}

		private void ReplaceFileProceed(object sender, ReplacerEventArgs e)
		{

			if (!this.gvResults.InvokeRequired)
			{
				ShowReplaceResult(e.ResultItem, e.TotalFilesCount);
			}
			else
			{
				SetReplaceResultCallback replaceResultCallback = new SetReplaceResultCallback(ShowReplaceResult);
				this.Invoke(replaceResultCallback, new object[] { e.ResultItem, e.TotalFilesCount });
			}
		}
	}
}
