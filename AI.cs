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
			var openList = new PriorityQueue<SearchState>((a, b) => b.G - a.G);
			openList.Enqueue(new SearchState { Location = location, G = 0, Board = board });

			var closedList = new HashSet<SearchState>();

			while (openList.Count > 0)
			{
				var searchState = openList.Dequeue();
				closedList.Add(searchState);

				if (MainWindow.IsWin(searchState.Board, searchState.TargetPiece.Color).GetValueOrDefault(false))
					return searchState.G;

				foreach (var movement in searchState.TargetPiece.GetPossibleMovements(searchState.Location, searchState.Board))
				{
					var nextBoard = board.Move(location, movement, out _);
					var s = new SearchState { Location = movement, Board = nextBoard, G = searchState.G + 1 };
					if (closedList.Contains(s)==false && CanBeEaten(movement, nextBoard) == false)
						openList.Enqueue(s);
				}
			}


			return int.MaxValue;
		}


		class SearchState
		{
			public IntPoint Location { get; set; }
			public int H => HeuristicToJiang(Location, Board.GetMyJiangLocation(TargetPiece.Color));
			public int G { get; set; }
			public int F => H + G;

			public Board Board { get; set; }

			public Piece TargetPiece => Board[Location.Y, Location.X];


			private static int HeuristicToJiang(IntPoint pieceLocation, IntPoint jiangLocation)
			{
				return Math.Abs(pieceLocation.X - jiangLocation.X) + Math.Abs(pieceLocation.Y - jiangLocation.Y);
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
