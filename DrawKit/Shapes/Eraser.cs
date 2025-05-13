using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace DrawKit.Shapes
{
	/// <summary>
	/// 消しゴム
	/// </summary>
	public class Eraser : Shape
	{
		Bitmap newCanvas;

		public Eraser() { }
		public Eraser(Bitmap canvas, Panel panel,float scale) : base(canvas, panel,scale) {}
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
			if (drawStatus == DrawStatus.CanvasAdjustable)
			{
				AdjustingCanvasRect = GetCanvasRegion();
				Offset = e.Location;
				drawStatus = DrawStatus.CanvasAdjusting;
			}
			else
			{
				newCanvas = (Bitmap)canvas.Clone();
				EndPoint = e.Location;
				DrawEraserPath(ConvertPoint(EndPoint), ConvertPoint(e.Location));
				drawStatus = DrawStatus.Creating;
			}
		}

		private void MouseRightButtonDownHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.Creating ||
				drawStatus == DrawStatus.CanvasAdjusting)
			{
				drawStatus = DrawStatus.CannotMovedOrAdjusted;
				newCanvas = null;
				panel.Invalidate();
				return;
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
			if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionAdjusting(deltaX, deltaY, ref AdjustingCanvasRect);
				Offset = e.Location;
				panel.Invalidate();
			}
			else
			{
				DrawEraserPath(ConvertPoint(EndPoint), ConvertPoint(e.Location));
				EndPoint = e.Location;
			}
		}

		private void DrawEraserPath(Point start, Point end)
		{
			if (newCanvas == null) return;
			float eraserSize = Size; 
			using (Graphics g = Graphics.FromImage(newCanvas))
			{
				double distance = Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));

				if (distance < 1)
				{
					g.FillRectangle(new SolidBrush(ForeColor),
						start.X - eraserSize / 2,
						start.Y - eraserSize / 2,
						eraserSize,
						eraserSize);
					return;
				}

				for (double t = 0; t <= 1; t += 1 / distance)
				{
					int x = (int)(start.X + t * (end.X - start.X));
					int y = (int)(start.Y + t * (end.Y - start.Y));

					g.FillRectangle(new SolidBrush(ForeColor),
						x - eraserSize / 2,
						y - eraserSize / 2,
						eraserSize,
						eraserSize);
				}
			}
			panel.Invalidate();
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
			if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				drawStatus = DrawStatus.CompleteCanvasAdjustment;
				panel.Invalidate();
			}
			else
			{
				if (newCanvas != null)
				{
					using (Graphics g = Graphics.FromImage(canvas))
					{
						g.DrawImage(newCanvas, new Point(0, 0));
					}
					newCanvas = null;
				}
				panel.Invalidate();
			}
		}

		public override void InPainting(Graphics graphics)
		{
			if (canvas != null)
			{
				BitmapDrawShape(canvas, graphics);
			}
			if (newCanvas != null)
			{
				BitmapDrawShape(newCanvas, graphics);
				//graphics.DrawImage(newCanvas,0,0);
			}
		    if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				DrawCanvasAdjusted(graphics);
			}
		}

		public override void Clear(Color color)
		{
			ClearBitmap(color);
		}

		public override void CommitCurrentShape(){}

		public override void RotateRight(){}

		public override void RotateLeft(){}

		public override void Rotate180(){}

		public override void FlipHorizontal() { }

		public override void FlipVertical() { }
	}
}
