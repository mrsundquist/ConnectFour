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
       // static Dictionary<string, stateData> historicalData;
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

        private bool playClose(Checker color)
        {
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
                return false; // can't choose on first turn
            
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
            int tries = 0;
            bool validChoice = false;
            int columnChoice = -1;
            do
            {
                columnChoice = rnd.Next(powerRangeMin, powerRangeMax+1);
                validChoice = placeComputerChecker(columnChoice, color);
            }
            while (tries < 15 && !validChoice); // try 15 times to get a random col in powersection
            return validChoice;
        }
    }
}