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

		private readonly Func<Piece, IntPoint, Board, IEnumerable<IntPoint>> getPossibleMovements;

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
		public virtual IEnumerable<IntPoint> GetPossibleMovements(IntPoint location, Board piecesOnBoard)
		{
			return getPossibleMovements(this, location, piecesOnBoard);
		}

		public override string ToString()
		{
			return (Color == ChessColor.Red ? "红" : "黑") + Name;
		}



		private static readonly Dictionary<string, Func<Piece, IntPoint, Board, IEnumerable<IntPoint>>> movesDictionary
			= new Dictionary<string, Func<Piece, IntPoint, Board, IEnumerable<IntPoint>>>()
			{
				["车"] = (p, location, pieces) =>
				{
					List<IntPoint> moves = new List<IntPoint>();
					for (int x = location.X - 1; x >= 0; x--)
					{
						if (pieces[location.Y, x] == null)
							moves.Add(new IntPoint(x, location.Y));
						else if (pieces[location.Y, x].Color != p.Color) //吃子
						{
							moves.Add(new IntPoint(x, location.Y));
							break;
						}
						else
							break;
					}

					for (int x = location.X + 1; x < 9; x++)
					{
						if (pieces[location.Y, x] == null)
							moves.Add(new IntPoint(x, location.Y));
						else if (pieces[location.Y, x].Color != p.Color) //吃子
						{
							moves.Add(new IntPoint(x, location.Y));
							break;
						}
						else
							break;
					}

					for (int y = location.Y - 1; y >= 0; y--)
					{
						if (pieces[y, location.X] == null)
							moves.Add(new IntPoint(location.X, y));
						else if (pieces[y, location.X].Color != p.Color) //吃子
						{
							moves.Add(new IntPoint(location.X, y));
							break;
						}
						else
							break;
					}

					for (int y = location.Y + 1; y < 10; y++)
					{
						if (pieces[y, location.X] == null)
							moves.Add(new IntPoint(location.X, y));
						else if (pieces[y, location.X].Color != p.Color) //吃子
						{
							moves.Add(new IntPoint(location.X, y));
							break;
						}
						else
							break;
					}
					return moves;
				},

				["马"] = (p, location, pieces) =>
				{
					List<IntPoint> moves = new List<IntPoint>();
					if (location.X - 2 >= 0)
					{
						if (pieces[location.Y, location.X - 1] == null)
						{
							//如果为null，则?.Color返回null，null!=p.Color。
							if (location.Y - 1 >= 0 && pieces[location.Y - 1, location.X - 2]?.Color != p.Color)
								moves.Add(new IntPoint(location.X - 2, location.Y - 1));
							if (location.Y + 1 <= 9 && pieces[location.Y + 1, location.X - 2]?.Color != p.Color)
								moves.Add(new IntPoint(location.X - 2, location.Y + 1));
						}
					}
					if (location.X + 2 < 9)
					{
						if (pieces[location.Y, location.X + 1] == null)
						{
							if (location.Y - 1 >= 0 && pieces[location.Y - 1, location.X + 2]?.Color != p.Color)
								moves.Add(new IntPoint(location.X + 2, location.Y - 1));
							if (location.Y + 1 <= 9 && pieces[location.Y + 1, location.X + 2]?.Color != p.Color)
								moves.Add(new IntPoint(location.X + 2, location.Y + 1));
						}
					}

					if (location.Y - 2 >= 0)
					{
						if (pieces[location.Y - 1, location.X] == null)
						{
							if (location.X - 1 >= 0 && pieces[location.Y - 2, location.X - 1]?.Color != p.Color)
								moves.Add(new IntPoint(location.X - 1, location.Y - 2));
							if (location.X < 8 && pieces[location.Y - 2, location.X + 1]?.Color != p.Color)
								moves.Add(new IntPoint(location.X + 1, location.Y - 2));
						}
					}

					if (location.Y + 2 < 10)
					{
						if (pieces[location.Y + 1, location.X] == null)
						{
							if (location.X - 1 >= 0 && pieces[location.Y + 2, location.X - 1]?.Color != p.Color)
								moves.Add(new IntPoint(location.X - 1, location.Y + 2));
							if (location.X < 8 && pieces[location.Y + 2, location.X + 1]?.Color != p.Color)
								moves.Add(new IntPoint(location.X + 1, location.Y + 2));
						}
					}

					return moves;
				},

				["象"] = (p, location, pieces) =>
				{
					var moves = new List<IntPoint>();
					if (location.Y - 2 >= 0 && location.X - 2 >= 0 && location.Y <= 4 && pieces[location.Y - 2, location.X - 2]?.Color != p.Color)
						moves.Add(new IntPoint(location.X - 2, location.Y - 2));

					if (location.Y - 2 >= 0 && location.X + 2 < 9 && location.Y <= 4 && pieces[location.Y - 2, location.X + 2]?.Color != p.Color)
						moves.Add(new IntPoint(location.X + 2, location.Y - 2));

					if (location.X - 2 >= 0 && location.Y <= 2 && pieces[location.Y + 2, location.X - 2]?.Color != p.Color)
						moves.Add(new IntPoint(location.X - 2, location.Y + 2));

					if (location.X + 2 < 9 && location.Y <= 2 && pieces[location.Y + 2, location.X + 2]?.Color != p.Color)
						moves.Add(new IntPoint(location.X + 2, location.Y + 2));


					if (location.X - 2 >= 0 && location.Y >= 7 && pieces[location.Y - 2, location.X - 2]?.Color != p.Color)
						moves.Add(new IntPoint(location.X - 2, location.Y - 2));

					if (location.X + 2 < 9 && location.Y >= 7 && pieces[location.Y - 2, location.X + 2]?.Color != p.Color)
						moves.Add(new IntPoint(location.X + 2, location.Y - 2));

					if (location.Y >= 5 && location.X - 2 >= 0 && location.Y + 2 <= 9 && pieces[location.Y + 2, location.X - 2]?.Color != p.Color)
						moves.Add(new IntPoint(location.X - 2, location.Y + 2));

					if (location.Y >= 5 && location.X + 2 < 9 && location.Y + 2 <= 9 && pieces[location.Y + 2, location.X + 2]?.Color != p.Color)
						moves.Add(new IntPoint(location.X + 2, location.Y + 2));

					return moves;
				},

				["士"] = (p, location, pieces) =>
				{
					var moves = new List<IntPoint>();
					if (location.X - 1 >= 0 && location.Y - 1 >= 0 && location.Y <= 2 && location.X >= 4 && pieces[location.Y - 1, location.X - 1]?.Color != p.Color)
						moves.Add(new IntPoint(location.X - 1, location.Y - 1));

					if (location.X + 1 >= 0 && location.Y - 1 >= 0 && location.Y <= 2 && location.X <= 4 && pieces[location.Y - 1, location.X + 1]?.Color != p.Color)
						moves.Add(new IntPoint(location.X + 1, location.Y - 1));

					if (location.X - 1 >= 0 && location.Y <= 1 && location.X >= 4 && pieces[location.Y + 1, location.X - 1]?.Color != p.Color)
						moves.Add(new IntPoint(location.X - 1, location.Y + 1));

					if (location.X + 1 >= 0 && location.Y <= 1 && location.X <= 4 && pieces[location.Y + 1, location.X + 1]?.Color != p.Color)
						moves.Add(new IntPoint(location.X + 1, location.Y + 1));


					if (location.X - 1 >= 0 && location.Y >= 7 && location.X >= 4 && pieces[location.Y - 1, location.X - 1]?.Color != p.Color)
						moves.Add(new IntPoint(location.X - 1, location.Y - 1));

					if (location.X + 1 >= 0 && location.Y >= 7 && location.X <= 4 && pieces[location.Y - 1, location.X + 1]?.Color != p.Color)
						moves.Add(new IntPoint(location.X + 1, location.Y - 1));

					if (location.X - 1 >= 0 && location.Y + 1 <= 9 && location.Y >= 7 && location.X >= 4 && pieces[location.Y + 1, location.X - 1]?.Color != p.Color)
						moves.Add(new IntPoint(location.X - 1, location.Y + 1));

					if (location.X + 1 >= 0 && location.Y + 1 <= 9 && location.Y >= 7 && location.X <= 4 && pieces[location.Y + 1, location.X + 1]?.Color != p.Color)
						moves.Add(new IntPoint(location.X + 1, location.Y + 1));

					return moves;
				},

				["将"] = (p, location, pieces) =>
				{
					var moves = new List<IntPoint>();
					if (location.X >= 4 && pieces[location.Y, location.X - 1]?.Color != p.Color)
						moves.Add(new IntPoint(location.X - 1, location.Y));

					if (location.X <= 4 && pieces[location.Y, location.X + 1]?.Color != p.Color)
						moves.Add(new IntPoint(location.X + 1, location.Y));

					if (1 <= location.Y && location.Y <= 2 && pieces[location.Y - 1, location.X]?.Color != p.Color)
						moves.Add(new IntPoint(location.X, location.Y - 1));

					if (location.Y <= 1 && pieces[location.Y + 1, location.X]?.Color != p.Color)
						moves.Add(new IntPoint(location.X, location.Y + 1));

					//将吃将
					if (location.Y <= 2)
					{
						for (int y = location.Y + 1; y < 10; y++)
						{
							if (pieces[y, location.X] == null)
								continue;
							else if (pieces[y, location.X].Name == "将")
							{
								moves.Add(new IntPoint(location.X, y));
							}
							break;
						}
					}


					if (8 <= location.Y && pieces[location.Y - 1, location.X]?.Color != p.Color)
						moves.Add(new IntPoint(location.X, location.Y - 1));

					if (7 <= location.Y && location.Y <= 8 && pieces[location.Y + 1, location.X]?.Color != p.Color)
						moves.Add(new IntPoint(location.X, location.Y + 1));

					if (location.Y > 2)
					{
						for (int y = location.Y - 1; y >= 0; y--)
						{
							if (pieces[y, location.X] == null)
								continue;
							else if (pieces[y, location.X].Name == "将")
							{
								moves.Add(new IntPoint(location.X, y));
							}
							break;
						}
					}

					return moves;
				},

				["炮"] = (p, location, pieces) =>
				{
					var moves = new List<IntPoint>();
					for (int x = location.X - 1; x >= 0; x--)
					{
						if (pieces[location.Y, x] == null)
							moves.Add(new IntPoint(x, location.Y));
						else
						{   //吃子
							for (int xx = x - 1; xx >= 0; xx--)
							{
								if (pieces[location.Y, xx] != null)
								{
									if (pieces[location.Y, xx].Color != p.Color)
										moves.Add(new IntPoint(xx, location.Y));
									break;
								}
							}
							break;
						}
					}

					for (int x = location.X + 1; x < 9; x++)
					{
						if (pieces[location.Y, x] == null)
							moves.Add(new IntPoint(x, location.Y));
						else
						{   //吃子
							for (int xx = x + 1; xx < 9; xx++)
							{
								if (pieces[location.Y, xx] != null)
								{
									if (pieces[location.Y, xx].Color != p.Color)
										moves.Add(new IntPoint(xx, location.Y));
									break;
								}
							}
							break;
						}
					}

					for (int y = location.Y - 1; y >= 0; y--)
					{
						if (pieces[y, location.X] == null)
							moves.Add(new IntPoint(location.X, y));
						else
						{
							for (int yy = y - 1; yy >= 0; yy--)
							{
								if (pieces[yy, location.X] != null)
								{
									if (pieces[yy, location.X].Color != p.Color)
										moves.Add(new IntPoint(location.X, yy));
									break;
								}
							}
							break;
						}
					}

					for (int y = location.Y + 1; y < 10; y++)
					{
						if (pieces[y, location.X] == null)
							moves.Add(new IntPoint(location.X, y));
						else
						{
							for (int yy = y + 1; yy < 10; yy++)
							{
								if (pieces[yy, location.X] != null)
								{
									if (pieces[yy, location.X].Color != p.Color)
										moves.Add(new IntPoint(location.X, yy));
									break;
								}
							}
							break;
						}
					}
					return moves;
				},

				["兵"] = (p, location, pieces) =>
				{
					var moves = new List<IntPoint>();
					var jiangLocation = pieces.GetOppositeJiangLocation(p.Color);
					if (jiangLocation.Y <= 4)
					{
						if (location.Y - 1 >= 0 && pieces[location.Y - 1, location.X]?.Color != p.Color)
							moves.Add(new IntPoint(location.X, location.Y - 1));
						if (location.Y <= 4) //如果过河了，就可以左右移动
						{
							if (location.X >= 1 && pieces[location.Y, location.X - 1]?.Color != p.Color)
								moves.Add(new IntPoint(location.X - 1, location.Y));

							if (location.X <= 7 && pieces[location.Y, location.X + 1]?.Color != p.Color)
								moves.Add(new IntPoint(location.X + 1, location.Y));
						}
					}
					else
					{
						if (location.Y + 1 < Board.Height && pieces[location.Y + 1, location.X]?.Color != p.Color)
							moves.Add(new IntPoint(location.X, location.Y + 1));
						if (location.Y > 4) //如果过河了，就可以左右移动
						{
							if (location.X >= 1 && pieces[location.Y, location.X - 1]?.Color != p.Color)
								moves.Add(new IntPoint(location.X - 1, location.Y));

							if (location.X <= 7 && pieces[location.Y, location.X + 1]?.Color != p.Color)
								moves.Add(new IntPoint(location.X + 1, location.Y));
						}
					}

					return moves;
				}
			};

	}

	public enum ChessColor
	{
		Black,
		Red
	}
}
