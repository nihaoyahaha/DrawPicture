using DrawKit.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawKit.Screenshot
{
	public partial class CaptureForm : Form
	{
		private Point _startPoint;
		private Point _endPoint;
		private Bitmap _screenBitmap;
		private Bitmap _selectedArea;
		private Color _overlayColor = Color.FromArgb(128, 77, 77, 77);
		private Color _editPointColor = Color.FromArgb(255, 7, 193, 96);
		private float _editPointSize = 5;
		private Point _offset = new Point();
		private Rectangle _selectionRect = Rectangle.Empty;
		private DrawStatus _drawStatus = DrawStatus.CannotMovedOrAdjusted;
		private List<(Rectangle rect, RectangleShapeFocusType focusType)> _canvasEditPoints = new List<(Rectangle rect, RectangleShapeFocusType focusType)>();
		private RectangleShapeFocusType _focusType;
		public CaptureForm()
		{
			InitializeComponent();
			TopMost = true;
			this.FormBorderStyle = FormBorderStyle.None;
			this.Bounds = Screen.PrimaryScreen.Bounds;
			this.DoubleBuffered = true;
			this.BackgroundImage = CaptureScreen();
		}

		private Bitmap CaptureScreen()
		{
			Rectangle bounds = Screen.GetBounds(this);
			_screenBitmap = new Bitmap(bounds.Width, bounds.Height);
			using (Graphics g = Graphics.FromImage(_screenBitmap))
			{
				g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
			}
			return _screenBitmap;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				MouseLeftButtonDownHandle(e);
			}
			else if (e.Button == MouseButtons.Right)
			{
				CloseForm();
			}
			base.OnMouseDown(e);
		}

		private void MouseLeftButtonDownHandle(MouseEventArgs e)
		{
			if (_drawStatus == DrawStatus.CannotMovedOrAdjusted)
			{
				if (_selectionRect != Rectangle.Empty) return;
				_startPoint = e.Location;
				_drawStatus = DrawStatus.Creating;
			}
			else if (_drawStatus == DrawStatus.CanMove)
			{
				panel_operation.Visible = false;
				_offset = e.Location;
				_drawStatus = DrawStatus.Moving;
			}
			else if (_drawStatus == DrawStatus.CanAdjusted)
			{
				panel_operation.Visible = false;
				_offset = e.Location;
				_drawStatus = DrawStatus.Adjusting;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				MouseMoveLeftButtonHandle(e);
			}
			else if (e.Button == MouseButtons.None)
			{
				_drawStatus = DrawStatus.CannotMovedOrAdjusted;
				this.Cursor = Cursors.Default;
				MouseOverResizeHandle(e.Location);
			}
			base.OnMouseMove(e);
		}

		private void MouseMoveLeftButtonHandle(MouseEventArgs e)
		{
			if (_drawStatus == DrawStatus.Creating)
			{
				_endPoint = e.Location;
				int x = Math.Min(_startPoint.X, _endPoint.X);
				int y = Math.Min(_startPoint.Y, _endPoint.Y);
				int width = Math.Abs(_startPoint.X - _endPoint.X);
				int height = Math.Abs(_startPoint.Y - _endPoint.Y);
				_selectionRect = new Rectangle(x, y, width, height);
				this.Invalidate();
			}
			else if (_drawStatus == DrawStatus.Moving)
			{
				int deltaX = e.X - _offset.X;
				int deltaY = e.Y - _offset.Y;
				_selectionRect.Offset(deltaX, deltaY);
				if (_selectionRect.X <= 0) _selectionRect.X = 0;
				if (_selectionRect.Y <= 0) _selectionRect.Y = 0;
				if (_selectionRect.Right >= _screenBitmap.Width) _selectionRect.X = _screenBitmap.Width - _selectionRect.Width;
				if (_selectionRect.Bottom >= _screenBitmap.Height) _selectionRect.Y = _screenBitmap.Height - _selectionRect.Height;
				_offset = e.Location;
				this.Invalidate();
			}
			else if (_drawStatus == DrawStatus.Adjusting)
			{
				int deltaX = e.X - _offset.X;
				int deltaY = e.Y - _offset.Y;
				SelectionAdjusting(deltaX, deltaY, ref _selectionRect);
				_offset = e.Location;
				this.Invalidate();
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				MouseLeftButtonUpHandel(e);
				SetOperationPanelLocation();
			}
			base.OnMouseUp(e);
		}

		private void MouseLeftButtonUpHandel(MouseEventArgs e)
		{
			if (_drawStatus == DrawStatus.Creating)
			{
				_drawStatus = DrawStatus.CanAdjusted;
				this.Invalidate();
			}
			else if (_drawStatus == DrawStatus.Moving)
			{
				_drawStatus = DrawStatus.CanMove;
				this.Invalidate();
			}
			else if (_drawStatus == DrawStatus.Adjusting)
			{
				_drawStatus = DrawStatus.CompleteAdjustment;
				RecalculateScope();
				this.Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			using (Brush overlayBrush = new SolidBrush(_overlayColor))
			{
				e.Graphics.FillRectangle(overlayBrush, this.ClientRectangle);
			}

			if (_drawStatus == DrawStatus.Creating)
			{
				if (_selectionRect.Width == 0 || _selectionRect.Height == 0) return;
				DrawCreating(e.Graphics);
			}
			else if (_drawStatus == DrawStatus.Moving ||
				_drawStatus == DrawStatus.CanMove ||
				_drawStatus == DrawStatus.CanAdjusted ||
				_drawStatus == DrawStatus.Adjusting ||
				_drawStatus == DrawStatus.CompleteAdjustment)
			{
				if (_selectionRect.Width == 0 || _selectionRect.Height == 0) return;
				DrawCanMoveOrAdjusted(e.Graphics);
			}

		}

		private void DrawSelectionRectSize(Graphics graphics, Point location)
		{
			graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			graphics.DrawString($"{Math.Abs(_selectionRect.Width)} x {Math.Abs(_selectionRect.Height)}", new Font("Segoe UI", 9, FontStyle.Bold), new SolidBrush(Color.White), location);
		}

		private void DrawCreating(Graphics graphics)
		{
			using (Pen pen = new Pen(Color.FromArgb(255, 7, 193, 96), 1))
			{
				_selectedArea = new Bitmap(_selectionRect.Width, _selectionRect.Height);
				using (Graphics g = Graphics.FromImage(_selectedArea))
				{
					g.DrawImage(_screenBitmap, new Rectangle(0, 0, _selectionRect.Width, _selectionRect.Height), _selectionRect, GraphicsUnit.Pixel);
				}
				graphics.DrawImage(_selectedArea, _selectionRect);

				graphics.DrawRectangle(pen, new Rectangle(_selectionRect.X, _selectionRect.Y, _selectionRect.Width, _selectionRect.Height));

				int y = _selectionRect.Y - 22 <= 0 ? _selectionRect.Y + 2 : _selectionRect.Y - 22;
				DrawSelectionRectSize(graphics, new Point(_selectionRect.X, y));
			}
		}

		private void DrawCanMoveOrAdjusted(Graphics graphics)
		{
			int x;
			int y;
			int width;
			int height;
			int textY;
			if (_selectionRect.Width < 0 && _selectionRect.Height < 0)
			{
				x = _selectionRect.X + _selectionRect.Width;
				y = _selectionRect.Y + _selectionRect.Height;
				width = Math.Abs(_selectionRect.Width);
				height = Math.Abs(_selectionRect.Height);
			}
			else if (_selectionRect.Width < 0 && _selectionRect.Height > 0)
			{
				x = _selectionRect.X + _selectionRect.Width;
				y = _selectionRect.Y;
				width = Math.Abs(_selectionRect.Width);
				height = _selectionRect.Height;
			}
			else if (_selectionRect.Width > 0 && _selectionRect.Height < 0)
			{
				x = _selectionRect.X;
				y = _selectionRect.Y + _selectionRect.Height;
				width = _selectionRect.Width;
				height = Math.Abs(_selectionRect.Height);
			}
			else
			{
				x = _selectionRect.X;
				y = _selectionRect.Y;
				width = _selectionRect.Width;
				height = _selectionRect.Height;
			}
			_selectedArea = new Bitmap(width, height);
			var rect = new Rectangle(x, y, width, height);
			using (Graphics g = Graphics.FromImage(_selectedArea))
			{
				g.DrawImage(_screenBitmap, new Rectangle(0, 0, width, height), rect, GraphicsUnit.Pixel);
			}
			graphics.DrawImage(_selectedArea, rect);

			using (Pen pen = new Pen(Color.FromArgb(255, 7, 193, 96), 1))
			{
				graphics.DrawRectangle(pen, new Rectangle(x, y, width, height));
			}
			textY = y - 22 <= 0 ? y + 2 : y - 22;
			DrawSelectionRectSize(graphics, new Point(x, textY));

			foreach (var item in GetEditPoints(_selectionRect))
			{
				graphics.FillRectangle(new SolidBrush(_editPointColor), new Rectangle(
					item.editPoint.X - (int)_editPointSize / 2,
					item.editPoint.Y - (int)_editPointSize / 2,
					(int)_editPointSize,
					(int)_editPointSize));
			}
		}

		private void pic_cancel_Click(object sender, EventArgs e)
		{
			CloseForm();
		}

		private void CloseForm()
		{
			Close();
			this.Dispose();
		}

		private void pic_OK_Click(object sender, EventArgs e)
		{
			CopyScreenshotToClipboard();
		}

		private void CopyScreenshotToClipboard()
		{
			if (_selectedArea == null) return;
			Clipboard.SetImage(_selectedArea);
			CloseForm();
		}

		private void MouseOverResizeHandle(Point mouseLocation)
		{
			foreach (var focusPoint in _canvasEditPoints)
			{
				if (focusPoint.rect.Contains(mouseLocation))
				{
					SetFoucsCursorType(focusPoint.focusType);
					_drawStatus = DrawStatus.CanvasAdjustable;
					_focusType = focusPoint.focusType;
					return;
				}
			}
			if (_selectionRect.Contains(mouseLocation))
			{
				_drawStatus = DrawStatus.CanMove;
				this.Cursor = Cursors.SizeAll;
			}
			foreach (var focusPoint in GetEditPoints(_selectionRect))
			{
				double distance = Math.Sqrt(Math.Pow(mouseLocation.X - focusPoint.editPoint.X, 2) + Math.Pow(mouseLocation.Y - focusPoint.editPoint.Y, 2));
				if (distance <= 10)
				{
					SetFoucsCursorType(focusPoint.focusType);
					_drawStatus = DrawStatus.CanAdjusted;
					_focusType = focusPoint.focusType;
				}
			}
		}

		private void SelectionAdjusting(int horizontalDistance, int verticalDistance, ref Rectangle rect)
		{
			int width = rect.Width;
			int height = rect.Height;
			switch (_focusType)
			{
				case RectangleShapeFocusType.TopLeft:
					rect.X += horizontalDistance;
					rect.Y += verticalDistance;
					rect.Width -= horizontalDistance;
					rect.Height -= verticalDistance;

					break;
				case RectangleShapeFocusType.TopCenter:
					rect.Y += verticalDistance;
					rect.Height -= verticalDistance;

					break;
				case RectangleShapeFocusType.TopRight:
					rect.Y += verticalDistance;
					rect.Width += horizontalDistance;
					rect.Height -= verticalDistance;

					break;
				case RectangleShapeFocusType.MiddleLeft:
					rect.X += horizontalDistance;
					rect.Width -= horizontalDistance;

					break;
				case RectangleShapeFocusType.MiddleRight:
					rect.Width += horizontalDistance;

					break;
				case RectangleShapeFocusType.BottomLeft:
					rect.X += horizontalDistance;
					rect.Width -= horizontalDistance;
					rect.Height += verticalDistance;

					break;
				case RectangleShapeFocusType.BottomCenter:
					rect.Height += verticalDistance;

					break;
				case RectangleShapeFocusType.BottomRight:
					rect.Width += horizontalDistance;
					rect.Height += verticalDistance;

					break;
			}
		}

		private IEnumerable<(Point editPoint, RectangleShapeFocusType focusType)> GetEditPoints(Rectangle rect)
		{
			yield return (new Point(rect.X, rect.Y), RectangleShapeFocusType.TopLeft);
			yield return (new Point(rect.X + rect.Width / 2, rect.Y), RectangleShapeFocusType.TopCenter);
			yield return (new Point(rect.X + rect.Width, rect.Y), RectangleShapeFocusType.TopRight);

			yield return (new Point(rect.X, rect.Y + rect.Height / 2), RectangleShapeFocusType.MiddleLeft);
			yield return (new Point(rect.X + rect.Width, rect.Y + rect.Height / 2), RectangleShapeFocusType.MiddleRight);

			yield return (new Point(rect.X, rect.Y + rect.Height), RectangleShapeFocusType.BottomLeft);
			yield return (new Point(rect.X + rect.Width / 2, rect.Y + rect.Height), RectangleShapeFocusType.BottomCenter);
			yield return (new Point(rect.X + rect.Width, rect.Y + rect.Height), RectangleShapeFocusType.BottomRight);
		}

		private void SetFoucsCursorType(RectangleShapeFocusType focusType)
		{
			switch (focusType)
			{
				case RectangleShapeFocusType.Unfocused:
					this.Cursor = default;
					break;
				case RectangleShapeFocusType.TopLeft:
					this.Cursor = Cursors.SizeNWSE;
					break;
				case RectangleShapeFocusType.TopCenter:
					this.Cursor = Cursors.SizeNS;
					break;
				case RectangleShapeFocusType.TopRight:
					this.Cursor = Cursors.SizeNESW;
					break;
				case RectangleShapeFocusType.MiddleLeft:
					this.Cursor = Cursors.SizeWE;
					break;
				case RectangleShapeFocusType.MiddleRight:
					this.Cursor = Cursors.SizeWE;
					break;
				case RectangleShapeFocusType.BottomLeft:
					this.Cursor = Cursors.SizeNESW;
					break;
				case RectangleShapeFocusType.BottomCenter:
					this.Cursor = Cursors.SizeNS;
					break;
				case RectangleShapeFocusType.BottomRight:
					this.Cursor = Cursors.SizeNWSE;
					break;
				case RectangleShapeFocusType.Move:
					this.Cursor = Cursors.SizeAll;
					break;
				default:
					break;
			}
		}

		private void RecalculateScope()
		{
			int x;
			int y;
			int width;
			int height;
			if (_selectionRect.Width < 0 && _selectionRect.Height < 0)
			{
				x = _selectionRect.X + _selectionRect.Width;
				y = _selectionRect.Y + _selectionRect.Height;
				width = Math.Abs(_selectionRect.Width);
				height = Math.Abs(_selectionRect.Height);
				_selectionRect = new Rectangle(x, y, width, height);
			}
			else if (_selectionRect.Width < 0 && _selectionRect.Height > 0)
			{
				x = _selectionRect.X + _selectionRect.Width;
				y = _selectionRect.Y;
				width = Math.Abs(_selectionRect.Width);
				height = _selectionRect.Height;
				_selectionRect = new Rectangle(x, y, width, height);
			}
			else if (_selectionRect.Width > 0 && _selectionRect.Height < 0)
			{
				x = _selectionRect.X;
				y = _selectionRect.Y + _selectionRect.Height;
				width = _selectionRect.Width;
				height = Math.Abs(_selectionRect.Height);
				_selectionRect = new Rectangle(x, y, width, height);
			}
		}

		private void SetOperationPanelLocation()
		{
			if (_selectionRect == Rectangle.Empty) return;
			int x = _selectionRect.Right - panel_operation.Width;
			int y = _selectionRect.Bottom + 5 + panel_operation.Height >= _screenBitmap.Height ? _selectionRect.Bottom - 5 - panel_operation.Height : _selectionRect.Bottom + 5;
			
			panel_operation.Location = new Point(x, y);
			panel_operation.Visible = true;
		}

		private void pic_Save_Click(object sender, EventArgs e)
		{
			SavePng();
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
					_selectedArea.Save(saveFileDialog.FileName, ImageFormat.Png);
					CloseForm();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void CaptureForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				CloseForm();
			}
		}
	}
}
