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
using System.ComponentModel;
using System.Linq;

namespace DrawKit
{
	public partial class CanvasForm : Form
	{
		[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
		private const int HOTKEY_ID = 1;
		private const int _canvasRightMargin = 20;
		private const int _canvasBottomMargin = 20;
		private Bitmap _canvas;
		private Shape _shape;
		private Color _canvasBackgroundColor = Color.White;
		private float[] _scales = { 0.125f, 0.25f, 0.5f, 0.75f, 1, 2, 3, 4, 5, 6, 7, 8 };
		private float _scaleDelta = 0.1f;
		private CaptureForm _captureForm;
		private string _cmbScaleLastText = "";
		private Point? _scrollPosition = null;

		public CanvasForm()
		{
			InitializeComponent();
			this.WindowState = FormWindowState.Maximized;
			InitControls();
			panel_main.MouseWheel += panel_main_MouseWheel;
			OperationStep.OnOperationCompleted += RevokeAndRedoAction;
			_shape = new Pencil(_canvas, this.panel_main, GetCmbscaleSelectedItemKey());
		}

		private void InitControls()
		{
			InitCmbScaleItems();
			InitBackColor();
			InitializeCanvas();
			LoadInstalledFonts();
		}

		//初始化可选择的缩放比例
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

		//画面背景色初始化
		private void InitBackColor()
		{
			BackColor = Color.FromArgb(247, 249, 254);
			panel_TextStyle.BackColor = Color.FromArgb(247, 249, 254);
			panel_main.BackColor = Color.FromArgb(247, 249, 254);
			toolStrip1.BackColor = Color.FromArgb(204, 213, 240);
			panel_Bottom.BackColor = Color.FromArgb(64, 80, 141);
		}

		//画布初始化
		private void InitializeCanvas()
		{
			_canvas = new Bitmap(1500, 700);
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
			SetPanelAutoScrollMinSize(_canvas.Width, _canvas.Height);
		}

		//初始画字体
		private void LoadInstalledFonts()
		{
			string[] commonFonts = new string[]
			{
				"微软雅黑", "Microsoft YaHei",
				"宋体", "SimSun",
				"黑体", "SimHei",
				"楷体", "KaiTi",
				"隶书", "LiSu",
				"幼圆", "YouYuan",
				"Arial",
				"Times New Roman",
				"Verdana",
				"Segoe UI" };

			InstalledFontCollection installedFonts = new InstalledFontCollection();
			foreach (string fontName in commonFonts)
			{
				if (Array.Exists(installedFonts.Families, f => f.Name == fontName))
				{
					cmb_FontFamily.Items.Add(fontName);
				}
			}
		}

		private void CanvasForm_Load(object sender, EventArgs e)
		{
			cmb_size.SelectedIndex = 0;
			cmb_TextSize.SelectedIndex = 0;
			cmb_FontFamily.SelectedIndex = 0;
			SetCanvasScale(GetCmbscaleSelectedItemKey());
			OperationStep.InitStack();
			if (!RegisterHotKey(this.Handle, HOTKEY_ID, (uint)0x0001, (uint)Keys.B))
			{
				MessageBox.Show("无法注册热键，请重试。");
			}
		}

		//释放 Bitmap 资源
		private void CanvasForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			_canvas?.Dispose();
			_canvas = null;
			OperationStep.OnOperationCompleted -= RevokeAndRedoAction;
		}

		private void CanvasForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (_shape is TextBoxArea && _shape.SelectionRect != Rectangle.Empty) return;
			if (!(_shape is RectangularSelection) && (e.Control) && (e.KeyCode == Keys.V))
			{
				SwitchToShapeTool<RectangularSelection>(btn_select, nameof(RectangularSelection), false);
			}
			_shape.KeyDown(e);
		}

		private void CanvasForm_Resize(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			CreateNewBitmap();
			SetPanelTextStyle();
		}

		//获取鼠标在画布上的位置
		private void GetMousePositionOnBitmap(Point point)
		{
			if (_shape.IsValidLocation(point))
			{
				var canvaslocation = _shape.GetCanvasRegion();
				lb_Penposition.Text = $"{(int)(point.X / _shape.Scale) - (int)(canvaslocation.X / _shape.Scale)}, {(int)(point.Y / _shape.Scale) - (int)(canvaslocation.Y / _shape.Scale)}像素";
			}
			else
			{
				lb_Penposition.Text = "";
			}
		}

		//画布的尺寸和编辑状态下图形的尺寸
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

		private void panel_main_MouseClick(object sender, MouseEventArgs e)
		{
			panel_main.Focus();
		}

