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

		public Piece(int id, string name, Func<Piece, IntPoint, Board, IEnumerable<IntPoint>> getPossibleMovements)
		{
			this.getPossibleMovements = getPossibleMovements;
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
	}

	public enum ChessColor
	{
		Black,
		Red
	}
}
