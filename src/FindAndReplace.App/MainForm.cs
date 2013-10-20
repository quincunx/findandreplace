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

	public partial class MainForm : Form
	{
		public const int ExtraWidthWhenResults = 350;

		private Finder _finder;
		private Replacer _replacer;
		private Thread _currentThread;

        public bool _isFindOnly;
	    private FormData _lastOperationFormData;
	    

		private delegate void SetFindResultCallback(Finder.FindResultItem resultItem, Stats stats, Status status);
		
		private delegate void SetReplaceResultCallback(Replacer.ReplaceResultItem resultItem, Stats stats, Status status);

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
			_isFindOnly = true;

			if (!ValidateForm())
				return;
			
			PrepareFinderGrid();

			lblStats.Text = "";
			lblStatus.Text = "Getting file list...";

			var finder = new Finder();
			finder.Dir = txtDir.Text;

			finder.IncludeSubDirectories = chkIncludeSubDirectories.Checked;
			finder.FileMask = txtFileMask.Text;
			finder.FindTextHasRegEx = chkIsRegEx.Checked;
			finder.FindText = txtFind.Text;
			finder.IsCaseSensitive = chkIsCaseSensitive.Checked;
			finder.SkipBinaryFileDetection = chkSkipBinaryFileDetection.Checked;
			finder.IncludeFilesWithoutMatches = chkIncludeFilesWithoutMatches.Checked;
			finder.ExcludeFileMask = txtExcludeFileMask.Text;

			CreateListener(finder);

			ShowResultPanel();

			SaveToRegistry();

			_currentThread = new Thread(DoFindWork);
			_currentThread.IsBackground = true;

			_currentThread.Start();
		}

		private void SaveToRegistry()
		{
			var data = new FormData();

		    data.IsFindOnly = _isFindOnly;

			data.Dir = txtDir.Text;
			data.FileMask = txtFileMask.Text;
			data.ExcludeFileMask = txtExcludeFileMask.Text;
			data.FindText = txtFind.Text;
			data.IncludeSubDirectories = chkIncludeSubDirectories.Checked;
			data.IsCaseSensitive = chkIsCaseSensitive.Checked;
			data.IsRegEx = chkIsRegEx.Checked;
			data.SkipBinaryFileDetection = chkSkipBinaryFileDetection.Checked;
			data.IncludeFilesWithoutMatches = chkIncludeFilesWithoutMatches.Checked;
			data.ReplaceText = txtReplace.Text;

			data.SaveToRegistry();

		    _lastOperationFormData = data;
		}


		private void PrepareFinderGrid()
		{
			gvResults.DataSource = null;

			gvResults.Rows.Clear();
			gvResults.Columns.Clear();

			AddResultsColumn("Filename", "Filename", 250);
			AddResultsColumn("Path", "Path", 450);
			AddResultsColumn("NumMatches", "Matches", 50);
			AddResultsColumn("ErrorMessage", "Error", 150);

			gvResults.Columns.Add("Tooltip", "");
			gvResults.Columns.Add("TooltipLineNums", "");
			gvResults.Columns[4].Visible = false;
			gvResults.Columns[5].Visible = false;
			HideMatchesPreviewPanel();

			progressBar.Value = 0;
		}

		private void AddResultsColumn(string dataPropertyName, string headerText, int width)
		{
			gvResults.Columns.Add(new DataGridViewColumn()
			                      	{
			                      		DataPropertyName = dataPropertyName,
			                      		HeaderText = headerText,
			                      		CellTemplate = new DataGridViewTextBoxCell(),
			                      		Width = width,
			                      		SortMode = DataGridViewColumnSortMode.Automatic
			                      	});
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
				ShowFindResult(e.ResultItem, e.Stats, e.Status);
			}
			else
			{
				SetFindResultCallback findResultCallback = ShowFindResult;
				this.Invoke(findResultCallback, new object[] { e.ResultItem, e.Stats, e.Status });
			}
		}

		private void ShowFindResult(Finder.FindResultItem findResultItem, Stats stats, Status status)
		{
			if (stats.Files.Total != 0)
			{
				if (findResultItem.IncludeInResultsList)
				{
					gvResults.Rows.Add();

					int currentRow = gvResults.Rows.Count - 1;

					gvResults.Rows[currentRow].ContextMenuStrip = CreateContextMenu(currentRow);

					gvResults.Rows[currentRow].Cells[0].Value = findResultItem.FileName;
					gvResults.Rows[currentRow].Cells[1].Value = findResultItem.FileRelativePath;
					gvResults.Rows[currentRow].Cells[2].Value = findResultItem.NumMatches;
					gvResults.Rows[currentRow].Cells[3].Value = findResultItem.ErrorMessage;
					
					gvResults.Rows[currentRow].Resizable = DataGridViewTriState.False;

					if (findResultItem.IsSuccess  && findResultItem.NumMatches > 0)  //Account for errors and IncludeFilesWithoutMatches
					{
						string fileContent = string.Empty;
						
						using (var sr = new StreamReader(findResultItem.FilePath, findResultItem.FileEncoding))
						{
							fileContent = sr.ReadToEnd();
						}

						
						List<MatchPreviewLineNumber> lineNumbers = Utils.GetLineNumbersForMatchesPreview(fileContent, findResultItem.Matches);
						gvResults.Rows[currentRow].Cells[4].Value = GenerateMatchesPreviewText(fileContent, lineNumbers.Select(ln => ln.LineNumber).ToList());
					}

					//Grid likes to select the first row for some reason
					if (gvResults.Rows.Count == 1)
						gvResults.ClearSelection();
					
				}

				progressBar.Maximum = stats.Files.Total;
				progressBar.Value = stats.Files.Processed;

				lblStatus.Text = "Processing " + stats.Files.Processed + " of " + stats.Files.Total + " files.  Last file: " +  findResultItem.FileRelativePath;

				ShowStats(stats);
			}
			else
			{
				HideResultPanel();

				txtNoMatches.Visible = true;

				HideStats();
			}
			
			

			//When last file - enable buttons back
			if (status == Status.Completed || status == Status.Cancelled)
			{
				if (status == Status.Completed)
					lblStatus.Text = "Processed " + stats.Files.Processed + " files.";
				
				if (status == Status.Cancelled)
					lblStatus.Text = "Operation was cancelled.";

				EnableButtons();
			}
				
		}

		private void DisableButtons()
		{
			//this.Cursor = Cursors.WaitCursor;

			UpdateButtons(false);
		}

		private void EnableButtons()
		{
			UpdateButtons(true);

			//this.Cursor = Cursors.Arrow;
		}

		private void UpdateButtons(bool enabled)
		{
			btnFindOnly.Enabled = enabled;
			btnReplace.Enabled = enabled;
			btnGenReplaceCommandLine.Enabled = enabled;
			btnCancel.Enabled = !enabled;
		}

		private void DoFindWork()
		{
			_finder.Find();
		}

		private void ShowResultPanel()
		{
			DisableButtons();

			txtNoMatches.Visible = false;
			
			HideCommandLinePanel();
			HideMatchesPreviewPanel();

			if (!pnlGridResults.Visible)
			{
				pnlGridResults.Visible = true;

				if (pnlCommandLine.Visible)
				{
					this.Height -= pnlCommandLine.Height + 10;
					pnlCommandLine.Visible = false;
				}

				this.Height += pnlGridResults.Height + 10;
				this.Width += ExtraWidthWhenResults;
			}
		}

		private void HideResultPanel()
		{
			if (pnlGridResults.Visible)
			{
				pnlGridResults.Visible = false;

				this.Height -= pnlGridResults.Height + 10;
				this.Width -= ExtraWidthWhenResults;
			}
		}
		
		private void ShowCommandLinePanel()
		{
			HideResultPanel();
			HideMatchesPreviewPanel();

			if (!pnlCommandLine.Visible)
			{
				pnlCommandLine.Visible = true;
				this.Height += pnlCommandLine.Height + 10;
				this.Width += ExtraWidthWhenResults;
			}
		}

		private void HideCommandLinePanel()
		{
			if (pnlCommandLine.Visible)
			{
				pnlCommandLine.Visible = false;
				this.Height -= pnlCommandLine.Height + 10;
				this.Width -= ExtraWidthWhenResults;
			}
		}

		private void ShowMatchesPreviewPanel()
		{
			if (!txtMatchesPreview.Visible)
			{
				txtMatchesPreview.Visible = true;
				this.Height += txtMatchesPreview.Height + 50;
			}

		}

		private void HideMatchesPreviewPanel()
		{
			if (txtMatchesPreview.Visible)
			{
				txtMatchesPreview.Visible = false;
				this.Height -= (txtMatchesPreview.Height + 50);
			}
		}

		private bool ValidateForm()
		{
			var isFormValid = true;
			Control firstInvalidControl = null;

			foreach (Control control in Controls)
			{
				control.Focus();

				if (!Validate() || errorProvider1.GetError(control) != "")
				{
					if (isFormValid)
						firstInvalidControl = control;

					isFormValid = false;
				}
				else
				{
					errorProvider1.SetError(control, "");
				}
			}

			//Focus on first invalid control
			if (firstInvalidControl != null)
				firstInvalidControl.Focus();

            if (!isFormValid && this.AutoValidate == AutoValidate.Disable)
                this.AutoValidate = AutoValidate.EnablePreventFocusChange;  //Revalidate on focus change
		    
			return isFormValid;
		}

		private void btnReplace_Click(object sender, EventArgs e)
		{
			_isFindOnly = false;

			if (!ValidateForm())
				return;

			if (String.IsNullOrEmpty(txtReplace.Text))
			{
				DialogResult dlgResult = MessageBox.Show(this, 
														"Are you sure you would like to replace with an empty string?", "Replace Confirmation",
				                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (dlgResult == DialogResult.No)
					return;
			}

			var replacer = new Replacer();

			replacer.Dir = txtDir.Text;
			replacer.IncludeSubDirectories = chkIncludeSubDirectories.Checked;

			replacer.FileMask = txtFileMask.Text;
			replacer.ExcludeFileMask = txtExcludeFileMask.Text;

			replacer.FindText = txtFind.Text;
			replacer.IsCaseSensitive = chkIsCaseSensitive.Checked;
			replacer.FindTextHasRegEx = chkIsRegEx.Checked;
			replacer.SkipBinaryFileDetection = chkSkipBinaryFileDetection.Checked;
			replacer.IncludeFilesWithoutMatches = chkIncludeFilesWithoutMatches.Checked;
			replacer.ReplaceText = txtReplace.Text;
		
			ShowResultPanel();

			lblStats.Text = "";
			lblStatus.Text = "Getting file list...";

			PrepareReplacerGrid();
			txtMatchesPreview.Visible = false;

			CreateListener(replacer);

			SaveToRegistry();

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

			AddResultsColumn("Filename", "Filename", 250);
			AddResultsColumn("Path", "Path", 400);
			AddResultsColumn("NumMatches", "Matches", 50);
			AddResultsColumn("IsSuccess", "Replaced", 60);
			AddResultsColumn("ErrorMessage", "Error", 150);

			gvResults.Columns.Add("MatchesPreview", "");
			gvResults.Columns.Add("MatchesPreviewLineNums", "");
			gvResults.Columns[5].Visible = false;
			gvResults.Columns[6].Visible = false;

			HideMatchesPreviewPanel();
			progressBar.Value = 0;
		}

		private void DoReplaceWork()
		{
			_replacer.Replace();
		}

		private void ShowReplaceResult(Replacer.ReplaceResultItem replaceResultItem, Stats stats, Status status)
		{
			if (stats.Files.Total > 0)
			{
				if (replaceResultItem.IncludeInResultsList)
				{
					gvResults.Rows.Add();

					int currentRow = gvResults.Rows.Count - 1;

					gvResults.Rows[currentRow].ContextMenuStrip = CreateContextMenu(currentRow);

					gvResults.Rows[currentRow].Cells[0].Value = replaceResultItem.FileName;
					gvResults.Rows[currentRow].Cells[1].Value = replaceResultItem.FileRelativePath;
					gvResults.Rows[currentRow].Cells[2].Value = replaceResultItem.NumMatches;
					gvResults.Rows[currentRow].Cells[3].Value = replaceResultItem.IsReplaced ? "Yes" : "No";
					gvResults.Rows[currentRow].Cells[4].Value = replaceResultItem.ErrorMessage;
					
					gvResults.Rows[currentRow].Resizable = DataGridViewTriState.False;

					if (replaceResultItem.IsSuccess && replaceResultItem.NumMatches > 0)  //Account for errors and IncludeFilesWithoutMatches
					{
						string fileContent = string.Empty;

						using (var sr = new StreamReader(replaceResultItem.FilePath, replaceResultItem.FileEncoding))
						{
							fileContent = sr.ReadToEnd();
						}
						
						List<MatchPreviewLineNumber> lineNumbers = Utils.GetLineNumbersForMatchesPreview(fileContent, replaceResultItem.Matches, _lastOperationFormData.ReplaceText.Length, true);
						gvResults.Rows[currentRow].Cells[5].Value = GenerateMatchesPreviewText(fileContent, lineNumbers.Select(ln => ln.LineNumber).ToList());
					}

					//Grid likes to select the first row for some reason
					if (gvResults.Rows.Count == 1)
						gvResults.ClearSelection();
				}

				progressBar.Maximum = stats.Files.Total;
				progressBar.Value = stats.Files.Processed;

				lblStatus.Text = "Processing " + stats.Files.Processed + " of " + stats.Files.Total + " files.  Last file: " + replaceResultItem.FileRelativePath;;

				ShowStats(stats, true);
			}
			else
			{
				HideResultPanel();

				txtNoMatches.Visible = true;

				HideStats();
			}
			

			//When last file - enable buttons back
			if (status == Status.Completed || status == Status.Cancelled)
			{
				if (status == Status.Completed)
					lblStatus.Text = "Processed " + stats.Files.Processed + " files.";

				if (status == Status.Cancelled)
					lblStatus.Text = "Operation was cancelled.";

				EnableButtons();
			}
		}

		private void ReplaceFileProceed(object sender, ReplacerEventArgs e)
		{
			if (!this.gvResults.InvokeRequired)
			{
				ShowReplaceResult(e.ResultItem,  e.Stats, e.Status);
			}
			else
			{
				var replaceResultCallback = new SetReplaceResultCallback(ShowReplaceResult);
				this.Invoke(replaceResultCallback, new object[] { e.ResultItem, e.Stats, e.Status });
			}
		}

		private void btnGenReplaceCommandLine_Click(object sender, EventArgs e)
		{
			if (!ValidateForm())
				return;

			ShowCommandLinePanel();
			lblStats.Text = "";
			
			txtCommandLine.Clear();
			
			string s = String.Format("\"{0}\" --cl --dir \"{1}\" --fileMask \"{2}\"{3}{4}{5}{6}{7}{8} --find \"{9}\" --replace \"{10}\"",
									 Application.ExecutablePath,
									 txtDir.Text.TrimEnd('\\'),
									 txtFileMask.Text,
									 String.IsNullOrEmpty(txtExcludeFileMask.Text) ? "" : String.Format(" --excludeFileMask \"{0}\"", CommandLineUtils.EncodeText(txtExcludeFileMask.Text)),
									 chkIncludeSubDirectories.Checked ? " --includeSubDirectories" : "",
									 chkIsCaseSensitive.Checked ? " --caseSensitive" : "",
									 chkIsRegEx.Checked ? " --useRegEx" : "",
									 chkSkipBinaryFileDetection.Checked ? " --skipBinaryFileDetection" : "",
									 chkIncludeFilesWithoutMatches.Checked ? " --includeFilesWithoutMatches" : "",
									 CommandLineUtils.EncodeText(txtFind.Text),
									 CommandLineUtils.EncodeText(txtReplace.Text)
									 );

			txtCommandLine.Text = s;
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

			validationResult = ValidationUtils.IsValidRegExp(txtFind.Text, "Find");

			if (chkIsRegEx.Checked && !validationResult.IsSuccess)
			{
				errorProvider1.SetError(txtFind, validationResult.ErrorMessage);
				return;
			}

			errorProvider1.SetError(txtFind, "");
		}

		private void gvResults_CellClick(object sender, DataGridViewCellEventArgs e)
		{
            if (e.RowIndex == -1)   //heading
                return;

            int matchesPreviewColNumber = _lastOperationFormData.IsFindOnly ? 4 : 5;

			if (gvResults.Rows[e.RowIndex].Cells[matchesPreviewColNumber].Value == null)
			{
				HideMatchesPreviewPanel();
				return;
			}

			ShowMatchesPreviewPanel();

            var matchesPreviewText = gvResults.Rows[e.RowIndex].Cells[matchesPreviewColNumber].Value.ToString();

			txtMatchesPreview.SelectionLength = 0;
			txtMatchesPreview.Clear();

			txtMatchesPreview.Text = matchesPreviewText;
		
			var font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

            //Use _lastOperation form data since user may change it before clicking on preview
            var findText = _lastOperationFormData.IsFindOnly ? _lastOperationFormData.FindText : _lastOperationFormData.ReplaceText;
            findText = findText.Replace("\r\n", "\n");

            findText = (_lastOperationFormData.IsRegEx && _lastOperationFormData.IsFindOnly) ? findText : Regex.Escape(findText);
            var mathches = Regex.Matches(txtMatchesPreview.Text, findText, Utils.GetRegExOptions(_lastOperationFormData.IsCaseSensitive));

			int count = 0;
			int maxCount = 1000;

			foreach (Match match in mathches)
			{
				txtMatchesPreview.SelectionStart = match.Index;

				txtMatchesPreview.SelectionLength = match.Length;

				txtMatchesPreview.SelectionFont = font;

				txtMatchesPreview.SelectionColor = Color.CadetBlue;

				//Limit highlighted matches, otherwise may lock up the app .  Happened with 65K+
				count++;
				if (count > maxCount)
					break;
			}

			txtMatchesPreview.SelectionLength = 0;
		}

		private string GenerateMatchesPreviewText(string content, List<int> rowNumbers)
		{
			var separator = Environment.NewLine;

			var lines = content.Split(new string[] { separator }, StringSplitOptions.None);

			var stringBuilder = new StringBuilder();

			rowNumbers = rowNumbers.Distinct().OrderBy(r => r).ToList();
			var prevLineIndex = 0;
			string lineSeparator = ("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");

			foreach (var rowNumber in rowNumbers)
			{
				if (rowNumber - prevLineIndex > 1 && prevLineIndex != 0)
				{
					stringBuilder.AppendLine("");
					stringBuilder.AppendLine(lineSeparator);
					stringBuilder.AppendLine("");
				}
				stringBuilder.AppendLine(lines[rowNumber]);
				prevLineIndex = rowNumber;
			}

			return stringBuilder.ToString();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			InitWithRegistryData();
		}

		//from http://stackoverflow.com/questions/334630/c-open-folder-and-select-the-file
		private void gvResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex == -1)   //heading
				return;

			OpenFileUsingExternalApp(e.RowIndex);
		}

		private void OpenFileUsingExternalApp(int rowIndex)
		{
			var filePath = gvResults.Rows[rowIndex].Cells[1].Value.ToString();

			string file = txtDir.Text + filePath.TrimStart('.');
			Process.Start(file);
		}

		private ContextMenuStrip CreateContextMenu(int rowNumber)
		{
			var contextMenu = new ContextMenuStrip();
			contextMenu.ShowImageMargin = false;

			var openMenuItem = new ToolStripMenuItem("Open");

			var eventArgs = new GVResultEventArgs();
			eventArgs.cellRow = rowNumber;
			openMenuItem.Click += delegate { contextMenu_ClickOpen(this, eventArgs); };

			var openFolderMenuItem = new ToolStripMenuItem("Open Containing Folder");
			openFolderMenuItem.Click += delegate { contextMenu_ClickOpenFolder(this, eventArgs); };

			contextMenu.Items.Add(openMenuItem);
			contextMenu.Items.Add(openFolderMenuItem);

			return contextMenu;
		}

		private void contextMenu_ClickOpen(object sender, GVResultEventArgs e)
		{
			OpenFileUsingExternalApp(e.cellRow);
		}

		private void contextMenu_ClickOpenFolder(object sender, GVResultEventArgs e)
		{
			var filePath = gvResults.Rows[e.cellRow].Cells[1].Value.ToString();

			string argument = @"/select, " + txtDir.Text + filePath.TrimStart('.');
			Process.Start("explorer.exe", argument);
		}

		private void ShowStats(Stats stats, bool showReplaceStats=false)
		{
			var sb = new StringBuilder();
			sb.AppendLine("Files:");
			sb.AppendLine("- Total: " + stats.Files.Total);
			sb.AppendLine("- Processed: " + stats.Files.Processed);
			sb.AppendLine("- Binary: " + stats.Files.Binary + " (skipped)");
			sb.AppendLine("- With Matches: " + stats.Files.WithMatches);
			sb.AppendLine("- Without Matches: " + stats.Files.WithoutMatches);
			sb.AppendLine("- Failed to Open: " + stats.Files.FailedToRead);
			
			if (showReplaceStats)
				sb.AppendLine("- Failed to Write: " + stats.Files.FailedToWrite);
			
			sb.AppendLine("");
			sb.AppendLine("Matches:");
			sb.AppendLine("- Found: " + stats.Matches.Found);

			if (showReplaceStats)
				sb.AppendLine("- Replaced: " + stats.Matches.Replaced);

			var passedSeconds = stats.Time.Passed.TotalSeconds;
			var remainingSeconds = stats.Time.Remaining.TotalSeconds;

			if (passedSeconds >= 1)
			{
				sb.AppendLine("");
				sb.AppendLine("Time:");
				sb.AppendLine("- Passed: " + Utils.FormatTimeSpan(stats.Time.Passed));

				if (passedSeconds >= 3 && (int)remainingSeconds != 0)
				{
					sb.AppendLine("- Remaining: " + Utils.FormatTimeSpan(stats.Time.Remaining) + " (estimated)");
				}
			}

			lblStats.Text = sb.ToString();
		}


		private void HideStats()
		{
			lblStats.Text = String.Empty;
		}
		

		public class GVResultEventArgs : EventArgs
		{
			public int cellRow { get; set; }
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			if (_currentThread.IsAlive)
			{
				if (_isFindOnly)
					_finder.CancelFind();
				else
					_replacer.CancelReplace();
			}
		}

		private void InitWithRegistryData()
		{
			var data = new FormData();
			if (data.IsEmpty()) //Keep defaults
				return;
			
			data.LoadFromRegistry();

			txtDir.Text = data.Dir;
			txtFileMask.Text = data.FileMask;
			txtExcludeFileMask.Text = data.ExcludeFileMask;
			txtFind.Text = data.FindText;
			txtReplace.Text = data.ReplaceText;
			chkIncludeSubDirectories.Checked = data.IncludeSubDirectories;
			chkIsCaseSensitive.Checked = data.IsCaseSensitive;
			chkSkipBinaryFileDetection.Checked = data.SkipBinaryFileDetection;
			chkIncludeFilesWithoutMatches.Checked = data.IncludeFilesWithoutMatches;
			chkIsRegEx.Checked = data.IsRegEx;
		}

		private void btnSelectDir_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = txtDir.Text;
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				txtDir.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void txtReplace_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.A))
			{
				txtReplace.SelectAll();
				e.SuppressKeyPress = true;
				e.Handled = true;
			}
		}

		private void txtFind_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.A))
			{
				txtFind.SelectAll();
				e.SuppressKeyPress = true;
				e.Handled = true;
			}
		}

		private void txtCommandLine_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.A))
			{
				txtCommandLine.SelectAll();
				e.SuppressKeyPress = true;
				e.Handled = true;
			}
		}

		private void btnSwap_Click(object sender, EventArgs e)
		{
			string findText = txtFind.Text;

			txtFind.Text = txtReplace.Text;
			txtReplace.Text = findText;
		}

	}
}
 