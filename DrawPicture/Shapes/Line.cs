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
		private int handleSize = 15; // 控制点矩形的大小

		public Line(Bitmap canvas, Panel panel) : base(canvas,panel)
		{

		}

		public override void MouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				mouseStatus = MouseStatus.LeftButtonPressMove;
				EndPoint = e.Location;
			}
			else if (e.Button == MouseButtons.Right)
			{
				mouseStatus = MouseStatus.RightButtonPressMove;
			}
			else
			{
				mouseStatus = MouseStatus.Move;
				if (drawStatus == DrawStatus.CanAdjusted)
				{
					
				}
			}
		}

		public override void MouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				StartPoint = new Point(e.X, e.Y);
				mouseStatus = MouseStatus.LeftButtonDown;
			}
			else if (e.Button == MouseButtons.Right)
			{
				mouseStatus = MouseStatus.RightButtonDown;
			}
		}

		//在 Bitmap 上绘制形状
		public override void MouseUp(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.Complete && EndPoint.X == 0 && EndPoint.Y == 0) return;
			if (e.Button == MouseButtons.Left)
			{
				mouseStatus = MouseStatus.LeftButtonUp;
				drawStatus = DrawStatus.CanAdjusted;
			}
			else if (e.Button == MouseButtons.Right)
			{
				mouseStatus = MouseStatus.RightButtonUp;
				
			}

			


			
			panel.Invalidate();

			//if (mouseStatus == MouseStatus.LeftButtonUp)
			//{
			//	// 获取 Graphics 对象
			//	using (Graphics g = Graphics.FromImage(canvas))
			//	{
			//		using (Pen pen = new Pen(ForeColor, Size){ DashStyle = DashStyle.Solid ,StartCap = LineCap.Round, EndCap = LineCap.Round})
			//		{
			//			g.SmoothingMode = SmoothingMode.HighQuality;
			//			g.PixelOffsetMode = PixelOffsetMode.HighQuality;
			//			g.DrawLine(pen, StartPoint, EndPoint);
			//		}
			//	}
			//	EndPoint = new Point();
			//	// 触发重绘以更新显示
			//	panel.Invalidate();
			//}
		}

		
		//绘图中描绘
		public override void InPainting(Graphics graphics)
		{
			//将位图缓存绘制到panel上
			if (canvas != null)
			{
				graphics.DrawImage(canvas, 0, 0);
			}

			if (mouseStatus == MouseStatus.LeftButtonPressMove)
			{
				using (Pen pen = new Pen(ForeColor, Size) { DashStyle = DashStyle.Solid ,StartCap = LineCap.Round, EndCap = LineCap.Round})
				{
					graphics.DrawLine(pen, StartPoint, EndPoint);
				}
			}
		}

		public override void DrawSelected(Graphics graphics)
		{
			using (Pen pen = new Pen(ForeColor, Size) { DashStyle = DashStyle.Solid, StartCap = LineCap.Round, EndCap = LineCap.Round })
			{
				graphics.DrawLine(pen, StartPoint, EndPoint);
			}
			DrawHandle(graphics, StartPoint);
			DrawHandle(graphics,EndPoint);
		}
		private void DrawHandle(Graphics g, Point point)
		{
			// 计算小矩形的位置
			Rectangle rect = new Rectangle(
				point.X - handleSize / 2,
				point.Y - handleSize / 2,
				handleSize,
				handleSize
			);
			g.DrawEllipse(Pens.Black, rect); // 绘制矩形边框
		}

		private void IsMouseOnHandle(Point mouseLocation, Point handleCenter)
		{
			// 判断鼠标是否在控制点范围内
			Rectangle rect = new Rectangle(
				handleCenter.X - handleSize / 2,
				handleCenter.Y - handleSize / 2,
				handleSize,
				handleSize
			);
			if (rect.Contains(mouseLocation)) drawStatus = DrawStatus.CanAdjusted;
		}

	}
}
