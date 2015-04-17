using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;

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
        Random rnd;

        public Board(bool computerFirst, bool computerBlack, int difficulty, StackPanel UIBoard)
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
                historicalData = new Dictionary<string, stateData>();
                Writer.Read(historicalData);
                
                firstTime = false;
            }
           
            this.gameStates = new HashSet<string>();
            
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
            Checker[,] shadowBoard = new Checker[6,7];
            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 7; column++)
                {
                    shadowBoard[row, column] = this.theBoard[row, column];
                }
            }
            return shadowBoard;
        }

        public bool placePlayerChecker(int column)
        {
            if (!this.gameOver)
            {
                lastColumn = column;
                bool validMove = placeChecker(column, playerColor, this.theBoard);
                gameStates.Add(getState(computerColor));
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
                gameStates.Add(getState(color));
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

        private void colorChecker(int row, int column, Checker color)
        {
            SolidColorBrush checkerColor;
            if (color == Checker.red) checkerColor = new SolidColorBrush(Colors.DarkRed);
            else if (color == Checker.black) checkerColor = new SolidColorBrush(Colors.Black);
            else checkerColor = new SolidColorBrush(Color.FromArgb(255,77, 77, 65));

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
            {
                //try for immediate win!
                int winningColumn = connectFour(color);
                if (winningColumn > -1)
                    validChoice = placeComputerChecker(winningColumn, color);
            
                //try for immediate block
                if (!validChoice)
                {
                    int blockingColumn = connectFour(offColor);
                    if (blockingColumn > -1)
                        validChoice = placeComputerChecker(blockingColumn, color);
                }
            }

            if (this.difficulty >= 3)
            {
                //try for historically smart choice
                if (!validChoice)
                {
                    int goodChoice = powerPlay();
                    if (goodChoice > -1)
                        validChoice = placeComputerChecker(goodChoice, color);
                }
            }
            
            //settle for a random choice
            if (!validChoice)
            {
                do { validChoice = placeComputerChecker(chooseRandom(), color); }
                while (!validChoice);
            }
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
            win = (checkHorizontal(color, gameBoard) || checkVertical(color, gameBoard) || 
                checkDiagonal1(color,gameBoard) || checkDiagonal2(color, gameBoard));
            if (!peek)
            {
                this.gameOver = win;
                if (win) endGame(color);
            }
            return win;
        }

        private bool checkHorizontal(Checker color, Checker[,] gameBoard)
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
                        return true;
                    }
                }
            }
            return false;
        }

        private bool checkVertical(Checker color, Checker[,] gameBoard)
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
                        return true;
                    }
                }
            }
            return false;
        }

        private bool checkDiagonal1(Checker color, Checker[,] gameBoard)
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
                        return true;
                    }
                }
            }
            return false;
        }

        private bool checkDiagonal2(Checker color, Checker[,] gameBoard)
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