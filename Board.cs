using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using IntPoint = System.Drawing.Point;

namespace 中国象棋
{
	public class Board : IReadOnlyCollection<Piece>
	{
		public const int Width = 9;
		public const int Height = 10;

		readonly Piece[,] board;

		public Piece this[int y, int x] => board[y, x];

		IntPoint? redJiangLocation;
		public IntPoint RedJiangLocation
		{
			get
			{
				if (redJiangLocation == null)
					redJiangLocation = CoordinatesOf(board, p => p.Name == "将" && p.Color == ChessColor.Red);

				return redJiangLocation.Value;
			}
		}

		IntPoint? blackJiangLocation;
		public IntPoint BlackJiangLocation
		{
			get
			{
				if (blackJiangLocation == null)
					blackJiangLocation = CoordinatesOf(board, p => p.Name == "将" && p.Color == ChessColor.Black);

				return blackJiangLocation.Value;
			}
		}

		private Board(Piece[,] board)
		{
			this.board = board;
		}

		/// <summary>
		/// 初始化棋盘，棋子以标准布局开始。
		/// </summary>
		public Board()
		{
			board = new Piece[10, 9];

			Func<Piece, IntPoint, Board, IEnumerable<IntPoint>> moves车 = (p, location, pieces) =>
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
			};

			Func<Piece, IntPoint, Board, IEnumerable<IntPoint>> moves马 = (p, location, pieces) =>
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
			};

			Func<Piece, IntPoint, Board, IEnumerable<IntPoint>> moves象 = (p, location, pieces) =>
			{
				List<IntPoint> moves = new List<IntPoint>();
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
			};

			Func<Piece, IntPoint, Board, IEnumerable<IntPoint>> moves士 = (p, location, pieces) =>
			{
				List<IntPoint> moves = new List<IntPoint>();
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
			};

			Func<Piece, IntPoint, Board, IEnumerable<IntPoint>> moves将 = (p, location, pieces) =>
			{
				List<IntPoint> moves = new List<IntPoint>();
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
			};

