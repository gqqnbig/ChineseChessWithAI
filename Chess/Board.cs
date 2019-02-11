using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using IntPoint = System.Drawing.Point;

namespace 中国象棋
{
	public class Board : IReadOnlyCollection<Piece>
	{
		public const int Width = 9;
		public const int Height = 10;



		readonly Piece[,] board;

		public Piece this[int y, int x] => board[y, x];

		public Piece this[IntPoint p] => board[p.Y, p.X];

		public IntPoint RedJiangLocation { get; private set; }

		public IntPoint BlackJiangLocation { get; private set; }

		public bool IsBlackWin { get; private set; }
		public bool IsRedWin { get; private set; }

		public bool IsWin(ChessColor color) => color == ChessColor.Red ? IsRedWin : IsBlackWin;


		private ulong redXEqualityRepresentation = 0;
		private ulong redYEqualityRepresentation = 0;
		private ulong blackXEqualityRepresentation = 0;
		private ulong blackYEqualityRepresentation = 0;

		private void InitilizeEqualityRepresentation()
		{
			redXEqualityRepresentation = 0;
			redYEqualityRepresentation = 0;
			blackXEqualityRepresentation = 0;
			blackYEqualityRepresentation = 0;
			for (int x = 0; x < 9; x++)
			{
				for (int y = 0; y < 10; y++)
				{
					if (board[y, x] != null)
					{
						if (board[y, x].Color == ChessColor.Red)
						{
							//11进制
							redXEqualityRepresentation += (ulong)((x + 1) * Math.Pow(11, board[y, x].ID));
							redYEqualityRepresentation += (ulong)((y + 1) * Math.Pow(11, board[y, x].ID));
						}
						else
						{
							blackXEqualityRepresentation += (ulong)((x + 1) * Math.Pow(11, board[y, x].ID));
							blackYEqualityRepresentation += (ulong)((y + 1) * Math.Pow(11, board[y, x].ID));
						}
					}
				}
			}
		}

		private static readonly char[] BaseChars =
			"0123456789A".ToCharArray();
		private static readonly Dictionary<char, int> CharValues = BaseChars
																   .Select((c, i) => new { Char = c, Index = i })
																   .ToDictionary(c => c.Char, c => c.Index);

		public static string LongToBase(ulong value)
		{
			ulong targetBase = (ulong)BaseChars.Length;
			// Determine exact number of characters to use.
			char[] buffer = new char[Math.Max(
											  (int)Math.Ceiling(Math.Log(value + 1, targetBase)), 1)];

			var i = buffer.Length;
			do
			{
				buffer[--i] = BaseChars[value % targetBase];
				value = value / targetBase;
			}
			while (value > 0);

			return new string(buffer, i, buffer.Length - i);
		}


		private Board(Piece[,] board)
		{
			this.board = board;

			//InitilizeEqualityRepresentation();
			InitializeJiangLocations();
		}

