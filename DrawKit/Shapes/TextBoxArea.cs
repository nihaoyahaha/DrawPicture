using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace DrawKit.Shapes
{
	/// <summary>
	/// テキスト
	/// </summary>
	public class TextBoxArea : Shape
	{
		private Rectangle _rectCreating = Rectangle.Empty;
		public RichTextBox richTextBox { get; set; }
		public TextBoxArea() { }
		public TextBoxArea(Bitmap bitmap, Panel panel,float scale) : base(bitmap, panel, scale) { }
		private void BitmapDrawText()
		{
			if (canvas == null) return;
			if (SelectionRect.Width == 0 && SelectionRect.Height == 0) return;
			using (Graphics g = Graphics.FromImage(canvas))
			{
				using (Pen selectionPen = new Pen(ForeColor, Size))
				{

					// 设置 StringFormat 为精确排版模式
					StringFormat format = (StringFormat)StringFormat.GenericTypographic.Clone();
					format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
					selectionPen.DashStyle = DashStyle.Solid;
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.PixelOffsetMode = PixelOffsetMode.HighQuality;
					g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
					g.DrawString(richTextBox.Text, richTextBox.Font, new SolidBrush(ForeColor), ConvertPoint(richTextBox.Location), format);

					
					//TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.NoPrefix;
					//TextRenderer.DrawText(g, richTextBox.Text, richTextBox.Font, ConvertPoint(richTextBox.Location), ForeColor, Color.Transparent,flags);
				}
			}
			SelectionRect = Rectangle.Empty;
			drawStatus = DrawStatus.CompleteDrawText;
			richTextBox.Text = "";
			panel.Invalidate();	
		}
		public override void MouseDown(MouseEventArgs e)
		{
			if (!IsValidLocation(e.Location) && drawStatus != DrawStatus.CanvasAdjustable) return;
			if (e.Button == MouseButtons.Left)
			{
				MouseLeftButtonDownHandle(e);
			}
		}

		private void MouseLeftButtonDownHandle(MouseEventArgs e)
		{
			if (drawStatus == DrawStatus.CannotMovedOrAdjusted)
			{
				StartPoint = e.Location;
				drawStatus = DrawStatus.Creating;
			}
			else if (drawStatus == DrawStatus.CanMove)
			{
				Offset = e.Location;
				drawStatus = DrawStatus.Moving;
			}
			else if (drawStatus == DrawStatus.CanAdjusted)
			{
				Offset = e.Location;
				drawStatus = DrawStatus.Adjusting;
			}
			else if (drawStatus == DrawStatus.CanvasAdjustable)
			{
				BitmapDrawText();
				AdjustingCanvasRect = GetCanvasRegion();
				Offset = e.Location;
				drawStatus = DrawStatus.CanvasAdjusting;
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
			if (drawStatus == DrawStatus.Creating)
			{
				EndPoint = e.Location;
				int x = Math.Min(StartPoint.X, EndPoint.X);
				int y = Math.Min(StartPoint.Y, EndPoint.Y);
				int width = Math.Abs(StartPoint.X - EndPoint.X);
				int height = Math.Abs(StartPoint.Y - EndPoint.Y);
				var rect = GetCanvasRegion();
				if (x <= rect.Left)
				{
					x = rect.Left;
					width = ConvertPoint(StartPoint).X;
				}
				if (y <= rect.Top)
				{
					y = rect.Top;
					height = ConvertPoint(StartPoint).Y;
				}
				if (x + width >= rect.Right) width = rect.Right - x;
				if (y + height >= rect.Bottom)
				{
					height = rect.Bottom - y;
				} 

				_rectCreating = new Rectangle(x, y, width, height);
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Moving)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionRect.Offset(deltaX, deltaY);
				Offset = e.Location;
				IsRectMovePositionValid();
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Adjusting)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				TextAreaSelectionAdjusting(deltaX, deltaY);
				Offset = e.Location;
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				int deltaX = e.X - Offset.X;
				int deltaY = e.Y - Offset.Y;
				SelectionAdjusting(deltaX, deltaY, ref AdjustingCanvasRect);
				Offset = e.Location;
				panel.Invalidate();
			}
		}

		/// <summary>
		/// 長方形選択範囲の調整
		/// </summary>
		/// <param name="horizontalDistance"></param>
		/// <param name="verticalDistance"></param>
		private void TextAreaSelectionAdjusting(int horizontalDistance, int verticalDistance)
		{
			int width = SelectionRect.Width;
			int height = SelectionRect.Height;
			int selectionX = SelectionRect.X;
			int selectionY = SelectionRect.Y;
			int selectionRight = SelectionRect.Right;
			int selectionBottom = SelectionRect.Bottom;
			var canvasRect = GetCanvasRegion();
			switch (FocusType)
			{
				case RectangleShapeFocusType.TopLeft:
					if (width - horizontalDistance <= 2) return;
					if (height - verticalDistance <= 2) return;

					if (selectionX + horizontalDistance <= canvasRect.X)
					{
						SelectionRect.X = canvasRect.X;
						SelectionRect.Width = selectionRight - canvasRect.X;
					}
					else
					{
						SelectionRect.X += horizontalDistance;
						SelectionRect.Width -= horizontalDistance;
					}

					if (selectionY + verticalDistance <= canvasRect.Y)
					{
						SelectionRect.Y = canvasRect.Y;
						SelectionRect.Height = selectionBottom - canvasRect.Y;
					}
					else
					{
						SelectionRect.Y += verticalDistance;
						SelectionRect.Height -= verticalDistance;
					}
					break;
				case RectangleShapeFocusType.TopCenter:
					if (height - verticalDistance <= 2) return;

					if (selectionY + verticalDistance <= canvasRect.Y)
					{
						SelectionRect.Y = canvasRect.Y;
						SelectionRect.Height = selectionBottom - canvasRect.Y;
					}
					else
					{
						SelectionRect.Y += verticalDistance;
						SelectionRect.Height -= verticalDistance;
					}
					break;
				case RectangleShapeFocusType.TopRight:
					if (width + horizontalDistance <= 2) return;
					if (height - verticalDistance <= 2) return;

					if (SelectionRect.Right + horizontalDistance < canvasRect.Right)
					{
						SelectionRect.Width += horizontalDistance;
					}
					if (SelectionRect.Top + verticalDistance <= canvasRect.Top)
					{
						SelectionRect.Y = canvasRect.Y;
						SelectionRect.Height = selectionBottom - canvasRect.Y;
					}
					else
					{
						SelectionRect.Y += verticalDistance;
						SelectionRect.Height -= verticalDistance;
					}
					break;
				case RectangleShapeFocusType.MiddleLeft:
					if (width - horizontalDistance <= 2) return;


					if (selectionX + horizontalDistance <= canvasRect.X)
					{
						SelectionRect.X = canvasRect.X;
						SelectionRect.Width = selectionRight - canvasRect.X;
					}
					else
					{
						SelectionRect.X += horizontalDistance;
						SelectionRect.Width -= horizontalDistance;
					}
					break;
				case RectangleShapeFocusType.MiddleRight:
					if (width + horizontalDistance <= 2) return;

					if (SelectionRect.Right + horizontalDistance < canvasRect.Right)
					{
						SelectionRect.Width += horizontalDistance;
					}
					break;
				case RectangleShapeFocusType.BottomLeft:
					if (width - horizontalDistance <= 2) return;
					if (height + verticalDistance <= 2) return;

					if (SelectionRect.Left + horizontalDistance <= canvasRect.Left)
					{
						SelectionRect.X = canvasRect.Left;
						SelectionRect.Width = selectionRight - canvasRect.X;
					}
					else
					{
						SelectionRect.X += horizontalDistance;
						SelectionRect.Width -= horizontalDistance;
					}
					if (SelectionRect.Bottom + verticalDistance < canvasRect.Bottom)
					{
						SelectionRect.Height += verticalDistance;
					}
					break;
				case RectangleShapeFocusType.BottomCenter:
					if (height + verticalDistance <= 2) return;

					if (SelectionRect.Bottom + verticalDistance < canvasRect.Bottom)
					{
						SelectionRect.Height += verticalDistance;
					}
					break;
				case RectangleShapeFocusType.BottomRight:
					if (width + horizontalDistance <= 2) return;
					if (height + verticalDistance <= 2) return;

					if (SelectionRect.Right + horizontalDistance < canvasRect.Right)
					{
						SelectionRect.Width += horizontalDistance;
					}
					if (SelectionRect.Bottom + verticalDistance < canvasRect.Bottom)
					{
						SelectionRect.Height += verticalDistance;
					}
					break;
			}
		}

		private void IsRectMovePositionValid()
		{
			var height = SelectionRect.Height;
			var rect = GetCanvasRegion();
			if (SelectionRect.Left <= rect.Left)
			{
				SelectionRect.X = rect.X;
			}
			if (SelectionRect.Top <= rect.Top)
			{
				SelectionRect.Y = rect.Y;
			}
			if (SelectionRect.Right >= rect.Right)
			{
				SelectionRect.X = rect.Right - SelectionRect.Width;
			}
			if (SelectionRect.Bottom >= rect.Bottom)
			{
				SelectionRect.Y = rect.Bottom - SelectionRect.Height;
				SelectionRect.Height = rect.Bottom - SelectionRect.Y;
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
			if (drawStatus == DrawStatus.Creating)
			{
				BitmapDrawText();

				if (drawStatus == DrawStatus.CompleteDrawText) return;

				if (_rectCreating.X == 0 && _rectCreating.Y == 0)
				{
					_rectCreating.X = StartPoint.X;
					_rectCreating.Y = StartPoint.Y;
				}

				SetRichTextBoxMinSize(richTextBox.Font.Size,ref _rectCreating);

				drawStatus = DrawStatus.CanAdjusted;
				SelectionRect = _rectCreating;
				panel.Invalidate();
				_rectCreating = Rectangle.Empty;
			}
			else if (drawStatus == DrawStatus.Moving)
			{
				drawStatus = DrawStatus.CanMove;
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.Adjusting)
			{
				drawStatus = DrawStatus.CompleteAdjustment;
				panel.Invalidate();
			}
			else if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				drawStatus = DrawStatus.CompleteCanvasAdjustment;
				panel.Invalidate();
			}
		}
		public override void InPainting(Graphics graphics)
		{
			if (canvas != null)
			{
				BitmapDrawShape(canvas, graphics);
			}
			if (drawStatus == DrawStatus.Creating)
			{
				if (_rectCreating.Width == 0 || _rectCreating.Height == 0) return;
				DrawCreating(graphics);
			}
			else if (drawStatus == DrawStatus.Moving ||
				drawStatus == DrawStatus.CanMove ||
				drawStatus == DrawStatus.CanAdjusted ||
				drawStatus == DrawStatus.Adjusting ||
				drawStatus == DrawStatus.CompleteAdjustment)
			{
				if (SelectionRect.Width == 0 || SelectionRect.Height == 0) return;
				DrawCanMoveOrAdjusted(graphics);
			}
			else if (drawStatus == DrawStatus.CanvasAdjusting)
			{
				DrawCanvasAdjusted(graphics);
			}
			else if (drawStatus == DrawStatus.CompleteCanvasAdjustment)
			{
				DrawCanMoveOrAdjusted(graphics);
			}
		}
		private void DrawCreating(Graphics graphics)
		{
			using (Pen selectionPen = new Pen(Color.Black, 0.5f))
			{
				selectionPen.DashStyle = DashStyle.Dash;
				selectionPen.DashPattern = new float[] { 5.0f, 4.0f };// 划线长，间隔长
				Rectangle bitmapArea = GetCanvasRegion();
				graphics.SetClip(bitmapArea);
				graphics.DrawRectangle(selectionPen, _rectCreating);
				graphics.ResetClip();
			}
			if (SelectionRect != Rectangle.Empty)
			{
				using (Pen selectionPen = new Pen(ResizerPointColor, 0.5f))
				{
					selectionPen.DashStyle = DashStyle.Dash;
					selectionPen.DashPattern = new float[] { 5.0f, 4.0f };// 划线长，间隔长
					graphics.DrawRectangle(selectionPen, SelectionRect);
				}
				foreach (var item in GetResizerPoints(SelectionRect))
				{
					graphics.FillEllipse(new SolidBrush(ResizerPointColor), new Rectangle(
						item.editPoint.X - (int)ResizerPointSize / 2,
						item.editPoint.Y - (int)ResizerPointSize / 2,
						(int)ResizerPointSize,
						(int)ResizerPointSize));
				}
			}
		}

		private void DrawCanMoveOrAdjusted(Graphics graphics)
		{
			using (Pen selectionPen = new Pen(ResizerPointColor, 0.5f))
			{
				selectionPen.DashStyle = DashStyle.Dash;
				selectionPen.DashPattern = new float[] { 5.0f, 4.0f };// 划线长，间隔长
				graphics.DrawRectangle(selectionPen, SelectionRect);
			}
			foreach (var item in GetResizerPoints(SelectionRect))
			{
				graphics.FillEllipse(new SolidBrush(ResizerPointColor), new Rectangle(
					item.editPoint.X - (int)ResizerPointSize / 2,
					item.editPoint.Y - (int)ResizerPointSize / 2,
					(int)ResizerPointSize,
					(int)ResizerPointSize));
			}
		}


		public override void Clear(Color color)
		{
			ClearBitmap(color);
		}

		public override void CommitCurrentShape()
		{
			BitmapDrawText();
		}

		public override void RotateRight(){}

		public override void RotateLeft(){}

		public override void Rotate180(){}

		public override void FlipHorizontal(){}

		public override void FlipVertical(){}

	}
}
