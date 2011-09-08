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
			this.txtFind = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtReplace = new System.Windows.Forms.TextBox();
			this.gvResults = new System.Windows.Forms.DataGridView();
			this.label3 = new System.Windows.Forms.Label();
			this.btnReplace = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.txtDir = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtFileMask = new System.Windows.Forms.TextBox();
			this.btnFindOnly = new System.Windows.Forms.Button();
			this.chkBoxCaseSense = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.gvResults)).BeginInit();
			this.SuspendLayout();
			// 
			// txtFind
			// 
			this.txtFind.Location = new System.Drawing.Point(83, 76);
			this.txtFind.Multiline = true;
			this.txtFind.Name = "txtFind";
			this.txtFind.Size = new System.Drawing.Size(539, 74);
			this.txtFind.TabIndex = 1;
			this.txtFind.Text = "Test";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(34, 76);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(30, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Find:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(34, 183);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Replace:";
			// 
			// txtReplace
			// 
			this.txtReplace.Location = new System.Drawing.Point(83, 183);
			this.txtReplace.Multiline = true;
			this.txtReplace.Name = "txtReplace";
			this.txtReplace.Size = new System.Drawing.Size(539, 74);
			this.txtReplace.TabIndex = 3;
			this.txtReplace.Text = "Test1";
			// 
			// gvResults
			// 
			this.gvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gvResults.Location = new System.Drawing.Point(83, 315);
			this.gvResults.Name = "gvResults";
			this.gvResults.Size = new System.Drawing.Size(539, 129);
			this.gvResults.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(32, 315);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Results:";
			// 
			// btnReplace
			// 
			this.btnReplace.Location = new System.Drawing.Point(547, 263);
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
			this.label5.Location = new System.Drawing.Point(11, 50);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(55, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "File Mask:";
			// 
			// txtFileMask
			// 
			this.txtFileMask.Location = new System.Drawing.Point(83, 47);
			this.txtFileMask.Name = "txtFileMask";
			this.txtFileMask.Size = new System.Drawing.Size(274, 20);
			this.txtFileMask.TabIndex = 11;
			this.txtFileMask.TabStop = false;
			this.txtFileMask.Text = "*.*";
			// 
			// btnFindOnly
			// 
			this.btnFindOnly.Location = new System.Drawing.Point(547, 154);
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
			this.chkBoxCaseSense.Location = new System.Drawing.Point(83, 156);
			this.chkBoxCaseSense.Name = "chkBoxCaseSense";
			this.chkBoxCaseSense.Size = new System.Drawing.Size(107, 17);
			this.chkBoxCaseSense.TabIndex = 13;
			this.chkBoxCaseSense.Text = "Is Case Sensitive";
			this.chkBoxCaseSense.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(691, 463);
			this.Controls.Add(this.chkBoxCaseSense);
			this.Controls.Add(this.btnFindOnly);
			this.Controls.Add(this.txtFileMask);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.txtDir);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btnReplace);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.gvResults);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtReplace);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtFind);
			this.Name = "MainForm";
			this.Text = "Find and Replace";
			((System.ComponentModel.ISupportInitialize)(this.gvResults)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtFind;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtReplace;
		private System.Windows.Forms.DataGridView gvResults;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnReplace;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtDir;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtFileMask;
		private System.Windows.Forms.Button btnFindOnly;
		private System.Windows.Forms.CheckBox chkBoxCaseSense;
	}
}

