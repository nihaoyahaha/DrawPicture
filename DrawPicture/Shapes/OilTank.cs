using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawPicture.Shapes
{
	/// <summary>
	/// ドラム缶
	/// </summary>
	public class OilTank : Shape
	{
		private Bitmap _tempCanvas;
		public OilTank(Bitmap bitmap, Panel panel) : base(bitmap, panel) {}

		public override void MouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				_tempCanvas = (Bitmap)canvas.Clone();
				StartPoint = e.Location;
				Color pixelColor = canvas.GetPixel(e.Location.X,e.Location.Y);
				FloodFillScanline(_tempCanvas, e.Location.X, e.Location.Y, pixelColor.ToArgb() /*Color.AliceBlue.ToArgb()*/, ForeColor.ToArgb());
				panel.Invalidate();
			}
			else if (e.Button == MouseButtons.Right)
			{
				_tempCanvas = null;
				panel.Invalidate();
				return;
			}
		}

		public override void MouseMove(MouseEventArgs e){}

		public override void MouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
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
				graphics.DrawImage(canvas, 0, 0);
			}
			if (_tempCanvas != null)
			{
				graphics.DrawImage(_tempCanvas, 0, 0);
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
			using (Graphics g = Graphics.FromImage(canvas))
			{
				g.Clear(color);
			}
			panel.Invalidate();
		}
	}
}
