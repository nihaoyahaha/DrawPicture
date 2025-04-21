using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawPicture.Shapes
{
	public abstract class Shape
	{
		public Bitmap canvas;
		protected Panel panel;
		
		public Shape(Bitmap bitmap, Panel panel)
		{
			canvas = bitmap;
			this.panel = panel;
		}

		//開始点
		protected Point StartPoint { get; set; }

		//終点
		protected Point EndPoint { get; set; }

		//太さ
		public float Size { get; set; } = 8;

		//前景色
		public Color ForeColor { get; set; } = Color.Black;

		//図形描画状態
		public DrawStatus drawStatus { get; set; } = DrawStatus.CannotMovedOrAdjusted;

		//ポイントカラーを編集する
		protected Color ResizerPointColor = Color.FromArgb(104, 139, 204);

		//ポイント寸法を編集するには
		protected float ResizerPointSize = 10;

		//選択範囲
		protected Rectangle SelectionRect = Rectangle.Empty;

		//ポイントタイプを編集する
		protected RectangleShapeFocusType FocusType;

		//オフセットポイント
		protected Point Offset = new Point();

		public abstract void MouseMove(MouseEventArgs e);
		public abstract void MouseDown(MouseEventArgs e);
		public abstract void MouseUp(MouseEventArgs e);
		//描画中
		public abstract void InPainting(Graphics graphics);
		//回転指定角度
		public abstract void Rotate(float angle);
		//水平反転
		public abstract void FlipHorizontal();
		//垂直反転
		public abstract void FlipVertical();

		//クリアランス
		public abstract void Clear(Color color);
		/// <summary>
		/// 点が直線上にあるかどうかを判断する
		/// </summary>
		/// <param name="pf"></param>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="range"></param>
		/// <returns></returns>
		protected bool GetPointIsInLine(Point pf, Point p1, Point p2, double range)
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

		/// <summary>
		/// 矩形編集点に戻る
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		protected IEnumerable<(Point editPoint, RectangleShapeFocusType focusType)> GetResizerPoints(Rectangle rect)
		{
			yield return (new Point(rect.X, rect.Y), RectangleShapeFocusType.TopLeft);
			yield return (new Point(rect.X + rect.Width / 2, rect.Y), RectangleShapeFocusType.TopCenter);
			yield return (new Point(rect.X + rect.Width, rect.Y), RectangleShapeFocusType.TopRight);

			yield return (new Point(rect.X, rect.Y + rect.Height / 2), RectangleShapeFocusType.MiddleLeft);
			yield return (new Point(rect.X + rect.Width, rect.Y + rect.Height / 2), RectangleShapeFocusType.MiddleRight);

			yield return (new Point(rect.X, rect.Y + rect.Height), RectangleShapeFocusType.BottomLeft);
			yield return (new Point(rect.X + rect.Width / 2, rect.Y + rect.Height), RectangleShapeFocusType.BottomCenter);
			yield return (new Point(rect.X + rect.Width, rect.Y + rect.Height), RectangleShapeFocusType.BottomRight);
		}

		/// <summary>
		/// マウスポインタの設定
		/// </summary>
		/// <param name="focusType"></param>
		protected void SetFoucsCursorType(RectangleShapeFocusType focusType)
		{
			switch (focusType)
			{
				case RectangleShapeFocusType.Unfocused:
					panel.Cursor = default;
					break;
				case RectangleShapeFocusType.TopLeft:
					panel.Cursor = Cursors.SizeNWSE;
					break;
				case RectangleShapeFocusType.TopCenter:
					panel.Cursor = Cursors.SizeNS;
					break;
				case RectangleShapeFocusType.TopRight:
					panel.Cursor = Cursors.SizeNESW;
					break;
				case RectangleShapeFocusType.MiddleLeft:
					panel.Cursor = Cursors.SizeWE;
					break;
				case RectangleShapeFocusType.MiddleRight:
					panel.Cursor = Cursors.SizeWE;
					break;
				case RectangleShapeFocusType.BottomLeft:
					panel.Cursor = Cursors.SizeNESW;
					break;
				case RectangleShapeFocusType.BottomCenter:
					panel.Cursor = Cursors.SizeNS;
					break;
				case RectangleShapeFocusType.BottomRight:
					panel.Cursor = Cursors.SizeNWSE;
					break;
				case RectangleShapeFocusType.Move:
					panel.Cursor = Cursors.SizeAll;
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// 長方形選択範囲の調整
		/// </summary>
		/// <param name="horizontalDistance"></param>
		/// <param name="verticalDistance"></param>
		protected void SelectionAdjusting(int horizontalDistance, int verticalDistance)
		{
			int width = SelectionRect.Width;
			int height = SelectionRect.Height;
			switch (FocusType)
			{
				case RectangleShapeFocusType.TopLeft:
					if (width - horizontalDistance <= 2) return;
					if (height - verticalDistance <= 2) return;

					SelectionRect.X += horizontalDistance;
					SelectionRect.Y += verticalDistance;
					SelectionRect.Width -= horizontalDistance;
					SelectionRect.Height -= verticalDistance;
					break;
				case RectangleShapeFocusType.TopCenter:
					if (height - verticalDistance <= 2) return;

					SelectionRect.Y += verticalDistance;
					SelectionRect.Height -= verticalDistance;
					break;
				case RectangleShapeFocusType.TopRight:
					if (width + horizontalDistance <= 2) return;
					if (height - verticalDistance <= 2) return;

					SelectionRect.Y += verticalDistance;
					SelectionRect.Width += horizontalDistance;
					SelectionRect.Height -= verticalDistance;
					break;
				case RectangleShapeFocusType.MiddleLeft:
					if (width - horizontalDistance <= 2) return;

					SelectionRect.X += horizontalDistance;
					SelectionRect.Width -= horizontalDistance;
					break;
				case RectangleShapeFocusType.MiddleRight:
					if (width + horizontalDistance <= 2) return;

					SelectionRect.Width += horizontalDistance;
					break;
				case RectangleShapeFocusType.BottomLeft:
					if (width - horizontalDistance <= 2) return;
					if (height + verticalDistance <= 2) return;

					SelectionRect.X += horizontalDistance;
					SelectionRect.Width -= horizontalDistance;
					SelectionRect.Height += verticalDistance;
					break;
				case RectangleShapeFocusType.BottomCenter:
					if (height + verticalDistance <= 2) return;

					SelectionRect.Height += verticalDistance;
					break;
				case RectangleShapeFocusType.BottomRight:
					if (width + horizontalDistance <= 2) return;
					if (height + verticalDistance <= 2) return;

					SelectionRect.Width += horizontalDistance;
					SelectionRect.Height += verticalDistance;
					break;
			}
		}

		/// <summary>
		/// マウスをストレッチ可能な点に置いたままにしているか
		/// </summary>
		/// <param name="mouseLocation"></param>
		protected void MouseOverResizeHandle(Point mouseLocation)
		{
			if (SelectionRect.Contains(mouseLocation))
			{
				drawStatus = DrawStatus.CanMove;
				panel.Cursor = Cursors.SizeAll;
			}
			foreach (var focusPoint in GetResizerPoints(SelectionRect))
			{
				double distance = Math.Sqrt(Math.Pow(mouseLocation.X - focusPoint.editPoint.X, 2) + Math.Pow(mouseLocation.Y - focusPoint.editPoint.Y, 2));
				if (distance <= Size)
				{
					SetFoucsCursorType(focusPoint.focusType);
					drawStatus = DrawStatus.CanAdjusted;
					FocusType = focusPoint.focusType;
				}
			}
		}

		/// <summary>
		/// bitmapをクリア
		/// </summary>
		/// <param name="color"></param>
		protected void ClearBitmap(Color color)
		{
			using (Graphics g = Graphics.FromImage(canvas))
			{
				g.Clear(color);
			}
			panel.Invalidate();
		}

		/// <summary>
		/// 描画解除
		/// </summary>
		protected virtual void CancelDrawing()
		{
			drawStatus = DrawStatus.CannotMovedOrAdjusted;
			SelectionRect = Rectangle.Empty;
			panel.Invalidate();
		}
	}
}