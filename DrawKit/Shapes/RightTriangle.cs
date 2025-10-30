using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawKit.Shapes
{
	/// <summary>
	/// 直角三角形
	/// </summary>
	public class RightTriangle : Shape
	{
		//顶点集合
		private List<Point> _vertexs = new List<Point>();

		public RightTriangle() { }
		public RightTriangle(Bitmap bitmap, Panel panel,float scale) : base(bitmap, panel, scale) { }

		private void BitmapDrawRightTriangle()
		{
			if (canvas == null) return;
			if (SelectionRect.Width == 0 && SelectionRect.Height == 0) return;
			//using (Graphics g = Graphics.FromImage(canvas))
			//{
			//	using (Pen selectionPen = new Pen(ForeColor, Size))
			//	{
			//		selectionPen.DashStyle = DashStyle.Solid;
			//		g.CompositingQuality = CompositingQuality.HighQuality;
			//		g.InterpolationMode = InterpolationMode.NearestNeighbor;
			//		g.SmoothingMode = SmoothingMode.None;
			//		g.DrawPolygon(selectionPen,ConvertVertexs(_vertexs));
			//	}
			//}

			DrawTempCanvasOnMain();

			drawStatus = DrawStatus.CannotMovedOrAdjusted;
			SelectionRect = Rectangle.Empty;
			RotationCount = 0;
			IsFlippedHorizontally = false;
			IsFlippedVertically = false;
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
		private void MouseLeftButtonDownHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.CannotMovedOrAdjusted)
			{
				if (!IsValidLocation(e.Location) && SelectionRect == Rectangle.Empty) return;
				BitmapDrawRightTriangle();
				StartPoint = e.Location;
				drawStatus = DrawStatus.Creating;
			}
			else if (drawStatus == DrawStatus.CanMove)
			{
				Offset = e.Location;
				drawStatus = DrawStatus.Moving;
			}
			else if (drawStatus == DrawStatus.CanAdjusted)
			{
				Offset = e.Location;
				drawStatus = DrawStatus.Adjusting;
			}
			else if (drawStatus == DrawStatus.CanvasAdjustable)
			{
				BitmapDrawRightTriangle();
				AdjustingCanvasRect = GetCanvasRegion();
				Offset = e.Location;
				drawStatus = DrawStatus.CanvasAdjusting;
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
				BitmapDrawRightTriangle();
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
				CalculateRightTrianglePoints();
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Moving)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionRect.Offset(deltaX, deltaY);
				Offset = e.Location;
				UpdateRightTrianglePoints();
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Adjusting)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionAdjusting(deltaX, deltaY, ref SelectionRect);
				Offset = e.Location;
				UpdateRightTrianglePoints();
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

		/// <summary>
		/// 计算直角三角形顶点
		/// </summary>
		private void CalculateRightTrianglePoints()
		{
			_vertexs.Clear();
			//顶部顶点
			var point = new Point(SelectionRect.Left, SelectionRect.Top);
			_vertexs.Add(point);

			//右下顶点
			point = new Point(SelectionRect.Right, SelectionRect.Bottom);
			_vertexs.Add(point);

			//左下顶点
			point = new Point(SelectionRect.Left, SelectionRect.Bottom);
			_vertexs.Add(point);
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
			if (tempCanvas != null)
			{
				BitmapDrawShape(tempCanvas, graphics);
			}
			if (drawStatus == DrawStatus.Creating)
			{
				if (SelectionRect.Width == 0 || SelectionRect.Height == 0) return;
				DrawCreating(graphics);
			}
			else if (drawStatus == DrawStatus.Moving ||
				drawStatus == DrawStatus.CanMove ||
				drawStatus == DrawStatus.CanAdjusted ||
				drawStatus == DrawStatus.Adjusting ||
				drawStatus == DrawStatus.CompleteAdjustment ||
				drawStatus == DrawStatus.AdjustTheStyle)
			{
				if (SelectionRect.Width == 0 || SelectionRect.Height == 0) return;
				DrawCanMoveOrAdjusted(graphics);
			}
			else if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				DrawCanvasAdjusted(graphics);
			}
		}
		private void DrawCreating(Graphics graphics)
		{
			tempCanvas = GetTempCanvas();
			using (Graphics g = Graphics.FromImage(tempCanvas))
			{
				using (Pen selectionPen = new Pen(ForeColor, Size))
				{
					if (EnableAntiAlias) g.SmoothingMode = SmoothingMode.AntiAlias;
					g.DrawPolygon(selectionPen, ConvertVertexs(_vertexs));
				}
			}
		}
		private void DrawCanMoveOrAdjusted(Graphics graphics)
		{
			tempCanvas = GetTempCanvas();
			using (Graphics g = Graphics.FromImage(tempCanvas))
			{
				using (Pen selectionPen = new Pen(ForeColor, Size))
				{
					if (EnableAntiAlias) g.SmoothingMode = SmoothingMode.AntiAlias;
					g.DrawPolygon(selectionPen, ConvertVertexs(_vertexs));
				}
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
	
		public override void Clear(Color color)
		{
			ClearBitmap(color);
		}

		public override void CommitCurrentShape()
		{
			BitmapDrawRightTriangle();
		}

		public override void RotateRight()
		{
			drawStatus = DrawStatus.CanAdjusted;
			SelectionRect = RotateRectangle90Degrees();
			RotationCount = (RotationCount + 1) % 4;
			UpdateRightTrianglePoints();
		}

		public override void RotateLeft()
		{
			drawStatus = DrawStatus.CanAdjusted;
			SelectionRect = RotateRectangle90Degrees();
			RotationCount = (RotationCount + 3) % 4;
			UpdateRightTrianglePoints();
		}

		public override void Rotate180()
		{
			drawStatus = DrawStatus.CanAdjusted;
			RotationCount = (RotationCount + 2) % 4;
			UpdateRightTrianglePoints();
		}
		public override void FlipHorizontal()
		{
			drawStatus = DrawStatus.CanAdjusted;
			IsFlippedHorizontally = !IsFlippedHorizontally;
			UpdateRightTrianglePoints();
		}

		public override void FlipVertical()
		{
			drawStatus = DrawStatus.CanAdjusted;
			IsFlippedVertically = !IsFlippedVertically;
			UpdateRightTrianglePoints();
		}
		private void UpdateRightTrianglePoints()
		{
			_vertexs.Clear();
			Point p1 = new Point(SelectionRect.Left, SelectionRect.Top);         // 左上
			Point p4 = new Point(SelectionRect.Right, SelectionRect.Top);        // 右上
			Point p2 = new Point(SelectionRect.Right, SelectionRect.Bottom);     // 右下
			Point p3 = new Point(SelectionRect.Left, SelectionRect.Bottom);      // 左下
			
			switch (RotationCount)
			{
				case 0:
					if (IsFlippedVertically == false && IsFlippedHorizontally == false)
					{
						_vertexs.Add(p1);
						_vertexs.Add(p2);
						_vertexs.Add(p3);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == false)
					{
						_vertexs.Add(p1);
						_vertexs.Add(p4);
						_vertexs.Add(p3);
					}
					else if (IsFlippedVertically == false && IsFlippedHorizontally == true)
					{
						_vertexs.Add(p4);
						_vertexs.Add(p2);
						_vertexs.Add(p3);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == true)
					{
						_vertexs.Add(p1);
						_vertexs.Add(p4);
						_vertexs.Add(p2);
					}
						break;
				case 1:
					if (IsFlippedVertically == false && IsFlippedHorizontally == false)
					{
						_vertexs.Add(p1);
						_vertexs.Add(p4);
						_vertexs.Add(p3);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == false)
					{
						_vertexs.Add(p1);
						_vertexs.Add(p2);
						_vertexs.Add(p3);
					}
					else if (IsFlippedVertically == false && IsFlippedHorizontally == true)
					{
						_vertexs.Add(p1);
						_vertexs.Add(p4);
						_vertexs.Add(p2);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == true)
					{
						_vertexs.Add(p4);
						_vertexs.Add(p2);
						_vertexs.Add(p3);
					}
					break;
				case 2:
					if (IsFlippedVertically == false && IsFlippedHorizontally == false)
					{
						_vertexs.Add(p1);
						_vertexs.Add(p4);
						_vertexs.Add(p2);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == false)
					{
						_vertexs.Add(p4);
						_vertexs.Add(p2);
						_vertexs.Add(p3);
					}
					else if (IsFlippedVertically == false && IsFlippedHorizontally == true)
					{
						_vertexs.Add(p1);
						_vertexs.Add(p4);
						_vertexs.Add(p3);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == true)
					{
						_vertexs.Add(p1);
						_vertexs.Add(p2);
						_vertexs.Add(p3);
					}
					break;
				case 3:
					if (IsFlippedVertically == false && IsFlippedHorizontally == false)
					{
						_vertexs.Add(p4);
						_vertexs.Add(p2);
						_vertexs.Add(p3);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == false)
					{
						_vertexs.Add(p1);
						_vertexs.Add(p4);
						_vertexs.Add(p2);
					}
					else if (IsFlippedVertically == false && IsFlippedHorizontally == true)
					{
						_vertexs.Add(p1);
						_vertexs.Add(p2);
						_vertexs.Add(p3);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == true)
					{
						_vertexs.Add(p1);
						_vertexs.Add(p4);
						_vertexs.Add(p3);
					}
					break;
			}
		}

		public override void KeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				if (drawStatus == DrawStatus.CanvasAdjusting) return;
				CancelDrawing();
			}
		}
	}
}
