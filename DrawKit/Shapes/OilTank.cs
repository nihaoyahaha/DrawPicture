using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrawKit.History;

namespace DrawKit.Shapes
{
	/// <summary>
	/// 油漆桶颜色填充
	/// </summary>
	public class OilTank : Shape
	{
		private bool isFilling = false;
		private Bitmap _tempCanvas;
		public OilTank() { }
		public OilTank(Bitmap bitmap, Panel panel,float scale) : base(bitmap, panel, scale) {}

		public override void MouseDown(MouseEventArgs e)
		{  
			if (e.Button == MouseButtons.Right)
			{
				_tempCanvas?.Dispose();
				_tempCanvas = null;
				CancelDrawing();
			}
			if (!IsValidLocation(e.Location) && drawStatus != DrawStatus.CanvasAdjustable) return;
			if (e.Button == MouseButtons.Left)
			{
				MouseLeftButtonDownHandle(e);
			}
		}
		
		private  void MouseLeftButtonDownHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.CanvasAdjustable)
			{
				AdjustingCanvasRect = GetCanvasRegion();
				Offset = e.Location;
				drawStatus = DrawStatus.CanvasAdjusting;
			}
			else
			{
				if (isFilling) return;
				_tempCanvas?.Dispose();
				_tempCanvas = null;
				_tempCanvas = (Bitmap)canvas.Clone();
				StartPoint = e.Location;
				Point pointIncanvas = ConvertPoint(e.Location);
				isFilling = true;
				try
				{
					Color replacementColor = ForeColor;  
					SeedFillNonRecursive(Color.Black, pointIncanvas, replacementColor, _tempCanvas);
					panel.Invalidate(); 
				}
				finally
				{
					isFilling = false; 
				}
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
				OperationStep.PushRevokeStack(canvas);
				if (_tempCanvas != null)
				{
					using (Graphics g = Graphics.FromImage(canvas))
					{
						g.DrawImage(_tempCanvas, new Point(0, 0));
					}
					_tempCanvas?.Dispose();
					_tempCanvas = null;
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
			if (_tempCanvas != null)
			{
				BitmapDrawShape(_tempCanvas,graphics);
			}
			if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				DrawCanvasAdjusted(graphics);
			}
		}

		private static void SeedFillNonRecursive(Color boundColor, Point seedPoint, Color fillColor, Bitmap bitmap)
		{
			Color targetColor = bitmap.GetPixel(seedPoint.X, seedPoint.Y);
			if (targetColor == fillColor || targetColor == boundColor) return;

			int width = bitmap.Width;
			int height = bitmap.Height;

			Stack<Point> stack = new Stack<Point>();
			bool[,] visited = new bool[width, height];

			stack.Push(seedPoint);

			while (stack.Count > 0)
			{
				Point p = stack.Pop();

				if (p.X < 0 || p.Y < 0 || p.X >= width || p.Y >= height || visited[p.X, p.Y])
					continue;

				visited[p.X, p.Y] = true;

				if (bitmap.GetPixel(p.X, p.Y) != targetColor)
					continue;

				bitmap.SetPixel(p.X, p.Y, fillColor);

				stack.Push(new Point(p.X - 1, p.Y));
				stack.Push(new Point(p.X + 1, p.Y));
				stack.Push(new Point(p.X, p.Y - 1));
				stack.Push(new Point(p.X, p.Y + 1));
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