		/// <summary>
		/// 初始化棋盘，棋子以标准布局开始。
		/// </summary>
		public Board()
		{
			board = new Piece[10, 9];
			board[0, 0] = new Piece(0, "车") { Color = ChessColor.Black };
			board[0, 8] = new Piece(1, "车") { Color = ChessColor.Black };
			board[0, 1] = new Piece(2, "马") { Color = ChessColor.Black };
			board[0, 7] = new Piece(3, "马") { Color = ChessColor.Black };
			board[0, 2] = new Piece(4, "象") { Color = ChessColor.Black, CanCrossRiver = false };
			board[0, 6] = new Piece(5, "象") { Color = ChessColor.Black, CanCrossRiver = false };
			board[0, 3] = new Piece(6, "士") { Color = ChessColor.Black, CanCrossRiver = false };
			board[0, 5] = new Piece(7, "士") { Color = ChessColor.Black, CanCrossRiver = false };
			board[2, 1] = new Piece(8, "炮") { Color = ChessColor.Black };
			board[2, 7] = new Piece(9, "炮") { Color = ChessColor.Black };
			board[3, 0] = new Piece(10, "兵") { Color = ChessColor.Black };
			board[3, 2] = new Piece(11, "兵") { Color = ChessColor.Black };
			//board[3, 4] = new Piece(12,"兵", moves兵) { Color = ChessColor.Black };
			board[3, 6] = new Piece(13, "兵") { Color = ChessColor.Black };
			board[3, 8] = new Piece(14, "兵") { Color = ChessColor.Black };
			board[0, 4] = new Piece(15, "将") { Color = ChessColor.Black, CanCrossRiver = false };



			board[9, 0] = new Piece(0, "车") { Color = ChessColor.Red };
			board[9, 8] = new Piece(1, "车") { Color = ChessColor.Red };
			board[9, 1] = new Piece(2, "马") { Color = ChessColor.Red };
			board[9, 7] = new Piece(3, "马") { Color = ChessColor.Red };
			board[9, 2] = new Piece(4, "象") { Color = ChessColor.Red, CanCrossRiver = false };
			board[9, 6] = new Piece(5, "象") { Color = ChessColor.Red, CanCrossRiver = false };
			board[9, 3] = new Piece(6, "士") { Color = ChessColor.Red, CanCrossRiver = false };
			board[9, 5] = new Piece(7, "士") { Color = ChessColor.Red, CanCrossRiver = false };
			board[3, 4] = new Piece(8, "炮") { Color = ChessColor.Red };
			board[7, 7] = new Piece(9, "炮") { Color = ChessColor.Red };
			board[6, 0] = new Piece(10, "兵") { Color = ChessColor.Red };
			board[6, 2] = new Piece(11, "兵") { Color = ChessColor.Red };
			board[6, 4] = new Piece(12, "兵") { Color = ChessColor.Red };
			board[6, 6] = new Piece(13, "兵") { Color = ChessColor.Red };
			board[6, 8] = new Piece(14, "兵") { Color = ChessColor.Red };
			board[9, 4] = new Piece(15, "将") { Color = ChessColor.Red, CanCrossRiver = false };




			InitilizeEqualityRepresentation();
			InitializeJiangLocations();
		}

		/// <summary>
		/// 用<see cref="Print"/>的输出创建棋盘。
		/// 小括号包起来的表示红色，中括号包起来的表示黑色。
		/// </summary>
		/// <param name="str"></param>
		public Board(string str)
		{
			board = new Piece[Height, Width];

			var redIdMap = GetIdMap();
			var blackIdMap = GetIdMap();


			var lines = str.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			for (int y = 0; y < Height; y++)
			{
				var line = lines[y].Trim();
				for (int i = 0; i < line.Length;)
				{
					if (line[i] == '-')
						i += 3;
					else
					{
						switch (line[i + 1])
						{
							case '车':
								board[y, i / 3] = new Piece((line[i] == '(' ? redIdMap : blackIdMap)["车"]++, "车") { Color = line[i] == '(' ? ChessColor.Red : ChessColor.Black };
								break;
							case '马':
								board[y, i / 3] = new Piece((line[i] == '(' ? redIdMap : blackIdMap)["马"]++, "马") { Color = line[i] == '(' ? ChessColor.Red : ChessColor.Black };
								break;
							case '象':
								board[y, i / 3] = new Piece((line[i] == '(' ? redIdMap : blackIdMap)["象"]++, "象") { Color = line[i] == '(' ? ChessColor.Red : ChessColor.Black };
								break;
							case '士':
								board[y, i / 3] = new Piece((line[i] == '(' ? redIdMap : blackIdMap)["士"]++, "士") { Color = line[i] == '(' ? ChessColor.Red : ChessColor.Black };
								break;
							case '将':
								board[y, i / 3] = new Piece((line[i] == '(' ? redIdMap : blackIdMap)["将"]++, "将") { Color = line[i] == '(' ? ChessColor.Red : ChessColor.Black };
								break;
							case '炮':
								board[y, i / 3] = new Piece((line[i] == '(' ? redIdMap : blackIdMap)["炮"]++, "炮") { Color = line[i] == '(' ? ChessColor.Red : ChessColor.Black };
								break;
							case '兵':
								board[y, i / 3] = new Piece((line[i] == '(' ? redIdMap : blackIdMap)["兵"]++, "兵") { Color = line[i] == '(' ? ChessColor.Red : ChessColor.Black };
								break;
						}

						i += 3;
					}
				}
			}
			InitilizeEqualityRepresentation();
			InitializeJiangLocations();

		}


