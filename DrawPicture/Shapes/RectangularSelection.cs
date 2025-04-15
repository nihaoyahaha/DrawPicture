using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawPicture.Shapes
{
	/// <summary>
	/// 矩形選択
	/// </summary>
	public class RectangularSelection : Shape
	{//104, 139, 204
		private Point _offset = new Point();
		private Bitmap _selectedBitmap;
		private Rectangle _selectionRect = Rectangle.Empty;
		private Rectangle _fillRect = Rectangle.Empty;
		private Color _selectedRectForeColor = Color.FromArgb(104, 139, 204);
		public RectangularSelection(Bitmap bitmap, Panel panel) : base(bitmap, panel)
		{
			Size = 10;
		}

		public override void MouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				MouseLeftButtonDownHandle(e);
			}
			else if (e.Button == MouseButtons.Right)
			{
				MouseRightButtonDownHandle(e);
			}
		}

		private void MouseRightButtonDownHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.Creating || drawStatus == DrawStatus.CanMove)
			{
				drawStatus = DrawStatus.CannotMovedOrAdjusted;
				_selectedBitmap = null;
				_selectionRect = Rectangle.Empty;
				_fillRect = Rectangle.Empty;
				panel.Invalidate();
				return;
			}
		}


		private void MouseLeftButtonDownHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.CannotMovedOrAdjusted)
			{
				BitmapDrawImage();
				StartPoint = e.Location;
				drawStatus = DrawStatus.Creating;
			}
			else if (drawStatus == DrawStatus.CanMove)
			{
				_offset = e.Location;
			}

		}

		private void BitmapDrawImage()
		{
			if (_selectedBitmap == null) return;
			using (Graphics g = Graphics.FromImage(canvas))
			{
				g.DrawImage(_selectedBitmap, _selectionRect);
				g.FillRectangle(new SolidBrush(ForeColor), _fillRect);
			}
			drawStatus = DrawStatus.CannotMovedOrAdjusted;
			_selectedBitmap = null;
			_selectionRect = Rectangle.Empty;
			_fillRect = Rectangle.Empty;
			panel.Invalidate();
		}


		public override void MouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				MouseMoveLeftButtonHandle(e);
			}
			else if (e.Button == MouseButtons.None)
			{
				drawStatus = DrawStatus.CannotMovedOrAdjusted;
				panel.Cursor = Cursors.Default;
				if (StartPoint.X == 0 && StartPoint.Y == 0) return;
				if (EndPoint.X == 0 && EndPoint.Y == 0) return;
				MouseMoveNoneButtonHandel(e.Location);
			}

		}
		private void MouseMoveLeftButtonHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.Creating)
			{
				EndPoint = e.Location;
				int x = Math.Min(StartPoint.X, EndPoint.X);
				int y = Math.Min(StartPoint.Y, EndPoint.Y);
				int width = Math.Abs(StartPoint.X - EndPoint.X);
				int height = Math.Abs(StartPoint.Y - EndPoint.Y);
				_selectionRect = new Rectangle(x, y, width, height);
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.CanMove)
			{
				int deltaX = e.X - _offset.X;
				int deltaY = e.Y - _offset.Y;
				_selectionRect.Offset(deltaX, deltaY);
				_offset = e.Location;
				panel.Invalidate();
			}
		}

		private void MouseMoveNoneButtonHandel(Point mouseLocation)
		{
			if (_selectionRect.Contains(mouseLocation))
			{
				drawStatus = DrawStatus.CanMove;
				panel.Cursor = Cursors.SizeAll;

			}
			foreach (var focusPoint in GetPointCollection())
			{
				double distance = Math.Sqrt(Math.Pow(mouseLocation.X - focusPoint.editPoint.X, 2) + Math.Pow(mouseLocation.Y - focusPoint.editPoint.Y, 2));
				if (distance <= Size)
				{
					SetFoucsCursorType(focusPoint.focusType);
					drawStatus = DrawStatus.CanAdjusted;
				}
			}

		}

		private void SetFoucsCursorType(RectangleShapeFocusType focusType)
		{
			switch (focusType)
			{
				case RectangleShapeFocusType.Unfocused:
					panel.Cursor = default;
					break;
				case RectangleShapeFocusType.TopLeft:
					panel.Cursor = Cursors.SizeNWSE;
					break;
				case RectangleShapeFocusType.TopCenter:
					panel.Cursor = Cursors.SizeNS;
					break;
				case RectangleShapeFocusType.TopRight:
					panel.Cursor = Cursors.SizeNESW;
					break;
				case RectangleShapeFocusType.MiddleLeft:
					panel.Cursor = Cursors.SizeWE;
					break;
				case RectangleShapeFocusType.MiddleRight:
					panel.Cursor = Cursors.SizeWE;
					break;
				case RectangleShapeFocusType.BottomLeft:
					panel.Cursor = Cursors.SizeNESW;
					break;
				case RectangleShapeFocusType.BottomCenter:
					panel.Cursor = Cursors.SizeNS;
					break;
				case RectangleShapeFocusType.BottomRight:
					panel.Cursor = Cursors.SizeNWSE;
					break;
				case RectangleShapeFocusType.Move:
					panel.Cursor = Cursors.SizeAll;
					break;
				default:
					break;
			}

		}

		public override void MouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				MouseLeftButtonUpHandel(e);
			}
			panel.Invalidate();
		}

		private void MouseLeftButtonUpHandel(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.Creating)
			{
				drawStatus = DrawStatus.CanAdjusted;
				if (_selectionRect.Width > 0 && _selectionRect.Height > 0)
				{
					_selectedBitmap = new Bitmap(_selectionRect.Width, _selectionRect.Height);
					using (Graphics g = Graphics.FromImage(_selectedBitmap))
					{
						g.DrawImage(canvas,
									new Rectangle(0, 0, _selectionRect.Width, _selectionRect.Height),
									_selectionRect,
									GraphicsUnit.Pixel);
					}
					_fillRect = new Rectangle(_selectionRect.X, _selectionRect.Y, _selectionRect.Width, _selectionRect.Height);
				}
			}
		}

		public override void InPainting(Graphics graphics)
		{
			if (canvas != null)
			{
				graphics.DrawImage(canvas, 0, 0);
			}

			if (drawStatus == DrawStatus.Creating)
			{
				if (_selectionRect.Width == 0 || _selectionRect.Height == 0) return;
				DrawCreating(graphics);
			}
			else if (drawStatus == DrawStatus.CanMove || drawStatus == DrawStatus.CanAdjusted)
			{
				DrawCanAdjust(graphics);
			}
		}

		private void DrawCreating(Graphics graphics)
		{
			using (Pen selectionPen = new Pen(Color.Black, 0.5f))
			{
				selectionPen.DashStyle = DashStyle.Dash;
				selectionPen.DashPattern = new float[] { 5.0f, 4.0f };// 划线长，间隔长
				graphics.DrawRectangle(selectionPen, _selectionRect);
			}
		}

		private void DrawCanAdjust(Graphics graphics)
		{
			if (_selectedBitmap == null) return;
			graphics.FillRectangle(new SolidBrush(ForeColor), _fillRect);
			graphics.DrawImage(_selectedBitmap, _selectionRect.Location);
			using (Pen selectionPen = new Pen(_selectedRectForeColor, 0.5f))
			{
				selectionPen.DashStyle = DashStyle.Dash;
				selectionPen.DashPattern = new float[] { 5.0f, 4.0f };// 划线长，间隔长
				graphics.DrawRectangle(selectionPen, _selectionRect);
			}
			foreach (var item in GetPointCollection())
			{
				//using (Pen selectionPen = new Pen(Color.Black, 0.5f))
				//{
				//	selectionPen.DashStyle = DashStyle.Solid;
				//	graphics.DrawEllipse(selectionPen, new Rectangle(
				//		item.editPoint.X - (int)Size / 2,
				//		item.editPoint.Y - (int)Size / 2,
				//		(int)Size, (int)Size
				//		));
				//}
				graphics.FillEllipse(new SolidBrush(_selectedRectForeColor), new Rectangle(
					item.editPoint.X - (int)Size / 2,
					item.editPoint.Y - (int)Size / 2,
					(int)Size, (int)Size
						));
			}
		}

		/// <summary>
		/// 矩形的可编辑点坐标和位置
		/// </summary>
		/// <returns></returns>
		private IEnumerable<(Point editPoint, RectangleShapeFocusType focusType)> GetPointCollection()
		{
			yield return (new Point(_selectionRect.X, _selectionRect.Y), RectangleShapeFocusType.TopLeft);
			yield return (new Point(_selectionRect.X + _selectionRect.Width / 2, _selectionRect.Y), RectangleShapeFocusType.TopCenter);
			yield return (new Point(_selectionRect.X + _selectionRect.Width, _selectionRect.Y), RectangleShapeFocusType.TopRight);

			yield return (new Point(_selectionRect.X, _selectionRect.Y + _selectionRect.Height / 2), RectangleShapeFocusType.MiddleLeft);
			yield return (new Point(_selectionRect.X + _selectionRect.Width, _selectionRect.Y + _selectionRect.Height / 2), RectangleShapeFocusType.MiddleRight);

			yield return (new Point(_selectionRect.X, _selectionRect.Y + _selectionRect.Height), RectangleShapeFocusType.BottomLeft);
			yield return (new Point(_selectionRect.X + _selectionRect.Width / 2, _selectionRect.Y + _selectionRect.Height), RectangleShapeFocusType.BottomCenter);
			yield return (new Point(_selectionRect.X + _selectionRect.Width, _selectionRect.Y + _selectionRect.Height), RectangleShapeFocusType.BottomRight);
		}

	}
}
