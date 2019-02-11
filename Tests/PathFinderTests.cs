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
	public class PathFinderTests
	{
		[TestMethod()]
		public void FindMovesCountToJiangTest()
		{
			var board = new Board(
@"[车][马][象][士][将][士][象][马][车]
-＋--＋--＋--＋--＋--＋--＋--＋--＋-
-＋-[炮]-＋--＋--＋--＋--＋-[炮]-＋-
[兵]-＋-[兵]-＋-(炮)-＋-[兵]-＋-[兵]
-＋--＋--＋--＋--＋--＋--＋--＋--＋-
-＋--＋--＋--＋--＋--＋--＋--＋--＋-
(兵)-＋-(兵)-＋-(兵)-＋-(兵)-＋-(兵)
-＋--＋--＋--＋--＋--＋--＋-(炮)-＋-
-＋--＋--＋--＋--＋--＋--＋--＋--＋-
(车)(马)(象)(士)(将)(士)(象)(马)(车)");
			Assert.AreEqual(3, PathFinder.FindMovesCountToJiang(board[7, 7], new IntPoint(7, 7), board));
		}
	}
}