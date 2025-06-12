using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawKit.Shapes
{
    public enum RectangleShapeFocusType
    {
		/// <summary>
		/// 无焦点
		/// </summary>
		Unfocused,

		/// <summary>
		/// 顶部左侧
		/// </summary>
		TopLeft,

		/// <summary>
		///顶部中央
		/// </summary>
		TopCenter,

		/// <summary>
		/// 顶部右侧
		/// </summary>
		TopRight,

		/// <summary>
		/// 左侧中央
		/// </summary>
		MiddleLeft,

		/// <summary>
		/// 右侧中央
		/// </summary>
		MiddleRight,

		/// <summary>
		/// 底部左侧
		/// </summary>
		BottomLeft,

		/// <summary>
		/// 底部中央
		/// </summary>
		BottomCenter,

		/// <summary>
		/// 底部右侧
		/// </summary>
		BottomRight,

		/// <summary>
		/// 移动
		/// </summary>
		Move,
	}
}