		private void panel_main_Scroll(object sender, ScrollEventArgs e)
		{
			_shape.CommitCurrentShape();
			CanvasScroll();

		}

		private void CanvasScroll()
		{
			if (_canvas == null) return;
			Bitmap newCanvas = new Bitmap(_canvas.Width, _canvas.Height);
			using (Graphics g = Graphics.FromImage(newCanvas))
			{
				g.Clear(_canvasBackgroundColor);
				g.DrawImage(_canvas, Point.Empty);
			}
			_canvas?.Dispose();
			_canvas = null;
			_canvas = newCanvas;
			_shape.canvas?.Dispose();
			_shape.canvas = null;
			_shape.canvas = _canvas;
			panel_main.Invalidate();
			_scrollPosition = panel_main.AutoScrollPosition;
		}

		private void panel_main_MouseWheel(object sender, MouseEventArgs e)
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
			else
			{
				if (panel_main.DisplayRectangle.Width > panel_main.ClientSize.Width)
				{
					CanvasScroll();
				}
				if (panel_main.DisplayRectangle.Height > panel_main.ClientSize.Height)
				{
					CanvasScroll();
				}
			}
		}

		private void panel_main_MouseMove(object sender, MouseEventArgs e)
		{
			_shape.MouseMove(e);
			GetMousePositionOnBitmap(e.Location);
			UpdateSizeDisplayBasedOnDrawStatus();
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

		private void panel_main_Paint(object sender, PaintEventArgs e)
		{
			if (_canvas != null)
			{
				e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				_shape.InPainting(e.Graphics);
			}
		}

		//设置透明文本框位置
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
			rtb_Text.Focus();
			SetTextFont();

		}

		//更新画布
		private void CreateNewBitmap()
		{
			if (_canvas == null) return;
			Bitmap newCanvas = new Bitmap(_canvas.Width, _canvas.Height);
			using (Graphics g = Graphics.FromImage(newCanvas))
			{
				g.Clear(_canvasBackgroundColor);
				g.DrawImage(_canvas, Point.Empty);
			}
			_canvas?.Dispose();
			_canvas = null;
			_canvas = newCanvas;
			_shape.canvas?.Dispose();
			_shape.canvas = null;
			_shape.canvas = _canvas;

			SetRichTextBoxLocation();
			_shape.drawStatus = DrawStatus.CanAdjusted;
			var rect = _shape.GetCanvasRegion();
			SetPanelAutoScrollMinSize(rect.Width, rect.Height);
			panel_main.Invalidate();
			lb_CanvasSize.Text = $"{_canvas.Width},{_canvas.Height}像素";
		}

		//画布尺寸调整
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
					var offsetPoint = _shape.BitmapStretchOffsetPoint;
					Point point = new Point((int)(offsetPoint.X / _shape.Scale), (int)(offsetPoint.Y / _shape.Scale));
					g.DrawImage(_canvas, point);
				}
				_canvas?.Dispose();
				_canvas = null;
				_canvas = newCanvas;
				_shape.canvas?.Dispose();
				_shape.canvas = null;
				_shape.canvas = _canvas;
			}
			panel_main.Invalidate();
			var rect = _shape.GetCanvasRegion();
			SetPanelAutoScrollMinSize(rect.Width, rect.Height);
		}

		//范围选择
		private void btn_select_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<RectangularSelection>(btn_select, nameof(RectangularSelection), false);
		}

		//向右旋转90度
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

		//向左旋转90度
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

		//旋转180度
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

		//垂直翻转
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

		//水平翻转
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

		//画笔
		private void btn_Pencil_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<Pencil>(btn_Pencil, nameof(Pencil), true);
		}

		//橡皮擦
		private void btn_Erase_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<Eraser>(btn_Erase, nameof(Eraser), true);
		}

		//颜色填充
		private void btn_Fill_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<OilTank>(btn_Fill, nameof(OilTank), false);
		}

		//选择粗细
		private void cmb_size_SelectedIndexChanged(object sender, EventArgs e)
		{
			_shape.Size = float.Parse(cmb_size.Text.Substring(0, cmb_size.Text.Length - 2));
			_shape.drawStatus = DrawStatus.AdjustTheStyle;
			if (_shape is RectangularSelection) return;
			panel_main.Refresh();
			panel_main.Refresh();
		}

		//文本
		private void btn_Text_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<TextBoxArea>(btn_Text, nameof(TextBoxArea), false);
			SetTextFont();
		}

		//直线
		private void btn_Line_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<Line>(btn_Line, nameof(Line), true);
		}

		//圆
		private void btn_circle_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<Circle>(btn_circle, nameof(Circle), true);
		}

		//矩形
		private void btn_rectangle_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<ShapeRectangle>(btn_rectangle, nameof(ShapeRectangle), true);
		}

		//圆角矩形
		private void btn_roundedRectangle_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<RoundedRectangle>(btn_roundedRectangle, nameof(RoundedRectangle), true);
		}

		//三角形
		private void btn_triangle_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<Triangle>(btn_triangle, nameof(Triangle), true);
		}

		//直角三角形
		private void btn_RightTriangle_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<RightTriangle>(btn_RightTriangle, nameof(RightTriangle), true);
		}

		//菱形
		private void btn_rhombus_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<Rhombus>(btn_rhombus, nameof(Rhombus), true);
		}

		//五边形
		private void btn_pentagon_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<Pentagon>(btn_pentagon, nameof(Pentagon), true);
		}

		//六边形
		private void btn_hexagon_Click(object sender, EventArgs e)
		{
			SwitchToShapeTool<Hexagon>(btn_hexagon, nameof(Hexagon), true);
		}

		//选择颜色
		private void btn_Color_Click(object sender, EventArgs e)
		{
			ColorDialog colorDialog = new ColorDialog();
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				btn_Color.BackColor = colorDialog.Color;
				SetShapeForeColor(btn_Color.BackColor);
			}
		}

		//保存
		private void btn_save_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			SavePng();
		}

		//打开
		private void btn_open_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			OpenPng();
			OperationStep.InitStack();
		}

		//清空
		private void btn_ClearAll_Click(object sender, EventArgs e)
		{
			OperationStep.PushRevokeStack(_canvas);
			_shape.Clear(_canvasBackgroundColor);
		}

		//撤销
		private void btn_revoke_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			var bitmap = OperationStep.Revoke(_canvas);
			RevokeOrRedo(bitmap);
			panel_main.Invalidate();
		}

		//重做
		private void btn_redo_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			var bitmap = OperationStep.Redo(_canvas);
			RevokeOrRedo(bitmap);
			panel_main.Invalidate();
		}

		//背景透明
		private void btn_MakeTransparent_Click(object sender, EventArgs e)
		{
			OperationStep.PushRevokeStack(_canvas);
			_shape.canvas.MakeTransparent();
			_canvasBackgroundColor = Color.Transparent;
			_shape.drawStatus = DrawStatus.AdjustTheStyle;
			panel_main.Refresh();
			panel_main.Refresh();
		}

		//截图
		private void btn_screenShot_Click(object sender, EventArgs e)
		{
			OpenCaptureForm();
		}

		//适应窗口大小
		private void pic_FitToWindow_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			AdjustCanvasToFit();
		}

		private void panel_Bottom_MouseClick(object sender, MouseEventArgs e)
		{
			panel_main.Focus();
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

		private void cmb_scales_Leave(object sender, EventArgs e)
		{
			PerformValidation(cmb_scales);
		}

		private void cmb_scales_MouseDown(object sender, MouseEventArgs e)
		{
			_shape.CommitCurrentShape();
			_cmbScaleLastText = cmb_scales.Text;
		}

		private void cmb_scales_Validating(object sender, CancelEventArgs e)
		{
			PerformValidation(cmb_scales);
		}

		private void cmb_scales_SelectedIndexChanged(object sender, EventArgs e)
		{
			var selectedKeyValuePair = (KeyValuePair<float, string>)cmb_scales.SelectedItem;
			RefreshCanvasScale(selectedKeyValuePair.Key, (int)Math.Round(selectedKeyValuePair.Key * 100));
		}

		//保存图片
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

		//打开图片
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
						_canvas?.Dispose();
						_canvas = null;
						_canvas = new Bitmap(tempBitmap);
					}
					_shape.canvas?.Dispose();
					_shape.canvas = null;
					_shape.canvas = _canvas;
					lb_CanvasSize.Text = $"{_canvas.Width},{_canvas.Height}像素";

					this.cmb_scales.SelectedIndexChanged -= new System.EventHandler(this.cmb_scales_SelectedIndexChanged);
					cmb_scales.SelectedIndex = 4;
					this.cmb_scales.SelectedIndexChanged += new System.EventHandler(this.cmb_scales_SelectedIndexChanged);
					RefreshCanvasScale(1, 100);
					panel_main.Invalidate();

				}
				catch (Exception ex)
				{
					MessageBox.Show($"无法加载图像：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		//设置图形前景色
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

		//缩小
		private void pic_reduce_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			if (trackBar_scale.Value - _scaleDelta * 100 >= trackBar_scale.Minimum)
			{
				this.trackBar_scale.ValueChanged -= new System.EventHandler(this.trackBar_scale_ValueChanged);
				trackBar_scale.Value = trackBar_scale.Value - (int)(_scaleDelta * 100);
				this.trackBar_scale.ValueChanged += new System.EventHandler(this.trackBar_scale_ValueChanged);
				float scale = trackBar_scale.Value / 100f;
				SetCanvasScale(scale);
			}
		}

		//放大
		private void pic_amplify_Click(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			if (trackBar_scale.Value + _scaleDelta * 100 <= trackBar_scale.Maximum)
			{
				this.trackBar_scale.ValueChanged -= new System.EventHandler(this.trackBar_scale_ValueChanged);
				trackBar_scale.Value = trackBar_scale.Value + (int)(_scaleDelta * 100);
				this.trackBar_scale.ValueChanged += new System.EventHandler(this.trackBar_scale_ValueChanged);
				float scale = trackBar_scale.Value / 100f;
				SetCanvasScale(scale);
			}
		}

		//移到滑块改变缩放比例
		private void trackBar_scale_ValueChanged(object sender, EventArgs e)
		{
			_shape.CommitCurrentShape();
			SetCanvasScale(trackBar_scale.Value / 100f);
		}

		//设置画布缩放比例
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
			SetPanelAutoScrollMinSize(rect.Width, rect.Height);
		}

		//根据撤销、重做操作更新画面位图
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
				_canvas?.Dispose();
				_canvas = null;
				_canvas = newCanvas;
				_shape.canvas?.Dispose();
				_shape.canvas = null;
				_shape.canvas = _canvas;
			}
			RevokeAndRedoAction();
			lb_CanvasSize.Text = $"{bitmap.Width},{bitmap.Height}像素";
		}

		//更新撤销和重做按钮的启用状态
		private void RevokeAndRedoAction()
		{
			btn_revoke.Enabled = OperationStep.AllowRevoke();
			btn_redo.Enabled = OperationStep.AllowRedo();
		}

		//根据工具类型设置尺寸下拉框的选项和可用状态
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

		//显示或隐藏文本样式面板，并将其居中显示在顶部工具栏下方
		private void SetPanelTextStyle()
		{
			panel_TextStyle.BringToFront();
			panel_TextStyle.Visible = _shape is TextBoxArea ? true : false;
			var x = (this.Width - panel_TextStyle.Width) / 2;
			var y = toolStrip1.Bottom + 1;
			panel_TextStyle.Location = new Point(x, y);
		}

		//设置文本样式
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

		//选择字体大小
		private void cmb_TextSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_shape is TextBoxArea area && area.SelectionRect.Width > 0 && area.SelectionRect.Height > 0)
			{
				area.SetRichTextBoxMinSize(float.Parse(cmb_TextSize.Text), ref area.SelectionRect);
				SetRichTextBoxLocation();
				_shape.drawStatus = DrawStatus.CanAdjusted;
				panel_main.Refresh();
				rtb_Text.Focus();
			}
		}

		//选择字体类型
		private void cmb_FontFamily_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetTextFont();
		}

		//加粗
		private void pic_Blod_Click(object sender, EventArgs e)
		{
			pic_Blod.BorderStyle = pic_Blod.BorderStyle == BorderStyle.None ? BorderStyle.FixedSingle : BorderStyle.None;
			SetTextFont();
		}

		//斜体
		private void pic_Italic_Click(object sender, EventArgs e)
		{
			pic_Italic.BorderStyle = pic_Italic.BorderStyle == BorderStyle.None ? BorderStyle.FixedSingle : BorderStyle.None;
			SetTextFont();
		}

		//下划线
		private void pic_underline_Click(object sender, EventArgs e)
		{
			pic_underline.BorderStyle = pic_underline.BorderStyle == BorderStyle.None ? BorderStyle.FixedSingle : BorderStyle.None;
			SetTextFont();
		}

		//删除线
		private void pic_strikethrough_Click(object sender, EventArgs e)
		{
			pic_strikethrough.BorderStyle = pic_strikethrough.BorderStyle == BorderStyle.None ? BorderStyle.FixedSingle : BorderStyle.None;
			SetTextFont();
		}

		//靠左
		private void pic_left_Click(object sender, EventArgs e)
		{
			pic_left.BorderStyle = BorderStyle.FixedSingle;
			pic_center.BorderStyle = BorderStyle.None;
			pic_right.BorderStyle = BorderStyle.None;
			SetTextFont();
		}

		//居中
		private void pic_center_Click(object sender, EventArgs e)
		{
			pic_center.BorderStyle = BorderStyle.FixedSingle;
			pic_left.BorderStyle = BorderStyle.None;
			pic_right.BorderStyle = BorderStyle.None;
			SetTextFont();
		}

		//靠右
		private void pic_right_Click(object sender, EventArgs e)
		{
			pic_right.BorderStyle = BorderStyle.FixedSingle;
			pic_center.BorderStyle = BorderStyle.None;
			pic_left.BorderStyle = BorderStyle.None;
			SetTextFont();
		}

		//设置面板自动滚动的最小尺寸
		private void SetPanelAutoScrollMinSize(int width, int height)
		{
			int horizontalMargin = (width - panel_main.Width) / 2;
			int verticalMargin = (height - panel_main.Height) / 2;

			if (horizontalMargin < 0 && verticalMargin < 0) _scrollPosition = null;

			panel_main.AutoScrollMinSize = new Size(width + _canvasRightMargin, height + _canvasBottomMargin);

			Point pt = _scrollPosition == null ? new Point(horizontalMargin, verticalMargin) : new Point(Math.Abs(_scrollPosition.Value.X), Math.Abs(_scrollPosition.Value.Y));
			panel_main.AutoScrollPosition = pt;
		}

		//根据缩放比例设置缩放步长
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

		//获取当前缩放比例
		private float GetCmbscaleSelectedItemKey()
		{
			string strScale = cmb_scales.Text.Substring(0, cmb_scales.Text.Length - 1);
			return (float.Parse(strScale) / 100f);
		}

		//画布自适应窗体尺寸
		private void AdjustCanvasToFit()
		{
			_scrollPosition = null;
			int clientWidth = panel_main.Width;
			int clientHeight = panel_main.Height;

			int canvasWidth = _canvas.Width + 50;
			int canvasHeight = _canvas.Height + 30;

			float scaleX = (float)clientWidth / canvasWidth;
			float scaleY = (float)clientHeight / canvasHeight;

			float minValue = Math.Min(scaleX, scaleY);
			float scale = (float)Math.Round(minValue, 2);

			if (scale > 8f) scale = 8f;
			if (scale < 0.125f) scale = 0.125f;
			this.trackBar_scale.ValueChanged -= new System.EventHandler(this.trackBar_scale_ValueChanged);
			trackBar_scale.Value = (int)(scale * 100);
			this.trackBar_scale.ValueChanged += new System.EventHandler(this.trackBar_scale_ValueChanged);
			SetCanvasScale(scale);
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

		//监听全局热键，触发后打开截图界面
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x0312 && m.WParam.ToInt32() == HOTKEY_ID)
			{
				OpenCaptureForm();
			}
			base.WndProc(ref m);
		}

		//打开截图窗体
		private void OpenCaptureForm()
		{
			if (_captureForm == null || _captureForm.IsDisposed)
			{
				_captureForm = new CaptureForm();
				_captureForm.Show(this);
			}
			else
			{
				_captureForm.Focus();
			}
		}

		//选择画图工具
		private void SwitchToShapeTool<T>(ToolStripButton highlightButton, string shapeName, bool cmbSizeEnable) where T : Shape, new()
		{
			SetShapeBtnBackColor();
			highlightButton.BackColor = Color.FromArgb(245, 204, 132);
			_shape.CommitCurrentShape();
			_shape = _shape.InitializeShape<T>();
			panel_main.Invalidate();
			UpdateSizeItems(shapeName, cmbSizeEnable);
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
			int integerPart = (int)value;
			float decimalPart = value - integerPart;
			decimalPart = (float)Math.Round(decimalPart, 1);

			if (decimalPart <= 0.2f)
			{
				return integerPart;
			}
			else if (decimalPart >= 0.8f)
			{
				return integerPart + 1;
			}
			else
			{
				return integerPart + 0.5f;
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
			this.trackBar_scale.ValueChanged -= new System.EventHandler(this.trackBar_scale_ValueChanged);
			trackBar_scale.Value = trackBarScaleValue;
			this.trackBar_scale.ValueChanged += new System.EventHandler(this.trackBar_scale_ValueChanged);
			_shape.Scale = scale;
			SetScaleDelta(scale);
			panel_main.Invalidate();
			var rect = _shape.GetCanvasRegion();
			SetPanelAutoScrollMinSize(rect.Width, rect.Height);
		}

		private void rtb_Text_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.V)
			{
				e.SuppressKeyPress = true;

				// 获取剪贴板中的纯文本
				if (Clipboard.ContainsText())
				{
					string text = Clipboard.GetText();
					int selectionStart = rtb_Text.SelectionStart;
					rtb_Text.SelectedText = text; // 将纯文本插入到当前位置
				}
			}
		}
	}
}
