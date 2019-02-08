using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IntPoint = System.Drawing.Point;

namespace 中国象棋
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		Piece[,] pieces;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Func<Piece, Piece[,], IEnumerable<IntPoint>> moves车 = (p, pieces) =>
			{
				List<IntPoint> moves = new List<IntPoint>();
				for (int x = p.Location.X - 1; x >= 0; x--)
				{
					if (pieces[p.Location.Y, x] == null)
						moves.Add(new IntPoint(x, p.Location.Y));
					else if (pieces[p.Location.Y, x].Color != p.Color) //吃子
					{
						moves.Add(new IntPoint(x, p.Location.Y));
						break;
					}
					else
						moves.Add(new IntPoint(x, p.Location.Y));
				}

				for (int x = p.Location.X + 1; x < 9; x++)
				{
					if (pieces[p.Location.Y, x] == null)
						moves.Add(new IntPoint(x, p.Location.Y));
					else if (pieces[p.Location.Y, x].Color != p.Color) //吃子
					{
						moves.Add(new IntPoint(x, p.Location.Y));
						break;
					}
					else
						moves.Add(new IntPoint(x, p.Location.Y));
				}

				for (int y = p.Location.Y - 1; y >= 0; y--)
				{
					if (pieces[y, p.Location.X] == null)
						moves.Add(new IntPoint(p.Location.X, y));
					else if (pieces[y, p.Location.X].Color != p.Color) //吃子
					{
						moves.Add(new IntPoint(p.Location.X, y));
						break;
					}
					else
						moves.Add(new IntPoint(p.Location.X, y));
				}

				for (int y = p.Location.Y + 1; y < 10; y++)
				{
					if (pieces[y, p.Location.X] == null)
						moves.Add(new IntPoint(p.Location.X, y));
					else if (pieces[y, p.Location.X].Color != p.Color) //吃子
					{
						moves.Add(new IntPoint(p.Location.X, y));
						break;
					}
					else
						moves.Add(new IntPoint(p.Location.X, y));
				}
				return moves;
			};

			Func<Piece, Piece[,], IEnumerable<IntPoint>> moves马 = (p, pieces) =>
			{
				List<IntPoint> moves = new List<IntPoint>();
				if (p.Location.X - 2 >= 0)
				{
					if (pieces[p.Location.Y, p.Location.X - 1] == null)
					{
						//如果为null，则?.Color返回null，null!=p.Color。
						if (pieces[p.Location.Y - 1, p.Location.X - 2]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X - 2, p.Location.Y - 1));
						if (pieces[p.Location.Y + 1, p.Location.X - 2]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X - 2, p.Location.Y - 1));
					}
				}
				if (p.Location.X + 2 < 9)
				{
					if (pieces[p.Location.Y, p.Location.X + 1] == null)
					{
						if (pieces[p.Location.Y - 1, p.Location.X + 2]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X + 2, p.Location.Y - 1));
						if (pieces[p.Location.Y + 1, p.Location.X + 2]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X + 2, p.Location.Y - 1));
					}
				}

				if (p.Location.Y - 2 >= 0)
				{
					if (pieces[p.Location.Y - 1, p.Location.X] == null)
					{
						if (pieces[p.Location.Y - 2, p.Location.X - 1]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X - 1, p.Location.Y - 2));
						if (pieces[p.Location.Y - 2, p.Location.X + 1]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X + 1, p.Location.Y - 2));
					}
				}

				if (p.Location.Y + 2 < 10)
				{
					if (pieces[p.Location.Y + 1, p.Location.X] == null)
					{
						if (pieces[p.Location.Y + 2, p.Location.X - 1]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X - 1, p.Location.Y - 2));
						if (pieces[p.Location.Y + 2, p.Location.X + 1]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X + 1, p.Location.Y - 2));
					}
				}

				return moves;
			};

			Func<Piece, Piece[,], IEnumerable<IntPoint>> moves象 = (p, pieces) =>
			{
				List<IntPoint> moves = new List<IntPoint>();
				if (p.Location.Y - 2 >= 0 && p.Location.X - 2 >= 0 && p.Location.Y <= 4 && pieces[p.Location.Y - 2, p.Location.X - 2]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X - 2, p.Location.Y - 2));

				if (p.Location.Y - 2 >= 0 && p.Location.X + 2 < 9 && p.Location.Y <= 4 && pieces[p.Location.Y - 2, p.Location.X + 2]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X + 2, p.Location.Y - 2));

				if (p.Location.X - 2 >= 0 && p.Location.Y <= 2 && pieces[p.Location.Y + 2, p.Location.X - 2]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X - 2, p.Location.Y + 2));

				if (p.Location.X + 2 < 9 && p.Location.Y <= 2 && pieces[p.Location.Y + 2, p.Location.X + 2]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X + 2, p.Location.Y + 2));


				if (p.Location.X - 2 >= 0 && p.Location.Y >= 7 && pieces[p.Location.Y - 2, p.Location.X - 2]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X - 2, p.Location.Y - 2));

				if (p.Location.X + 2 < 9 && p.Location.Y >= 7 && pieces[p.Location.Y - 2, p.Location.X + 2]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X + 2, p.Location.Y - 2));

				if (p.Location.Y >= 5 && p.Location.X - 2 >= 0 && p.Location.Y + 2 <= 9 && pieces[p.Location.Y + 2, p.Location.X - 2]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X - 2, p.Location.Y + 2));

				if (p.Location.Y >= 5 && p.Location.X + 2 < 9 && p.Location.Y + 2 <= 9 && pieces[p.Location.Y + 2, p.Location.X + 2]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X + 2, p.Location.Y + 2));

				return moves;
			};

			Func<Piece, Piece[,], IEnumerable<IntPoint>> moves士 = (p, pieces) =>
			{
				List<IntPoint> moves = new List<IntPoint>();
				if (p.Location.X - 1 >= 0 && p.Location.Y - 1 >= 0 && p.Location.Y <= 2 && p.Location.X >= 4 && pieces[p.Location.Y - 1, p.Location.X - 1]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X - 1, p.Location.Y - 1));

				if (p.Location.X + 1 >= 0 && p.Location.Y - 1 >= 0 && p.Location.Y <= 2 && p.Location.X <= 4 && pieces[p.Location.Y - 1, p.Location.X + 1]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X + 1, p.Location.Y - 1));

				if (p.Location.X - 1 >= 0 && p.Location.Y <= 1 && p.Location.X >= 4 && pieces[p.Location.Y + 1, p.Location.X - 1]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X - 1, p.Location.Y + 1));

				if (p.Location.X + 1 >= 0 && p.Location.Y <= 1 && p.Location.X <= 4 && pieces[p.Location.Y + 1, p.Location.X + 1]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X + 1, p.Location.Y + 1));


				if (p.Location.X - 1 >= 0 && p.Location.Y >= 7 && p.Location.X >= 4 && pieces[p.Location.Y - 1, p.Location.X - 1]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X - 1, p.Location.Y - 1));

				if (p.Location.X + 1 >= 0 && p.Location.Y >= 7 && p.Location.X <= 4 && pieces[p.Location.Y - 1, p.Location.X + 1]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X + 1, p.Location.Y - 1));

				if (p.Location.X - 1 >= 0 && p.Location.Y + 1 <= 9 && p.Location.Y >= 7 && p.Location.X >= 4 && pieces[p.Location.Y + 1, p.Location.X - 1]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X - 1, p.Location.Y + 1));

				if (p.Location.X + 1 >= 0 && p.Location.Y + 1 <= 9 && p.Location.Y >= 7 && p.Location.X <= 4 && pieces[p.Location.Y + 1, p.Location.X + 1]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X + 1, p.Location.Y + 1));

				return moves;
			};

			Func<Piece, Piece[,], IEnumerable<IntPoint>> moves将 = (p, pieces) =>
			{
				List<IntPoint> moves = new List<IntPoint>();
				if (p.Location.X >= 4 && pieces[p.Location.Y, p.Location.X - 1]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X - 1, p.Location.Y));

				if (p.Location.X <= 4 && pieces[p.Location.Y, p.Location.X + 1]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X + 1, p.Location.Y));

				if (1 <= p.Location.Y && p.Location.Y <= 2 && pieces[p.Location.Y - 1, p.Location.X]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X, p.Location.Y - 1));

				if (p.Location.Y <= 1 && pieces[p.Location.Y + 1, p.Location.X]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X, p.Location.Y + 1));



				if (8 <= p.Location.Y && pieces[p.Location.Y - 1, p.Location.X]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X, p.Location.Y - 1));

				if (7 <= p.Location.Y && p.Location.Y <= 8 && pieces[p.Location.Y + 1, p.Location.X]?.Color != p.Color)
					moves.Add(new IntPoint(p.Location.X, p.Location.Y + 1));

				return moves;
			};

			Func<Piece, Piece[,], IEnumerable<IntPoint>> moves炮 = (p, pieces) =>
			{
				List<IntPoint> moves = new List<IntPoint>();
				for (int x = p.Location.X - 1; x >= 0; x--)
				{
					if (pieces[p.Location.Y, x] == null)
						moves.Add(new IntPoint(x, p.Location.Y));
					else
					{   //吃子
						for (int xx = x - 1; xx >= 0; xx--)
						{
							if (pieces[p.Location.Y, xx] != null && pieces[p.Location.Y, xx].Color != p.Color)
							{
								moves.Add(new IntPoint(xx, p.Location.Y));
								break;
							}
						}
						break;
					}
				}

				for (int x = p.Location.X + 1; x < 9; x++)
				{
					if (pieces[p.Location.Y, x] == null)
						moves.Add(new IntPoint(x, p.Location.Y));
					else
					{   //吃子
						for (int xx = x + 1; xx < 9; xx++)
						{
							if (pieces[p.Location.Y, xx] != null && pieces[p.Location.Y, xx].Color != p.Color)
							{
								moves.Add(new IntPoint(xx, p.Location.Y));
								break;
							}
						}
						break;
					}
				}

				for (int y = p.Location.Y - 1; y >= 0; y--)
				{
					if (pieces[y, p.Location.X] == null)
						moves.Add(new IntPoint(p.Location.X, y));
					else
					{
						for (int yy = y - 1; yy >= 0; yy--)
						{
							if (pieces[yy, p.Location.X] != null && pieces[yy, p.Location.X].Color != p.Color)
							{
								moves.Add(new IntPoint(p.Location.X, yy));
								break;
							}
						}
						break;
					}
				}

				for (int y = p.Location.Y + 1; y < 10; y++)
				{
					if (pieces[y, p.Location.X] == null)
						moves.Add(new IntPoint(p.Location.X, y));
					else
					{
						for (int yy = y + 1; yy < 10; yy--)
						{
							if (pieces[yy, p.Location.X] != null && pieces[yy, p.Location.X].Color != p.Color)
							{
								moves.Add(new IntPoint(p.Location.X, yy));
								break;
							}
						}
						break;
					}
				}
				return moves;
			};

			Func<Piece, Piece[,], IEnumerable<IntPoint>> moves兵 = (p, pieces) =>
			{
				List<IntPoint> moves = new List<IntPoint>();
				var jiangLocation = CoordinatesOf(pieces, jiang => jiang.Name == "将" && jiang.Color == p.Color);
				if (jiangLocation.Y <= 4)
				{
					if (pieces[p.Location.Y + 1, p.Location.X]?.Color != p.Color)
						moves.Add(new IntPoint(p.Location.Y + 1, p.Location.X));
					if (p.Location.Y > 4) //如果过河了，就可以左右移动
					{
						if (p.Location.X >= 1 && pieces[p.Location.Y, p.Location.X - 1]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X - 1, p.Location.Y));

						if (p.Location.X <= 7 && pieces[p.Location.Y, p.Location.X + 1]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X + 1, p.Location.Y));
					}
				}
				else
				{
					if (pieces[p.Location.Y - 1, p.Location.X]?.Color != p.Color)
						moves.Add(new IntPoint(p.Location.Y - 1, p.Location.X));
					if (p.Location.Y <= 4) //如果过河了，就可以左右移动
					{
						if (p.Location.X >= 1 && pieces[p.Location.Y, p.Location.X - 1]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X - 1, p.Location.Y));

						if (p.Location.X <= 7 && pieces[p.Location.Y, p.Location.X + 1]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X + 1, p.Location.Y));
					}
				}
				return moves;

			};

			pieces[0, 0] = new Piece(moves车) { Name = "车", Color = ChessColor.Black };
			pieces[0, 1] = new Piece(moves马) { Name = "马", Color = ChessColor.Black, Location = new IntPoint(1, 0) };
			pieces[0, 2] = new Piece(moves象) { Name = "象", Color = ChessColor.Black, Location = new IntPoint(2, 0) };
			pieces[0, 3] = new Piece(moves士) { Name = "士", Color = ChessColor.Black, Location = new IntPoint(3, 0) };
			pieces[0, 4] = new Piece(moves将) { Name = "将", Color = ChessColor.Black, Location = new IntPoint(4, 0) };
			pieces[0, 5] = new Piece(moves士) { Name = "士", Color = ChessColor.Black, Location = new IntPoint(5, 0) };
			pieces[0, 6] = new Piece(moves象) { Name = "象", Color = ChessColor.Black, Location = new IntPoint(6, 0) };
			pieces[0, 7] = new Piece(moves马) { Name = "马", Color = ChessColor.Black, Location = new IntPoint(7, 0) };
			pieces[0, 8] = new Piece(moves车) { Name = "车", Color = ChessColor.Black, Location = new IntPoint(8, 0) };

			pieces[2, 1] = new Piece(moves炮) { Name = "炮", Color = ChessColor.Black, Location = new IntPoint(1, 2) };
			pieces[2, 7] = new Piece(moves炮) { Name = "炮", Color = ChessColor.Black, Location = new IntPoint(7, 2) };

			pieces[3, 0] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black, Location = new IntPoint(0, 3) };
			pieces[3, 2] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black, Location = new IntPoint(2, 3) };
			pieces[3, 4] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black, Location = new IntPoint(4, 3) };
			pieces[3, 6] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black, Location = new IntPoint(6, 3) };
			pieces[3, 8] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black, Location = new IntPoint(8, 3) };

			var redPieces = new[]{
				new Piece(moves车) { Name = "车", Color = ChessColor.Red, Location = new IntPoint(0, 9) },
				new Piece(moves马) { Name = "马", Color = ChessColor.Red, Location = new IntPoint(1, 9) },
				new Piece(moves象) { Name = "象", Color = ChessColor.Red, Location = new IntPoint(2, 9) },
				new Piece(moves士) { Name = "士", Color = ChessColor.Red, Location = new IntPoint(3, 9) },
				new Piece(moves将) { Name = "将", Color = ChessColor.Red, Location = new IntPoint(4, 9) },
				new Piece(moves士) { Name = "士", Color = ChessColor.Red, Location = new IntPoint(5, 9) },
				new Piece(moves象) { Name = "象", Color = ChessColor.Red, Location = new IntPoint(6, 9) },
				new Piece(moves马) { Name = "马", Color = ChessColor.Red, Location = new IntPoint(7, 9) },
				new Piece(moves车) { Name = "车", Color = ChessColor.Red, Location = new IntPoint(8, 9) },

				new Piece(moves炮) { Name = "炮", Color = ChessColor.Red, Location = new IntPoint(1, 7) },
				new Piece(moves炮) { Name = "炮", Color = ChessColor.Red, Location = new IntPoint(7, 7) },

				new Piece(moves兵) { Name = "兵", Color = ChessColor.Red, Location = new IntPoint(0, 6) },
				new Piece(moves兵) { Name = "兵", Color = ChessColor.Red, Location = new IntPoint(2, 6) },
				new Piece(moves兵) { Name = "兵", Color = ChessColor.Red, Location = new IntPoint(4, 6) },
				new Piece(moves兵) { Name = "兵", Color = ChessColor.Red, Location = new IntPoint(6, 6) },
				new Piece(moves兵) { Name = "兵", Color = ChessColor.Red, Location = new IntPoint(8, 6) },
			};

			pieces = redPieces.Union(blackPieces);
			UpdateBoard();
		}

		void UpdateBoard()
		{
			board.Children.Clear();
			foreach (var piece in pieces)
			{
				var button = new ToggleButton();
				button.Content = piece.Name;
				button.Foreground = piece.Color == ChessColor.Red ? Brushes.Red : Brushes.Black;


				Grid.SetColumn(button, piece.Location.X);
				Grid.SetRow(button, piece.Location.Y);
				board.Children.Add(button);
			}
		}

		public static IntPoint CoordinatesOf<T>(T[,] matrix, Predicate<T> p)
		{
			int w = matrix.GetLength(0); // width
			int h = matrix.GetLength(1); // height

			for (int x = 0; x < w; ++x)
			{
				for (int y = 0; y < h; ++y)
				{
					if (p(matrix[x, y]))
						return new IntPoint(x, y);
				}
			}

			return new IntPoint(-1, -1);
		}
	}
}
