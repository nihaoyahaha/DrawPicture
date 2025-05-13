using DrawKit.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;

namespace DrawKit
{
	public partial class CanvasForm : Form
	{
		public string FilePath;
		public event Action OnConfirm;
		private Bitmap _canvas;
		private Shape _shape;
		private Color _canvasBackgroundColor = Color.White;
		private float[] _scales = { 0.125f, 0.25f, 0.5f, 1, 2, 3, 4, 5, 6, 7, 8 };

		public CanvasForm()
		{
			InitializeComponent();
			
			panel_main.BackColor = Color.AliceBlue;
			panel_main.MouseWheel += Panel_MouseWheel;
			rtb_Text.Visible = false;
			
			InitializeCanvas();
			LoadInstalledFonts();
			_shape = new Circle(_canvas, this.panel_main, _scales[trackBar_scale.Value]);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			cmb_size.SelectedIndex = 0;
			cmb_TextSize.SelectedIndex = 0;
			cmb_FontFamily.SelectedIndex = 0;
			SetTextFont();
			UpdateLabel(trackBar_scale.Value);
		}

		private void panel_main_MouseMove(object sender, MouseEventArgs e)
		{
			_shape.MouseMove(e);
			GetMousePositionOnBitmap(e.Location);
			GetBitmapSize();
		}

		private void GetMousePositionOnBitmap(Point point)
		{
			if (_shape.IsValidLocation(point))
			{
				int offsetX = (panel_main.Width - _canvas.Width) / 2;
				int offsetY = (panel_main.Height - _canvas.Height) / 2;
				var canvaslocation = _shape.GetCanvasRegion();
				lb_Penposition.Text = $"{(int)(point.X/ _scales[trackBar_scale.Value]) -(int)( canvaslocation.X/ _scales[trackBar_scale.Value])}, {(int)(point.Y/ _scales[trackBar_scale.Value]) - (int)(canvaslocation.Y/ _scales[trackBar_scale.Value])}ピクセル";
			}
			else
			{
				lb_Penposition.Text = "";
			}
		}

		private void GetBitmapSize()
		{
			if (_shape.drawStatus == DrawStatus.CanvasAdjusting)
			{
				lb_CanvasSize.Text = $"{(int)(_shape.AdjustingCanvasRect.Width/_shape.Scale)},{(int)(_shape.AdjustingCanvasRect.Height/_shape.Scale)}ピクセル";
			}
			else if (_shape.drawStatus == DrawStatus.Creating ||
				_shape.drawStatus == DrawStatus.Adjusting)
			{
				lb_SelectionSize.Text = $"{(int)(_shape.SelectionRect.Width/_shape.Scale)},{(int)(_shape.SelectionRect.Height/_shape.Scale)}ピクセル";
			}
		}

		private void Panel_MouseWheel(object sender, MouseEventArgs e)
		{
			if (ModifierKeys == Keys.Control)
			{
				if (e.Delta > 0)
				{
					pic_amplify_Click(null, null);
				}
				else if (e.Delta < 0)
				{
					pic_reduce_Click(null, null);
				}
			}
			if (panel_main.DisplayRectangle.Width > panel_main.ClientSize.Width)
			{
				CreateNewBitmap(_canvas.Width, _canvas.Height);
			}
			if (panel_main.DisplayRectangle.Height > panel_main.ClientSize.Height)
			{
				CreateNewBitmap(_canvas.Width, _canvas.Height);
			}
		}

		private void panel_main_MouseDown(object sender, MouseEventArgs e)
		{
			_shape.MouseDown(e);
		}

		private void panel_main_MouseUp(object sender, MouseEventArgs e)
		{
			if (_shape is TextBoxArea textBoxArea)
			{
				textBoxArea.richTextBox = rtb_Text;
			}
			_shape.MouseUp(e);
			//if (_shape.drawStatus == DrawStatus.Creating)
			//{
			//	rtb_Text.Text = "";
			//}

			if (_shape.drawStatus == DrawStatus.CompleteCanvasAdjustment)
			{
				GenerateStretchedBitmap();
			}
			SetRichTextBoxLocation();
		}

