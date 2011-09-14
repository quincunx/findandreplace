namespace FindAndReplace
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
			this.chkBoxCaseSense = new System.Windows.Forms.CheckBox();
			this.chkSubDir = new System.Windows.Forms.CheckBox();
			this.txtCommandLine = new System.Windows.Forms.TextBox();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.label3 = new System.Windows.Forms.Label();
			this.gvResults = new System.Windows.Forms.DataGridView();
			this.btnGen = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.gvResults)).BeginInit();
			this.SuspendLayout();
			// 
			// txtFind
			// 
			this.txtFind.Location = new System.Drawing.Point(83, 93);
			this.txtFind.Multiline = true;
			this.txtFind.Name = "txtFind";
			this.txtFind.Size = new System.Drawing.Size(539, 74);
			this.txtFind.TabIndex = 1;
			this.txtFind.Text = "Test";
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
			this.txtReplace.Location = new System.Drawing.Point(83, 200);
			this.txtReplace.Multiline = true;
			this.txtReplace.Name = "txtReplace";
			this.txtReplace.Size = new System.Drawing.Size(539, 74);
			this.txtReplace.TabIndex = 3;
			this.txtReplace.Text = "Test1";
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
			this.txtDir.Location = new System.Drawing.Point(83, 19);
			this.txtDir.Name = "txtDir";
			this.txtDir.Size = new System.Drawing.Size(539, 20);
			this.txtDir.TabIndex = 9;
			this.txtDir.Text = "C:\\Temp\\FindAndReplaceTest";
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
			this.txtFileMask.Location = new System.Drawing.Point(83, 64);
			this.txtFileMask.Name = "txtFileMask";
			this.txtFileMask.Size = new System.Drawing.Size(274, 20);
			this.txtFileMask.TabIndex = 11;
			this.txtFileMask.TabStop = false;
			this.txtFileMask.Text = "*.*";
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
			// chkBoxCaseSense
			// 
			this.chkBoxCaseSense.AutoSize = true;
			this.chkBoxCaseSense.Location = new System.Drawing.Point(83, 173);
			this.chkBoxCaseSense.Name = "chkBoxCaseSense";
			this.chkBoxCaseSense.Size = new System.Drawing.Size(107, 17);
			this.chkBoxCaseSense.TabIndex = 13;
			this.chkBoxCaseSense.Text = "Is Case Sensitive";
			this.chkBoxCaseSense.UseVisualStyleBackColor = true;
			// 
			// chkSubDir
			// 
			this.chkSubDir.AutoSize = true;
			this.chkSubDir.Checked = true;
			this.chkSubDir.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSubDir.Location = new System.Drawing.Point(83, 41);
			this.chkSubDir.Name = "chkSubDir";
			this.chkSubDir.Size = new System.Drawing.Size(133, 17);
			this.chkSubDir.TabIndex = 13;
			this.chkSubDir.Text = "Include SubDirectories";
			this.chkSubDir.UseVisualStyleBackColor = true;
			// 
			// txtCommandLine
			// 
			this.txtCommandLine.Location = new System.Drawing.Point(83, 338);
			this.txtCommandLine.Multiline = true;
			this.txtCommandLine.Name = "txtCommandLine";
			this.txtCommandLine.Size = new System.Drawing.Size(539, 74);
			this.txtCommandLine.TabIndex = 15;
			this.txtCommandLine.Visible = false;
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(83, 581);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(539, 23);
			this.progressBar1.TabIndex = 18;
			this.progressBar1.Visible = false;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(32, 434);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 13);
			this.label3.TabIndex = 17;
			this.label3.Text = "Results:";
			this.label3.Visible = false;
			// 
			// gvResults
			// 
			this.gvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gvResults.Location = new System.Drawing.Point(83, 434);
			this.gvResults.Name = "gvResults";
			this.gvResults.Size = new System.Drawing.Size(539, 129);
			this.gvResults.TabIndex = 16;
			this.gvResults.Visible = false;
			// 
			// btnGen
			// 
			this.btnGen.Location = new System.Drawing.Point(448, 309);
			this.btnGen.Name = "btnGen";
			this.btnGen.Size = new System.Drawing.Size(174, 23);
			this.btnGen.TabIndex = 19;
			this.btnGen.Text = "Gen Replace Command Line";
			this.btnGen.UseVisualStyleBackColor = true;
			this.btnGen.Click += new System.EventHandler(this.btnGen_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(641, 338);
			this.Controls.Add(this.btnGen);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.gvResults);
			this.Controls.Add(this.txtCommandLine);
			this.Controls.Add(this.chkSubDir);
			this.Controls.Add(this.chkBoxCaseSense);
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
			this.Name = "MainForm";
			this.Text = "Find and Replace";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.gvResults)).EndInit();
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
		private System.Windows.Forms.CheckBox chkBoxCaseSense;
		private System.Windows.Forms.CheckBox chkSubDir;
		private System.Windows.Forms.TextBox txtCommandLine;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.DataGridView gvResults;
		private System.Windows.Forms.Button btnGen;
	}
}

