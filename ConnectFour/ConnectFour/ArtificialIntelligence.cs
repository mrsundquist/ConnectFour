using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour
{
    struct stateData { public int numPlays; public float numWins; public float winPct; }


    public partial class Board
    {
        static bool firstTime = true;
        static Dictionary<string, stateData> historicalData;
        HashSet<string> gameStates;

        private string getState(Checker computerColor)
        {
            string stateString = null;
            for (int column = 0; column < 7; column++)
            {
                for (int row = 5; row > 2; row--)
                {
                    if (this.theBoard[row, column] == Checker.empty)
                    {
                        stateString += "x";
                        break;
                    }
                    else if (this.theBoard[row, column] == computerColor)
                    {
                        stateString += "C";
                    }
                    else
                    {
                        stateString += "P";
                    }
                    if (row == 3)
                    {
                        stateString += "x";
                    }
                }
            }
            return stateString;
        }

        private void endGame(Checker result)
        {
            float score = 0; // computer lose
            if (result == computerColor) score += 1; // computer win
            if (result == Checker.empty) score += 0.5F; // tie

            string[] data = new string[gameStates.Count];

            List<string> dataList = new List<string>();

            foreach (string state in gameStates)
            {
                stateData record = new stateData();
                if (historicalData.ContainsKey(state))
                {
                    record.numPlays = historicalData[state].numPlays;
                    record.numWins = historicalData[state].numWins;
                }
                record.numPlays += 1;
                record.numWins += score;
                record.winPct = record.numWins / (record.numPlays);
                historicalData[state] = record;

                string recordString = null;
                recordString = state + " " + record.numPlays.ToString() + " " 
                    + record.numWins.ToString() + " " + record.winPct.ToString();
                dataList.Add(recordString);
            }

            //output to CSV file
            Writer.Write(dataList.ToArray());
        }

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

        private int powerPlay()
        {
            
            return chooseRandom(); // remove after implementation
        }
    }
}
