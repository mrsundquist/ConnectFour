using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
        }

        Board theGame;

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
            if (!FirstPlayer.IsOn)
            {
                theGame.computeChoice();
                log.Text += ("Computer: Column " + theGame.getLastColumn() + "\n");
            }
        }

        private void hideOptions()
        {
            Options.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Labels.Visibility = Windows.UI.Xaml.Visibility.Visible;
            UserInput.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void unhideOptions()
        {
            Options.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Labels.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            UserInput.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void reset(object sender, TappedRoutedEventArgs e)
        {
            theGame = new Board(!FirstPlayer.IsOn, !ComputerColor.IsOn, (int)Difficulty.Value, YellowSquare);
            log.Text = "";
            FirstPlayer.IsOn = false;
            ComputerColor.IsOn = false;
            Difficulty.Value = 1;
        }

        private async void playerInput(object sender, TappedRoutedEventArgs e)
        {
            int column = Convert.ToInt32(((AppBarButton)sender).Tag); // tag holds column number
            if (theGame.placePlayerChecker(column)) // returns true if checker placed
            {
                log.Text += ("Player:   Column " + theGame.getLastColumn() + "\n");
                
                //check for win
                if (theGame.checkPlayerWin())
                {
                    log.Text += "\n<--PLAYER WINS-->";
                    playerScore.Text = (Convert.ToInt32(playerScore.Text) + 1).ToString();
                    unhideOptions();
                }

                else if (theGame.checkCats())
                {
                    log.Text += "\n<--CATS GAME-->";
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
                        log.Text += "\n<--COMPUTER WINS-->";
                        computerScore.Text = (Convert.ToInt32(computerScore.Text) + 1).ToString();
                        unhideOptions();
                    }

                    else if (theGame.checkCats())
                    {
                        log.Text += "\n\n<--CATS GAME-->";
                        unhideOptions();
                    }
                }
            }
        }

        private void CollectData(object sender, TappedRoutedEventArgs e)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();
            for (int numGames = 0; numGames < 2; numGames++)
            {
                theGame = new Board(true, true, 2, YellowSquare);
                bool computerGoing = true;
                do
                {
                    theGame.computeChoice(computerGoing);
                    computerGoing = !computerGoing;
                } while (!(theGame.checkComputerWin() || theGame.checkComputerWin(false) || theGame.checkCats()));

            }
            //timer.Stop();
            //TimeSpan ts = timer.Elapsed;
            //string elapsedTime = String.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            //log.Text += "RunTime " + elapsedTime + "\n";
        }
    }
}
