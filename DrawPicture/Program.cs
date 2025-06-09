using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrawKit;

namespace DrawPicture
{
    static class Program
	{
		//[DllImport("user32.dll")]
		//public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

		//[DllImport("user32.dll")]
		//public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new  DrawKit.Screenshot.CaptureForm());return;
			var form = new ScreenshotApp();

			// 注册 Alt+A 快捷键
			//RegisterHotKey(form.Handle, ScreenshotApp.HOTKEY_ID, 1, (int)Keys.B);

			Application.Run(form);

			//UnregisterHotKey(form.Handle, ScreenshotApp.HOTKEY_ID);
		}
    }
}
