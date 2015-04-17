using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFour
{
    struct stateData { public int numPlays; public int numWins;}


    public partial class Board
    {
        static bool firstTime = true;
        static Dictionary<string, stateData> historicalData;
        HashSet<string> gameStates;
        Random rnd;

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
            int score = 0; // computer lose
            if (result == computerColor) score += 2; // computer win
            if (result == Checker.empty) score += 1; // tie

            string[] data = new string[gameStates.Count];

            List<string> dataList = new List<string>();

            bool blackRecord = true; // computer record are black, comp2 are red
                                    // need to record every other as a win if won, etc.
            foreach (string state in gameStates)
            {
                stateData record = new stateData();
                //if (historicalData.ContainsKey(state))
                //{
                //    record.numPlays = historicalData[state].numPlays;
                //    record.numWins = historicalData[state].numWins;
                //}
                record.numPlays = 1;
                if (blackRecord) record.numWins += score;
                else record.numWins += (2 - score); // 0 if comp won, 1 if tie, 2 if comp lost [for red]
                historicalData[state] = record;
                blackRecord = !blackRecord;

                string recordString = null;
                recordString = state + " " + record.numPlays.ToString() + " " + record.numWins.ToString();
                dataList.Add(recordString);
            }

            //output to file
            Write(dataList.ToArray());
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
