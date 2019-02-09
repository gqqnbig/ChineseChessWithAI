using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntPoint = System.Drawing.Point;

namespace 中国象棋
{
	public class Piece
	{
		public string Name { get; set; }

		public ChessColor Color { get; set; }

		public bool CanCrossRiver { get; set; } = true;

		private Func<Piece, IntPoint, Board, IEnumerable<IntPoint>> getPossibleMovements;

		public Piece(Func<Piece, IntPoint, Board, IEnumerable<IntPoint>> getPossibleMovements)
		{
			this.getPossibleMovements = getPossibleMovements;
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
	}

	public enum ChessColor
	{
		Black,
		Red
	}
}
