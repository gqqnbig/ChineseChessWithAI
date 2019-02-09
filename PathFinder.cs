using System;
using System.Collections.Generic;
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

			var openList = new PriorityQueue<SearchState>((a, b) => a.F.CompareTo(b.F), 9 * 10);
			openList.Enqueue(new SearchState { Location = location, G = 0, Board = board });

			var closedList = new HashSet<Board>();

			while (openList.Count > 0)
			{
				var searchState = openList.Dequeue();
				closedList.Add(searchState.Board);

				if (MainWindow.IsWin(searchState.Board, searchState.TargetPiece.Color).GetValueOrDefault(false))
					return searchState.G;

				//不可扩展点
				if (CanBeEaten(searchState.Location, searchState.Board))
					continue;

				foreach (var movement in searchState.TargetPiece.GetPossibleMovements(searchState.Location, searchState.Board))
				{
					var nextBoard = searchState.Board.Move(searchState.Location, movement, out _);
					if (closedList.Contains(nextBoard) == false)
						openList.Enqueue(new SearchState { Location = movement, Board = nextBoard, G = searchState.G + 1 });
				}
			}


			return int.MaxValue;
		}


		class SearchState
		{
			public SearchState Parent { get; set; }

			public IntPoint Location { get; set; }
			public float H
			{
				get
				{
					var jiang = Board.GetOppositeJiangLocation(TargetPiece.Color);
					if (jiang.X == -1)
						return 0;
					switch (TargetPiece.Name)
					{
						case "车":
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
			}
			public int G { get; set; }

			/// <summary>
			/// F=G+H。类型是float，因为float有<see cref="float.PositiveInfinity"/>。
			/// </summary>
			public float F => H + G;

			public Board Board { get; set; }

			public Piece TargetPiece => Board[Location.Y, Location.X];

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
		}
	}

	public class AIPiece : Piece
	{
		private IEnumerable<IntPoint> cachedMoves;

		public AIPiece(Func<Piece, IntPoint, Board, IEnumerable<IntPoint>> getPossibleMovements) : base(getPossibleMovements)
		{ }

		public override IEnumerable<IntPoint> GetPossibleMovements(IntPoint location, Board piecesOnBoard)
		{
			if (cachedMoves == null)
				cachedMoves = base.GetPossibleMovements(location, piecesOnBoard);
			return cachedMoves;
		}
	}
}
