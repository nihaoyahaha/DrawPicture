using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPicture.Shapes
{
    public enum DrawStatus
    {
		/// <summary>
		/// 移動可能
		/// </summary>
		CanMove,

		/// <summary>
		/// 調整可能
		/// </summary>
		CanAdjusted,

		/// <summary>
		/// 移動不可調整不可
		/// </summary>
		CannotMovedOrAdjusted,

		/// <summary>
		/// 描画中
		/// </summary>
		Creating,

		/// <summary>
		/// スタイルの調整
		/// </summary>
		AdjustTheStyle

	}
}
