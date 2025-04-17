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
		/// 移動中
		/// </summary>
		Moving,

		/// <summary>
		/// 調整可能
		/// </summary>
		CanAdjusted,

		/// <summary>
		/// 調整中
		/// </summary>
		Adjusting,

		/// <summary>
		/// 調整の完了
		/// </summary>
		CompleteAdjustment,

		/// <summary>
		/// 移動不可調整不可
		/// </summary>
		CannotMovedOrAdjusted,

		/// <summary>
		/// 作成描画中
		/// </summary>
		Creating,

		/// <summary>
		/// スタイルの調整
		/// </summary>
		AdjustTheStyle

	}
}
