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
	//ひし形
	public class Rhombus : Shape
	{
		public Rhombus() { }
		public Rhombus(Bitmap bitmap, Panel panel,float scale) : base(bitmap, panel, scale){}

		//頂点の集合
		private List<Point> _vertexs = new List<Point>();

		private void BitmapDrawRhombus()
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
				BitmapDrawRhombus();
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
				BitmapDrawRhombus();
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
				BitmapDrawRhombus();
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
				CalculateRhombusPoints();
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Moving)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionRect.Offset(deltaX, deltaY);
				Offset = e.Location;
				//CalculateRhombusPoints();
				UpdateRhombusPoints();
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Adjusting)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionAdjusting(deltaX, deltaY, ref SelectionRect);
				Offset = e.Location;
				//CalculateRhombusPoints();
				UpdateRhombusPoints();
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
		/// ひし形頂点の計算
		/// </summary>
		private void CalculateRhombusPoints()
		{
			_vertexs.Clear();
			//上部頂点
			var point = new Point(SelectionRect.Left + SelectionRect.Width / 2, SelectionRect.Top);
			_vertexs.Add(point);

			//右中の頂点
			point = new Point(SelectionRect.Right, SelectionRect.Top+SelectionRect.Height/2);
			_vertexs.Add(point);

			//下部の頂点
			point = new Point(SelectionRect.Left + SelectionRect.Width / 2, SelectionRect.Bottom);
			_vertexs.Add(point);

			//左中の頂点
			point = new Point(SelectionRect.Left,SelectionRect.Top+SelectionRect.Height/2);
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
			//using (Pen selectionPen = new Pen(ForeColor, Size * Scale))
			//{
			//	selectionPen.DashStyle = DashStyle.Solid;
			//	Rectangle bitmapArea = GetCanvasRegion();
			//	graphics.SetClip(bitmapArea);
			//	graphics.DrawPolygon(selectionPen, _vertexs.ToArray());
			//	graphics.ResetClip();
			//}
			tempCanvas = (Bitmap)canvas.Clone();
			using (Graphics g = Graphics.FromImage(tempCanvas))
			{
				using (Pen selectionPen = new Pen(ForeColor, Size))
				{
					g.DrawPolygon(selectionPen, ConvertVertexs(_vertexs));
				}
			}
		}
		private void DrawCanMoveOrAdjusted(Graphics graphics)
		{
			//using (Pen selectionPen = new Pen(ForeColor, Size * Scale))
			//{
			//	selectionPen.DashStyle = DashStyle.Solid;
			//	Rectangle bitmapArea = GetCanvasRegion();
			//	graphics.SetClip(bitmapArea);
			//	graphics.DrawPolygon(selectionPen, _vertexs.ToArray());
			//	graphics.ResetClip();
			//}
			tempCanvas = (Bitmap)canvas.Clone();
			using (Graphics g = Graphics.FromImage(tempCanvas))
			{
				using (Pen selectionPen = new Pen(ForeColor, Size))
				{
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
			BitmapDrawRhombus();
		}

		public override void RotateRight()
		{
			drawStatus = DrawStatus.CanAdjusted;
			SelectionRect = RotateRectangle90Degrees();
			RotationCount = (RotationCount + 1) % 4;
			UpdateRhombusPoints();
		}

		public override void RotateLeft()
		{
			drawStatus = DrawStatus.CanAdjusted;
			SelectionRect = RotateRectangle90Degrees();
			RotationCount = (RotationCount + 3) % 4;
			UpdateRhombusPoints();
		}

		public override void Rotate180()
		{
			drawStatus = DrawStatus.CanAdjusted;
		}

		public override void FlipHorizontal()
		{
			drawStatus = DrawStatus.CanAdjusted;
		}

		public override void FlipVertical()
		{
			drawStatus = DrawStatus.CanAdjusted;
		}

		private void UpdateRhombusPoints()
		{
			_vertexs.Clear();

			Point p1 = new Point(SelectionRect.Left + SelectionRect.Width / 2, SelectionRect.Top);//上中
			Point p2 = new Point(SelectionRect.Right, SelectionRect.Top + SelectionRect.Height / 2);//右中
			Point p3 = new Point(SelectionRect.Left + SelectionRect.Width / 2, SelectionRect.Bottom);//下中
			Point p4 = new Point(SelectionRect.Left, SelectionRect.Top + SelectionRect.Height / 2);//左中

			switch (RotationCount)
			{
				case 0:
					_vertexs.Add(p1);
					_vertexs.Add(p2);
					_vertexs.Add(p3);
					_vertexs.Add(p4);
					break;

				case 1:
					_vertexs.Add(p1);
					_vertexs.Add(p2);
					_vertexs.Add(p3);
					_vertexs.Add(p4);
					break;

				case 2:
					_vertexs.Add(p1);
					_vertexs.Add(p2);
					_vertexs.Add(p3);
					_vertexs.Add(p4);
					break;

				case 3:
					_vertexs.Add(p1);
					_vertexs.Add(p2);
					_vertexs.Add(p3);
					_vertexs.Add(p4);
					break;
			}
			
		}

	}
}
