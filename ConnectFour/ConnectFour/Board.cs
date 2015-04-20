using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace ConnectFour
{
    enum Checker { empty, black, red };

    public partial class Board
    {
        Checker[,] theBoard;
        bool computerFirst;
        Checker computerColor;
        Checker playerColor;
        int difficulty;
        StackPanel UIBoard;
        bool gameOver;
        int lastColumn;
        bool dataRecord;

       
        public Board(bool computerFirst, bool computerBlack, int difficulty, StackPanel UIBoard, bool dataRecord = false)
        {
            this.computerFirst = computerFirst;
            this.computerColor = computerBlack ? Checker.black : Checker.red;
            this.playerColor = !computerBlack ? Checker.black : Checker.red;
            this.difficulty = difficulty;
            this.UIBoard = UIBoard;
            this.gameOver = false;
            this.lastColumn = 0;
            this.rnd = new Random();
            this.gameStates = new HashSet<string>();
            if (firstTime)
            {
                //historicalData = new Dictionary<string, stateData>();
                //Writer.Read(historicalData);
                Board.dataList = new List<string>();
                firstTime = false;
            }
            this.dataRecord = dataRecord;

            this.theBoard = new Checker[6, 7];
            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 7; column++)
                {
                    this.theBoard[row, column] = Checker.empty;
                    colorChecker(row, column, Checker.empty);
                }
            }
        }

        private Checker[,] shadow() // returns copy of the logical board, not UI board
        {
            Checker[,] shadowBoard = new Checker[6, 7];
            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 7; column++)
                {
                    shadowBoard[row, column] = this.theBoard[row, column];
                }
            }
            return shadowBoard;
        }

        public bool placePlayerChecker(int column, bool godMode = false)
        {
            if (!this.gameOver)
            {
                Checker color = this.playerColor;
                if (godMode) color = this.computerColor;
               
                lastColumn = column;
                bool validMove = placeChecker(column, color, this.theBoard);
                gameStates.Add(getState(color));
                return validMove;
            }
            else
                return true;
        }

        private bool placeComputerChecker(int column, Checker color)
        {
            if (!this.gameOver)
            {
                lastColumn = column;
                bool validMove = placeChecker(column, color, this.theBoard);
                gameStates.Add(getState(color)); // add historical state data
                return validMove;
            }
            else
                return true;
        }

        private bool placeChecker(int column, Checker color, Checker[,] gameBoard, bool peek = false)
        {
            for (int row = 5; row >= 0; row--) // 5 is bottom row, 0 is top row
            {
                if (gameBoard[row, column] == Checker.empty)
                {
                    gameBoard[row, column] = color;
                    if (!peek) colorChecker(row, column, color);
                    return true;
                }
            }
            return false;
        }

        private void colorChecker(int row, int column, Checker color, bool winning = false)
        {
            SolidColorBrush checkerColor;
            if (!winning)
            {
                if (color == Checker.red) checkerColor = new SolidColorBrush(Colors.DarkRed);
                else if (color == Checker.black) checkerColor = new SolidColorBrush(Colors.Black);
                else checkerColor = new SolidColorBrush(Color.FromArgb(255, 77, 77, 65));
            }
            else
            {
                if (color == Checker.red) checkerColor = new SolidColorBrush(Colors.IndianRed);
                else checkerColor = new SolidColorBrush(Colors.DimGray);
            }

            //find the current checker
            UIElementCollection rows = UIBoard.Children;
            StackPanel thisRow = (StackPanel)rows[row];
            UIElementCollection columns = thisRow.Children;
            Border thisColumn = (Border)columns[column];
            Ellipse thisChecker = (Ellipse)thisColumn.Child;
            //paint it color
            thisChecker.Fill = checkerColor;
        }

        public void computeChoice(bool computerPlaying = true)
        {
            Checker color = computerColor;
            Checker offColor = playerColor;
            if (!computerPlaying)
            {
                color = playerColor;
                offColor = computerColor;
            }

            bool validChoice = false;

            if (this.difficulty >= 2)
                //try for immediate win!
                validChoice = attemptConnectFour(color);

            if (!validChoice && this.difficulty >= 3)
                //try for immediate block
                validChoice = attemptBlockFour(offColor, color);

            if (!validChoice && this.difficulty >= 5)
                //choose a powerful position based on history (and try for no set up)
                validChoice = attemptPowerPlay(color);

            if (!validChoice && this.difficulty >= 4)
                //prevent setting other player up to connect four
                validChoice = attemptNoSetup(color);

            if (!validChoice && this.difficulty >= 1)
                //choose a position not too far from current checkers
                validChoice = attemptPlayClose(color);

            if (!validChoice)
            {
                //choose a random column
                attemptChooseRandom(color);
            }
        }

        private bool attemptConnectFour(Checker color)
        {
            int winningColumn = connectFour(color);
            if (winningColumn > -1)
                return placeComputerChecker(winningColumn, color);
            return false;
        }

        private bool attemptBlockFour(Checker offColor, Checker color)
        {
            int blockingColumn = connectFour(offColor);
            if (blockingColumn > -1)
                return placeComputerChecker(blockingColumn, color);
            return false;
        }

        private bool attemptNoSetup(Checker color)
        {
            //not implemented

            return false;
        }

        private bool attemptPowerPlay(Checker color)
        {
            //not implemented

            //please implement no set-up into powerplay
            //e.g., choose top state (if over .50), if set up, choose next,
            //continue for each 7 columns until under .50 or no set up

            //int goodChoice = powerPlay();
            //if (goodChoice > -1)
            //    return placeComputerChecker(goodChoice, color);
            return false;
        }

        private bool attemptPlayClose(Checker color)
        {
            int closeColumn = playClose(color);
            if (closeColumn > -1)
                return placeComputerChecker(closeColumn, color);
            return false;
        }

        private bool attemptChooseRandom(Checker color)
        {
            bool validChoice = false;
            do
            {
                validChoice = placeComputerChecker(chooseRandom(), color);
            }
            while (!validChoice);
            return validChoice;
        }
        
        public bool checkPlayerWin()
        {
            return checkWin(playerColor, this.theBoard);
        }

        public bool checkComputerWin(bool computerPlaying = true)
        {
            Checker color = computerColor;
            Checker offColor = playerColor;
            if (!computerPlaying)
            {
                color = playerColor;
                offColor = computerColor;
            }
            return checkWin(color, this.theBoard);
        }

        private bool checkWin(Checker color, Checker[,] gameBoard, bool peek = false)
        {
            bool win = false;
            win = (checkHorizontal(color, gameBoard, peek) || checkVertical(color, gameBoard, peek) ||
                checkDiagonal1(color, gameBoard, peek) || checkDiagonal2(color, gameBoard, peek));
            if (!peek)
            {
                this.gameOver = win;
                if (win) endGame(color);
            }
            return win;
        }

        private bool checkHorizontal(Checker color, Checker[,] gameBoard, bool peek = false)
        {
            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    if (gameBoard[row, column] == color &&
                        gameBoard[row, column + 1] == color &&
                        gameBoard[row, column + 2] == color &&
                        gameBoard[row, column + 3] == color)
                    {
                        if (!peek)
                            for (int c = 0; c < 4; c++)
                                colorChecker(row, column + c, color, true);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool checkVertical(Checker color, Checker[,] gameBoard, bool peek = false)
        {
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 7; column++)
                {
                    if (gameBoard[row, column] == color &&
                        gameBoard[row + 1, column] == color &&
                        gameBoard[row + 2, column] == color &&
                        gameBoard[row + 3, column] == color)
                    {
                        if (!peek)
                        {
                            for (int r = 0; r < 4; r++)
                                colorChecker(row + r, column, color, true);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private bool checkDiagonal1(Checker color, Checker[,] gameBoard, bool peek = false)
        {
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    if (gameBoard[row, column] == color &&
                        gameBoard[row + 1, column + 1] == color &&
                        gameBoard[row + 2, column + 2] == color &&
                        gameBoard[row + 3, column + 3] == color)
                    {
                        if (!peek)
                        {
                            for (int r = 0; r < 4; r++)
                                colorChecker(row + r, column + r, color, true);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private bool checkDiagonal2(Checker color, Checker[,] gameBoard, bool peek = false)
        {
            for (int row = 3; row < 6; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    if (gameBoard[row, column] == color &&
                        gameBoard[row - 1, column + 1] == color &&
                        gameBoard[row - 2, column + 2] == color &&
                        gameBoard[row - 3, column + 3] == color)
                    {
                        if (!peek)
                        {
                            for (int r = 0; r < 4; r++)
                                colorChecker(row - r, column + r, color, true);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public bool checkCats()
        {
            gameOver = true;
            int row = 0;
            for (int column = 0; column < 7; column++)
            {
                if (this.theBoard[row, column] == Checker.empty)
                {
                    gameOver = false;
                    return false;
                }
            }
            endGame(Checker.empty);
            return true;
        }

        public string getLastColumn()
        {
            switch (this.lastColumn)
            {
                case 0:
                    return "A";
                case 1:
                    return "B";
                case 2:
                    return "C";
                case 3:
                    return "D";
                case 4:
                    return "E";
                case 5:
                    return "F";
                case 6:
                    return "G";
                default:
                    return "";
            }
        }

    }
}