		private void SetRichTextBoxLocation()
		{
			if (_shape is TextBoxArea)
			{
				var rect = _shape.GetCanvasRegion();

				if (_shape.SelectionRect.X <= rect.X)
				{
					_shape.SelectionRect.X = rect.X;
				}
				if (_shape.SelectionRect.Y <= rect.Y)
				{
					_shape.SelectionRect.Y = rect.Y;
				}
				if (_shape.SelectionRect.X + _shape.SelectionRect.Width >= rect.Right)
				{
					if (rect.Right - _shape.SelectionRect.X <= 5)
					{
						_shape.SelectionRect.Width = 5;
						_shape.SelectionRect.X = rect.Right - 20;
					}
					else
					{
						_shape.SelectionRect.Width = rect.Right - _shape.SelectionRect.X;
					}

				}

				//if (_shape.SelectionRect.Y + _shape.SelectionRect.Height >= rect.Bottom)
				//	_shape.SelectionRect.Height = rect.Bottom - rect.Y;

				rtb_Text.Location = new Point(_shape.SelectionRect.X + 5, _shape.SelectionRect.Y + 5);
				rtb_Text.Size = new Size(_shape.SelectionRect.Width - 10, _shape.SelectionRect.Height - 10);
				rtb_Text.Visible = true;

				string text = rtb_Text.Text;
				rtb_Text.Text = text;
				rtb_Text.SelectionStart = text.Length;
				rtb_Text.Focus();
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
			SetRichTextBoxLocation();
			_shape.drawStatus = DrawStatus.CanAdjusted;
			var rect = _shape.GetCanvasRegion();
			panel_main.AutoScrollMinSize = new Size(rect.Width ,rect.Height );
			panel_main.Invalidate();
		}

		private Bitmap GetNonScaledBitmap()
		{
			int width = (int)(_canvas.Width / _shape.Scale);
			int height = (int)(_canvas.Height / _shape.Scale);
			Bitmap newCanvas = new Bitmap(width, height);
			if (_canvas != null)
			{
				using (Graphics g = Graphics.FromImage(newCanvas))
				{
					g.Clear(_canvasBackgroundColor);
					var offsetPoint = _shape.BitmapStretchOffsetPoint;
					Point point = new Point((int)(offsetPoint.X / _shape.Scale), (int)(offsetPoint.Y / _shape.Scale));
					g.DrawImage(_canvas, point);
				}
				_canvas.Dispose();
				_canvas = newCanvas;
			}
			return _canvas;
		}

		private void GenerateStretchedBitmap()
		{
			//int width = _shape.AdjustingCanvasRect.Width;
		    //int height = _shape.AdjustingCanvasRect.Height;
			int width = (int)(_shape.AdjustingCanvasRect.Width / _shape.Scale);
			int height = (int)(_shape.AdjustingCanvasRect.Height / _shape.Scale);
			Bitmap newCanvas = new Bitmap(width, height);
			if (_canvas != null)
			{
				using (Graphics g = Graphics.FromImage(newCanvas))
				{
					g.Clear(_canvasBackgroundColor);
					//g.DrawImage(_canvas, _shape.BitmapStretchOffsetPoint);
					var offsetPoint = _shape.BitmapStretchOffsetPoint;
					Point point = new Point((int)(offsetPoint.X / _shape.Scale), (int)(offsetPoint.Y / _shape.Scale));
					g.DrawImage(_canvas,point);
				}
				_canvas.Dispose();
				_canvas = newCanvas;
				_shape.canvas = _canvas;
			}
			//panel_main.AutoScrollMinSize = new Size(_canvas.Width, _canvas.Height);
			panel_main.Invalidate();
			var rect = _shape.GetCanvasRegion();
			panel_main.AutoScrollMinSize = new Size(rect.Width, rect.Height);
		}

		private void panel_main_Paint(object sender, PaintEventArgs e)
		{
		//	if (panel_main.AutoScrollPosition.X != 0) return;
		//	if (panel_main.AutoScrollPosition.Y != 0) return;
			if (_canvas != null)
			{

				e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

				_shape.InPainting(e.Graphics);
			}
		}

		//直線
		private void btn_Line_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			//var bitmap = GetNonScaledBitmap();
			_shape = new Line(_canvas, panel_main, _scales[trackBar_scale.Value])
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//消しゴム
		private void btn_Erase_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			_shape = new Eraser(_canvas, panel_main, _scales[trackBar_scale.Value])
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//矩形選択
		private void btn_select_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			_shape = new RectangularSelection(_canvas, panel_main, _scales[trackBar_scale.Value])
			{
				ForeColor = btn_showColor.BackColor
			};
			panel_main.Invalidate();
		}

		//カラーフィル
		private void btn_Fill_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			_shape = new OilTank(_canvas, panel_main, _scales[trackBar_scale.Value])
			{
				ForeColor = btn_showColor.BackColor
			};
			panel_main.Invalidate();
		}

