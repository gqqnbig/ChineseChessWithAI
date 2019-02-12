using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntPoint = System.Drawing.Point;

namespace 中国象棋
{
	public class Piece
	{
		internal int ID { get; }

		public string Name { get; }

		public ChessColor Color { get; set; }

		public bool CanCrossRiver { get; set; } = true;

		private readonly Func<Piece, IntPoint, Board, MoveFilters, IEnumerable<Move>> getPossibleMovements;

		public Piece(int id, string name)
		{
			this.getPossibleMovements = movesDictionary[name];
			Name = name;
			ID = id;


			if (Name == "车")
				Debug.Assert(id == 0 || id == 1);
			if (Name == "马")
				Debug.Assert(id == 2 || id == 3);
			if (Name == "象")
				Debug.Assert(id == 4 || id == 5);
			if (Name == "士")
				Debug.Assert(id == 6 || id == 7);
			if (Name == "炮")
				Debug.Assert(id == 8 || id == 9);
			if (Name == "兵")
				Debug.Assert(10 <= id && id < 15);
			if (Name == "将")
				Debug.Assert(id == 15);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="location">本棋子的位置</param>
		/// <param name="piecesOnBoard"></param>
		/// <returns></returns>
		public virtual IEnumerable<Move> GetPossibleMovements(IntPoint location, Board piecesOnBoard, MoveFilters filters = MoveFilters.None)
		{
			return getPossibleMovements(this, location, piecesOnBoard, filters);
		}

		public override string ToString()
		{
			return (Color == ChessColor.Red ? "红" : "黑") + Name;
		}



		private static readonly Dictionary<string, Func<Piece, IntPoint, Board, MoveFilters, IEnumerable<Move>>> movesDictionary
			= new Dictionary<string, Func<Piece, IntPoint, Board, MoveFilters, IEnumerable<Move>>>()
			{
				["车"] = (p, location, pieces, filters) =>
				{
					var moves = new List<Move>();
					foreach (int xDirection in new[] { -1, 1 })
					{
						for (int x = location.X + xDirection; x >= 0 && x < Board.Width; x += xDirection)
						{
							if (pieces[location.Y, x] == null)
							{
								if (filters.HasFlag(MoveFilters.PreventHorizontal) == false)
									moves.Add(new Move(new IntPoint(x, location.Y), MoveFilters.PreventHorizontal));
							}
							else if (pieces[location.Y, x].Color != p.Color) //吃子
							{
								moves.Add(new Move(new IntPoint(x, location.Y)));
								break;
							}
							else
								break;
						}
					}

					foreach (int yDirection in new[] { -1, 1 })
					{
						for (int y = location.Y + yDirection; y >= 0 && y < Board.Height; y += yDirection)
						{
							if (pieces[y, location.X] == null)
							{
								if (filters.HasFlag(MoveFilters.PreventVertical) == false)
									moves.Add(new Move(new IntPoint(location.X, y), MoveFilters.PreventVertical));
							}
							else if (pieces[y, location.X].Color != p.Color) //吃子
							{
								moves.Add(new Move(new IntPoint(location.X, y)));
								break;
							}
							else
								break;
						}
					}

					return moves;
				},

				["马"] = (p, location, pieces, filters) =>
				{
					var moves = new List<Move>();
					if (location.X - 2 >= 0)
					{
						if (pieces[location.Y, location.X - 1] == null)
						{
							//如果为null，则?.Color返回null，null!=p.Color。
							if (location.Y - 1 >= 0 && pieces[location.Y - 1, location.X - 2]?.Color != p.Color)
								moves.Add(new Move(new IntPoint(location.X - 2, location.Y - 1)));
							if (location.Y + 1 <= 9 && pieces[location.Y + 1, location.X - 2]?.Color != p.Color)
								moves.Add(new Move(new IntPoint(location.X - 2, location.Y + 1)));
						}
					}
					if (location.X + 2 < 9)
					{
						if (pieces[location.Y, location.X + 1] == null)
						{
							if (location.Y - 1 >= 0 && pieces[location.Y - 1, location.X + 2]?.Color != p.Color)
								moves.Add(new Move(new IntPoint(location.X + 2, location.Y - 1)));
							if (location.Y + 1 <= 9 && pieces[location.Y + 1, location.X + 2]?.Color != p.Color)
								moves.Add(new Move(new IntPoint(location.X + 2, location.Y + 1)));
						}
					}

					if (location.Y - 2 >= 0)
					{
						if (pieces[location.Y - 1, location.X] == null)
						{
							if (location.X - 1 >= 0 && pieces[location.Y - 2, location.X - 1]?.Color != p.Color)
								moves.Add(new Move(new IntPoint(location.X - 1, location.Y - 2)));
							if (location.X < 8 && pieces[location.Y - 2, location.X + 1]?.Color != p.Color)
								moves.Add(new Move(new IntPoint(location.X + 1, location.Y - 2)));
						}
					}

					if (location.Y + 2 < 10)
					{
						if (pieces[location.Y + 1, location.X] == null)
						{
							if (location.X - 1 >= 0 && pieces[location.Y + 2, location.X - 1]?.Color != p.Color)
								moves.Add(new Move(new IntPoint(location.X - 1, location.Y + 2)));
							if (location.X < 8 && pieces[location.Y + 2, location.X + 1]?.Color != p.Color)
								moves.Add(new Move(new IntPoint(location.X + 1, location.Y + 2)));
						}
					}

					return moves;
				},

				["象"] = (p, location, pieces, filters) =>
				{
					var moves = new List<Move>();
					if (location.Y - 2 >= 0 && location.X - 2 >= 0 && location.Y <= 4 && pieces[location.Y - 2, location.X - 2]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X - 2, location.Y - 2)));

					if (location.Y - 2 >= 0 && location.X + 2 < 9 && location.Y <= 4 && pieces[location.Y - 2, location.X + 2]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X + 2, location.Y - 2)));

