﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntPoint = System.Drawing.Point;

namespace 中国象棋
{
	class PathFinder
	{
		private static bool CanBeEaten(IntPoint location, Board board)
		{
			ChessColor color = board[location.Y, location.X].Color;
			for (int x = 0; x < 9; x++)
			{
				for (int y = 0; y < 10; y++)
				{
					if (board[y, x] == null)
						continue;

					if (board[y, x].Color != color && board[y, x].GetPossibleMovements(new IntPoint(x, y), board).Contains(location))
						return true;
				}
			}
			return false;
		}


		/// <summary>
		/// 查找吃到将的步数。避免走步中被对方子吃掉，如果没有不被对方吃掉的步数，则返回<see cref="int.MaxValue"/>。
		/// </summary>
		/// <returns></returns>
		public static int FindMovesCountToJiang(Piece piece, IntPoint location, Board board)
		{
			if (piece.Name == "士" || piece.Name == "象")
				return int.MaxValue;

#if DEBUG
			Stopwatch sw = new Stopwatch();
			sw.Start();
			var expandedNodes = 0;
#endif
			var openList = new PriorityQueue<SearchState>((a, b) => a.F.CompareTo(b.F), 9 * 10);
			openList.Enqueue(new SearchState { Location = location, G = 0, Board = board });

			var closedList = new HashSet<Board>();

			while (openList.Count > 0)
			{
				var searchState = openList.Dequeue();

#if DEBUG
				_ = searchState.H;
#endif
				closedList.Add(searchState.Board);

				if (MainWindow.IsWin(searchState.Board, searchState.TargetPiece.Color).GetValueOrDefault(false))
				{
#if DEBUG
					sw.Stop();
					Debug.WriteLine($"ExpandedNodes={expandedNodes}，用时{sw.ElapsedMilliseconds}。");
#endif
					return searchState.G;
				}

#if DEBUG
				expandedNodes++;
#endif
				foreach (var movement in searchState.TargetPiece.GetPossibleMovements(searchState.Location, searchState.Board))
				{
					var nextBoard = searchState.Board.Move(searchState.Location, movement, out _);
					if (CanBeEaten(movement, nextBoard) == false && closedList.Contains(nextBoard) == false)
						openList.Enqueue(new SearchState { Location = movement, Board = nextBoard, G = searchState.G + 1, });
				}
			}

#if DEBUG
			sw.Stop();
			Debug.WriteLine($"ExpandedNodes={expandedNodes}，用时{sw.ElapsedMilliseconds}。");
#endif       
			return int.MaxValue;
		}


		class SearchState
		{
			public SearchState Parent { get; set; }

			public IntPoint Location { get; set; }


			private float? h;
			public float H
			{
				get
				{
					if (h == null)
						h = GetHeuristic();

					return h.Value;
				}
			}
			public int G { get; set; }

			/// <summary>
			/// F=G+H。类型是float，因为float有<see cref="float.PositiveInfinity"/>。
			/// </summary>
			public float F => H + G;

			public Board Board { get; set; }

			public Piece TargetPiece => Board[Location.Y, Location.X];


			private float GetHeuristic()
			{
				var jiang = Board.GetOppositeJiangLocation(TargetPiece.Color);
				if (jiang.X == -1)
					return 0;
				switch (TargetPiece.Name)
				{
					case "车":
						return Get车Heuristic(Location, jiang);
					case "炮":
						if (Location.X != jiang.X && Location.Y != jiang.Y)
							return 2;
						else
							return 1;
					case "马":
						//https://stackoverflow.com/a/41704071/746461
						//answered Jan 17 '17 at 18:08
						//Anthor: simon

						int dx = Math.Abs(Location.X - jiang.X);
						int dy = Math.Abs(Location.Y - jiang.Y);

						if (dx < dy)
						{
							var t = dx;
							dx = dy;
							dy = t;
						}

						if (dx == 1 && dy == 0)
							return 3;
						if (dx == 2 && dy == 2)
							return 4;

						var delta = dx - dy;
						if (dy > delta)
							return delta - 2 * (int)Math.Floor((delta - dy) / 3d);
						else
							return delta - 2 * (int)Math.Floor((delta - dy) / 4d);

					case "兵":
						return Math.Abs(Location.X - jiang.X) + Math.Abs(Location.Y - jiang.Y);

					case "将":
						if (Location.X == jiang.X)
							return 1;
						else
							return float.PositiveInfinity;


					default:
						return float.PositiveInfinity;
				}
			}

			private float Get车Heuristic(IntPoint start, IntPoint destination)
			{
				if (start == destination)
					return 0;

				if (start.X != destination.X && start.Y != destination.Y)
				{
					//路径1
					var a = Get车Heuristic(start, new IntPoint(start.X, destination.Y));
					if (a < 3)
						a += Get车Heuristic(new IntPoint(start.X, destination.Y), destination);

					//路径2
					var b = Get车Heuristic(start, new IntPoint(destination.X, start.Y));
					if (b < 3 && b < a)
						b += Get车Heuristic(new IntPoint(destination.X, start.Y), destination);

					return Math.Min(3, Math.Min(a, b));
				}

				if (start.X == destination.X)
				{
					var oppositePieceCount = 1;
					foreach (var y in RangeExcludeExclude(start.Y, destination.Y))
					{
						if (Board[y, start.X] == null)
							continue;
						else if (Board[y, start.X].Color == TargetPiece.Color) //中间有我方棋子阻挡
							return 3;
						else
							oppositePieceCount++;
					}
					return oppositePieceCount;
				}
				else
				{
					var oppositePieceCount = 1;
					foreach (var x in RangeExcludeExclude(start.X, destination.X))
					{
						if (Board[start.Y, x] == null)
							continue;
						else if (Board[start.Y, x].Color == TargetPiece.Color) //中间有我方棋子阻挡
							return 3;
						else
							oppositePieceCount++;
					}
					return oppositePieceCount;
				}
			}


			public override int GetHashCode()
			{
				return Board.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				var other = obj as SearchState;
				if (other == null)
					return false;

				return Board.Equals(other.Board);
			}

			public override string ToString()
			{
				return $"G={G}, H={H}, F={G + H}";
			}


			private static IEnumerable<int> RangeExcludeExclude(int start, int to)
			{
				if (start < to)
				{
					while (++start < to)
					{
						yield return start;
					}
				}
				else
				{
					while (--start > to)
					{
						yield return start;
					}
				}
			}
		}
	}
}
