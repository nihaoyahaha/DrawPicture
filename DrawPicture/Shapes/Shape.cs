using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawPicture.Shapes
{
	public abstract class Shape
	{
		public Bitmap canvas;
		public Panel panel;
		
		public Shape(Bitmap bitmap, Panel panel)
		{
			canvas = bitmap;
			this.panel = panel;
		}
		public MouseStatus mouseStatus { get; set; } = MouseStatus.Move;
		public Point StartPoint { get; set; }
		public Point EndPoint { get; set; }
		public float Size { get; set; } = 1;
		public Color ForeColor { get; set; } = Color.Black;
		public bool IsSelected { get; set; } = false;
		public DrawStatus drawStatus { get; set; } = DrawStatus.Complete;
		public bool CanItMove { get; set; } = false;

		public abstract void MouseMove(MouseEventArgs e);
		public abstract void MouseDown(MouseEventArgs e);
		public abstract void MouseUp(MouseEventArgs e);
		//绘画中
		public abstract void InPainting(Graphics graphics);
		//选中
		public abstract void DrawSelected(Graphics graphics);

		public bool GetPointIsInLine(Point pf, Point p1, PointF p2, double range)
		{
			pf = new Point (int.Parse(Math.Round((double)pf.X / 1, 0).ToString()), int.Parse(Math.Round((double)pf.Y / 1, 0).ToString()));
			double cross = (p2.X - p1.X) * (pf.X - p1.X) + (p2.Y - p1.Y) * (pf.Y - p1.Y);
			if (cross <= 0) return false;
			double d2 = (p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y);
			if (cross >= d2) return false;

			double r = cross / d2;
			double px = p1.X + (p2.X - p1.X) * r;
			double py = p1.Y + (p2.Y - p1.Y) * r;

			return Math.Sqrt((pf.X - px) * (pf.X - px) + (py - pf.Y) * (py - pf.Y)) <= range;
		}

	}
}