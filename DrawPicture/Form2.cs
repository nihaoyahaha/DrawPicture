using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawPicture
{
    public partial class Form2: Form
    {
        public Form2()
        {
            InitializeComponent();
        }

		private void Form2_Load(object sender, EventArgs e)
		{
			// 创建一个 5x5 的位图，并初始化为黑色
			Bitmap bmp = new Bitmap(5, 5);
			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.Clear(Color.Black);
			}

			// 在中央绘制一个白色矩形区域
			for (int x = 1; x <= 3; x++)
			{
				for (int y = 1; y <= 3; y++)
				{
					bmp.SetPixel(x, y, Color.White);
				}
			}

			Console.WriteLine("初始图像：");
			PrintBitmap(bmp);

			// 调用高效的洪水填充算法
			FloodFillFast(bmp, 2, 2, Color.White.ToArgb(), Color.Red.ToArgb());

			Console.WriteLine("\n填充后的图像：");
			PrintBitmap(bmp);
		}

		/// <summary>
		/// 高效的洪水填充算法（基于 LockBits）
		/// </summary>
		/// <param name="bmp">目标位图</param>
		/// <param name="x">起始点的 X 坐标</param>
		/// <param name="y">起始点的 Y 坐标</param>
		/// <param name="targetColor">需要替换的目标颜色（ARGB 格式）</param>
		/// <param name="replacementColor">新颜色（ARGB 格式）</param>
		static unsafe void FloodFillFast(Bitmap bmp, int x, int y, int targetColor, int replacementColor)
		{
			// 锁定位图数据
			Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
			BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			try
			{
				// 获取位图数据的起始地址
				byte* ptr = (byte*)bmpData.Scan0;

				// 计算每行的字节数
				int stride = bmpData.Stride;

				// 将二维坐标转换为一维索引
				int index = y * stride + x * 4;

				// 如果起始点的颜色不是目标颜色，则直接返回
				if (*(int*)(ptr + index) != targetColor)
					return;

				// 使用栈实现洪水填充
				Stack<(int, int)> stack = new Stack<(int, int)>();
				stack.Push((x, y));

				while (stack.Count > 0)
				{
					var (currentX, currentY) = stack.Pop();
					int currentIndex = currentY * stride + currentX * 4;

					// 确保当前像素在范围内，且颜色为目标颜色
					if (currentX >= 0 && currentX < bmp.Width &&
						currentY >= 0 && currentY < bmp.Height &&
						*(int*)(ptr + currentIndex) == targetColor)
					{
						// 替换颜色
						*(int*)(ptr + currentIndex) = replacementColor;

						// 将周围的四个方向压入栈中
						stack.Push((currentX - 1, currentY)); // 左
						stack.Push((currentX + 1, currentY)); // 右
						stack.Push((currentX, currentY - 1)); // 上
						stack.Push((currentX, currentY + 1)); // 下
					}
				}
			}
			finally
			{
				// 解锁位图
				bmp.UnlockBits(bmpData);
			}
		}

		/// <summary>
		/// 打印位图内容到控制台
		/// </summary>
		/// <param name="bmp">要打印的位图</param>
		static void PrintBitmap(Bitmap bmp)
		{
			for (int y = 0; y < bmp.Height; y++)
			{
				for (int x = 0; x < bmp.Width; x++)
				{
					Color pixelColor = bmp.GetPixel(x, y);
					if (pixelColor == Color.Black)
						Console.Write("⬛"); // 黑色方块
					else if (pixelColor == Color.White)
						Console.Write("⬜"); // 白色方块
					else if (pixelColor == Color.Red)
						Console.Write("🟥"); // 红色方块
					else
						Console.Write("❓"); // 其他颜色
				}
				Console.WriteLine();
			}
		}
	}
}
