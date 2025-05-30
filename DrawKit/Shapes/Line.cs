﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawKit.Shapes
{
	/// <summary>
	/// 直線
	/// </summary>
	public class Line : Shape
	{
		private int _handleSize = 15; // 控制点矩形的大小
		private bool _startPointSelected = false;
		public Line(Bitmap canvas, Panel panel,float scale) : base(canvas,panel, scale){}

		public Line() { }

		private void BitmapDrawLine()
		{
			if (EndPoint.X == 0 && EndPoint.Y == 0) return;
			//using (Graphics g = Graphics.FromImage(canvas))
			//{
			//	using (Pen pen = new Pen(ForeColor, Size) { DashStyle = DashStyle.Solid, StartCap = LineCap.Round, EndCap = LineCap.Round })
			//	{
			//		g.CompositingQuality = CompositingQuality.HighQuality;
			//		g.InterpolationMode = InterpolationMode.NearestNeighbor;
			//		g.SmoothingMode = SmoothingMode.None;
			//		g.DrawLine(pen,ConvertPoint(StartPoint),ConvertPoint(EndPoint));
			//	}
			//}

			DrawTempCanvasOnMain();

			EndPoint = new Point();
			SelectionRect = Rectangle.Empty;
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
				BitmapDrawLine();
				StartPoint = e.Location;
				drawStatus = DrawStatus.Creating;
			}
			else if (drawStatus == DrawStatus.CanMove)
			{
				Offset = e.Location;
			}
			else if (drawStatus == DrawStatus.CanAdjusted)
			{
				if (_startPointSelected)
				{
					Point temp = StartPoint;
					StartPoint = EndPoint;
					EndPoint = EndPoint;
				}
				drawStatus = DrawStatus.Adjusting;
			}
			else if (drawStatus == DrawStatus.CanvasAdjustable)
			{
				BitmapDrawLine();
				AdjustingCanvasRect = GetCanvasRegion();
				Offset = e.Location;
				drawStatus = DrawStatus.CanvasAdjusting;
			}

		}
		private void MouseRightButtonDownHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.Creating || 
				drawStatus == DrawStatus.Adjusting || 
				drawStatus == DrawStatus.CanMove ||
				drawStatus == DrawStatus.CanvasAdjusting)
			{
				EndPoint = new Point();
				CancelDrawing();
				return;
			}
		}

		public override void MouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				MouseMoveLeftButtonHandle(e);
			}
			else if(e.Button == MouseButtons.None)
			{
				_startPointSelected = false;
				drawStatus = DrawStatus.CannotMovedOrAdjusted;
				panel.Cursor = Cursors.Default;
				
				IsMouseOnHandle(e.Location, StartPoint, EndPoint);
				if (GetPointIsInLine(e.Location, StartPoint, EndPoint, 10))
				{
					panel.Cursor = Cursors.SizeAll;
					drawStatus = DrawStatus.CanMove;
				}
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
			else if (drawStatus == DrawStatus.CanMove)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				StartPoint = new Point(StartPoint.X+deltaX,StartPoint.Y+deltaY);
				EndPoint = new Point(EndPoint.X+deltaX,EndPoint.Y+deltaY);
				Offset = new Point(e.X, e.Y);
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Adjusting)
			{
				EndPoint = e.Location;
				int x = Math.Min(StartPoint.X, EndPoint.X);
				int y = Math.Min(StartPoint.Y, EndPoint.Y);
				int width = Math.Abs(StartPoint.X - EndPoint.X);
				int height = Math.Abs(StartPoint.Y - EndPoint.Y);
				SelectionRect = new Rectangle(x, y, width, height);
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
			if (drawStatus == DrawStatus.Creating && EndPoint.X == 0 && EndPoint.Y == 0) 
			{ 
				drawStatus = DrawStatus.CannotMovedOrAdjusted;
				StartPoint = new Point();
				return; 
			}
			if (e.Button == MouseButtons.Left)
			{
				MouseLeftButtonUpHandle(e);
			}
		}

		private void MouseLeftButtonUpHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.Creating)
			{
				drawStatus = DrawStatus.CanAdjusted;
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
				if (EndPoint.X == 0 && EndPoint.Y == 0) return;
				DrawCreating(graphics);
			}
			else if (drawStatus == DrawStatus.CanMove || 
				drawStatus == DrawStatus.CanAdjusted ||
				drawStatus == DrawStatus.Adjusting || 
				drawStatus == DrawStatus.AdjustTheStyle)
			{
				if (EndPoint.X == 0 && EndPoint.Y == 0) return;
				DrawMoveOrAdjusted(graphics);
			}
			else if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				DrawCanvasAdjusted(graphics);
			}
		}

		private void DrawCreating(Graphics graphics)
		{
			//using (Pen pen = new Pen(ForeColor, Size * Scale) { DashStyle = DashStyle.Solid, StartCap = LineCap.Round, EndCap = LineCap.Round })
			//{
			//	Rectangle bitmapArea = GetCanvasRegion();
			//	graphics.SetClip(bitmapArea);
			//	graphics.DrawLine(pen, StartPoint, EndPoint);
			//	graphics.ResetClip();
			//}

			tempCanvas = (Bitmap)canvas.Clone();
			using (Graphics g = Graphics.FromImage(tempCanvas))
			{
				using (Pen pen = new Pen(ForeColor, Size))
				{
					g.DrawLine(pen, ConvertPoint(StartPoint), ConvertPoint(EndPoint));
				}
			}
		}

		private void DrawMoveOrAdjusted(Graphics graphics)
		{
			//using (Pen pen = new Pen(ForeColor, Size * Scale) { DashStyle = DashStyle.Solid, StartCap = LineCap.Round, EndCap = LineCap.Round })
			//{
			//	Rectangle bitmapArea = GetCanvasRegion();
			//	graphics.SetClip(bitmapArea);
			//	graphics.DrawLine(pen, StartPoint, EndPoint);
			//	graphics.ResetClip();
			//}

			tempCanvas = (Bitmap)canvas.Clone();
			using (Graphics g = Graphics.FromImage(tempCanvas))
			{
				using (Pen pen = new Pen(ForeColor, Size))
				{
					g.DrawLine(pen, ConvertPoint(StartPoint), ConvertPoint(EndPoint));
				}
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
			g.FillEllipse(new SolidBrush(ResizerPointColor), rect); // 绘制矩形边框

			rect = new Rectangle(
				endPoint.X - _handleSize / 2,
				endPoint.Y - _handleSize / 2,
				_handleSize,
				_handleSize
			);
			g.FillEllipse(new SolidBrush(ResizerPointColor), rect);// 绘制矩形边框
		}

		private void IsMouseOnHandle(Point mouseLocation ,Point startPoint, Point endPoint)
		{
			if (StartPoint.X == 0 && StartPoint.Y == 0) return;
			if (EndPoint.X == 0 && EndPoint.Y == 0) return;
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

		public override void Clear(Color color)
		{
			StartPoint = new Point();
			EndPoint = new Point();
			ClearBitmap(color);
		}

		public override void CommitCurrentShape()
		{
			BitmapDrawLine();
		}

		public override void RotateRight()
		{
			drawStatus = DrawStatus.CanAdjusted;
			UpdateLinePoints();
		}

		public override void RotateLeft()
		{
			drawStatus = DrawStatus.CanAdjusted;
			UpdateLinePoints();
		}

		public override void Rotate180()
		{
			drawStatus = DrawStatus.CanAdjusted;
		}
		public override void FlipHorizontal() {
			drawStatus = DrawStatus.CanAdjusted;
		}

		public override void FlipVertical() {
			drawStatus = DrawStatus.CanAdjusted;
		}

		private void UpdateLinePoints()
		{
			int centerX = (StartPoint.X + EndPoint.X) / 2;
			int centerY = (StartPoint.Y + EndPoint.Y) / 2;

			int deltaX = EndPoint.X - StartPoint.X;
			int deltaY = EndPoint.Y - StartPoint.Y;

			int rotatedDeltaX = deltaY;
			int rotatedDeltaY = -deltaX;

			StartPoint = new Point(
				centerX - rotatedDeltaX / 2,
				centerY - rotatedDeltaY / 2);

			EndPoint = new Point(
				centerX + rotatedDeltaX / 2,
				centerY + rotatedDeltaY / 2);
		}

	}
}
