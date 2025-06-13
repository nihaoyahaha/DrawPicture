using DrawKit.Shapes;
using DrawKit.History;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using DrawKit.Screenshot;
using System.Runtime.InteropServices;
using System.Linq;

namespace DrawKit
{
	public partial class CanvasForm : Form
	{
		[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
		private const int HOTKEY_ID = 1;

		public string FilePath;
		public event Action OnConfirm;
		private Bitmap _canvas;
		private Shape _shape;
		private Color _canvasBackgroundColor = Color.White;
		private float[] _scales = { 0.125f, 0.25f, 0.5f, 0.75f, 1, 2, 3, 4, 5, 6, 7, 8 };
		private float _scaleDelta = 0.1f;
		private CaptureForm _captureForm;
		private MouseMessageFilter messageFilter;
		private string _cmbScaleLastText = "";
		public CanvasForm()
		{
			InitializeComponent();
			InitCmbScaleItems();
			InitBackColor();
			panel_main.MouseWheel += Panel_MouseWheel;
			OperationStep.OnOperationCompleted += RevokeAndRedoAction;
			rtb_Text.Visible = false;
			InitializeCanvas();
			LoadInstalledFonts();
			_shape = new Pencil(_canvas, this.panel_main, GetCmbscaleSelectedItemKey());
			messageFilter = new MouseMessageFilter(cmb_scales, this);
			Application.AddMessageFilter(messageFilter);
		}

		private void InitBackColor()
		{
			BackColor = Color.FromArgb(247, 249, 254);
			panel_TextStyle.BackColor = Color.FromArgb(247, 249, 254);
			panel_main.BackColor = Color.FromArgb(247, 249, 254);
			toolStrip1.BackColor = Color.FromArgb(204, 213, 240);
			panel1.BackColor = Color.FromArgb(64, 80, 141);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			cmb_size.SelectedIndex = 0;
			cmb_TextSize.SelectedIndex = 0;
			cmb_FontFamily.SelectedIndex = 0;
			SetTextFont();
			SetCanvasScale(GetCmbscaleSelectedItemKey());
			OperationStep.InitStack();
			SetPanelTextStyle();
			if (!RegisterHotKey(this.Handle, HOTKEY_ID, (uint)0x0001, (uint)Keys.B))
			{
				MessageBox.Show("无法注册热键，请重试。");
			}
		}

		private void panel_main_MouseMove(object sender, MouseEventArgs e)
		{
			_shape.MouseMove(e);
			GetMousePositionOnBitmap(e.Location);
			UpdateSizeDisplayBasedOnDrawStatus();
		}

		private void GetMousePositionOnBitmap(Point point)
		{
			if (_shape.IsValidLocation(point))
			{
				var canvaslocation = _shape.GetCanvasRegion();
				lb_Penposition.Text = $"{(int)(point.X / GetCmbscaleSelectedItemKey()) - (int)(canvaslocation.X / GetCmbscaleSelectedItemKey())}, {(int)(point.Y / GetCmbscaleSelectedItemKey()) - (int)(canvaslocation.Y / GetCmbscaleSelectedItemKey())}像素";
			}
			else
			{
				lb_Penposition.Text = "";
			}
		}

		private void UpdateSizeDisplayBasedOnDrawStatus()
		{
			if (_shape.drawStatus == DrawStatus.CanvasAdjusting)
			{
				lb_CanvasSize.Text = $"{(int)(_shape.AdjustingCanvasRect.Width / _shape.Scale)},{(int)(_shape.AdjustingCanvasRect.Height / _shape.Scale)}像素";
			}
			else if (_shape.drawStatus == DrawStatus.Creating ||
				_shape.drawStatus == DrawStatus.Adjusting)
			{
				lb_SelectionSize.Text = $"{(int)(_shape.SelectionRect.Width / _shape.Scale)},{(int)(_shape.SelectionRect.Height / _shape.Scale)}像素";
			}
		}

		private void Panel_MouseWheel(object sender, MouseEventArgs e)
		{
			if (_shape is RectangularSelection rectSelection) rectSelection.Cancel();
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
				CreateNewBitmap();
			}
			if (panel_main.DisplayRectangle.Height > panel_main.ClientSize.Height)
			{
				CreateNewBitmap();
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
			if (_shape.drawStatus == DrawStatus.CompleteCanvasAdjustment)
			{
				OperationStep.PushRevokeStack(_canvas);
				GenerateStretchedBitmap();
			}
			SetRichTextBoxLocation();
			lb_CanvasSize.Text = $"{_canvas.Width},{_canvas.Height}像素";
		}

		private void SetRichTextBoxLocation()
		{
			if (_shape.GetType() != typeof(TextBoxArea)) return;
			if (_shape.drawStatus == DrawStatus.CompleteDrawText ||
				_shape.drawStatus == DrawStatus.CannotMovedOrAdjusted ||
				_shape.drawStatus == DrawStatus.CompleteCanvasAdjustment) return;

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

			//string text = rtb_Text.Text;
			//rtb_Text.Text = text;
			//rtb_Text.SelectionStart = text.Length;
			rtb_Text.Focus();
			SetTextFont();

		}

		private void CreateNewBitmap()
		{
			if (_canvas == null) return;
			Bitmap newCanvas = new Bitmap(_canvas.Width, _canvas.Height);
			//将现在bitmap其绘制到新的bitmap上
			using (Graphics g = Graphics.FromImage(newCanvas))
			{
				g.Clear(_canvasBackgroundColor);
				g.DrawImage(_canvas, Point.Empty);
			}
			_canvas.Dispose();
			_canvas = newCanvas;
			_shape.canvas = _canvas;

			SetRichTextBoxLocation();
			_shape.drawStatus = DrawStatus.CanAdjusted;
			var rect = _shape.GetCanvasRegion();
			panel_main.AutoScrollMinSize = new Size(rect.Width, rect.Height);
			panel_main.Invalidate();
			lb_CanvasSize.Text = $"{_canvas.Width},{_canvas.Height}像素";
		}

		private void GenerateStretchedBitmap()
		{
			if (_shape.AdjustingCanvasRect.Width == 0) return;
			if (_shape.AdjustingCanvasRect.Height == 0) return;
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
					g.DrawImage(_canvas, point);
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
			if (_canvas != null)
			{
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				_shape.InPainting(e.Graphics);
			}
		}

		//直线
		private void btn_Line_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_Line.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<Line>();
			panel_main.Invalidate();
			UpdateSizeItems(nameof(Line), true);
		}

