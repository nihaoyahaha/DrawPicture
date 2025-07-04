﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawKit.Shapes
{
	/// <summary>
	/// 圆角矩形
	/// </summary>
	public class RoundedRectangle : Shape
	{
		public RoundedRectangle() { }
		public RoundedRectangle(Bitmap bitmap, Panel panel,float scale) : base(bitmap, panel, scale) { }

		//顶点集合
		private GraphicsPath _path = new GraphicsPath();
		private GraphicsPath _pathInBitmap = new GraphicsPath();
		private int _radius = 20;
		private void BitmapDrawRoundedRectangle()
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
			//		g.DrawPath(selectionPen, _pathInBitmap);
			//	}
			//}

			DrawTempCanvasOnMain();

			drawStatus = DrawStatus.CannotMovedOrAdjusted;
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
				if (!IsValidLocation(e.Location) && SelectionRect == Rectangle.Empty) return;
				BitmapDrawRoundedRectangle();
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
				BitmapDrawRoundedRectangle();
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
				BitmapDrawRoundedRectangle();
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
				GetRoundedRectanglePath();
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Moving)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionRect.Offset(deltaX, deltaY);
				Offset = e.Location;
				GetRoundedRectanglePath();
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Adjusting)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionAdjusting(deltaX, deltaY, ref SelectionRect);
				Offset = e.Location;
				GetRoundedRectanglePath();
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
		/// 获取圆角矩形路径
		/// </summary>
		private void GetRoundedRectanglePath()
		{
			_path = new GraphicsPath();
			_pathInBitmap = new GraphicsPath();
			_radius = Math.Min(SelectionRect.Width, SelectionRect.Height) / 2;
			if (_radius < 1)
			{
				_radius = 1;
			}
			_path.AddArc(SelectionRect.Left, SelectionRect.Top, _radius, _radius, 180, 90);
			_path.AddArc(SelectionRect.Right - _radius, SelectionRect.Y, _radius, _radius, 270, 90); // 右上角
			_path.AddArc(SelectionRect.Right - _radius, SelectionRect.Bottom - _radius, _radius, _radius, 0, 90); // 右下角
			_path.AddArc(SelectionRect.X, SelectionRect.Bottom - _radius, _radius, _radius, 90, 90); // 左下角
			_path.CloseFigure(); // 闭合路径

			var rect = ConvertSelectionRectToCanvasRect(SelectionRect);
			_radius = (int)(_radius / Scale);
			if (_radius < 1)
			{
				_radius = 1;
			}
			_pathInBitmap.AddArc(rect.Left, rect.Top, _radius, _radius, 180, 90);
			_pathInBitmap.AddArc(rect.Right - _radius, rect.Y, _radius, _radius, 270, 90); // 右上角
			_pathInBitmap.AddArc(rect.Right - _radius, rect.Bottom - _radius, _radius, _radius, 0, 90); // 右下角
			_pathInBitmap.AddArc(rect.X, rect.Bottom - _radius, _radius, _radius, 90, 90); // 左下角
			_pathInBitmap.CloseFigure(); // 闭合路径
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
					g.DrawPath(selectionPen, _pathInBitmap);
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
					g.DrawPath(selectionPen, _pathInBitmap);
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
			BitmapDrawRoundedRectangle();
		}

		public override void RotateRight()
		{
			drawStatus = DrawStatus.CanAdjusted;
			SelectionRect = RotateRectangle90Degrees();
			GetRoundedRectanglePath();
		}

		public override void RotateLeft()
		{
			drawStatus = DrawStatus.CanAdjusted;
			SelectionRect = RotateRectangle90Degrees();
			GetRoundedRectanglePath();
		}

		public override void Rotate180()
		{
			drawStatus = DrawStatus.CanAdjusted;
			GetRoundedRectanglePath();
		}
		public override void FlipHorizontal()
		{
			drawStatus = DrawStatus.CanAdjusted;
			GetRoundedRectanglePath();
		}
		public override void FlipVertical()
		{
			drawStatus = DrawStatus.CanAdjusted;
			GetRoundedRectanglePath();
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
