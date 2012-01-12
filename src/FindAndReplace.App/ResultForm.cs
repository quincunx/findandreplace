using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FindAndReplace.App
{
	public partial class ResultForm : Form
	{
		public ResultForm()
		{
			InitializeComponent();
		}

		private void closeBtn_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
