using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawKit.UserControl
{
	[ToolboxBitmap(typeof(RichTextBox))]
	public class TransparentRichTextBox : RichTextBox
	{
		private string _emptyTextTip;
		private Color _emptyTextTipColor = Color.DarkGray;
		private const int WM_PAINT = 0xF;
		public TransparentRichTextBox() : base()
		{
			base.BorderStyle = BorderStyle.None;
		}
		[DefaultValue("")]
		public string EmptyTextTip
		{
			get { return _emptyTextTip; }
			set
			{
				_emptyTextTip = value;
				base.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "DarkGray")]
		public Color EmptyTextTipColor
		{
			get { return _emptyTextTipColor; }
			set
			{
				_emptyTextTipColor = value;
				base.Invalidate();
			}
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr LoadLibrary(string lpFileName);
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams prams = base.CreateParams;
				if (LoadLibrary("msftedit.dll") != IntPtr.Zero)
				{
					prams.ExStyle |= 0x020; // transparent 
					prams.ClassName = "RICHEDIT50W";
				}
				return prams;
			}
		}



		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (m.Msg == WM_PAINT && !Focused && Text.Length == 0 && !string.IsNullOrEmpty(_emptyTextTip))
			{
				using (Graphics graphics = Graphics.FromHwnd(base.Handle))
				{
					TextFormatFlags format =
						TextFormatFlags.EndEllipsis |
						TextFormatFlags.VerticalCenter;

					if (RightToLeft == RightToLeft.Yes)
					{
						format |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
					}

					TextRenderer.DrawText(
						graphics,
						_emptyTextTip,
						Font,
						base.ClientRectangle,
						_emptyTextTipColor,
						format);
				}
			}
		}


	}
}
