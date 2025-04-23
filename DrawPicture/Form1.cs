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
		private Color _canvasBackgroundColor = Color.White;
		public Form1()
		{
			InitializeComponent();
			InitializeCanvas();
			_shape = new Circle(_canvas, this.panel_main);
			panel_main.BackColor = Color.AliceBlue;
		}
		
		private void Form1_Load(object sender, EventArgs e)
		{
			cmb_size.SelectedIndex = 3;
		}

		private void panel_main_MouseMove(object sender, MouseEventArgs e)
		{
			_shape.MouseMove(e);
			//int offsetX = (panel_main.Width - _canvas.Width) / 2;
			//int offsetY = (panel_main.Height - _canvas.Height) / 2;
			//lb_Penposition.Text = $"{e.Location.X-offsetX}, {e.Location.Y-offsetY}像素";
		}

		private void panel_main_MouseDown(object sender, MouseEventArgs e)
		{
			_shape.MouseDown(e);
		}

		private void panel_main_MouseUp(object sender, MouseEventArgs e)
		{
			_shape.MouseUp(e);
			if (_shape.drawStatus == DrawStatus.CompleteCanvasAdjustment)
			{
				int width = _shape.AdjustingCanvasRect.Width;
				int height = _shape.AdjustingCanvasRect.Height;
				GenerateStretchedBitmap(width,height);
			}
		}
		private void CreateNewBitmap(int width, int height)
		{
			Bitmap newCanvas = new Bitmap(width, height);
			if (_canvas != null)
			{
				//将现在bitmap其绘制到新的bitmap上
				using (Graphics g = Graphics.FromImage(newCanvas))
				{
					g.Clear(_canvasBackgroundColor);
					g.DrawImage(_canvas, Point.Empty);
				}
				_canvas.Dispose();
				_canvas = newCanvas;
				_shape.canvas = _canvas;
			}
			panel_main.Invalidate();
		}

		private void GenerateStretchedBitmap(int width, int height)
		{
			Bitmap newCanvas = new Bitmap(width, height);
			if (_canvas != null)
			{
				using (Graphics g = Graphics.FromImage(newCanvas))
				{
					g.Clear(_canvasBackgroundColor);
					g.DrawImage(_canvas, _shape.BitmapStretchOffsetPoint);
				}
				_canvas.Dispose();
				_canvas = newCanvas;
				_shape.canvas = _canvas;
			}
			panel_main.Invalidate();
		}

		private void panel_main_Paint(object sender, PaintEventArgs e)
		{
			if (_canvas != null)
			{
				e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				_shape.InPainting(e.Graphics);
			}
		}

		//直線
		private void btn_Line_Click(object sender, EventArgs e)
		{
			_shape = new Line(_canvas, panel_main)
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//消しゴム
		private void btn_Erase_Click(object sender, EventArgs e)
		{
			_shape = new Eraser(_canvas, panel_main)
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//矩形選択
		private void btn_select_Click(object sender, EventArgs e)
		{
			_shape = new RectangularSelection(_canvas, panel_main)
			{
				ForeColor = btn_showColor.BackColor
			};
			panel_main.Invalidate();
		}

		//カラーフィル
		private void btn_Fill_Click(object sender, EventArgs e)
		{
			_shape = new OilTank(_canvas, panel_main)
			{
				ForeColor = btn_showColor.BackColor
			};
			panel_main.Invalidate();
		}

		//長方形
		private void btn_rectangle_Click(object sender, EventArgs e)
		{
			_shape = new ShapeRectangle(_canvas, panel_main)
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//五角形
		private void btn_pentagon_Click(object sender, EventArgs e)
		{
			_shape = new Pentagon(_canvas, panel_main)
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//円
		private void btn_circle_Click(object sender, EventArgs e)
		{
			_shape = new Circle(_canvas, panel_main)
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//三角形
		private void btn_triangle_Click(object sender, EventArgs e)
		{
			_shape = new Triangle(_canvas, panel_main)
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//直角三角形
		private void btn_RightTriangle_Click(object sender, EventArgs e)
		{
			_shape = new RightTriangle(_canvas, panel_main)
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//ひし形
		private void btn_rhombus_Click(object sender, EventArgs e)
		{
			_shape = new Rhombus(_canvas, panel_main)
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}
		//六角形
		private void btn_hexagon_Click(object sender, EventArgs e)
		{
			_shape = new Hexagon(_canvas, panel_main)
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}
		//フィレット長方形
		private void btn_roundedRectangle_Click(object sender, EventArgs e)
		{
			_shape = new RoundedRectangle(_canvas, panel_main)
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}
		private void Form1_Resize(object sender, EventArgs e)
		{
			CreateNewBitmap(_canvas.Width,_canvas.Height);
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
			_shape.drawStatus = DrawStatus.AdjustTheStyle;
			if (_shape is RectangularSelection) return;
			panel_main.Refresh();
		}

		private void InitializeCanvas()
		{
			_canvas = new Bitmap(400, 300);
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
			SetShapeForeColor(Color.Black);
		}

		//灰色50%
		private void btn_greyColor_Click(object sender, EventArgs e)
		{
			SetShapeForeColor(Color.FromArgb(255, 128, 128, 128));
		}

		//深红色
		private void btn_darkRedColor_Click(object sender, EventArgs e)
		{
			SetShapeForeColor(ColorTranslator.FromHtml("#8B0000"));
		}
		//红色
		private void btn_RedColor_Click(object sender, EventArgs e)
		{
			SetShapeForeColor(Color.Red);
		}

		private void btn_OrangeColor_Click(object sender, EventArgs e)
		{
			SetShapeForeColor(Color.Orange);
		}

		private void btn_WhileColor_Click(object sender, EventArgs e)
		{
			SetShapeForeColor(Color.White);
		}

		private void SetShapeForeColor( Color color)
		{
			btn_showColor.BackColor = color;
			_shape.ForeColor = color;
			_shape.drawStatus = DrawStatus.AdjustTheStyle;
			if (_shape is RectangularSelection) return;
			panel_main.Refresh();
		}

		private void btn_RightRotate90_Click(object sender, EventArgs e)
		{
			if (_shape is Eraser) return;
			if (_shape is OilTank) return;
			_shape.Rotate(90);
			panel_main.Refresh();
		}

		private void btn_LeftRotate90_Click(object sender, EventArgs e)
		{
			if (_shape is Eraser) return;
			if (_shape is OilTank) return;
			_shape.Rotate(-90);
			panel_main.Refresh();
		}

		private void btn_Rotate180_Click(object sender, EventArgs e)
		{
			if (_shape is Eraser) return;
			if (_shape is OilTank) return;
			_shape.Rotate(180);
			panel_main.Refresh();
		}

		private void btn_FlipVertical_Click(object sender, EventArgs e)
		{
			if (_shape is Eraser) return;
			if (_shape is OilTank) return;
			_shape.FlipHorizontal();
			panel_main.Refresh();
		}

		private void btn_FlipHorizontal_Click(object sender, EventArgs e)
		{
			if (_shape is Eraser) return;
			if (_shape is OilTank) return;
			_shape.FlipVertical();
			panel_main.Refresh();
		}

		private void btn_ClearAll_Click(object sender, EventArgs e)
		{
			_shape.Clear(_canvasBackgroundColor);
		}



	}
}
