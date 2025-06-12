using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawKit.Shapes
{
    public enum DrawStatus
    {
		/// <summary>
		/// 可移动
		/// </summary>
		CanMove,

		/// <summary>
		/// 正在移动
		/// </summary>
		Moving,

		/// <summary>
		/// 可调整
		/// </summary>
		CanAdjusted,

		/// <summary>
		/// 正在调整
		/// </summary>
		Adjusting,

		/// <summary>
		/// 完成调整
		/// </summary>
		CompleteAdjustment,

		/// <summary>
		/// 不可移动不可调整
		/// </summary>
		CannotMovedOrAdjusted,

		/// <summary>
		/// 正在绘制
		/// </summary>
		Creating,

		/// <summary>
		/// 调整样式
		/// </summary>
		AdjustTheStyle,

		/// <summary>
		/// Bitmap可调整
		/// </summary>
		CanvasAdjustable,

		/// <summary>
		///  Bitmap正在调整
		/// </summary>
		CanvasAdjusting,

		/// <summary>
		/// Bitmap完成调整
		/// </summary>
		CompleteCanvasAdjustment,

		/// <summary>
		/// 完成绘制文本
		/// </summary>
	    CompleteDrawText

	}
}
