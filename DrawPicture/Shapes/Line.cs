using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawPicture.Shapes
{
	public class Line : Shape
	{
		private int _handleSize = 15; // 控制点矩形的大小
		private bool _startPointSelected = false;
		private Point _offset = new Point();
		public Line(Bitmap canvas, Panel panel) : base(canvas,panel)
		{
			ForeColor = Color.Black;
		}

		public override void MouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				MouseMoveLeftButtonHandle(e);
			}
			else if (e.Button == MouseButtons.Right)
			{

			}
			else
			{
				_startPointSelected = false;
				drawStatus = DrawStatus.CannotMovedOrAdjusted;
				panel.Cursor = Cursors.Default;
				if (StartPoint.X == 0 && StartPoint.Y == 0) return;
				if (EndPoint.X == 0 && EndPoint.Y == 0) return; 
				IsMouseOnHandle(e.Location, StartPoint, EndPoint);
				if (GetPointIsInLine(e.Location, StartPoint, EndPoint, 10))
				{
					panel.Cursor = Cursors.SizeAll;
					drawStatus = DrawStatus.CanMove;
				}
			}
		}

		private void MouseMoveLeftButtonHandle(MouseEventArgs e)
		{   
			if (drawStatus == DrawStatus.Creating)
			{
				EndPoint = e.Location;
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.CanMove)
			{
				int deltaX = e.X - _offset.X;
				int deltaY = e.Y - _offset.Y;
				StartPoint = new Point(StartPoint.X+deltaX,StartPoint.Y+deltaY);
				EndPoint = new Point(EndPoint.X+deltaX,EndPoint.Y+deltaY);
				_offset = new Point(e.X, e.Y);
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.CanAdjusted)
			{
				EndPoint = e.Location;
				panel.Invalidate();
			}
		}

		public override void MouseDown(MouseEventArgs e)
		{
			if ((drawStatus == DrawStatus.Creating || drawStatus == DrawStatus.CanAdjusted || drawStatus == DrawStatus.CanMove)
				&& e.Button == MouseButtons.Right)
			{
				drawStatus = DrawStatus.CannotMovedOrAdjusted;
				EndPoint = new Point();
				panel.Invalidate();
				return;
			}
			MouseDownHandle(e);
		}

		private void MouseDownHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.CannotMovedOrAdjusted)
			{
				BitmapDrawLine();
				StartPoint = new Point(e.X, e.Y);
				drawStatus = DrawStatus.Creating;
			}
			else if (drawStatus == DrawStatus.CanMove)
			{
				_offset = new Point(e.X, e.Y);
			}
			else if (drawStatus == DrawStatus.CanAdjusted)
			{
				if (_startPointSelected)
				{
					Point temp = StartPoint;
					StartPoint = EndPoint;
					EndPoint = EndPoint;
				}
			}

		}

		private void BitmapDrawLine()
		{
			if (EndPoint.X == 0 && EndPoint.Y == 0) return;
			using (Graphics g = Graphics.FromImage(canvas))
			{
				using (Pen pen = new Pen(ForeColor, Size) { DashStyle = DashStyle.Solid, StartCap = LineCap.Round, EndCap = LineCap.Round })
				{
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.PixelOffsetMode = PixelOffsetMode.HighQuality;
					g.DrawLine(pen, StartPoint, EndPoint);
				}
			}
			EndPoint = new Point();
			// 触发重绘以更新显示
			panel.Invalidate();
		}


		//在 Bitmap 上绘制形状
		public override void MouseUp(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.Creating && EndPoint.X == 0 && EndPoint.Y == 0) 
			{ 
				drawStatus = DrawStatus.CannotMovedOrAdjusted;
				return; 
			}
			
			if (e.Button == MouseButtons.Left)
			{
				MouseLeftButtonUpHandle(e);
			}
			else if (e.Button == MouseButtons.Right)
			{

			}
			panel.Invalidate();
		}

		private void MouseLeftButtonUpHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.Creating)
			{
				drawStatus = DrawStatus.CanAdjusted;
			}
			
		}

		//绘图中描绘
		public override void InPainting(Graphics graphics)
		{
			//将位图缓存绘制到panel上
			if (canvas != null)
			{
				graphics.DrawImage(canvas, 0, 0);
			}
			if (drawStatus == DrawStatus.Creating)
			{
				if (EndPoint.X == 0 && EndPoint.Y == 0) return;
				DrawCreating(graphics);
			}
			else if (drawStatus == DrawStatus.CanMove || drawStatus == DrawStatus.CanAdjusted || drawStatus == DrawStatus.AdjustTheStyle)
			{
				DrawMoveOrAdjusted(graphics);
			}
		}

		private void DrawCreating(Graphics graphics)
		{
			using (Pen pen = new Pen(ForeColor, Size) { DashStyle = DashStyle.Solid, StartCap = LineCap.Round, EndCap = LineCap.Round })
			{
				graphics.DrawLine(pen, StartPoint, EndPoint);
			}
		}

		public void DrawMoveOrAdjusted(Graphics graphics)
		{
			using (Pen pen = new Pen(ForeColor, Size) { DashStyle = DashStyle.Solid, StartCap = LineCap.Round, EndCap = LineCap.Round })
			{
				graphics.DrawLine(pen, StartPoint, EndPoint);
			}
			DrawHandle(graphics, StartPoint, EndPoint);
		}

		private void DrawHandle(Graphics g, Point startPoint ,Point endPoint)
		{
			// 计算小矩形的位置
			Rectangle rect = new Rectangle(
				startPoint.X - _handleSize / 2,
				startPoint.Y - _handleSize / 2,
				_handleSize,
				_handleSize
			);
			g.DrawEllipse(Pens.Black, rect); // 绘制矩形边框

			rect = new Rectangle(
				endPoint.X - _handleSize / 2,
				endPoint.Y - _handleSize / 2,
				_handleSize,
				_handleSize
			);
			g.DrawEllipse(Pens.Black, rect); // 绘制矩形边框
		}

		private void IsMouseOnHandle(Point mouseLocation ,Point startPoint, Point endPoint)
		{
			// 判断鼠标是否在控制点范围内
			Rectangle rect = new Rectangle(
				startPoint.X - _handleSize / 2,
				startPoint.Y - _handleSize / 2,
				_handleSize,
				_handleSize
			);
			if (rect.Contains(mouseLocation))
			{
				drawStatus = DrawStatus.CanAdjusted;
				_startPointSelected = true;
				panel.Cursor = Cursors.SizeNS;
			}
			rect = new Rectangle(
				endPoint.X - _handleSize / 2,
				endPoint.Y - _handleSize / 2,
				_handleSize,
				_handleSize
			);
			if (rect.Contains(mouseLocation))
			{
				drawStatus = DrawStatus.CanAdjusted;
				panel.Cursor = Cursors.SizeNS;
			}

		}

	}
}
