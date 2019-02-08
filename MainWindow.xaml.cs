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
		Piece[,] pieces = new Piece[10, 9];

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Func<Piece, IntPoint, Piece[,], IEnumerable<IntPoint>> moves车 = (p, location, pieces) =>
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

			Func<Piece, IntPoint, Piece[,], IEnumerable<IntPoint>> moves马 = (p, location, pieces) =>
			 {
				 List<IntPoint> moves = new List<IntPoint>();
				 if (location.X - 2 >= 0)
				 {
					 if (pieces[location.Y, location.X - 1] == null)
					 {
						 //如果为null，则?.Color返回null，null!=p.Color。
						 if (pieces[location.Y - 1, location.X - 2]?.Color != p.Color)
							 moves.Add(new IntPoint(location.X - 2, location.Y - 1));
						 if (pieces[location.Y + 1, location.X - 2]?.Color != p.Color)
							 moves.Add(new IntPoint(location.X - 2, location.Y - 1));
					 }
				 }
				 if (location.X + 2 < 9)
				 {
					 if (pieces[location.Y, location.X + 1] == null)
					 {
						 if (pieces[location.Y - 1, location.X + 2]?.Color != p.Color)
							 moves.Add(new IntPoint(location.X + 2, location.Y - 1));
						 if (pieces[location.Y + 1, location.X + 2]?.Color != p.Color)
							 moves.Add(new IntPoint(location.X + 2, location.Y - 1));
					 }
				 }

				 if (location.Y - 2 >= 0)
				 {
					 if (pieces[location.Y - 1, location.X] == null)
					 {
						 if (pieces[location.Y - 2, location.X - 1]?.Color != p.Color)
							 moves.Add(new IntPoint(location.X - 1, location.Y - 2));
						 if (pieces[location.Y - 2, location.X + 1]?.Color != p.Color)
							 moves.Add(new IntPoint(location.X + 1, location.Y - 2));
					 }
				 }

				 if (location.Y + 2 < 10)
				 {
					 if (pieces[location.Y + 1, location.X] == null)
					 {
						 if (pieces[location.Y + 2, location.X - 1]?.Color != p.Color)
							 moves.Add(new IntPoint(location.X - 1, location.Y + 2));
						 if (pieces[location.Y + 2, location.X + 1]?.Color != p.Color)
							 moves.Add(new IntPoint(location.X + 1, location.Y + 2));
					 }
				 }

				 return moves;
			 };

			Func<Piece, IntPoint, Piece[,], IEnumerable<IntPoint>> moves象 = (p, location, pieces) =>
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

			Func<Piece, IntPoint, Piece[,], IEnumerable<IntPoint>> moves士 = (p, location, pieces) =>
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

			Func<Piece, IntPoint, Piece[,], IEnumerable<IntPoint>> moves将 = (p, location, pieces) =>
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



				 if (8 <= location.Y && pieces[location.Y - 1, location.X]?.Color != p.Color)
					 moves.Add(new IntPoint(location.X, location.Y - 1));

				 if (7 <= location.Y && location.Y <= 8 && pieces[location.Y + 1, location.X]?.Color != p.Color)
					 moves.Add(new IntPoint(location.X, location.Y + 1));

				 return moves;
			 };

			Func<Piece, IntPoint, Piece[,], IEnumerable<IntPoint>> moves炮 = (p, location, pieces) =>
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
						 for (int yy = y + 1; yy < 10; yy--)
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

			Func<Piece, IntPoint, Piece[,], IEnumerable<IntPoint>> moves兵 = (p, location, pieces) =>
			 {
				 List<IntPoint> moves = new List<IntPoint>();
				 var jiangLocation = CoordinatesOf(pieces, jiang => jiang != null && jiang.Name == "将" && jiang.Color == p.Color);
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
					 if (pieces[location.Y - 1, location.X]?.Color != p.Color)
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

			pieces[0, 0] = new Piece(moves车) { Name = "车", Color = ChessColor.Black };
			pieces[0, 1] = new Piece(moves马) { Name = "马", Color = ChessColor.Black };
			pieces[0, 2] = new Piece(moves象) { Name = "象", Color = ChessColor.Black };
			pieces[0, 3] = new Piece(moves士) { Name = "士", Color = ChessColor.Black };
			pieces[0, 4] = new Piece(moves将) { Name = "将", Color = ChessColor.Black };
			pieces[0, 5] = new Piece(moves士) { Name = "士", Color = ChessColor.Black };
			pieces[0, 6] = new Piece(moves象) { Name = "象", Color = ChessColor.Black };
			pieces[0, 7] = new Piece(moves马) { Name = "马", Color = ChessColor.Black };
			pieces[0, 8] = new Piece(moves车) { Name = "车", Color = ChessColor.Black };

			pieces[2, 1] = new Piece(moves炮) { Name = "炮", Color = ChessColor.Black };
			pieces[2, 7] = new Piece(moves炮) { Name = "炮", Color = ChessColor.Black };

			pieces[3, 0] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black };
			pieces[3, 2] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black };
			pieces[3, 4] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black };
			pieces[3, 6] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black };
			pieces[3, 8] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Black };

			pieces[9, 0] = new Piece(moves车) { Name = "车", Color = ChessColor.Red };
			pieces[9, 1] = new Piece(moves马) { Name = "马", Color = ChessColor.Red };
			pieces[9, 2] = new Piece(moves象) { Name = "象", Color = ChessColor.Red };
			pieces[9, 3] = new Piece(moves士) { Name = "士", Color = ChessColor.Red };
			pieces[9, 4] = new Piece(moves将) { Name = "将", Color = ChessColor.Red };
			pieces[9, 5] = new Piece(moves士) { Name = "士", Color = ChessColor.Red };
			pieces[9, 6] = new Piece(moves象) { Name = "象", Color = ChessColor.Red };
			pieces[9, 7] = new Piece(moves马) { Name = "马", Color = ChessColor.Red };
			pieces[9, 8] = new Piece(moves车) { Name = "车", Color = ChessColor.Red };

			pieces[7, 1] = new Piece(moves炮) { Name = "炮", Color = ChessColor.Red };
			pieces[7, 7] = new Piece(moves炮) { Name = "炮", Color = ChessColor.Red };

			pieces[6, 0] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Red };
			pieces[6, 2] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Red };
			pieces[6, 4] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Red };
			pieces[6, 6] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Red };
			pieces[6, 8] = new Piece(moves兵) { Name = "兵", Color = ChessColor.Red };

			UpdateBoard();
		}

		void UpdateBoard()
		{
			board.Children.Clear();
			for (int x = 0; x < 9; x++)
			{
				for (int y = 0; y < 10; y++)
				{
					var piece = pieces[y, x];
					if (piece == null)
						continue;

					var button = new ToggleButton();
					button.Content = piece.Name;
					button.Foreground = piece.Color == ChessColor.Red ? Brushes.Red : Brushes.Black;
					button.Checked += Button_Checked;
					button.Unchecked += Button_Unchecked;

					Grid.SetColumn(button, x);
					Grid.SetRow(button, y);
					board.Children.Add(button);
				}
			}
		}

		private void Button_Unchecked(object sender, RoutedEventArgs e)
		{
			for (int i = board.Children.Count - 1; i >= 0; i--)
			{
				if (board.Children[i] is Rectangle)
					board.Children.RemoveAt(i);
			}
		}

		private void Button_Checked(object sender, RoutedEventArgs e)
		{
			ToggleButton button = (ToggleButton)sender;

			for (int i = board.Children.Count-1; i >=0 ; i--)
			{
				ToggleButton otherButton = board.Children[i] as ToggleButton;
				if (otherButton != null && otherButton != button)
				{
					otherButton.IsChecked = false;
				}
			}

			int x = Grid.GetColumn(button);
			int y = Grid.GetRow(button);

			var moves = pieces[y, x].GetPossibleMovements(new IntPoint(x, y), pieces);

			foreach (var move in moves)
			{
				Rectangle rect = new Rectangle();
				rect.StrokeDashArray = new DoubleCollection() { 2, 4 };
				rect.StrokeThickness = 2;
				rect.Stroke = Brushes.OrangeRed;
				Panel.SetZIndex(rect, 1);

				Grid.SetColumn(rect, move.X);
				Grid.SetRow(rect, move.Y);
				board.Children.Add(rect);
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
						return new IntPoint(y, x);
				}
			}

			return new IntPoint(-1, -1);
		}
	}
}