		//長方形
		private void btn_rectangle_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			_shape = new ShapeRectangle(_canvas, panel_main, _scales[trackBar_scale.Value])
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//五角形
		private void btn_pentagon_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			_shape = new Pentagon(_canvas, panel_main, _scales[trackBar_scale.Value])
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//円
		private void btn_circle_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			_shape = new Circle(_canvas, panel_main, _scales[trackBar_scale.Value])
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//三角形
		private void btn_triangle_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			_shape = new Triangle(_canvas, panel_main, _scales[trackBar_scale.Value])
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//直角三角形
		private void btn_RightTriangle_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			_shape = new RightTriangle(_canvas, panel_main, _scales[trackBar_scale.Value])
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}

		//ひし形
		private void btn_rhombus_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			_shape = new Rhombus(_canvas, panel_main, _scales[trackBar_scale.Value])
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}
		//六角形
		private void btn_hexagon_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			_shape = new Hexagon(_canvas, panel_main, _scales[trackBar_scale.Value])
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}
		//フィレット長方形
		private void btn_roundedRectangle_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			_shape = new RoundedRectangle(_canvas, panel_main, _scales[trackBar_scale.Value])
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}
		//テキスト
		private void btn_Text_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			_shape = new TextBoxArea(_canvas, panel_main, _scales[trackBar_scale.Value])
			{
				ForeColor = btn_showColor.BackColor,
				Size = float.Parse(cmb_size.Text.Substring(0, 1))
			};
			panel_main.Invalidate();
		}
		private void Form1_Resize(object sender, EventArgs e)
		{
			CreateNewBitmap(_canvas.Width, _canvas.Height);
		}

		private void btn_save_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			SavePng();
		}

		private void btn_open_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
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
			_canvas = new Bitmap(320, 192);
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
			lb_SelectionSize.Text = "";
			lb_CanvasSize.Text = $"{(int)(_canvas.Width/ _scales[trackBar_scale.Value])},{(int)(_canvas.Height/ _scales[trackBar_scale.Value])}ピクセル";
			panel_main.AutoScrollMinSize = new Size(_canvas.Width, _canvas.Height);
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
					MessageBox.Show("画像は正常に保存されました！", "保存に成功しました", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"保存に失敗しました：{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void Save()
		{
			if (string.IsNullOrEmpty(FilePath)) return;
			ImageFormat imageFormat = GetImageFormatFromExtension(Path.GetExtension(FilePath));
			_canvas.Save(FilePath, imageFormat);

		}

		private ImageFormat GetImageFormatFromExtension(string extension)
		{
			var formatMap = new Dictionary<string, ImageFormat>(StringComparer.OrdinalIgnoreCase){
			{ ".bmp", ImageFormat.Bmp },
			{ ".gif", ImageFormat.Gif },
			{ ".jpg", ImageFormat.Jpeg },
			{ ".jpeg", ImageFormat.Jpeg },
			{ ".png", ImageFormat.Png },
			{ ".tiff", ImageFormat.Tiff },
			{ ".wmf", ImageFormat.Wmf },
			{ ".emf", ImageFormat.Emf }};
			return formatMap.TryGetValue(extension, out var format) ? format : ImageFormat.Jpeg;
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
					using (Bitmap tempBitmap = new Bitmap(openFileDialog.FileName))
					{
						_canvas = new Bitmap(tempBitmap);
					}
					_shape.canvas = _canvas;

					panel_main.Invalidate();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"画像をロードできません：{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

		private void SetShapeForeColor(Color color)
		{
			btn_showColor.BackColor = color;
			_shape.ForeColor = color;
			_shape.drawStatus = DrawStatus.AdjustTheStyle;
			rtb_Text.ForeColor = color;
			if (_shape is RectangularSelection) return;
			panel_main.Refresh();
		}

		private void btn_RightRotate90_Click(object sender, EventArgs e)
		{
			if (_shape is TextBoxArea) return;
			if (_shape.SelectionRect == Rectangle.Empty || _shape.SelectionRect.Width == 0 || _shape.SelectionRect.Height ==0)
			{
				_shape.CanvasRotateRight();
				CreateNewBitmap(_canvas.Width, _canvas.Height);
			}
			else
			{
				_shape.RotateRight();
				panel_main.Refresh();
			}	
		}

		private void btn_LeftRotate90_Click(object sender, EventArgs e)
		{
			if (_shape is TextBoxArea) return;
			if (_shape.SelectionRect == Rectangle.Empty || _shape.SelectionRect.Width == 0 || _shape.SelectionRect.Height == 0)
			{
				_shape.CanvasRotateLeft();
				CreateNewBitmap(_canvas.Width, _canvas.Height);
			}
			else
			{
				_shape.RotateLeft();
				panel_main.Refresh();
			}
		}

		private void btn_Rotate180_Click(object sender, EventArgs e)
		{
			if (_shape is TextBoxArea) return;
			if (_shape.SelectionRect == Rectangle.Empty || _shape.SelectionRect.Width == 0 || _shape.SelectionRect.Height == 0)
			{
				_shape.CanvasRotate180();
				CreateNewBitmap(_canvas.Width, _canvas.Height);
			}
			else
			{
				_shape.Rotate180();
				panel_main.Refresh();
			}
		}

		private void btn_FlipVertical_Click(object sender, EventArgs e)
		{
			if (_shape is TextBoxArea) return;
			if (_shape.SelectionRect == Rectangle.Empty || _shape.SelectionRect.Width == 0 || _shape.SelectionRect.Height == 0)
			{
				_shape.CanvasFlipVertical();
				CreateNewBitmap(_canvas.Width, _canvas.Height);
			}
			else
			{
				_shape.FlipVertical();
				panel_main.Refresh();
			}
		}

		private void btn_FlipHorizontal_Click(object sender, EventArgs e)
		{
			if (_shape is TextBoxArea) return;
			if (_shape.SelectionRect == Rectangle.Empty || _shape.SelectionRect.Width == 0 || _shape.SelectionRect.Height == 0)
			{
				_shape.CanvasFlipHorizontal();
				CreateNewBitmap(_canvas.Width, _canvas.Height);
			}
			else
			{
				_shape.FlipHorizontal();
				panel_main.Refresh();
			}
		}

		private void btn_ClearAll_Click(object sender, EventArgs e)
		{
			_shape.Clear(_canvasBackgroundColor);
		}

		private void cmb_TextSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetTextFont();
			if (_shape is TextBoxArea area && area.SelectionRect.Width > 0 && area.SelectionRect.Height > 0)
			{
				area.SetRichTextBoxMinSize(rtb_Text.Font.Size, ref area.SelectionRect);
				SetRichTextBoxLocation();
				_shape.drawStatus = DrawStatus.CanAdjusted;
				panel_main.Invalidate();
				rtb_Text.Focus();
			}
		}

		private void SetTextFont()
		{
			int fontSize = int.Parse(cmb_TextSize.Text);
			string fontFamily = cmb_FontFamily.Text;
			Font font = new Font(fontFamily, fontSize);
			rtb_Text.Font = font;



			//MessageBox.Show(rtb_Text.PreferredHeight.ToString());
			//// 尝试创建字体
			//using (Font font = new Font(fontFamily, fontSize))
			//{
			//	// 设置 RichTextBox 的默认字体
			//	rtb_Text.Font = font;

			//	// 确保当前插入点的字体也被设置为新的字体
			//	rtb_Text.SelectionFont = rtb_Text.Font;
			//}

			//// 检查字体是否存在
			//if (FontFamily.Families.Any(f => f.Name == fontFamily))
			//{
			//	Font font = new Font(fontFamily, fontSize);

			//	// 选择整个内容
			//	rtb_Text.SelectAll();
			//	rtb_Text.Font = font;


			//}
		}
		private void LoadInstalledFonts()
		{
			InstalledFontCollection installedFonts = new InstalledFontCollection();
			char japaneseChar = '你';//'あ';

			foreach (FontFamily fontFamily in installedFonts.Families)
			{
				try
				{
					using (Font font = new Font(fontFamily, 12))
					{
						if (TextRenderer.MeasureText(japaneseChar.ToString(), font).Width > 0)
						{
							cmb_FontFamily.Items.Add(fontFamily.Name);
						}
					}
				}
				catch
				{
				}
			}
		}

		private void cmb_FontFamily_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetTextFont();
		}

		private void btn_Ok_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			Save();
			OnConfirm?.Invoke();
		}
		
		/// <summary>
		/// 拡大
		/// </summary>
		private void pic_reduce_Click(object sender, EventArgs e)
		{
			if (trackBar_scale.Value - 1 >= trackBar_scale.Minimum)
			{
				UpdateLabel(trackBar_scale.Value -= 1);
			}
		}
	
		/// <summary>
		/// 縮小
		/// </summary>
		private void pic_amplify_Click(object sender, EventArgs e)
		{
			if (trackBar_scale.Value + 1 <= trackBar_scale.Maximum)
			{
				UpdateLabel(trackBar_scale.Value += 1);
			}
		}

		private void trackBar_scale_ValueChanged(object sender, EventArgs e)
		{
			UpdateLabel(trackBar_scale.Value);
		}
		private void UpdateLabel(int index)
		{
			float currentValue = _scales[index];
			lb_scale.Text = $"{currentValue*100}%";
			_shape.Scale = currentValue;
			//panel_main.AutoScrollMinSize = new Size(_canvas.Width,_canvas.Height);
			panel_main.Invalidate();
			var rect = _shape.GetCanvasRegion();
			panel_main.AutoScrollMinSize = new Size(rect.Width,rect.Height);
		}

		private void panel_main_Scroll(object sender, ScrollEventArgs e)
		{
			CreateNewBitmap(_canvas.Width,_canvas.Height);
		}
	}
}
