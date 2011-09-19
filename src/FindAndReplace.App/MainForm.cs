using System;
using System.Threading;
using System.Windows.Forms;


namespace FindAndReplace.App
{

	public partial class MainForm : Form
	{
		private Finder _finder;
		private Replacer _replacer;
		private Thread _currentThread;

		private delegate void SetFinderResultCallback(Finder.FindResultItem resultItem, int totalCount);

		private delegate void SetReplacerResultCallback(Replacer.ReplaceResultItem resultItem, int totalCount);

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_currentThread != null && _currentThread.IsAlive) _currentThread.Abort();
		}


		private void btnFindOnly_Click(object sender, EventArgs e)
		{
			PrepareFinderGrid();

			var finder = new Finder();
			finder.Dir = txtDir.Text;

			finder.IncludeSubDirectories = chkIncludeSubDirectories.Checked;
			finder.FileMask = txtFileMask.Text;

			finder.FindText = txtFind.Text;
			finder.IsCaseSensitive = chkIsCaseSensitive.Checked;
			CreateListener(finder);

			ShowResultPanel();

			_currentThread = new Thread(DoFindWork);
			_currentThread.IsBackground = true;
			_currentThread.Start();
		}


		private void PrepareFinderGrid()
		{
			gvResults.DataSource = null;

			gvResults.Rows.Clear();
			gvResults.Columns.Clear();
			gvResults.Columns.Add(new DataGridViewColumn() { DataPropertyName = "Filename", HeaderText = "Filename", CellTemplate = new DataGridViewTextBoxCell(), Width = 200 });
			gvResults.Columns.Add(new DataGridViewColumn() { DataPropertyName = "Path",  HeaderText = "Path", CellTemplate = new DataGridViewTextBoxCell(), Width = 400 });
			gvResults.Columns.Add("NumMatches", "Matches");

			progressBar.Value = 0;
		}

		private void CreateListener(Finder finder)
		{
			_finder = finder;
			_finder.FileProcessed += OnFinderFileProcessed;
		}

		private void OnFinderFileProcessed(object sender, FinderEventArgs e)
		{
			if (!this.gvResults.InvokeRequired)
			{
				ShowFindResult(e.ResultItem, e.TotalFilesCount);
			}
			else
			{
				SetFinderResultCallback finderResultCallback = ShowFindResult;
				this.Invoke(finderResultCallback, new object[] {e.ResultItem, e.TotalFilesCount});
			}
		}

		private void ShowFindResult(Finder.FindResultItem findResultItem, int totalCount)
		{
			gvResults.Rows.Add();

			int currentRow = gvResults.Rows.Count - 1;

			gvResults.Rows[currentRow].Cells[0].Value = findResultItem.FileName;
			gvResults.Rows[currentRow].Cells[1].Value = findResultItem.FilePath;
			gvResults.Rows[currentRow].Cells[2].Value = findResultItem.NumMatches;

			progressBar.Maximum = totalCount;
			progressBar.Value++;
		}

		private void DoFindWork()
		{
			_finder.Find();
		}


		private void ShowResultPanel()
		{
			if (!pnlGridResults.Visible)
			{
				pnlGridResults.Visible = true;

				if (pnlCommandLine.Visible)
				{
					this.Height -= pnlCommandLine.Height+10;
					pnlCommandLine.Visible = false;
				}

				this.Height += pnlGridResults.Height+10;
			}
		}


		private void btnReplace_Click(object sender, EventArgs e)
		{
			var replacer = new Replacer();

			replacer.Dir = txtDir.Text;
			replacer.IncludeSubDirectories = chkIncludeSubDirectories.Checked;

			replacer.FileMask = txtFileMask.Text;

			replacer.FindText = txtFind.Text;
			replacer.IsCaseSensitive = chkIsCaseSensitive.Checked;

			replacer.ReplaceText = txtReplace.Text;

			ShowResultPanel();

			PrepareReplacerGrid();

			CreateListener(replacer);

			_currentThread = new Thread(DoReplaceWork);
			_currentThread.IsBackground = true;
			_currentThread.Start();
		}

		private void CreateListener(Replacer replacer)
		{
			_replacer = replacer;
			_replacer.FileProcessed += ReplaceFileProceed;
		}

		private void PrepareReplacerGrid()
		{
			gvResults.DataSource = null;

			gvResults.Rows.Clear();
			gvResults.Columns.Clear();
			gvResults.Columns.Add("Filename", "Filename");
			gvResults.Columns.Add(new DataGridViewColumn() { DataPropertyName = "Path", HeaderText = "Path", CellTemplate = new DataGridViewTextBoxCell(), Width = 400 });
			gvResults.Columns.Add("NumMatches", "Matches");
			gvResults.Columns.Add("IsSuccess", "Success");

			progressBar.Value = 0;
		}


		private void DoReplaceWork()
		{
			_replacer.Replace();
		}


		private void ShowReplaceResult(Replacer.ReplaceResultItem replaceResultItem, int totalCount)
		{
			gvResults.Rows.Add();

			int currentRow = gvResults.Rows.Count - 2;

			gvResults.Rows[currentRow].Cells[0].Value = replaceResultItem.FileName;
			gvResults.Rows[currentRow].Cells[1].Value = replaceResultItem.FilePath;
			gvResults.Rows[currentRow].Cells[2].Value = replaceResultItem.NumMatches;
			gvResults.Rows[currentRow].Cells[3].Value = replaceResultItem.IsSuccess;

			progressBar.Maximum = totalCount;
			progressBar.Value++;
		}


		private void ReplaceFileProceed(object sender, ReplacerEventArgs e)
		{

			if (!this.gvResults.InvokeRequired)
			{
				ShowReplaceResult(e.ResultItem, e.TotalFilesCount);
			}
			else
			{
				SetReplacerResultCallback replaceResultCallback = new SetReplacerResultCallback(ShowReplaceResult);
				this.Invoke(replaceResultCallback, new object[] {e.ResultItem, e.TotalFilesCount});
			}
		}

		private void btnGenReplaceCommandLine_Click(object sender, EventArgs e)
		{
			ShowCommandLinePanel();
			txtCommandLine.Clear();

			string s = String.Format("{0} --cl --dir \"{1}\" --fileMask \"{2}\" {3} --find \"{4}\" --replace \"{5}\" {6}",
			                         Application.ExecutablePath,
			                         txtDir.Text,
			                         txtFileMask.Text,

			                         chkIncludeSubDirectories.Checked ? "--includeSubDirectories" : "",
			                         ParseText(txtFind.Text), ParseText(txtReplace.Text),
			                         chkIsCaseSensitive.Checked ? "--caseSensitive" : "");

			txtCommandLine.Text = s;
		}

		private void ShowCommandLinePanel()
		{
			if (!pnlCommandLine.Visible)
			{
				pnlCommandLine.Visible = true;

				if (pnlGridResults.Visible)
				{
					pnlGridResults.Visible = false;
					this.Height -= pnlGridResults.Height + 10;
				}

				this.Height += pnlCommandLine.Height + 10;
			}
		}

		private string ParseText(string original)
		{
			return original
				.Replace(Environment.NewLine, "\\r\\n")
				.Replace("\"", "\\\"");
		}
	}
}
