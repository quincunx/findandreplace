using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Linq;


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
			if (_currentThread != null && _currentThread.IsAlive)
				_currentThread.Abort();
		}

		private void btnFindOnly_Click(object sender, EventArgs e)
		{
			txtDir.CausesValidation = true;
			txtFileMask.CausesValidation = true;
			txtFind.CausesValidation = true;
			txtReplace.CausesValidation = false;

			var isFormValid = true; 
			foreach (Control control in Controls)
			{
				control.Focus();

				if (!Validate() || errorProvider1.GetError(control) != "") isFormValid = false;
				errorProvider1.SetError(control, "");
			}
			
			if (!isFormValid) return;

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
			gvResults.Columns.Add(new DataGridViewColumn() { DataPropertyName = "Path", HeaderText = "Path", CellTemplate = new DataGridViewTextBoxCell(), Width = 400 });
			gvResults.Columns.Add("NumMatches", "Matches");
			gvResults.Columns.Add("Tooltip", "");
			gvResults.Columns[3].Visible = false;

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
				this.Invoke(finderResultCallback, new object[] { e.ResultItem, e.TotalFilesCount });
			}
		}

		private void ShowFindResult(Finder.FindResultItem findResultItem, int totalCount)
		{
			if (totalCount != 0)
			{
				if (findResultItem.NumMatches > 0)
				{
					gvResults.Rows.Add();

					int currentRow = gvResults.Rows.Count - 1;

					gvResults.Rows[currentRow].Cells[0].Value = findResultItem.FileName;
					gvResults.Rows[currentRow].Cells[1].Value = findResultItem.FileRelativePath;
					gvResults.Rows[currentRow].Cells[2].Value = findResultItem.NumMatches;
					gvResults.Rows[currentRow].Cells[3].Value = findResultItem.ToolTip;
				}

				progressBar.Maximum = totalCount;
				progressBar.Value++;

				lblStatus.Text = "Processing " + progressBar.Value + " of " + totalCount + " files.  Last file: " +
				                 findResultItem.FileRelativePath;

			}
			else
			{
				HideResultPanel();

				txtNoMathces.Visible = true;
			}
			

			
			//When last file - enable buttons back
			if (totalCount == progressBar.Value)
				EnableButtons();
			
		}

		private void DisableButtons()
		{
			this.Cursor = Cursors.WaitCursor;

			UpdateButtons(false);
		}

		private void EnableButtons()
		{
			UpdateButtons(true);

			this.Cursor = Cursors.Default;
		}

		private void UpdateButtons(bool enabled)
		{
			btnFindOnly.Enabled = enabled;
			btnReplace.Enabled = enabled;
			btnGenReplaceCommandLine.Enabled = enabled;
		}

		private void DoFindWork()
		{
			_finder.Find();
		}

		private void ShowResultPanel()
		{
			DisableButtons();

			txtNoMathces.Visible = false;

			if (!pnlGridResults.Visible)
			{
				pnlGridResults.Visible = true;

				if (pnlCommandLine.Visible)
				{
					this.Height -= pnlCommandLine.Height + 10;
					pnlCommandLine.Visible = false;
				}

				this.Height += pnlGridResults.Height + 10;
			}
		}

		private void HideResultPanel()
		{
			if (pnlGridResults.Visible)
			{
				pnlGridResults.Visible = false;

				this.Height -= pnlGridResults.Height + 10;
			}
		}

		private void btnReplace_Click(object sender, EventArgs e)
		{
			txtDir.CausesValidation = true;
			txtFileMask.CausesValidation = true;
			txtFind.CausesValidation = true;
			txtReplace.CausesValidation = true;

			var isFormValid = true;
			foreach (Control control in Controls)
			{
				control.Focus();

				if (!Validate() || errorProvider1.GetError(control) != "") isFormValid = false;
				else errorProvider1.SetError(control, "");
			}

			if (!isFormValid) return;
			
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
			if (totalCount>0)
			{
				if (replaceResultItem.NumMatches > 0)
				{
					gvResults.Rows.Add();

					int currentRow = gvResults.Rows.Count - 1;

					gvResults.Rows[currentRow].Cells[0].Value = replaceResultItem.FileName;
					gvResults.Rows[currentRow].Cells[1].Value = replaceResultItem.FileRelativePath;
					gvResults.Rows[currentRow].Cells[2].Value = replaceResultItem.NumMatches;
					gvResults.Rows[currentRow].Cells[3].Value = replaceResultItem.IsSuccess;
				}

				progressBar.Maximum = totalCount;
				progressBar.Value++;

				lblStatus.Text = "Processing " + progressBar.Value + " of " + totalCount + " files.  Last file: " +
				                 replaceResultItem.FileRelativePath;
			}
			else
			{
				HideResultPanel();

				txtNoMathces.Visible = true;
			}
			//When last file - enable buttons back
			if (totalCount == progressBar.Value)
				EnableButtons();
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
				this.Invoke(replaceResultCallback, new object[] { e.ResultItem, e.TotalFilesCount });
			}
		}

		private void btnGenReplaceCommandLine_Click(object sender, EventArgs e)
		{
			txtDir.CausesValidation = true;
			txtFileMask.CausesValidation = true;
			txtFind.CausesValidation = true;
			txtReplace.CausesValidation = true;

			var isFormValid = true;
			foreach (Control control in Controls)
			{
				control.Focus();

				if (!Validate() || errorProvider1.GetError(control) != "") isFormValid = false;
				else errorProvider1.SetError(control, "");
			}

			if (!isFormValid) return;
			
			
			
			ShowCommandLinePanel();
			txtCommandLine.Clear();

			string s = String.Format("{0} --cl --dir \"{1}\" --fileMask \"{2}\" {3} --find \"{4}\" --replace \"{5}\" {6}",
									 Application.ExecutablePath,
									 txtDir.Text,
									 txtFileMask.Text,
									 chkIncludeSubDirectories.Checked ? "--includeSubDirectories" : "",
									 CommandLineUtils.EncodeText(txtFind.Text),
									 CommandLineUtils.EncodeText(txtReplace.Text),
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

		private void txtDir_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (txtDir.Text.Trim() == "")
			{
				errorProvider1.SetError(txtDir, "Dir is required");
				return;
			}

			Regex dirRegex=new Regex(@"^(([a-zA-Z]:)|(\\{2}[^\/\\:*?<>|]+))(\\([^\/\\:*?<>|]*))*(\\)?$");
			if (!dirRegex.IsMatch(txtDir.Text))
			{
				errorProvider1.SetError(txtDir, "Dir is invalid");
				//e.Cancel = true;
				return;
			}

			errorProvider1.SetError(txtDir, "");
		}

		private void txtFileMask_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (txtFileMask.Text.Trim() == "")
			{
				errorProvider1.SetError(txtFileMask, "FileMask is required");
				return;
			}

			errorProvider1.SetError(txtFileMask, "");
		}

		private void txtFind_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (txtFind.Text.Trim() == "")
			{
				errorProvider1.SetError(txtFind, "Find is required");
				return;
			}

			errorProvider1.SetError(txtFind, "");
		}

		private void txtReplace_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (txtReplace.Text.Trim() == "")
			{
				errorProvider1.SetError(txtReplace, "Replace is required");
				return;
			}

			errorProvider1.SetError(txtReplace, "");
		}

		private void gvResults_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
		{
			e.ToolTipText = "ooo";

		}

		private void gvResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			var tooltip = gvResults.Rows[e.RowIndex].Cells[3].Value.ToString();
			
			ResultForm resultForm=new ResultForm();
			resultForm.richTextBox1.Text = tooltip;
			resultForm.richTextBox1.ReadOnly = true;

			var lines = resultForm.richTextBox1.Text.Split("\r\n".ToCharArray());
			var clearLines = new List<string>();
			for (int i=0;i<lines.Count();i++)
				if (i%2 ==0 ) clearLines.Add(lines[i]);

			var font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);

			var mathches = chkIsCaseSensitive.Checked ? Regex.Matches(resultForm.richTextBox1.Text, txtFind.Text) : Regex.Matches(resultForm.richTextBox1.Text, txtFind.Text, RegexOptions.IgnoreCase);
			foreach (Match match in mathches)
			{
				resultForm.richTextBox1.SelectionStart = match.Index - DetectMatchLine(clearLines.ToArray(), match.Index);

				var nCount = txtFind.Text.Split("\n".ToCharArray()).Count() - 1;

				resultForm.richTextBox1.SelectionLength=match.Length-nCount;

				resultForm.richTextBox1.SelectionFont=font;

				resultForm.richTextBox1.SelectionColor = Color.CadetBlue;
			}

			resultForm.richTextBox1.SelectionLength = 0;

			resultForm.ShowDialog();
		}

		private int DetectMatchLine(string[] lines, int position)
		{
			var separatorLength = 2;
			int i = 0;
			int charsCount = lines[0].Length + separatorLength;

			while (charsCount <= position)
			{
				i++;
				charsCount += lines[i].Length + separatorLength;
			}

			return i;
		}
	}
}
