using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntPoint = System.Drawing.Point;

namespace 中国象棋
{
	public class PathFinder
	{
		private static List<IntPoint> BeEatenBy(IntPoint location, Board board)
		{
			List<IntPoint> list = new List<Point>();
			ChessColor color = board[location.Y, location.X].Color;
			for (int x = 0; x < 9; x++)
			{
				for (int y = 0; y < 10; y++)
				{
					if (board[y, x] == null)
						continue;

					if (board[y, x].Color != color && board[y, x].GetPossibleMovements(new IntPoint(x, y), board).Any(m => m.Destination == location))
						list.Add(new IntPoint(x, y));
				}
			}

			return list;
		}


		/// <summary>
		/// 查找吃到将的步数。避免走步中被对方子吃掉，如果没有不被对方吃掉的步数，则返回<see cref="int.MaxValue"/>。
		/// </summary>
		/// <returns></returns>
		public static int FindMovesCountToJiang(Piece piece, IntPoint location, Board board)
		{
			if (piece.Name == "士" || piece.Name == "象")
				return int.MaxValue;

			//#if DEBUG
			Stopwatch sw = new Stopwatch();
			sw.Start();
			var expandedNodes = 0;
			//#endif
			var openList = new PriorityQueue<SearchState>((a, b) => a.F.CompareTo(b.F), 9 * 10);
			var jiangLocation = board.GetOppositeJiangLocation(board[location].Color);
			openList.Enqueue(new SearchState(location, jiangLocation) { G = 0, Board = board });

			var closedList = new HashSet<Board>();

			while (openList.Count > 0)
			{
				var currentState = openList.Dequeue();

#if DEBUG
				_ = currentState.H;
#endif
				closedList.Add(currentState.Board);

				if (currentState.Location == currentState.GoalLocations.First.Value)
					currentState.GoalLocations.RemoveFirst();

				if (currentState.GoalLocations.Count == 0)
				{
					//#if DEBUG
					sw.Stop();
					Console.WriteLine($"ExpandedNodes={expandedNodes}，用时{sw.ElapsedMilliseconds}。");
					//#endif
					return currentState.G;
				}

				//#if DEBUG
				expandedNodes++;
				//#endif
				foreach (var movement in currentState.TargetPiece.GetPossibleMovements(currentState.Location, currentState.Board, currentState.MoveFilters))
				{
					var nextBoard = currentState.Board.Move(currentState.Location, movement.Destination, out _);
					if (closedList.Contains(nextBoard))
						continue;


					var controllerList = BeEatenBy(movement.Destination, nextBoard);

					//吃掉将就赢了，不用考虑自己会不会被吃掉。
					if (controllerList.Count == 0 || nextBoard.IsWin(currentState.TargetPiece.Color))
						openList.Enqueue(new SearchState(movement.Destination, jiangLocation) { Board = nextBoard, G = currentState.G + 1, MoveFilters = movement.PreventNext });
					else
					{
						//既然会被这些子吃掉，何不试试先吃掉它们。

						foreach (var controllerPoint in controllerList.Except(currentState.GoalLocations))
							openList.Enqueue(new SearchState(currentState.Location, controllerPoint, currentState.GoalLocations) { Board = currentState.Board, G = currentState.G });
					}
				}
			}

			//#if DEBUG
			sw.Stop();
			Console.WriteLine($"ExpandedNodes={expandedNodes}，用时{sw.ElapsedMilliseconds}。");
			//#endif       
			return int.MaxValue;
		}


		class SearchState
		{
			public SearchState Parent { get; set; }

			public IntPoint Location { get; }

			public LinkedList<IntPoint> GoalLocations { get; private set; }


			private float? h;


			public float H
			{
				get
				{
					if (h == null)
					{
						Debug.Assert(GoalLocations.Last.Value == Board.GetOppositeJiangLocation(TargetPiece.Color) || Board.GetOppositeJiangLocation(TargetPiece.Color).X == -1);
						h = 0;

						if (Board.GetOppositeJiangLocation(TargetPiece.Color).X != -1)
						{
							var l = Location;
							var node = GoalLocations.First;
							while (node != null)
							{
								h += GetHeuristic(l, node.Value);
								l = node.Value;
								node = node.Next;
							}

							Debug.Assert(float.IsInfinity(h.Value) == false);
						}
					}

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

			public MoveFilters MoveFilters { get; set; }

			public SearchState(IntPoint startLocation, IntPoint goalLocation)
			{
				Location = startLocation;
				GoalLocations = new LinkedList<IntPoint>();
				GoalLocations.AddLast(goalLocation);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="startLocation"></param>
			/// <param name="preGoal">在完成goalLocations之前，先完成preGoal</param>
			/// <param name="goalLocations"></param>
			public SearchState(IntPoint startLocation, IntPoint preGoal, LinkedList<IntPoint> goalLocations)
			{
				Location = startLocation;
				GoalLocations = new LinkedList<IntPoint>(goalLocations);
				GoalLocations.AddFirst(preGoal);
			}

			private float GetHeuristic(IntPoint startLocation, IntPoint goalLocation)
			{
				Debug.Assert(startLocation != goalLocation);
				Debug.Assert(Board.GetOppositeJiangLocation(TargetPiece.Color).X != -1, "应该有将。将被吃掉的情况在FindMovesCountToJiang应该已经被计算了。");

				switch (TargetPiece.Name)
				{
					case "车":
						return Get车Heuristic(startLocation, goalLocation);
					case "炮":
						if (startLocation.X != goalLocation.X && startLocation.Y != goalLocation.Y)
							return 2;
						else
							return 1;
					case "马":
						//https://stackoverflow.com/a/41704071/746461
						//answered Jan 17 '17 at 18:08
						//Anthor: simon

						int dx = Math.Abs(startLocation.X - goalLocation.X);
						int dy = Math.Abs(startLocation.Y - goalLocation.Y);

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
						return Math.Abs(startLocation.X - goalLocation.X) + Math.Abs(startLocation.Y - goalLocation.Y);

					case "将":
						if (startLocation.X == goalLocation.X)
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
				if (Board[destination]?.Color == TargetPiece.Color)
					return float.PositiveInfinity;

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
						{
							oppositePieceCount++;

							if (oppositePieceCount == 3)
								return 3;
						}
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
						{
							oppositePieceCount++;

							if (oppositePieceCount == 3)
								return 3;
						}
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
