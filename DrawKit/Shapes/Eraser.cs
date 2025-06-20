using DrawKit.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace DrawKit.Shapes
{
	/// <summary>
	/// 橡皮擦
	/// </summary>
	public class Eraser : Shape
	{
		private Point _cursorLocation;
		private Bitmap _eraserBitmap;
		public Eraser() { }
		public Eraser(Bitmap canvas, Panel panel, float scale) : base(canvas, panel, scale) { }
		public override void MouseDown(MouseEventArgs e)
		{
			if (!IsValidLocation(e.Location) && drawStatus != DrawStatus.CanvasAdjustable) return;
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
				tempCanvas = GetTempCanvas();
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
				tempCanvas?.Dispose();
				tempCanvas = null;
				_eraserBitmap = null;
				panel.Invalidate();
				return;
			}
		}

		public override void MouseMove(MouseEventArgs e)
		{
			_cursorLocation = e.Location;
			if (e.Button == MouseButtons.Left)
			{
				MouseMoveLeftButtonHandle(e);
			}
			else if (e.Button == MouseButtons.None)
			{
				if (IsValidLocation(e.Location))
				{
					var assembly = Assembly.GetExecutingAssembly();
					drawStatus = DrawStatus.CannotMovedOrAdjusted;
					panel.Cursor = new Cursor(assembly.GetManifestResourceStream("DrawKit.Cursors.Cursor_null.cur")); //Cursors.Default;
				}
				else
				{
					panel.Cursor = Cursors.Default;
				}
				MouseOverResizeHandle(e.Location);
				panel.Invalidate();
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
			if (tempCanvas == null) return;
			float eraserSize = Size;

			using (Graphics g = Graphics.FromImage(tempCanvas))
			{
				double distance = Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));

				if (distance < 1)
				{
					g.FillRectangle(new SolidBrush(Color.White/*ForeColor*/),
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

					g.FillRectangle(new SolidBrush(Color.White/*ForeColor*/),
						x - eraserSize / 2,
						y - eraserSize / 2,
						eraserSize,
						eraserSize);
				}
			}
			panel.Invalidate();
		}

		private void DrawCursor(Graphics graphics)
		{
			var point = ConvertPoint(_cursorLocation);
			_eraserBitmap?.Dispose();
			_eraserBitmap = null;
			_eraserBitmap = tempCanvas == null ? (Bitmap)canvas.Clone() : (Bitmap)tempCanvas.Clone();
			using (Graphics g = Graphics.FromImage(_eraserBitmap))
			{
				g.FillRectangle(new SolidBrush(Color.White),
					point.X - Size / 2,
					point.Y - Size / 2,
					Size,
					Size);
				using (Pen selectionPen = new Pen(Color.Black, 1))
				{
					g.DrawRectangle(selectionPen,
					point.X - Size / 2,
					point.Y - Size / 2,
					Size - 1,
					Size - 1);
				}
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
			if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				drawStatus = DrawStatus.CompleteCanvasAdjustment;
				panel.Invalidate();
			}
			else
			{
				DrawTempCanvasOnMain();
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
			DrawCursor(graphics);
			if (_eraserBitmap != null)
			{
				BitmapDrawShape(_eraserBitmap, graphics);
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

		public override void CommitCurrentShape() { }

		public override void RotateRight() { }

		public override void RotateLeft() { }

		public override void Rotate180() { }

		public override void FlipHorizontal() { }

		public override void FlipVertical() { }

	}
}
