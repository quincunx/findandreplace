using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Linq;


namespace FindAndReplace.App
{

	public class GVResultEventArgs: EventArgs
	{
		public int cellRow { get; set; }
	}

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
			gvResults.Columns.Add("TooltipLineNums", "");
			gvResults.Columns[3].Visible = false;
			gvResults.Columns[4].Visible = false;

			progressBar.Value = 0;

			if (txtMatches.Visible)
			{
				txtMatches.Visible = false;
				this.Height -= (txtMatches.Height + 50);
			}
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

					gvResults.Rows[currentRow].ContextMenuStrip = CreateContextMenu(currentRow);

					gvResults.Rows[currentRow].Cells[0].Value = findResultItem.FileName;
					gvResults.Rows[currentRow].Cells[1].Value = findResultItem.FileRelativePath;
					gvResults.Rows[currentRow].Cells[2].Value = findResultItem.NumMatches;

					var linesToPreview = new List<int>();

					foreach (Match match in findResultItem.Matches)
					{
						linesToPreview.AddRange(GetLineNumbersForMatchesPreview(findResultItem.FilePath, match));
					}

					gvResults.Rows[currentRow].Cells[3].Value = GenerateMatchesPreviewText(findResultItem.FilePath, linesToPreview);

					StringBuilder sb=new StringBuilder();
					foreach (var line in linesToPreview)
					{
						sb.Append(String.Format("{0} ", line));
					}

					gvResults.Rows[currentRow].Cells[4].Value = sb.ToString();
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
			{
				EnableButtons();
				gvResults.ClearSelection();
			}

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
			txtMatches.Visible = false;

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
			gvResults.Columns.Add(new DataGridViewColumn() { DataPropertyName = "Path", HeaderText = "Path", CellTemplate = new DataGridViewTextBoxCell(), Width = 300 });
			gvResults.Columns.Add("NumMatches", "Matches");
			gvResults.Columns.Add("IsSuccess", "Success");
			gvResults.Columns.Add("ErrorMessage", "Error");
			gvResults.Columns.Add("Tooltip", "");
			gvResults.Columns[5].Visible = false;

			if (txtMatches.Visible)
			{
				txtMatches.Visible = false;
				this.Height -= (txtMatches.Height + 50);
			}

