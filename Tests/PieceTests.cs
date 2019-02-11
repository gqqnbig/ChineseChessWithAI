using Microsoft.VisualStudio.TestTools.UnitTesting;
using 中国象棋;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntPoint = System.Drawing.Point;

namespace 中国象棋.Tests
{
	[TestClass()]
	public class PieceTests
	{
		[TestMethod()]
		public void PieceTest()
		{
			Assert.Fail();
		}

		[TestMethod()]
		public void Get炮MovementsTest()
		{

			var board = new Board(
@"[车][马][象][士][将][士][象][马][车]
-＋--＋--＋--＋--＋--＋--＋--＋--＋-
-＋-[炮]-＋--＋--＋--＋--＋-[炮]-＋-
[兵]-＋-[兵]-＋-(炮)-＋-[兵]-＋-[兵]
-＋--＋--＋--＋--＋--＋--＋--＋--＋-
-＋--＋--＋--＋--＋--＋--＋--＋--＋-
(兵)-＋-(兵)-＋-(兵)-＋-(兵)-＋-(兵)
-＋--＋--＋--＋-(炮)-＋--＋--＋--＋-
-＋--＋--＋--＋--＋--＋--＋--＋--＋-
(车)(马)(象)(士)(将)(士)(象)(马)(车)");

			Assert.AreEqual(9, board[7, 4].GetPossibleMovements(new IntPoint(4, 7), board).Count());
		}

		[TestMethod()]
		public void ToStringTest()
		{
			Assert.Fail();
		}
	}
}