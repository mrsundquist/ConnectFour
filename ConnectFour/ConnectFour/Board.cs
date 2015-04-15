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

    class Board
    {
        Checker[,] theBoard;
        bool computerFirst;
        Checker computerColor;
        Checker playerColor;
        StackPanel UIBoard;
        bool gameOver;
        int lastColumn;

        public Board(bool computerFirst, bool computerBlack, StackPanel UIBoard)
        {
            this.computerFirst = computerFirst;
            this.computerColor = computerBlack ? Checker.black : Checker.red;
            this.playerColor = !computerBlack ? Checker.black : Checker.red;
            this.UIBoard = UIBoard;
            this.gameOver = false;
            this.lastColumn = 0;
            
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
        
        public bool placePlayerChecker(int column)
        {
            if (!this.gameOver)
            {
                lastColumn = column;
                return placeChecker(column, playerColor);
            }
            else
                return true;
        }

        private bool placeComputerChecker(int column)
        {
            if (!this.gameOver)
            {
                lastColumn = column;
                return placeChecker(column, computerColor);
            }
            else
                return true;
        }

        private bool placeChecker(int column, Checker color)
        {
            for (int row = 5; row >= 0; row--) // 5 is bottom row, 0 is top row
            {
                if (this.theBoard[row, column] == Checker.empty)
                {
                    this.theBoard[row, column] = color;
                    colorChecker(row, column, color);
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

        public void computeChoice()
        {
            bool validChoice = false;
            do
            {
                validChoice = placeComputerChecker(chooseRandom());
            }
            while (!validChoice);
        }

        private int chooseRandom()
        {
            Random rnd = new Random();
            return rnd.Next(0, 7);
        }

        public bool checkPlayerWin()
        {
            return checkWin(playerColor);
        }

        public bool checkComputerWin()
        {
            return checkWin(computerColor);
        }

        private bool checkWin(Checker color)
        {
            this.gameOver = (checkHorizontal(color) || checkVertical(color) || 
                checkDiagonal1(color) || checkDiagonal2(color));
            return this.gameOver;
        }

        private bool checkHorizontal(Checker color)
        {
            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    if (this.theBoard[row, column] == color &&
                        this.theBoard[row, column + 1] == color &&
                        this.theBoard[row, column + 2] == color &&
                        this.theBoard[row, column + 3] == color)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool checkVertical(Checker color)
        {
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 7; column++)
                {
                    if (this.theBoard[row, column] == color &&
                        this.theBoard[row + 1, column] == color &&
                        this.theBoard[row + 2, column] == color &&
                        this.theBoard[row + 3, column] == color)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool checkDiagonal1(Checker color)
        {
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    if (this.theBoard[row, column] == color &&
                        this.theBoard[row + 1, column + 1] == color &&
                        this.theBoard[row + 2, column + 2] == color &&
                        this.theBoard[row + 3, column + 3] == color)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool checkDiagonal2(Checker color)
        {
            for (int row = 3; row < 6; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    if (this.theBoard[row, column] == color &&
                        this.theBoard[row - 1, column + 1] == color &&
                        this.theBoard[row - 2, column + 2] == color &&
                        this.theBoard[row - 3, column + 3] == color)
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
            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 7; column++)
                {
                    if (this.theBoard[row, column] != Checker.empty)
                    {
                        gameOver = false;
                        return false;
                    }
                }
            }
            return true;
        }

        public string getLastColumn()
        {
            switch (this.lastColumn)
            {
                case 0:
                    return "A"; break;
                case 1:
                    return "B"; break;
                case 2:
                    return "C"; break;
                case 3:
                    return "D"; break;
                case 4:
                    return "E"; break;
                case 5:
                    return "F"; break;
                case 6:
                    return "G"; break;
                default:
                    return "";
            }
        }
    }
}