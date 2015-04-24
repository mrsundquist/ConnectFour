using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ConnectFour
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            theGame = new Board(true, true, 0, YellowSquare); // load data on first created board
            Options.IsOpen = true;
        }

        Board theGame;
        bool GodPlayer = false;

        private void StartGame(object sender, TappedRoutedEventArgs e)
        {
            this.hideOptions();
            theGame = new Board(!FirstPlayer.IsOn, !ComputerColor.IsOn, (int)Difficulty.Value, YellowSquare);
            log.Text = "";
            if (ComputerColor.IsOn)
            {
                computerScore.BorderBrush = new SolidColorBrush(Colors.DarkRed);
                playerScore.BorderBrush = new SolidColorBrush(Colors.Black);
            }
            else
            {
                computerScore.BorderBrush = new SolidColorBrush(Colors.Black);
                playerScore.BorderBrush = new SolidColorBrush(Colors.DarkRed);
            }
            if (GodMode.IsOn) FirstPlayer.IsOn = true;
            if (!FirstPlayer.IsOn)
            {
                theGame.computeChoice();
                log.Text += ("Computer: Column " + theGame.getLastColumn() + "\n");
            }
        }

        private void hideOptions()
        {
            Options.IsOpen = false;
            Labels.Opacity = 100;
            ChooseA.IsEnabled = true;
            ChooseB.IsEnabled = true;
            ChooseC.IsEnabled = true;
            ChooseD.IsEnabled = true;
            ChooseE.IsEnabled = true;
            ChooseF.IsEnabled = true;
            ChooseG.IsEnabled = true;
        }

        private void unhideOptions()
        {
            Options.IsOpen = true;
            Labels.Opacity = 25;
            ChooseA.IsEnabled = false;
            ChooseB.IsEnabled = false;
            ChooseC.IsEnabled = false;
            ChooseD.IsEnabled = false;
            ChooseE.IsEnabled = false;
            ChooseF.IsEnabled = false;
            ChooseG.IsEnabled = false;
        }

        private void reset(object sender, TappedRoutedEventArgs e)
        {
            theGame = new Board(!FirstPlayer.IsOn, !ComputerColor.IsOn, (int)Difficulty.Value, YellowSquare);
            log.Text = "";
            FirstPlayer.IsOn = false;
            ComputerColor.IsOn = false;
            Difficulty.Value = 5;
        }

        private async void playerInput(object sender, TappedRoutedEventArgs e)
        {
            if (GodMode.IsOn) playerInputGodMode(sender, e);
            else
            {
                int column = Convert.ToInt32(((AppBarButton)sender).Tag); // tag holds column number
                if (theGame.placePlayerChecker(column)) // returns true if checker placed
                {
                    log.Text += ("Player:   Column " + theGame.getLastColumn() + "\n");

                    //check for win
                    if (theGame.checkPlayerWin())
                    {
                        log.Text += "\n<--PLAYER WINS-->\n";
                        playerScore.Text = (Convert.ToInt32(playerScore.Text) + 1).ToString();
                        unhideOptions();
                    }

                    else if (theGame.checkCats())
                    {
                        log.Text += "\n<--CATS GAME-->\n";
                        unhideOptions();
                    }

                    else
                    {
                        //computer goes
                        await Task.Delay(175);
                        theGame.computeChoice();
                        log.Text += ("Computer: Column " + theGame.getLastColumn() + "\n");

                        //check for win
                        if (theGame.checkComputerWin())
                        {
                            log.Text += "\n<--COMPUTER WINS-->\n";
                            computerScore.Text = (Convert.ToInt32(computerScore.Text) + 1).ToString();
                            unhideOptions();
                        }

                        else if (theGame.checkCats())
                        {
                            log.Text += "\n<--CATS GAME-->\n";
                            unhideOptions();
                        }
                    }
                }
            }
        }

        private void playerInputGodMode(object sender, TappedRoutedEventArgs e)
        {
            int column = Convert.ToInt32(((AppBarButton)sender).Tag); // tag holds column number
            GodPlayer = !GodPlayer;

            if (GodPlayer)
            {
                if (theGame.placePlayerChecker(column, false)) // returns true if checker placed
                {
                    log.Text += ("Player:   Column " + theGame.getLastColumn() + "\n");
                }
            }
            else
            {
                if (theGame.placePlayerChecker(column, true)) // returns true if checker placed
                {
                    log.Text += ("Player:   Column " + theGame.getLastColumn() + "\n");
                }
            }

            //check for win
            if (theGame.checkPlayerWin() || theGame.checkComputerWin())
            {
                log.Text += "\n<--PLAYER WINS-->\n";
                playerScore.Text = (Convert.ToInt32(playerScore.Text) + 1).ToString();
                unhideOptions();
            }

            else if (theGame.checkCats())
            {
                log.Text += "\n<--CATS GAME-->\n";
                unhideOptions();
            }
        }

        private void CollectData(object sender, TappedRoutedEventArgs e)
        {
            int playGames = 2000;
            int saveRate = 2000;

            Stopwatch timer = new Stopwatch();
            timer.Start();
            for (int numGames = 1; numGames <= playGames; numGames++)
            {
                bool writeData = false;
                if (numGames % saveRate == 0)
                    writeData = true;
                automate(writeData);
            }  
            timer.Stop();
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            double gpms = ((ts.Hours * 60 * 60 * 1000) + (ts.Minutes * 60 * 1000) + (ts.Seconds * 1000) + (ts.Milliseconds)) / playGames;
            string newLog = "# Games: " + playGames.ToString() + "\n" +
                "RunTime: " + elapsedTime + "\n" +
                "ms/game: " + gpms.ToString() + "\n\n";
            log.Text = newLog + log.Text;
        }

        private void automate(bool writeData = false)
        {
            try
            {
                theGame = new Board(true, true, 4, YellowSquare, writeData);
                bool computerGoing = true;
                do
                {
                    theGame.computeChoice(computerGoing);
                    computerGoing = !computerGoing;
                } while (!(theGame.checkComputerWin() || theGame.checkComputerWin(false) || theGame.checkCats()));
            }
            catch { Exception ex = new Exception(); ex.ToString(); }
        }

        private void UndoLastMove(object sender, TappedRoutedEventArgs e)
        {
            theGame.undo();
            hideOptions();
            log.Text += "\n<--UNDO LAST 2 MOVES-->\n";
        }

        private void Exit(object sender, TappedRoutedEventArgs e)
        {
            App.Current.Exit();
        }

        private void scoreReset(object sender, TappedRoutedEventArgs e)
        {
            computerScore.Text = "0";
            playerScore.Text = "0";
        }
    }
}