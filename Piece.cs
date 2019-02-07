using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntPoint = System.Drawing.Point;

namespace 中国象棋
{
	class Piece
	{
		public string Name { get; set; }

		public ChessColor Color { get; set; }

		public IntPoint Location { get; set; }

		private Func<Piece, Piece[][], IEnumerable<IntPoint>> getPossibleMovements;

		public Piece(Func<Piece, Piece[][], IEnumerable<IntPoint>> getPossibleMovements)
		{
			this.getPossibleMovements = getPossibleMovements;
		}

		public IEnumerable<IntPoint> GetPossibleMovements(Piece[][] piecesOnBoard)
		{
			return getPossibleMovements(this, piecesOnBoard);
		}
	}

	public enum ChessColor
	{
		Black,
		Red
	}
}