		private void InitializeJiangLocations()
		{
			RedJiangLocation = new IntPoint(-1, -1);
			BlackJiangLocation = new IntPoint(-1, -1);

			for (int x = 3; x <= 5; x++)
			{
				for (int y = 0; y <= 2; y++)
				{
					if (board[y, x]?.Name == "将")
					{
						if (board[y, x].Color == ChessColor.Red)
							RedJiangLocation = new IntPoint(x, y);
						else
							BlackJiangLocation = new IntPoint(x, y);

						break;
					}
				}
			}

			for (int x = 3; x <= 5; x++)
			{
				for (int y = 7; y <= 9; y++)
				{
					if (board[y, x]?.Name == "将")
					{
						if (board[y, x].Color == ChessColor.Red)
							RedJiangLocation = new IntPoint(x, y);
						else
							BlackJiangLocation = new IntPoint(x, y);
						break;
					}
				}
			}

			if (RedJiangLocation.X == -1)
				IsBlackWin = true;

			if (BlackJiangLocation.X == -1)
				IsRedWin = true;
		}


		public Board Move(IntPoint location, IntPoint targetLocation, out Piece eatenPiece)
		{
			Debug.Assert(board[location.Y, location.X] != null, $"想要从({location.X},{location.Y})走子，但该位置没有棋子。");


			var newBoard = (Piece[,])board.Clone();
			eatenPiece = newBoard[targetLocation.Y, targetLocation.X];

			newBoard[targetLocation.Y, targetLocation.X] = newBoard[location.Y, location.X];
			newBoard[location.Y, location.X] = null;

			var b = new Board(newBoard);
			b.redXEqualityRepresentation = redXEqualityRepresentation;
			b.redYEqualityRepresentation = redYEqualityRepresentation;
			b.blackXEqualityRepresentation = blackXEqualityRepresentation;
			b.blackYEqualityRepresentation = blackYEqualityRepresentation;

			var movingColor = board[location.Y, location.X].Color;
			var movingId = board[location.Y, location.X].ID;
			if (movingColor == ChessColor.Red)
			{
				b.redXEqualityRepresentation -= (ulong)((location.X + 1) * Math.Pow(11, movingId));
				b.redYEqualityRepresentation -= (ulong)((location.Y + 1) * Math.Pow(11, movingId));

				b.redXEqualityRepresentation += (ulong)((targetLocation.X + 1) * Math.Pow(11, movingId));
				b.redYEqualityRepresentation += (ulong)((targetLocation.Y + 1) * Math.Pow(11, movingId));

				if (eatenPiece != null)
				{
					b.blackXEqualityRepresentation -= (ulong)((targetLocation.X + 1) * Math.Pow(11, eatenPiece.ID));
					b.blackYEqualityRepresentation -= (ulong)((targetLocation.Y + 1) * Math.Pow(11, eatenPiece.ID));
				}
			}
			else
			{
				b.blackXEqualityRepresentation -= (ulong)((location.X + 1) * Math.Pow(11, movingId));
				b.blackYEqualityRepresentation -= (ulong)((location.Y + 1) * Math.Pow(11, movingId));

				b.blackXEqualityRepresentation += (ulong)((targetLocation.X + 1) * Math.Pow(11, movingId));
				b.blackYEqualityRepresentation += (ulong)((targetLocation.Y + 1) * Math.Pow(11, movingId));

				if (eatenPiece != null)
				{
					b.redXEqualityRepresentation -= (ulong)((targetLocation.X + 1) * Math.Pow(11, eatenPiece.ID));
					b.redYEqualityRepresentation -= (ulong)((targetLocation.Y + 1) * Math.Pow(11, eatenPiece.ID));
				}
			}

#if DEBUG
			ulong rx = b.redXEqualityRepresentation,
				  ry = b.redYEqualityRepresentation,
				  bx = b.blackXEqualityRepresentation,
				  by = b.blackYEqualityRepresentation;

			b.InitilizeEqualityRepresentation();

			Debug.Assert(b.redXEqualityRepresentation==rx);
			Debug.Assert(b.redYEqualityRepresentation==ry);
			Debug.Assert(b.blackXEqualityRepresentation==bx);
			Debug.Assert(b.blackYEqualityRepresentation==by);
#endif

			return b;
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

			if (other == null)
				return false;



			bool r = redXEqualityRepresentation == other.redXEqualityRepresentation &&
					redYEqualityRepresentation == other.redYEqualityRepresentation &&
					blackXEqualityRepresentation == other.blackXEqualityRepresentation &&
					blackYEqualityRepresentation == other.blackYEqualityRepresentation;


			//bool b = OldEquals(other);

			//Debug.Assert(r == b, "新旧方法结果不同！");

			return r;
		}

