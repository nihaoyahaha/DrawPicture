using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPicture.Shapes
{
    public enum RectangleShapeFocusType
    {
		/// <summary>
		/// フォーカスなし
		/// </summary>
		Unfocused,

		/// <summary>
		/// 上部左側
		/// </summary>
		TopLeft,

		/// <summary>
		/// 上部中央
		/// </summary>
		TopCenter,

		/// <summary>
		/// 上部の右側
		/// </summary>
		TopRight,

		/// <summary>
		/// 左側中央
		/// </summary>
		MiddleLeft,

		/// <summary>
		/// 右中央
		/// </summary>
		MiddleRight,

		/// <summary>
		/// 下部の左側
		/// </summary>
		BottomLeft,

		/// <summary>
		/// 下部中央
		/// </summary>
		BottomCenter,

		/// <summary>
		/// 下部の右側
		/// </summary>
		BottomRight,

		/// <summary>
		/// 移動
		/// </summary>
		Move,
	}
}
