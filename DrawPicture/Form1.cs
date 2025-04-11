using DrawPicture.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawPicture
{
	public partial class Form1 : Form
	{
		private Bitmap _canvas;
		private Shape _shape;
		private Color _canvasBackgroundColor = Color.AliceBlue;
		public Form1()
		{
			InitializeComponent();
			InitializeCanvas(panel_main.Width,panel_main.Height);
			_shape = new Line(_canvas, this.panel_main);
			_shape.ForeColor = Color.Black;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			cmb_size.SelectedIndex = 0;
		}

		private void panel_main_MouseMove(object sender, MouseEventArgs e)
		{
			_shape.ForeColor = btn_showColor.BackColor;
			_shape.MouseMove(e);

			if (_shape.mouseStatus == MouseStatus.LeftButtonPressMove)
			{
				panel_main.Invalidate();
			}
			lb_Penposition.Text = $"{e.Location.X}, {e.Location.Y}像素";
		}

		private void panel_main_MouseDown(object sender, MouseEventArgs e)
		{
			_shape.MouseDown(e);
		}

		private void panel_main_MouseUp(object sender, MouseEventArgs e)
		{
			_shape.MouseUp(e);
		}
		private void panel_main_Paint(object sender, PaintEventArgs e)
		{
			if (_canvas != null)
			{
				e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				_shape.InPainting(e.Graphics);
				if (_shape.IsSelected) 
				{
					_shape.DrawSelected(e.Graphics);
				}
			}
		}
		private void btn_Line_Click(object sender, EventArgs e)
		{
			_shape = new Line(_canvas, panel_main);
		}

		private void Form1_Resize(object sender, EventArgs e)
		{
			ResizeCanvasToPanel();
		}

		private void btn_save_Click(object sender, EventArgs e)
		{
			SavePng();
		}

		private void btn_open_Click(object sender, EventArgs e)
		{
			OpenPng();
		}

		//释放 Bitmap 资源
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			_canvas.Dispose();
		}

		private void cmb_size_SelectedIndexChanged(object sender, EventArgs e)
		{
			_shape.Size = float.Parse(cmb_size.Text.Substring(0, 1));
		}

		private void InitializeCanvas(int width, int height)
		{
			_canvas = new Bitmap(width, width);
			using (Graphics g = Graphics.FromImage(_canvas))
			{
				g.Clear(_canvasBackgroundColor); // 初始化背景色
				g.SmoothingMode = SmoothingMode.HighQuality; //高质量
				g.PixelOffsetMode = PixelOffsetMode.HighQuality; //高像素偏移质量
			}
			// 启用双缓冲
			typeof(Panel).InvokeMember(
			"DoubleBuffered",
			System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
			null,
			panel_main,
			new object[] { true });
		}

		private void ResizeCanvasToPanel()
		{
			// 获取新的 Panel 尺寸
			int newWidth = this.Width;
			int newHeight = this.Height;

			if (newWidth <= 500) return;
			if (newHeight <= 500) return;

			// 创建一个新的 Bitmap，尺寸与 Panel 相同
			 Bitmap newCanvas = new Bitmap(newWidth, newHeight);

			// 如果已有画布内容，将其绘制到新的画布上
			if (_canvas != null)
			{
				using (Graphics g = Graphics.FromImage(newCanvas))
				{
					g.Clear(_canvasBackgroundColor); // 清除背景
					g.DrawImage(_canvas, Point.Empty); // 绘制原有内容
				}
			}

			// 更新当前画布
			_canvas?.Dispose(); // 释放旧的画布资源
			_canvas = newCanvas;
			_shape.canvas = _canvas;
			panel_main.Invalidate();
		}
		private void SavePng()
		{
			// 保存位图为 PNG 文件
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "PNG Image|*.png",
				Title = "保存为 PNG 图片"
			};

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					_canvas.Save(saveFileDialog.FileName, ImageFormat.Png);
					MessageBox.Show("图片已成功保存！", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void OpenPng()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "PNG Image|*.png|All Files|*.*",
				Title = "打开图片"
			};

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					// 使用 using 确保临时对象被正确释放
					using (Bitmap tempBitmap = new Bitmap(openFileDialog.FileName))
					{
						// 创建深拷贝以避免文件句柄锁定
						_canvas = new Bitmap(tempBitmap);
					}
					_shape.canvas = _canvas;

					// 调整 Panel 大小以适应图片
					panel_main.Width = _canvas.Width;
					panel_main.Height = _canvas.Height;

					// 触发重绘
					panel_main.Invalidate();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"无法加载图片：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		//黑色
		private void btn_BlackColor_Click(object sender, EventArgs e)
		{
			btn_showColor.BackColor = Color.Black;
		}

		//灰色50%
		private void btn_greyColor_Click(object sender, EventArgs e)
		{
			btn_showColor.BackColor = Color.FromArgb(128, 128, 128, 128);
		}

		//深红色
		private void btn_darkRedColor_Click(object sender, EventArgs e)
		{
			btn_showColor.BackColor = ColorTranslator.FromHtml("#8B0000");
		}
		//红色
		private void btn_RedColor_Click(object sender, EventArgs e)
		{
			btn_showColor.BackColor = Color.Red;
		}

		private void btn_OrangeColor_Click(object sender, EventArgs e)
		{
			btn_showColor.BackColor = Color.Orange;
		}
	}
}
