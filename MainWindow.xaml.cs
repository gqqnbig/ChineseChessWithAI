using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Func<Piece, Piece[][], IEnumerable<IntPoint>> moves车 = (p, pieces) =>
			 {
				 List<IntPoint> moves = new List<IntPoint>();
				 for (int x = p.Location.X - 1; x >= 0; x--)
				 {
					 if (pieces[p.Location.Y][x] == null)
						 moves.Add(new IntPoint(x, p.Location.Y));
					 else if (pieces[p.Location.Y][x].Color != p.Color) //吃子
					 {
						 moves.Add(new IntPoint(x, p.Location.Y));
						 break;
					 }
					 else
						 moves.Add(new IntPoint(x, p.Location.Y));
				 }

				 for (int x = p.Location.X + 1; x < 9; x++)
				 {
					 if (pieces[p.Location.Y][x] == null)
						 moves.Add(new IntPoint(x, p.Location.Y));
					 else if (pieces[p.Location.Y][x].Color != p.Color) //吃子
					 {
						 moves.Add(new IntPoint(x, p.Location.Y));
						 break;
					 }
					 else
						 moves.Add(new IntPoint(x, p.Location.Y));
				 }

				 for (int y = p.Location.Y - 1; y >= 0; y--)
				 {
					 if (pieces[y][p.Location.X] == null)
						 moves.Add(new IntPoint(p.Location.X, y));
					 else if (pieces[y][p.Location.X].Color != p.Color) //吃子
					 {
						 moves.Add(new IntPoint(p.Location.X, y));
						 break;
					 }
					 else
						 moves.Add(new IntPoint(p.Location.X, y));
				 }

				 for (int y = p.Location.Y + 1; y < 10; y++)
				 {
					 if (pieces[y][p.Location.X] == null)
						 moves.Add(new IntPoint(p.Location.X, y));
					 else if (pieces[y][p.Location.X].Color != p.Color) //吃子
					 {
						 moves.Add(new IntPoint(p.Location.X, y));
						 break;
					 }
					 else
						 moves.Add(new IntPoint(p.Location.X, y));
				 }
				 return moves;
			 };

			Func<Piece, Piece[][], IEnumerable<IntPoint>> moves马 = (p, pieces) =>
			{
				List<IntPoint> moves = new List<IntPoint>();
				if (p.Location.X - 2 >= 0)
				{
					if (pieces[p.Location.Y][p.Location.X - 1] == null)
					{
						//如果为null，则?.Color返回null，null!=p.Color。
						if (pieces[p.Location.Y - 1][p.Location.X - 2]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X - 2, p.Location.Y - 1));
						if (pieces[p.Location.Y + 1][p.Location.X - 2]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X - 2, p.Location.Y - 1));
					}
				}
				if (p.Location.X + 2 < 9)
				{
					if (pieces[p.Location.Y][p.Location.X + 1] == null)
					{
						if (pieces[p.Location.Y - 1][p.Location.X + 2]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X + 2, p.Location.Y - 1));
						if (pieces[p.Location.Y + 1][p.Location.X + 2]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X + 2, p.Location.Y - 1));
					}
				}

				if (p.Location.Y - 2 >= 0)
				{
					if (pieces[p.Location.Y - 1][p.Location.X] == null)
					{
						if (pieces[p.Location.Y - 2][p.Location.X - 1]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X - 1, p.Location.Y - 2));
						if (pieces[p.Location.Y - 2][p.Location.X + 1]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X + 1, p.Location.Y - 2));
					}
				}

				if (p.Location.Y + 2 < 10)
				{
					if (pieces[p.Location.Y + 1][p.Location.X] == null)
					{
						if (pieces[p.Location.Y + 2][p.Location.X - 1]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X - 1, p.Location.Y - 2));
						if (pieces[p.Location.Y + 2][p.Location.X + 1]?.Color != p.Color)
							moves.Add(new IntPoint(p.Location.X + 1, p.Location.Y - 2));
					}
				}

				return moves;
			};

			Func<Piece, Piece[][], IEnumerable<IntPoint>> moves象 = (p, pieces) =>
			{

			};

		var blackPieces = new[]{
				new Piece(moves车) { Name = "车", Color = ChessColor.Black, Location = new IntPoint(0, 0) },
				new Piece(moves马) { Name = "马", Color = ChessColor.Black, Location = new IntPoint(1, 0) },
				new Piece { Name = "象", Color = ChessColor.Black, Location = new IntPoint(2, 0) },
				new Piece { Name = "士", Color = ChessColor.Black, Location = new IntPoint(3, 0) },
				new Piece { Name = "将", Color = ChessColor.Black, Location = new IntPoint(4, 0) },
				new Piece { Name = "士", Color = ChessColor.Black, Location = new IntPoint(5, 0) },
				new Piece { Name = "象", Color = ChessColor.Black, Location = new IntPoint(6, 0) },
				new Piece(moves马) { Name = "马", Color = ChessColor.Black, Location = new IntPoint(7, 0) },
				new Piece(moves车) { Name = "车", Color = ChessColor.Black, Location = new IntPoint(8, 0) },

				new Piece { Name = "炮", Color = ChessColor.Black, Location = new IntPoint(1, 2) },
				new Piece { Name = "炮", Color = ChessColor.Black, Location = new IntPoint(7, 2) },

				new Piece { Name = "兵", Color = ChessColor.Black, Location = new IntPoint(0, 3) },
				new Piece { Name = "兵", Color = ChessColor.Black, Location = new IntPoint(2, 3) },
				new Piece { Name = "兵", Color = ChessColor.Black, Location = new IntPoint(4, 3) },
				new Piece { Name = "兵", Color = ChessColor.Black, Location = new IntPoint(6, 3) },
				new Piece { Name = "兵", Color = ChessColor.Black, Location = new IntPoint(8, 3) },
			};

		var redPieces = new[]{
				new Piece(moves车) { Name = "车", Color = ChessColor.Red, Location = new IntPoint(0, 9) },
				new Piece(moves马) { Name = "马", Color = ChessColor.Red, Location = new IntPoint(1, 9) },
				new Piece { Name = "象", Color = ChessColor.Red, Location = new IntPoint(2, 9) },
				new Piece { Name = "士", Color = ChessColor.Red, Location = new IntPoint(3, 9) },
				new Piece { Name = "将", Color = ChessColor.Red, Location = new IntPoint(4, 9) },
				new Piece { Name = "士", Color = ChessColor.Red, Location = new IntPoint(5, 9) },
				new Piece { Name = "象", Color = ChessColor.Red, Location = new IntPoint(6, 9) },
				new Piece(moves马) { Name = "马", Color = ChessColor.Red, Location = new IntPoint(7, 9) },
				new Piece(moves车) { Name = "车", Color = ChessColor.Red, Location = new IntPoint(8, 9) },

				new Piece { Name = "炮", Color = ChessColor.Red, Location = new IntPoint(1, 7) },
				new Piece { Name = "炮", Color = ChessColor.Red, Location = new IntPoint(7, 7) },

				new Piece { Name = "兵", Color = ChessColor.Red, Location = new IntPoint(0, 6) },
				new Piece { Name = "兵", Color = ChessColor.Red, Location = new IntPoint(2, 6) },
				new Piece { Name = "兵", Color = ChessColor.Red, Location = new IntPoint(4, 6) },
				new Piece { Name = "兵", Color = ChessColor.Red, Location = new IntPoint(6, 6) },
				new Piece { Name = "兵", Color = ChessColor.Red, Location = new IntPoint(8, 6) },
			};

		UpdateBoard(redPieces.Union(blackPieces));
	}

	void UpdateBoard(IEnumerable<Piece> pieces)
	{
		board.Children.Clear();
		foreach (var piece in pieces)
		{
			var button = new Button();
			button.Content = piece.Name;
			button.Foreground = piece.Color == ChessColor.Red ? Brushes.Red : Brushes.Black;
			Grid.SetColumn(button, piece.Location.X);
			Grid.SetRow(button, piece.Location.Y);
			board.Children.Add(button);
		}
	}
}
}
