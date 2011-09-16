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

			Thread thread = new Thread(new ThreadStart(DoFindWork));
			thread.Start();
			//finder.AsyncFind();

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

			var results = replacer.Replace();

			ShowResultPanel();

			gvResults.Rows.Clear();
			gvResults.Columns.Clear();
			gvResults.DataSource = results;
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{

		}

		private void CreateListener(Finder finder)
		{
			_finder = finder;
			_finder.FileProcessed += new FileProcessedEventHandler(FindFileProceed);
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

				gvResults.Rows.Add();

				int currentRow = gvResults.Rows.Count - 2;

				gvResults.Rows[currentRow].Cells[0].Value = e.ResultItem.FileName;
				gvResults.Rows[currentRow].Cells[1].Value = e.ResultItem.FilePath;
				gvResults.Rows[currentRow].Cells[2].Value = e.ResultItem.NumMatches;
			}

			//progressBar1.Maximum = e.TotalFilesCount;
			//progressBar1.Increment(1);
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

			string s = String.Format("{0}.exe --cl --dir \"{1}\" --fileMask \"{2}\" --find \"{3}\" --replace \"{4}\" {5} {6}",
			                         Application.ProductName, txtDir.Text, txtFileMask.Text, txtFind.Text, txtReplace.Text, chkSubDir.Checked?"--includeSubDir":"", chkBoxCaseSense.Checked?"--caseSensitive":"");

			txtCommandLine.Text = s;
		}

		private void DoFindWork()
		{
			_finder.AsyncFind();
		}
	}
}
