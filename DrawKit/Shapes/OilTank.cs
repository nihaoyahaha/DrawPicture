using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawKit.Shapes
{
	/// <summary>
	/// カラーフィル
	/// </summary>
	public class OilTank : Shape
	{
		private Bitmap _tempCanvas;
		public OilTank(Bitmap bitmap, Panel panel) : base(bitmap, panel) {}

		public override void MouseDown(MouseEventArgs e)
		{
			if (!IsValidLocation(e.Location) && drawStatus != DrawStatus.CanvasAdjustable) return;
			if (e.Button == MouseButtons.Left)
			{
				MouseLeftButtonDownHandle(e);
			}
			else if (e.Button == MouseButtons.Right)
			{
				_tempCanvas = null;
				panel.Invalidate();
				return;
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
				_tempCanvas = (Bitmap)canvas.Clone();
				StartPoint = e.Location;
				Point pointIncanvas = ConvertPoint(e.Location);
				Color pixelColor = canvas.GetPixel(pointIncanvas.X, pointIncanvas.Y);
				FloodFillScanline(_tempCanvas, pointIncanvas.X, pointIncanvas.Y, pixelColor.ToArgb() , ForeColor.ToArgb());
				panel.Invalidate();
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
				if (_tempCanvas != null)
				{
					using (Graphics g = Graphics.FromImage(canvas))
					{
						g.DrawImage(_tempCanvas, new Point(0, 0));
					}
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
				//graphics.DrawImage(_tempCanvas, 0, 0);
			}
			if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				DrawCanvasAdjusted(graphics);
			}
		}

		/// <summary>
		/// 洪水填充算法，扫描线填充
		/// </summary>
		/// <param name="bmp">目标位图</param>
		/// <param name="x">起始点的 X 坐标</param>
		/// <param name="y">起始点的 Y 坐标</param>
		/// <param name="targetColor">需要替换的目标颜色（ARGB 格式）</param>
		/// <param name="replacementColor">新颜色（ARGB 格式）</param>
		unsafe void FloodFillScanline(Bitmap bmp, int x, int y, int targetColor, int replacementColor)
		{
			Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
			BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			try
			{
				byte* ptr = (byte*)bmpData.Scan0;
				int stride = bmpData.Stride;

				int width = bmp.Width;
				int height = bmp.Height;

				Queue<(int, int)> queue = new Queue<(int, int)>();
				queue.Enqueue((x, y));

				while (queue.Count > 0)
				{
					var (startX, startY) = queue.Dequeue();
					int currentLineIndex = startY * stride + startX * 4;

					// 向左填充
					int leftX = startX;
					while (leftX >= 0 && *(int*)(ptr + (startY * stride + leftX * 4)) == targetColor)
					{
						*(int*)(ptr + (startY * stride + leftX * 4)) = replacementColor;
						leftX--;
					}

					// 向右填充
					int rightX = startX + 1;
					while (rightX < width && *(int*)(ptr + (startY * stride + rightX * 4)) == targetColor)
					{
						*(int*)(ptr + (startY * stride + rightX * 4)) = replacementColor;
						rightX++;
					}

					// 检查上下两行是否有需要填充的像素
					if (startY > 0)
					{
						for (int i = leftX + 1; i < rightX; i++)
						{
							if (*(int*)(ptr + ((startY - 1) * stride + i * 4)) == targetColor)
							{
								queue.Enqueue((i, startY - 1));
							}
						}
					}

					if (startY < height - 1)
					{
						for (int i = leftX + 1; i < rightX; i++)
						{
							if (*(int*)(ptr + ((startY + 1) * stride + i * 4)) == targetColor)
							{
								queue.Enqueue((i, startY + 1));
							}
						}
					}
				}
			}
			finally
			{
				bmp.UnlockBits(bmpData);
			}
		}

		public override void Rotate(float angle){}

		public override void FlipHorizontal(){}

		public override void FlipVertical(){}

		public override void Clear(Color color)
		{
			ClearBitmap(color);
		}
	}
}
