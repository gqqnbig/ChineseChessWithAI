using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;
using IntPoint = System.Drawing.Point;

namespace 中国象棋
{
	public class BoardEncoded : Board
	{
		/// <summary>
		/// 棋盘上一个位置被敌方多少子控制
		/// </summary>
		private HashSet<IntPoint>[,] controlMap = null;

		private BoardEncoded(Piece[,] pieces) : base(pieces)
		{ }

		public BoardEncoded(Board board) : base(board)
		{ }


		private void InitializeControlMap(ChessColor controllerColor)
		{
			controlMap = new HashSet<IntPoint>[Height, Width];
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					if (this[y, x] == null)
						continue;

					if (this[y, x].Color == controllerColor)
					{
						foreach (var m in this[y, x].GetPossibleMovements(new IntPoint(x, y), this))
						{
							if (m.IsEat == false) //炮的移动不一定能吃子
								continue;

							var p = m.Target;
							if (controlMap[p.Y, p.X] == null)
								controlMap[p.Y, p.X] = new HashSet<IntPoint>();

							controlMap[p.Y, p.X].Add(new IntPoint(x, y));
						}
					}
				}
			}
		}

		public bool CanBeEaten(IntPoint location)
		{
			ChessColor color = this[location.Y, location.X].Color;
			if (controlMap == null)
				InitializeControlMap(color == ChessColor.Red ? ChessColor.Black : ChessColor.Red);

			return controlMap[location.Y, location.X]?.Any() ?? false;
		}

		public override Board Move(IntPoint location, IntPoint targetLocation, out Piece eatenPiece)
		{
			/*
			 * 求证：当一个棋子按规则从位置a移到位置b，若位置b原不被任何子吃，
			 * 则该棋子移动后也不被任何子吃。
			 *
			 * 解：若士贴着马，士走斜线后会被马吃。
			 */

			HashSet<IntPoint>[,] newControlMap = null;
			var arr = MoveCore(location, targetLocation, out eatenPiece);
			var newBoard = new BoardEncoded(arr);
			if (controlMap != null)
			{
				newControlMap = new HashSet<IntPoint>[Height, Width];
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < Height; y++)
					{
						if (controlMap[y, x] == null)
							continue;

						newControlMap[y, x] = new HashSet<IntPoint>(controlMap[y, x]);
					}
				}

				if (eatenPiece != null)
				{
					//被吃掉的子就不再控制区域了。
					RemoveFromControlMap(targetLocation, newControlMap);
				}

				var pointsToAdjust1 = GetAdjacent马(location).Concat(GetDiagonal象(location)).Concat(GetStraightLine车或炮(location)).Concat(GetJumpOver炮(location));
				Debug.Assert(pointsToAdjust1.Distinct().Count() == pointsToAdjust1.Count(), "按原位置计算的需要调整的坐标数不应该重复。");
				//pointsToAdjust1 可以包含被吃掉的子的位置
				pointsToAdjust1 = pointsToAdjust1.Except(new[] { targetLocation });

				var pointsToAdjust2 = newBoard.GetAdjacent马(targetLocation).Concat(newBoard.GetDiagonal象(targetLocation)).Concat(newBoard.GetStraightLine车或炮(targetLocation)).Concat(newBoard.GetJumpOver炮(targetLocation));
				Debug.Assert(pointsToAdjust2.Distinct().Count() == pointsToAdjust2.Count(), "按新位置计算的需要调整的坐标数不应该重复。");
				//pointsToAdjust1和pointsToAdjust2可以重复。比如，红炮以黑炮为桩吃了个子。以原位置计算，黑炮控制域需要调整；以新位置计算，黑炮控制域也需要调整。

				var pointsToAdjust = pointsToAdjust1.Union(pointsToAdjust2);
				foreach (var piece in pointsToAdjust)
				{
					//按照一般原则，去除老棋盘中的控制点，计算新棋盘的控制点
					RemoveFromControlMap(piece, newControlMap);

					foreach (var m in newBoard[piece.Y, piece.X].GetPossibleMovements(piece, newBoard))
					{
						if (m.IsEat == false)
							continue;

						if (newControlMap[m.Target.Y, m.Target.X] == null)
							newControlMap[m.Target.Y, m.Target.X] = new HashSet<IntPoint>();
						newControlMap[m.Target.Y, m.Target.X].Add(piece);
					}
				}

#if DEBUG
		
				//用于测试
				BoardEncoded verifyBoard = new BoardEncoded(arr);
				verifyBoard.InitializeControlMap(this[location.Y, location.X].Color == ChessColor.Red ? ChessColor.Black : ChessColor.Red);
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < Height; y++)
					{
						if ((verifyBoard.controlMap[y, x] == null || verifyBoard.controlMap[y, x].Count == 0) && (newControlMap[y, x] == null || newControlMap[y, x].Count == 0))
							continue;
						else if (verifyBoard.controlMap[y, x] != null && newControlMap[y, x] != null && verifyBoard.controlMap[y, x].SetEquals(newControlMap[y, x]))
						{
							continue;
						}
						else
							Debug.Fail($"优化算法算出的ControlMap与传统算法在({x},{y})不同。");

					}
				}		