			progressBar.Value = 0;
		}

		private void DoReplaceWork()
		{
			_replacer.Replace();
		}

		private void ShowReplaceResult(Replacer.ReplaceResultItem replaceResultItem, int totalCount)
		{
			if (totalCount > 0)
			{
				if (replaceResultItem.NumMatches > 0)
				{
					gvResults.Rows.Add();

					int currentRow = gvResults.Rows.Count - 1;

					gvResults.Rows[currentRow].Cells[0].Value = replaceResultItem.FileName;
					gvResults.Rows[currentRow].Cells[1].Value = replaceResultItem.FileRelativePath;
					gvResults.Rows[currentRow].Cells[2].Value = replaceResultItem.NumMatches;
					gvResults.Rows[currentRow].Cells[3].Value = replaceResultItem.IsSuccess;
					gvResults.Rows[currentRow].Cells[4].Value = replaceResultItem.ErrorMessage;

					var linesToPreview = new List<int>();

					foreach (Match match in replaceResultItem.Matches)
					{
						linesToPreview.AddRange(GetLineNumbersForMatchesPreview(replaceResultItem.FilePath, match));
					}

					gvResults.Rows[currentRow].Cells[5].Value = GenerateMatchesPreviewText(replaceResultItem.FilePath, linesToPreview);
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
			{
				EnableButtons();
				gvResults.ClearSelection();
			}
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

			if (txtMatches.Visible)
			{
				txtMatches.Visible = false;
				this.Height -= (txtMatches.Height + 50);
			}
		}

		private void txtDir_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var validationResult = ValidationUtils.IsDirValid(txtDir.Text, "Dir");

			if (!validationResult.IsSuccess)
			{
				errorProvider1.SetError(txtDir, validationResult.ErrorMessage);
				return;
			}

			errorProvider1.SetError(txtDir, "");
		}

		private void txtFileMask_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var validationResult = ValidationUtils.IsNotEmpty(txtFileMask.Text, "FileMask");

			if (!validationResult.IsSuccess)
			{
				errorProvider1.SetError(txtFileMask, validationResult.ErrorMessage);
				return;
			}

			errorProvider1.SetError(txtFileMask, "");
		}

		private void txtFind_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var validationResult = ValidationUtils.IsNotEmpty(txtFind.Text, "Find");

			if (!validationResult.IsSuccess)
			{
				errorProvider1.SetError(txtFind, validationResult.ErrorMessage);
				return;
			}

			errorProvider1.SetError(txtFind, "");
		}

		private void txtReplace_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var validationResult = ValidationUtils.IsNotEmpty(txtReplace.Text, "Replace");

			if (!validationResult.IsSuccess)
			{
				errorProvider1.SetError(txtReplace, validationResult.ErrorMessage);
				return;
			}

			errorProvider1.SetError(txtReplace, "");
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

		private void gvResults_CellClick(object sender, DataGridViewCellEventArgs e)
		{
            if (e.RowIndex == -1)   //heading
                return;

			if (!txtMatches.Visible)
			{
				txtMatches.Visible = true;
				this.Height += txtMatches.Height + 50;
			}

			var matchesPreviewColNumber = gvResults.Columns[4].Visible ? 5 : 3;


            var matchesPreviewText = gvResults.Rows[e.RowIndex].Cells[matchesPreviewColNumber].Value.ToString();

            txtMatches.Text = matchesPreviewText;
			//txtMatches.ReadOnly = true;

			var font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);

			var findText = gvResults.Columns[4].Visible ? txtReplace.Text.Replace("\r\n", "\n") : txtFind.Text.Replace("\r\n", "\n");

			var mathches = chkIsCaseSensitive.Checked ? Regex.Matches(txtMatches.Text, findText) : Regex.Matches(txtMatches.Text, findText, RegexOptions.IgnoreCase);

			foreach (Match match in mathches)
			{
				txtMatches.SelectionStart = match.Index;

				txtMatches.SelectionLength = match.Length;

				txtMatches.SelectionFont = font;

				txtMatches.SelectionColor = Color.CadetBlue;
			}

			txtMatches.SelectionLength = 0;

			//////Add line numbers
			//var linesTooltipString = gvResults.Rows[e.RowIndex].Cells[matchesPreviewColNumber + 1].Value.ToString();
			//var lines = linesTooltipString.Split(' ');

			//int temp;
			//var lineNumbs = lines.Where(l => Int32.TryParse(l, out temp)).Select(l => Convert.ToInt32(l)).ToList();

			//int prevLineIndex = 0, rowIndex = 0;

			//var newLines = new List<string>();

			//for (int i = 0; i < lineNumbs.Count; i++)
			//{
			//    if (i > 0 && lineNumbs[i] - prevLineIndex > 1)
			//    {
			//        newLines.Add("");
			//        rowIndex++;
			//    }

			//    newLines.Add(String.Format("{0}:", lineNumbs[i].ToString("D2")));

			//    rowIndex++;
			//    prevLineIndex = lineNumbs[i];
			//}
		}

		private List<int> GetLineNumbersForMatchesPreview(string filePath, Match match)
		{
			string content = string.Empty;

			using (var sr = new StreamReader(filePath))
			{
				content = sr.ReadToEnd();
			}

            var separator = Environment.NewLine;
			var lines = content.Split(separator.ToCharArray());

			var clearLines = new List<string>();
			for (int i = 0; i < lines.Count(); i++)
				if (i % 2 == 0) clearLines.Add(lines[i]);

			var lineIndexStart = DetectMatchLine(clearLines.ToArray(), match.Index);
			var lineIndexEnd = DetectMatchLine(clearLines.ToArray(), match.Index + match.Length);


			var result = new List<int>();

			for (int i = lineIndexStart - 2; i <= lineIndexEnd + 2; i++)
			{
				if (i >= 0 && i < clearLines.Count())
					result.Add(i);
			}

			return result;

		}

		private string GenerateMatchesPreviewText(string filePath, List<int> rowNumbers)
		{
			string content = string.Empty;

			using (var sr = new StreamReader(filePath))
			{
				content = sr.ReadToEnd();
			}

			var separator = Environment.NewLine;

			var lines = content.Split(separator.ToCharArray());
			var clearLines = new List<string>();
			for (int i = 0; i < lines.Count(); i++)
				if (i % 2 == 0) clearLines.Add(lines[i]);

			var stringBuilder = new StringBuilder();

			rowNumbers = rowNumbers.Distinct().OrderBy(r => r).ToList();
			var prevLineIndex = 0;
			foreach (var rowNumber in rowNumbers)
			{
				if (rowNumber - prevLineIndex > 1 && prevLineIndex != 0) stringBuilder.AppendLine("");
				stringBuilder.AppendLine(clearLines[rowNumber]);
				prevLineIndex = rowNumber;
			}

			return stringBuilder.ToString();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{

		}

		//from http://stackoverflow.com/questions/334630/c-open-folder-and-select-the-file
		private void gvResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex == -1)   //heading
				return;

			var filePath = gvResults.Rows[e.RowIndex].Cells[1].Value.ToString();

			//string argument = @"/select, " + txtDir.Text + filePath.TrimStart('.');
			//Process.Start("explorer.exe", argument);

			string file = txtDir.Text + filePath.TrimStart('.');
			Process.Start(file);

		}

		private ContextMenuStrip CreateContextMenu(int rowNumber)
		{
			var contextMenu = new ContextMenuStrip();

			var openStripItem = new ToolStripMenuItem("Open");


			var eventArgs = new GVResultEventArgs();
			eventArgs.cellRow = rowNumber;
			openStripItem.Click += delegate { toolStripClickOpen(this, eventArgs); };

			var openFolderStripItem = new ToolStripMenuItem("Open Containing Folder");

			openFolderStripItem.Click += delegate { toolStripClickOpenFolder(this, eventArgs); };

			contextMenu.Items.Add(openStripItem);
			contextMenu.Items.Add(openFolderStripItem);

			return contextMenu;
		}

		private void toolStripClickOpen(object sender, GVResultEventArgs e)
		{
			var filePath = gvResults.Rows[e.cellRow].Cells[1].Value.ToString();

			string file = txtDir.Text + filePath.TrimStart('.');
			Process.Start(file);
		}

		private void toolStripClickOpenFolder(object sender, GVResultEventArgs e)
		{
			var filePath = gvResults.Rows[e.cellRow].Cells[1].Value.ToString();

			string argument = @"/select, " + txtDir.Text + filePath.TrimStart('.');
			Process.Start("explorer.exe", argument);
		}
	}
}
