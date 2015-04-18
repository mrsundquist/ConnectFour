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
        static List<string> dataList;


        private string getState(Checker color)
        {
            string stateString = null;
            for (int column = 0; column < 7; column++)
            {
                for (int row = 5; row > 2; row--)
                {
                    if (this.theBoard[row, column] == Checker.empty)
                    {
                        stateString += "^";
                        break;
                    }
                    else if (this.theBoard[row, column] == color)
                    {
                        stateString += "s"; // 
                    }
                    else
                    {
                        stateString += "o";
                    }
                    if (row == 3)
                    {
                        stateString += "^";
                    }
                }

            }
            stateString += " ";
            stateString += color.ToString();
            return stateString;
        }

        private void endGame(Checker result)
        {
            //lock (this)
            {
                int score = 0; // computer lose
                if (result == computerColor) score += 2; // computer win
                if (result == Checker.empty) score += 1; // tie

                string[] data = new string[gameStates.Count];

                foreach (string state in gameStates)
                {
                    stateData record = new stateData();
                    record.numPlays = 1;
                    string[] dataParts = new string[2];
                    dataParts = state.Split(' ');
                    string saveStateString = dataParts[0];
                    string colorString = dataParts[1];

                    if (result.ToString() == colorString) record.numWins += score;
                    else record.numWins += (2 - score); // 0 if comp won, 1 if tie, 2 if comp lost
                    historicalData[saveStateString] = record;

                    string recordString = null;
                    recordString = saveStateString + " " + record.numPlays.ToString() + " " + record.numWins.ToString();
                    Board.dataList.Add(recordString);
                }

                //output to file
                if (this.dataRecord)
                {
                    Writer.Write(dataList.ToArray());
                    Board.dataList.Clear();
                }
            }
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