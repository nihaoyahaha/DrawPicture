using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
		private Bitmap _copySelectedBitmap;
		private Rectangle _rectBeforeAdjust;
		private Rectangle _fillRect = Rectangle.Empty;
		private Color _FillRectColor;

		public RectangularSelection() { }
		public RectangularSelection(Bitmap bitmap, Panel panel, float scale) : base(bitmap, panel, scale) { }
		private void BitmapDrawImage()
		{
			if (_selectedBitmap == null) return;
			if (SelectionRect == Rectangle.Empty) return;
			DrawTempCanvasOnMain();
			drawStatus = DrawStatus.CannotMovedOrAdjusted;
			_selectedBitmap?.Dispose();
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

		public void DelSelectedBitmap()
		{
			tempCanvas = GetTempCanvas();
			using (Graphics g = Graphics.FromImage(tempCanvas))
			{
				g.FillRectangle(new SolidBrush(_FillRectColor), ConvertSelectionRectToCanvasRect(_fillRect));
			}
			BitmapDrawImage();
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

		public void Cancel()
		{
			CancelDrawing();
		}
		protected override void CancelDrawing()
		{
			drawStatus = DrawStatus.CannotMovedOrAdjusted;
			tempCanvas?.Dispose();
			tempCanvas = null;
			_selectedBitmap?.Dispose();
			_selectedBitmap = null;
			SelectionRect = Rectangle.Empty;
			_fillRect = Rectangle.Empty;
			panel.Invalidate();
		}

		private void MouseLeftButtonDownHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.CannotMovedOrAdjusted)
			{
				if (!IsValidLocation(e.Location) && SelectionRect == Rectangle.Empty) return;
				BitmapDrawImage();
				StartPoint = e.Location;
				drawStatus = DrawStatus.Creating;
				_FillRectColor = Color.White; //ForeColor;
			}
			else if (drawStatus == DrawStatus.CanMove)
			{
				if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
				{
					var rect = SelectionRect;
					_copySelectedBitmap?.Dispose();
					_copySelectedBitmap = null;
					_copySelectedBitmap = (Bitmap)_selectedBitmap.Clone();
					CommitCurrentShape();
					_selectedBitmap = (Bitmap)_copySelectedBitmap.Clone();
					SelectionRect = rect;
				}
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
					_selectedBitmap?.Dispose();
					_selectedBitmap = null;
					_selectedBitmap = new Bitmap(SelectionRect.Width, SelectionRect.Height);
					using (Graphics g = Graphics.FromImage(_selectedBitmap))
					{
						ImageAttributes imageAttr = new ImageAttributes();
						imageAttr.SetColorKey(Color.White, Color.White);
						g.CompositingQuality = CompositingQuality.HighQuality;
						g.InterpolationMode = InterpolationMode.NearestNeighbor;
						g.SmoothingMode = SmoothingMode.None;
						var rect = ConvertSelectionRectToCanvasRect(SelectionRect);
						g.DrawImage(canvas,
									new Rectangle(0, 0, SelectionRect.Width, SelectionRect.Height),
									rect.X,
									rect.Y,
									rect.Width,
									rect.Height,
									GraphicsUnit.Pixel,
									imageAttr);

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
				BitmapDrawShape(canvas, graphics);
			}
			if (tempCanvas != null)
			{
				BitmapDrawShape(tempCanvas, graphics);
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

			//tempCanvas = (Bitmap)canvas.Clone();
			//using (Graphics g = Graphics.FromImage(tempCanvas))
			//{
			//	using (Pen selectionPen = new Pen(Color.Black, 0.5f))
			//	{   selectionPen.DashStyle = DashStyle.Dash;
			//		selectionPen.DashPattern = new float[] { 5.0f, 4.0f };// 划线长，间隔长
			//		g.CompositingQuality = CompositingQuality.HighQuality;
			//		g.InterpolationMode = InterpolationMode.NearestNeighbor;
			//		g.SmoothingMode = SmoothingMode.None;
			//		g.DrawRectangle(selectionPen,ConvertSelectionRectToCanvasRect( SelectionRect));
			//	}
			//}
		}

		private void DrawCanMoveOrAdjusted(Graphics graphics)
		{
			if (_selectedBitmap == null) return;
			tempCanvas = GetTempCanvas();
			using (Graphics g = Graphics.FromImage(tempCanvas))
			{
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.InterpolationMode = InterpolationMode.NearestNeighbor;
				g.SmoothingMode = SmoothingMode.None;
				g.FillRectangle(new SolidBrush(_FillRectColor), ConvertSelectionRectToCanvasRect(_fillRect));
				g.DrawImage(_selectedBitmap, ConvertSelectionRectToCanvasRect(SelectionRect));
			}

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
			tempCanvas = GetTempCanvas();
			using (Graphics g = Graphics.FromImage(tempCanvas))
			{
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.InterpolationMode = InterpolationMode.NearestNeighbor;
				g.SmoothingMode = SmoothingMode.None;
				g.FillRectangle(new SolidBrush(_FillRectColor), ConvertSelectionRectToCanvasRect(_fillRect));
				g.DrawImage(_selectedBitmap, ConvertSelectionRectToCanvasRect(_rectBeforeAdjust));
			}

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
			tempCanvas = GetTempCanvas();
			using (Graphics g = Graphics.FromImage(tempCanvas))
			{
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.InterpolationMode = InterpolationMode.NearestNeighbor;
				g.SmoothingMode = SmoothingMode.None;
				g.FillRectangle(new SolidBrush(_FillRectColor), ConvertSelectionRectToCanvasRect(_fillRect));
				g.DrawImage(_selectedBitmap, ConvertSelectionRectToCanvasRect(SelectionRect));
			}
			graphics.DrawImage(tempCanvas, GetCanvasRegion());
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

		public override void KeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				DelSelectedBitmap();
			}
			else if ((e.Control) && (e.KeyCode == Keys.C))
			{
				CopySelectionRectToClipboard();
			}
			else if ((e.Control) && (e.KeyCode == Keys.V))
			{
				PasteBitmapFromClipboard();
			}
		}

		private void CopySelectionRectToClipboard()
		{
			if (SelectionRect == Rectangle.Empty) return;
			if (_selectedBitmap == null) return;
			var bitmap = RestoreBitmap(_selectedBitmap, Scale);
			Clipboard.SetImage(bitmap);
			bitmap?.Dispose();
			_copySelectedBitmap?.Dispose();
			_copySelectedBitmap = null;
			_copySelectedBitmap = (Bitmap)_selectedBitmap.Clone();
		}

		private Bitmap RestoreBitmap(Bitmap scaledBitmap, float scale)
		{
			int originalWidth = (int)(scaledBitmap.Width / scale);
			int originalHeight = (int)(scaledBitmap.Height / scale);

			Bitmap result = new Bitmap(originalWidth, originalHeight);
			using (Graphics g = Graphics.FromImage(result))
			{
				g.Clear(Color.White);
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.InterpolationMode = InterpolationMode.NearestNeighbor;
				g.SmoothingMode = SmoothingMode.None;
				g.DrawImage(scaledBitmap, new Rectangle(0, 0, originalWidth, originalHeight));
			}
			return result;
		}
		private Bitmap EnlargeBitmap(Bitmap originalBitmap,float scale)
		{
			int newWidth = (int)(originalBitmap.Width * scale);
			int newHeight = (int)(originalBitmap.Height * scale);

			Bitmap enlargedBitmap = new Bitmap(newWidth, newHeight);
			using (Graphics g = Graphics.FromImage(enlargedBitmap))
			{
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.InterpolationMode = InterpolationMode.NearestNeighbor;
				g.SmoothingMode = SmoothingMode.None;
				g.DrawImage(originalBitmap, new Rectangle(0, 0, newWidth, newHeight));
			}
			return enlargedBitmap;
		}
		private void PasteBitmapFromClipboard()
		{
			var bitmap = (Bitmap)Clipboard.GetImage();
			if (bitmap == null)	return;
			CommitCurrentShape();
			_selectedBitmap = EnlargeBitmap(bitmap,Scale);
			var scrollPos = new Point(Math.Abs(panel.AutoScrollPosition.X), Math.Abs(panel.AutoScrollPosition.Y));
			var rect = GetCanvasRegion();
			SelectionRect = new Rectangle(rect.X + scrollPos.X, rect.Y + scrollPos.Y, _selectedBitmap.Width, _selectedBitmap.Height);
			drawStatus = DrawStatus.CanAdjusted;
			panel.Refresh();
			panel.Refresh();
		}

	}
}
