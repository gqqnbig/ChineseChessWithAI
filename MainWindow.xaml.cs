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
		Board pieces;
		bool isRedTurn = true;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			pieces = new Board();
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
					button.IsEnabled = (piece.Color == ChessColor.Red && isRedTurn) || (piece.Color == ChessColor.Black && isRedTurn == false);
					button.Checked += Button_Checked;
					button.Unchecked += Button_Unchecked;

					Grid.SetColumn(button, x);
					Grid.SetRow(button, y);
					Panel.SetZIndex(button, 1);
					board.Children.Add(button);
				}
			}

			var termination = IsWin(pieces, isRedTurn ? ChessColor.Red : ChessColor.Black);
			if (termination.HasValue)
			{
				MessageBox.Show((isRedTurn ? "红" : "黑") + (termination.Value ? "胜" : "负") + "了");

				board.IsEnabled = false;
				return;
			}
		}

		private void Button_Unchecked(object sender, RoutedEventArgs e)
		{
			for (int i = board.Children.Count - 1; i >= 0; i--)
			{
				if (board.Children[i] is Border)
					board.Children.RemoveAt(i);
			}
		}

		private void Button_Checked(object sender, RoutedEventArgs e)
		{
			ToggleButton button = (ToggleButton)sender;

			for (int i = board.Children.Count - 1; i >= 0; i--)
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
				Border border = new Border();
				border.BorderThickness = new Thickness(2);
				Rectangle rect = new Rectangle() { Stroke = Brushes.OrangeRed, StrokeThickness = 2, StrokeDashArray = new DoubleCollection() { 2, 4 } };
				var relativeSource = new RelativeSource(RelativeSourceMode.FindAncestor);
				relativeSource.AncestorType = typeof(Border);

				rect.SetBinding(Rectangle.RadiusXProperty, new Binding("CornerRadius.TopRight") { RelativeSource = relativeSource });
				rect.SetBinding(Rectangle.RadiusYProperty, new Binding("CornerRadius.BottomLeft") { RelativeSource = relativeSource });
				rect.SetBinding(Rectangle.WidthProperty, new Binding("ActualWidth") { RelativeSource = relativeSource });
				rect.SetBinding(Rectangle.HeightProperty, new Binding("ActualHeight") { RelativeSource = relativeSource });

				border.BorderBrush = new VisualBrush() { Visual = rect };

				Button b = new Button() { Opacity = 0 };
				b.Click += MoveButton_Click;
				b.Tag = new IntPoint(x, y);
				border.Child = b;


				Panel.SetZIndex(border, 2);

				Grid.SetColumn(border, move.X);
				Grid.SetRow(border, move.Y);
				board.Children.Add(border);
			}

		}

		private void MoveButton_Click(object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;
			IntPoint pieceLocation = (IntPoint)button.Tag;
			int x = Grid.GetColumn((UIElement)button.Parent);
			int y = Grid.GetRow((UIElement)button.Parent);

			pieces = pieces.Move(pieceLocation, new IntPoint(x, y), out _);

			isRedTurn = !isRedTurn;
			UpdateBoard();
		}


		public static IntPoint CoordinatesOf<T>(T[,] matrix, Predicate<T> p)
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



		/// <summary>
		/// 如果指定的颜色胜，返回true；如果指定的颜色负，返回false；如果没有胜负，返回null。
		/// </summary>
		/// <param name="pieces"></param>
		/// <returns></returns>
		public static bool? IsWin(Board pieces, ChessColor color)
		{
			switch (color)
			{
				case ChessColor.Black:
					if (pieces.RedJiangLocation.X == -1)
						return true;
					else if (pieces.BlackJiangLocation.X == -1)
						return false;
					break;
				case ChessColor.Red:
					if (pieces.BlackJiangLocation.X == -1)
						return true;
					else if (pieces.RedJiangLocation.X == -1)
						return false;
					break;
				default:
					break;
			}
			return null;
		}
	}
}
