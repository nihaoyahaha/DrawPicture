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
	{
		private Bitmap _selectedBitmap;
		private Rectangle _rectBeforeAdjust;
		private Rectangle _fillRect = Rectangle.Empty;
		private Color _FillRectColor;
		
		private float _angle=0;
		public RectangularSelection(Bitmap bitmap, Panel panel) : base(bitmap, panel)
		{
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
			if (drawStatus == DrawStatus.Creating || drawStatus == DrawStatus.Moving || drawStatus == DrawStatus.Adjusting)
			{
				CancelDrawing();
				return;
			}
			else if (drawStatus == DrawStatus.CannotMovedOrAdjusted && SelectionRect != Rectangle.Empty)
			{
				BitmapDrawImage();
			}
		}

		private void CancelDrawing()
		{
			drawStatus = DrawStatus.CannotMovedOrAdjusted;
			_selectedBitmap = null;
			SelectionRect = Rectangle.Empty;
			_fillRect = Rectangle.Empty;
			panel.Invalidate();
		}

		private void MouseLeftButtonDownHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.CannotMovedOrAdjusted)
			{
				BitmapDrawImage();
				StartPoint = e.Location;
				drawStatus = DrawStatus.Creating;
				_FillRectColor = ForeColor;
			}
			else if (drawStatus == DrawStatus.CanMove)
			{
				Offset = e.Location;
				drawStatus = DrawStatus.Moving;
				_rectBeforeAdjust = new Rectangle(SelectionRect.X, SelectionRect.Y, SelectionRect.Width, SelectionRect.Height);
			}
			else if (drawStatus == DrawStatus.CanAdjusted)
			{
				Offset = e.Location;
				drawStatus = DrawStatus.Adjusting;
				_rectBeforeAdjust = new Rectangle(SelectionRect.X, SelectionRect.Y, SelectionRect.Width, SelectionRect.Height);
			}
		}

		private void BitmapDrawImage()
		{
			if (_selectedBitmap == null) return;
			if (_fillRect.Equals(SelectionRect))
			{
				CancelDrawing();
				return;
			}
			using (Graphics g = Graphics.FromImage(canvas))
			{
				g.FillRectangle(new SolidBrush(_FillRectColor), _fillRect);
				g.DrawImage(_selectedBitmap, SelectionRect);
			}
			drawStatus = DrawStatus.CannotMovedOrAdjusted;
			_selectedBitmap = null;
			SelectionRect = Rectangle.Empty;
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
				MouseOverResizeHandle(e.Location);
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
				SelectionRect = new Rectangle(x, y, width, height);
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Moving)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionRect.Offset(deltaX, deltaY);
				Offset = e.Location;
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Adjusting)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionAdjusting(deltaX, deltaY);
				Offset = e.Location;
				panel.Invalidate();
			}
		}

		public override void MouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				MouseLeftButtonUpHandel(e);
			}

		}

		private void MouseLeftButtonUpHandel(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.Creating)
			{
				drawStatus = DrawStatus.CanAdjusted;
				if (SelectionRect.Width > 0 && SelectionRect.Height > 0)
				{
					_selectedBitmap = new Bitmap(SelectionRect.Width, SelectionRect.Height);
					using (Graphics g = Graphics.FromImage(_selectedBitmap))
					{
						g.DrawImage(canvas,
									new Rectangle(0, 0, SelectionRect.Width, SelectionRect.Height),
									SelectionRect,
									GraphicsUnit.Pixel);
					}
					_fillRect = new Rectangle(SelectionRect.X, SelectionRect.Y, SelectionRect.Width, SelectionRect.Height);
				}
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Moving)
			{
				drawStatus = DrawStatus.CanMove;
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Adjusting)
			{
				drawStatus = DrawStatus.CompleteAdjustment;
				panel.Invalidate();
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
				if (SelectionRect.Width == 0 || SelectionRect.Height == 0) return;
				DrawCreating(graphics);
			}
			else if (drawStatus == DrawStatus.Moving || drawStatus == DrawStatus.CanMove || drawStatus == DrawStatus.CanAdjusted)
			{
				DrawCanMoveOrAdjusted(graphics);
			}
			else if (drawStatus == DrawStatus.Adjusting)
			{
				DrawAdjusting(graphics);
			}
			else if (drawStatus == DrawStatus.CompleteAdjustment)
			{
				DrawAdjustComplate(graphics);
			}
		}

		private void DrawCreating(Graphics graphics)
		{
			using (Pen selectionPen = new Pen(Color.Black, 0.5f))
			{
				selectionPen.DashStyle = DashStyle.Dash;
				selectionPen.DashPattern = new float[] { 5.0f, 4.0f };// 划线长，间隔长
				graphics.DrawRectangle(selectionPen, SelectionRect);
			}
		}

		private void DrawCanMoveOrAdjusted(Graphics graphics)
		{
			if (_selectedBitmap == null) return;
			graphics.FillRectangle(new SolidBrush(_FillRectColor), _fillRect);
			graphics.DrawImage(_selectedBitmap, SelectionRect);
			// 保存当前绘图状态
			//GraphicsState state = graphics.Save();

			//try
			//{
			//	// 设置旋转中心（_selectionRect 的中心点）
			//	float centerX = _selectionRect.X + _selectionRect.Width / 2f;
			//	float centerY = _selectionRect.Y + _selectionRect.Height / 2f;

			//	// 平移到旋转中心
			//	graphics.TranslateTransform(centerX, centerY);

			//	// 旋转 90 度（顺时针）
			//	graphics.RotateTransform(_angle);

			//	// 平移回原点
			//	//graphics.TranslateTransform(-centerX, -centerY);

			//	// 绘制图像
			//	graphics.DrawImage(_selectedBitmap,Point.Empty);
			//	graphics.ResetTransform();
			//}
			//finally
			//{
			//	// 恢复原始状态
			//	//graphics.Restore(state);
			//}


			using (Pen selectionPen = new Pen(ResizerPointColor, 0.5f))
			{
				selectionPen.DashStyle = DashStyle.Dash;
				selectionPen.DashPattern = new float[] { 5.0f, 4.0f };// 划线长，间隔长
				graphics.DrawRectangle(selectionPen, SelectionRect);
			}
			foreach (var item in GetResizerPoints(SelectionRect))
			{
				graphics.FillEllipse(new SolidBrush(ResizerPointColor), new Rectangle(
					item.editPoint.X - (int)ResizerPointSize / 2,
					item.editPoint.Y - (int)ResizerPointSize / 2,
					(int)ResizerPointSize, 
					(int)ResizerPointSize));
			}
		}

		private void DrawAdjusting(Graphics graphics)
		{
			graphics.FillRectangle(new SolidBrush(_FillRectColor), _fillRect);
			graphics.DrawImage(_selectedBitmap, _rectBeforeAdjust);
			using (Pen selectionPen = new Pen(ResizerPointColor, 0.5f))
			{
				selectionPen.DashStyle = DashStyle.Dash;
				selectionPen.DashPattern = new float[] { 5.0f, 4.0f };// 划线长，间隔长
				graphics.DrawRectangle(selectionPen, _rectBeforeAdjust);

				selectionPen.Color = Color.Black;
				selectionPen.DashPattern = new float[] { 1.0f, 1.0f };// 划线长，间隔长
				graphics.DrawRectangle(selectionPen, SelectionRect);
			}
			foreach (var item in GetResizerPoints(_rectBeforeAdjust))
			{
				graphics.FillEllipse(new SolidBrush(ResizerPointColor), new Rectangle(
					item.editPoint.X - (int)ResizerPointSize / 2,
					item.editPoint.Y - (int)ResizerPointSize / 2,
					(int)ResizerPointSize,
					(int)ResizerPointSize));
			}
		}

		private void DrawAdjustComplate(Graphics graphics)
		{
			graphics.FillRectangle(new SolidBrush(_FillRectColor), _fillRect);
			graphics.DrawImage(_selectedBitmap, SelectionRect);
			using (Pen selectionPen = new Pen(ResizerPointColor, 0.5f))
			{
				selectionPen.DashStyle = DashStyle.Dash;
				selectionPen.DashPattern = new float[] { 5.0f, 4.0f };// 划线长，间隔长
				graphics.DrawRectangle(selectionPen, SelectionRect);
			}
			foreach (var item in GetResizerPoints(SelectionRect))
			{
				graphics.FillEllipse(new SolidBrush(ResizerPointColor), new Rectangle(
					item.editPoint.X - (int)ResizerPointSize / 2,
					item.editPoint.Y - (int)ResizerPointSize / 2,
					(int)ResizerPointSize,
					(int)ResizerPointSize));
			}
		}

		public override void Rotate(float angle)
		{
			drawStatus = DrawStatus.CanAdjusted;
			var center = GetCenter();
			var matrix = new Matrix();
			matrix.RotateAt(angle, center);
			ApplyTransform(matrix);
			
		}
		public PointF GetCenter()
		{
			return new PointF(SelectionRect.X + SelectionRect.Width / 2f, SelectionRect.Y + SelectionRect.Height / 2f);
		}
		// 应用变换并更新边界
		public void ApplyTransform(Matrix transform)
		{
			var points = GetPoints();
			transform.TransformPoints(points);

			// 更新边界为变换后顶点的包围盒
			float minX = points.Min(p => p.X);
			float maxX = points.Max(p => p.X);
			float minY = points.Min(p => p.Y);
			float maxY = points.Max(p => p.Y);

			SelectionRect = Rectangle.FromLTRB((int)Math.Round(minX), (int)Math.Round(minY),
										(int)Math.Round(maxX), (int)Math.Round(maxY));
		}

		// 获取矩形的四个顶点
		public PointF[] GetPoints()
		{
			return new PointF[]
			{
			new PointF(SelectionRect.Left, SelectionRect.Top),
			new PointF(SelectionRect.Right, SelectionRect.Top),
			new PointF(SelectionRect.Right, SelectionRect.Bottom),
			new PointF(SelectionRect.Left, SelectionRect.Bottom)
			};
		}


		public override void FlipHorizontal()
		{
			
		}

		public override void FlipVertical()
		{
			
		}

		public override void Clear(Color color)
		{
			ClearBitmap(color);
		}
	}
}
