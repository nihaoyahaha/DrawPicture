using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawPicture
{
	public partial class ScreenshotApp : Form
	{
		public ScreenshotApp()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			using (var captureForm = new CaptureForm())
			{
				captureForm.ShowDialog(this);
			}
		}
	}
}
