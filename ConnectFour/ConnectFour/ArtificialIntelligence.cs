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
        bool dataRecord;

        private string getState(Checker color, Checker[,] board, bool incColor = true)
        {
            Checker[,] currentBoard = copyBoard(board); 
            
            string stateString = null;
            for (int column = 0; column < 7; column++)
            {
                for (int row = 5; row > 2; row--)
                {
                    if (currentBoard[row, column] == Checker.empty)
                    {
                        stateString += "^";
                        break;
                    }
                    else if (currentBoard[row, column] == color)
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

            if (incColor)
            {
                stateString += " ";
                stateString += color.ToString();
            }
            return stateString;
        }

        private void endGame(Checker result)
        {
            {
                int score = 0; // computer lose
                if (result == computerColor) score += 2; // computer win
                if (result == Checker.empty) score += 1; // tie

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
                   // historicalData[saveStateString] = record;

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

        private int connectFour(Checker color, Checker[,] board)
        {
            for (int testColumn = 0; testColumn < 7; testColumn++)
            {
                Checker[,] shadowBoard = copyBoard(board);
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

        private int powerPlay(Checker color, Checker opponentColor)
        {
            Dictionary<int, double> columnScores = new Dictionary<int,double>();

            for (int possibleColumn = 0; possibleColumn < 7; possibleColumn++)
            {
                Checker[,] shadowBoard = this.shadow();
                if (!placeChecker(possibleColumn, color, shadowBoard, true)) // place column
                    columnScores[possibleColumn] = 0; // this is not a valid move
                else if ((connectFour(opponentColor, shadowBoard) >= 0)) //opponent could win on this move
                    columnScores[possibleColumn] = 0;
                else
                {
                    string possibleState = getState(color, shadowBoard, false);
                    //stateData historicalRecord = historicalData[possibleState];
                    stateData historicalRecord;
                    if (historicalData.TryGetValue(possibleState, out historicalRecord))
                    {
                        double score = (double)historicalRecord.numWins / (double)historicalRecord.numPlays;
                        columnScores[possibleColumn] = score;
                    }
                    else
                    {
                        columnScores[possibleColumn] = 1; // no record
                    }
                }
            }

            int returnColumn = -1;
            double highScore = 0; // 1.0 is 50% win ratio
            
            int col = 0;
            for (int i = 0; i < 7; i++)
            {
                if (columnScores[col] >= highScore)
                {
                    highScore = columnScores[col];
                    returnColumn = col;
                }
                col = col + (int)Math.Pow(-1, i) * (6 - i); // col goes 0, 6, 1, 5, 2, 4, 3 (inside cols have priority in ties)
            }
            
            return returnColumn;
        }

        private int playClose(Checker color)
        {
            if (rnd.Next(0, 2) == 0) return -1; //half the time don't do this b/c it can get locked in
            int[] columnCounts = new int[7];
            for (int col = 0; col < 7; col++)
            {
                for (int row = 0; row < 6; row++)
                {
                    if (this.theBoard[row, col] == color)
                        columnCounts[col]++;
                }
            }

            int[] powerSections = new int[5]; // sec 0 represent cols A-C, 1 is B-D, etc
            for (int sec = 0; sec < 5; sec++)
            {
                for (int offset = 0; offset < 3; offset++)
                {
                    powerSections[sec] += columnCounts[sec+offset];
                }
            }
            
            if (powerSections.Max() == 0)
                return -1; // can't choose on first turn
            
            //choose power column, starting from middle
            int powerColumn = 3;
            if (powerSections[2] == powerSections.Max())
                powerColumn = 3;
            else if (powerSections[1] == powerSections.Max())
                powerColumn = 2;
            else if (powerSections[3] == powerSections.Max())
                powerColumn = 4;
            else if (powerSections[0] == powerSections.Max())
                powerColumn = 1;
            else if (powerSections[4] == powerSections.Max())
                powerColumn = 5;

            int powerRangeMin = (powerColumn - 2 < 0) ? (0) : (powerColumn - 2);
            int powerRangeMax = (powerColumn + 2 > 6) ? (6) : (powerColumn + 2);
            Checker[,] shadowBoard = this.shadow();
            int tries = 0;
            bool validChoice = false;
            int columnChoice = -1;
            do
            {
                columnChoice = rnd.Next(powerRangeMin, powerRangeMax+1);
                validChoice = placeChecker(columnChoice, color, shadowBoard, true);
            }
            while (tries++ < 20 && !validChoice); // try 20 times to get a random col in powersection
            return columnChoice;
        }

        private int noSetup(Checker color, Checker opponentColor)
        {
            int tries = 0;
            bool validChoice = false;
            int columnChoice = -1;
            do
            {
                Checker[,] shadowBoard = this.shadow();
                int possibleColumn = playClose(color);
                if (possibleColumn < 0) break;
                if (!placeChecker(possibleColumn, color, shadowBoard, true)) break;// try it out
                if (connectFour(opponentColor, shadowBoard) < 0) //opponent cannont win on this move
                {
                    columnChoice = possibleColumn;
                    validChoice = true;
                }
            }
            while (tries++ < 25 && !validChoice); //try 25 times to get a safe, close qualified column

            tries = 0;
            while (tries++ < 35 && !validChoice) //try 35 times to get a safe, random column
            {
                Checker[,] shadowBoard = this.shadow();
                int possibleColumn = chooseRandom();
                if (!placeChecker(possibleColumn, color, shadowBoard, true)) break;//try it out
                if (connectFour(opponentColor, shadowBoard) < 0) //opponent cannot win on this move
                {
                    columnChoice = possibleColumn;
                    validChoice = true;
                }
            }
            return columnChoice;
        }
    }
}