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
		private GraphicsPath path = new GraphicsPath(); // 存储鼠标移动路径
		public Eraser(Bitmap canvas, Panel panel) : base(canvas, panel)
		{
			
		}
		public override void MouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				EndPoint = e.Location;
				drawStatus = DrawStatus.Creating;
			}
			else if (drawStatus == DrawStatus.Creating && e.Button == MouseButtons.Right)
			{
				path = new GraphicsPath();
				drawStatus = DrawStatus.CannotMovedOrAdjusted;
				panel.Invalidate();
				return;
			}
		}

		public override void MouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				path.AddRectangle(new Rectangle(e.X,e.Y,10,10));
			
				EndPoint = e.Location;

				panel.Invalidate();
			}
		}

		public override void MouseUp(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				MouseLeftButtonUpHandle(e);
			}

		}

		private void MouseLeftButtonUpHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.CannotMovedOrAdjusted) return;
			using (Graphics g = Graphics.FromImage(canvas))
			{
				using (Pen pen = new Pen(Color.Blue, 2))
				{
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.PixelOffsetMode = PixelOffsetMode.HighQuality;
					g.DrawPath(pen, path);
				}
			}
			EndPoint = new Point();
			path = new GraphicsPath();
			panel.Invalidate();
		}

		public override void InPainting(Graphics graphics)
		{
			if (canvas != null)
			{
				graphics.DrawImage(canvas, 0, 0);
			}
			//using (Pen pen = new Pen(Color.Blue, 2))
			//{
			//	graphics.DrawPath(pen, path);
			//}
			using (Brush brush = new SolidBrush(Color.Blue))
			{
				graphics.FillEllipse(brush, new Rectangle(EndPoint.X,EndPoint.Y,10,10));
			}
		}

	}
}
