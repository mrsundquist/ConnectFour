using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour
{
    public partial class Board
    {
        private int chooseRandom()
        {
            return rnd.Next(0, 7);
        }

        private int connectFour(Checker color)
        {
            for (int testColumn = 0; testColumn < 7; testColumn++)
            {
                Checker[,] shadowBoard = this.shadow();
                if (placeChecker(testColumn, color, shadowBoard, true)) // if a legal move
                {
                    if (checkWin(color, shadowBoard, true)) // this is a winning move!
                    {
                        return testColumn; // do it for sure!
                    }
                }
            }
            return -1; // no winning move
        }
    }
}