					if (location.X - 2 >= 0 && location.Y <= 2 && pieces[location.Y + 2, location.X - 2]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X - 2, location.Y + 2)));

					if (location.X + 2 < 9 && location.Y <= 2 && pieces[location.Y + 2, location.X + 2]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X + 2, location.Y + 2)));


					if (location.X - 2 >= 0 && location.Y >= 7 && pieces[location.Y - 2, location.X - 2]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X - 2, location.Y - 2)));

					if (location.X + 2 < 9 && location.Y >= 7 && pieces[location.Y - 2, location.X + 2]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X + 2, location.Y - 2)));

					if (location.Y >= 5 && location.X - 2 >= 0 && location.Y + 2 <= 9 && pieces[location.Y + 2, location.X - 2]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X - 2, location.Y + 2)));

					if (location.Y >= 5 && location.X + 2 < 9 && location.Y + 2 <= 9 && pieces[location.Y + 2, location.X + 2]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X + 2, location.Y + 2)));

					return moves;
				},

				["士"] = (p, location, pieces, filters) =>
				{
					var moves = new List<Move>();
					if (location.X - 1 >= 0 && location.Y - 1 >= 0 && location.Y <= 2 && location.X >= 4 && pieces[location.Y - 1, location.X - 1]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X - 1, location.Y - 1)));

					if (location.X + 1 >= 0 && location.Y - 1 >= 0 && location.Y <= 2 && location.X <= 4 && pieces[location.Y - 1, location.X + 1]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X + 1, location.Y - 1)));

					if (location.X - 1 >= 0 && location.Y <= 1 && location.X >= 4 && pieces[location.Y + 1, location.X - 1]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X - 1, location.Y + 1)));

					if (location.X + 1 >= 0 && location.Y <= 1 && location.X <= 4 && pieces[location.Y + 1, location.X + 1]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X + 1, location.Y + 1)));


					if (location.X - 1 >= 0 && location.Y >= 7 && location.X >= 4 && pieces[location.Y - 1, location.X - 1]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X - 1, location.Y - 1)));

					if (location.X + 1 >= 0 && location.Y >= 7 && location.X <= 4 && pieces[location.Y - 1, location.X + 1]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X + 1, location.Y - 1)));

					if (location.X - 1 >= 0 && location.Y + 1 <= 9 && location.Y >= 7 && location.X >= 4 && pieces[location.Y + 1, location.X - 1]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X - 1, location.Y + 1)));

					if (location.X + 1 >= 0 && location.Y + 1 <= 9 && location.Y >= 7 && location.X <= 4 && pieces[location.Y + 1, location.X + 1]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X + 1, location.Y + 1)));

					return moves;
				},

				["将"] = (p, location, pieces, filters) =>
				{
					var moves = new List<Move>();
					if (location.X >= 4 && pieces[location.Y, location.X - 1]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X - 1, location.Y)));

					if (location.X <= 4 && pieces[location.Y, location.X + 1]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X + 1, location.Y)));

					if (1 <= location.Y && location.Y <= 2 && pieces[location.Y - 1, location.X]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X, location.Y - 1)));

					if (location.Y <= 1 && pieces[location.Y + 1, location.X]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X, location.Y + 1)));

					//将吃将
					if (location.Y <= 2)
					{
						for (int y = location.Y + 1; y < 10; y++)
						{
							if (pieces[y, location.X] == null)
								continue;
							else if (pieces[y, location.X].Name == "将")
							{
								moves.Add(new Move(new IntPoint(location.X, y)));
							}
							break;
						}
					}


					if (8 <= location.Y && pieces[location.Y - 1, location.X]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X, location.Y - 1)));

					if (7 <= location.Y && location.Y <= 8 && pieces[location.Y + 1, location.X]?.Color != p.Color)
						moves.Add(new Move(new IntPoint(location.X, location.Y + 1)));

					if (location.Y > 2)
					{
						for (int y = location.Y - 1; y >= 0; y--)
						{
							if (pieces[y, location.X] == null)
								continue;
							else if (pieces[y, location.X].Name == "将")
							{
								moves.Add(new Move(new IntPoint(location.X, y)));
							}
							break;
						}
					}

					return moves;
				},

				["炮"] = (p, location, pieces, filters) =>
				{
					var moves = new List<Move>();
					foreach (int xDirection in new[] { -1, 1 })
					{
						for (int x = location.X + xDirection; x >= 0 && x < Board.Width; x += xDirection)
						{
							if (pieces[location.Y, x] == null)
							{
								if (filters.HasFlag(MoveFilters.PreventHorizontal) == false)
									moves.Add(new Move(new IntPoint(x, location.Y), MoveFilters.PreventHorizontal));
							}
							else
							{   //吃子
								for (int xx = x + xDirection; xx >= 0 && xx < Board.Width; xx += xDirection)
								{
									if (pieces[location.Y, xx] != null)
									{
										if (pieces[location.Y, xx].Color != p.Color)
											moves.Add(new Move(new IntPoint(xx, location.Y)));
										break;
									}
								}
								break;
							}
						}
					}

					foreach (int yDirection in new[] { -1, 1 })
					{
						for (int y = location.Y + yDirection; y >= 0 && y < Board.Height; y += yDirection)
						{
							if (pieces[y, location.X] == null)
							{
								if (filters.HasFlag(MoveFilters.PreventVertical) == false)
									moves.Add(new Move(new IntPoint(location.X, y), MoveFilters.PreventVertical));
							}
							else
							{
								for (int yy = y + yDirection; yy >= 0 && yy < Board.Height; yy += yDirection)
								{
									if (pieces[yy, location.X] != null)
									{
										if (pieces[yy, location.X].Color != p.Color)
											moves.Add(new Move(new IntPoint(location.X, yy)));
										break;
									}
								}
								break;
							}
						}
					}

					return moves;
				},

				["兵"] = (p, location, pieces, filters) =>
				{
					var moves = new List<Move>();
					var jiangLocation = pieces.GetOppositeJiangLocation(p.Color);
					if (jiangLocation.Y <= 4)
					{
						if (location.Y - 1 >= 0 && pieces[location.Y - 1, location.X]?.Color != p.Color)
							moves.Add(new Move(new IntPoint(location.X, location.Y - 1)));
						if (location.Y <= 4) //如果过河了，就可以左右移动
						{
							if (location.X >= 1 && pieces[location.Y, location.X - 1]?.Color != p.Color)
								moves.Add(new Move(new IntPoint(location.X - 1, location.Y)));

							if (location.X <= 7 && pieces[location.Y, location.X + 1]?.Color != p.Color)
								moves.Add(new Move(new IntPoint(location.X + 1, location.Y)));
						}
					}
					else
					{
						if (location.Y + 1 < Board.Height && pieces[location.Y + 1, location.X]?.Color != p.Color)
							moves.Add(new Move(new IntPoint(location.X, location.Y + 1)));
						if (location.Y > 4) //如果过河了，就可以左右移动
						{
							if (location.X >= 1 && pieces[location.Y, location.X - 1]?.Color != p.Color)
								moves.Add(new Move(new IntPoint(location.X - 1, location.Y)));

							if (location.X <= 7 && pieces[location.Y, location.X + 1]?.Color != p.Color)
								moves.Add(new Move(new IntPoint(location.X + 1, location.Y)));
						}
					}

					return moves;
				}
			};

	}


	public struct Move
	{
		public Move(IntPoint destination, MoveFilters preventNext = MoveFilters.None)
		{
			Destination = destination;
			PreventNext = preventNext;
		}

		public IntPoint Destination { get; }

		public MoveFilters PreventNext { get; }


	}

	[Flags]
	public enum MoveFilters
	{
		None = 0,
		/// <summary>
		/// 不要进行自由的水平移动，吃子不算。
		/// </summary>
		PreventHorizontal = 1,
		/// <summary>
		/// 不要进行自由的垂直移动，吃子不算。
		/// </summary>
		PreventVertical = 2
	}


	public enum ChessColor
	{
		Black,
		Red
	}
}