			Func<Piece, IntPoint, Board, IEnumerable<IntPoint>> moves炮 = (p, location, pieces) =>
			{
				List<IntPoint> moves = new List<IntPoint>();
				for (int x = location.X - 1; x >= 0; x--)
				{
					if (pieces[location.Y, x] == null)
						moves.Add(new IntPoint(x, location.Y));
					else
					{   //吃子
						for (int xx = x - 1; xx >= 0; xx--)
						{
							if (pieces[location.Y, xx] != null && pieces[location.Y, xx].Color != p.Color)
							{
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
							if (pieces[location.Y, xx] != null && pieces[location.Y, xx].Color != p.Color)
							{
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
							if (pieces[yy, location.X] != null && pieces[yy, location.X].Color != p.Color)
							{
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
							if (pieces[yy, location.X] != null && pieces[yy, location.X].Color != p.Color)
							{
								moves.Add(new IntPoint(location.X, yy));
								break;
							}
						}
						break;
					}
				}
				return moves;
			};

			Func<Piece, IntPoint, Board, IEnumerable<IntPoint>> moves兵 = (p, location, pieces) =>
			{
				List<IntPoint> moves = new List<IntPoint>();
				var jiangLocation = p.Color == ChessColor.Red ? pieces.RedJiangLocation : pieces.BlackJiangLocation;
				if (jiangLocation.Y <= 4)
				{
					if (pieces[location.Y + 1, location.X]?.Color != p.Color)
						moves.Add(new IntPoint(location.X, location.Y + 1));
					if (location.Y > 4) //如果过河了，就可以左右移动
					{
						if (location.X >= 1 && pieces[location.Y, location.X - 1]?.Color != p.Color)
							moves.Add(new IntPoint(location.X - 1, location.Y));

						if (location.X <= 7 && pieces[location.Y, location.X + 1]?.Color != p.Color)
							moves.Add(new IntPoint(location.X + 1, location.Y));
					}
				}
				else
				{
					if (location.Y >= 1 && pieces[location.Y - 1, location.X]?.Color != p.Color)
						moves.Add(new IntPoint(location.X, location.Y - 1));
					if (location.Y <= 4) //如果过河了，就可以左右移动
					{
						if (location.X >= 1 && pieces[location.Y, location.X - 1]?.Color != p.Color)
							moves.Add(new IntPoint(location.X - 1, location.Y));

						if (location.X <= 7 && pieces[location.Y, location.X + 1]?.Color != p.Color)
							moves.Add(new IntPoint(location.X + 1, location.Y));
					}
				}
				return moves;

			};

			board[0, 0] = new Piece(moves车) { Name = "车", Color = ChessColor.Black };
			board[0, 1] = new Piece(moves马) { Name = "马", Color = ChessColor.Black };
			board[0, 2] = new Piece(moves象) { Name = "象", Color = ChessColor.Black, CanCrossRiver = false };
			board[0, 3] = new Piece(moves士) { Name = "士", Color = ChessColor.Black, CanCrossRiver = false };
			board[0, 4] = new Piece(moves将) { Name = "将", Color = ChessColor.Black, CanCrossRiver = false };
			board[0, 5] = new Piece(moves士) { Name = "士", Color = ChessColor.Black, CanCrossRiver = false };
			board[0, 6] = new Piece(moves象) { Name = "象", Color = ChessColor.Black, CanCrossRiver = false };
			board[0, 7] = new Piece(moves马) { Name = "马", Color = ChessColor.Black };
			board[0, 8] = new Piece(moves车) { Name = "车", Color = ChessColor.Black };

			board[2, 1] = new Piece(moves炮) { Name = "炮", Color = ChessColor.Black };
			board[2, 7] = new Piece(moves炮) { Name = "炮", Color = ChessColor.Black };

			board[3, 0] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black };
			board[3, 2] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black };
			board[3, 4] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black };
			board[3, 6] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black };
			board[3, 8] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black };

			board[9, 0] = new Piece(moves车) { Name = "车", Color = ChessColor.Red };
			board[9, 1] = new Piece(moves马) { Name = "马", Color = ChessColor.Red };
			board[9, 2] = new Piece(moves象) { Name = "象", Color = ChessColor.Red, CanCrossRiver = false };
			board[9, 3] = new Piece(moves士) { Name = "士", Color = ChessColor.Red, CanCrossRiver = false };
			board[9, 4] = new Piece(moves将) { Name = "将", Color = ChessColor.Red, CanCrossRiver = false };
			board[9, 5] = new Piece(moves士) { Name = "士", Color = ChessColor.Red, CanCrossRiver = false };
			board[9, 6] = new Piece(moves象) { Name = "象", Color = ChessColor.Red, CanCrossRiver = false };
			board[9, 7] = new Piece(moves马) { Name = "马", Color = ChessColor.Red };
			board[9, 8] = new Piece(moves车) { Name = "车", Color = ChessColor.Red };

			board[7, 1] = new Piece(moves炮) { Name = "炮", Color = ChessColor.Red };
			board[7, 7] = new Piece(moves炮) { Name = "炮", Color = ChessColor.Red };

			board[6, 0] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Red };
			board[6, 2] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Red };
			board[6, 4] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Red };
			board[6, 6] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Red };
			board[6, 8] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Red };
		}

		public Board Move(IntPoint location, IntPoint targetLocation, out Piece eatenPiece)
		{
			Debug.Assert(board[location.Y, location.X] != null, $"想要从({location.X},{location.Y})走子，但该位置没有棋子。");

			var newBoard = (Piece[,])board.Clone();
			eatenPiece = newBoard[targetLocation.Y, targetLocation.X];

			newBoard[targetLocation.Y, targetLocation.X] = newBoard[location.Y, location.X];
			newBoard[location.Y, location.X] = null;

			return new Board(newBoard);
		}


		public IntPoint GetMyJiangLocation(ChessColor color)
		{
			if (color == ChessColor.Red)
				return RedJiangLocation;
			else
				return BlackJiangLocation;
		}

		public IntPoint GetOppositeJiangLocation(ChessColor color)
		{
			if (color == ChessColor.Black)
				return RedJiangLocation;
			else
				return BlackJiangLocation;
		}


		public override bool Equals(object obj)
		{
			var other = obj as Board;

			for (int x = 0; x < 9; x++)
			{
				for (int y = 0; y < 10; y++)
				{
					if (board[y, x] != null && other.board[y, x] != null)
					{
						if (board[y, x].Name == other.board[y, x].Name &&
							board[y, x].Color == other.board[y, x].Color)
							continue;
						else
							return false;
					}
					else if (board[y, x] == null && other.board[y, x] == null)
						continue;
					else
						return false;
				}
			}

			return true;
		}

		int? hashcode;

		public override int GetHashCode()
		{
			if (hashcode == null)
			{
				hashcode = 0;
				for (int x = 0; x < 9; x++)
				{
					for (int y = 0; y < 10; y++)
					{
						if (board[y, x] != null)
						{
							hashcode = hashcode ^ x;
							hashcode = hashcode ^ y;
							hashcode = hashcode ^ board[y, x].Name.GetHashCode();
							hashcode = hashcode ^ board[y, x].Color.GetHashCode();
						}
					}
				}
			}
			return hashcode.Value;
		}

		private static IntPoint CoordinatesOf<T>(T[,] matrix, Predicate<T> p)
		{
			int w = matrix.GetLength(0); // width
			int h = matrix.GetLength(1); // height

			for (int x = 0; x < w; ++x)
			{
				for (int y = 0; y < h; ++y)
				{
					if (matrix[x, y] != null && p(matrix[x, y]))
						return new IntPoint(y, x);
				}
			}

			return new IntPoint(-1, -1);
		}


		public int Count => throw new NotImplementedException();

		public IEnumerator<Piece> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public void PrintPretty()
		{
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if (this[y, x] == null)
						Console.Write("＋");
					else
					{
						var backup = Console.ForegroundColor;
						Console.ForegroundColor = this[y, x].Color == ChessColor.Red ? ConsoleColor.Red : ConsoleColor.Black;
						Console.Write(this[y,x].Name);
						Console.ForegroundColor = backup;
					}
				}
				Console.WriteLine();
			}

		}

	}
}
