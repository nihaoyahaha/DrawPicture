using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawKit.Shapes
{
	public abstract class Shape
	{
		public Shape() { }

		public Shape(Bitmap bitmap, Panel panel,float scale=1f)
		{
			canvas = bitmap;
			this.panel = panel;
			Scale = scale;
		}

		//キャンバス調整点コレクション
		private List<(Rectangle rect, RectangleShapeFocusType focusType)> _canvasEditPoints = new List<(Rectangle rect, RectangleShapeFocusType focusType)>();

		protected Panel panel;
		//開始点
		protected Point StartPoint { get; set; }

		//終点
		protected Point EndPoint { get; set; }

		//ポイントカラーを編集する
		protected Color ResizerPointColor = Color.FromArgb(104, 139, 204);

		//ポイント寸法を編集するには
		protected float ResizerPointSize = 10;

		//ポイントタイプを編集する
		protected RectangleShapeFocusType FocusType;

		//オフセットポイント
		protected Point Offset = new Point();

		//時計回りの回転数を記録する
		protected int RotationCount = 0;

		//水平反転するかどうか
		protected bool IsFlippedHorizontally = false;

		//垂直反転するかどうか
		protected bool IsFlippedVertically = false;

		public Bitmap canvas;
		//ズーム倍率
		public float Scale { get; set; }

		//選択範囲
		public Rectangle SelectionRect = Rectangle.Empty;

		//調整中のbitmap範囲
		public Rectangle AdjustingCanvasRect = Rectangle.Empty;

		//太さ
		public float Size { get; set; } = 8;

		//前景色
		public Color ForeColor { get; set; } = Color.Black;

		//図形描画状態
		public DrawStatus drawStatus { get; set; } = DrawStatus.CannotMovedOrAdjusted;

		//ビットマップストレッチオフセットポイント
		public Point BitmapStretchOffsetPoint = Point.Empty;

		public abstract void MouseMove(MouseEventArgs e);
		public abstract void MouseDown(MouseEventArgs e);
		public abstract void MouseUp(MouseEventArgs e);
		//描画中
		public abstract void InPainting(Graphics graphics);

		//編集状態の図を保存
		public abstract void CommitCurrentShape();

		//インスタンスの作成
		public T InitializeShape<T>() where T : Shape, new()
		{
			T t = new T
			{
				canvas = canvas,
				panel = panel,
				Scale = Scale,
				ForeColor = ForeColor,
				Size = Size
			};
			return t;
		}

		//右に90度回転
		public abstract void RotateRight();

		//左に90度回転
		public abstract void RotateLeft();

		//180度回転
		public abstract void Rotate180();

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
			if (StartPoint.X == 0 && StartPoint.Y == 0) return false;
			if (EndPoint.X == 0 && EndPoint.Y == 0) return false;
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
		protected void SelectionAdjusting(int horizontalDistance, int verticalDistance,ref Rectangle rect)
		{
			int width = rect.Width;
			int height = rect.Height;
			var canvasRegion = GetCanvasRegion();
			switch (FocusType)
			{
				case RectangleShapeFocusType.TopLeft:
					if (width - horizontalDistance <= 2) return;
					if (height - verticalDistance <= 2) return;

					rect.X += horizontalDistance;
					rect.Y += verticalDistance;
					rect.Width -= horizontalDistance;
					rect.Height -= verticalDistance;

					//BitmapStretchOffsetPoint = new Point(rect.Width-canvas.Width, rect.Height-canvas.Height);
					BitmapStretchOffsetPoint = new Point(rect.Width - canvasRegion.Width, rect.Height - canvasRegion.Height);
					break;
				case RectangleShapeFocusType.TopCenter:
					if (height - verticalDistance <= 2) return;

					rect.Y += verticalDistance;
					rect.Height -= verticalDistance;

					//BitmapStretchOffsetPoint = new Point(0, rect.Height-canvas.Height);
					BitmapStretchOffsetPoint = new Point(0, rect.Height - canvasRegion.Height);
					break;
				case RectangleShapeFocusType.TopRight:
					if (width + horizontalDistance <= 2) return;
					if (height - verticalDistance <= 2) return;

					rect.Y += verticalDistance;
					rect.Width += horizontalDistance;
					rect.Height -= verticalDistance;

					//BitmapStretchOffsetPoint = new Point(0, rect.Height-canvas.Height);
					BitmapStretchOffsetPoint = new Point(0, rect.Height - canvasRegion.Height);
					break;
				case RectangleShapeFocusType.MiddleLeft:
					if (width - horizontalDistance <= 2) return;

					rect.X += horizontalDistance;
					rect.Width -= horizontalDistance;

					//BitmapStretchOffsetPoint = new Point(rect.Width-canvas.Width,0);
					BitmapStretchOffsetPoint = new Point(rect.Width - canvasRegion.Width, 0);
					break;
				case RectangleShapeFocusType.MiddleRight:
					if (width + horizontalDistance <= 2) return;

					rect.Width += horizontalDistance;

					BitmapStretchOffsetPoint = Point.Empty;
					break;
				case RectangleShapeFocusType.BottomLeft:
					if (width - horizontalDistance <= 2) return;
					if (height + verticalDistance <= 2) return;

					rect.X += horizontalDistance;
					rect.Width -= horizontalDistance;
					rect.Height += verticalDistance;

					//BitmapStretchOffsetPoint = new Point(rect.Width-canvas.Width,0);
					BitmapStretchOffsetPoint = new Point(rect.Width - canvasRegion.Width, 0);
					break;
				case RectangleShapeFocusType.BottomCenter:
					if (height + verticalDistance <= 2) return;

					rect.Height += verticalDistance;

					BitmapStretchOffsetPoint = Point.Empty;
					break;
				case RectangleShapeFocusType.BottomRight:
					if (width + horizontalDistance <= 2) return;
					if (height + verticalDistance <= 2) return;

					rect.Width += horizontalDistance;
					rect.Height += verticalDistance;

					BitmapStretchOffsetPoint = Point.Empty;
					break;
			}
		}

		/// <summary>
		/// マウスをストレッチ可能な点に置いたままにしているか
		/// </summary>
		/// <param name="mouseLocation"></param>
		protected void MouseOverResizeHandle(Point mouseLocation)
		{
			foreach (var focusPoint in _canvasEditPoints)
			{
				if (focusPoint.rect.Contains(mouseLocation))
				{
					SetFoucsCursorType(focusPoint.focusType);
					drawStatus = DrawStatus.CanvasAdjustable;
					FocusType = focusPoint.focusType;
					return;
				}
			}
			if (this is Line) return;
			if (SelectionRect == Rectangle.Empty) return;
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
			SelectionRect = Rectangle.Empty;
			drawStatus = DrawStatus.CannotMovedOrAdjusted;
			RotationCount = 0;
			IsFlippedHorizontally = false;
			IsFlippedVertically = false;
			panel.Invalidate();
		}

		/// <summary>
		/// 描画解除
		/// </summary>
		protected virtual void CancelDrawing()
		{
			drawStatus = DrawStatus.CannotMovedOrAdjusted;
			SelectionRect = Rectangle.Empty;
			RotationCount = 0;
			IsFlippedHorizontally = false;
			IsFlippedVertically = false;
			panel.Invalidate();
		}

		protected void BitmapDrawShape(Bitmap bitmap, Graphics graphics)
		{
			if (bitmap != null)
			{
				graphics.DrawImage(bitmap,GetCanvasRegion());
				DrawCanvasEditPoint(graphics);
			}
		}

		/// <summary>
		/// bitmapの場所を取得
		/// </summary>
		/// <returns></returns>
		private (int X,int Y) GetCanvasLocation()
		{
			int offsetX = (panel.Width - (int)(canvas.Width*Scale)) / 2;
			int offsetY = (panel.Height - (int)(canvas.Height*Scale)) / 2;

			offsetX += panel.AutoScrollPosition.X;
			offsetY += panel.AutoScrollPosition.Y;

			if (offsetX <= 0 && panel.AutoScrollPosition.X == 0)
			{
				offsetX = 10;
			}
			if (offsetY <= 0 && panel.AutoScrollPosition.Y == 0)
			{
				offsetY = 10;
			}
			return (offsetX,offsetY);
		}

		public Rectangle GetCanvasRegion()
		{
			var canvaslocation = GetCanvasLocation();
			return new Rectangle(
				canvaslocation.X,
				canvaslocation.Y,
		        (int)(canvas.Width*Scale) ,
			    (int)(canvas.Height*Scale)
			);
		}

		public Rectangle ConvertSelectionRectToCanvasRect(Rectangle rect)
		{
			var canvaslocation = GetCanvasLocation();
			return new Rectangle(
					(int)(rect.X/Scale) - (int)(canvaslocation.X/Scale),
					(int)(rect.Y/Scale) - (int)(canvaslocation.Y/Scale),
					(int)(rect.Width / Scale),
					(int)(rect.Height / Scale)
				);
		}

		protected Point[] ConvertVertexs(List<Point> points)
		{
			var canvaslocation = GetCanvasLocation();
			return points.Select(v => new Point((int)(v.X/Scale) -(int) (canvaslocation.X/Scale), (int)(v.Y/Scale) - (int)(canvaslocation.Y/Scale))).ToArray();
		}

		public Point ConvertPoint(Point point)
		{
			var canvaslocation = GetCanvasLocation();
			return new Point((int)(point.X/Scale) - (int)(canvaslocation.X/Scale),(int)( point.Y/Scale) - (int)(canvaslocation.Y/Scale));
		}

		public bool IsValidLocation(Point point)
		{
			var canvaslocation = GetCanvasLocation();
			Rectangle canvasRect = new Rectangle(
				canvaslocation.X,
				canvaslocation.Y,
				(int)(canvas.Width*Scale),
				(int)(canvas.Height*Scale)
			);
			if (canvasRect.Contains(point)) return true;
			return false;
		}

		private void DrawCanvasEditPoint(Graphics graphics)
		{
			int resizerSize = 8;
			Rectangle rect;
			_canvasEditPoints = new List<(Rectangle rect, RectangleShapeFocusType focusType)>();
			foreach (var item in GetResizerPoints(GetCanvasRegion()))
			{
				var point = item.editPoint;
				using (Pen pen = new Pen(Color.Black, 1)) 
				{
					if (item.focusType == RectangleShapeFocusType.TopLeft)
					{
						rect = new Rectangle(
							point.X - resizerSize,
							point.Y - resizerSize,
							resizerSize,
							resizerSize
							);
						graphics.DrawRectangle(pen, rect);
						_canvasEditPoints.Add((rect, RectangleShapeFocusType.TopLeft));
					}
					else if (item.focusType == RectangleShapeFocusType.TopCenter)
					{
						rect = new Rectangle(
								point.X - resizerSize / 2,
								point.Y - resizerSize,
								resizerSize,
								resizerSize
								);
						graphics.DrawRectangle(pen,rect );
						_canvasEditPoints.Add((rect, RectangleShapeFocusType.TopCenter));
					}
					else if (item.focusType == RectangleShapeFocusType.TopRight)
					{
						rect = new Rectangle(
									point.X,
									point.Y - resizerSize,
									resizerSize,
									resizerSize
									);
						graphics.DrawRectangle(pen,rect);
						_canvasEditPoints.Add((rect, RectangleShapeFocusType.TopRight));
					}
					else if (item.focusType == RectangleShapeFocusType.MiddleLeft)
					{
						rect = new Rectangle(
										point.X - resizerSize,
										point.Y - resizerSize / 2,
										resizerSize,
										resizerSize
										);
						graphics.DrawRectangle(pen,rect);
						_canvasEditPoints.Add((rect, RectangleShapeFocusType.MiddleLeft));
					}
					else if (item.focusType == RectangleShapeFocusType.MiddleRight)
					{
						rect = new Rectangle(
										point.X,
										point.Y - resizerSize / 2,
										resizerSize,
										resizerSize
										);
						graphics.DrawRectangle(pen, rect);
						_canvasEditPoints.Add((rect, RectangleShapeFocusType.MiddleRight));
					}
					else if (item.focusType == RectangleShapeFocusType.BottomLeft)
					{
						rect = new Rectangle(
											point.X - resizerSize,
											point.Y,
											resizerSize,
											resizerSize
											);
						graphics.DrawRectangle(pen, rect);
						_canvasEditPoints.Add((rect, RectangleShapeFocusType.BottomLeft));
					}
					else if (item.focusType == RectangleShapeFocusType.BottomCenter)
					{
						rect = new Rectangle(
											point.X - resizerSize / 2,
											point.Y,
											resizerSize,
											resizerSize
											);
						graphics.DrawRectangle(pen, rect);
						_canvasEditPoints.Add((rect, RectangleShapeFocusType.BottomCenter));
					}
					else if (item.focusType == RectangleShapeFocusType.BottomRight)
					{
						rect = new Rectangle(
											point.X,
											point.Y,
											resizerSize,
											resizerSize
											);
						graphics.DrawRectangle(pen, rect);
						_canvasEditPoints.Add((rect, RectangleShapeFocusType.BottomRight));
					}
				}	
			}
		}

		protected void DrawCanvasAdjusted(Graphics graphics)
		{
			using (Pen selectionPen = new Pen(ResizerPointColor, 0.5f))
			{
				selectionPen.DashStyle = DashStyle.Dash;
				selectionPen.DashPattern = new float[] { 5.0f, 4.0f };// 划线长，间隔长
				graphics.DrawRectangle(selectionPen, AdjustingCanvasRect);
			}
		}

		public Rectangle SetRichTextBoxMinSize(float size,ref Rectangle rectangle)
		{
			int width = rectangle.Width;
			int height = rectangle.Height;
			switch (size)
			{
				case 8:
					if (width <= 110) rectangle.Width = 110;
					if (height <= 16) rectangle.Height = 26;
					break;
				case 9:
					if (width <= 130) rectangle.Width = 130;
					if (height <= 27) rectangle.Height = 37;
					break;
				case 10:
					if (width <= 130) rectangle.Width = 130;
					if (height <= 29) rectangle.Height = 39;
					break;
				case 11:
					if (width <= 150) rectangle.Width = 150;
					if (height <= 30) rectangle.Height = 40;
					break;
				case 12:
					if (width <= 170) rectangle.Width = 170;
					if (height <= 31) rectangle.Height = 41;
					break;
				case 14:
					if (width <= 190) rectangle.Width = 190;
					if (height <= 34) rectangle.Height = 44;
					break;
				case 16:
					if (width <= 210) rectangle.Width = 210;
					if (height <= 38) rectangle.Height = 48;
					break;
				case 18:
					if (width <= 250) rectangle.Width = 250;
					if (height <= 41) rectangle.Height = 51;
					break;
				case 20:
					if (width <= 270) rectangle.Width = 270;
					if (height <= 45) rectangle.Height = 55;
					break;
				case 22:
					if (width <= 290) rectangle.Width = 290;
					if (height <= 48) rectangle.Height = 58;
					break;
				case 24:
					if (width <= 310) rectangle.Width = 310;
					if (height <= 51) rectangle.Height = 61;
					break;
				case 26:
					if (width <= 330) rectangle.Width = 330;
					if (height <= 55) rectangle.Height = 65;
					break;
				case 28:
					if (width <= 370) rectangle.Width = 370;
					if (height <= 58) rectangle.Height = 68;
					break;
				case 36:
					if (width <= 415) rectangle.Width = 415;
					if (height <= 72) rectangle.Height = 82;
					break;
				case 48:
					if (width <= 847) rectangle.Width = 847;
					if (height <= 134) rectangle.Height = 144;
					break;
				case 72:
					if (width <= 930) rectangle.Width = 930;
					if (height <= 134) rectangle.Height = 144;
					break;
			}
			return rectangle;
		}

		protected Rectangle RotateRectangle90Degrees()
		{
			int centerX = SelectionRect.Left + SelectionRect.Width / 2;
			int centerY = SelectionRect.Top + SelectionRect.Height / 2;

			int newWidth = SelectionRect.Height;
			int newHeight = SelectionRect.Width;

			int newLeft = centerX - newWidth / 2;
			int newTop = centerY - newHeight / 2;

			return new Rectangle(newLeft, newTop, newWidth, newHeight);
		}

		public void CanvasRotateRight()
		{
			canvas.RotateFlip(RotateFlipType.Rotate90FlipNone);
			panel.Invalidate();
		}

		public void CanvasRotateLeft()
		{
			canvas.RotateFlip(RotateFlipType.Rotate270FlipNone);
			panel.Invalidate();
		}

		public void CanvasRotate180()
		{
			canvas.RotateFlip(RotateFlipType.Rotate180FlipNone);
			panel.Invalidate();
		}

		public void CanvasFlipHorizontal()
		{
			canvas.RotateFlip(RotateFlipType.RotateNoneFlipX);
			panel.Invalidate();
		}

		public void CanvasFlipVertical()
		{
			canvas.RotateFlip(RotateFlipType.RotateNoneFlipY);
			panel.Invalidate();
		}

	}
}