		//private bool OldEquals(Board other)
		//{
		//	for (int x = 0; x < 9; x++)
		//	{
		//		for (int y = 0; y < 10; y++)
		//		{
		//			if (board[y, x] != null && other.board[y, x] != null)
		//			{
		//				if (board[y, x].Name == other.board[y, x].Name &&
		//					board[y, x].Color == other.board[y, x].Color)
		//					continue;
		//				else
		//					return false;
		//			}
		//			else if (board[y, x] == null && other.board[y, x] == null)
		//				continue;
		//			else
		//				return false;
		//		}
		//	}

		//	return true;
		//}


		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 0;

				//uint.MaxValue 的二进制全是1。
				int low32 = (int)(redXEqualityRepresentation & uint.MaxValue);
				int high32 = (int)(redXEqualityRepresentation >> 32);

				hash = low32 ^ high32;

				low32 = (int)(redYEqualityRepresentation & uint.MaxValue);
				high32 = (int)(redYEqualityRepresentation >> 32);

				hash = hash ^ low32 ^ high32;

				low32 = (int)(blackXEqualityRepresentation & uint.MaxValue);
				high32 = (int)(blackXEqualityRepresentation >> 32);

				hash = hash ^ low32 ^ high32;

				low32 = (int)(blackYEqualityRepresentation & uint.MaxValue);
				high32 = (int)(blackYEqualityRepresentation >> 32);

				hash = hash ^ low32 ^ high32;
				return hash;
			}
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

		public string Print()
		{
			var sb = new StringBuilder();
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if (this[y, x] == null)
						sb.Append("-＋-");
					else
					{
						//var backup = Console.ForegroundColor;
						//Console.ForegroundColor = this[y, x].Color == ChessColor.Red ? ConsoleColor.Red : ConsoleColor.Black;
						sb.AppendFormat(this[y, x].Color == ChessColor.Red ? "({0})" : "[{0}]", this[y, x].Name);
						//Console.ForegroundColor = backup;
					}
				}

				sb.AppendLine();
			}

			return sb.ToString().TrimEnd();
		}

		private static Dictionary<string, int> GetIdMap()
		{
			return new Dictionary<string, int>()
			{
				["车"] = 0,
				["马"] = 2,
				["象"] = 4,
				["士"] = 6,
				["炮"] = 8,
				["兵"] = 10,
				["将"] = 15,
			};
		}

	}
}
