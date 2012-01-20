namespace FindAndReplace.App
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.btnReplace = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtFileMask = new System.Windows.Forms.TextBox();
            this.btnFindOnly = new System.Windows.Forms.Button();
            this.chkIsCaseSensitive = new System.Windows.Forms.CheckBox();
            this.chkIncludeSubDirectories = new System.Windows.Forms.CheckBox();
            this.btnGenReplaceCommandLine = new System.Windows.Forms.Button();
            this.txtCommandLine = new System.Windows.Forms.TextBox();
            this.lblCommandLine = new System.Windows.Forms.Label();
            this.pnlCommandLine = new System.Windows.Forms.Panel();
            this.gvResults = new System.Windows.Forms.DataGridView();
            this.lblResults = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlGridResults = new System.Windows.Forms.Panel();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtNoMathces = new System.Windows.Forms.Label();
            this.txtMatches = new System.Windows.Forms.RichTextBox();
            this.pnlCommandLine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvResults)).BeginInit();
            this.pnlGridResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtFind
            // 
            this.txtFind.CausesValidation = false;
            this.txtFind.Location = new System.Drawing.Point(83, 93);
            this.txtFind.Multiline = true;
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(539, 74);
            this.txtFind.TabIndex = 1;
            this.txtFind.Validating += new System.ComponentModel.CancelEventHandler(this.txtFind_Validating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Find:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 200);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Replace:";
            // 
            // txtReplace
            // 
            this.txtReplace.CausesValidation = false;
            this.txtReplace.Location = new System.Drawing.Point(83, 200);
            this.txtReplace.Multiline = true;
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(539, 74);
            this.txtReplace.TabIndex = 3;
            this.txtReplace.Validating += new System.ComponentModel.CancelEventHandler(this.txtReplace_Validating);
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(547, 280);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(75, 23);
            this.btnReplace.TabIndex = 7;
            this.btnReplace.Text = "Replace";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Dir:";
            // 
            // txtDir
            // 
            this.txtDir.CausesValidation = false;
            this.txtDir.Location = new System.Drawing.Point(83, 19);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(539, 20);
            this.txtDir.TabIndex = 9;
            this.txtDir.Validating += new System.ComponentModel.CancelEventHandler(this.txtDir_Validating);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "File Mask:";
            // 
            // txtFileMask
            // 
            this.txtFileMask.CausesValidation = false;
            this.txtFileMask.Location = new System.Drawing.Point(83, 64);
            this.txtFileMask.Name = "txtFileMask";
            this.txtFileMask.Size = new System.Drawing.Size(274, 20);
            this.txtFileMask.TabIndex = 11;
            this.txtFileMask.TabStop = false;
            this.txtFileMask.Text = "*.*";
            this.txtFileMask.Validating += new System.ComponentModel.CancelEventHandler(this.txtFileMask_Validating);
            // 
            // btnFindOnly
            // 
            this.btnFindOnly.Location = new System.Drawing.Point(547, 171);
            this.btnFindOnly.Name = "btnFindOnly";
            this.btnFindOnly.Size = new System.Drawing.Size(75, 23);
            this.btnFindOnly.TabIndex = 12;
            this.btnFindOnly.Text = "Find Only";
            this.btnFindOnly.UseVisualStyleBackColor = true;
            this.btnFindOnly.Click += new System.EventHandler(this.btnFindOnly_Click);
            // 
            // chkIsCaseSensitive
            // 
            this.chkIsCaseSensitive.AutoSize = true;
            this.chkIsCaseSensitive.Checked = true;
            this.chkIsCaseSensitive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIsCaseSensitive.Location = new System.Drawing.Point(83, 173);
            this.chkIsCaseSensitive.Name = "chkIsCaseSensitive";
            this.chkIsCaseSensitive.Size = new System.Drawing.Size(96, 17);
            this.chkIsCaseSensitive.TabIndex = 13;
            this.chkIsCaseSensitive.Text = "Case Sensitive";
            this.chkIsCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // chkIncludeSubDirectories
            // 
            this.chkIncludeSubDirectories.AutoSize = true;
            this.chkIncludeSubDirectories.Checked = true;
            this.chkIncludeSubDirectories.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIncludeSubDirectories.Location = new System.Drawing.Point(83, 41);
            this.chkIncludeSubDirectories.Name = "chkIncludeSubDirectories";
            this.chkIncludeSubDirectories.Size = new System.Drawing.Size(133, 17);
            this.chkIncludeSubDirectories.TabIndex = 13;
            this.chkIncludeSubDirectories.Text = "Include SubDirectories";
            this.chkIncludeSubDirectories.UseVisualStyleBackColor = true;
            // 
            // btnGenReplaceCommandLine
            // 
            this.btnGenReplaceCommandLine.Location = new System.Drawing.Point(448, 308);
            this.btnGenReplaceCommandLine.Name = "btnGenReplaceCommandLine";
            this.btnGenReplaceCommandLine.Size = new System.Drawing.Size(174, 23);
            this.btnGenReplaceCommandLine.TabIndex = 19;
            this.btnGenReplaceCommandLine.Text = "Gen Replace Command Line";
            this.btnGenReplaceCommandLine.UseVisualStyleBackColor = true;
            this.btnGenReplaceCommandLine.Click += new System.EventHandler(this.btnGenReplaceCommandLine_Click);
            // 
            // txtCommandLine
            // 
            this.txtCommandLine.Location = new System.Drawing.Point(76, 11);
            this.txtCommandLine.Multiline = true;
            this.txtCommandLine.Name = "txtCommandLine";
            this.txtCommandLine.Size = new System.Drawing.Size(711, 74);
            this.txtCommandLine.TabIndex = 15;
            // 
            // lblCommandLine
            // 
            this.lblCommandLine.AutoSize = true;
            this.lblCommandLine.Location = new System.Drawing.Point(-3, 11);
            this.lblCommandLine.Name = "lblCommandLine";
            this.lblCommandLine.Size = new System.Drawing.Size(80, 13);
            this.lblCommandLine.TabIndex = 20;
            this.lblCommandLine.Text = "Command Line:";
            // 
            // pnlCommandLine
            // 
            this.pnlCommandLine.Controls.Add(this.lblCommandLine);
            this.pnlCommandLine.Controls.Add(this.txtCommandLine);
            this.pnlCommandLine.Location = new System.Drawing.Point(7, 344);
            this.pnlCommandLine.Name = "pnlCommandLine";
            this.pnlCommandLine.Size = new System.Drawing.Size(797, 100);
            this.pnlCommandLine.TabIndex = 21;
            this.pnlCommandLine.Visible = false;
            // 
            // gvResults
            // 
            this.gvResults.AllowUserToAddRows = false;
            this.gvResults.AllowUserToDeleteRows = false;
            this.gvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvResults.Location = new System.Drawing.Point(68, 9);
            this.gvResults.MultiSelect = false;
            this.gvResults.Name = "gvResults";
            this.gvResults.ReadOnly = true;
            this.gvResults.RowHeadersVisible = false;
            this.gvResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvResults.Size = new System.Drawing.Size(712, 129);
            this.gvResults.TabIndex = 18;
            this.gvResults.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvResults_CellClick);
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Location = new System.Drawing.Point(17, 9);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(45, 13);
            this.lblResults.TabIndex = 19;
            this.lblResults.Text = "Results:";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(68, 170);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(712, 23);
            this.progressBar.TabIndex = 20;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(70, 154);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(16, 13);
            this.lblStatus.TabIndex = 21;
            this.lblStatus.Text = "...";
            // 
            // pnlGridResults
            // 
            this.pnlGridResults.Controls.Add(this.lblStatus);
            this.pnlGridResults.Controls.Add(this.progressBar);
            this.pnlGridResults.Controls.Add(this.lblResults);
            this.pnlGridResults.Controls.Add(this.gvResults);
            this.pnlGridResults.Location = new System.Drawing.Point(6, 345);
            this.pnlGridResults.Name = "pnlGridResults";
            this.pnlGridResults.Size = new System.Drawing.Size(797, 196);
            this.pnlGridResults.TabIndex = 22;
            this.pnlGridResults.Visible = false;
            // 
            // errorProvider1
            // 
            this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider1.ContainerControl = this;
            // 
            // txtNoMathces
            // 
            this.txtNoMathces.AutoSize = true;
            this.txtNoMathces.Location = new System.Drawing.Point(80, 317);
            this.txtNoMathces.Name = "txtNoMathces";
            this.txtNoMathces.Size = new System.Drawing.Size(97, 13);
            this.txtNoMathces.TabIndex = 23;
            this.txtNoMathces.Text = " No matches found";
            this.txtNoMathces.Visible = false;
            // 
            // txtAreaMatches
            // 
            this.txtMatches.BackColor = System.Drawing.SystemColors.Info;
            this.txtMatches.Location = new System.Drawing.Point(74, 563);
            this.txtMatches.Name = "txtMatches";
            this.txtMatches.ReadOnly = true;
            this.txtMatches.Size = new System.Drawing.Size(729, 166);
            this.txtMatches.TabIndex = 24;
            this.txtMatches.Text = "";
            this.txtMatches.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 339);
            this.Controls.Add(this.txtMatches);
            this.Controls.Add(this.txtNoMathces);
            this.Controls.Add(this.pnlGridResults);
            this.Controls.Add(this.btnGenReplaceCommandLine);
            this.Controls.Add(this.chkIncludeSubDirectories);
            this.Controls.Add(this.chkIsCaseSensitive);
            this.Controls.Add(this.btnFindOnly);
            this.Controls.Add(this.txtFileMask);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtDir);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFind);
            this.Controls.Add(this.pnlCommandLine);
            this.Name = "MainForm";
            this.Text = "Find and Replace";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.pnlCommandLine.ResumeLayout(false);
            this.pnlCommandLine.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvResults)).EndInit();
            this.pnlGridResults.ResumeLayout(false);
            this.pnlGridResults.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtFind;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtReplace;
		private System.Windows.Forms.Button btnReplace;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtDir;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtFileMask;
		private System.Windows.Forms.Button btnFindOnly;
		private System.Windows.Forms.CheckBox chkIsCaseSensitive;
		private System.Windows.Forms.CheckBox chkIncludeSubDirectories;
		private System.Windows.Forms.Button btnGenReplaceCommandLine;
		private System.Windows.Forms.TextBox txtCommandLine;
		private System.Windows.Forms.Label lblCommandLine;
		private System.Windows.Forms.Panel pnlCommandLine;
		public System.Windows.Forms.DataGridView gvResults;
		private System.Windows.Forms.Label lblResults;
		public System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Panel pnlGridResults;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Label txtNoMathces;
		public System.Windows.Forms.RichTextBox txtMatches;
	}
}

