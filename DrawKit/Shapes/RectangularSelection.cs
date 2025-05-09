using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawKit.Shapes
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
		
		public RectangularSelection(Bitmap bitmap, Panel panel) : base(bitmap, panel){}
		private void BitmapDrawImage()
		{
			if (_selectedBitmap == null) return;
			if (SelectionRect == Rectangle.Empty) return;
			//if (_fillRect.Equals(SelectionRect))
			//{
			//	CancelDrawing();
			//	return;
			//}
			using (Graphics g = Graphics.FromImage(canvas))
			{
				//g.FillRectangle(new SolidBrush(_FillRectColor), _fillRect);
				//g.DrawImage(_selectedBitmap, SelectionRect);
				g.FillRectangle(new SolidBrush(_FillRectColor), ConvertSelectionRectToCanvasRect( _fillRect));
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;
				g.DrawImage(_selectedBitmap, ConvertSelectionRectToCanvasRect( SelectionRect));
			}
			drawStatus = DrawStatus.CannotMovedOrAdjusted;
			_selectedBitmap = null;
			SelectionRect = Rectangle.Empty;
			_fillRect = Rectangle.Empty;
			panel.Invalidate();
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
			if (drawStatus == DrawStatus.Creating || 
				drawStatus == DrawStatus.Moving || 
				drawStatus == DrawStatus.Adjusting ||
				drawStatus == DrawStatus.CanvasAdjusting)
			{
				CancelDrawing();
				return;
			}
			else if (drawStatus == DrawStatus.CannotMovedOrAdjusted && SelectionRect != Rectangle.Empty)
			{
				BitmapDrawImage();
			}
		}

		protected override void  CancelDrawing()
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
			else if (drawStatus == DrawStatus.CanvasAdjustable)
			{
				BitmapDrawImage();
				AdjustingCanvasRect = GetCanvasRegion();
				Offset = e.Location;
				drawStatus = DrawStatus.CanvasAdjusting;
			}
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
				//if (StartPoint.X == 0 && StartPoint.Y == 0) return;
				//if (EndPoint.X == 0 && EndPoint.Y == 0) return;
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
				SelectionAdjusting(deltaX, deltaY, ref SelectionRect);
				Offset = e.Location;
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionAdjusting(deltaX, deltaY, ref AdjustingCanvasRect);
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
									ConvertSelectionRectToCanvasRect( SelectionRect),
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
			else if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				drawStatus = DrawStatus.CompleteCanvasAdjustment;
				panel.Invalidate();
			}
		}

		public override void InPainting(Graphics graphics)
		{
			if (canvas != null)
			{
				BitmapDrawShape(canvas,graphics);
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
			else if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				DrawCanvasAdjusted(graphics);
			}
		}

		private void DrawCreating(Graphics graphics)
		{
			using (Pen selectionPen = new Pen(Color.Black, 0.5f))
			{
				selectionPen.DashStyle = DashStyle.Dash;
				selectionPen.DashPattern = new float[] { 5.0f, 4.0f };// 划线长，间隔长
				Rectangle bitmapArea = GetCanvasRegion();
				graphics.SetClip(bitmapArea);
				graphics.DrawRectangle(selectionPen, SelectionRect);
				graphics.ResetClip();
			}
		}

		private void DrawCanMoveOrAdjusted(Graphics graphics)
		{
			if (_selectedBitmap == null) return;
			Rectangle bitmapArea = GetCanvasRegion();
			graphics.SetClip(bitmapArea);
			graphics.FillRectangle(new SolidBrush(_FillRectColor), _fillRect);
			graphics.DrawImage(_selectedBitmap, SelectionRect);
			graphics.ResetClip();

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
			Rectangle bitmapArea = GetCanvasRegion();
			graphics.SetClip(bitmapArea);
			graphics.FillRectangle(new SolidBrush(_FillRectColor), _fillRect);
			graphics.DrawImage(_selectedBitmap, _rectBeforeAdjust);
			graphics.ResetClip();
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
			Rectangle bitmapArea = GetCanvasRegion();
			graphics.SetClip(bitmapArea);
			graphics.FillRectangle(new SolidBrush(_FillRectColor), _fillRect);
			graphics.DrawImage(_selectedBitmap, SelectionRect);
			graphics.ResetClip();
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

		public override void Clear(Color color)
		{
			ClearBitmap(color);
		}

		public override void CommitCurrentShape()
		{
			BitmapDrawImage();
		}

		public override void RotateRight()
		{
			drawStatus = DrawStatus.CanAdjusted;
			SelectionRect = RotateRectangle90Degrees();
			_selectedBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
		}

		public override void RotateLeft()
		{
			drawStatus = DrawStatus.CanAdjusted;
			SelectionRect = RotateRectangle90Degrees();
			_selectedBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
		}

		public override void Rotate180()
		{
			drawStatus = DrawStatus.CanAdjusted;
			_selectedBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
		}

		public override void FlipHorizontal()
		{
			drawStatus = DrawStatus.CanAdjusted;
			_selectedBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
		}

		public override void FlipVertical()
		{
			drawStatus = DrawStatus.CanAdjusted;
			_selectedBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
		}
	}
}
