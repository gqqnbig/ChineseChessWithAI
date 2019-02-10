using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using IntPoint = System.Drawing.Point;

namespace 中国象棋
{
	class PathFinder
	{

		public static int FindMovesCountToJiang(Piece piece, Point location, Board pieces)
		{
			return FindMovesCountToJiang(piece, location, new BoardEncoded(pieces));
		}

		/// <summary>
		/// 查找吃到将的步数。避免走步中被对方子吃掉，如果没有不被对方吃掉的步数，则返回<see cref="int.MaxValue"/>。
		/// </summary>
		/// <returns></returns>
		public static int FindMovesCountToJiang(Piece piece, IntPoint location, BoardEncoded board)
		{
			if (piece.Name == "士" || piece.Name == "象")
				return int.MaxValue;

			Stopwatch sw = new Stopwatch();
			sw.Start();

			var openList = new PriorityQueue<SearchState>((a, b) => a.F.CompareTo(b.F), 9 * 10);
			openList.Enqueue(new SearchState { Location = location, G = 0, Board = board });

			var closedList = new HashSet<Board>();

			var expandedNodes = 0;
			while (openList.Count > 0)
			{
				var searchState = openList.Dequeue();

				_ = searchState.H;

				closedList.Add(searchState.Board);

				if (MainWindow.IsWin(searchState.Board, searchState.TargetPiece.Color).GetValueOrDefault(false))
				{
					sw.Stop();
					Debug.WriteLine($"ExpandedNodes={expandedNodes}，用时{sw.ElapsedMilliseconds}。");
					return searchState.G;
				}

				expandedNodes++;
				foreach (var movement in searchState.TargetPiece.GetPossibleMovements(searchState.Location, searchState.Board))
				{
					if (movement.IsProtecting)
						continue;

					BoardEncoded nextBoard = (BoardEncoded)searchState.Board.Move(searchState.Location, movement.Target, out _);
					if (nextBoard.CanBeEaten(movement.Target) == false)
					{
						if (closedList.Contains(nextBoard) == false)
							openList.Enqueue(new SearchState { Location = movement.Target, Board = nextBoard, G = searchState.G + 1 });
					}
				}
			}

			sw.Stop();
			Debug.WriteLine($"ExpandedNodes={expandedNodes}，用时{sw.ElapsedMilliseconds}。");
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


			//public SearchedState GetSearchedRepresentation()
			//{

			//	return new SearchedState(Location, EatenPieces);
			//}


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

		//class SearchedState : IEquatable<SearchedState>
		//{
		//	private readonly SortedSet<IntPoint> eatenPieces;

		//	public IntPoint Location { get; }


		//	/// <summary>
		//	/// 被吃掉的棋子
		//	/// </summary>
		//	public IReadOnlyCollection<IntPoint> EatenPieces => eatenPieces;

		//	public SearchedState(IntPoint location, IEnumerable<IntPoint> eatenPieces)
		//	{
		//		Location = location;
		//		this.eatenPieces = new SortedSet<IntPoint>(eatenPieces,new PointComparer());
		//	}

		//	public bool Equals(SearchedState other)
		//	{
		//		if (ReferenceEquals(null, other)) return false;
		//		if (ReferenceEquals(this, other)) return true;

		//		return Location == other.Location && eatenPieces.SetEquals(other.eatenPieces);
		//	}

		//	public override bool Equals(object obj)
		//	{
		//		if (ReferenceEquals(null, obj)) return false;
		//		if (ReferenceEquals(this, obj)) return true;
		//		if (obj.GetType() != this.GetType()) return false;
		//		return Equals((SearchedState)obj);
		//	}

		//	public override int GetHashCode()
		//	{
		//		unchecked
		//		{
		//			int h = 0;
		//			int i = 0;
		//			foreach (IntPoint p in eatenPieces)
		//			{
		//				h ^= p.GetHashCode() << i;
		//				i++;
		//			}

		//			return (Location.GetHashCode() * 397) ^ h;
		//		}
		//	}


		//	class PointComparer:IComparer<IntPoint>
		//	{
		//		public int Compare(Point x, Point y)
		//		{
		//			int a = x.X - y.X;
		//			if (a != 0)
		//				return x.Y - y.Y;
		//			else
		//				return a;
		//		}
		//	}
		//}

	}

	//public class AIPiece : Piece
	//{
	//	private IEnumerable<IntPoint> cachedMoves;

	//	public AIPiece(Func<Piece, IntPoint, Board, IEnumerable<IntPoint>> getPossibleMovements) : base(getPossibleMovements)
	//	{ }

	//	public override IEnumerable<IntPoint> GetPossibleMovements(IntPoint location, Board piecesOnBoard)
	//	{
	//		if (cachedMoves == null)
	//			cachedMoves = base.GetPossibleMovements(location, piecesOnBoard);
	//		return cachedMoves;
	//	}
	//}
}