#endif

			}

			newBoard.controlMap = newControlMap;


			return newBoard;
		}

		private void RemoveFromControlMap(IntPoint location, HashSet<IntPoint>[,] controlMap)
		{
			foreach (var m in this[location].GetPossibleMovements(location, this))
			{
				if (m.IsEat == false)
					continue;

				var r = controlMap[m.Target.Y, m.Target.X].Remove(location);
				Debug.Assert(r, $"{location}的{this[location.Y, location.X]}本应控制{m.Target}，但却没有。除非移动的是士，士别马眼。");
			}
		}

		private IEnumerable<IntPoint> GetAdjacent马(IntPoint location)
		{
			if (location.Y - 1 >= 0 && this[location.Y - 1, location.X]?.Name == "马" && this[location.Y - 1, location.X]?.Color != this[location].Color)
				yield return new IntPoint(location.X, location.Y - 1);

			if (location.Y + 1 < Height && this[location.Y + 1, location.X]?.Name == "马" && this[location.Y + 1, location.X]?.Color != this[location].Color)
				yield return new IntPoint(location.X, location.Y + 1);

			if (location.X - 1 >= 0 && this[location.Y, location.X - 1]?.Name == "马" && this[location.Y, location.X - 1]?.Color != this[location].Color)
				yield return new IntPoint(location.X - 1, location.Y);

			if (location.X + 1 < Width && this[location.Y, location.X + 1]?.Name == "马" && this[location.Y, location.X + 1]?.Color != this[location].Color)
				yield return new IntPoint(location.X + 1, location.Y);
		}


		private IEnumerable<IntPoint> GetDiagonal象(IntPoint location)
		{
			if (location.X - 1 >= 0 && location.Y - 1 >= 0 && this[location.Y - 1, location.X - 1]?.Name == "象" && this[location.Y - 1, location.X - 1]?.Color != this[location].Color)
				yield return new IntPoint(location.X - 1, location.Y - 1);

			if (location.X - 1 >= 0 && location.Y + 1 < Height && this[location.Y + 1, location.X - 1]?.Name == "象" && this[location.Y + 1, location.X - 1]?.Color != this[location].Color)
				yield return new IntPoint(location.X - 1, location.Y + 1);

			if (location.X + 1 < Width && location.Y - 1 >= 0 && this[location.Y - 1, location.X + 1]?.Name == "象" && this[location.Y - 1, location.X + 1]?.Color != this[location].Color)
				yield return new IntPoint(location.X + 1, location.Y - 1);

			if (location.X + 1 < Width && location.Y + 1 < Height && this[location.Y + 1, location.X + 1]?.Name == "象" && this[location.Y + 1, location.X + 1]?.Color != this[location].Color)
				yield return new IntPoint(location.X + 1, location.Y + 1);
		}

		private IEnumerable<IntPoint> GetStraightLine车或炮(IntPoint location)
		{
			for (int x = location.X - 1; x >= 0; x--)
			{
				if (this[location.Y, x] == null)
					continue;

				if ((this[location.Y, x].Name == "车" || this[location.Y, x].Name == "炮") && this[location.Y, x].Color != this[location].Color)
					yield return new IntPoint(x, location.Y);
				break;
			}

			for (int x = location.X + 1; x < Width; x++)
			{
				if (this[location.Y, x] == null)
					continue;

				if ((this[location.Y, x].Name == "车" || this[location.Y, x].Name == "炮") && this[location.Y, x].Color != this[location].Color)
					yield return new IntPoint(x, location.Y);
				break;
			}

			for (int y = location.Y - 1; y >= 0; y--)
			{
				if (this[y, location.X] == null)
					continue;

				if ((this[y, location.X].Name == "车" || this[y, location.X].Name == "炮") && this[y, location.X].Color != this[location].Color)
					yield return new IntPoint(location.X, y);
				break;
			}

			for (int y = location.Y + 1; y < Height; y++)
			{
				if (this[y, location.X] == null)
					continue;

				if ((this[y, location.X].Name == "车" || this[y, location.X].Name == "炮") && this[y, location.X].Color != this[location].Color)
					yield return new IntPoint(location.X, y);
				break;
			}
		}

		private IEnumerable<IntPoint> GetJumpOver炮(IntPoint location)
		{
			foreach (int xDirection in new[] { -1, 0, 1 })
			{
				foreach (var yDirection in new[] { -1, 0, 1 })
				{
					if (Math.Abs(xDirection) + Math.Abs(yDirection) != 1)
						continue;

					int step = 1;
					for (; ; step++)
					{
						int x = location.X + xDirection * step;
						int y = location.Y + yDirection * step;

						if (x < 0 || x >= Width || y < 0 || y >= Height)
							break;

						if (this[y, x] != null)
							break;
					}

					step++;
					for (; ; step++)
					{
						int x = location.X + xDirection * step;
						int y = location.Y + yDirection * step;

						if (x < 0 || x >= Width || y < 0 || y >= Height)
							break;

						if (this[y, x] != null)
						{
							if (this[y, x].Name == "炮" && this[y, x].Color != this[location].Color)
								yield return new IntPoint(x, y);

							break;
						}
					}
				}
			}
		}
	}
}
