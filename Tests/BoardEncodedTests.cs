using Microsoft.VisualStudio.TestTools.UnitTesting;
using 中国象棋;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 中国象棋.Tests
{
	[TestClass()]
	public class BoardEncodedTests
	{
		[TestMethod()]
		public void PrintTest()
		{
			var input =
@"(车)(马)(象)(士)(将)(士)(象)(马)(车)
-＋--＋--＋--＋--＋--＋--＋--＋--＋-
-＋-(炮)-＋--＋--＋--＋--＋-(炮)-＋-
(兵)-＋-(兵)-＋--＋--＋-(兵)-＋-(兵)
-＋--＋--＋--＋--＋--＋--＋--＋--＋-
-＋--＋--＋--＋--＋--＋--＋--＋--＋-
[兵]-＋-[兵]-＋-[兵]-＋-[兵]-＋-[兵]
-＋--＋--＋-[炮]-＋--＋--＋-[炮]-＋-
-＋--＋--＋--＋--＋--＋--＋--＋--＋-
[车][马][象][士][将][士][象][马][车]";

			var board = new Board(input);

			Assert.AreEqual(input, board.Print());
		}
	}
}