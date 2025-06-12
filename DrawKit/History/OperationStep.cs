using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawKit.History
{
	public static class OperationStep
	{
		public static event Action OnOperationCompleted;

		private const int _MaxOperationSteps = 100;

		private static Stack<Bitmap> _revokeStack = new Stack<Bitmap>();

		private static Stack<Bitmap> _redoStack = new Stack<Bitmap>();

		public static void InitStack()
		{
			_revokeStack = new Stack<Bitmap>();
			_redoStack = new Stack<Bitmap>();
			OnOperationCompleted?.Invoke();
		}

		public static void PushRevokeStack(Bitmap canvas)
		{
			if (canvas == null) return;
			LimitRevokeStackElements();
			_revokeStack.Push((Bitmap)canvas.Clone());
			_redoStack.Clear();
			OnOperationCompleted?.Invoke();
		}

		private static void LimitRevokeStackElements()
		{
			if (_revokeStack.Count >= _MaxOperationSteps)
			{
				Stack<Bitmap> tempStack = new Stack<Bitmap>();
				int itemsToKeep = _MaxOperationSteps - 1;
				for (int i = 0; i < itemsToKeep; i++)
				{
					if (_revokeStack.Count > 0)
					{
						tempStack.Push(_revokeStack.Pop());
					}
				}
				if (_revokeStack.Count > 0)
				{
					_revokeStack.Pop();
				}
				while (tempStack.Count > 0)
				{
					_revokeStack.Push(tempStack.Pop());
				}
			}
		}

		/// <summary>
		/// 撤销
		/// </summary>
		public static Bitmap Revoke(Bitmap canvas)
		{
			if (_revokeStack.Count == 0) return null;
			_redoStack.Push((Bitmap)canvas.Clone());
			return _revokeStack.Pop();
		}

		/// <summary>
		/// 重做
		/// </summary>
		public static Bitmap Redo(Bitmap canvas)
		{
			if (_redoStack.Count == 0) return null;
			_revokeStack.Push((Bitmap)canvas.Clone());
			return _redoStack.Pop();
		}

		public static bool AllowRevoke()
		{
			return _revokeStack.Count > 0 ? true : false;
		}

		public static bool AllowRedo()
		{
			return _redoStack.Count > 0 ? true : false;
		}
	}
}
