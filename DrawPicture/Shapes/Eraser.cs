using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace DrawPicture.Shapes
{
	/// <summary>
	/// 消しゴム
	/// </summary>
	public class Eraser : Shape
	{
		Bitmap newCanvas;
		public Eraser(Bitmap canvas, Panel panel) : base(canvas, panel)
		{
			
		}
		public override void MouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				newCanvas = (Bitmap)canvas.Clone();
				EndPoint = e.Location;
				DrawEraserPath(EndPoint,e.Location);
				drawStatus = DrawStatus.Creating;
			}
			else if (drawStatus == DrawStatus.Creating && e.Button == MouseButtons.Right)
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
				DrawEraserPath(EndPoint,e.Location);
				EndPoint = e.Location;
			}
		}

		private void DrawEraserPath(Point start, Point end)
		{
			if (newCanvas == null) return;
			float eraserSize = Size; // 橡皮擦大小
			using (Graphics g = Graphics.FromImage(newCanvas))
			{
				// 计算两点之间的距离
				double distance = Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));

				// 如果距离小于1，则直接绘制一个点
				if (distance < 1)
				{
					g.FillRectangle(new SolidBrush(ForeColor),
						start.X - eraserSize / 2,
						start.Y - eraserSize / 2,
						eraserSize,
						eraserSize);
					return;
				}

				// 插值计算中间点
				for (double t = 0; t <= 1; t += 1 / distance)
				{
					int x = (int)(start.X + t * (end.X - start.X));
					int y = (int)(start.Y + t * (end.Y - start.Y));

					// 绘制橡皮擦区域
					g.FillRectangle(new SolidBrush(ForeColor),
						x - eraserSize / 2,
						y - eraserSize / 2,
						eraserSize,
						eraserSize);
				}
			}

			// 刷新 Panel 显示
			panel.Invalidate();
		}

		public override void MouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (newCanvas != null)
				{
					using (Graphics g = Graphics.FromImage(canvas))
					{
						g.DrawImage(newCanvas, new Point(0, 0)); // 将 newBitmap 叠加到 originalBitmap 上
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
				graphics.DrawImage(canvas, 0, 0);
			}
			if (newCanvas != null)
			{
				graphics.DrawImage(newCanvas,0,0);
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