		//橡皮擦
		private void btn_Erase_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_Erase.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<Eraser>();
			panel_main.Invalidate();
			UpdateSizeItems(nameof(Eraser), true);
		}

		//范围选择
		private void btn_select_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_select.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<RectangularSelection>();
			panel_main.Invalidate();
			UpdateSizeItems(nameof(RectangularSelection), false);
		}

		//颜色填充
		private void btn_Fill_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_Fill.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<OilTank>();
			panel_main.Invalidate();
			UpdateSizeItems(nameof(OilTank), false);
		}

		//矩形
		private void btn_rectangle_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_rectangle.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<ShapeRectangle>();
			panel_main.Invalidate();
			UpdateSizeItems(nameof(ShapeRectangle), true);
		}

		//五边形
		private void btn_pentagon_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_pentagon.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<Pentagon>();
			panel_main.Invalidate();
			UpdateSizeItems(nameof(Pentagon), true);
		}

		//圆
		private void btn_circle_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_circle.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<Circle>();
			panel_main.Invalidate();
			UpdateSizeItems(nameof(Circle), true);
		}

		//三角形
		private void btn_triangle_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_triangle.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<Triangle>();
			panel_main.Invalidate();
			UpdateSizeItems(nameof(Triangle), true);
		}

		//直角三角形
		private void btn_RightTriangle_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_RightTriangle.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<RightTriangle>();
			panel_main.Invalidate();
			UpdateSizeItems(nameof(RightTriangle), true);
		}

		//菱形
		private void btn_rhombus_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_rhombus.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<Rhombus>();
			panel_main.Invalidate();
			UpdateSizeItems(nameof(Rhombus), true);
		}
		//六边形
		private void btn_hexagon_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_hexagon.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<Hexagon>();
			panel_main.Invalidate();
			UpdateSizeItems(nameof(Hexagon), true);
		}
		//圆角矩形
		private void btn_roundedRectangle_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_roundedRectangle.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<RoundedRectangle>();
			panel_main.Invalidate();
			UpdateSizeItems(nameof(RoundedRectangle), true);
		}
		//文本
		private void btn_Text_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_Text.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<TextBoxArea>();
			panel_main.Invalidate();
			SetPanelTextStyle();
			UpdateSizeItems(nameof(TextBoxArea), false);
			SetTextFont();
		}
		private void Form1_Resize(object sender, EventArgs e)
		{
			CreateNewBitmap();
			SetPanelTextStyle();
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
			OperationStep.InitStack();
		}

		//释放 Bitmap 资源
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			_canvas.Dispose();
			OperationStep.OnOperationCompleted -= RevokeAndRedoAction;
		}

		private void cmb_size_SelectedIndexChanged(object sender, EventArgs e)
		{
			_shape.Size = float.Parse(cmb_size.Text.Substring(0, cmb_size.Text.Length - 2));
			_shape.drawStatus = DrawStatus.AdjustTheStyle;
			if (_shape is RectangularSelection) return;
			panel_main.Refresh();
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
			lb_CanvasSize.Text = $"{(int)(_canvas.Width / GetCmbscaleSelectedItemKey())},{(int)(_canvas.Height / GetCmbscaleSelectedItemKey())}像素";
			panel_main.AutoScrollMinSize = new Size(_canvas.Width, _canvas.Height);
		}

		private void SavePng()
		{
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
					MessageBox.Show("图像已成功保存!", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
					lb_CanvasSize.Text = $"{_canvas.Width},{_canvas.Height}像素";
					panel_main.Invalidate();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"无法加载图像：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void SetShapeForeColor(Color color)
		{
			_shape.ForeColor = color;
			_shape.drawStatus = DrawStatus.AdjustTheStyle;
			rtb_Text.ForeColor = color;
			if (_shape is RectangularSelection) return;
			if (_shape is TextBoxArea) return;
			panel_main.Refresh();
			panel_main.Refresh();
		}

		private void btn_RightRotate90_Click(object sender, EventArgs e)
		{
			if (_shape is TextBoxArea) return;
			if (_shape.SelectionRect == Rectangle.Empty || _shape.SelectionRect.Width == 0 || _shape.SelectionRect.Height == 0)
			{
				OperationStep.PushRevokeStack(_canvas);
				_shape.CanvasRotateRight();
				CreateNewBitmap();
			}
			else
			{
				_shape.RotateRight();
				panel_main.Refresh();
				panel_main.Refresh();
			}
		}

		private void btn_LeftRotate90_Click(object sender, EventArgs e)
		{
			if (_shape is TextBoxArea) return;
			if (_shape.SelectionRect == Rectangle.Empty || _shape.SelectionRect.Width == 0 || _shape.SelectionRect.Height == 0)
			{
				OperationStep.PushRevokeStack(_canvas);
				_shape.CanvasRotateLeft();
				CreateNewBitmap();
			}
			else
			{
				_shape.RotateLeft();
				panel_main.Refresh();
				panel_main.Refresh();
			}
		}

		private void btn_Rotate180_Click(object sender, EventArgs e)
		{
			if (_shape is TextBoxArea) return;
			if (_shape.SelectionRect == Rectangle.Empty || _shape.SelectionRect.Width == 0 || _shape.SelectionRect.Height == 0)
			{
				OperationStep.PushRevokeStack(_canvas);
				_shape.CanvasRotate180();
				CreateNewBitmap();
			}
			else
			{
				_shape.Rotate180();
				panel_main.Refresh();
				panel_main.Refresh();
			}
		}

		private void btn_FlipVertical_Click(object sender, EventArgs e)
		{
			if (_shape is TextBoxArea) return;
			if (_shape.SelectionRect == Rectangle.Empty || _shape.SelectionRect.Width == 0 || _shape.SelectionRect.Height == 0)
			{
				OperationStep.PushRevokeStack(_canvas);
				_shape.CanvasFlipVertical();
				CreateNewBitmap();
			}
			else
			{
				_shape.FlipVertical();
				panel_main.Refresh();
				panel_main.Refresh();
			}
		}

		private void btn_FlipHorizontal_Click(object sender, EventArgs e)
		{
			if (_shape is TextBoxArea) return;
			if (_shape.SelectionRect == Rectangle.Empty || _shape.SelectionRect.Width == 0 || _shape.SelectionRect.Height == 0)
			{
				OperationStep.PushRevokeStack(_canvas);
				_shape.CanvasFlipHorizontal();
				CreateNewBitmap();
			}
			else
			{
				_shape.FlipHorizontal();
				panel_main.Refresh();
				panel_main.Refresh();
			}
		}

		private void btn_ClearAll_Click(object sender, EventArgs e)
		{
			OperationStep.PushRevokeStack(_canvas);
			_shape.Clear(_canvasBackgroundColor);
		}

		private void cmb_TextSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			//SetTextFont();
			if (_shape is TextBoxArea area && area.SelectionRect.Width > 0 && area.SelectionRect.Height > 0)
			{
				area.SetRichTextBoxMinSize(float.Parse(cmb_TextSize.Text), ref area.SelectionRect);
				SetRichTextBoxLocation();
				_shape.drawStatus = DrawStatus.CanAdjusted;
				panel_main.Refresh();
				rtb_Text.Focus();
			}
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
		/// 缩小
		/// </summary>
		private void pic_reduce_Click(object sender, EventArgs e)
		{
			if (trackBar_scale.Value - _scaleDelta * 100 >= trackBar_scale.Minimum)
			{
				trackBar_scale.Value = trackBar_scale.Value - (int)(_scaleDelta * 100);
				float scale = trackBar_scale.Value / 100f;
				SetCanvasScale(scale);
			}
		}

		/// <summary>
		///  放大
		/// </summary>
		private void pic_amplify_Click(object sender, EventArgs e)
		{
			if (trackBar_scale.Value + _scaleDelta * 100 <= trackBar_scale.Maximum)
			{
				trackBar_scale.Value = trackBar_scale.Value + (int)(_scaleDelta * 100);
				float scale = trackBar_scale.Value / 100f;
				SetCanvasScale(scale);
			}
		}

		private void trackBar_scale_ValueChanged(object sender, EventArgs e)
		{
			SetCanvasScale(trackBar_scale.Value / 100f);
		}
		

		private void panel_main_Scroll(object sender, ScrollEventArgs e)
		{
			if (_shape is RectangularSelection rectSelection) rectSelection.Cancel();
			CreateNewBitmap();
		}

		private void btn_revoke_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			var bitmap = OperationStep.Revoke(_canvas);
			RevokeOrRedo(bitmap);
			panel_main.Invalidate();
		}

		private void RevokeOrRedo(Bitmap bitmap)
		{
			if (bitmap == null) return;
			Bitmap newCanvas = new Bitmap(bitmap.Width, bitmap.Height);
			if (_canvas != null)
			{
				using (Graphics g = Graphics.FromImage(newCanvas))
				{
					g.Clear(_canvasBackgroundColor);
					g.DrawImage(bitmap, Point.Empty);
				}
				_canvas.Dispose();
				_canvas = newCanvas;
				_shape.canvas = _canvas;
			}
			RevokeAndRedoAction();
			lb_CanvasSize.Text = $"{bitmap.Width},{bitmap.Height}像素";
		}

		private void btn_redo_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			var bitmap = OperationStep.Redo(_canvas);
			RevokeOrRedo(bitmap);
			panel_main.Invalidate();
		}

		private void RevokeAndRedoAction()
		{
			btn_revoke.Enabled = OperationStep.AllowRevoke();
			btn_redo.Enabled = OperationStep.AllowRedo();
		}

		private void UpdateSizeItems(string type, bool enable)
		{
			int index = cmb_size.SelectedIndex;
			cmb_size.Items.Clear();
			if (type == nameof(Eraser))
			{
				cmb_size.Items.AddRange(new object[] { "4px", "6px", "8px", "10px" });
			}
			else
			{
				cmb_size.Items.AddRange(new object[] { "1px", "3px", "5px", "8px" });
			}
			cmb_size.SelectedIndex = index;
			cmb_size.Enabled = enable;
			SetPanelTextStyle();
		}

		private void SetPanelTextStyle()
		{
			panel_TextStyle.BringToFront();
			panel_TextStyle.Visible = _shape is TextBoxArea ? true : false;
			var x = (this.Width - panel_TextStyle.Width) / 2;
			var y = toolStrip1.Bottom + 1;
			panel_TextStyle.Location = new Point(x, y);
		}

		private void btn_MakeTransparent_Click(object sender, EventArgs e)
		{
			OperationStep.PushRevokeStack(_canvas);
			_shape.canvas.MakeTransparent();
			_shape.drawStatus = DrawStatus.AdjustTheStyle;
			panel_main.Refresh();
			panel_main.Refresh();
		}

		private void SetTextFont()
		{
			int fontSize = int.Parse(cmb_TextSize.Text);
			string fontFamily = cmb_FontFamily.Text;
			FontStyle newFontStyle = FontStyle.Regular;

			if (pic_Blod.BorderStyle == BorderStyle.FixedSingle)
			{
				newFontStyle |= FontStyle.Bold;
			}
			if (pic_Italic.BorderStyle == BorderStyle.FixedSingle)
			{
				newFontStyle |= FontStyle.Italic;
			}
			if (pic_underline.BorderStyle == BorderStyle.FixedSingle)
			{
				newFontStyle |= FontStyle.Underline;
			}
			if (pic_strikethrough.BorderStyle == BorderStyle.FixedSingle)
			{
				newFontStyle |= FontStyle.Strikeout;
			}

			if (pic_left.BorderStyle == BorderStyle.FixedSingle)
			{
				rtb_Text.SelectionAlignment = HorizontalAlignment.Left;
			}
			else if (pic_center.BorderStyle == BorderStyle.FixedSingle)
			{
				rtb_Text.SelectionAlignment = HorizontalAlignment.Center;
			}
			else if (pic_right.BorderStyle == BorderStyle.FixedSingle)
			{
				rtb_Text.SelectionAlignment = HorizontalAlignment.Right;
			}
			Font font = new Font(fontFamily, fontSize * _shape.Scale, newFontStyle);
			rtb_Text.Font = font;
			if (_shape is TextBoxArea text) text.FontSize = fontSize;
		}
		private void pic_Blod_Click(object sender, EventArgs e)
		{
			pic_Blod.BorderStyle = pic_Blod.BorderStyle == BorderStyle.None ? BorderStyle.FixedSingle : BorderStyle.None;
			SetTextFont();
		}
		private void pic_Italic_Click(object sender, EventArgs e)
		{
			pic_Italic.BorderStyle = pic_Italic.BorderStyle == BorderStyle.None ? BorderStyle.FixedSingle : BorderStyle.None;
			SetTextFont();
		}

		private void pic_underline_Click(object sender, EventArgs e)
		{
			pic_underline.BorderStyle = pic_underline.BorderStyle == BorderStyle.None ? BorderStyle.FixedSingle : BorderStyle.None;
			SetTextFont();
		}

		private void pic_strikethrough_Click(object sender, EventArgs e)
		{
			pic_strikethrough.BorderStyle = pic_strikethrough.BorderStyle == BorderStyle.None ? BorderStyle.FixedSingle : BorderStyle.None;
			SetTextFont();
		}

		private void pic_left_Click(object sender, EventArgs e)
		{
			pic_left.BorderStyle = BorderStyle.FixedSingle;
			pic_center.BorderStyle = BorderStyle.None;
			pic_right.BorderStyle = BorderStyle.None;
			SetTextFont();
		}

		private void pic_center_Click(object sender, EventArgs e)
		{
			pic_center.BorderStyle = BorderStyle.FixedSingle;
			pic_left.BorderStyle = BorderStyle.None;
			pic_right.BorderStyle = BorderStyle.None;
			SetTextFont();
		}

		private void pic_right_Click(object sender, EventArgs e)
		{
			pic_right.BorderStyle = BorderStyle.FixedSingle;
			pic_center.BorderStyle = BorderStyle.None;
			pic_left.BorderStyle = BorderStyle.None;
			SetTextFont();
		}

		private void cmb_scales_SelectedIndexChanged(object sender, EventArgs e)
		{
			//if (_shape is null) return;
			//var selectedKeyValuePair = (KeyValuePair<float, string>)cmb_scales.SelectedItem;
			//if (_shape.SelectionRect.Width != 0 && _shape.SelectionRect.Height != 0)
			//{
			//	_shape.drawStatus = DrawStatus.CanAdjusted;
			//}
			//trackBar_scale.Value = (int)Math.Round(selectedKeyValuePair.Key * 100);
			//_shape.Scale = selectedKeyValuePair.Key;
			//SetScaleDelta(selectedKeyValuePair.Key);
			//panel_main.Invalidate();
			//var rect = _shape.GetCanvasRegion();
			//panel_main.AutoScrollMinSize = new Size(rect.Width, rect.Height);

			var selectedKeyValuePair = (KeyValuePair<float, string>)cmb_scales.SelectedItem;
			RefreshCanvasScale(selectedKeyValuePair.Key, (int)Math.Round(selectedKeyValuePair.Key * 100));
		}

		private void SetScaleDelta(float scale)
		{
			if (scale >= 0.125f && scale <= 2)
			{
				_scaleDelta = 0.1f;
			}
			else if (scale > 2 && scale <= 6)
			{
				_scaleDelta = 0.25f;
			}
			else if (scale > 6 && scale <= 8)
			{
				_scaleDelta = 0.5f;
			}
		}

		private float GetCmbscaleSelectedItemKey()
		{
			//var selectedKeyValuePair = (KeyValuePair<float, string>)cmb_scales.SelectedItem;
			//return selectedKeyValuePair.Key;

			string strScale = cmb_scales.Text.Substring(0, cmb_scales.Text.Length - 1);
			return (float.Parse(strScale) / 100f);
		}

		private void InitCmbScaleItems()
		{
			Dictionary<float, string> dataSource = new Dictionary<float, string>();
			foreach (var scale in _scales)
			{
				dataSource.Add(scale, $"{scale * 100}%");
			}
			var bindingList = new List<KeyValuePair<float, string>>(dataSource);

			cmb_scales.DataSource = bindingList;
			cmb_scales.DisplayMember = "Value";
			cmb_scales.ValueMember = "Key";

			cmb_scales.SelectedIndex = 4;
		}
		//适应窗口大小
		private void pic_FitToWindow_Click(object sender, EventArgs e)
		{
			AdjustCanvasToFit();
		}
		private void AdjustCanvasToFit()
		{
			int clientWidth = panel_main.Width;
			int clientHeight = panel_main.Height;

			int canvasWidth = _canvas.Width + 50;
			int canvasHeight = _canvas.Height + 30;

			float scaleX = (float)clientWidth / canvasWidth;
			float scaleY = (float)clientHeight / canvasHeight;

			float minValue = Math.Min(scaleX, scaleY);
			float scale = (float)Math.Round(minValue, 1);

			if (scale > 8f) scale = 8f;
			if (scale < 0.125f) scale = 0.125f;

			trackBar_scale.Value = (int)(scale * 100);
			SetCanvasScale(scale);
		}

		private void btn_Pencil_Click(object sender, EventArgs e)
		{
			SetShapeBtnBackColor();
			btn_Pencil.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<Pencil>();
			panel_main.Invalidate();
			UpdateSizeItems(nameof(Pencil), true);
		}

		private void btn_Color_Click(object sender, EventArgs e)
		{
			ColorDialog colorDialog = new ColorDialog();
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				btn_Color.BackColor = colorDialog.Color;
				SetShapeForeColor(btn_Color.BackColor);
			}
		}

		private void SetShapeBtnBackColor()
		{
			btn_select.BackColor = Color.Transparent;
			btn_Pencil.BackColor = Color.Transparent;
			btn_Erase.BackColor = Color.Transparent;
			btn_Fill.BackColor = Color.Transparent;
			btn_Text.BackColor = Color.Transparent;
			btn_Line.BackColor = Color.Transparent;
			btn_circle.BackColor = Color.Transparent;
			btn_rectangle.BackColor = Color.Transparent;
			btn_roundedRectangle.BackColor = Color.Transparent;
			btn_triangle.BackColor = Color.Transparent;
			btn_RightTriangle.BackColor = Color.Transparent;
			btn_rhombus.BackColor = Color.Transparent;
			btn_pentagon.BackColor = Color.Transparent;
			btn_hexagon.BackColor = Color.Transparent;
		}

		private void btn_screenShot_Click(object sender, EventArgs e)
		{
			OpenCaptureForm();
		}

		private void CanvasForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && _shape is RectangularSelection)
			{
				var rectSelection = _shape as RectangularSelection;
				rectSelection.DelSelectedBitmap();
			}
		}
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x0312 && m.WParam.ToInt32() == HOTKEY_ID)
			{
				OpenCaptureForm();
			}
			base.WndProc(ref m);
		}

		private void OpenCaptureForm()
		{
			if (_captureForm == null || _captureForm.IsDisposed)
			{
				_captureForm = new CaptureForm();
				_captureForm.ShowDialog();
			}
			else
			{ 
				_captureForm.Focus();
			}
		}
	
		private void SetCanvasScale(float scale)
		{
			if (_shape.SelectionRect.Width != 0 && _shape.SelectionRect.Height != 0)
			{
				_shape.drawStatus = DrawStatus.CanAdjusted;
			}
			_shape.Scale = scale;
			cmb_scales.Text = $"{scale * 100}%";
			toolTip1.SetToolTip(trackBar_scale, $"{scale * 100}");
			panel_main.Invalidate();
			var rect = _shape.GetCanvasRegion();
			panel_main.AutoScrollMinSize = new Size(rect.Width, rect.Height);
		}

		private void cmb_scales_MouseDown(object sender, MouseEventArgs e)
		{
			_cmbScaleLastText = cmb_scales.Text;
		}

		private void cmb_scales_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				e.SuppressKeyPress = true;
				PerformValidation(cmb_scales);
				panel_main.Focus();
			}
		}

		private void cmb_scales_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			PerformValidation(cmb_scales);
		}

		private void cmb_scales_Leave(object sender, EventArgs e)
		{
			PerformValidation(cmb_scales);
		}

		private bool IsValidDecimal(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return false;

			text = text.Trim();

			if (text.Contains('.'))
			{
				string[] parts = text.Split('.');
				if (parts.Length != 2)
					return false;

				string integerPart = parts[0];
				string decimalPart = parts[1];

				if (string.IsNullOrEmpty(integerPart) || string.IsNullOrEmpty(decimalPart))
					return false;

				if (decimalPart.Length > 1)
					return false;

				foreach (char c in integerPart)
				{
					if (!char.IsDigit(c))
						return false;
				}

				foreach (char c in decimalPart)
				{
					if (!char.IsDigit(c))
						return false;
				}
			}
			else
			{
				foreach (char c in text)
				{
					if (!char.IsDigit(c))
						return false;
				}
			}

			return true;
		}

		private bool ValidateInput(string input, out float resultValue)
		{
			resultValue = 0f;

			if (string.IsNullOrWhiteSpace(input))
				return false;

			input = input.Trim();

			bool endsWithPercent = input.EndsWith("%");
			string numberPart = endsWithPercent ? input.Substring(0, input.Length - 1) : input;

			if (!IsValidDecimal(numberPart))
				return false;

			if (!float.TryParse(numberPart, out float numericValue))
				return false;

			if (numericValue < 10 || numericValue > 800)
				return false;

			// 应用自定义舍入规则
			resultValue = CustomRound(numericValue);
			return true;
		}

		private float CustomRound(float value)
		{
			//int integerPart = (int)value;
			//float decimalPart = value - integerPart;

			int integerPart = (int)value;
			float decimalPart = value - integerPart;
			decimalPart = (float)Math.Round(decimalPart, 1); // 四舍五入到 1 位小数

			if (decimalPart <= 0.2f)
			{
				return integerPart; // 舍去小数
			}
			else if (decimalPart >= 0.8f)
			{
				return integerPart + 1; // 向上取整
			}
			else
			{
				return integerPart + 0.5f; // 变成 .5
			}
		}

		private void PerformValidation(ComboBox comboBox)
		{
			string input = comboBox.Text;

			if (ValidateInput(input, out float validValue))
			{
				comboBox.Text = validValue.ToString() + "%";
				float trackBarScaleValue = (float)Math.Ceiling(validValue);
				this.trackBar_scale.ValueChanged -= new System.EventHandler(this.trackBar_scale_ValueChanged);
				RefreshCanvasScale(validValue / 100f, (int)trackBarScaleValue);
				toolTip1.SetToolTip(trackBar_scale, $"{trackBarScaleValue}");
				this.trackBar_scale.ValueChanged += new System.EventHandler(this.trackBar_scale_ValueChanged);
			}
			else
			{
				comboBox.Text = _cmbScaleLastText;
			}
		}

		private void RefreshCanvasScale(float scale, int trackBarScaleValue)
		{
			if (_shape is null) return;
			if (_shape.SelectionRect.Width != 0 && _shape.SelectionRect.Height != 0)
			{
				_shape.drawStatus = DrawStatus.CanAdjusted;
			}
			trackBar_scale.Value = trackBarScaleValue;
			_shape.Scale = scale;
			SetScaleDelta(scale);
			panel_main.Invalidate();
			var rect = _shape.GetCanvasRegion();
			panel_main.AutoScrollMinSize = new Size(rect.Width, rect.Height);
		}


	}

	public class MouseMessageFilter : IMessageFilter
	{
		private readonly ComboBox _comboBox;
		private readonly Form _form;

		public MouseMessageFilter(ComboBox comboBox, Form form)
		{
			_comboBox = comboBox;
			_form = form;
		}

		public bool PreFilterMessage(ref Message m)
		{
			const int WM_LBUTTONDOWN = 0x0201;

			if (m.Msg == WM_LBUTTONDOWN)
			{
				// 获取鼠标当前屏幕坐标
				Point cursorPos = Cursor.Position;

				// 获取 ComboBox 的屏幕区域
				Rectangle comboBoxRect = _comboBox.RectangleToScreen(_comboBox.ClientRectangle);

				// 如果点击不在 ComboBox 区域内，则让它失去焦点
				if (!comboBoxRect.Contains(cursorPos))
				{
					_form.ActiveControl = null; // 清除焦点
				}
			}

			return false; // 返回 false 表示继续传递消息
		}
	}
}
