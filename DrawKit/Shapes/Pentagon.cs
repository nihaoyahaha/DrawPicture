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
	/// 五角形
	/// </summary>
	public class Pentagon : Shape
	{
		//頂点の集合
		private List<Point> _vertexs = new List<Point>();
		//中央頂点スケール
		private float _middleVertexRatio = 0.37f;
		//下部頂点スケール
		private float _bottomVertexRatio = 0.19f;

		public Pentagon() { }
		public Pentagon(Bitmap bitmap, Panel panel,float scale) : base(bitmap, panel, scale){}

		private void BitmapDrawPentagon()
		{
			if (canvas == null) return;
			if (SelectionRect.Width == 0 && SelectionRect.Height == 0) return;
			using (Graphics g = Graphics.FromImage(canvas))
			{
				using (Pen selectionPen = new Pen(ForeColor, Size))
				{
					selectionPen.DashStyle = DashStyle.Solid;
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.PixelOffsetMode = PixelOffsetMode.HighQuality;
					g.DrawPolygon(selectionPen,ConvertVertexs( _vertexs));
				}
			}
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
				BitmapDrawPentagon();
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
				BitmapDrawPentagon();
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
				BitmapDrawPentagon();
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
				CalculatePentagonPoints();
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Moving)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionRect.Offset(deltaX, deltaY);
				Offset = e.Location;
				//CalculatePentagonPoints();
				UpdatePentagonPoints();
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Adjusting)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionAdjusting(deltaX, deltaY, ref SelectionRect);
				Offset = e.Location;
				//CalculatePentagonPoints();
				UpdatePentagonPoints();
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

		/// <summary>
		/// 五角形頂点の計算
		/// </summary>
		private void CalculatePentagonPoints()
		{
			_vertexs.Clear();
			//上部頂点
			var point = new Point(SelectionRect.X+SelectionRect.Width/2,SelectionRect.Y);
			_vertexs.Add(point);

			//右上の頂点
			int middleVertexDistance = (int)(SelectionRect.Height * _middleVertexRatio);
			point = new Point(SelectionRect.Right,SelectionRect.Top+ middleVertexDistance);
			_vertexs.Add(point);

			//右下の頂点
			int bottomVertexDistance = (int)(SelectionRect.Width * _bottomVertexRatio);
			point = new Point(SelectionRect.Right- bottomVertexDistance, SelectionRect.Bottom);
			_vertexs.Add(point);

			//左下の頂点
			point = new Point(SelectionRect.Left+ bottomVertexDistance, SelectionRect.Bottom);
			_vertexs.Add(point);

			//左上の頂点
			point = new Point(SelectionRect.Left, SelectionRect.Top + middleVertexDistance);
			_vertexs.Add(point);
		}


		private void DrawCreating(Graphics graphics)
		{
			using (Pen selectionPen = new Pen(ForeColor, Size* Scale))
			{
				selectionPen.DashStyle = DashStyle.Solid;
				Rectangle bitmapArea = GetCanvasRegion();
				graphics.SetClip(bitmapArea);
				graphics.DrawPolygon(selectionPen, _vertexs.ToArray());
				graphics.ResetClip();
			}
		}

		private void DrawCanMoveOrAdjusted(Graphics graphics)
		{
			using (Pen selectionPen = new Pen(ForeColor, Size * Scale))
			{
				selectionPen.DashStyle = DashStyle.Solid;
				Rectangle bitmapArea = GetCanvasRegion();
				graphics.SetClip(bitmapArea);
				graphics.DrawPolygon(selectionPen, _vertexs.ToArray());
				graphics.ResetClip();
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
			BitmapDrawPentagon();
		}

		public override void RotateRight()
		{
			drawStatus = DrawStatus.CanAdjusted;
			SelectionRect = RotateRectangle90Degrees();
			RotationCount = (RotationCount + 1) % 4;
			UpdatePentagonPoints();
		}

		public override void RotateLeft()
		{
			drawStatus = DrawStatus.CanAdjusted;
			SelectionRect = RotateRectangle90Degrees();
			RotationCount = (RotationCount + 3) % 4;
			UpdatePentagonPoints();
		}

		public override void Rotate180()
		{
			drawStatus = DrawStatus.CanAdjusted;
			RotationCount = (RotationCount + 2) % 4;
			UpdatePentagonPoints();
		}

		public override void FlipHorizontal()
		{
			drawStatus = DrawStatus.CanAdjusted;
			IsFlippedHorizontally = !IsFlippedHorizontally;
			UpdatePentagonPoints();
		}

		public override void FlipVertical()
		{
			drawStatus = DrawStatus.CanAdjusted;
			IsFlippedVertically = !IsFlippedVertically;
			UpdatePentagonPoints();
		}
		private void UpdatePentagonPoints()
		{
			_vertexs.Clear();
			Point point;
			int middleVertexDistance;
			int bottomVertexDistance;
			switch (RotationCount)
			{
				case 0:
					if (IsFlippedVertically == false && IsFlippedHorizontally == false)
					{
						//上部頂点
						point = new Point(SelectionRect.X + SelectionRect.Width / 2, SelectionRect.Y);
						_vertexs.Add(point);

						//右上の頂点
						middleVertexDistance = (int)(SelectionRect.Height * _middleVertexRatio);
						point = new Point(SelectionRect.Right, SelectionRect.Top + middleVertexDistance);
						_vertexs.Add(point);

						//右下の頂点
						bottomVertexDistance = (int)(SelectionRect.Width * _bottomVertexRatio);
						point = new Point(SelectionRect.Right - bottomVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);

						//左下の頂点
						point = new Point(SelectionRect.Left + bottomVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);

						//左上の頂点
						point = new Point(SelectionRect.Left, SelectionRect.Top + middleVertexDistance);
						_vertexs.Add(point);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == false)
					{
						//下中
						point = new Point(SelectionRect.Left + SelectionRect.Width / 2, SelectionRect.Bottom);
						_vertexs.Add(point);

						//頂点左
						middleVertexDistance = (int)(SelectionRect.Height * _middleVertexRatio);
						point = new Point(SelectionRect.Left, SelectionRect.Bottom - middleVertexDistance);
						_vertexs.Add(point);

						//頂点左上
						bottomVertexDistance = (int)(SelectionRect.Width * _bottomVertexRatio);
						point = new Point(SelectionRect.Left + bottomVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);

						//頂点右上
						point = new Point(SelectionRect.Right - bottomVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);

						//頂点右
						point = new Point(SelectionRect.Right, SelectionRect.Bottom - middleVertexDistance);
						_vertexs.Add(point);
					}
					else if (IsFlippedVertically == false && IsFlippedHorizontally == true)
					{
						//上部頂点
						point = new Point(SelectionRect.X + SelectionRect.Width / 2, SelectionRect.Y);
						_vertexs.Add(point);

						//右上の頂点
						middleVertexDistance = (int)(SelectionRect.Height * _middleVertexRatio);
						point = new Point(SelectionRect.Right, SelectionRect.Top + middleVertexDistance);
						_vertexs.Add(point);

						//右下の頂点
						bottomVertexDistance = (int)(SelectionRect.Width * _bottomVertexRatio);
						point = new Point(SelectionRect.Right - bottomVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);

						//左下の頂点
						point = new Point(SelectionRect.Left + bottomVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);

						//左上の頂点
						point = new Point(SelectionRect.Left, SelectionRect.Top + middleVertexDistance);
						_vertexs.Add(point);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == true)
					{
						//下中
						point = new Point(SelectionRect.Left + SelectionRect.Width / 2, SelectionRect.Bottom);
						_vertexs.Add(point);

						//頂点左
						middleVertexDistance = (int)(SelectionRect.Height * _middleVertexRatio);
						point = new Point(SelectionRect.Left, SelectionRect.Bottom - middleVertexDistance);
						_vertexs.Add(point);

						//頂点左上
						bottomVertexDistance = (int)(SelectionRect.Width * _bottomVertexRatio);
						point = new Point(SelectionRect.Left + bottomVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);

						//頂点右上
						point = new Point(SelectionRect.Right - bottomVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);

						//頂点右
						point = new Point(SelectionRect.Right, SelectionRect.Bottom - middleVertexDistance);
						_vertexs.Add(point);
					}
					break;
				case 1:
					if (IsFlippedVertically == false && IsFlippedHorizontally == false)
					{
						//右中
						point = new Point(SelectionRect.Right, SelectionRect.Top + SelectionRect.Height / 2);
						_vertexs.Add(point);

						//頂点下
						middleVertexDistance = (int)(SelectionRect.Width * _middleVertexRatio);
						point = new Point(SelectionRect.Right - middleVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);

						//頂点左下
						bottomVertexDistance = (int)(SelectionRect.Height * _bottomVertexRatio);
						point = new Point(SelectionRect.Left, SelectionRect.Bottom - bottomVertexDistance);
						_vertexs.Add(point);

						//頂点左上
						point = new Point(SelectionRect.Left, SelectionRect.Top + bottomVertexDistance);
						_vertexs.Add(point);

						//頂点上
						point = new Point(SelectionRect.Right - middleVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == false)
					{
						//右中
						point = new Point(SelectionRect.Right, SelectionRect.Top + SelectionRect.Height / 2);
						_vertexs.Add(point);

						//頂点下
						middleVertexDistance = (int)(SelectionRect.Width * _middleVertexRatio);
						point = new Point(SelectionRect.Right - middleVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);

						//頂点左下
						bottomVertexDistance = (int)(SelectionRect.Height * _bottomVertexRatio);
						point = new Point(SelectionRect.Left, SelectionRect.Bottom - bottomVertexDistance);
						_vertexs.Add(point);

						//頂点左上
						point = new Point(SelectionRect.Left, SelectionRect.Top + bottomVertexDistance);
						_vertexs.Add(point);

						//頂点上
						point = new Point(SelectionRect.Right - middleVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);
					}
					else if (IsFlippedVertically == false && IsFlippedHorizontally == true)
					{
						//頂点左中
						point = new Point(SelectionRect.Left, SelectionRect.Top + SelectionRect.Height / 2);
						_vertexs.Add(point);

						//頂点上
						middleVertexDistance = (int)(SelectionRect.Width * _middleVertexRatio);
						point = new Point(SelectionRect.Left + middleVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);

						//頂点右上
						bottomVertexDistance = (int)(SelectionRect.Height * _bottomVertexRatio);
						point = new Point(SelectionRect.Right, SelectionRect.Top + bottomVertexDistance);
						_vertexs.Add(point);

						//頂点右下
						point = new Point(SelectionRect.Right, SelectionRect.Bottom - bottomVertexDistance);
						_vertexs.Add(point);

						//頂点下
						point = new Point(SelectionRect.Left + middleVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == true)
					{
						//頂点左中
						point = new Point(SelectionRect.Left, SelectionRect.Top + SelectionRect.Height / 2);
						_vertexs.Add(point);

						//頂点上
						middleVertexDistance = (int)(SelectionRect.Width * _middleVertexRatio);
						point = new Point(SelectionRect.Left + middleVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);

						//頂点右上
						bottomVertexDistance = (int)(SelectionRect.Height * _bottomVertexRatio);
						point = new Point(SelectionRect.Right, SelectionRect.Top + bottomVertexDistance);
						_vertexs.Add(point);

						//頂点右下
						point = new Point(SelectionRect.Right, SelectionRect.Bottom - bottomVertexDistance);
						_vertexs.Add(point);

						//頂点下
						point = new Point(SelectionRect.Left + middleVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);
					}
					break;
				case 2:
					if (IsFlippedVertically == false && IsFlippedHorizontally == false)
					{
						//下中
						point = new Point(SelectionRect.Left + SelectionRect.Width / 2, SelectionRect.Bottom);
						_vertexs.Add(point);

						//頂点左
						middleVertexDistance = (int)(SelectionRect.Height * _middleVertexRatio);
						point = new Point(SelectionRect.Left, SelectionRect.Bottom - middleVertexDistance);
						_vertexs.Add(point);

						//頂点左上
						bottomVertexDistance = (int)(SelectionRect.Width * _bottomVertexRatio);
						point = new Point(SelectionRect.Left + bottomVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);

						//頂点右上
						point = new Point(SelectionRect.Right - bottomVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);

						//頂点右
						point = new Point(SelectionRect.Right, SelectionRect.Bottom - middleVertexDistance);
						_vertexs.Add(point);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == false)
					{
						//上部頂点
						point = new Point(SelectionRect.X + SelectionRect.Width / 2, SelectionRect.Y);
						_vertexs.Add(point);

						//右上の頂点
						middleVertexDistance = (int)(SelectionRect.Height * _middleVertexRatio);
						point = new Point(SelectionRect.Right, SelectionRect.Top + middleVertexDistance);
						_vertexs.Add(point);

						//右下の頂点
						bottomVertexDistance = (int)(SelectionRect.Width * _bottomVertexRatio);
						point = new Point(SelectionRect.Right - bottomVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);

						//左下の頂点
						point = new Point(SelectionRect.Left + bottomVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);

						//左上の頂点
						point = new Point(SelectionRect.Left, SelectionRect.Top + middleVertexDistance);
						_vertexs.Add(point);
					}
					else if (IsFlippedVertically == false && IsFlippedHorizontally == true)
					{
						//下中
						point = new Point(SelectionRect.Left + SelectionRect.Width / 2, SelectionRect.Bottom);
						_vertexs.Add(point);

						//頂点左
						middleVertexDistance = (int)(SelectionRect.Height * _middleVertexRatio);
						point = new Point(SelectionRect.Left, SelectionRect.Bottom - middleVertexDistance);
						_vertexs.Add(point);

						//頂点左上
						bottomVertexDistance = (int)(SelectionRect.Width * _bottomVertexRatio);
						point = new Point(SelectionRect.Left + bottomVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);

						//頂点右上
						point = new Point(SelectionRect.Right - bottomVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);

						//頂点右
						point = new Point(SelectionRect.Right, SelectionRect.Bottom - middleVertexDistance);
						_vertexs.Add(point);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == true)
					{
						//上部頂点
						point = new Point(SelectionRect.X + SelectionRect.Width / 2, SelectionRect.Y);
						_vertexs.Add(point);

						//右上の頂点
						middleVertexDistance = (int)(SelectionRect.Height * _middleVertexRatio);
						point = new Point(SelectionRect.Right, SelectionRect.Top + middleVertexDistance);
						_vertexs.Add(point);

						//右下の頂点
						bottomVertexDistance = (int)(SelectionRect.Width * _bottomVertexRatio);
						point = new Point(SelectionRect.Right - bottomVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);

						//左下の頂点
						point = new Point(SelectionRect.Left + bottomVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);

						//左上の頂点
						point = new Point(SelectionRect.Left, SelectionRect.Top + middleVertexDistance);
						_vertexs.Add(point);
					}

					break;
				case 3:
					if (IsFlippedVertically == false && IsFlippedHorizontally == false)
					{
						//頂点左中
						point = new Point(SelectionRect.Left, SelectionRect.Top + SelectionRect.Height / 2);
						_vertexs.Add(point);

						//頂点上
						middleVertexDistance = (int)(SelectionRect.Width * _middleVertexRatio);
						point = new Point(SelectionRect.Left + middleVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);

						//頂点右上
						bottomVertexDistance = (int)(SelectionRect.Height * _bottomVertexRatio);
						point = new Point(SelectionRect.Right, SelectionRect.Top + bottomVertexDistance);
						_vertexs.Add(point);

						//頂点右下
						point = new Point(SelectionRect.Right, SelectionRect.Bottom - bottomVertexDistance);
						_vertexs.Add(point);

						//頂点下
						point = new Point(SelectionRect.Left + middleVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == false)
					{
						//頂点左中
						point = new Point(SelectionRect.Left, SelectionRect.Top + SelectionRect.Height / 2);
						_vertexs.Add(point);

						//頂点上
						middleVertexDistance = (int)(SelectionRect.Width * _middleVertexRatio);
						point = new Point(SelectionRect.Left + middleVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);

						//頂点右上
						bottomVertexDistance = (int)(SelectionRect.Height * _bottomVertexRatio);
						point = new Point(SelectionRect.Right, SelectionRect.Top + bottomVertexDistance);
						_vertexs.Add(point);

						//頂点右下
						point = new Point(SelectionRect.Right, SelectionRect.Bottom - bottomVertexDistance);
						_vertexs.Add(point);

						//頂点下
						point = new Point(SelectionRect.Left + middleVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);
					}
					else if (IsFlippedVertically == false && IsFlippedHorizontally == true)
					{
						//右中
						point = new Point(SelectionRect.Right, SelectionRect.Top + SelectionRect.Height / 2);
						_vertexs.Add(point);

						//頂点下
						middleVertexDistance = (int)(SelectionRect.Width * _middleVertexRatio);
						point = new Point(SelectionRect.Right - middleVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);

						//頂点左下
						bottomVertexDistance = (int)(SelectionRect.Height * _bottomVertexRatio);
						point = new Point(SelectionRect.Left, SelectionRect.Bottom - bottomVertexDistance);
						_vertexs.Add(point);

						//頂点左上
						point = new Point(SelectionRect.Left, SelectionRect.Top + bottomVertexDistance);
						_vertexs.Add(point);

						//頂点上
						point = new Point(SelectionRect.Right - middleVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);
					}
					else if (IsFlippedVertically == true && IsFlippedHorizontally == true)
					{
						//右中
						point = new Point(SelectionRect.Right, SelectionRect.Top + SelectionRect.Height / 2);
						_vertexs.Add(point);

						//頂点下
						middleVertexDistance = (int)(SelectionRect.Width * _middleVertexRatio);
						point = new Point(SelectionRect.Right - middleVertexDistance, SelectionRect.Bottom);
						_vertexs.Add(point);

						//頂点左下
						bottomVertexDistance = (int)(SelectionRect.Height * _bottomVertexRatio);
						point = new Point(SelectionRect.Left, SelectionRect.Bottom - bottomVertexDistance);
						_vertexs.Add(point);

						//頂点左上
						point = new Point(SelectionRect.Left, SelectionRect.Top + bottomVertexDistance);
						_vertexs.Add(point);

						//頂点上
						point = new Point(SelectionRect.Right - middleVertexDistance, SelectionRect.Top);
						_vertexs.Add(point);
					}
					break;
			}
		}
	}
}
